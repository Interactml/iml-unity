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
    /// Editor class drawing a IMLVector4 Feature - receiving a Vector4 or drawing editable Vector4 field 
    /// </summary>
    [CustomNodeEditor(typeof(Vector4Node))]
    public class Vector4NodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private Vector4Node m_Vector4Node;

        /// <summary>
        /// Initialise node specific interface values
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_Vector4Node = (target as Vector4Node);

            // Initialise node name
            NodeName = "LIVE VECTOR4 DATA";

            // Initialise node body height
            m_BodyRect.height = 170;
            nodeSpace = 170;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Vector4\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("m_Out", "Vector4\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_Vector4Node.tooltips;

            // Initialise axis labels
            feature_labels = new string[4] { "x: ", "y: ", "z: ", "w: "};
        }

        /// <summary>
        /// Draws node specific body fields
        /// </summary>
        protected override void ShowBodyFields()
        {
            // Checks if node if receiving data- sets green toggle if data incoming, red if no data incoming
            DataInToggle(m_Vector4Node.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }

        /// <summary>
        //// Draws position and values of toggle and labels, draws green toggle if data incoming, red if no data incoming
        /// </summary>
        protected override void DrawFeatureValueTogglesAndLabels(GUIStyle style)
        {
            // If something is connected to the input port show incoming data
            if (m_Vector4Node.InputConnected)
            {
                //draws node data fields
                IMLNodeEditorMethods.DrawFeatureValueToggleAndLabel(m_Vector4Node, this, style);
            }
            // If there is nothing connected to the input port show editable fields
            else
            {
                //draws node editable fields
                DataTypeNodeEditorMethods.DrawEditableFieldsAndToggles(m_Vector4Node, this, style);
            }

        }

    }
}

