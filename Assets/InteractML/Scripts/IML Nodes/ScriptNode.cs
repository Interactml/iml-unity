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
        [IMLMonobehaviour, SerializeField]
        private MonoBehaviour m_ScriptInternal;

        /// <summary>
        /// Hash value from the script. Useful to identify to which script instance this node belongs to
        /// </summary>
        [SerializeField, HideInInspector]
        public int ScriptHashCode;

        /// <summary>
        /// Marks if this script is already assigned to a script
        /// </summary>
        public bool IsTaken { get { return (m_ScriptInternal != null); } }

        /// <summary>
        /// Dictionary 
        /// </summary>
        private NodePortFieldInfoDictionary m_PortsPerFieldInfo;

#if UNITY_EDITOR
        /// <summary>
        /// Flag that marks if this node was created during playMode (useful when deleting things after leaving playmode)
        /// </summary>
        [HideInInspector]
        public bool CreatedDuringPlaymode;
#endif

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
            if ((m_PortsPerFieldInfo != null && m_PortsPerFieldInfo.Count > 0) && m_ScriptInternal != null)
            {
                FieldInfo info;
                m_PortsPerFieldInfo.TryGetValue(port, out info);
                if (info != null)
                    return info.GetValue(m_ScriptInternal);
                else
                    Debug.LogError("The field info is null!");
            }
            return null;
        }

        #endregion

        #region Methods

        public MonoBehaviour GetScript()
        {
            return m_ScriptInternal;
        }

        public void SetScript(MonoBehaviour newScript)
        {
            // If we are overriding the script...
            if (m_ScriptInternal != null && m_ScriptInternal.GetType() != newScript.GetType() || m_ScriptInternal == null && Ports.Count() > 0)
            {
                // Make sure to reset all dynamic ports!
                ClearDynamicPorts();
                // Clear dictionary
                m_PortsPerFieldInfo.Clear();
            }

            // Set the script 
            m_ScriptInternal = newScript;
            // Update name
            name = m_ScriptInternal.GetType().Name + " (Script)";
            // Update reference of type held
            ScriptHashCode = m_ScriptInternal.GetHashCode();

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
                SetScript(gameComponent);
            }

            // Gets all fields information from the game component (using System.Reflection)
            FieldInfo[] serializedFields = m_ScriptInternal.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

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
                        field.SetValue(m_ScriptInternal, port.GetInputValue());
                    }
                }
            }

        }

        #endregion
    }
}
