using UnityEngine;

using UnityEditor;
using System;
using System.Collections.Generic;
using XNode;
using InteractML.ControllerCustomisers;


namespace InteractML.ControllerCustomisers
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
        // public IMLInputDevices device;//s device list 
        public string[] deviceNames;
        public int deviceNo;
        public IInputType[] devices;
        private IInputType device; 

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

        private string m_deleteAllName = "deleteAll";
        private string m_deleteLastName = "deleteLast";
        private string m_toggleRecordName = "toggleRecord";
        private string m_recordOneName = "recordOne";
        private string m_trainName = "train";
        private string m_toggleRunName = "toggleRun";
        public string deleteAllName { get => m_deleteAllName; }
        public string deleteLastName { get => m_deleteLastName; }
        public string toggleRecordName { get => m_toggleRecordName; }
        public string recordOneName { get => m_recordOneName; }
        public string trainName { get => m_trainName; }
        public string toggleRunName { get => m_toggleRunName; }

        public string currentMLS;
        public string currentTraining;

        public string selectedMLS;
        public string selectedTraining;

        // delegate that adds all devices to menu 
       

        public override void Initialize()
        {
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
            //SubscribeToEvents();
            IMLEventDispatcher.UniversalControlChange?.Invoke(enableUniversalInterface);
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
            IMLEventDispatcher.UniversalControlChange?.Invoke(enableUniversalInterface);
            device.SaveDeviceInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnInputDeviceChange()
        {
            if (device == null)
                device = devices[0];
            //DeleteAll = device.InitializeButtonHandler(deleteAllButtonNo, deleteAllButtonTT, deleteAllName);
        }

        /// <summary>
        /// Set the button type in the handler 
        /// </summary>
        /// <param name="handlerName">name of the handler to be set</param>
        /// <param name="button">number from enum in editor</param>
        public void OnButtonChange(string handlerName, int button)
        {
            if (trainingEnabled)
                device.OnButtonChange(handlerName, button, trainingHandlers);
            else
                device.OnButtonChange(handlerName, button, mlsHandlers);
        }
        /// <summary>
        /// Trigger the button change in the device input setup 
        /// </summary>
        /// <param name="handlerName">name of the button</param>
        /// <param name="triggerT">trigger type to change to</param>
        public void OnTriggerChange(string handlerName, IMLTriggerTypes triggerT)
        {
            /*if (trainingEnabled)
                device.OnTriggerChange(handlerName, triggerT, trainingHandlers);
            else
                device.OnTriggerChange(handlerName, triggerT, mlsHandlers);
            */
        }
        
      
        private void EnableTraining()
        {
            trainingEnabled = true;
        }
        private void DisableTraining()
        {
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

      
        // add all available devices to input setup
        public void AddDevices(List<IInputType> inputTypes)
        {
            devices = inputTypes.ToArray();
            OnInputDeviceChange();
        }


        private void ActivateInput(IMLComponent graph)
        {
            activeUniversalInterface = true;
        }
        
        private void DeactivateInput(IMLComponent graph)
        {
            activeUniversalInterface = false;
        }

        public void SubscribeToEvents()
        {
            //DeleteLast.ButtonFire += IMLEventDispatcher.DeleteLastCallback;
            DeleteAll.ButtonFire += IMLEventDispatcher.DeleteAllExamplesInNodeCallback;
            ToggleRecord.ButtonFire += IMLEventDispatcher.ToggleRecordCallback;
            RecordOne.ButtonFire += IMLEventDispatcher.RecordOneCallback;
            Train.ButtonFire += IMLEventDispatcher.TrainMLSCallback;
            ToggleRun.ButtonFire += IMLEventDispatcher.ToggleRunCallback;

            IMLEventDispatcher.selectGraph += ActivateInput;
            IMLEventDispatcher.deselectGraph += DeactivateInput;
            IMLEventDispatcher.SetUniversalTrainingID += SetTrainingID;
            IMLEventDispatcher.SetUniversalMLSID += SetMLSID;

            IMLEventDispatcher.EnableTraining += EnableTraining;
            IMLEventDispatcher.DisableTraining += DisableTraining;
        }

        public void UnsubscribeFromEvents()
        {
            //DeleteLast.ButtonFire -= IMLEventDispatcher.DeleteLastCallback;
            DeleteAll.ButtonFire -= IMLEventDispatcher.DeleteAllExamplesInNodeCallback;
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

       
        private void OnDestroy()
        {
            IMLEventDispatcher.DestroyIMLGrab?.Invoke();
            //UnsubscribeFromEvents();
        }
    }
}

