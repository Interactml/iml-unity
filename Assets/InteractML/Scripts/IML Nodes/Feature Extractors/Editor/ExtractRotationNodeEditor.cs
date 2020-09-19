using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractRotationQuaternion))]
    public class ExtractRotationNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractRotationQuaternion m_ExtractRotationQuaternion;

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_ExtractRotationQuaternion = (target as ExtractRotationQuaternion);

            // Initialise node name
            NodeName = "LIVE ROTATION DATA";
            NodeSubtitle = "Quaternion";

            // Initialise node height
            m_BodyRect.height = 180;
            nodeSpace = 180;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("GameObjectDataIn", "Game Object\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("LiveDataOut", "Rotation\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_ExtractRotationQuaternion.tooltips;

        }


        protected override void ShowBodyFields()
        {
            DataInToggle(m_ExtractRotationQuaternion.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }

        /// <summary>
        /// Show the local space toggle 
        /// </summary>
        protected override void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractRotationQuaternion.x_switch = EditorGUILayout.Toggle(m_ExtractRotationQuaternion.x_switch, style);
            EditorGUILayout.LabelField("x: " + System.Math.Round(m_ExtractRotationQuaternion.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractRotationQuaternion.y_switch = EditorGUILayout.Toggle(m_ExtractRotationQuaternion.y_switch, style);
            EditorGUILayout.LabelField("y: " + System.Math.Round(m_ExtractRotationQuaternion.FeatureValues.Values[1], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractRotationQuaternion.z_switch = EditorGUILayout.Toggle(m_ExtractRotationQuaternion.z_switch, style);
            EditorGUILayout.LabelField("z: " + System.Math.Round(m_ExtractRotationQuaternion.FeatureValues.Values[2], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractRotationQuaternion.w_switch = EditorGUILayout.Toggle(m_ExtractRotationQuaternion.w_switch, style);
            EditorGUILayout.LabelField("w: " + System.Math.Round(m_ExtractRotationQuaternion.FeatureValues.Values[3], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractRotationQuaternion.LocalSpace = EditorGUILayout.Toggle(m_ExtractRotationQuaternion.LocalSpace, m_NodeSkin.GetStyle("Local Space Toggle"));
            EditorGUILayout.LabelField("Use local space for transform", m_NodeSkin.GetStyle("Node Local Space Label"));
            GUILayout.EndHorizontal();
        }

    }

}
