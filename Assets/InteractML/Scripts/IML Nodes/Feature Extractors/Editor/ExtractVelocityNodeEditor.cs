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

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_ExtractVelocity = (target as ExtractVelocity);
            NodeName = "LIVE VECTOR 3 DATA";
            nodeSpace = 140;
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            InputPortsNamesOverride = new Dictionary<string, string>();
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("FeatureToInput", "Feature\nTo Input");
            base.OutputPortsNamesOverride.Add("VelocityExtracted", "Velocity\nExtracted");
            base.nodeTips = m_ExtractVelocity.tooltips;
            m_BodyRect.height = 140;
            base.OnBodyGUI();
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
