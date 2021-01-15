using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    internal struct TouchModel
    {
        internal struct ImplementationData
        {
            /// <summary>
            /// This tracks the current GUI targets being hovered over.  Syncs up to <see cref="PointerEventData.hovered"/>.
            /// </summary>
            public List<GameObject> hoverTargets { get; set; }

            /// <summary>
            ///  Tracks the current enter/exit target being hovered over at any given moment. Syncs up to <see cref="PointerEventData.pointerEnter"/>.
            /// </summary>
            public GameObject pointerTarget { get; set; }

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

                if (hoverTargets == null)
                {
                    hoverTargets = new List<GameObject>();
                }
                else
                {
                    hoverTargets.Clear();
                }
            }
        }

        public int pointerId { get; private set; }

        public TouchPhase selectPhase
        {
            get
            {
                return m_SelectPhase;
            }
            set
            {
                if (m_SelectPhase != value)
                {
                    if (value == TouchPhase.Began)
                        selectDelta |= ButtonDeltaState.Pressed;

                    if (value == TouchPhase.Ended || value == TouchPhase.Canceled)
                        selectDelta |= ButtonDeltaState.Released;

                    m_SelectPhase = value;

                    changedThisFrame = true;
                }
            }
        }

        public ButtonDeltaState selectDelta { get; private set; }

        public bool changedThisFrame { get; private set; }

        /// <summary>
        /// The pixel-space position of the touch object.
        /// </summary>
        public Vector2 position
        {
            get
            {
                return m_Position;
            }
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

        public TouchModel(int pointerId)
        {
            this.pointerId = pointerId;

            m_Position = deltaPosition = Vector2.zero;

            m_SelectPhase = TouchPhase.Canceled;
            changedThisFrame = false;
            selectDelta = ButtonDeltaState.NoChange;

            m_ImplementationData = new ImplementationData();
            m_ImplementationData.Reset();
        }

        public void Reset()
        {
            m_Position = deltaPosition = Vector2.zero;
            changedThisFrame = false;
            selectDelta = ButtonDeltaState.NoChange;
            m_ImplementationData.Reset();
        }

        public void OnFrameFinished()
        {
            deltaPosition = Vector2.zero;
            selectDelta = ButtonDeltaState.NoChange;
            changedThisFrame = false;
        }

        public void CopyTo(PointerEventData eventData)
        {
            eventData.pointerId = pointerId;
            eventData.position = position;
            eventData.delta = ((selectDelta & ButtonDeltaState.Pressed) != 0) ? Vector2.zero : deltaPosition;

            eventData.pointerEnter = m_ImplementationData.pointerTarget;
            eventData.dragging = m_ImplementationData.isDragging;
            eventData.clickTime = m_ImplementationData.pressedTime;
            eventData.pressPosition = m_ImplementationData.pressedPosition;
            eventData.pointerPressRaycast = m_ImplementationData.pressedRaycast;
            eventData.pointerPress = m_ImplementationData.pressedGameObject;
            eventData.rawPointerPress = m_ImplementationData.pressedGameObjectRaw;
            eventData.pointerDrag = m_ImplementationData.draggedGameObject;

            eventData.hovered.Clear();
            eventData.hovered.AddRange(m_ImplementationData.hoverTargets);
        }

        public void CopyFrom(PointerEventData eventData)
        {
            m_ImplementationData.pointerTarget = eventData.pointerEnter;
            m_ImplementationData.isDragging = eventData.dragging;
            m_ImplementationData.pressedTime = eventData.clickTime;
            m_ImplementationData.pressedPosition = eventData.pressPosition;
            m_ImplementationData.pressedRaycast = eventData.pointerPressRaycast;
            m_ImplementationData.pressedGameObject = eventData.pointerPress;
            m_ImplementationData.pressedGameObjectRaw = eventData.rawPointerPress;
            m_ImplementationData.draggedGameObject = eventData.pointerDrag;

            m_ImplementationData.hoverTargets.Clear();
            m_ImplementationData.hoverTargets.AddRange(eventData.hovered);
        }

        private TouchPhase m_SelectPhase;
        private Vector2 m_Position;

        private ImplementationData m_ImplementationData;
    }
}
