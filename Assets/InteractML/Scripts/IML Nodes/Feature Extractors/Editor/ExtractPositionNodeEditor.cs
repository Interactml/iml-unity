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
            GUILayout.Space(60);
            // set body space based on node editors rects 
            GUILayout.BeginArea(m_BodyRect);
            GUILayout.Space(20);

            //draws node data fields
            FeatureExtractorEditorMethods.DrawFeatureValueToggleAndLabel(this, m_ExtractPosition, m_ExtractPosition.FeatureValues.Values.Length, IMLNodeEditorMethods.DataInToggle(this, m_ExtractPosition.ReceivingData));

            GUILayout.Space(10);
            //draw toggle to select whether to use localspace
            m_ExtractPosition.LocalSpace = FeatureExtractorEditorMethods.DrawLocalSpaceToggle(this, m_ExtractPosition.LocalSpace);

            GUILayout.EndArea();
        }


    }

}
