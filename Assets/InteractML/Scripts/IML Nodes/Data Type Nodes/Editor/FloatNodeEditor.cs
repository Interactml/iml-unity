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
            NodeName = "FLOAT";

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
            // draws each feature in the node
            DataTypeNodeEditorMethods.DrawFeatureOrEditableFields(this, m_FloatNode, m_BodyRect);
        }

        

    }
}
