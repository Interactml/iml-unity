using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// Represents the state of a joystick in the uGUI system. Keeps track of various book-keeping regarding UI selection, and move and button states.
    /// </summary>
    internal struct JoystickModel
    {
        public struct ImplementationData
        {
            /// <summary>
            /// Bookkeeping values for uGUI that tracks the number of sequential move commands in the same direction that have been sent.  Used to handle proper repeat timing.
            /// </summary>
            public int consecutiveMoveCount { get; set; }

            /// <summary>
            /// Bookkeeping values for uGUI that tracks the direction of the last move command.  Used to handle proper repeat timing.
            /// </summary>
            public MoveDirection lastMoveDirection { get; set; }

            /// <summary>
            /// Bookkeeping values for uGUI that tracks the last time a move command was sent.  Used to handle proper repeat timing.
            /// </summary>
            public float lastMoveTime { get; set; }

            /// <summary>
            /// Resets this object to it's default, unused state.
            /// </summary>
            public void Reset()
            {
                consecutiveMoveCount = 0;
                lastMoveTime = 0.0f;
                lastMoveDirection = MoveDirection.None;
            }
        }

        /// <summary>
        /// A 2D Vector that represents a UI Selection movement command.  Think moving up and down in options menus or highlighting options.
        /// </summary>
        public Vector2 move { get; set; }

        /// <summary>
        /// Tracks the current state of the submit or 'move forward' button.  Setting this also updates the <see cref="submitButtonDelta"/> to track if a press or release occurred in the frame.
        /// </summary>
        public bool submitButtonDown
        {
            get
            {
                return m_SubmitButtonDown;
            }
            set
            {
                if (m_SubmitButtonDown != value)
                {
                    submitButtonDelta = value ? ButtonDeltaState.Pressed : ButtonDeltaState.Released;
                    m_SubmitButtonDown = value;
                }
            }
        }

        /// <summary>
        /// Tracks the changes in <see cref="submitButtonDown"/> between calls to <see cref="OnFrameFinished"/>
        /// </summary>
        internal ButtonDeltaState submitButtonDelta { get; private set; }

        /// <summary>
        /// Tracks the current state of the submit or 'move backward' button.  Setting this also updates the <see cref="cancelButtonDelta"/> to track if a press or release occurred in the frame.
        /// </summary>
        public bool cancelButtonDown
        {
            get
            {
                return m_CancelButtonDown;
            }
            set
            {
                if (cancelButtonDown != value)
                {
                    cancelButtonDelta = value ? ButtonDeltaState.Pressed : ButtonDeltaState.Released;
                    m_CancelButtonDown = value;
                }
            }
        }

        /// <summary>
        /// Tracks the changes in <see cref="cancelButtonDown"/> between calls to <see cref="OnFrameFinished"/>
        /// </summary>
        internal ButtonDeltaState cancelButtonDelta { get; private set; }

        /// <summary>
        /// Internal bookkeeping data used by the uGUI system.
        /// </summary>
        internal ImplementationData implementationData { get; set; }

        /// <summary>
        /// Resets this object to it's default, unused state.
        /// </summary>
        public void Reset()
        {
            move = Vector2.zero;
            m_SubmitButtonDown = m_CancelButtonDown = false;
            submitButtonDelta = cancelButtonDelta = ButtonDeltaState.NoChange;

            implementationData.Reset();
        }

        /// <summary>
        /// Call this at the end of polling for per-frame changes.  This resets delta values, such as <see cref="submitButtonDelta"/> and <see cref="cancelButtonDelta"/>.
        /// </summary>
        public void OnFrameFinished()
        {
            submitButtonDelta = ButtonDeltaState.NoChange;
            cancelButtonDelta = ButtonDeltaState.NoChange;
        }

        private bool m_SubmitButtonDown;
        private bool m_CancelButtonDown;
    }
}
