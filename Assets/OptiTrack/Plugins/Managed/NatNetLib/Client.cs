//======================================================================================================
// Copyright 2016, NaturalPoint Inc.
//======================================================================================================

using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;


namespace NaturalPoint.NatNetLib
{
    [Serializable]
    public class NatNetException : System.Exception
    {
        public NatNetException()
        {
        }


        public NatNetException( string message )
            : base( message )
        {
        }


        public NatNetException( string message, Exception inner )
            : base( message, inner )
        {
        }


        internal static void ThrowIfNotOK( NatNetError result, string message )
        {
            if ( result != NatNetError.NatNetError_OK )
            {
                throw new NatNetException( message + " (" + result.ToString() + ")" );
            }
        }
    }


    internal class NatNetClient : IDisposable
    {
        public class DataDescriptions
        {
            public List<sMarkerSetDescription> MarkerSetDescriptions;
            public List<sRigidBodyDescription> RigidBodyDescriptions;
            public List<sSkeletonDescription> SkeletonDescriptions;
            public List<sForcePlateDescription> ForcePlateDescriptions;
        }


        public static Version NatNetLibVersion
        {
            get
            {
                Byte[] natNetLibVersion = new Byte[4];
                NatNetLib.NativeMethods.NatNet_GetVersion( natNetLibVersion );
                return new Version( natNetLibVersion[0], natNetLibVersion[1], natNetLibVersion[2], natNetLibVersion[3] );
            }
        }

        public bool Connected { get; private set; }

        /// <summary>
        /// This event is raised when a new frame is received via the network.
        /// IMPORTANT: This executes (via reverse P/Invoke) in the context of
        /// the NatNetLib network service thread.
        /// </summary>
        /// <remarks>
        /// NB: The sFrameOfMocapData native structure is large and expensive
        /// to marshal. In particular, each invocation allocates ~200 KB.
        /// </remarks>
        public event EventHandler<NativeFrameReceivedEventArgs> NativeFrameReceived;

        public class NativeFrameReceivedEventArgs : EventArgs
        {
            private sFrameOfMocapData? m_marshaledFrame;
            private IntPtr m_nativeFrame;

            public IntPtr NativeFramePointer {
                get
                {
                    return m_nativeFrame;
                }

                set
                {
                    // Invalidate lazily-evaluated cached marshaled frame.
                    m_marshaledFrame = null;

                    m_nativeFrame = value;
                }
            }

            public sFrameOfMocapData MarshaledFrame {
                get {
                    if ( m_marshaledFrame.HasValue == false )
                    {
                        m_marshaledFrame = (sFrameOfMocapData)Marshal.PtrToStructure( NativeFramePointer, typeof( sFrameOfMocapData ) );
                    }

                    return m_marshaledFrame.Value;
                }
            }
        }


        #region Private fields
        private IntPtr m_clientHandle = IntPtr.Zero;
        private bool m_disposed = false;
        private NatNetFrameReceivedCallback m_nativeFrameReceivedHandler;
        private NativeFrameReceivedEventArgs m_nativeFrameReceivedEventArgs = new NativeFrameReceivedEventArgs();
        #endregion Private fields


        public NatNetClient( NatNetConnectionType connectionType )
        {
            NatNetError retval = NatNetLib.NativeMethods.NatNet_Client_Create( out m_clientHandle, connectionType );
            NatNetException.ThrowIfNotOK( retval, "NatNet_Client_Create failed." );

            if ( m_clientHandle == IntPtr.Zero )
            {
                throw new NatNetException( "NatNet_Client_Create returned null handle." );
            }

            // This ensures the reverse P/Invoke delegate passed to the native code stays alive.
            m_nativeFrameReceivedHandler = FrameReceivedNativeThunk;

            retval = NatNetLib.NativeMethods.NatNet_Client_SetFrameReceivedCallback( m_clientHandle, m_nativeFrameReceivedHandler );
            NatNetException.ThrowIfNotOK( retval, "NatNet_Client_SetFrameReceivedCallback failed." );
        }


