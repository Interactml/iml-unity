﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    public class Vector2Node_Old : Node
    {
        [SerializeField]
        public Vector2 Value;

        [Output]
        public Vector2 Vector2ToOutput;

        public string ValueName;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            Vector2ToOutput = Value;
            return Vector2ToOutput;
        }
    }
}

