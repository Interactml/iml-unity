using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

namespace InteractML.ControllerCustomisers
{
    public class InputSetUp : IMLNode
    {
        // whether this is accessible during the game 
        public bool enableUniversalInterface;
        // whether the interface is active
        public bool activeUniversalInterface;

        // private List<InputDevice> inputDevices = new List<InputDevice>();
        //public string[] inputDevicesArray;
        public IMLInputDevices device;


        public IMLSides trainingHand;
        public IMLSides mlsHand;

        public string[] buttonOptions;

        // which 
        public InputHandler DeleteLast;
        public InputHandler DeleteAll;
        public InputHandler ToggleRecord;
        public InputHandler Train;
        public InputHandler ToggleRun;
        private List<InputHandler> trainingHandlers;
        private List<InputHandler> mlsHandlers;
        private List<InputHandler> allHandlers;


        public string currentMLS;
        public string currentTraining;

        public override void Initialize()
        {
            InstantiateVRButtonHandlers();
            SetButtonNames();
            trainingHandlers = new List<InputHandler>();
            trainingHandlers.Add(DeleteLast);
            trainingHandlers.Add(DeleteAll);
            trainingHandlers.Add(ToggleRecord);
            mlsHandlers = new List<InputHandler>();
            mlsHandlers.Add(Train);
            mlsHandlers.Add(ToggleRun);
            allHandlers = trainingHandlers;
            allHandlers.AddRange(mlsHandlers);
            foreach (InputHandler handler in allHandlers)
            {
                handler.ButtonDown += testDown;
            }
            
            OnInputDeviceChange();
            OnHandChange(trainingHand);
            OnHandChange(mlsHand);
            
        }

        public void UpdateLogic()
        {
            
            foreach (InputHandler handler in allHandlers)
            {
                handler.HandleState();
            }
        }

        

        public void OnInputDeviceChange()
        {
            switch (device)
            {
                case IMLInputDevices.Keyboard:
                    buttonOptions = System.Enum.GetNames(typeof(Keyboard));
                    break;
                case IMLInputDevices.Mouse:
                    buttonOptions = System.Enum.GetNames(typeof(UnityEngine.InputSystem.LowLevel.MouseButton));
                    break;
                case IMLInputDevices.VRControllers:
                    InstantiateVRButtonHandlers();
                    buttonOptions = System.Enum.GetNames(typeof(IMLControllerInputs));
                    break;
                default:
                    buttonOptions = new string[] { "none" };
                    break;
            }

            

            
        }
        /// <summary>
        /// Changes hand type in handler 
        /// </summary>
        /// <param name="side">which side it is being changed to</param>
        public void OnHandChange(IMLSides side)
        {
            List<InputHandler> handlers = new List<InputHandler>();
            if (side == mlsHand)
                handlers = mlsHandlers;
            else
                handlers = trainingHandlers;
            
            foreach (InputHandler handler in handlers)
            {
                VRButtonHandler vrHandler = handler as VRButtonHandler;
                vrHandler.SetController(side);
            }
        }
        /// <summary>
        /// Set the button type in the handler 
        /// </summary>
        /// <param name="handlerName">name of the handler to be set</param>
        /// <param name="button">number from enum in editor</param>
        public void OnButtonChange(string handlerName, int button)
        {
            foreach (InputHandler handler in trainingHandlers)
            {
                if (handlerName == handler.buttonName){
                    handler.SetButtonNo(button);
                }
            }
        }
        public void OnTriggerChange(string handlerName, IMLTriggerTypes triggerT)
        {
            foreach (InputHandler handler in trainingHandlers)
            {
                if (handlerName == handler.buttonName){
                    handler.SetTriggerType(triggerT);
                }
            }
        }
        /// <summary>
        /// Create instances of VR button handlers
        /// </summary>
        private void InstantiateVRButtonHandlers()
        {
            //Debug.Log("here");
            DeleteAll = new VRButtonHandler();
            DeleteLast = new VRButtonHandler();
            ToggleRecord = new VRButtonHandler();
            Train = new VRButtonHandler();
            ToggleRun= new VRButtonHandler();
            
        }
        /// <summary>
        /// Set the names of the handlers used to set button types from editor
        /// </summary>
        private void SetButtonNames()
        {
            DeleteLast.buttonName = "deleteLast";
            DeleteAll.buttonName = "deleteAll";
            ToggleRecord.buttonName = "toggleRecord";
            Train.buttonName = "train";
            ToggleRun.buttonName = "toggleRun";
        }
        public void SubscribeToEvents()
        {
            
        }

        public void UnsubscribeFromEvents()
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

        public bool testDown()
        {
            Debug.Log("down ");
            return true;
        }
        public bool testUp()
        {
            Debug.Log("up ");
            return true;
        }
        public bool testPress()
        {
            Debug.Log("hold ");
            return true;
        }
    }
}

