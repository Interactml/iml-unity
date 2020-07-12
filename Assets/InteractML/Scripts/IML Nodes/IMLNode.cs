using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML
{
    public class IMLNode : Node
    {
        [HideInInspector]
        public IMLNodeTooltips tooltips;

        /// <summary>
        /// Has the init() mehtod been called?
        /// </summary>
        [HideInInspector]
        public bool IsInitialized = false;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            IsInitialized = true;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }
    }
}
