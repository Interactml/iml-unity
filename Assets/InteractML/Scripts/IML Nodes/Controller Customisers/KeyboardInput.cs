using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

namespace InteractML.ControllerCustomisers
{
    public class KeyboardInput: IInputType
    {
        private string m_InputName = "Keyboard";
        public string inputName
        {
            get => m_InputName;
            set => m_InputName = "Keyboard";
        }
        public InputHandler InitializeButtonHandler(int buttonNo, IMLTriggerTypes triggerType, string buttonName)
        {
            InputHandler handler = new KeyboardHandler(buttonNo, triggerType, buttonName);
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

