using UnityEngine;

/// <summary>
/// This is the same Camera FreeLookCam of the Stardard Assets Modify to Fit My Needs
/// </summary>
namespace MalbersAnimations
{
    public class MFreeLookCamera : MonoBehaviour
    {
        public enum UpdateType                                      // The available methods of updating are:
        {
            FixedUpdate,                                            // Update in FixedUpdate (for tracking rigidbodies).
            LateUpdate,                                             // Update in LateUpdate. (for tracking objects that are moved in Update)
            Update
        }

#if REWIRED
        [Header("Rewired Connection")]
#else
        [HideInInspector]
#endif
        public string PlayerID = "Player0";

        [Space]

        public Transform m_Target;                                  // The target object to follow
        public UpdateType updateType = UpdateType.FixedUpdate;      // stores the selected update type

        private Transform cam;                                      // the transform of the camera
        private Transform pivot;                                    // the point at which the camera pivots around

        public float m_MoveSpeed = 10f;                             // How fast the rig will move to keep up with the target's position.
        //public float smoothTime = 0.5f;                             // How fast the rig will move to keep up with the target's position.
        [Range(0f, 10f)]
        public float m_TurnSpeed = 10f;                             // How fast the rig will rotate from user input.
        public float m_TurnSmoothing = 10f;                         // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
        public float m_TiltMax = 75f;                               // The maximum value of the x axis rotation of the pivot.
        public float m_TiltMin = 45f;                               // The minimum value of the x axis rotation of the pivot.

        //[Header("Camera Axis")]
        public InputAxis Vertical = new InputAxis("Mouse Y", true, false);
        public InputAxis Horizontal = new InputAxis("Mouse X", true, false);

        [Space]
        public bool m_LockCursor = false;                           // Whether the cursor should be hidden and locked.
        [Space]
        public FreeLockCameraManager manager;
        public FreeLookCameraState startState;

        private float m_LookAngle;                                  // The rig's y axis rotation.
        private float m_TiltAngle;                                  // The pivot's x axis rotation.
        private const float k_LookDistance = 100f;                  // How far in front of the pivot the character's look target is.
        private Vector3 m_PivotEulers;
        private Quaternion m_PivotTargetRot;
        private Quaternion m_TransformTargetRot;

        float x,  y;

        public Transform Target
        {
            get { return m_Target; }
        }

        public Transform Cam
        {
            get { return cam; }
            set { cam = value; }
        }

        public Transform Pivot
        {
            get { return pivot; }
            set { pivot = value; }
        }

        IInputSystem inputSystem;
            
        protected void Awake()
        {
            Cam = GetComponentInChildren<Camera>().transform;
            Pivot = Cam.parent;

            if (manager)  manager.SetCamera(this);
        
            if (startState) SetState(startState);

            Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None;  // Lock or unlock the cursor.
            Cursor.visible = !m_LockCursor;

            m_PivotEulers = Pivot.rotation.eulerAngles;
            m_PivotTargetRot = Pivot.transform.localRotation;
            m_TransformTargetRot = transform.localRotation;

            inputSystem = DefaultInput.GetInputSystem(PlayerID);

            Horizontal.InputSystem = Vertical.InputSystem = inputSystem;        //Update the Input System on the Axis
        }

        public virtual void SetState(FreeLookCameraState profile)
        {
            Pivot.localPosition = profile.PivotPos;
            Cam.localPosition = profile.CamPos;
            Cam.GetComponent<Camera>().fieldOfView = profile.CamFOV;
        }

        protected void FollowTarget(float deltaTime)
        {
            if (m_Target == null) return;

            transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime * m_MoveSpeed);  // Move the rig towards target position.
        }

        private void HandleRotationMovement()
        {
            if (Time.timeScale < float.Epsilon) return;

            x = Horizontal.GetAxis;
            y = Vertical.GetAxis;

            m_LookAngle += x * m_TurnSpeed;                                                     // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
            m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);                       // Rotate the rig (the root object) around Y axis only:

            m_TiltAngle -= y * m_TurnSpeed;                                                 // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
            m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);                  // and make sure the new value is within the tilt range

            m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_PivotEulers.y, m_PivotEulers.z); // Tilt input around X is applied to the pivot (the child of this object)

            if (m_TurnSmoothing > 0)
            {
                Pivot.localRotation = Quaternion.Slerp(Pivot.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
            }
            else
            {
                Pivot.localRotation = m_PivotTargetRot;
                transform.localRotation = m_TransformTargetRot;
            }
        }
        void Update()
        {
            HandleRotationMovement();
            if (updateType == UpdateType.Update) FollowTarget(Time.deltaTime);
        }

        void FixedUpdate()
        {
            if (updateType == UpdateType.FixedUpdate) FollowTarget(Time.fixedDeltaTime);  // we update from here if updatetype is set to Fixed, or in auto mode,
        }

        void LateUpdate()
        {
            if (updateType == UpdateType.LateUpdate) FollowTarget(Time.deltaTime);
        }


        public virtual void SetTarget(Transform newTransform)
        {
            m_Target = newTransform;
        }

        public virtual void SetTarget(GameObject newGO)
        {
            m_Target = newGO.transform;
        }
    }
}
