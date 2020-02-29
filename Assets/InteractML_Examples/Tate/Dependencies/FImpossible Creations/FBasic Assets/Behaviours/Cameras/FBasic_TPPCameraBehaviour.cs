using UnityEngine;

namespace FIMSpace.Basics
{
    /// <summary>
    /// FM: Simple component to controll behaviour of camera in TPP mode
    /// </summary>
    public class FBasic_TPPCameraBehaviour : MonoBehaviour
    {
        [Header("Transform to be followed by camera")]
        public Transform ToFollow;

        [Header("Offset in position in reference to target transform (focus point)")]
        public Vector3 FollowingOffset = new Vector3(0f, 1.5f, 0f);

        [Header("Offset in position in reference to camera orientation")]
        public Vector3 FollowingOffsetDirection = new Vector3(0f, 0f, 0f);

        /// <summary> Variables to controll distance of camera to following object </summary>
        [Header("Clamp values for zoom of camera")]
        public Vector2 DistanceRanges = new Vector2(5f, 10f);
        private float targetDistance;
        private float animatedDistance;

        /// <summary> Variables to controll rotation of camera around followed object </summary>
        public Vector2 RotationRanges = new Vector2(-60f, 60f);
        private Vector2 targetSphericRotation = new Vector2(0f, 0f);
        private Vector2 animatedSphericRotation = new Vector2(0f, 0f);

        [Space(10)]
        [Tooltip("Sensitivity value for rotating camera around following object")]
        public float RotationSensitivity = 10f;

        [Header("If you want camera rotation to be smooth")]
        [Range(0.1f, 1f)]
        // V1.1
        public float RotationSpeed = 1f;

        [Header("If you want camera to follow target with some smoothness")]
        [Range(0f,1f)]
        // V1.1
        public float HardFollowValue = 1f;

        [Header("If you want to hold cursor (cursor switch on TAB)")]
        public bool LockCursor = true;

        /// <summary> Just to make turning off lock cursor less annoying </summary>
        private bool rotateCamera = true;

        /// <summary> Raycast checking if there is obstacle blocking our vision </summary>
        private RaycastHit sightObstacleHit;

        [Header("Layer mask to check obstacles in sight ray")]
        public LayerMask SightLayerMask;

        /// <summary> Target position for camera from basic calculations, if raycast hit something, there is used other position </summary>
        private Vector3 targetPosition;

        [Header("How far forward raycast should check collision for camera")]
        public float CollisionOffset = 1f;

        public EFUpdateClock UpdateClock = EFUpdateClock.Update;

        /// <summary>
        /// Setting some basic variables for initialization
        /// </summary>
        void Start()
        {
            targetDistance = (DistanceRanges.x + DistanceRanges.y) / 2;
            animatedDistance = DistanceRanges.y;

            targetSphericRotation = new Vector2(0f, 23f);
            animatedSphericRotation = targetSphericRotation;

            if ( LockCursor )
            {
                HelperSwitchCursor();
            }
        }

        void UpdateMethods()
        {
            InputCalculations();
            ZoomCalculations();
            FollowCalculations();
            RaycastCalculations();
            SwitchCalculations();
        }

        /// <summary>
        /// Execute methods responsible for component's behaviour
        /// </summary>
        void LateUpdate()
        {
            if (UpdateClock != EFUpdateClock.LateUpdate) return;
            UpdateMethods();
        }

        void Update()
        {
            if (UpdateClock != EFUpdateClock.Update) return;
            UpdateMethods();
        }

        void FixedUpdate()
        {
            if (UpdateClock != EFUpdateClock.FixedUpdate) return;
            UpdateMethods();
        }

        /// <summary> 
        /// Calculations for input mouse controll 
        /// </summary>
        void InputCalculations()
        {
            targetDistance -= (Input.GetAxis("Mouse ScrollWheel") * 5f);

            if (!rotateCamera) return;

            targetSphericRotation.x += Input.GetAxis("Mouse X") * RotationSensitivity;
            targetSphericRotation.y -= Input.GetAxis("Mouse Y") * RotationSensitivity;
        }

        /// <summary> 
        /// Calculations for zoom / distance value of camera 
        /// </summary>
        private void ZoomCalculations()
        {
            if ( !sightObstacleHit.transform ) targetDistance = Mathf.Clamp(targetDistance, DistanceRanges.x, DistanceRanges.y);
            animatedDistance = Mathf.Lerp(animatedDistance, targetDistance, Time.deltaTime * 8f);
        }

        /// <summary>
        /// Simple calculations to follow target object
        /// </summary>
        private void FollowCalculations()
        {
            targetSphericRotation.y = HelperClampAngle(targetSphericRotation.y, RotationRanges.x, RotationRanges.y);

            if (RotationSpeed < 1f) animatedSphericRotation = new Vector2(Mathf.LerpAngle(animatedSphericRotation.x, targetSphericRotation.x, Time.deltaTime * 30 * RotationSpeed), Mathf.LerpAngle(animatedSphericRotation.y, targetSphericRotation.y, Time.deltaTime * 30 * RotationSpeed)); else animatedSphericRotation = targetSphericRotation;

            Quaternion rotation = Quaternion.Euler(animatedSphericRotation.y, animatedSphericRotation.x, 0f);
            transform.rotation = rotation;

            Vector3 targetPosition = ToFollow.transform.position + FollowingOffset;

            if ( HardFollowValue < 1f)
            {
                float lerpValue = Mathf.Lerp(0.5f, 40f, HardFollowValue);
                targetPosition = Vector3.Lerp(this.targetPosition, targetPosition, Time.deltaTime * lerpValue);
            }

            this.targetPosition = targetPosition;
        }

        /// <summary> 
        /// Basic collision check to prevent camera from going through objects 
        /// </summary>
        private void RaycastCalculations()
        {
            Vector3 followPoint = ToFollow.transform.position + FollowingOffset + transform.TransformVector(FollowingOffsetDirection);
            Quaternion cameraDir = Quaternion.Euler(targetSphericRotation.y, targetSphericRotation.x, 0f);
            Ray directionRay = new Ray(followPoint, cameraDir * -Vector3.forward);

            // If there is something in sight ray way
            if ( Physics.Raycast(directionRay, out sightObstacleHit, targetDistance + CollisionOffset, SightLayerMask, QueryTriggerInteraction.Ignore) )
            {
                transform.position = sightObstacleHit.point - directionRay.direction * CollisionOffset;
            }
            else
            {
                Vector3 rotationOffset = transform.rotation * -Vector3.forward * animatedDistance;
                transform.position = targetPosition + rotationOffset + transform.TransformVector(FollowingOffsetDirection);
            }
        }

        /// <summary> 
        /// Calculations for switching cursor state etc. 
        /// </summary>
        private void SwitchCalculations()
        {
            if ( LockCursor )
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    HelperSwitchCursor();
                    if (Cursor.visible) rotateCamera = false; else rotateCamera = true;
                }
            }
        }

        #region Helpers

        /// <summary>
        /// Clamping angle in 360 circle
        /// </summary>
        private float HelperClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360.0f;

            if (angle > 360)
                angle -= 360.0f;

            return Mathf.Clamp(angle, min, max);
        }

        /// <summary>
        /// Switching cursor state for right work of camera rotating mechanics
        /// </summary>
        private void HelperSwitchCursor()
        {
            if (Cursor.visible)
            {
                if (Application.isFocused)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        #endregion
    }
}