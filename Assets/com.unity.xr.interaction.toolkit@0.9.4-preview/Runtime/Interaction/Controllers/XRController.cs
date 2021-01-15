using System;
using UnityEngine;
#if LIH_PRESENT
using UnityEngine.Experimental.XR.Interaction;
#endif

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// XRController MonoBehaviour that interprets InputSystem events into
    /// XR Interaction Interactor position, rotation and interaction states.
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("XR/XR Controller")]
    public class XRController : MonoBehaviour
    {
        /// <summary>
        /// The update type being used by the tracked pose driver
        /// </summary>
        public enum UpdateType
        {
            /// <summary>
            /// Sample input at both update, and directly before rendering. For smooth head pose tracking,
            /// we recommend using this value as it will provide the lowest input latency for the device.
            /// This is the default value for the UpdateType option
            /// </summary>
            UpdateAndBeforeRender,
            /// <summary>
            /// Only sample input during the update phase of the frame.
            /// </summary>
            Update,
            /// <summary>
            /// Only sample input directly before rendering
            /// </summary>
            BeforeRender,
        }
        [Header("Tracking Configuration")]

        [SerializeField]
        [Tooltip("The time within the frame that the XRController will sample input.")]
        UpdateType m_UpdateTrackingType = UpdateType.UpdateAndBeforeRender;
        /// <summary>
        /// The update type being used by the tracked pose driver
        /// </summary>
        public UpdateType updateTrackingType
        {
            get { return m_UpdateTrackingType; }
            set { m_UpdateTrackingType = value; }
        }

        bool m_EnableInputTracking = true;
        /// <summary>Gets or sets if input is enabled for this controller.</summary>
        public bool enableInputTracking
        {
            get { return m_EnableInputTracking; }
            set { m_EnableInputTracking = value; }
        }


        [Header("Configuration")]

        [SerializeField, Tooltip("Used to disable an input state changing in the interactor. useful for swapping to a different interactor on the same object")]
        bool m_EnableInputActions = true;
        public bool enableInputActions { get { return m_EnableInputActions; } set { m_EnableInputActions = value; } }

#if LIH_PRESENT
        [SerializeField, Tooltip("Pose provider used to provide tracking data separate from the XR Node")]
        BasePoseProvider m_PoseProvider;
        public BasePoseProvider poseProvider { get { return m_PoseProvider; } set { m_PoseProvider = value; } }
#endif

        [SerializeField]
        [Tooltip("Gets or sets the XRNode for this controller.")]
        XRNode m_ControllerNode;
        /// <summary>Gets or sets the XRNode for this controller.</summary>
        public XRNode controllerNode { get { return m_ControllerNode; } set { m_ControllerNode = value; } }

        [SerializeField]
        [Tooltip("The input usage that triggers a select, activate or uiInteraction action")]
        InputHelpers.Button m_SelectUsage = InputHelpers.Button.Grip;
        /// <summary>Gets or sets the usage to use for detecting selection.</summary>
        public InputHelpers.Button selectUsage { get { return m_SelectUsage; } set { m_SelectUsage = value; } }

        [SerializeField]
        [Tooltip("Gets or sets the usage to use for detecting activation.")]
        InputHelpers.Button m_ActivateUsage = InputHelpers.Button.Trigger;
        /// <summary>Gets or sets the usage to use for detecting activation.</summary>
        public InputHelpers.Button activateUsage { get { return m_ActivateUsage; } set { m_ActivateUsage = value; } }

        [SerializeField]
        [Tooltip("Gets or sets the usage to use for detecting a UI press.")]
        InputHelpers.Button m_UIPressUsage = InputHelpers.Button.Trigger;
        /// <summary>Gets or sets the usage to use for detecting a UI press.</summary>
        public InputHelpers.Button uiPressUsage { get { return m_UIPressUsage; } set { m_UIPressUsage = value; } }

        [SerializeField]
        [Tooltip("Gets or sets the the amount the axis needs to be pressed to trigger an interaction event.")]
        float m_AxisToPressThreshold = 0.1f;
        /// <summary>Gets or sets the the amount the axis needs to be pressed to trigger an interaction event.</summary>
        public float axisToPressThreshold { get { return m_AxisToPressThreshold; } set { m_AxisToPressThreshold = value; } }

        [Header("Model")]

        [SerializeField]
        [Tooltip("Gets or sets the model prefab to show for this controller.")]
        Transform m_ModelPrefab;
        /// <summary>Gets or sets the model prefab to show for this controller.</summary>
        public Transform modelPrefab { get { return m_ModelPrefab; } set { m_ModelPrefab = value; } }

        [SerializeField]
        [Tooltip("Gets or sets the model transform that is used as the parent for the controller model.")]
        Transform m_ModelTransform;
        /// <summary>Gets or sets the model transform that is used as the parent for the controller model.
        /// Note: setting this will not automatically destroy the previous model transform object.
        /// </summary>
        public Transform modelTransform { get { return m_ModelTransform; } }

        [SerializeField]
        [Tooltip("Gets or sets whether this model animates in response to interaction events.")]
        bool m_AnimateModel;
        /// <summary>Gets or sets whether this model animates in response to interaction events.</summary>
        public bool animateModel { get { return m_AnimateModel; } set { m_AnimateModel = value; } }

        [SerializeField]
        [Tooltip("Gets or sets the animation transition to enable when selecting.")]
        string m_ModelSelectTransition;
        /// <summary>Gets or sets the animation transition to enable when selecting.</summary>
        public string modelSelectTransition { get { return m_ModelSelectTransition; } set { m_ModelSelectTransition = value; } }

        [SerializeField]
        [Tooltip("Gets or sets the animation transition to enable when de-selecting.")]
        string m_ModelDeSelectTransition;
        /// <summary>Gets or sets the animation transition to enable when de-selecting.</summary>
        public string modelDeSelectTransition { get { return m_ModelDeSelectTransition; } set { m_ModelDeSelectTransition = value; } }

        /// <summary>
        /// InteractionState type to hold current state for a given interaction.
        /// </summary>
        internal struct InteractionState
        {
            /// <summary>This field is true if it is is currently on.</summary>
            public bool active;
            /// <summary>This field is true if the interaction state was activated this frame.</summary>
            public bool activatedThisFrame;
            /// <summary>This field is true if the interaction state was de-activated this frame.</summary>
            public bool deActivatedThisFrame;
        }

        internal enum InteractionTypes { select, activate, uiPress };
        InteractionState m_SelectInteractionState;
        InteractionState m_ActivateInteractionState;
        InteractionState m_UIPressInteractionState;

        /// <summary>Gets the current select interaction state.</summary>
        internal InteractionState selectInteractionState { get { return m_SelectInteractionState; } }
        /// <summary>Gets the current activate interaction state.</summary>
        internal InteractionState activateInteractionState { get { return m_ActivateInteractionState; } }
        /// <summary>Gets the current ui press interaction state.</summary>
        internal InteractionState uiPressInteractionState { get { return m_UIPressInteractionState; } }

        ////MODIFIED BY TONY!
        /// <summary>Gets the InputDevice being used to read data from.</summary>
        public InputDeviceWrapper inputDevice
        {
            get
            {
                return m_InputDevice.isValid ? m_InputDevice : (m_InputDevice = new InputDeviceWrapper(controllerNode));
            }
        }
        private InputDeviceWrapper m_InputDevice;

        // Flag to indicate that setup should be (re)performed on Update.
        bool m_PerformSetup = true;

        GameObject m_ModelGO;
        bool m_HideControllerModel = false;

        /// <summary>Gets or sets whether the controller model should be hidden.</summary>
        public bool hideControllerModel
        {
            get { return m_HideControllerModel; }
            set
            {
                m_HideControllerModel = value;
                if (m_ModelGO)
                    m_ModelGO.SetActive(!m_HideControllerModel);
            }
        }

        protected virtual void OnEnable()
        {
            Application.onBeforeRender += OnBeforeRender;
        }

        protected virtual void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
        }

        protected virtual void OnBeforeRender()
        {
            if (enableInputTracking &&
               (m_UpdateTrackingType == UpdateType.BeforeRender ||
                m_UpdateTrackingType == UpdateType.UpdateAndBeforeRender))
            {
                UpdateTrackingInput();
            }
        }

        protected virtual void Awake()
        {
            // create empty model transform if none specified
            if (m_ModelTransform == null)
            {
                var modelGO = new GameObject(string.Format("[{0}] Model", gameObject.name));
                if (modelGO != null)
                {
                    m_ModelTransform = modelGO.transform;
                    m_ModelTransform.SetParent(transform);
                    modelGO.transform.localPosition = Vector3.zero;
                    modelGO.transform.localRotation = Quaternion.identity;
                }
            }
        }

        void PerformSetup()
        {
            SetupModel();
        }

        void SetupModel()
        {
            if (m_ModelGO)
                Destroy(m_ModelGO);
            var model = m_ModelPrefab;
            if (model != null)
            {
                m_ModelGO = Instantiate(model).gameObject;
                m_ModelGO.transform.parent = m_ModelTransform;
                m_ModelGO.transform.localPosition = new Vector3(0f, 0f, 0f);
                m_ModelGO.transform.localRotation = Quaternion.identity;
                m_ModelGO.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                m_ModelGO.transform.gameObject.SetActive(true);
            }
        }

        bool ShouldUpdateTrackingInput()
        {
            return enableInputTracking &&
                (m_UpdateTrackingType == UpdateType.Update ||
                m_UpdateTrackingType == UpdateType.UpdateAndBeforeRender);
        }

        void Update()
        {
            if (m_PerformSetup)
            {
                PerformSetup();
                m_PerformSetup = false;
            }

            if (enableInputTracking &&
                (m_UpdateTrackingType == UpdateType.Update ||
                m_UpdateTrackingType == UpdateType.UpdateAndBeforeRender))
            {
                UpdateTrackingInput();

            }

            if (enableInputActions)
            {
                UpdateInput();
            }
        }

        protected void UpdateTrackingInput()
        {
            Vector3 devicePosition = new Vector3();
            Quaternion deviceRotation = new Quaternion();

#if LIH_PRESENT_V1API
            if (m_PoseProvider != null)
            {
                Pose poseProviderPose = new Pose();
                if(m_PoseProvider.TryGetPoseFromProvider(out poseProviderPose))
                {
                    transform.localPosition = poseProviderPose.position;
                    transform.localRotation = poseProviderPose.rotation;
                }
            }
            else
#elif LIH_PRESENT_V2API
            if (m_PoseProvider != null)
            {
                Pose poseProviderPose = new Pose();
                var retFlags = m_PoseProvider.GetPoseFromProvider(out poseProviderPose);
                if ((retFlags & SpatialTracking.PoseDataFlags.Position) > 0)
                {
                    transform.localPosition = poseProviderPose.position;
                }
                if ((retFlags & SpatialTracking.PoseDataFlags.Rotation) > 0)
                {
                    transform.localRotation = poseProviderPose.rotation;
                }
            }
            else
#endif
            {
                if (inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition))
                    transform.localPosition = devicePosition;

                if (inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out deviceRotation))
                    transform.localRotation = deviceRotation;
            }
        }

        void UpdateInput()
        {
            // clear state for selection, activation and press state dependent on this frame
            m_SelectInteractionState.activatedThisFrame = m_SelectInteractionState.deActivatedThisFrame = false;
            m_ActivateInteractionState.activatedThisFrame = m_ActivateInteractionState.deActivatedThisFrame = false;
            m_UIPressInteractionState.activatedThisFrame = m_UIPressInteractionState.deActivatedThisFrame = false;

            HandleInteractionAction(controllerNode, m_SelectUsage, ref m_SelectInteractionState);
            HandleInteractionAction(controllerNode, m_ActivateUsage, ref m_ActivateInteractionState);
            HandleInteractionAction(controllerNode, m_UIPressUsage, ref m_UIPressInteractionState);

            UpdateControllerModelAnimation();
        }

        void HandleInteractionAction(XRNode node, InputHelpers.Button button, ref InteractionState interactionState)
        {
            bool pressed = false;
            inputDevice.IsPressed(button, out pressed, m_AxisToPressThreshold);

            if (pressed)
            {
                if (!interactionState.active)
                {
                    interactionState.activatedThisFrame = true;
                    interactionState.active = true;
                }
            }
            else
            {
                if (interactionState.active)
                {
                    interactionState.deActivatedThisFrame = true;
                    interactionState.active = false;
                }
            }
        }

        // Override the XRController's current interaction state (used for interaction state playback)
        internal void UpdateInteractionType(InteractionTypes interactionStateType, bool isInteractionStateOn)
        {
            switch (interactionStateType)
            {
                case InteractionTypes.select:
                    UpdateInteractionState(ref m_SelectInteractionState, isInteractionStateOn);
                    break;
                case InteractionTypes.activate:
                    UpdateInteractionState(ref m_ActivateInteractionState, isInteractionStateOn);
                    break;
                case InteractionTypes.uiPress:
                    UpdateInteractionState(ref m_UIPressInteractionState, isInteractionStateOn);
                    break;
            }
        }

        static void UpdateInteractionState(ref InteractionState interactionState, bool isInteractionStateOn)
        {
            bool previousActive = interactionState.active;
            interactionState.active = isInteractionStateOn;

            if (interactionState.active && !previousActive)
                interactionState.activatedThisFrame = true;
            else if (!interactionState.active && previousActive)
                interactionState.deActivatedThisFrame = true;
        }

        // Override the XRController's controller model's animation (if the prefab contains an animator)
        internal void UpdateControllerModelAnimation()
        {
            if (m_ModelGO && m_AnimateModel)
            {
                Animator animator = m_ModelGO.GetComponent<Animator>();
                if (animator)
                {
                    if (m_SelectInteractionState.activatedThisFrame)
                        animator.SetTrigger(modelSelectTransition);
                    else if (m_SelectInteractionState.deActivatedThisFrame)
                        animator.SetTrigger(modelDeSelectTransition);
                }
            }
        }

        // Override the XRController's current position and rotation (used for interaction state playback)
        internal void UpdateControllerPose(Vector3 position, Quaternion rotation)
        {
            transform.localPosition = position;
            transform.localRotation = rotation;
        }

        /// <summary>Play a haptic impulse on the controller if one is available</summary>
        /// <param name="amplitude">Amplitude (from 0.0 to 1.0) to play impulse at.</param>
        /// <param name="duration">Duration (in seconds) to play haptic impulse.</param>
        public bool SendHapticImpulse(float amplitude, float duration)
        {
            HapticCapabilities capabilities;
            if (inputDevice.TryGetHapticCapabilities(out capabilities) &&
                capabilities.supportsImpulse)
            {
                return inputDevice.SendHapticImpulse(0, amplitude, duration);
            }
            return false;
        }
    }
}