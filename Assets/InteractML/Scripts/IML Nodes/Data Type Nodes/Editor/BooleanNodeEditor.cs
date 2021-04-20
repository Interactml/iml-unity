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
    /// Editor class drawing a IMLBoolean Feature - receiving a bool or drawing editable bool field 
    /// </summary>
    [CustomNodeEditor(typeof(BooleanNode))]
    public class BooleanNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private BooleanNode m_BooleanNode;

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_BooleanNode = (target as BooleanNode);

            // Initialise node name
            NodeName = "BOOLEAN";

            // Initialise node height
            m_BodyRect.height = 80;
            nodeSpace = 80;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Bool\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("m_Out", "Bool\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_BooleanNode.tooltips;

            // Initialise axis labels
            feature_labels = new string[1] { " " };

        }

        /// <summary>
        /// Draws node specific body fields
        /// </summary>
        protected override void ShowBodyFields()
        {
            // draws each feature in the node
            DataTypeNodeEditorMethods.DrawFeatureOrEditableFields(this, m_BooleanNode, m_BodyRect);
        }



    }
}
