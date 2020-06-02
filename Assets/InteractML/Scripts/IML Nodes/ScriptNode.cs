using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XNode;
using System.Linq;

namespace InteractML
{
    /// <summary>
    /// Node that will show information about a script subscribed to an IML Controller (MonoBehaviour)
    /// </summary>
    public class ScriptNode : IMLNode
    {
        #region Variables

        [Input]
        public float testIn;

        [Input]
        public float testIn2;

        [Output]
        public float testOut;


        /// <summary>
        /// The script we are referencing
        /// </summary>
        public MonoBehaviour Script;

        /// <summary>
        /// Dictionary 
        /// </summary>
        private Dictionary<NodePort, FieldInfo> m_PortsPerFieldInfo;

        #endregion

        #region XNode Messages

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            // Init data containers dictionary
            if (m_PortsPerFieldInfo == null)
                m_PortsPerFieldInfo = new Dictionary<NodePort, FieldInfo>();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            // Only return a value if we have a dictionary and a script
            if ((m_PortsPerFieldInfo != null || m_PortsPerFieldInfo.Count > 0) && Script != null)
            {
                FieldInfo info;
                m_PortsPerFieldInfo.TryGetValue(port, out info);
                return info.GetValue(Script);
            }
            return null;
        }

        /// <summary>
        /// Updates the ports fields displayed in the editor
        /// </summary>
        /// <param name="serializedFields"></param>
        internal void UpdatePortFields(FieldInfo[] serializedFields)
        {
            // Go through all the fields
            for (int i = 0; i < serializedFields.Length; i++)
            {
                var fieldToUse = serializedFields[i];

                // Check if the field is marked with the "SendToIMLController" attribute
                SendToIMLController dataForIMLController = Attribute.GetCustomAttribute(fieldToUse, typeof(SendToIMLController)) as SendToIMLController;
                // We check now if the field is marked with the "PullFromIMLController" attribute
                PullFromIMLController dataFromIMLController = Attribute.GetCustomAttribute(fieldToUse, typeof(PullFromIMLController)) as PullFromIMLController;
                // Define flags to identify attribute behaviour
                bool isInputData = false, isOutputData = false;
                // Update flags
                if (dataForIMLController != null)
                    isInputData = true;
                if (dataFromIMLController != null)
                    isOutputData = true;

                // If the field is marked as either input or output...
                if (isInputData || isOutputData)
                {
                    // Debug type of that value in console
                    //Debug.Log(fieldToUse.Name + " Used in IMLComponent, With Type: " + fieldToUse.FieldType + ", With Value: " + fieldToUse.GetValue(gameComponent).ToString());

                    // Make sure that the dictionaries are initialised
                    if (m_PortsPerFieldInfo == null)
                        m_PortsPerFieldInfo = new Dictionary<NodePort, FieldInfo>();

                    // Check if the dictionary DOESN'T contain a fieldInfo for this reflected value, and then create nodes and dictionary values
                    if (!m_PortsPerFieldInfo.ContainsValue(fieldToUse))
                    {
                        // Secondly, we create a port (based on its type) for this fieldInfo and add it to the node
                        NodePort newPort = null;
                        // Add port to node
                        if (isInputData)
                            newPort = AddDynamicInput(fieldToUse.FieldType, fieldName: fieldToUse.Name);
                        else if (isOutputData)
                            newPort = AddDynamicOutput(fieldToUse.FieldType, fieldName: fieldToUse.Name);

                        // Add that to the dictionary
                        m_PortsPerFieldInfo.Add(newPort, fieldToUse);

                    }
                    // If the dictionary already contains a fieldInfo, update it
                    //else if (true)
                    //{
                    //    // Get the port linked to that field info
                    //    var port = m_PortsPerFieldInfo;

                    //    // Detect the type of the node
                    //    switch (port.DataType)
                    //    {
                    //        // FLOAT
                    //        case IMLSpecifications.DataTypes.Float:
                    //            // If it is input...
                    //            if (isInputData)
                    //                (port.nodeForField as DataTypeNodes.FloatNode).Value = (float)fieldToUse.GetValue(gameComponent);
                    //            // If it is output...
                    //            if (isOutputData)
                    //                fieldToUse.SetValue(gameComponent, (port.nodeForField as DataTypeNodes.FloatNode).Value);
                    //            break;
                    //        // INTEGER
                    //        case IMLSpecifications.DataTypes.Integer:
                    //            // If it is input...
                    //            if (isInputData)
                    //                (port.nodeForField as DataTypeNodes.IntegerNode).Value = (int)fieldToUse.GetValue(gameComponent);
                    //            // If it is output...
                    //            if (isOutputData)
                    //                fieldToUse.SetValue(gameComponent, (port.nodeForField as DataTypeNodes.IntegerNode).Value);
                    //            break;
                    //        // VECTOR 2
                    //        case IMLSpecifications.DataTypes.Vector2:
                    //            // If it is input...
                    //            if (isInputData)
                    //                (port.nodeForField as DataTypeNodes.Vector2Node).Value = (Vector2)fieldToUse.GetValue(gameComponent);
                    //            // If it is output...
                    //            if (isOutputData)
                    //                fieldToUse.SetValue(gameComponent, (port.nodeForField as DataTypeNodes.Vector2Node).Value);
                    //            break;
                    //        // VECTOR 3
                    //        case IMLSpecifications.DataTypes.Vector3:
                    //            // If it is input...
                    //            if (isInputData)
                    //                (port.nodeForField as DataTypeNodes.Vector3Node).Value = (Vector3)fieldToUse.GetValue(gameComponent);
                    //            // If it is output...
                    //            if (isOutputData)
                    //                fieldToUse.SetValue(gameComponent, (port.nodeForField as DataTypeNodes.Vector3Node).Value);
                    //            break;
                    //        // VECTOR 4
                    //        case IMLSpecifications.DataTypes.Vector4:
                    //            // If it is input...
                    //            if (isInputData)
                    //                (port.nodeForField as DataTypeNodes.Vector4Node).Value = (Vector4)fieldToUse.GetValue(gameComponent);
                    //            // If it is output...
                    //            if (isOutputData)
                    //                fieldToUse.SetValue(gameComponent, (port.nodeForField as DataTypeNodes.Vector4Node).Value);
                    //            break;
                    //        // SERIAL VECTOR
                    //        case IMLSpecifications.DataTypes.SerialVector:
                    //            // If it is input...
                    //            if (isInputData)
                    //                (port.nodeForField as DataTypeNodes.SerialVectorNode).Value = (float[])fieldToUse.GetValue(gameComponent);
                    //            // If it is output...
                    //            if (isOutputData)
                    //                fieldToUse.SetValue(gameComponent, (port.nodeForField as DataTypeNodes.SerialVectorNode).Value);
                    //            break;
                    //        // DEFAULT SWITCH
                    //        default:
                    //            break;
                    //    }
                    //}

                }
            }

        }

        #endregion
    }
}
