using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.MovementFeatures
{
    [CustomNodeEditor(typeof(VelocityNode))]
    public class ExtractVelocityNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private VelocityNode m_ExtractVelocity;

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_ExtractVelocity = (target as VelocityNode);

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
            if (m_ExtractVelocity.FeatureValues.Values != null)
            {
                nodeSpace = 60 + (m_ExtractVelocity.FeatureValues.Values.Length * 20);
                m_BodyRect.height = 60 + (m_ExtractVelocity.FeatureValues.Values.Length * 20);
                GUILayout.Space(nodeSpace);

                // set body space based on node editors rects 
                GUILayout.BeginArea(m_BodyRect);
                GUILayout.Space(20);
                MovementFeatureEditorMethods.DrawFeatureValueToggleAndLabelDynamic(this, m_ExtractVelocity);
                GUILayout.EndArea();
            }
            else
            {
                nodeSpace = 60;
                m_BodyRect.height = 60;
                GUILayout.Space(60);

                // set body space based on node editors rects 
                GUILayout.BeginArea(m_BodyRect);
                GUILayout.Space(20);
                // draw alert to connect input
                EditorGUILayout.LabelField("Connect an input", m_NodeSkin.GetStyle("Node Body Label"));
                GUILayout.EndArea();
            }
                
        
            
            
        }

    }

}
