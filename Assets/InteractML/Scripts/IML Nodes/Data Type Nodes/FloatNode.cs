using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    public class FloatNode : Node
    {
        [Input]
        public float In;

        [SerializeField]
        public float Value;

        [Output]
        public float Out;

        public string ValueName;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            Out = Value;
            return Out; 
        }
    }
}
