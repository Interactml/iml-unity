using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// The XR Rig component is typically attached to the base object of the XR Rig, the game object that will be manipulated via locomotion.
    /// It is also used for offsetting the camera.
    /// </summary>
    [AddComponentMenu("XR/XR Rig")]
    [DisallowMultipleComponent]
    public class XRRig : MonoBehaviour
    {
        const float k_DefaultCameraYOffset = 1.36144f;

        [SerializeField]
        [Tooltip("The 'Rig' game object is used to refer to the base of the XR Rig, by default it is attached object." +
            " This is the game object that will be manipulated via locomotion.")]
        GameObject m_RigBaseGameObject = null;
        /// <summary>
        /// The "Rig" game object is used to refer to the base of the XR Rig, by default it is attached object.
        /// This is the game object that will be manipulated via locomotion.
        /// </summary>
        public GameObject rig { get { return m_RigBaseGameObject; } set { m_RigBaseGameObject = value; } }


        [SerializeField]
        [Tooltip("GameObject to move to desired height off the floor (defaults to this object if none provided).")]
        GameObject m_CameraFloorOffsetObject = null;
        /// <summary>Gets or sets the GameObject to move to desired height off the floor (defaults to this object if none provided).</summary>
        public GameObject cameraFloorOffsetObject { get { return m_CameraFloorOffsetObject; } set { m_CameraFloorOffsetObject = value; SetupCamera(); } }


        [SerializeField]
        [Tooltip("The game object that contains the camera, this is usually the 'Head' of XR rigs.")]
        GameObject m_CameraGameObject = null;
        /// <summary>
        /// The game object that contains the camera, this is usually the 'Head' of XR rigs.
        /// </summary>
        public GameObject cameraGameObject { get { return m_CameraGameObject;} set { m_CameraGameObject = value;}}

#if UNITY_2019_3_OR_NEWER
        [SerializeField]
        [Tooltip("Sets the type of tracking origin to use for this Rig. Tracking origins identify where 0,0,0 is in the world of tracking.")]
        /// <summary>Gets or sets the type of tracking origin to use for this Rig. Tracking origins identify where 0,0,0 is in the world of tracking. Not all devices support all tracking spaces; if the selected tracking space is not set it will fall back to Stationary.</summary>
        TrackingOriginModeFlags m_TrackingOriginMode = TrackingOriginModeFlags.Unknown;
        public TrackingOriginModeFlags TrackingOriginMode { get { return m_TrackingOriginMode; } set { m_TrackingOriginMode = value; TryInitializeCamera(); } }
#endif

        // Disable Obsolete warnings for TrackingSpaceType, explicitely to read in old data and upgrade.
#pragma warning disable 0618
        [SerializeField]
        [Tooltip("Set if the XR experience is Room Scale or Stationary.")]
        TrackingSpaceType m_TrackingSpace = TrackingSpaceType.Stationary;

        /// <summary>Gets or sets if the experience is rooms scale or stationary.  Not all devices support all tracking spaces; if the selected tracking space is not set it will fall back to Stationary.</summary>
#if UNITY_2019_3_OR_NEWER
        [Obsolete("XRRig.trackingSpace is obsolete.  Please use XRRig.trackingOriginMode.")]
#endif
        public TrackingSpaceType trackingSpace { get { return m_TrackingSpace; } set { m_TrackingSpace = value; TryInitializeCamera(); } }
#pragma warning restore 0618

        [SerializeField]
        [Tooltip("Camera Height to be used when in Stationary tracking space.")]
        float m_CameraYOffset = k_DefaultCameraYOffset;
        /// <summary>Gets or sets the amount the camera is offset from the floor (by moving the camera offset object).</summary>
        public float cameraYOffset { get { return m_CameraYOffset; } set { m_CameraYOffset = value; TryInitializeCamera(); } }

        /// <summary>Gets the rig's local position in camera space.</summary>
        public Vector3 rigInCameraSpacePos { get { return m_CameraGameObject.transform.InverseTransformPoint(m_RigBaseGameObject.transform.position); } }

        /// <summary>Gets the camera's local position in rig space.</summary>
        public Vector3 cameraInRigSpacePos { get { return m_RigBaseGameObject.transform.InverseTransformPoint(m_CameraGameObject.transform.position); } }

        /// <summary>Gets the camera's height relative to the rig.</summary>
        public float cameraInRigSpaceHeight { get { return cameraInRigSpacePos.y; } }

        /// <summary>
        /// Used to cache the input subsystems without creating additional garbage.
        /// </summary>
        static List<XRInputSubsystem> s_InputSubsystems = new List<XRInputSubsystem>();

        // Bookkeeping to track lazy initialization of the tracking space type.
        bool m_CameraInitialized = false;
        bool m_CameraInitializing = false;

        /// Utility helper to migrate from TrackingSpace to TrackingOrigin seamlessly
        void UpgradeTrackingSpaceToTrackingOriginMode()
        {
#if UNITY_2019_3_OR_NEWER
            // Disable Obsolete warnings for TrackingSpaceType, explicitely to allow a proper upgrade path.
#pragma warning disable 0618
            if (m_TrackingOriginMode == TrackingOriginModeFlags.Unknown && m_TrackingSpace <= TrackingSpaceType.RoomScale)
            {
                switch (m_TrackingSpace)
                {
                    case TrackingSpaceType.RoomScale:
                        {
                            m_TrackingOriginMode = TrackingOriginModeFlags.Floor;
                            break; 
                        }
                    case TrackingSpaceType.Stationary:
                        {
                            m_TrackingOriginMode = TrackingOriginModeFlags.Device;
                            break; 
                        }
                    default:
                        break;
                }

                // Tag is Invalid not to be used.
                m_TrackingSpace = (TrackingSpaceType)3;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif //UNITY_EDITOR
#pragma warning restore 0618
            }
#endif //UNITY_2019_3_OR_NEWER
        }


        void Awake()
        {
            if (!m_CameraFloorOffsetObject)
            {
                Debug.LogWarning("No camera container specified for XR Rig, using attached GameObject");
                m_CameraFloorOffsetObject = this.gameObject;
            }
        }

        void Start()
        {
            TryInitializeCamera();
        }

        void OnValidate()
        {
            UpgradeTrackingSpaceToTrackingOriginMode();

            if (rig == null)
                rig = gameObject;

            TryInitializeCamera();
        }

        void TryInitializeCamera()
        {
            m_CameraInitialized = SetupCamera();            
            if (!m_CameraInitialized & !m_CameraInitializing)
                StartCoroutine(RepeatInitializeCamera());
        }

        /// <summary>
        /// Repeatedly attempt to initialize the camera.
        /// </summary>
        /// <returns></returns>
        IEnumerator RepeatInitializeCamera()
        {
            m_CameraInitializing = true;
            yield return null;
            while (!m_CameraInitialized)
            {
                m_CameraInitialized = SetupCamera();
                yield return null;
            }
            m_CameraInitializing = false;
        }

        /// <summary>
        /// Handles re-centering and off-setting the camera in space depending on which tracking space it is setup in.
        /// </summary>
#if UNITY_2019_3_OR_NEWER
        bool SetupCamera()
        {
            SubsystemManager.GetInstances<XRInputSubsystem>(s_InputSubsystems);

            bool initialized = true;
            if (s_InputSubsystems.Count != 0)
            {
                for (int i = 0; i < s_InputSubsystems.Count; i++)
                {
                    initialized &= SetupCamera(s_InputSubsystems[i]);
                }
            }
            else
            {
                // Disable Obsolete warnings for TrackingSpaceType, explicitely to allow a proper upgrade path.
#pragma warning disable 0618
                if (m_TrackingOriginMode == TrackingOriginModeFlags.Floor)
                {
                    SetupCameraLegacy(TrackingSpaceType.RoomScale);
                }
                else if (m_TrackingOriginMode == TrackingOriginModeFlags.Floor)
                {
                    SetupCameraLegacy(TrackingSpaceType.Stationary);
                }
#pragma warning restore 0618
            }

            return initialized;
        }

        bool SetupCamera(XRInputSubsystem subsystem)
        {
            if (subsystem == null)
                return false;

            bool trackingSettingsSet = false;

            float desiredOffset = cameraYOffset;
            if (m_TrackingOriginMode == TrackingOriginModeFlags.Floor)
            {
                // We need to check for Unknown because we may not be in a state where we can read this data yet.
                if((subsystem.GetSupportedTrackingOriginModes() & (TrackingOriginModeFlags.Floor | TrackingOriginModeFlags.Unknown)) == 0)
                {
                    Debug.LogWarning("XRRig.SetupCamera: Attempting to set the tracking space to Room, but that is not supported by the SDK.");
                    return true;
                }

                if(subsystem.TrySetTrackingOriginMode(m_TrackingOriginMode))
                {
                    desiredOffset = 0;
                    trackingSettingsSet = true;
                }        
            }

            if (m_TrackingOriginMode == TrackingOriginModeFlags.Device)
            {
                // We need to check for Unknown because we may not be in a state where we can read this data yet.
                if ((subsystem.GetSupportedTrackingOriginModes() & (TrackingOriginModeFlags.Device | TrackingOriginModeFlags.Unknown)) == 0)
                {
                    Debug.LogWarning("XRRig.SetupCamera: Attempting to set the tracking space to Stationary, but that is not supported by the SDK.");
                    return true;
                }

                if (subsystem.TrySetTrackingOriginMode(m_TrackingOriginMode))
                {
                    trackingSettingsSet = subsystem.TryRecenter();
                }                    
            }

            if(trackingSettingsSet)
            {
                // Move camera to correct height
                if (m_CameraFloorOffsetObject)
                    m_CameraFloorOffsetObject.transform.localPosition = new Vector3(m_CameraFloorOffsetObject.transform.localPosition.x, desiredOffset, m_CameraFloorOffsetObject.transform.localPosition.z);

            }
            return trackingSettingsSet;
        }
#else
        bool SetupCamera()
        {
            SetupCameraLegacy(m_TrackingSpace);
            return true;
        }
#endif

        // Disable Obsolete warnings for TrackingSpaceType, explicitely to allow for using legacy data if available.
#pragma warning disable 0618
        void SetupCameraLegacy(TrackingSpaceType trackingSpace)
        {
            float cameraYOffset = m_CameraYOffset;
            XRDevice.SetTrackingSpaceType(trackingSpace);
            if (trackingSpace == TrackingSpaceType.Stationary)
                InputTracking.Recenter();
            else if (trackingSpace == TrackingSpaceType.RoomScale)
                cameraYOffset = 0;

            // Move camera to correct height
            if (m_CameraFloorOffsetObject)
                m_CameraFloorOffsetObject.transform.localPosition = new Vector3(m_CameraFloorOffsetObject.transform.localPosition.x, cameraYOffset, m_CameraFloorOffsetObject.transform.localPosition.z);
        }
#pragma warning restore 0618

        /// <summary>
        /// Rotates the rig object around the camera object by the provided angleDegrees, this rotation only occurs around the rig's Up vector
        /// </summary>
        /// <param name="angleDegrees">the amount of rotation in degrees.</param>
        /// <returns>true if the rotation is performed</returns>
        public bool RotateAroundCameraUsingRigUp(float angleDegrees)
        {
            return RotateAroundCameraPosition(m_RigBaseGameObject.transform.up, angleDegrees);
        }

        /// <summary>
        /// Rotates the rig object around the camera objects position in world space using the provided vector as the rotation axis. The
        /// Rig object is rotated by the amount of degrees provided in angleDegrees
        /// </summary>
        /// <param name="vector">the axis of the rotation</param>
        /// <param name="angleDegrees">the amount of rotation in degrees.</param>
        /// <returns>true if the rotation is performed</returns>
        public bool RotateAroundCameraPosition(Vector3 vector, float angleDegrees)
        {
            if (m_CameraGameObject == null || m_RigBaseGameObject == null)
            {
                return false;
            }
            // rotate around the camera position, using the rig up vector as the axis for the rotation.
            m_RigBaseGameObject.transform.RotateAround(m_CameraGameObject.transform.position, vector, angleDegrees);

            return true;
        }

        /// <summary>
        /// This function will rotate the rig object such that the rig's up vector will match the provided vector.
        /// </summary>
        /// <param name="destinationUp">the vector to which the rig object's up vector will be matched.</param>
        /// <returns>true if the rotation is performed or the vectors have already been matched. </returns>
        public bool MatchRigUp(Vector3 destinationUp)
        {
            if (m_RigBaseGameObject.transform.up == destinationUp)
                return true;

            if (m_RigBaseGameObject == null)
            {
                return false;
            }
            Quaternion rigUp = Quaternion.FromToRotation(m_RigBaseGameObject.transform.up, destinationUp);
            m_RigBaseGameObject.transform.rotation = rigUp * transform.rotation;
            
            return true;
        }

        /// <summary>
        /// This function will rotate the rig object around the camera object using the destinationUp vector such that
        /// - the camera will look at the area in the direction of the destinationForward
        /// - the projection of camera's forward vector on the plane with the normal destinationUp will be in the direction of destinationForward
        /// - the up vector of the rig object will match the provided destinationUp vector, note that the camera's Up vector can not be manipulated
        /// </summary>
        /// <param name="destinationUp">the up vector that the rig's up vector will be matched to</param>
        /// <param name="destinationForward">the forward vector that will be matched to the projection of the camera's forward vector on the plane with the normal destinationUp.</param>
        /// <returns>true if the rotation is performed. </returns>
        public bool MatchRigUpCameraForward(Vector3 destinationUp, Vector3 destinationForward)
        {
            if (m_CameraGameObject != null && MatchRigUp(destinationUp))
            {
                // project current camera's forward vector on the destination plane, whose normal vector is destinationUp.
                Vector3 projectedCamForward = Vector3.ProjectOnPlane(cameraGameObject.transform.forward, destinationUp).normalized;

                // the angle that we want the rig to rotate is the signed angle between projectedCamForward and destinationForward, after the up vectors are matched. 
                float signedAngle = Vector3.SignedAngle(projectedCamForward, destinationForward, destinationUp);

                RotateAroundCameraPosition(destinationUp, signedAngle);

                return true;
            }

            return false;
        }


        /// <summary>
        /// This function will rotate the rig object around the camera object using the destinationUp vector such that 
        /// - the forward vector of the rig object, which is the direction the player moves in Unity when walking forward in the physical world, will match the provided destinationUp vector
        /// - the up vector of the rig object will match the provided destinationUp vector
        /// </summary>
        /// <param name="destinationUp">the up vector that the rig's up vector will be matched to</param>
        /// <param name="destinationForward">the forward vector that will be matched to the forward vector of the rig object,
        /// which is the direction the player moves in Unity when walking forward in the physical world.</param>
        /// <returns>true if the rotation is performed. </returns>
        public bool MatchRigUpRigForward (Vector3 destinationUp, Vector3 destinationForward)
        {
            if (m_RigBaseGameObject != null && MatchRigUp(destinationUp))
            {
                // the angle that we want the rig to rotate is the signed angle between the rig's forward and destinationForward, after the up vectors are matched. 
                float signedAngle = Vector3.SignedAngle(m_RigBaseGameObject.transform.forward, destinationForward, destinationUp);

                RotateAroundCameraPosition(destinationUp, signedAngle);

                return true;
            }

            return false;
        }


        /// <summary>
        /// This function moves the camera to the world location provided by desiredWorldLocation. 
        /// It does this by moving the rig object so that the camera's world location matches the desiredWorldLocation
        /// </summary>
        /// <param name="desiredWorldLocation">the position in world space that the camera should be moved to</param>
        /// <returns>true if the move is performed</returns>
        public bool MoveCameraToWorldLocation(Vector3 desiredWorldLocation)
        {
            if (m_CameraGameObject == null)
            {
                return false;
            }

            Matrix4x4 rot = Matrix4x4.Rotate(cameraGameObject.transform.rotation);
            Vector3 delta = rot.MultiplyPoint3x4(rigInCameraSpacePos);
            m_RigBaseGameObject.transform.position = delta + desiredWorldLocation;

            return true;
        }


        private void OnDrawGizmos()
        {

            if (m_RigBaseGameObject != null)
            {
                // draw XR Rig box
                Gizmos.color = Color.green;
                GizmoHelpers.DrawWireCubeOriented(m_RigBaseGameObject.transform.position, m_RigBaseGameObject.transform.rotation, 3.0f);
                GizmoHelpers.DrawAxisArrows(m_RigBaseGameObject.transform, 0.5f);
            }

            if(m_CameraFloorOffsetObject != null)
            {
                GizmoHelpers.DrawAxisArrows(m_CameraFloorOffsetObject.transform, 0.5f);
            }

            if(m_CameraGameObject != null)
            {
                Gizmos.color = Color.red;
                GizmoHelpers.DrawWireCubeOriented(m_CameraGameObject.transform.position, m_CameraGameObject.transform.rotation, 0.1f);
                GizmoHelpers.DrawAxisArrows(m_CameraGameObject.transform, 0.5f);

                Vector3 floorPos = m_CameraGameObject.transform.position;
                floorPos.y = m_RigBaseGameObject.transform.position.y;
                Gizmos.DrawLine(floorPos, m_CameraGameObject.transform.position);
            }
        }
    }
}