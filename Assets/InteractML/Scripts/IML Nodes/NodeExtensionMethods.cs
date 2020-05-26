using InteractML.DataTypeNodes;
using System;
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

        /// <summary>
        /// Disconnects two nodes if types don't match (3 type evaluation)
        /// </summary>
        public static bool DisconnectIfNotType<T, U, W>(this Node node, NodePort from, NodePort to)
        {
            // Check if types are matching
            bool isValidType1 = false;
            bool isValidType2 = false;
            bool disconnect = false;

            if (to.node is T)
            {
                isValidType1 = from.node is U;
                if (isValidType1)
                    // return here since we don't need to disconnect anything
                    return disconnect = false;

                isValidType2 = from.node is W;
                if (isValidType2)
                    // return here since we don't need to disconnect anything
                    return disconnect = false;

                // If we reach here, it is not a validType
                from.Disconnect(to);
                disconnect = true;
            }

            return disconnect;
        }

        public static bool DisconnectIfNotSameDataType<T> (this BaseDataTypeNode<T> node, NodePort from, NodePort to)
        {
            bool disconnect = false;
            // Check if types are matching
            Type nodeConnectedType = from.node.GetType();
            // Check if the from node is a dataType
            var isDataType = ReusableMethods.Types.IsSubclassOfRawGeneric(typeof(BaseDataTypeNode<>), nodeConnectedType);
            // If it is a dataType...
            if (isDataType)
            {
                // Check if both from and to are the same dataType
                var isSameType = nodeConnectedType.Equals(node.GetType());
                // If not (i.e. a float vs a Vector3)
                if (!isSameType)
                {
                    // We disconnect the nodes
                    from.Disconnect(to);
                    disconnect = true;
                }
            }

            return disconnect;
        }
    }
}