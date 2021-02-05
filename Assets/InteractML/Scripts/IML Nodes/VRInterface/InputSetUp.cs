using UnityEngine.InputSystem;

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

        public string currentMLS;
        public string currentTraining;

        public override void Initialize()
        {
            InstantiateVRButtonHandlers();
            
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
                    InstantiateVRButtonHandlers();
                    buttonOptions = System.Enum.GetNames(typeof(InputHelpers.Button));
                    break;
                default:
                    buttonOptions = new string[] { "none" };
                    break;
            }

            
        }
        
        private void InstantiateVRButtonHandlers()
        {
            DeleteAll = new VRButtonHandler();
            DeleteLast = new VRButtonHandler();
            ToggleRecord = new VRButtonHandler();
            Train = new VRButtonHandler();
            ToggleRun= new VRButtonHandler();
        }
        public void SubscribeToEvents()
        {
            
        }

        public void UnsubscribeFromEvents()
        {

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

