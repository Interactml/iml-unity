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
            nodeSpace = 60 + (m_ExtractVelocity.FeatureValues.Values.Length * 20);
            GUILayout.Space(nodeSpace);
            m_BodyRect.height = 60 + (m_ExtractVelocity.FeatureValues.Values.Length * 20);
            // set body space based on node editors rects 
            GUILayout.BeginArea(m_BodyRect);
            GUILayout.Space(20);

            // check if there are any feature connected
            if (m_ExtractVelocity.FeatureValues.Values != null || m_ExtractVelocity.FeatureValues.Values.Length != 0)
            {
                //draws node data fields
                FeatureExtractorEditorMethods.DrawFeatureValueToggleAndLabelDynamic(this, m_ExtractVelocity, m_ExtractVelocity.FeatureValues.Values.Length, IMLNodeEditorMethods.DataInToggle(this, m_ExtractVelocity.ReceivingData));
            }
            else
            {
                // draw alert to connect input
                EditorGUILayout.LabelField("Connect an input", m_NodeSkin.GetStyle("Node Body Label"));
            }

            GUILayout.EndArea();
        }

    }

}
