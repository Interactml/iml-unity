using InteractML.DataTypeNodes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XNode;

namespace InteractML
{
    /// <summary>
    /// Extension methods for nodes
    /// </summary>
    public static class NodeExtensionMethods
    {
        #region Information On Connected Nodes

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
        /// Returns the name of a connected hardware input (keyboard press, VR Trigger, etc.)
        /// </summary>
        /// <param name="node"></param>
        /// <param name="portName"></param>
        /// <returns></returns>
        public static string GetConnectedKeyButtonName(this Node node, string portName)
        {
            string buttonName = "";
            var port = node.GetPort(portName);
            if (port.IsConnected)
            {
                var hardwareInputPort = port.GetConnection(0);
                var hardwareInputNode = hardwareInputPort.node as ControllerCustomisers.CustomController;
                if (hardwareInputNode != null) buttonName = hardwareInputNode.GetButtonName();
            }
            return buttonName;
        }

        #endregion

        #region Conditional Disconnect Methods

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
                    NodeDebug.LogError($"Can't connect {from.fieldName} because node doesn't match expected type: {typeof(T)}, {typeof(U)}", node);
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
                NodeDebug.LogError($"Can't connect {from.fieldName} because node doesn't match expected type: {typeof(T)}, {typeof(U)}, {typeof(W)}", node);
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
                    NodeDebug.LogError($"Can't connect {from.fieldName} because node doesn't match expected data type node: {typeof(T)}", node);
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
                        NodeDebug.LogError($"Can't connect {from.fieldName} because node doesn't match expected feature types: {expectedType}", node);
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
                        NodeDebug.LogError($"Can't connect {from.fieldName} because node doesn't match expected IML data type: {expectedType}", node);
                    }
                }
            }
            return disconnect;
        }


        /// <summary>
        /// Disconnect if all types are NOT FOUND in FROM node (relaxed)
        /// </summary>
        /// <param name="node"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="typesAccepted"></param>
        /// <returns></returns>
        public static bool DisconnectFROMPortIsNotTypes(this Node node, NodePort from, NodePort to, Type[] typesAccepted)
        {
            bool disconnect = true;
            // If not types are passed, then do nothing
            if (typesAccepted == null || typesAccepted.Length < 1)
            {
                disconnect = false;
                return disconnect;
            }
            bool typesEqual = true;
            // If one of the types is detected as equal, don't disconnect
            for (int i = 0; i < typesAccepted.Length; i++)
            {
                typesEqual = CheckNodeEqualTypes(node, from, to, typesAccepted);
                if (typesEqual)
                {
                    disconnect = false;
                    // End search
                    return disconnect;
                }
            }

            if (disconnect)
            {
                from.Disconnect(to);
                NodeDebug.LogError($"Can't connect {from.fieldName} because node doesn't match expected types: {string.Join(",", Array.ConvertAll(typesAccepted, item => item.ToString()))}", node);
            }

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
                    // Compare FROM node type equality to current type (including inheritance)
                    equals = (from.node.GetType().Equals(typesAccepted[i]) || from.node.GetType().IsSubclassOf(typesAccepted[i]) );                                            
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
        /// Disconnects if NONE (relaxed) of the FROM port or node don't meet type requirements. All type checks fail, then disconnect
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
                NodeDebug.LogError($"Can't connect {from.fieldName} because port or node don't match expected port types: {string.Join(",", Array.ConvertAll(typesPort, item => item.ToString()))} or expected node types: {string.Join(",", Array.ConvertAll(typesNode, item => item.ToString()))}", node);
                return false;
            }
        }

        /// <summary>
        /// Disconnects if ANY (strict) of the FROM port or node don't meet type requirements. One type check fails, then disconnect
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
            
            // If all of them are true, DON'T disconnect
            if (equalPorts && equalNodeType)
            {
                return true;
            }
            // If any of them is false, DO disconnect
            else
            {
                NodeDebug.LogError($"Can't connect {from.fieldName} because doesn't match expected port types:  {string.Join(",", Array.ConvertAll(typesPort, item => item.ToString()))} and node types: {string.Join(",", Array.ConvertAll(typesNode, item => item.ToString()))}", node);
                from.Disconnect(to);
                return false;
            }
        }

        /// <summary>
        /// Check if a nodeport exists and if not creates it
        /// </summary>
        /// <param name="node"></param>
        /// <param name="portFieldName"></param>
        /// <param name="fieldType"></param>
        /// <param name="portIOType"></param>
        /// <param name="portConnectionType"></param>
        /// <param name="portTypeConstraint"></param>
        /// <returns></returns>
        public static NodePort GetOrCreateDynamicPort(this Node node, string portFieldName, Type fieldType, NodePort.IO portIOType, Node.ConnectionType portConnectionType = Node.ConnectionType.Multiple, Node.TypeConstraint portTypeConstraint = Node.TypeConstraint.None)
        {
            // Check if nodeport exists
            NodePort auxPort = node.GetPort(portFieldName);
            // If not, create input or ouput depending on option
            if (auxPort == null)
            {
                if (portIOType == NodePort.IO.Input)
                {
                    auxPort = node.AddDynamicInput(fieldType, portConnectionType, portTypeConstraint, portFieldName);
                }
                else if (portIOType == NodePort.IO.Output)
                {
                    auxPort = node.AddDynamicOutput(fieldType, portConnectionType, portTypeConstraint, portFieldName);
                }
            }
            // Return nodeport
            return auxPort;
        }

        #endregion

        #region Comparison IMLOutputs

        /// <summary>
        /// Equality list of IMLOutputs
        /// </summary>
        /// <param name="newClass"></param>
        /// <param name="existingUniqueClass"></param>
        /// <returns></returns>
        public static bool OutputsEqual(List<IMLOutput> newClass, List<IMLOutput> existingUniqueClass)
        {
            if (newClass == null || existingUniqueClass == null)
            {
                Debug.LogError("Null reference when comparing outputs!");
                return false;
            }

            // First we compare size
            bool sizeIsEqual = newClass.Count == existingUniqueClass.Count ? true : false;
            if (!sizeIsEqual) return false;

            // Compare values (knowing that sizes are equal)
            bool valuesEqual = true;
            for (int i = 0; i < newClass.Count; i++)
            {
                var newOutput = newClass[i];
                var uniqueOutputKnown = existingUniqueClass[i];
                // no nullrefs
                if (newOutput != null && newOutput.OutputData != null && newOutput.OutputData.Values != null
                    && uniqueOutputKnown != null && uniqueOutputKnown.OutputData != null && uniqueOutputKnown.OutputData.Values != null
                    // and datatypes mach
                    && newOutput.OutputData.DataType == uniqueOutputKnown.OutputData.DataType)
                {
                    // Compare each value
                    for (int j = 0; j < newOutput.OutputData.Values.Length; j++)
                    {
                        if (newOutput.OutputData.Values[j] != uniqueOutputKnown.OutputData.Values[j])
                            valuesEqual = false;
                    }
                }
                // there were nullrefs or data types didn't match
                else
                {
                    valuesEqual = false;
                }
            }
            return valuesEqual;

        }

    }

    #endregion

}