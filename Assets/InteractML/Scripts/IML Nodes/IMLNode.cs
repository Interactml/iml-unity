using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML
{
    public class IMLNode : Node
    {
        [HideInInspector]
        public string id;
        [HideInInspector]
        public int numberInComponentList = -1;
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
            id = NodeID.CheckNodeID(id, this);
            base.Init();
            IsInitialized = true;
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        public IMLComponent FindController()
        {
            IMLController MLController = graph as IMLController;
            IMLComponent MLComponent = MLController.SceneComponent;
            return MLComponent;
        }

        public void FindComponentListNumber<T>(List<T> list, IMLComponent MLComponent)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (this == list[i] as IMLNode)
                {
                    numberInComponentList = i;
                    break;
                }

            }
        }
    }
}
