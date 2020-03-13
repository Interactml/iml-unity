using UnityEngine;

namespace FIMSpace.Basics
{
    /// <summary>
    /// FM: Simple component to controll behaviour of camera in free flying mode
    /// </summary>
    public class FBasic_FreeCameraBehaviour : MonoBehaviour
    {
        [Header("> Hold right mouse button to rotate camera <")]
        [Tooltip("How fast camera should fly")]
        public float SpeedMultiplier = 10f;
        [Tooltip("Value of acceleration smoothness")]
        public float AccelerationSmothnessValue = 10f;

        [Tooltip("Value of rotation smoothness")]
        public float RotationSmothnessValue = 10f;

        /// <summary> Just multiplier for rotation </summary>
        public float MouseSensitivity = 5f;

        /// <summary> Variables controlling turbo speed on shift key </summary>
        private float turboModeMultiply = 5f;

        /// <summary> Variable to hold speeds of directions for camera to fly </summary>
        private Vector3 speeds;

        private float ySpeed;

        /// <summary> Holding rotation value for camera to rotate</summary>
        private Vector3 rotation;

        /// <summary> Turbo multiplier for faster movement </summary>
        private float turbo = 1f;

        /// <summary> 
        /// Just initializing few variables 
        /// </summary>
        void Start()
        {
            speeds = Vector3.zero;
            ySpeed = 0f;
            rotation = transform.rotation.eulerAngles;
        }

        void Update()
        {
            // Detecting key movement factors
            float f = Input.GetAxis("Vertical");
            float r = Input.GetAxis("Horizontal");

            float forward = f * Time.smoothDeltaTime * SpeedMultiplier;
            float right = r * Time.smoothDeltaTime * SpeedMultiplier;

            // Smooth change turbo speed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                turbo = Mathf.Lerp(turbo, turboModeMultiply, Time.smoothDeltaTime * 5f);
            }
            else
            {
                turbo = Mathf.Lerp(turbo, 1f, Time.smoothDeltaTime * 5f);
            }

            forward *= turbo;
            right *= turbo;

            // Movement to sides with pressed rmb
            if (Input.GetMouseButton(1))
            {
                rotation.x -= (Input.GetAxis("Mouse Y") * 1f * MouseSensitivity);
                rotation.y += (Input.GetAxis("Mouse X") * 1f * MouseSensitivity);
            }

            // Lerping speed variables for smooth effect
            speeds.z = Mathf.Lerp(speeds.z, forward, Time.smoothDeltaTime * AccelerationSmothnessValue);
            speeds.x = Mathf.Lerp(speeds.x, right, Time.smoothDeltaTime * AccelerationSmothnessValue);

            // Applying translation for current transform orientation
            transform.position += transform.forward * speeds.z;
            transform.position += transform.right * speeds.x;
            transform.position += transform.up * speeds.y;

            // Lerping rotation for smooth effect
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), Time.smoothDeltaTime * RotationSmothnessValue);

            // Going in Up / Down directions in world reference
            if (Input.GetKey(KeyCode.LeftControl))
            {
                ySpeed = Mathf.Lerp(ySpeed, 1f, Time.smoothDeltaTime * AccelerationSmothnessValue);
            }
            else
            if (Input.GetButton("Jump"))
            {
                ySpeed = Mathf.Lerp(ySpeed, -1f, Time.smoothDeltaTime * AccelerationSmothnessValue);
            }
            else
                ySpeed = Mathf.Lerp(ySpeed, 0f, Time.smoothDeltaTime * AccelerationSmothnessValue);

            transform.position += Vector3.down * ySpeed * turbo * Time.smoothDeltaTime * SpeedMultiplier;
        }

        /// <summary> 
        /// Cursor locking stuff 
        /// </summary>
        public void FixedUpdate()
        {
            if (Input.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}