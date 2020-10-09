using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    /// <summary>
    /// Editor class drawing a IMLFloat Feature - receiving a float or drawing editable float field 
    /// </summary>
    [CustomNodeEditor(typeof(FloatNode))]
    public class FloatNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private FloatNode m_FloatNode;

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_FloatNode = (target as FloatNode);

            // Initialise node name
            NodeName = "LIVE FLOAT DATA";

            // Initialise node height
            m_BodyRect.height = 80;
            nodeSpace = 80;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Float\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("m_Out", "Float\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_FloatNode.tooltips;

            // Initialise axis labels
            feature_labels = new string[1] { " " };

        }

        /// <summary>
        /// Draws node specific body fields
        /// </summary>
        protected override void ShowBodyFields()
        {
            // Checks if node if receiving data- sets green toggle if data incoming, red if no data incoming
            DataInToggle(m_FloatNode.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }

        /// <summary>
        //// Draws position and values of toggle and labels, draws green toggle if data incoming, red if no data incoming
        /// </summary>
        protected override void DrawFeatureValueTogglesAndLabels(GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            // If something is connected to the input port show incoming data
            if (m_FloatNode.InputConnected)
            {
                //draws node data fields
                IMLNodeEditorMethods.DrawFeatureValueToggleAndLabel(m_FloatNode, this, style);
            }
            // If there is nothing connected to the input port show editable fields
            else
            {
                //draws node editable fields
                DataTypeNodeEditorMethods.DrawEditableFieldsAndToggles(m_FloatNode, this, style);
            }
            GUILayout.EndHorizontal();
        }

    }
}