        public void Connect( IPAddress localAddress,
                             IPAddress serverAddress,
                             UInt16 serverCommandPort = NatNetConstants.DefaultCommandPort,
                             UInt16 serverDataPort = NatNetConstants.DefaultDataPort,
                             IPAddress multicastAddress = null )
        {
            ThrowIfDisposed();

            sNatNetClientConnectParams initParams = new sNatNetClientConnectParams {
                LocalAddress = localAddress.ToString(),
                ServerAddress = serverAddress.ToString(),
                ServerCommandPort = serverCommandPort,
                ServerDataPort = serverDataPort,
                MulticastAddress = multicastAddress == null ? null : multicastAddress.ToString()
            };

            NatNetError retval = NatNetLib.NativeMethods.NatNet_Client_Connect( m_clientHandle, ref initParams );
            NatNetException.ThrowIfNotOK( retval, "NatNet_Client_Connect failed." );

            Connected = true;
        }


        public void Disconnect()
        {
            ThrowIfDisposed();

            if ( Connected )
            {
                NatNetError retval = NatNetLib.NativeMethods.NatNet_Client_Disconnect( m_clientHandle );
                NatNetException.ThrowIfNotOK( retval, "NatNet_Client_Disconnect failed." );

                Connected = false;
            }
        }


        public DataDescriptions GetDataDescriptions()
        {
            ThrowIfDisposed();

            IntPtr pDataDescriptions;
            NatNetError retval = NatNetLib.NativeMethods.NatNet_Client_GetDataDescriptionList( m_clientHandle, out pDataDescriptions );
            NatNetException.ThrowIfNotOK( retval, "NatNet_Client_GetDataDescriptions failed." );

            sDataDescriptions dataDescriptions = (sDataDescriptions)Marshal.PtrToStructure( pDataDescriptions, typeof( sDataDescriptions ) );

            // Do a quick first pass to determine the required capacity for the returned lists.
            Int32 numMarkerSetDescs = 0;
            Int32 numRigidBodyDescs = 0;
            Int32 numSkeletonDescs = 0;
            Int32 numForcePlateDescs = 0;

            for ( Int32 i = 0; i < dataDescriptions.DataDescriptionCount; ++i )
            {
                sDataDescription desc = dataDescriptions.DataDescriptions[i];

                switch ( desc.DescriptionType )
                {
                    case (Int32)NatNetDataDescriptionType.NatNetDataDescriptionType_MarkerSet:
                        ++numMarkerSetDescs;
                        break;
                    case (Int32)NatNetDataDescriptionType.NatNetDataDescriptionType_RigidBody:
                        ++numRigidBodyDescs;
                        break;
                    case (Int32)NatNetDataDescriptionType.NatNetDataDescriptionType_Skeleton:
                        ++numSkeletonDescs;
                        break;
                    case (Int32)NatNetDataDescriptionType.NatNetDataDescriptionType_ForcePlate:
                        ++numForcePlateDescs;
                        break;
                }
            }

            // Allocate the lists to be returned based on our counts.
            DataDescriptions retDescriptions = new DataDescriptions {
                MarkerSetDescriptions = new List<sMarkerSetDescription>( numMarkerSetDescs ),
                RigidBodyDescriptions = new List<sRigidBodyDescription>( numRigidBodyDescs ),
                SkeletonDescriptions = new List<sSkeletonDescription>( numSkeletonDescs ),
                ForcePlateDescriptions = new List<sForcePlateDescription>( numForcePlateDescs ),
            };

            // Now populate the lists.
            for ( Int32 i = 0; i < dataDescriptions.DataDescriptionCount; ++i )
            {
                sDataDescription desc = dataDescriptions.DataDescriptions[i];

                switch ( desc.DescriptionType )
                {
                    case (Int32)NatNetDataDescriptionType.NatNetDataDescriptionType_MarkerSet:
                        sMarkerSetDescription markerSetDesc = (sMarkerSetDescription)Marshal.PtrToStructure( desc.Description, typeof( sMarkerSetDescription ) );
                        retDescriptions.MarkerSetDescriptions.Add( markerSetDesc );
                        break;
                    case (Int32)NatNetDataDescriptionType.NatNetDataDescriptionType_RigidBody:
                        sRigidBodyDescription rigidBodyDesc = (sRigidBodyDescription)Marshal.PtrToStructure( desc.Description, typeof( sRigidBodyDescription ) );
                        retDescriptions.RigidBodyDescriptions.Add( rigidBodyDesc );
                        break;
                    case (Int32)NatNetDataDescriptionType.NatNetDataDescriptionType_Skeleton:
                        sSkeletonDescription skeletonDesc = (sSkeletonDescription)Marshal.PtrToStructure( desc.Description, typeof( sSkeletonDescription ) );
                        retDescriptions.SkeletonDescriptions.Add( skeletonDesc );
                        break;
                    case (Int32)NatNetDataDescriptionType.NatNetDataDescriptionType_ForcePlate:
                        sForcePlateDescription forcePlateDesc = (sForcePlateDescription)Marshal.PtrToStructure( desc.Description, typeof( sForcePlateDescription ) );
                        retDescriptions.ForcePlateDescriptions.Add( forcePlateDesc );
                        break;
                }
            }

            return retDescriptions;
        }


