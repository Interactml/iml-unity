﻿using System;
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
            Node node = null;
            // If the graph is attached to an IMLComponent...
            if (SceneComponent != null)
            {
                // base logic
                node = base.AddNode(type);
                bool success = false;
                bool isCustomType = false;

                // Check if the node should be added to our IML Graph
                if (node is ScriptNode)
                {
                    // Add scriptNode.script to all lists
                    success = SceneComponent.AddScriptNode((ScriptNode)node);
                    isCustomType = true;
                }
                else if (node is GameObjectNode)
                {
                    // Add gameObjectNode to all lists
                    success = SceneComponent.AddGameObjectNode((GameObjectNode)node);
                    isCustomType = true;
                }

                if (isCustomType)
                {
                    // If we couldn't add the node...
                    if (success == false)
                    {
                        // Remove node from nodes list (it was added in the base logic)
                        nodes.Remove(node);
                        // Destroy node
#if UNITY_EDITOR
                        ScriptableObject.DestroyImmediate(node, true);
#else
                        ScriptableObject.Destroy(node);
#endif
                    }

                }
            }
            // If this graph is not attached to an IML Component...
            else
            {
                Debug.LogError("You can't add nodes to an IMLController that is not attached to an IMLComponent!");
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
                // GameObjectNode
                if (node is GameObjectNode)
                {
                    var goNode = node as GameObjectNode;
                    SceneComponent.DeleteGameObjectNode(goNode);
                }
            }
            RemoveNodeImmediate(node);  
        }

        /// <summary>
        /// Forcefully removes the node from the asset file
        /// </summary>
        /// <param name="node"></param>
        private void RemoveNodeImmediate(Node node)
        {
            node.ClearConnections();
            nodes.Remove(node);
            if (Application.isPlaying) DestroyImmediate(node, true);
        }

    }
}

