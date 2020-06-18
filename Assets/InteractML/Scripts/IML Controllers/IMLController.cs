using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML
{
    /// <summary> 
    /// Defines an example nodegraph IML Controller that can be created as an asset in the Project window. 
    /// </summary>
    [CreateAssetMenu(fileName = "New IML Controller", menuName = "InteractML/IML Controller")]
    public class IMLController : NodeGraph
    {
        /// <summary>
        /// The IML Component in the scene that this graph is attached to
        /// </summary>
        public IMLComponent SceneComponent;

        /// <summary>
        /// Flag that tells us if the graph is supposed to currently run
        /// </summary>
        public bool IsGraphRunning { get { return (SceneComponent != null); } }

        /// <summary>
        /// Override addNode to account for custom adding logic
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public override Node AddNode(Type type)
        {
            // TO DO: account for specific node types and add them to custom lists (to avoid pulling nodes every frame)
            var node = base.AddNode(type);
            if (SceneComponent != null)
            {
                if (node is ScriptNode)
                {
                    // Add scriptNode.script to all lists
                    SceneComponent.AddScriptNode((ScriptNode)node);

                }


            }
            return node;
        }

        /// <summary>
        /// Override removeNode to account for custom removal logic
        /// </summary>
        /// <param name="node"></param>
        public override void RemoveNode(Node node)
        {
            if (SceneComponent != null)
            {
                // If we are deleting a scriptNode...
                if (node is ScriptNode)
                {
                    var scriptNode = node as ScriptNode;
                    // Remove scriptNode.script from all lists
                    SceneComponent.DeleteScriptNode(scriptNode);
                }
            }
            base.RemoveNode(node);  
        }

    }
}

