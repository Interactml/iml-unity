using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

namespace InteractML.ControllerCustomisers
{
    public interface IInputType
    {
        public string inputName
        {
            get;
            set;
        }

       public void LoadDeviceInfo();
        public void SaveDeviceInfo();
        public void OnTriggerChange(string handlerName, IMLTriggerTypes triggerT, List<InputHandler> handlers);
        public void OnButtonChange(string handlerName, int button, List<InputHandler> handlers);
    }
}

