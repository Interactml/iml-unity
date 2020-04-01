using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    [NodeTint("#3A3B5B")]
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
            name = "VECTOR4";

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            Vector4ToOutput = Value;
            return Vector4ToOutput;
        }
    }
}
