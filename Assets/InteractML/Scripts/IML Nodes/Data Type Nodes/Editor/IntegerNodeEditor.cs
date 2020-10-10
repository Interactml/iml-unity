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
    /// Editor class drawing a IMLInteger Feature - receiving an integer or drawing editable integer field 
    /// </summary>
    [CustomNodeEditor(typeof(IntegerNode))]
    public class IntegerNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private IntegerNode m_IntegerNode;

        /// <summary>
        /// Initialise node specific interface values
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_IntegerNode = (target as IntegerNode);

            // Initialise node name
            NodeName = "LIVE INTEGER DATA";

            // Initialise node body height
            m_BodyRect.height = 80;
            nodeSpace = 80;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Integer\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("m_Out", "Integer\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_IntegerNode.tooltips;
            
            // Initialise axis labels
            feature_labels = new string[1] { " " };
        }

        /// <summary>
        /// Draws node specific body fields
        /// </summary>
        protected override void ShowBodyFields()
        {
            // draws each feature in the node
            DataTypeNodeEditorMethods.DrawFeatureOrEditableFields(this, m_IntegerNode, m_BodyRect);
        }

    }
}

