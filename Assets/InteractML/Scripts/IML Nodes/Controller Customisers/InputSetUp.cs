using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using XNode;
using InteractML.ControllerCustomisers;


namespace InteractML.CustomControllers
{
    [NodeWidth(420)]
    public class InputSetUp : IMLNode
    {
        // whether this is accessible during the game 
        public bool enableUniversalInterface;
        // whether the interface is active
        public bool activeUniversalInterface;

        private bool trainingEnabled = false;

        // private List<InputDevice> inputDevices = new List<InputDevice>();
        //public string[] inputDevicesArray;
        public IMLInputDevices device;

        public IMLSides trainingHand;
        public IMLSides mlsHand;

        public string[] buttonOptions;

        public string mlsID;
        public string trainingID;

        // which 
        //public InputHandler DeleteLast;
        public InputHandler DeleteAll;
        public InputHandler ToggleRecord;
        public InputHandler RecordOne;
        public InputHandler Train;
        public InputHandler ToggleRun;
        private List<InputHandler> trainingHandlers;
        private List<InputHandler> mlsHandlers;
        private List<InputHandler> allHandlers;

        public int deleteLastButtonNo;
        public IMLTriggerTypes deleteLastButtonTT;
        public int deleteAllButtonNo;
        public IMLTriggerTypes deleteAllButtonTT;
        public int toggleRecordButtonNo;
        public IMLTriggerTypes recordOneButtonTT;
        public int recordOneButtonNo;
        public IMLTriggerTypes toggleRecordButtonTT;
        public int trainButtonNo;
        public IMLTriggerTypes trainButtonTT;
        public int toggleRunButtonNo;
        public IMLTriggerTypes toggleRunButtonTT;

        private string deleteAllName = "deleteAll";
        private string deleteLastName = "deleteLast";
        private string toggleRecordName = "toggleRecord";
        private string recordOneName = "recordOne";
        private string trainName = "train";
        private string toggleRunName = "toggleRun";

        public string currentMLS;
        public string currentTraining;

        public string selectedMLS;
        public string selectedTraining;

        public delegate bool UniversalSetUp(bool value);
        public event UniversalSetUp setUpChange;


        public override void Initialize()
        {
            LoadFromFile();
            OnInputDeviceChange();
            trainingHandlers = new List<InputHandler>();
            //trainingHandlers.Add(DeleteLast);
            trainingHandlers.Add(DeleteAll);
            trainingHandlers.Add(ToggleRecord);
            trainingHandlers.Add(RecordOne);
            mlsHandlers = new List<InputHandler>();
            mlsHandlers.Add(Train);
            mlsHandlers.Add(ToggleRun);
            allHandlers = new List<InputHandler>();
            allHandlers.AddRange(mlsHandlers);
            allHandlers.AddRange(trainingHandlers);
            SubscribeToEvents();
        }



        public void UpdateLogic()
        {
            if (trainingEnabled)
            {
                foreach (InputHandler handler in trainingHandlers)
                {
                    handler.HandleState();
                }
            } else 
            {
                foreach (InputHandler handler in mlsHandlers)
                {
                    handler.HandleState();
                }
            }
        }

