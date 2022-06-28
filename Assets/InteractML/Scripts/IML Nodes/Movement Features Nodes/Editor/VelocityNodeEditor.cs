using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.GameObjectMovementFeatures
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
            NodeName = "VELOCITY";

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
            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);

            if (m_ExtractVelocity.FeatureValues.Values != null)
            {
                float extraSpace = 30;
                // dynamically adjust node length based on amount of velocity features
                nodeSpace = 120 + (m_ExtractVelocity.FeatureValues.Values.Length * extraSpace);
                m_BodyRect.height = 60 + (m_ExtractVelocity.FeatureValues.Values.Length * extraSpace);

                // draw each velocity values
                MovementFeatureEditorMethods.DrawFeatureValueToggleAndLabelDynamic(this, m_ExtractVelocity);
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
