using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    public abstract class BaseDataTypeNode<T> : Node
    {        
        public abstract T In { get; set; }

        public abstract T Value { get; set; }

        public abstract T Out { get; set; }

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
