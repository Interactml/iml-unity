using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using XNode;

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

        public bool InputChange { get => inputChange; }

        public abstract void UpdateLogic();
        public abstract void OnButtonChange();
       // public abstract void OnButtonChange(int buttonNo);
        public abstract void OnTriggerChange();

        public override object GetValue(NodePort port)
        {
            inputValue = inputChange;

            return inputValue;
        }

    }
}

