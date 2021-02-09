﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEditor;
using InteractML.ControllerCustomisers;

namespace InteractML
{
    public class KeyboardHandler : InputHandler
    {
        //public InputHelpers.Button button = InputHelpers.Button.Trigger;
        public UnityEngine.KeyCode button { get => _button; }
        [SerializeField]
        private UnityEngine.KeyCode _button;

        private string[] keyOptions;
        //[SerializeField]
        //private IMLControllerInputs imlButton;
        //[SerializeField]
        //private IMLSides controllerSide;

        public override event StateChange ButtonFire;

        public KeyboardHandler(int butNo, IMLTriggerTypes type, string name)
        {
            this.buttonNo = butNo;
            this.triggerType = type;
            this.buttonName = name;
            keyOptions = InputHelperMethods.deviceEnumSetUp(IMLInputDevices.Keyboard);
            SetButtonNo(buttonNo);
        }
        /// <summary>
        /// Handle button state fires events
        /// </summary>
        public override void HandleState()
        {
            Debug.Log(_button);
            
            
            // if the button is held and the trigger type is hold
            if (Input.GetKey(KeyCode.Q) && triggerType == IMLTriggerTypes.Hold)
            {
                // if it hasn't been pressed previously
                if (!previousPress)
                {
                    Debug.Log("hold");
                    // fire event 
                    ButtonFire?.Invoke();
                    // set previous press to true
                    previousPress = true;
                }
            }else
            {
                // if it was previously pressed
                if (previousPress)
                {
                    Debug.Log("hold");
                    // fire 
                    ButtonFire?.Invoke();
                    previousPress = false;
                }
            }
            // if button key is down and trigger type is down 
            if (Input.GetKeyDown(_button) && triggerType == IMLTriggerTypes.Down)
            {
                Debug.Log("down");
                ButtonFire?.Invoke();
            }
            // if button key is up and trigger type is up
            if (Input.GetKeyUp(_button) && triggerType == IMLTriggerTypes.Up)
            {
                Debug.Log("up");
                ButtonFire?.Invoke();
            }
            
        }

        /// <summary>
        /// Set the button from the buttonNo called from InputSetUP and keyboard node 
        /// </summary>
        /// <param name="buttonNum">buttonNo of the keyboard enum</param>
        public override void SetButtonNo(int buttonNum)
        {
            string button = keyOptions[buttonNum];
            _button = (KeyCode)System.Enum.Parse(typeof(KeyCode), button);
        }


        /// <summary>
        /// Set the type of trigger up, down, hold called from inputsetup and keyboard node
        /// </summary>
        /// <param name="triggerT">type of trigger</param>
        public override void SetTriggerType(IMLTriggerTypes triggerT)
        {
            triggerType = triggerT;
        }
        
      

    }
}
