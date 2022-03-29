using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

namespace InteractML.ControllerCustomisers
{
    public interface IInputType
    {
        string inputName
        {
            get;
            set;
        }

       void LoadDeviceInfo();
       void SaveDeviceInfo();
       void OnTriggerChange(string handlerName, IMLTriggerTypes triggerT, List<InputHandler> handlers);
       void OnButtonChange(string handlerName, int button, List<InputHandler> handlers);
    }
}

