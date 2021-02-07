using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML
{
    public class VRButtonHandler : InputHandler
    {
        //public InputHelpers.Button button = InputHelpers.Button.Trigger;
        [SerializeField]
        private UnityEngine.XR.InputFeatureUsage<bool> button;
        public IMLControllerInputs triggerButton;
        public IMLSides controllerSide;
        public List<XRController> controllers;
        public List<UnityEngine.XR.InputDevice> _controllers;

        public override event StateChange ButtonDown;
        public override event StateChange ButtonUp;
        public override event StateChange ButtonHold;

        int test;

        public VRButtonHandler()
        {

        }

        public override void HandleState()
        {
            Debug.Log(buttonName + ": " + button.name);
            if (_controllers.Count > 0)
            {
                Debug.Log(_controllers.Count);
                foreach (UnityEngine.XR.InputDevice controller in _controllers)
                {
                    bool triggerValue;
                    if (controller.TryGetFeatureValue(button, out triggerValue) && triggerValue)
                    {
                        if (previousPress != triggerValue)
                        {
                            previousPress = triggerValue;
                            if (triggerValue)
                            {
                                ButtonDown?.Invoke();
                                ButtonHold?.Invoke();
                            }
                            else
                            {
                                ButtonUp?.Invoke();
                                ButtonDown?.Invoke();
                            }
                        }
                    }
                   
                }
            } else
            {
                //SetController(controllerSide);
            }
            
        }

        public void SetController(IMLSides side)
        {
            controllerSide = side;

            if (_controllers != null)
                _controllers.Clear();
            else
                _controllers = new List<UnityEngine.XR.InputDevice>();

            if(controllerSide == IMLSides.Both)
            {
                //Debug.Log("here both");
                UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.Controller, _controllers);

            } else if (controllerSide == IMLSides.Left)
            {
                //Debug.Log("here left");
                UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.Left, _controllers);
            } else
            {
                Debug.Log("here right");
                UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.Right, _controllers);
            }
        }

        public override void SetButtonNo(int buttonNum)
        {
            buttonNo = buttonNum;
            SetButton();
        }
        public override void SetTriggerType(IMLTriggerTypes triggerT)
        {
            triggerType = triggerT;
        }
        
        private void SetButton()
        {
            Debug.Log(buttonNo);
            triggerButton = (IMLControllerInputs)buttonNo;
            switch (triggerButton)
            {
                case IMLControllerInputs.Trigger:
                    button = UnityEngine.XR.CommonUsages.triggerButton;
                    break;
                case IMLControllerInputs.Grip:
                    button = UnityEngine.XR.CommonUsages.gripButton;
                    break;
                case IMLControllerInputs.Primary:
                    button = UnityEngine.XR.CommonUsages.primaryButton;
                    break;
                case IMLControllerInputs.Secondary:
                    button = UnityEngine.XR.CommonUsages.secondaryButton;
                    break;
                default:
                    Debug.Log("none");
                    break;
            }
        }

    }
}