        //let rest of code know that the 
        public void SetUniversalSetUp()
        {
            setUpChange?.Invoke(enableUniversalInterface);
            SaveToFile();
        }
        public void OnInputDeviceChange()
        {
            switch (device)
            {
                case IMLInputDevices.Keyboard:
                    InstantiateKeyboardButtonHandlers();
                    Debug.Log("here");
                    break;
                /*case IMLInputDevices.Mouse:
                    
                    break;*/
                case IMLInputDevices.VRControllers:
                    InstantiateVRButtonHandlers();
                    break;
                default:
                    Debug.Log("device not set");
                    break;
            }
            buttonOptions = InputHelperMethods.deviceEnumSetUp(device);


        }
        /// <summary>
        /// Changes hand type in handler 
        /// </summary>
        /// <param name="side">which side it is being changed to</param>
        public void OnHandChange(IMLSides side, string group)
        {
            List<InputHandler> handlers = new List<InputHandler>();
            if (group == "mlsHand")
                handlers = mlsHandlers;
            else
                handlers = trainingHandlers;

            foreach (InputHandler handler in handlers)
            {
                VRButtonHandler vrHandler = handler as VRButtonHandler;
                vrHandler.SetController(side);
            }
            SaveToFile();
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
                if (handlerName == handler.buttonName)
                {
                    handler.SetButtonNo(button);
                    Debug.Log(button);
                }
            }
            SaveToFile();
        }
        public void OnTriggerChange(string handlerName, IMLTriggerTypes triggerT)
        {
            foreach (InputHandler handler in trainingHandlers)
            {
                if (handlerName == handler.buttonName)
                {
                    handler.SetTriggerType(triggerT);
                }
            }
            SaveToFile();
        }
        /// <summary>
        /// Create instances of VR button handlers
        /// </summary>
        private void InstantiateVRButtonHandlers()
        {
            DeleteAll = new VRButtonHandler(deleteAllButtonNo, trainingHand, deleteAllButtonTT, deleteAllName);
            //DeleteLast = new VRButtonHandler(deleteLastButtonNo, trainingHand, deleteLastButtonTT, deleteLastName);
            RecordOne = new VRButtonHandler(recordOneButtonNo, trainingHand, recordOneButtonTT, recordOneName);
            ToggleRecord = new VRButtonHandler(toggleRecordButtonNo, trainingHand, toggleRecordButtonTT, toggleRecordName);
            Train = new VRButtonHandler(trainButtonNo, mlsHand, trainButtonTT, trainName);
            ToggleRun = new VRButtonHandler(toggleRunButtonNo, mlsHand, toggleRunButtonTT, toggleRunName);
        }
        private void InstantiateKeyboardButtonHandlers()
        {
            Debug.Log(deleteAllButtonNo);
            DeleteAll = new KeyboardHandler(deleteAllButtonNo, deleteAllButtonTT, deleteAllName);
            //DeleteLast = new KeyboardHandler(deleteLastButtonNo, deleteLastButtonTT, "deleteLast");
            RecordOne = new KeyboardHandler(recordOneButtonNo, recordOneButtonTT, recordOneName);
            ToggleRecord = new KeyboardHandler(toggleRecordButtonNo, toggleRecordButtonTT, toggleRecordName);
            Train = new KeyboardHandler(trainButtonNo, trainButtonTT, trainName);
            ToggleRun = new KeyboardHandler(toggleRunButtonNo, toggleRunButtonTT, toggleRunName);
        }
        private void EnableTraining()
        {
            trainingEnabled = true;
        }
        private void DisableTraining()
        {
            Debug.Log("disable");
            trainingEnabled = false;
        }
        public bool SetTrainingID(string id)
        {
            foreach (InputHandler handler in trainingHandlers)
            {
                handler.nodeID = id;
            }
            return true;
        }
        public bool SetMLSID(string id)
        {
            foreach (InputHandler handler in mlsHandlers)
            {
                handler.nodeID = id;
            }
            return true;
        }

        private void SaveToFile()
        {
            InputSetUpVRSettings setUP = new InputSetUpVRSettings();
            setUP.isEnabled = enableUniversalInterface;
            setUP.device = device;
            /*setUP.deleteLastButtonNo = deleteLastButtonNo;
            setUP.deleteLastButtonTT = deleteLastButtonTT;*/
            setUP.deleteAllButtonNo = deleteAllButtonNo;
            setUP.deleteAllButtonTT = deleteAllButtonTT;
            setUP.toggleRecordButtonNo = toggleRecordButtonNo;
            setUP.toggleRecordButtonTT = toggleRecordButtonTT;
            setUP.recordOneButtonNo = recordOneButtonNo;
            setUP.recordOneButtonTT = recordOneButtonTT;
            setUP.trainButtonNo = trainButtonNo;
            setUP.trainButtonTT = trainButtonTT;
            setUP.toggleRunButtonNo = toggleRunButtonNo;
            setUP.toggleRunButtonTT = toggleRunButtonTT;
            setUP.trainingSide = trainingHand;
            setUP.mlsSide = mlsHand;
            IMLInputSetUpSerialization.SaveInputSettingToDisk(setUP);
        }

