//======================================================================================================
// Copyright 2016, NaturalPoint Inc.
//======================================================================================================

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using NaturalPoint.NatNetLib;


// NOTE: These native structure representations are in some places incomplete
// (e.g. IntPtr to unspecified data) or untested (e.g. the force plate data
// types have not received any testing). See NatNetTypes.h for more info.
//
// This code may be unstable and is subject to change.

namespace NaturalPoint.NatNetLib
{
    internal static class NatNetConstants
    {
        public const string NatNetLibDllBaseName = "NatNetLib";
        public const CallingConvention NatNetLibCallingConvention = CallingConvention.Cdecl;

        public const int MaxNameLength = 256;
        public const int MaxModels = 200;
        public const int MaxRigidBodies = 1000;
        public const int MaxSkeletons = 100;
        public const int MaxSkeletonRigidBodies = 200;
        public const int MaxLabeledMarkers = 1000;
        public const int MaxForcePlates = 8;
        public const int MaxAnalogChannels = 32;
        public const int MaxAnalogSubframes = 30;

        public const UInt16 DefaultCommandPort = 1510;
        public const UInt16 DefaultDataPort = 1511;
    }


    #region Enumerations
    internal enum NatNetError
    {
        NatNetError_OK = 0,
        NatNetError_Internal,
        NatNetError_External,
        NatNetError_Network,
        NatNetError_Other,
        NatNetError_InvalidArgument,
        NatNetError_InvalidOperation
    }


    internal enum NatNetConnectionType
    {
        NatNetConnectionType_Multicast = 0,
        NatNetConnectionType_Unicast
    }


    internal enum NatNetDataDescriptionType
    {
        NatNetDataDescriptionType_MarkerSet = 0,
        NatNetDataDescriptionType_RigidBody,
        NatNetDataDescriptionType_Skeleton,
        NatNetDataDescriptionType_ForcePlate
    };
    #endregion Enumerations


