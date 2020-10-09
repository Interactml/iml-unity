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

            // Initialise axis labels
            feature_labels = new string[3] { "x: ", "y: ", "z: " };
        }

        protected override void ShowBodyFields()
        {
            DataInToggle(m_ExtractPosition.isReceivingData, m_InnerBodyRect, m_BodyRect);   
        }

        protected override void DrawFeatureValueTogglesAndLabels(GUIStyle style)
        {
            //draws node data fields
            IMLNodeEditorMethods.DrawFeatureValueToggleAndLabel(m_ExtractPosition, this, style);

            GUILayout.Space(30);

            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            m_ExtractPosition.LocalSpace = EditorGUILayout.Toggle(m_ExtractPosition.LocalSpace, m_NodeSkin.GetStyle("Local Space Toggle"));
            GUILayout.Space(5);
            EditorGUILayout.LabelField("Use local space for transform", m_NodeSkin.GetStyle("Node Local Space Label"));
            GUILayout.EndHorizontal();
        }

    }

}
