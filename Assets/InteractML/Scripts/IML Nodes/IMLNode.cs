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
        public bool IsInitialized { get {return m_IsInitialized; } }
        private bool m_IsInitialized = false;

       
        /// <summary>
        /// Initializes the nodes 
        /// Called when the node is added in IMLController 
        /// Also called in IMLComponent by event OnEnable 
        /// </summary>
        public void NodeInitalize()
        {
            // if id not initalised use this to tell whether node has been properly added to the graph 
            if(id == null)
            {
                // method that loads uniqie id of this node
                id = NodeID.CheckNodeID(id, this);
                //load reference to the graph which this node is a member of 
                var MLController = graph as IMLController;
                // if that graph exists refresh the nodes in that graph 
                if (MLController.SceneComponent != null)
                {
                    MLController.SceneComponent.GetAllNodes();

                }
            }
            
            //potentially delete checks if initalized has been called 
            m_IsInitialized = true;
            // load tooltips for the node using reference to the nodes class name
            IMLTooltipsSerialization.LoadTooltip(this.GetType().Name + "Tooltips");
            //Debug.Log(this.GetType().Name);
            // all other initialize code needed for the node - to be overriden in the subclass if there more that needs to be added 
            Initialize();

        }
        /// <summary>
        /// Method which initializes node specific code 
        /// Called from NodeInitialize
        /// Should be overridden in all nodes 
        /// </summary>
        public virtual void Initialize()
        {
            // to be overridden by all nodes
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