        private void LoadFromFile()
        {
            InputSetUpVRSettings setUP = IMLInputSetUpSerialization.LoadInputSettings();
            enableUniversalInterface = setUP.isEnabled;
            device = setUP.device;
            /*deleteLastButtonNo = setUP.deleteLastButtonNo;
            deleteLastButtonTT = setUP.deleteLastButtonTT;*/
            deleteAllButtonNo = setUP.deleteAllButtonNo;
            deleteAllButtonTT = setUP.deleteAllButtonTT;
            recordOneButtonNo = setUP.recordOneButtonNo;
            recordOneButtonTT = setUP.recordOneButtonTT;
            toggleRecordButtonNo = setUP.toggleRecordButtonNo;
            toggleRecordButtonTT = setUP.toggleRecordButtonTT;
            trainButtonNo = setUP.trainButtonNo;
            trainButtonTT = setUP.trainButtonTT;
            toggleRunButtonNo = setUP.toggleRunButtonNo;
            toggleRunButtonTT = setUP.toggleRunButtonTT;
            trainingHand = setUP.trainingSide;
            mlsHand = setUP.mlsSide;

            if (device == IMLInputDevices.None)
            {
                device = IMLInputDevices.Keyboard;
                deleteLastButtonNo = 1;
                deleteLastButtonTT = IMLTriggerTypes.Hold;
                deleteAllButtonNo = 2;
                deleteAllButtonTT = IMLTriggerTypes.Hold;
                toggleRecordButtonNo = 3;
                toggleRecordButtonTT = IMLTriggerTypes.Hold;
                trainButtonNo = 4;
                trainButtonTT = IMLTriggerTypes.Hold;
                toggleRunButtonNo = 5;
                toggleRunButtonTT = IMLTriggerTypes.Hold;
                mlsHand = setUP.mlsSide;
                trainingHand = IMLSides.Left;
                mlsHand = IMLSides.Right;
            }
           
            
        }



        public void SubscribeToEvents()
        {
           // DeleteLast.ButtonFire += IMLEventDispatcher.DeleteLastCallback;
            DeleteAll.ButtonFire += IMLEventDispatcher.DeleteAllCallback;
            RecordOne.ButtonFire += IMLEventDispatcher.RecordOneCallback;
            ToggleRecord.ButtonFire += IMLEventDispatcher.ToggleRecordCallback;
            Train.ButtonFire += IMLEventDispatcher.TrainMLSCallback;
            ToggleRun.ButtonFire += IMLEventDispatcher.ToggleRunCallback;

            IMLEventDispatcher.selectGraph += ActivateInput;
            IMLEventDispatcher.deselectGraph += DeactivateInput;
            IMLEventDispatcher.SetUniversalTrainingID += SetTrainingID;
            IMLEventDispatcher.EnableTraining += EnableTraining;
            IMLEventDispatcher.DisableTraining += DisableTraining;
            IMLEventDispatcher.SetUniversalMLSID += SetMLSID;
        }

        private void ActivateInput(IMLComponent graph)
        {
            activeUniversalInterface = true;
        }
        
        private void DeactivateInput(IMLComponent graph)
        {
            activeUniversalInterface = false;
        }
        

        public void UnsubscribeFromEvents()
        {
            //DeleteLast.ButtonFire -= IMLEventDispatcher.DeleteLastCallback;
            DeleteAll.ButtonFire -= IMLEventDispatcher.DeleteAllCallback;
            ToggleRecord.ButtonFire -= IMLEventDispatcher.ToggleRecordCallback;
            RecordOne.ButtonFire -= IMLEventDispatcher.RecordOneCallback;
            Train.ButtonFire -= IMLEventDispatcher.TrainMLSCallback;
            ToggleRun.ButtonFire -= IMLEventDispatcher.ToggleRunCallback;

            IMLEventDispatcher.selectGraph -= ActivateInput;
            IMLEventDispatcher.deselectGraph -= DeactivateInput;
            IMLEventDispatcher.SetUniversalTrainingID -= SetTrainingID;
            IMLEventDispatcher.SetUniversalMLSID -= SetMLSID;

            IMLEventDispatcher.EnableTraining -= EnableTraining;
            IMLEventDispatcher.DisableTraining -= DisableTraining;
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

       
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}

