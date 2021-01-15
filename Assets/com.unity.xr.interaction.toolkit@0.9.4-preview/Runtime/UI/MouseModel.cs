using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    /// <summary>
    /// A series of flags to determine if a button has been pressed or released since the last time checked.
    /// Useful for identifying press/release events that occur in a single frame or sample.
    /// </summary>
    [Flags]
    public enum ButtonDeltaState
    {
        NoChange = 0,
        Pressed = 1,
        Released = 2,
    }

    /// <summary>
    /// Represents the state of a single mouse button within the uGUI system.  Keeps track of various book-keeping regarding clicks, drags, and presses.
    /// Can be converted to and from PointerEventData for sending into uGUI.
    /// </summary>
    public struct MouseButtonModel
    {
        internal struct ImplementationData
        {
            /// <summary>
            /// Used to cache whether or not the current mouse button is being dragged.  See <see cref="PointerEventData.dragging"/> for more details.
            /// </summary>
            public bool isDragging { get; set; }

            /// <summary>
            /// Used to cache the last time this button was pressed.  See <see cref="PointerEventData.clickTime"/> for more details.
            /// </summary>
            public float pressedTime { get; set; }

            /// <summary>
            /// The position on the screen that this button was last pressed.  In the same scale as <see cref="MouseModel.position"/>, and caches the same value as <see cref="PointerEventData.pressPosition"/>.
            /// </summary>
            public Vector2 pressedPosition { get; set; }

            /// <summary>
            /// The Raycast data from the time it was pressed.  See <see cref="PointerEventData.pointerPressRaycast"/> for more details.
            /// </summary>
            public RaycastResult pressedRaycast { get; set; }

            /// <summary>
            /// The last gameobject pressed on that can handle press or click events.  See <see cref="PointerEventData.pointerPress"/> for more details.
            /// </summary>
            public GameObject pressedGameObject { get; set; }

            /// <summary>
            /// The last gameobject pressed on regardless of whether it can handle events or not.  See <see cref="PointerEventData.rawPointerPress"/> for more details.
            /// </summary>
            public GameObject pressedGameObjectRaw { get; set; }

            /// <summary>
            /// The gameobject currently being dragged if any.  See <see cref="PointerEventData.pointerDrag"/> for more details.
            /// </summary>
            public GameObject draggedGameObject { get; set; }

            /// <summary>
            /// Resets this object to it's default, unused state.
            /// </summary>
            public void Reset()
            {
                isDragging = false;
                pressedTime = 0.0f;
                pressedPosition = Vector2.zero;
                pressedRaycast = new RaycastResult();
                pressedGameObject = pressedGameObjectRaw = draggedGameObject = null;
            }
        }

        /// <summary>
        /// Used to store the current binary state of the button.  When set, will also track the changes between calls of <see cref="OnFrameFinished"/> in <see cref="lastFrameDelta"/>.
        /// </summary>
        public bool isDown
        {
            get
            {
                return m_IsDown;
            }
            set
            {
                if (m_IsDown != value)
                {
                    m_IsDown = value;
                    lastFrameDelta |= value ? ButtonDeltaState.Pressed : ButtonDeltaState.Released;
                }
            }
        }

        /// <summary>
        /// A set of flags to identify the changes that have occured between calls of <see cref="OnFrameFinished"/>.
        /// </summary>
        internal ButtonDeltaState lastFrameDelta { get; private set; }

        /// <summary>
        /// Set's this object to it's default, unused state.
        /// </summary>
        public void Reset()
        {
            lastFrameDelta = ButtonDeltaState.NoChange;
            m_IsDown = false;

            m_ImplementationData.Reset();
        }

        /// <summary>
        /// Call this on each frame in order to reset properties that detect whether or not a certain condition was met this frame.
        /// </summary>
        public void OnFrameFinished()
        {
            lastFrameDelta = ButtonDeltaState.NoChange;
        }

        /// <summary>
        /// Fills a <see cref="PointerEventData"/> with this mouse button's internally cached values.
        /// </summary>
        /// <param name="eventData">These objects are used to send data through the uGUI system.</param>
        public void CopyTo(PointerEventData eventData)
        {
            eventData.dragging = m_ImplementationData.isDragging;
            eventData.clickTime = m_ImplementationData.pressedTime;
            eventData.pressPosition = m_ImplementationData.pressedPosition;
            eventData.pointerPressRaycast = m_ImplementationData.pressedRaycast;
            eventData.pointerPress = m_ImplementationData.pressedGameObject;
            eventData.rawPointerPress = m_ImplementationData.pressedGameObjectRaw;
            eventData.pointerDrag = m_ImplementationData.draggedGameObject;
        }

        /// <summary>
        /// Fills this object with the values from a <see cref="PointerEventData"/>.
        /// </summary>
        /// <param name="eventData">These objects are used to send data through the uGUI system.</param>
        public void CopyFrom(PointerEventData eventData)
        {
            m_ImplementationData.isDragging = eventData.dragging;
            m_ImplementationData.pressedTime = eventData.clickTime;
            m_ImplementationData.pressedPosition = eventData.pressPosition;
            m_ImplementationData.pressedRaycast = eventData.pointerPressRaycast;
            m_ImplementationData.pressedGameObject = eventData.pointerPress;
            m_ImplementationData.pressedGameObjectRaw = eventData.rawPointerPress;
            m_ImplementationData.draggedGameObject = eventData.pointerDrag;
        }

        private bool m_IsDown;
        private ImplementationData m_ImplementationData;
    }

    internal struct MouseModel
    {
        internal struct InternalData
        {
            /// <summary>
            /// This tracks the current GUI targets being hovered over.  Syncs up to <see cref="PointerEventData.hovered"/>.
            /// </summary>
            public List<GameObject> hoverTargets { get; set; }

            /// <summary>
            ///  Tracks the current enter/exit target being hovered over at any given moment. Syncs up to <see cref="PointerEventData.pointerEnter"/>.
            /// </summary>
            public GameObject pointerTarget { get; set; }

            public void Reset()
            {
                pointerTarget = null;
                hoverTargets = new List<GameObject>();
            }
        }

        /// <summary>
        /// An Id representing a unique pointer.  See <see cref="UnityEngine.Experimental.Input.Pointer.pointerId"/> for more details.
        /// </summary>
        public int pointerId { get; private set; }

        /// <summary>
        /// A flag representing whether any mouse data has changed this frame, meaning that events should be processed.
        /// </summary>
        /// <remarks>
        /// This only checks for changes in mouse state (<see cref="position"/>, <see cref="leftButton"/>, <see cref="rightButton"/>, <see cref="middleButton"/>, or <see cref="scrollPosition"/>).
        /// </remarks>
        public bool changedThisFrame { get; private set; }

        public Vector2 position
        {
            get { return m_Position; }
            set
            {
                if (m_Position != value)
                {
                    deltaPosition = value - m_Position;
                    m_Position = value;
                    changedThisFrame = true;
                }
            }
        }

        /// <summary>
        /// The pixel-space change in <see cref="position"/> since the last call to <see cref="OnFrameFinished"/>.
        /// </summary>
        public Vector2 deltaPosition { get; private set; }

        public Vector2 scrollPosition
        {
            get { return m_ScrollPosition; }
            set
            {
                if (m_ScrollPosition != value)
                {
                    changedThisFrame = true;
                    scrollDelta = value - m_ScrollPosition;
                    m_ScrollPosition = value;
                }
            }
        }

        /// <summary>
        /// The change in <see cref="scrollPosition"/> since the last call to <see cref="OnFrameFinished"/>.
        /// </summary>
        public Vector2 scrollDelta { get; private set; }


        /// <summary>
        /// Cached data and button state representing a left mouse button on a mouse.  Used by uGUI to keep track of persistent click, press, and drag states.
        /// </summary>
        public MouseButtonModel leftButton
        {
            get
            {
                return m_LeftButton;
            }
            set
            {
                changedThisFrame |= (value.lastFrameDelta != ButtonDeltaState.NoChange);
                m_LeftButton = value;
            }
        }

        /// <summary>
        /// Shorthand to set the pressed state of the left mouse button.
        /// </summary>
        public bool leftButtonPressed
        {
            set
            {
                changedThisFrame |= m_LeftButton.isDown != value;
                m_LeftButton.isDown = value;
            }
        }

        /// <summary>
        /// Cached data and button state representing a right mouse button on a mouse.  Used by uGUI to keep track of persistent click, press, and drag states.
        /// </summary>
        public MouseButtonModel rightButton
        {
            get
            {
                return m_RightButton;
            }
            set
            {
                changedThisFrame |= (value.lastFrameDelta != ButtonDeltaState.NoChange);
                m_RightButton = value;
            }
        }

        /// <summary>
        /// Shorthand to set the pressed state of the right mouse button.
        /// </summary>
        public bool rightButtonPressed
        {
            set
            {
                changedThisFrame |= m_RightButton.isDown != value;
                m_RightButton.isDown = value;
            }
        }

        /// <summary>
        /// Cached data and button state representing a middle mouse button on a mouse.  Used by uGUI to keep track of persistent click, press, and drag states.
        /// </summary>
        public MouseButtonModel middleButton
        {
            get
            {
                return m_MiddleButton;
            }
            set
            {
                changedThisFrame |= (value.lastFrameDelta != ButtonDeltaState.NoChange);
                m_MiddleButton = value;
            }
        }

        /// <summary>
        /// Shorthand to set the pressed state of the middle mouse button.
        /// </summary>
        public bool middleButtonPressed
        {
            set
            {
                changedThisFrame |= m_MiddleButton.isDown != value;
                m_MiddleButton.isDown = value;
            }
        }

        public MouseModel(int pointerId)
        {
            this.pointerId = pointerId;
            changedThisFrame = false;
            m_Position = deltaPosition = m_ScrollPosition = scrollDelta = Vector2.zero;

            m_LeftButton = new MouseButtonModel();
            m_RightButton = new MouseButtonModel();
            m_MiddleButton = new MouseButtonModel();
            m_LeftButton.Reset();
            m_RightButton.Reset();
            m_MiddleButton.Reset();

            m_InternalData = new InternalData();
            m_InternalData.Reset();
            m_InternalData.pointerTarget = null;
            m_InternalData.hoverTargets = new List<GameObject>();
        }

        /// <summary>
        /// Call this at the end of polling for per-frame changes.  This resets delta values, such as <see cref="deltaPosition"/>, <see cref="scrollDelta"/>, and <see cref="MouseButtonModel.lastFrameDelta"/>.
        /// </summary>
        public void OnFrameFinished()
        {
            changedThisFrame = false;
            deltaPosition = scrollDelta = Vector2.zero;
            m_LeftButton.OnFrameFinished();
            m_RightButton.OnFrameFinished();
            m_MiddleButton.OnFrameFinished();
        }

        public void CopyTo(PointerEventData eventData)
        {
            eventData.pointerId = pointerId;
            eventData.position = position;
            eventData.delta = deltaPosition;
            eventData.scrollDelta = scrollDelta;

            eventData.pointerEnter = m_InternalData.pointerTarget;
            eventData.hovered.Clear();
            eventData.hovered.AddRange(m_InternalData.hoverTargets);

            // This is unset in legacy systems and can safely assumed to stay true.
            eventData.useDragThreshold = true;
        }

        public void CopyFrom(PointerEventData eventData)
        {
            var hoverTargets = m_InternalData.hoverTargets;
            m_InternalData.hoverTargets.Clear();
            m_InternalData.hoverTargets.AddRange(eventData.hovered);
            m_InternalData.hoverTargets = hoverTargets;
            m_InternalData.pointerTarget = eventData.pointerEnter;
        }

        private Vector2 m_Position;
        private Vector2 m_ScrollPosition;
        private MouseButtonModel m_LeftButton;
        private MouseButtonModel m_RightButton;
        private MouseButtonModel m_MiddleButton;

        private InternalData m_InternalData;
    }
}
