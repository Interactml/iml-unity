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

        /// <summary>
        /// The script we are referencing
        /// </summary>
        public MonoBehaviour Script;

        /// <summary>
        /// Dictionary 
        /// </summary>
        private NodePortFieldInfoDictionary m_PortsPerFieldInfo;

        #endregion

        #region XNode Messages

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            // Init data containers dictionary
            if (m_PortsPerFieldInfo == null)
                m_PortsPerFieldInfo = new NodePortFieldInfoDictionary();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            // Only return a value if we have a dictionary and a script
            if ((m_PortsPerFieldInfo != null && m_PortsPerFieldInfo.Count > 0) && Script != null)
            {
                FieldInfo info;
                m_PortsPerFieldInfo.TryGetValue(port, out info);
                if (info != null)
                    return info.GetValue(Script);
                else
                    Debug.LogError("The field info is null!");
            }
            return null;
        }

        /// <summary>
        /// Updates the ports fields displayed in the editor
        /// </summary>
        internal void UpdatePortFields(MonoBehaviour gameComponent, bool overrideScript = false)
        {
            if (gameComponent == null)
                return;

            if (overrideScript)
            {
                // Update the script present in the node (although this might return null if it is a clone and gets destroyed?)
                Script = gameComponent;
                // Update name of node
                name = Script.GetType().Name + " (Script)";

            }

            // Gets all fields information from the game component (using System.Reflection)
            FieldInfo[] serializedFields = Script.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

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
                        m_PortsPerFieldInfo = new NodePortFieldInfoDictionary();

                    // Check if the dictionary DOESN'T contain a fieldInfo for this reflected value, and then create nodes and dictionary values
                    if (!m_PortsPerFieldInfo.ContainsValue(fieldToUse))
                    {
                        // Secondly, we create a port (based on its type) for this fieldInfo and add it to the node
                        NodePort newPort = null;
                        // Add port to node
                        if (isInputData)
                            newPort = AddDynamicOutput(fieldToUse.FieldType, fieldName: fieldToUse.Name);
                        else if (isOutputData)
                            newPort = AddDynamicInput(fieldToUse.FieldType, fieldName: fieldToUse.Name);

                        // Add that to the dictionary
                        m_PortsPerFieldInfo.Add(newPort, fieldToUse);

                    }
                    // If the dictionary already contains a fieldInfo (and it is output), update it
                    else if (isOutputData)
                    {
                        // Get the port linked to that field info
                        var port = m_PortsPerFieldInfo.GetKey(fieldToUse);

                        FieldInfo field;
                        m_PortsPerFieldInfo.TryGetValue(port, out field);

                        
                        // Set value by reflection
                        field.SetValue(Script, port.GetInputValue());
                    }
                }
            }

        }

        #endregion
    }
}
