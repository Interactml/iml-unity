using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    public class SerialVectorNode : Node
    {
        [SerializeField]
        public float[] Value;

        [Output]
        public float[] arrayToOutput;

        public string ValueName;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            arrayToOutput = Value;
            return arrayToOutput;
        }

    }
}
