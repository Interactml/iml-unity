using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractVelocity))]
    public class ExtractVelocityNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractVelocity m_ExtractVelocity;

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_ExtractVelocity = (target as ExtractVelocity);

            // Initialise node name
            NodeName = "LIVE VELOCITY DATA";

            // Initialise node height
            m_BodyRect.height = 140;
            nodeSpace = 140;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("FeatureToInput", "Feature\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("VelocityExtracted", "Velocity\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_ExtractVelocity.tooltips;

        }

        protected override void ShowBodyFields()
        {
            DataInToggle(m_ExtractVelocity.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }


        protected override void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            if (m_ExtractVelocity.FeatureValues.Values.Length != 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(bodySpace);
                m_ExtractVelocity.x_switch = EditorGUILayout.Toggle(m_ExtractVelocity.x_switch, style);
                EditorGUILayout.LabelField("x: " + System.Math.Round(m_ExtractVelocity.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                GUILayout.Space(bodySpace);
                m_ExtractVelocity.y_switch = EditorGUILayout.Toggle(m_ExtractVelocity.y_switch, style);
                EditorGUILayout.LabelField("y: " + System.Math.Round(m_ExtractVelocity.FeatureValues.Values[1], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                GUILayout.Space(bodySpace);
                m_ExtractVelocity.z_switch = EditorGUILayout.Toggle(m_ExtractVelocity.z_switch, style);
                EditorGUILayout.LabelField("z: " + System.Math.Round(m_ExtractVelocity.FeatureValues.Values[2], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("Connect a feature extractor", m_NodeSkin.GetStyle("Node Body Label"));
            }
            
           
            
        }

        /// <summary>
        /// Display help button
        /// </summary>
        private void ShowHelpButton()
        {
            m_HelpRect.x = m_HelpRect.x + 20;
            m_HelpRect.y = m_HelpRect.y + 10;
            m_HelpRect.width = m_HelpRect.width - 30;

            GUILayout.BeginArea(m_HelpRect);
            GUILayout.BeginHorizontal();
            GUILayout.Label("");
            GUILayout.Button("Help", m_NodeSkin.GetStyle("Help Button"));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }

}
