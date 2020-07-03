using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractRotation))]
    public class ExtractRotationNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractRotation m_ExtractRotation;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_ExtractRotation = (target as ExtractRotation);
            nodeSpace = 150;
            NodeName = "LIVE POSITION DATA";
            base.OnHeaderGUI();

        }


        public override void OnBodyGUI()
        {
            InputPortsNamesOverride = new Dictionary<string, string>();
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("GameObjectDataIn", "Game Object\nData In");
            base.OutputPortsNamesOverride.Add("LiveDataOut", "Rotation\nData Out");
            base.nodeTips = m_ExtractRotation.tooltips;
            m_BodyRect.height = 150;
            base.OnBodyGUI();
        }

        protected override void ShowBodyFields()
        {
            DataInToggle(m_ExtractRotation.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }

        /// <summary>
        /// Show the local space toggle 
        /// </summary>
        protected override void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractRotation.x_switch = EditorGUILayout.Toggle(m_ExtractRotation.x_switch, style);
            EditorGUILayout.LabelField("x: " + System.Math.Round(m_ExtractRotation.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractRotation.y_switch = EditorGUILayout.Toggle(m_ExtractRotation.y_switch, style);
            EditorGUILayout.LabelField("y: " + System.Math.Round(m_ExtractRotation.FeatureValues.Values[1], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractRotation.z_switch = EditorGUILayout.Toggle(m_ExtractRotation.z_switch, style);
            EditorGUILayout.LabelField("z: " + System.Math.Round(m_ExtractRotation.FeatureValues.Values[2], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractRotation.LocalSpace = EditorGUILayout.Toggle(m_ExtractRotation.LocalSpace, m_NodeSkin.GetStyle("Local Space Toggle"));
            EditorGUILayout.LabelField("Use local space for transform", m_NodeSkin.GetStyle("Node Local Space Label"));
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show the local space toggle 
        /// </summary>
       /* private void ShowLocalSpaceToggle()
        {
            m_InnerLocalSpaceRect.x = m_LocalSpaceRect.x + 24;
            m_InnerLocalSpaceRect.y = m_LocalSpaceRect.y + 16;
            m_InnerLocalSpaceRect.width = m_LocalSpaceRect.width;
            m_InnerLocalSpaceRect.height = m_LocalSpaceRect.height;

            GUILayout.BeginArea(m_InnerLocalSpaceRect);
            GUILayout.BeginHorizontal();
            m_ExtractRotation.LocalSpace = EditorGUILayout.Toggle(m_ExtractRotation.LocalSpace, m_NodeSkin.GetStyle("Local Space Toggle"));
            EditorGUILayout.LabelField("Use local space for transform", m_NodeSkin.GetStyle("Node Local Space Label"));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            
        }*/

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
