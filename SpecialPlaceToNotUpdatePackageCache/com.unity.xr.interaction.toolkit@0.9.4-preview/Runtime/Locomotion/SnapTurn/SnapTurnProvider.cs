using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit
{
    /// <summary>
    /// The snap turn provider is a locomotion provider that allows the user to rotate their rig using a specified 2d axis input.
    /// the provider can take input from two different devices (eg: L & R hands). 
    /// </summary>
    public sealed class SnapTurnProvider : LocomotionProvider
    {
        /// <summary>
        /// This is the list of possible valid "InputAxes" that we allow users to read from.
        /// </summary>
        public enum InputAxes
        {
            Primary2DAxis = 0,
            Secondary2DAxis = 1,   
        };

        // Mapping of the above InputAxes to actual common usage values
        static readonly InputFeatureUsage<Vector2>[] m_Vec2UsageList = new InputFeatureUsage<Vector2>[] {
            CommonUsages.primary2DAxis,
            CommonUsages.secondary2DAxis,            
        };

        [SerializeField]
        [Tooltip("The 2D Input Axis on the primary devices that will be used to trigger a snap turn.")]
        InputAxes m_TurnUsage = InputAxes.Primary2DAxis;
        /// <summary>
        /// The 2D Input Axis on the primary device that will be used to trigger a snap turn.
        /// </summary>
        public InputAxes turnUsage { get { return m_TurnUsage; } set { m_TurnUsage = value; } }

        [SerializeField]
        [Tooltip("A list of controllers that allow Snap Turn.  If an XRController is not enabled, or does not have input actions enabled.  Snap Turn will not work.")]
        List<XRController> m_Controllers = new List<XRController>();
        /// <summary>
        /// The XRControllers that allow SnapTurn.  An XRController must be enabled in order to Snap Turn.
        /// </summary>
        public List<XRController> controllers { get { return m_Controllers; } set { m_Controllers = value; } }

        [SerializeField]
        [Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
        float m_TurnAmount = 45.0f;
        /// <summary>
        /// The number of degrees clockwise to rotate when snap turning clockwise.
        /// </summary>
        public float turnAmount {  get { return m_TurnAmount; } set { m_TurnAmount = value; } }

        [SerializeField]
        [Tooltip("The amount of time that the system will wait before starting another snap turn.")]
        float m_DebounceTime = 0.5f;
        /// <summary>
        /// The amount of time that the system will wait before starting another snap turn.
        /// </summary>
        public float debounceTime { get { return m_DebounceTime; } set { m_DebounceTime = value; } }

        [SerializeField]
        [Tooltip("The deadzone that the controller movement will have to be above to trigger a snap turn.")]
        float m_DeadZone = 0.75f;
        /// <summary>
        /// The deadzone that the controller movement will have to be above to trigger a snap turn.
        /// </summary>
        public float deadZone {  get { return m_DeadZone;  } set { m_DeadZone = value; } }

        // state data
        float m_CurrentTurnAmount = 0.0f;
        float m_TimeStarted = 0.0f;

        List<bool> m_ControllersWereActive = new List<bool>();

        private void Update()
        {           
            // wait for a certain amount of time before allowing another turn.
            if (m_TimeStarted > 0.0f && (m_TimeStarted + m_DebounceTime < Time.time))
            {
                m_TimeStarted = 0.0f;
                return;
            }

            if(m_Controllers.Count > 0)
            {
                EnsureControllerDataListSize();

                InputFeatureUsage<Vector2> feature = m_Vec2UsageList[(int)m_TurnUsage];
                for (int i = 0; i < m_Controllers.Count; i++)
                {
                    XRController controller = m_Controllers[i];
                    if (controller != null)
                    {
                        if (controller.enableInputActions && m_ControllersWereActive[i])
                        {
                            InputDevice device = controller.inputDevice;

                            Vector2 currentState;
                            if (device.TryGetFeatureValue(feature, out currentState))
                            {
                                if (currentState.x > deadZone)
                                {
                                    StartTurn(m_TurnAmount);
                                }
                                else if (currentState.x < -deadZone)
                                {
                                    StartTurn(-m_TurnAmount);
                                }
                            }
                        }
                        else //This adds a 1 frame delay when enabling input actions, so that the frame it's enabled doesn't trigger a snap turn.
                        {
                            m_ControllersWereActive[i] = controller.enableInputActions;
                        }
                    }
                }
            }
            
            if (Math.Abs(m_CurrentTurnAmount) > 0.0f && BeginLocomotion())
            {
                var xrRig = system.xrRig;
                if (xrRig != null)
                {
                    xrRig.RotateAroundCameraUsingRigUp(m_CurrentTurnAmount);
                }
                m_CurrentTurnAmount = 0.0f;
                EndLocomotion();
            }
        }

        void EnsureControllerDataListSize()
        {
            if(m_Controllers.Count != m_ControllersWereActive.Count)
            {
                while(m_ControllersWereActive.Count < m_Controllers.Count)
                {
                    m_ControllersWereActive.Add(false);
                }

                while(m_ControllersWereActive.Count < m_Controllers.Count)
                {
                    m_ControllersWereActive.RemoveAt(m_ControllersWereActive.Count - 1);
                }
            }
        }

        internal void FakeStartTurn(bool isLeft)
        {
            StartTurn(isLeft ? -m_TurnAmount : m_TurnAmount);
        }

        private void StartTurn(float amount)
        {
            if (m_TimeStarted != 0.0f) 
                return;

            if (!CanBeginLocomotion())
                return;
           
            m_TimeStarted = Time.time;
            m_CurrentTurnAmount = amount;            
        }
    }
}
