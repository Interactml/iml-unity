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

            // Initialise axis labels
            feature_labels = new string[0];

        }


        protected override void ShowBodyFields()
        {
            
            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);

            UpdateFeatureLabelArray();

            // check if there are any feature connected
            if (m_ArrayNode.FeatureValues.Values != null && m_ArrayNode.FeatureValues.Values.Length != 0)
            {

                    // dynamically adjust node length based on amount of velocity features
                    nodeSpace = 120 + (m_ArrayNode.FeatureValues.Values.Length * 20);
                    m_BodyRect.height = 60 + (m_ArrayNode.FeatureValues.Values.Length * 20);

                    // draws each feature in the node
                    DataTypeNodeEditorMethods.DrawFeatureValueToggleAndLabel(this, m_ArrayNode);
                

            }
            else
            {
                
                // set node length
                nodeSpace = 120;
                m_BodyRect.height = 60;

                EditorGUILayout.LabelField("Connect an array", m_NodeSkin.GetStyle("Node Body Label"));
                
            }
            GUILayout.EndArea();

            
        }


        protected void UpdateFeatureLabelArray()
        {
            if (feature_labels.Length != m_ArrayNode.FeatureValues.Values.Length)
            {
                feature_labels = new string[m_ArrayNode.FeatureValues.Values.Length];
                for (int i = 0; i < feature_labels.Length; i++)
                    feature_labels[i] = " ";           
            }
        }
    }
}

