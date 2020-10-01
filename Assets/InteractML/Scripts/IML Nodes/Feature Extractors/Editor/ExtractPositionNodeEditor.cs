using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractPosition))]
    public class ExtractPositionNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractPosition m_ExtractPosition;

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_ExtractPosition = (target as ExtractPosition);

            // Initialise node name
            NodeName = "LIVE POSITION DATA";

            // Initialise node height
            m_BodyRect.height = 150;
            nodeSpace = 150;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("GameObjectDataIn", "Game Object\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("LiveDataOut", "Position\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_ExtractPosition.tooltips;

        }

        protected override void ShowBodyFields()
        {
            DataInToggle(m_ExtractPosition.ReceivingData, m_InnerBodyRect, m_BodyRect);   
        }

        protected override void DrawFeatureValueTogglesAndLabels(GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractPosition.x_switch = EditorGUILayout.Toggle(m_ExtractPosition.x_switch, style);
            EditorGUILayout.LabelField("x: " + System.Math.Round(m_ExtractPosition.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractPosition.y_switch = EditorGUILayout.Toggle(m_ExtractPosition.y_switch, style);
            EditorGUILayout.LabelField("y: " + System.Math.Round(m_ExtractPosition.FeatureValues.Values[1], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractPosition.z_switch = EditorGUILayout.Toggle(m_ExtractPosition.z_switch, style);
            EditorGUILayout.LabelField("z: " + System.Math.Round(m_ExtractPosition.FeatureValues.Values[2], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractPosition.LocalSpace = EditorGUILayout.Toggle(m_ExtractPosition.LocalSpace, m_NodeSkin.GetStyle("Local Space Toggle"));
            EditorGUILayout.LabelField("Use local space for transform", m_NodeSkin.GetStyle("Node Local Space Label"));
            GUILayout.EndHorizontal();
        }

    }

}
