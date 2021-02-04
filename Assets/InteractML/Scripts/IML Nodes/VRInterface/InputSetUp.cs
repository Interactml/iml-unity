using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML.ControllerCustomisers
{
    public class InputSetUp : IMLNode
    {
        // whether this is accessible during the game 
        public bool enableUniversalInterface;

        // private List<InputDevice> inputDevices = new List<InputDevice>();
        //public string[] inputDevicesArray;
        public IMLInputDevices device;


        public IMLSides trainingHand;
        public IMLSides mlsHand;

        public string[] buttonOptions;

        public int deleteLastButtonNo;
        public IMLTriggerTypes deleteLastTriggerType;
        public int deleteAllButtonNo;
        public IMLTriggerTypes deleteAllTriggerType;
        public int toggleRecordButtonNo;
        public IMLTriggerTypes toggleRecordTriggerType;
        public int trainButtonNo;
        public IMLTriggerTypes trainTriggerType;
        public int toggleRunButtonNo;
        public IMLTriggerTypes toggleRunTriggerType;

        public string deleteLastButton;
        public string deleteAllButton;
        public string toggleRecordButton;
        public string trainButton;
        public string toggleRecord;
        

        // which 
        private ButtonHandler DeleteLast;
        private ButtonHandler DeleteAll;
        private ButtonHandler ToggleRecord;
        
        private ButtonHandler Train;
        private ButtonHandler ToggleRun;

        public string currentMLS;
        public string currentTraining;

        public override void Initialize()
        {
            
            
        }

        public void OnInputDeviceChange()
        {
            switch (device)
            {
                case IMLInputDevices.Keyboard:
                    buttonOptions = System.Enum.GetNames(typeof(Key));
                    break;
                case IMLInputDevices.Mouse:
                    buttonOptions = System.Enum.GetNames(typeof(UnityEngine.InputSystem.LowLevel.MouseButton));
                    break;
                case IMLInputDevices.VRControllers:
                    buttonOptions = System.Enum.GetNames(typeof(InputHelpers.Button));
                    break;
                default:
                    buttonOptions = new string[] { "none" };
                    break;
            }
        }
        
        

        public void OnChangeButton()
        {

        }
        private void OnDestroy()
        {
            
        }

        public void UpdateInputDevices()
        {

        }

        //method for getting inputs from system come back for future
        /*
         private void InputSystemOnDeviceChange(InputDevice device, InputDeviceChange deviceChange)
        {
            UpdateInputDevices();
        }
        private void UpdateInputList()
         {
             Debug.Log("input set up");
            // if there are already devices in list clear list 
             if (inputDevices.Count > 0)
             {
                 inputDevices.Clear();
             }
             //add devices to the input device 
             for (int i = 0; i < InputSystem.devices.Count; i++)
             {
                 inputDevices.Add(InputSystem.devices[i]);

             }


             inputDevicesArray = new string[inputDevices.Count + 2];
             for (int i = 0; i < inputDevices.Count -1; i++)
             {
                 inputDevicesArray[i] = inputDevices[i].displayName;
             }

            // inputDevicesArray[inputDevices.Count - 2] = "XRControllers";
             //inputDevicesArray[inputDevices.Count - 2] = "XRHands";
             Debug.Log(inputDevices.Count);

         }*/
    }
}

