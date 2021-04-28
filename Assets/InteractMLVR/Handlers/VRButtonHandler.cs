﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


namespace InteractML
{
    public class VRButtonHandler : InputHandler
    {
        /// <summary>
        /// public reference to VR button 
        /// </summary>
        public UnityEngine.XR.InputFeatureUsage<bool> button { get => _button; }
        /// <summary>
        /// private reference to button 
        /// </summary>
        [SerializeField]
        private UnityEngine.XR.InputFeatureUsage<bool> _button;
        /// <summary>
        /// float which hold the float output of the button 
        /// </summary>
        [SerializeField]
        private UnityEngine.XR.InputFeatureUsage<float> _buttonFloat;
        /// <summary>
        /// reference to IML button used to set UnityEngine.XR.InutFeatureUsage _button
        /// </summary>
        [SerializeField]
        private IMLControllerInputs imlButton;
        /// <summary>
        /// which controller left / right / both is being used
        /// </summary>
        [SerializeField]
        private IMLSides controllerSide;
        /// <summary>
        /// method to find controllers
        /// </summary>
        [SerializeField]
        private List<UnityEngine.XR.InputDevice> _controllers;
        /// <summary>
        /// controller which is currently being used 
        /// </summary>
        [SerializeField]
        private UnityEngine.XR.InputDevice currentController;

        /// <summary>
        /// event to fire when action happens 
        /// </summary>
        public override event IMLEventDispatcher.IMLEvent ButtonFire;

        public VRButtonHandler(int butNo, IMLSides side, IMLTriggerTypes type, string name)
        {
            this.buttonNo = butNo;
            this.triggerType = type;
            this.buttonName = name;
            SetButtonNo(buttonNo);
            SetButton();
            this.controllerSide = side;
            this.nodeID = "";
            _controllers = new List<UnityEngine.XR.InputDevice>();
            SetController(controllerSide);
        }

        public override void HandleState()
        {
            //Debug.Log(buttonName);
            if (_controllers.Count > 0)
            {

                foreach (UnityEngine.XR.InputDevice controller in _controllers)
                {
                  
                    //Debug.Log("found controllers " + _controllers.Count);
                    bool triggerValue;
                    if (controller.TryGetFeatureValue(_button, out triggerValue) && triggerValue)
                    {
                        float fValue;
                        controller.TryGetFeatureValue(_buttonFloat, out fValue);

                        if (fValue == 0)
                            fValue = 0.6f;

                        if (!previousPress && fValue > 0.5f)
                        {
                            currentController = controller;
                            //Debug.Log("previous press");
                            previousPress = true;
                            if (triggerType == IMLTriggerTypes.Down)
                            {
                                ButtonFire?.Invoke(nodeID);
                                //Debug.Log("down" + controller.characteristics + " " + buttonName);
                            }
                            if (triggerType == IMLTriggerTypes.Hold)
                            {
                                ButtonFire?.Invoke(nodeID);
                                //Debug.Log("hold " + controller.characteristics + " " + buttonName);
                            }
                        } 
                    }
                    else
                    {
                        
                        //Debug.Log("not event " + buttonName + controller.characteristics.ToString());
                        if (previousPress && currentController == controller)
                        {
                            previousPress = false;
                            if (triggerType == IMLTriggerTypes.Up)
                            {
                                ButtonFire?.Invoke(nodeID);
                                //Debug.Log("up" + buttonName);
                            }
                            if (triggerType == IMLTriggerTypes.Hold)
                            {
                                ButtonFire?.Invoke(nodeID);
                                //Debug.Log("hold " + controller.characteristics + " " + buttonName);
                            }
                        }
                    }

                }
            } else
            {
                // Changed from EditorApplication.isPlaying to Application.isPlaying in order to build without errors
                if (Application.isPlaying)
                {
                    SetController(controllerSide);
                }
            }
            
        }

        public void SetController(IMLSides side)
        {
            controllerSide = side;

            if (_controllers != null)
                _controllers.Clear();

            if(controllerSide == IMLSides.Both)
            {
                UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.Controller, _controllers);

            } else if (controllerSide == IMLSides.Left)
            {
                UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.Left, _controllers);
            } else
            {
                UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.Right, _controllers);
            }
        }

        public override void SetButtonNo(int buttonNum)
        {
            buttonNo = buttonNum;
            imlButton = (IMLControllerInputs)buttonNo;
            SetButton();
        }
        public override void SetTriggerType(IMLTriggerTypes triggerT)
        {
            triggerType = triggerT;
        }
        
        public void SetButton()
        {
            switch (imlButton)
            {
                case IMLControllerInputs.Trigger:
                    this._button = UnityEngine.XR.CommonUsages.triggerButton;
                    this._buttonFloat = UnityEngine.XR.CommonUsages.trigger;
                    break;
                case IMLControllerInputs.Grip:
                    this._button = UnityEngine.XR.CommonUsages.gripButton;
                    this._buttonFloat = UnityEngine.XR.CommonUsages.grip;
                    break;
                case IMLControllerInputs.Primary:
                    this._button = UnityEngine.XR.CommonUsages.primaryButton;
                    this._buttonFloat = new UnityEngine.XR.InputFeatureUsage<float>();
                    break;
                case IMLControllerInputs.Secondary:
                   this._button = UnityEngine.XR.CommonUsages.secondaryButton;
                    this._buttonFloat = new UnityEngine.XR.InputFeatureUsage<float>();
                    break;
                default:
                    Debug.Log("none");
                    break;
            }
        }

    }
}
