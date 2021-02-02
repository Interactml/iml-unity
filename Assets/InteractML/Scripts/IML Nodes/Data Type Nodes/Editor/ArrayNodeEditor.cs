using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(ArrayNode))]
    public class ArrayNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ArrayNode m_ArrayNode;

        /// <summary>
        /// Initialise node specific interface values
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_ArrayNode = (target as ArrayNode);

            // Initialise node name
            NodeName = "LIVE ARRAY DATA";

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Array\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("m_Out", "Array \nData Out");

            // Initialise node tooltips
            base.nodeTips = m_ArrayNode.tooltips;

        }

        /// <summary>
        /// Show the serial value fields with attribute name
        /// </summary>
        protected override void ShowBodyFields()
        {
            nodeSpace = 120 + (m_ConnectedInputs * 20);
            m_ConnectedInputs = m_ArrayNode.FeatureValues.Values.Length;
            m_BodyRect.height = 60 + (m_ConnectedInputs * 20);

            GUILayout.BeginArea(m_InnerBodyRect);

            if (m_ArrayNode.FeatureValues.Values.Length == 0 || m_ArrayNode.FeatureValues.Values == null)
            {
                EditorGUILayout.LabelField("Connect an array", m_NodeSkin.GetStyle("Node Body Label"));
            }
            else
            {
                // draws each feature in the node
                DataTypeNodeEditorMethods.DrawFeatureOrEditableFields(this, m_ArrayNode, m_BodyRect);
            }
            GUILayout.EndArea();

        }
    }
}

