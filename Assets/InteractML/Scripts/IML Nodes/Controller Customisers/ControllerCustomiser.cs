using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractML.ControllerCustomisers
{
    [NodeWidth(420)]
    public abstract class CustomController : IMLNode
    {
        
        [Output, SerializeField]
        public bool inputValue;
        public int inputNo;
        public IMLTriggerTypes trigger;
        public string[] buttonOptions;
        public string name;
        protected bool inputChange;

        public abstract void UpdateLogic();
        public abstract void OnButtonChange();
        public abstract void OnTriggerChange();

    }
}

