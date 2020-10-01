using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(FloatNode))]
    public class FloatNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private FloatNode m_FloatNode;

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_FloatNode = (target as FloatNode);

            // Initialise node name
            NodeName = "LIVE FLOAT DATA";

            // Initialise node height
            m_BodyRect.height = 80;
            nodeSpace = 80;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Float\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("m_Out", "Float\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_FloatNode.tooltips;
            
        }
        

        protected override void ShowBodyFields()
        {
            DataInToggle(m_FloatNode.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }

        
        protected override void DrawFeatureValueTogglesAndLabels(GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            // If something is connected to the input port show incoming data
            if (m_FloatNode.InputConnected)
            {
                m_FloatNode.float_switch = EditorGUILayout.Toggle(m_FloatNode.float_switch, style, GUILayout.MaxWidth(dataWidth));
                EditorGUILayout.LabelField("Float: " + System.Math.Round(m_FloatNode.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
            }
            // If there is nothing connected to the input port show editable fields
            else
            {
                m_FloatNode.float_switch = EditorGUILayout.Toggle(m_FloatNode.float_switch, style, GUILayout.MaxWidth(dataWidth));
                EditorGUILayout.LabelField("Float: ", m_NodeSkin.GetStyle("Node Body Label"), GUILayout.MaxWidth(dataWidth));
                GUILayout.Space(10);
                m_FloatNode.m_UserInput = EditorGUILayout.FloatField(m_FloatNode.m_UserInput, GUILayout.MaxWidth(inputWidth));
                
            }
            GUILayout.EndHorizontal();
        }

    }
}
