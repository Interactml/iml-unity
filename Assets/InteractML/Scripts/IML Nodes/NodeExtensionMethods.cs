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

        /// <summary>
        /// Disconnects two nodes if two Data Type Nodes are not exactly the same
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static bool DisconnectIfNotSameDataTypeNode<T> (this BaseDataTypeNode<T> node, NodePort from, NodePort to)
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

        /// <summary>
        /// Disconnect a Feature Extractor from a Data Type Node if the feature extracted doesn't match our expected type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="expectedType"></param>
        /// <returns></returns>
        public static bool DisconnectFeatureNotSameIMLDataType<T>(this BaseDataTypeNode<T> node, NodePort from, NodePort to, IMLSpecifications.DataTypes expectedType)
        {
            // Make sure that the feature connected is matching our type
            bool disconnect = false;
            // If it is a feature...
            if (from.node is IFeatureIML featureConnected)
            {
                // If it is a feature...
                if (featureConnected != null)
                {
                    // Check that dataType is the same as the expected one
                    if (featureConnected.FeatureValues.DataType != expectedType)
                    {
                        from.Disconnect(to);
                        disconnect = true;
                    }
                }

            }
            return disconnect;
        }

        /// <summary>
        /// Disconnect two IML Data Types if they are not equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="expectedType"></param>
        /// <returns></returns>
        public static bool DisconnectIfNotSameIMLDataType<T>(this BaseDataTypeNode<T> node, NodePort from, NodePort to, IMLSpecifications.DataTypes expectedType)
        {
            // Make sure that the IMLDataType connected is matching our type
            bool disconnect = false;
            // If it is a IMLDataType, check that it is the exact same one
            if (from.node.GetType().Equals(typeof(IMLBaseDataType)))
            {
                var featureConnected = from.node as IFeatureIML;
                // If it is a feature...
                if (featureConnected != null)
                {
                    // Check that dataType is the same as the expected one
                    if (featureConnected.FeatureValues.DataType != expectedType)
                    {
                        from.Disconnect(to);
                        disconnect = true;
                    }
                }
            }
            return disconnect;
        }


        /// <summary>
        /// Disconnect if at least not one of the types is found in the FROM node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="typesAccepted"></param>
        /// <returns></returns>
        public static bool DisconnectFROMPortIsNotTypes(this Node node, NodePort from, NodePort to, Type[] typesAccepted)
        {
            bool disconnect = false;
            // If not types are passed, then do nothing
            if (typesAccepted == null || typesAccepted.Length < 1)
            {
                disconnect = false;
                return disconnect;
            }
            bool typesEqual = false;
            // If one of the types is detected as equal, don't disconnect
            for (int i = 0; i < typesAccepted.Length; i++)
            {
                typesEqual = from.ValueType.Equals(typesAccepted);
                if (typesEqual)
                {
                    disconnect = false;
                    // End search
                    return disconnect;
                }
            }

            if (disconnect)
                from.Disconnect(to);

            return disconnect;
            
        }

        /// <summary>
        /// Is FROM NodePort EQUAL to ANY of the Types?
        /// </summary>
        /// <param name="node"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="typesAccepted"></param>
        /// <returns></returns>
        public static bool CheckPortEqualTypes(this Node node, NodePort from, NodePort to, Type[] typesAccepted)
        {
            bool equals = false;
            // If not types are passed, then do nothing
            if (typesAccepted == null || typesAccepted.Length < 1)
            {
                equals = false;
                return equals;
            }
            // If one of the types is detected as equal, return true and stop searching
            for (int i = 0; i < typesAccepted.Length; i++)
            {
                // If this type is an interface...
                if (typesAccepted[i].IsInterface)
                {
                    // Extract the interfaces in FROM port and compare equality
                    var interfaces = from.ValueType.GetInterfaces();
                    for (int j = 0; j < interfaces.Length; j++)
                    {
                        equals = interfaces[j].Equals(typesAccepted[i]);
                    }
                }
                // If it is not an interface...
                else
                {
                    // Compare FROM port type equality to current type
                    equals = from.ValueType.Equals(typesAccepted[i]);
                }
                if (equals)
                {
                    // End search
                    return equals;
                }
            }

            return equals;

        }

        /// <summary>
        /// Is FROM node equal to ANY of the types?
        /// </summary>
        /// <param name="node"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="typesAccepted"></param>
        /// <returns></returns>
        public static bool CheckNodeEqualTypes(this Node node, NodePort from, NodePort to, Type[] typesAccepted)
        {
            bool equals = false;
            // If not types are passed, then do nothing
            if (typesAccepted == null || typesAccepted.Length < 1)
            {
                equals = false;
                return equals;
            }
            // If one of the types is detected as equal, return true and stop searching
            for (int i = 0; i < typesAccepted.Length; i++)
            {
                // If this type is an interface...
                if (typesAccepted[i].IsInterface)
                {
                    // Extract the interfaces in FROM node and compare equality
                    var interfaces = from.node.GetType().GetInterfaces();
                    for (int j = 0; j < interfaces.Length; j++)
                    {
                        equals = interfaces[j].Equals(typesAccepted[i]);
                    }
                }
                // If it is not an interface...
                else
                {
                    // Compare FROM node type equality to current type
                    equals = from.node.GetType().Equals(typesAccepted[i]);
                }

                if (equals)
                {
                    // End search
                    return equals;
                }
            }

            return equals;

        }

        /// <summary>
        /// Is FROM node NOT equal to ANY of the types?
        /// </summary>
        /// <param name="node"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static bool CheckNodeNOTEqualTypes(this Node node, NodePort from, NodePort to , Type[] types)
        {
            bool notEquals = false;
            // If not types are passed, then do nothing
            if (types == null || types.Length < 1)
            {
                notEquals = false;
                return notEquals;
            }
            // If one of the types is detected as equal, return true and stop searching
            for (int i = 0; i < types.Length; i++)
            {
                // If this type is an interface...
                if (types[i].IsInterface)
                {
                    // Extract the interfaces in FROM node and compare equality
                    var interfaces = from.node.GetType().GetInterfaces();
                    for (int j = 0; j < interfaces.Length; j++)
                    {
                        notEquals = !(interfaces[j].Equals(types[i]));
                    }
                }
                // If it is not an interface...
                else
                {
                    // Compare FROM node type equality to current type
                    notEquals = !(from.node.GetType().Equals(types[i]));
                }

                if (notEquals)
                {
                    // End search
                    return notEquals;
                }
            }

            return notEquals;

        }

        /// <summary>
        /// Disconnects if NONE of the FROM port or node don't meet type requirements
        /// </summary>
        /// <param name="node"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="typesPort"></param>
        /// <param name="typesNode"></param>
        /// <returns></returns>
        public static bool DisconnectPortAndNodeIfNONETypes(this Node node, NodePort from, NodePort to, Type[] typesPort, Type[] typesNode)
        {
            // Init equals
            bool equalPorts = false;
            bool equalNodeType = false;
            
            // If null or empty, allow connections
            // Port Types
            if (typesPort == null && typesPort.Length < 1)
                equalPorts = true;
            else
                equalPorts = CheckPortEqualTypes(node, from, to, typesPort);
            
            // Node Types
            if (typesNode == null && typesNode.Length < 1)
                equalNodeType = true;
            else
                equalNodeType = CheckNodeEqualTypes(node, from, to, typesNode);
            
            // If any of them is true, DON'T disconnect
            if (equalPorts || equalNodeType)
            {
                return true;
            }
            // If all of them are false, DO disconnect
            else
            {
                from.Disconnect(to);
                return false;
            }
        }

        /// <summary>
        /// Disconnects if ANY of the FROM port or node don't meet type requirements
        /// </summary>
        /// <param name="node"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="typesPort"></param>
        /// <param name="typesNode"></param>
        /// <returns></returns>
        public static bool DisconnectPortAndNodeIfANYTypes(this Node node, NodePort from, NodePort to, Type[] typesPort, Type[] typesNode)
        {
            // Init equals
            bool equalPorts = true;
            bool equalNodeType = true;
            
            // If null or empty, allow connections
            // Port Types
            if (typesPort == null && typesPort.Length < 1)
                equalPorts = false;
            else
                equalPorts = CheckPortEqualTypes(node, from, to, typesPort);
            
            // Node Types
            if (typesNode == null && typesNode.Length < 1)
                equalNodeType = false;
            else
                equalNodeType = CheckNodeEqualTypes(node, from, to, typesNode);
            
            // If any of them is true, DO disconnect
            if (equalPorts || equalNodeType)
            {
                from.Disconnect(to);
                return true;
            }
            // If all of them are false, DON'T disconnect
            else
            {
                return false;
            }
        }
    }
}