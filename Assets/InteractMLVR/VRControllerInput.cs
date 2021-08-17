using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

namespace InteractML.ControllerCustomisers
{
    public class VRControllerInput: IInputType
    {
        private string m_InputName = "VR_Controller";
        public string inputName
        {
            get => m_InputName;
            set => m_InputName = "VR_Controller";
        }
        public InputHandler InitializeButtonHandler(int buttonNo, IMLTriggerTypes triggerType, IMLSides side, string buttonName)
        {
            InputHandler handler = new VRButtonHandler(buttonNo, side, triggerType, buttonName);
            return handler;
        }
        public void LoadDeviceInfo()
        {


        }
        public void SaveDeviceInfo()
        {

        }
        public void OnTriggerChange(string handlerName, IMLTriggerTypes triggerT, List<InputHandler> handlers)
        {

        }
        public void OnButtonChange(string handlerName, int button, List<InputHandler> handlers)
        {

        }
    }
}

