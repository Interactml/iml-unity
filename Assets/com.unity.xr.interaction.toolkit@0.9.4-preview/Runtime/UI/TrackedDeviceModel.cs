using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    public struct TrackedDeviceModel
    {
        const float k_DefaultMaxRaycastDistance = 1000;

        public struct ImplementationData
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
            /// The last raycast done for this model.
            /// </summary>
            public RaycastResult lastFrameRaycast { get; set; }

            /// <summary>
            /// The index within the list of raycast points that the lastFrameRaycast refers to.
            /// </summary>
            public int lastFrameRaycastResultPositionInLine { get; set; }

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
                lastFrameRaycastResultPositionInLine = -1;

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

        internal ImplementationData implementationData { get { return m_ImplementationData; } }

        /// <summary>A unique Id to identify this model from others within the UI system.</summary>
        public int pointerId { get; private set; }
        
        /// <summary>The maximum distance to raycast to check for UI.</summary>
        public float maxRaycastDistance { get; set; }

        /// <summary>Whether or not the model should be selecting UI at this moment. This is the equivalent of left mouse down for a mouse.</summary>
        public bool select
        {
            get
            {
                return m_SelectDown;
            }
            set
            {
                if (m_SelectDown != value)
                {
                    m_SelectDown = value;
                    selectDelta |= value ? ButtonDeltaState.Pressed : ButtonDeltaState.Released;
                    changedThisFrame = true;
                }
            }
        }

        /// <summary>Whether the state of the select option has changed this frame.</summary>
        public ButtonDeltaState selectDelta { get; private set; }

        /// <summary> this to check if this model has meaningfully changed this frame.  This is used by the UI system to avoid excessive work.  Use TrackedDeviceModel.OnFrameFinished to reset.</summary>
        public bool changedThisFrame { get; private set; }

        /// <summary>The 3D, world position of the model in Unity's world.</summary>
        public Vector3 position
        {
            get
            {
                return m_Position;
            }
            set
            {
                if (m_Position != value)
                {
                    m_Position = value;
                    changedThisFrame = true;
                }
            }
        }

        /// <summary>The 3D, world orientation of the model in Unity's world.</summary>
        public Quaternion orientation
        {
            get
            {
                return m_Orientation;
            }
            set
            {
                if (m_Orientation != value)
                {
                    m_Orientation = value;
                    changedThisFrame = true;
                }
            }
        }

        /// <summary>A series of Ray segments used to hit UI.</summary>
        public List<Vector3> raycastPoints
        {
            get
            {
                return m_RaycastPoints;
            }
            set
            {
                changedThisFrame |= m_RaycastPoints.Count != value.Count;
                m_RaycastPoints = value;
            }
        }

        public LayerMask raycastLayerMask
        {
            get
            {
                return m_RaycastLayerMask;
            }
            set
            {
                if(m_RaycastLayerMask != value)
                {
                    changedThisFrame = true;
                    m_RaycastLayerMask = value;
                }
            }
        }

        public TrackedDeviceModel(int pointerId)
        {
            this.pointerId = pointerId;
            maxRaycastDistance = k_DefaultMaxRaycastDistance;

            m_Orientation = Quaternion.identity;
            m_Position = Vector3.zero;
            m_SelectDown = changedThisFrame = false;
            selectDelta = ButtonDeltaState.NoChange;
            m_RaycastPoints = new List<Vector3>();
            m_RaycastLayerMask = Physics.DefaultRaycastLayers;

            m_ImplementationData = new ImplementationData();
            m_ImplementationData.Reset();
        }

        /// <summary>Resets this object back to defaults.</summary>
        /// <param name="resetImplementation">If false, will reset only the external state of the object, and not internal, UI-used variables.  Defaults to true.</param>
        public void Reset(bool resetImplementation = true)
        {
            m_Orientation = Quaternion.identity;
            m_Position = Vector3.zero;
            m_RaycastPoints.Clear();
            m_RaycastLayerMask = Physics.DefaultRaycastLayers;
            m_SelectDown = changedThisFrame = false;
            selectDelta = ButtonDeltaState.NoChange;

            if(resetImplementation)
                m_ImplementationData.Reset();
        }

        /// <summary>To be called at the end of each frame to reset any tracking of changes within the frame.</summary>
        public void OnFrameFinished()
        {
            selectDelta = ButtonDeltaState.NoChange;
            changedThisFrame = false;
        }

        /// <summary>Copies data from this model to the UI Event.</summary>
        /// <param name="eventData">The event that copies the data.</param>
        public void CopyTo(TrackedDeviceEventData eventData)
        {
            eventData.rayPoints = m_RaycastPoints;
            // Demolish the position so we don't trigger any checks from the Graphics Raycaster.
            eventData.position = new Vector2(float.MinValue, float.MinValue);
            eventData.layerMask = m_RaycastLayerMask;

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

        /// <summary>Copies data from the UI Event to this model</summary>
        /// <param name="eventData">The data to copy from.</param>
        public void CopyFrom(TrackedDeviceEventData eventData)
        {
            m_ImplementationData.pointerTarget = eventData.pointerEnter;
            m_ImplementationData.isDragging = eventData.dragging;
            m_ImplementationData.pressedTime = eventData.clickTime;
            m_ImplementationData.pressedPosition = eventData.pressPosition;
            m_ImplementationData.pressedRaycast = eventData.pointerPressRaycast;
            m_ImplementationData.pressedGameObject = eventData.pointerPress;
            m_ImplementationData.pressedGameObjectRaw = eventData.rawPointerPress;
            m_ImplementationData.draggedGameObject = eventData.pointerDrag;
            m_ImplementationData.lastFrameRaycast = eventData.pointerCurrentRaycast;
            m_ImplementationData.lastFrameRaycastResultPositionInLine = eventData.rayHitIndex;

            m_ImplementationData.hoverTargets.Clear();
            m_ImplementationData.hoverTargets.AddRange(eventData.hovered);
        }

        private bool m_SelectDown;
        private Vector3 m_Position;
        private Quaternion m_Orientation;
        private List<Vector3> m_RaycastPoints;
        private LayerMask m_RaycastLayerMask;

        private ImplementationData m_ImplementationData;
    }
}