    #region Definition types
    [StructLayout( LayoutKind.Sequential )]
    internal struct sDataDescriptions
    {
        public Int32 DataDescriptionCount;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = NatNetConstants.MaxModels )]
        public sDataDescription[] DataDescriptions;
    }


    [StructLayout( LayoutKind.Sequential )]
    internal struct sDataDescription
    {
        public Int32 DescriptionType;
        public IntPtr Description;
    }


    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
    internal struct sMarkerSetDescription
    {
        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = NatNetConstants.MaxNameLength )]
        public string Name;

        public Int32 MarkerCount;
        public IntPtr MarkerNames; // char**, "array of marker names"
    }


    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
    internal struct sRigidBodyDescription
    {
        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = NatNetConstants.MaxNameLength )]
        public string Name;

        public Int32 Id;
        public Int32 ParentId;
        public float OffsetX;
        public float OffsetY;
        public float OffsetZ;
    }


    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
    internal struct sSkeletonDescription
    {
        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = NatNetConstants.MaxNameLength )]
        public string Name;

        public Int32 Id;
        public Int32 RigidBodyCount;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = NatNetConstants.MaxSkeletonRigidBodies )]
        public sRigidBodyDescription[] RigidBodies;
    }


    // Marshalling helper for char[][] sForcePlateDescription.ChannelNames.
    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
    internal struct sForcePlateChannelName
    {
        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = NatNetConstants.MaxNameLength )]
        private string Value;

        public static implicit operator string( sForcePlateChannelName source )
        {
            return source.Value;
        }

        public static implicit operator sForcePlateChannelName( string source )
        {
            // Note that longer strings would be silently truncated if we didn't explicitly check this here.
            if ( source.Length >= NatNetConstants.MaxNameLength )
                throw new ArgumentException( "String too large for field: \"" + source + "\"" );

            return new sForcePlateChannelName { Value = source };
        }
    }


    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
    internal struct sForcePlateDescription
    {
        public Int32 Id;

        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 128 )]
        public string SerialNo;

        public float Width;
        public float Length;
        public float OriginX;
        public float OriginY;
        public float OriginZ;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 12*12 )]
        public float[] CalibrationMatrix;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 4*3 )]
        public float[] Corners;

        public Int32 PlateType;
        public Int32 ChannelDataType;
        public Int32 ChannelCount;

        [MarshalAs( UnmanagedType.ByValArray, SizeConst = NatNetConstants.MaxAnalogChannels )]
        public sForcePlateChannelName[] ChannelNames;
    }
    #endregion Definition types


    #region Data types
    [StructLayout( LayoutKind.Sequential )]
    internal struct sFrameOfMocapData
    {
        public Int32 FrameNumber;

        public Int32 MarkerSetCount;
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = NatNetConstants.MaxModels )]
        public sMarkerSetData[] MarkerSets;

        public Int32 OtherMarkerCount;
        public IntPtr OtherMarkers; // Pointer to float[OtherMarkerCount][3]

        public Int32 RigidBodyCount;
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = NatNetConstants.MaxRigidBodies )]
        public sRigidBodyData[] RigidBodies;

        public Int32 SkeletonCount;
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = NatNetConstants.MaxSkeletons )]
        public sSkeletonData[] Skeletons;

        public Int32 LabeledMarkerCount;
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = NatNetConstants.MaxLabeledMarkers )]
        public sMarker[] LabeledMarkers;

        public Int32 ForcePlateCount;
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = NatNetConstants.MaxForcePlates )]
        public sForcePlateData[] ForcePlates;

        public float Latency;
        public UInt32 Timecode;
        public UInt32 TimecodeSubframe;
        public double Timestamp;
        public Int16 Params;
    }


    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
    internal struct sMarkerSetData
    {
        [MarshalAs( UnmanagedType.ByValTStr, SizeConst = NatNetConstants.MaxNameLength )]
        public string Name;

        public Int32 MarkerCount;
        public IntPtr Markers; // Pointer to float[MarkerCount][3]
    }


    [StructLayout( LayoutKind.Sequential )]
    internal struct sRigidBodyData
    {
        public Int32 Id;
        public float X;
        public float Y;
        public float Z;
        public float QX;
        public float QY;
        public float QZ;
        public float QW;
        public Int32 MarkerCount;
        public IntPtr Markers; // Pointer to float[MarkerCount][3]
        public IntPtr MarkerIds; // Pointer to Int32[MarkerCount]
        public IntPtr MarkerSizes; // Pointer to float[MarkerCount]
        public float MeanError;
        public Int16 Params;
    }


    [StructLayout( LayoutKind.Sequential )]
    internal struct sSkeletonData
    {
        public Int32 Id;
        public Int32 RigidBodyCount;
        public IntPtr RigidBodies; // Pointer to sRigidBodyData[RigidBodyCount]


        public sRigidBodyData[] MarshalRigidBodies()
        {
            sRigidBodyData[] returnArray = new sRigidBodyData[RigidBodyCount];

            IntPtr buffer = Marshal.AllocCoTaskMem( Marshal.SizeOf( typeof( sRigidBodyData ) ) * RigidBodyCount );

            for ( int i = 0; i < RigidBodyCount; ++i )
            {
                returnArray[i] = (sRigidBodyData)Marshal.PtrToStructure( RigidBodies, typeof( sRigidBodyData ) );
                RigidBodies = new IntPtr( RigidBodies.ToInt64() + Marshal.SizeOf( typeof( sRigidBodyData ) ) );
            }

            Marshal.FreeCoTaskMem( buffer );

            return returnArray;
        }
    }


    [StructLayout( LayoutKind.Sequential )]
    internal struct sMarker
    {
        public Int32 Id;
        public float X;
        public float Y;
        public float Z;
        public float Size;
        public Int16 Params;
    }


    [StructLayout( LayoutKind.Sequential )]
    internal struct sForcePlateData
    {
        public Int32 Id;
        public Int32 ChannelCount;
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = NatNetConstants.MaxAnalogChannels )]
        public sAnalogChannelData[] ChannelData;
        public Int16 Params;
    }


    [StructLayout( LayoutKind.Sequential )]
    internal struct sAnalogChannelData
    {
        public Int32 FrameCount;
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = NatNetConstants.MaxAnalogSubframes )]
        public float[] Values;
    }
    #endregion Data types


    [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
    internal struct sNatNetClientConnectParams
    {
        public Int32 ServerCommandPort;
        public Int32 ServerDataPort;
        public string ServerAddress;
        public string LocalAddress;
        public string MulticastAddress;
    };


    /// <summary>
    /// Reverse P/Invoke delegate type for <see cref="NativeMethods.NatNet_Client_SetFrameReceivedCallback"/>.
    /// </summary>
    /// <param name="pFrameOfMocapData">Native pointer to <see cref="sFrameOfMocapData"/>.</param>
    /// <param name="pUserData">User-provided context (void pointer).</param>
    [UnmanagedFunctionPointer( NatNetConstants.NatNetLibCallingConvention )]
    internal delegate void NatNetFrameReceivedCallback( IntPtr pFrameOfMocapData, IntPtr pUserData );


    internal static class NativeMethods
    {
        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern void NatNet_GetVersion( [In, Out, MarshalAs( UnmanagedType.LPArray, SizeConst=4 )] Byte[] version );

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern void NatNet_DecodeID( Int32 compositeId, out Int32 entityId, out Int32 memberId );


        //////////////////////////////////////////////////////////////////////
        // These functions are not a supported part of the public API, and are
        // subject to change without notice.

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Client_Create( out IntPtr clientHandle, NatNetConnectionType connectionType );

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Client_Destroy( IntPtr clientHandle );

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Client_Connect( IntPtr clientHandle, ref sNatNetClientConnectParams connectParams );

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Client_Disconnect( IntPtr clientHandle );

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Client_SetFrameReceivedCallback( IntPtr clientHandle, NatNetFrameReceivedCallback pfnDataCallback );

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Client_GetDataDescriptionList( IntPtr clientHandle, out IntPtr pDataDescriptions );


        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Frame_GetRigidBodyCount( IntPtr pFrameOfMocapData, out Int32 rigidBodyCount );

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Frame_GetRigidBody( IntPtr pFrameOfMocapData, Int32 rigidBodyIndex, out sRigidBodyData rigidBodyData );

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Frame_RigidBody_Marker_GetPosition( IntPtr pFrameOfMocapData, Int32 rigidBodyIndex, Int32 markerIndex, out float markerPosX, out float markerPosY, out float markerPosZ );

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Frame_RigidBody_Marker_GetSize( IntPtr pFrameOfMocapData, Int32 rigidBodyIndex, Int32 markerIndex, out float markerSize );


        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Frame_GetSkeletonCount( IntPtr pFrameOfMocapData, out Int32 skeletonCount );

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Frame_Skeleton_GetId( IntPtr pFrameOfMocapData, Int32 skeletonIndex, out Int32 skeletonId );

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Frame_Skeleton_GetRigidBodyCount( IntPtr pFrameOfMocapData, Int32 skeletonIndex, out Int32 rigidBodyCount );

        [DllImport( NatNetConstants.NatNetLibDllBaseName, CallingConvention = NatNetConstants.NatNetLibCallingConvention )]
        public static extern NatNetError NatNet_Frame_Skeleton_GetRigidBody( IntPtr pFrameOfMocapData, Int32 skeletonIndex, Int32 rigidBodyIndex, out sRigidBodyData rigidBodyData );
    }
}
