using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    public class Vector4Node : Node
    {
        [SerializeField]
        public Vector4 Value;

        [Output]
        public Vector4 Vector4ToOutput;

        public string ValueName;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            Vector4ToOutput = Value;
            return Vector4ToOutput;
        }
    }
}
