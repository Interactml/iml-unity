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

        /// <summary>
        /// Disconnects two nodes if types don't match
        /// </summary>
        /// <typeparam name="T">Type from</typeparam>
        /// <typeparam name="U">Type to</typeparam>
        /// <param name="node"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>True if disconnected. False otherwise</returns>
        public static bool DisconnectIfNotType<T, U>(this Node node, NodePort from, NodePort to)
        {
            // Check if types are matching
            bool isValidType = false;
            if (to.node is T)
            {
                isValidType = from.node is U;
                if (!isValidType)
                {
                    from.Disconnect(to);
                }
            }

            //True if disconnected.False otherwise
            return !isValidType;
        }
    }
}