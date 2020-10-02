using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractDistanceToFirstInput))]
    public class ExtractDistanceToFirstInputNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractDistanceToFirstInput m_ExtractDistanceToFirstInput;

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_ExtractDistanceToFirstInput = (target as ExtractDistanceToFirstInput);

            // Initialise node name
            NodeName = "DISTANCE BETWEEN INPUTS";

            // Initialise node height
            m_BodyRect.height = 80;
            nodeSpace = 80;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("FirstInput", "First Input");
            base.InputPortsNamesOverride.Add("SecondInputs", "Second Input");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("DistanceBetweenInputs", "Distance\nBetween\nInputs");

            // Initialise node tooltips
            base.nodeTips = m_ExtractDistanceToFirstInput.tooltips;

        }

        #region Methods

        protected override void ShowBodyFields()
        {
            nodeSpace = 110 + (m_ConnectedInputs * 20);
            m_ConnectedInputs = m_ExtractDistanceToFirstInput.FeatureValues.Values.Length;
            m_BodyRect.height = 60 + (m_ConnectedInputs * 20);
            ShowDistanceBetweenInputsValue();
        }

        /// <summary>
        /// Show the position value fields 
        /// </summary>
        private void ShowDistanceBetweenInputsValue()
        {
            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);


            if (m_ExtractDistanceToFirstInput.FeatureValues.Values == null || m_ExtractDistanceToFirstInput.FeatureValues.Values.Length == 0)
            {
                EditorGUILayout.LabelField("Connect 2 inputs", m_NodeSkin.GetStyle("Node Body Label"));
            }
            else
            {
                // Go through the list of output distances
                EditorGUILayout.LabelField("Distance between first input", m_NodeSkin.GetStyle("Node Body Label"));
                for (int i = 0; i < m_ExtractDistanceToFirstInput.FeatureValues.Values.Length; i++)
                {
                    EditorGUILayout.LabelField("and input " + (i + 1) + ": " + m_ExtractDistanceToFirstInput.FeatureValues.Values[i], m_NodeSkin.GetStyle("Node Body Label"));
                }
            }
            GUILayout.EndArea();

        }


        #endregion

    }

}