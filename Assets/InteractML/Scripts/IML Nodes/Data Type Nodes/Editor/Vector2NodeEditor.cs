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
    /// Editor class drawing a IMLVector2 Feature - receiving a Vector2 or drawing editable Vector2 field 
    /// </summary>
    [CustomNodeEditor(typeof(Vector2Node))]
    public class Vector2NodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private Vector2Node m_Vector2Node;

        /// <summary>
        /// Initialise node specific interface values
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_Vector2Node = (target as Vector2Node);

            // Initialise node name
            NodeName = "VECTOR2";

            // Initialise node body height
            m_BodyRect.height = 110;
            nodeSpace = 110;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Vector2\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("m_Out", "Vector2\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_Vector2Node.tooltips;

            // Initialise axis labels
            feature_labels = new string[2] { "x: ", "y: " };

        }

        /// <summary>
        /// Draws node specific body fields
        /// </summary>
        protected override void ShowBodyFields()
        {
            // draws each feature in the node
            DataTypeNodeEditorMethods.DrawFeatureOrEditableFields(this, m_Vector2Node, m_BodyRect);
        }


    }
}

