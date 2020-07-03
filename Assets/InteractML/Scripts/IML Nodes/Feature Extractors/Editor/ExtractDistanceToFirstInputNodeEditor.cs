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
        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_ExtractDistanceToFirstInput = (target as ExtractDistanceToFirstInput);
            nodeSpace = 150;
            NodeName = "DISTANCE BETWEEN INPUTS";
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            InputPortsNamesOverride = new Dictionary<string, string>();
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("FirstInput", "First Input");
            base.InputPortsNamesOverride.Add("SecondInput", "Second Input");
            base.OutputPortsNamesOverride.Add("DistanceBetweenInputs", "Distance\nBetween\nInputs");
            base.nodeTips = m_ExtractDistanceToFirstInput.tooltips;
            m_BodyRect.height = 100;
            base.OnBodyGUI();
        }

        #region Methods

        protected override void ShowBodyFields()
        {
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
                EditorGUILayout.LabelField("distance between inputs: " + 0, m_NodeSkin.GetStyle("Node Body Label"));
            }
            else
            {
                // Go through the list output distances
                for (int i = 0; i < m_ExtractDistanceToFirstInput.FeatureValues.Values.Length; i++)
                {
                    EditorGUILayout.LabelField("distance between inputs: " + m_ExtractDistanceToFirstInput.FeatureValues.Values[i], m_NodeSkin.GetStyle("Node Body Label"));
                }
            }
            GUILayout.EndArea();
            
        }


        #endregion

    }

}