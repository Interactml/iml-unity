using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace InteractML
{
    public static class NodeExtensionMethods
    {
        /// <summary>
        /// Returns the list of all nodes connected
        /// </summary>
        /// <param name="node"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static List<Node> GetInputNodesConnected(this Node node, string fieldName)
        {
            // Access node port
            NodePort port = node.GetPort(fieldName);
            // If we have connections...
            if (port != null && port.IsConnected)
            {
                var portsConnected = port.GetConnections().ToList();
                List<Node> nodesConnected = new List<Node>();
                foreach (var connection in portsConnected)
                {
                    Debug.Log(connection.node.name + " is connected");
                    nodesConnected.Add(connection.node);
                }
                // Return the nodes from all the connections
                return nodesConnected;
            }
            // If we don't have any connections, we return null
            else
            {
                return null;
            }
        }
    }
}