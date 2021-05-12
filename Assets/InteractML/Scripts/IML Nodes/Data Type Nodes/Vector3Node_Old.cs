using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    public class Vector3Node_Old : Node
    {
        [SerializeField]
        public Vector3 Value;

        [Output]
        public Vector3 Vector3ToOutput;

        public string ValueName;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            Vector3ToOutput = Value;
            return Vector3ToOutput;
        }
    }
}
