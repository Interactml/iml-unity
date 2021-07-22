//======================================================================================================
// Copyright 2016, NaturalPoint Inc.
//======================================================================================================

using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class OptitrackHmd : MonoBehaviour
{
    public OptitrackStreamingClient StreamingClient;
    public Int32 RigidBodyId;
    
    private GameObject m_hmdCameraObject;
    private IntPtr m_driftCorrHandle;

    private List<XRDisplaySubsystemDescriptor> displaysDescs = new List<XRDisplaySubsystemDescriptor>();
    private List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();


    void Start()
    {
        // If the user didn't explicitly associate a client, find a suitable default.
        if ( this.StreamingClient == null )
        {
            this.StreamingClient = OptitrackStreamingClient.FindDefaultClient();

            // If we still couldn't find one, disable this component.
            if ( this.StreamingClient == null )
            {
                Debug.LogError( GetType().FullName + ": Streaming client not set, and no " + typeof( OptitrackStreamingClient ).FullName + " components found in scene; disabling this component.", this );
                this.enabled = false;
                return;
            }
        }

        /*SubsystemManager.GetSubsystemDescriptors(displaysDescs);

        if (displaysDescs.Count > 0)
        {
            string vrDeviceFamily = UnityEngine.XR.XRDevice.family;
            string vrDeviceModel = UnityEngine.XR.XRDevice.model;
            bool isOculusDevice = String.Equals( vrDeviceFamily, "oculus", StringComparison.CurrentCultureIgnoreCase );

            if ( isOculusDevice )
            {
                if ( TryDisableOvrPositionTracking() == false )
                {
                    Debug.LogError( GetType().FullName + ": Detected Oculus HMD (\"" + vrDeviceModel + "\", but could not disable OVR position tracking.", this );
                }
                else
                {
                    Debug.Log( GetType().FullName + ": Successfully disabled position tracking for HMD \"" + vrDeviceModel + "\".", this );
                }
            }
            else
            {
                Debug.LogWarning( GetType().FullName + ": Unrecognized HMD type (\"" + vrDeviceFamily + "\", \"" + vrDeviceModel + "\").", this );
            }
        }
        else
        {
            Debug.LogWarning( GetType().FullName + ": No VRDevice present.", this );

        }*/

        // Cache a reference to the gameobject containing the HMD Camera.
        Camera hmdCamera = this.GetComponentInChildren<Camera>();
        if ( hmdCamera == null )
        {
            Debug.LogError( GetType().FullName + ": Couldn't locate HMD-driven Camera component in children.", this );
        }
        else
        {
            m_hmdCameraObject = hmdCamera.gameObject;
        }
    }


    void OnEnable()
    {
        NpHmdResult result = NativeMethods.NpHmd_Create( out m_driftCorrHandle );
        if ( result != NpHmdResult.OK || m_driftCorrHandle == IntPtr.Zero )
        {
            Debug.LogError( GetType().FullName + ": NpHmd_GetOrientationCorrection failed.", this );
            m_driftCorrHandle = IntPtr.Zero;
            this.enabled = false;
            return;
        }
    }


    void OnDisable()
    {
        if ( m_driftCorrHandle != IntPtr.Zero )
        {
            NativeMethods.NpHmd_Destroy( m_driftCorrHandle );
            m_driftCorrHandle = IntPtr.Zero;
        }
    }


    void Update()
    {
        OptitrackRigidBodyState rbState = StreamingClient.GetLatestRigidBodyState( RigidBodyId );
        if ( rbState != null && rbState.DeliveryTimestamp.AgeSeconds < 1.0f )
        {
            // Update position.
            this.transform.localPosition = rbState.Pose.Position;

            // Update drift correction.
            if ( m_driftCorrHandle != IntPtr.Zero && m_hmdCameraObject )
            {
                NpHmdQuaternion opticalOri = new NpHmdQuaternion( rbState.Pose.Orientation );
                NpHmdQuaternion inertialOri = new NpHmdQuaternion( m_hmdCameraObject.transform.localRotation );

                NpHmdResult result = NativeMethods.NpHmd_MeasurementUpdate(
                    m_driftCorrHandle,
                    ref opticalOri, // const
                    ref inertialOri, // const
                    Time.deltaTime
                );

                if ( result == NpHmdResult.OK )
                {
                    NpHmdQuaternion newCorrection;
                    result = NativeMethods.NpHmd_GetOrientationCorrection( m_driftCorrHandle, out newCorrection );

                    if ( result == NpHmdResult.OK )
                    {
                        this.transform.localRotation = newCorrection;
                    }
                    else
                    {
                        Debug.LogError( GetType().FullName + ": NpHmd_GetOrientationCorrection failed.", this );
                        this.enabled = false;
                        return;
                    }
                }
                else
                {
                    Debug.LogError( GetType().FullName + ": NpHmd_MeasurementUpdate failed.", this );
                    this.enabled = false;
                    return;
                }
            }
        }
    }


    private enum NpHmdResult
    {
        OK = 0,
        InvalidArgument
    }


    private struct NpHmdQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public NpHmdQuaternion( UnityEngine.Quaternion other )
        {
            this.x = other.x;
            this.y = other.y;
            this.z = other.z;
            this.w = other.w;
        }

        public static implicit operator UnityEngine.Quaternion( NpHmdQuaternion nphmdQuat )
        {
            return new UnityEngine.Quaternion
            {
                w = nphmdQuat.w,
                x = nphmdQuat.x,
                y = nphmdQuat.y,
                z = nphmdQuat.z
            };
        }
    }


    private static class NativeMethods
    {
        public const string NpHmdDllBaseName = "HmdDriftCorrection";
        public const CallingConvention NpHmdDllCallingConvention = CallingConvention.Cdecl;

        [DllImport( NpHmdDllBaseName, CallingConvention = NpHmdDllCallingConvention )]
        public static extern NpHmdResult NpHmd_UnityInit();

        [DllImport( NpHmdDllBaseName, CallingConvention = NpHmdDllCallingConvention )]
        public static extern NpHmdResult NpHmd_Create( out IntPtr hmdHandle );

        [DllImport( NpHmdDllBaseName, CallingConvention = NpHmdDllCallingConvention )]
        public static extern NpHmdResult NpHmd_Destroy( IntPtr hmdHandle );

        [DllImport( NpHmdDllBaseName, CallingConvention = NpHmdDllCallingConvention )]
        public static extern NpHmdResult NpHmd_MeasurementUpdate( IntPtr hmdHandle, ref NpHmdQuaternion opticalOrientation, ref NpHmdQuaternion inertialOrientation, float deltaTimeSec );

        [DllImport( NpHmdDllBaseName, CallingConvention = NpHmdDllCallingConvention )]
        public static extern NpHmdResult NpHmd_GetOrientationCorrection( IntPtr hmdHandle, out NpHmdQuaternion correction );


        public const string OvrPluginDllBaseName = "OVRPlugin";
        public const CallingConvention OvrPluginDllCallingConvention = CallingConvention.Cdecl;

        [DllImport( OvrPluginDllBaseName, CallingConvention = OvrPluginDllCallingConvention )]
        public static extern Int32 ovrp_GetCaps();

        [DllImport( OvrPluginDllBaseName, CallingConvention = OvrPluginDllCallingConvention )]
        public static extern Int32 ovrp_SetCaps( Int32 caps );

        [DllImport( OvrPluginDllBaseName, CallingConvention = OvrPluginDllCallingConvention )]
        public static extern Int32 ovrp_SetTrackingIPDEnabled( Int32 value );
    }


    private bool TryDisableOvrPositionTracking()
    {
        try
        {
            const Int32 kCapsPositionBit = (1 << 5);
            Int32 oldCaps = NativeMethods.ovrp_GetCaps();
            bool bSucceeded = NativeMethods.ovrp_SetCaps( oldCaps & ~kCapsPositionBit ) != 0;

            try
            {
                NativeMethods.ovrp_SetTrackingIPDEnabled( 1 );
            }
            catch ( Exception ex )
            {
                Debug.LogError( GetType().FullName + ": ovrp_SetTrackingIPDEnabled failed. OVRPlugin too old?", this );
                Debug.LogException( ex, this );
                bSucceeded = false;
            }

            return bSucceeded;
        }
        catch ( Exception ex )
        {
            Debug.LogException( ex, this );
            return false;
        }
    }
}
