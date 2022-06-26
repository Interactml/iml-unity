using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.GameObjectMovementFeatures
{
    [CustomNodeEditor(typeof(WindowFeatureNode))]
    public class WindowFeatureNodeEditor : IMLNodeEditor
    {
        private WindowFeatureNode m_ExtractWindowNode;

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_ExtractWindowNode = (target as WindowFeatureNode);

            // Initialise node name
            NodeName = "WINDOW OF FEATURES";

            // Initialise node height
            m_BodyRect.height = 140;
            nodeSpace = 140;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("FeaturesAsInput", "Features\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("LiveDataOut", "Window\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_ExtractWindowNode.tooltips;

        }


        protected override void ShowBodyFields()
        {
            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);

            if (m_ExtractWindowNode.FeatureValues.Values != null)
            {
                float extraSpace = 40;
                // dynamically adjust node length based on amount of features
                nodeSpace = 120 + (m_ExtractWindowNode.FeatureValues.Values.Length * extraSpace);
                m_BodyRect.height = 60 + (m_ExtractWindowNode.FeatureValues.Values.Length * extraSpace);

                EditorGUILayout.LabelField("Sample Size");
                // Draw slider to select size of window
                m_ExtractWindowNode.WindowSamples =  EditorGUILayout.IntSlider(m_ExtractWindowNode.WindowSamples, 1, 100);
                EditorGUILayout.Space(5);

                // draw each window values
                MovementFeatureEditorMethods.DrawFeatureValueToggleAndLabelDynamic(this, m_ExtractWindowNode);
            }
            else
            {
                // set node length
                nodeSpace = 120;
                m_BodyRect.height = 60;

                // print alert on node
                EditorGUILayout.LabelField("Connect an input", m_NodeSkin.GetStyle("Node Body Label"));
            }

            GUILayout.EndArea();

        }



    }
}