        /// <summary>
        /// Reverse P/Invoke delegate passed to <see cref="NativeMethods.NatNet_Client_SetFrameReceivedCallback"/>.
        /// </summary>
        /// <param name="pFrameOfMocapData">Native pointer to a <see cref="sFrameOfMocapData"/> struct.</param>
        /// <param name="pUserData">Native user-provided callback context pointer (void*).</param>
        private void FrameReceivedNativeThunk( IntPtr pFrameOfMocapData, IntPtr pUserData )
        {
            try
            {
                ThrowIfDisposed();

                if ( NativeFrameReceived != null )
                {
                    m_nativeFrameReceivedEventArgs.NativeFramePointer = pFrameOfMocapData;
                    NativeFrameReceived( this, m_nativeFrameReceivedEventArgs );
                }
            }
            catch ( Exception ex )
            {
                // It's important that we consume any exceptions here, since an exception thrown
                // from this reverse P/Invoke delegate would transform into an SEH exception once
                // propagated across the native code boundary, and NatNetLib would blow up.
                System.Diagnostics.Debug.WriteLine( "ERROR - Exception occurred in FrameReceivedNativeThunk: " + ex.ToString() );
            }
        }


        #region Dispose pattern
        ~NatNetClient()
        {
            Dispose( false );
        }


        /// <summary>Implements IDisposable.</summary>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }


        /// <summary>
        /// Called by both the <see cref="IDisposable.Dispose()"/> override,
        /// as well as the finalizer, to do the actual cleanup work.
        /// </summary>
        /// <param name="disposing">
        /// True if <see cref="Dispose()"/> was called explicitly. False if
        /// running as part of the finalizer. If false, do not attempt to
        /// reference other managed objects, since they may have already been
        /// finalized themselves.
        /// </param>
        protected virtual void Dispose( bool disposing )
        {
            if ( m_disposed )
                return;

            // Disconnect if necessary.
            if ( Connected )
            {
                NatNetError disconnectResult = NatNetLib.NativeMethods.NatNet_Client_Disconnect( m_clientHandle );

                if ( disconnectResult != NatNetError.NatNetError_OK )
                {
                    System.Diagnostics.Debug.WriteLine( "NatNet_Client_Disconnect returned " + disconnectResult.ToString() + "." );
                }

                Connected = false;
            }

            // Now destroy the native client.
            NatNetError destroyResult = NatNetLib.NativeMethods.NatNet_Client_Destroy( m_clientHandle );

            if ( destroyResult != NatNetError.NatNetError_OK )
            {
                System.Diagnostics.Debug.WriteLine( "NatNet_Client_Destroy returned " + destroyResult.ToString() + "." );
            }

            m_clientHandle = IntPtr.Zero;

            m_disposed = true;
        }


        private void ThrowIfDisposed()
        {
            if ( m_disposed )
            {
                throw new ObjectDisposedException( GetType().FullName );
            }
        }
        #endregion Dispose pattern
    }
}
