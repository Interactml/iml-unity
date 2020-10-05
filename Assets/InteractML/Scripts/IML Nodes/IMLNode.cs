using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        /// Has the init() method been called?
        /// </summary>
        [HideInInspector]
        public bool IsInitialized = false;

        // Use this for initialization
        protected override void Init()
        {
            // method that loads uniqie id of this node
            id = NodeID.CheckNodeID(id, this);
            // call base init 
            base.Init();
            //potentially delete checks if initalized has been called 
            IsInitialized = true;
            //load reference to the graph which this node is a member of 
            var MLController = graph as IMLController;
            // if that graph exists refresh the nodes in that graph 
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.GetAllNodes();

            }
            // load tooltips for the node using reference to the nodes class name 
            LoadTooltips(this.GetType().Name);
            // all other initialize code needed for the node - to be overriden in the subclass if there more that needs to be added 
            Initialize();

        }

        public virtual void Initialize()
        {
            // to be overridden by all nodes
        }

        public void LoadTooltips(string name)
        {
            if (File.Exists(name))
            {
                tooltips = IMLTooltipsSerialization.LoadTooltip(this.name);
            }
        }
        // Return the correct value of an output port when requested
        public override object GetValue(XNode.NodePort port)
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
