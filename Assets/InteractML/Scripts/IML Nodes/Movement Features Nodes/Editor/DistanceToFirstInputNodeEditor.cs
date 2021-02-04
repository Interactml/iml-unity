using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.MovementFeatures
{
    [CustomNodeEditor(typeof(DistanceToFirstInputNode))]
    public class ExtractDistanceToFirstInputNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private DistanceToFirstInputNode m_ExtractDistanceToFirstInput;

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_ExtractDistanceToFirstInput = (target as DistanceToFirstInputNode);

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

            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);
            
            // check if there are any feature connected
            if (m_ExtractDistanceToFirstInput.FeatureValues.Values != null && m_ExtractDistanceToFirstInput.FeatureValues.Values.Length != 0)
            {
                // dynamically adjust node length based on amount of velocity features
                nodeSpace = 120 + (m_ExtractDistanceToFirstInput.FeatureValues.Values.Length * 20);
                m_BodyRect.height = 60 + (m_ExtractDistanceToFirstInput.FeatureValues.Values.Length * 20);
                
                //draws node data fields
                MovementFeatureEditorMethods.DrawFeatureValueToggleAndLabelDynamic(this, m_ExtractDistanceToFirstInput);
                
            }
            else
            {
                // set node length
                nodeSpace = 120;
                m_BodyRect.height = 60;

                // print alert on node
                EditorGUILayout.LabelField("Connect 2 inputs", m_NodeSkin.GetStyle("Node Body Label"));
            }

            GUILayout.EndArea();
        }

        #endregion

    }

}