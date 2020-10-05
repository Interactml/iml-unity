using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(Vector3Node))]
    public class Vector3NodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Editor class drawing an IMLVector3 Feature - receiving a Vector3 or drawing editable Vector3 field 
        /// </summary>
        private Vector3Node m_Vector3Node;

        /// <summary>
        /// Initialise node specific interface values
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_Vector3Node = (target as Vector3Node);

            // Initialise node name
            NodeName = "LIVE VECTOR3 DATA";

            // Initialise node body height
            m_BodyRect.height = 130;
            nodeSpace = 130;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Vector3\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("m_Out", "Vector3\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_Vector3Node.tooltips;

            // Initialise axis labels
            feature_labels = new string[3] { "x: ", "y: ", "z: " };
        }

        /// <summary>
        /// Draws node specific body fields
        /// </summary>
        protected override void ShowBodyFields()
        {
            // Checks if node if receiving data- sets green toggle if data incoming, red if no data incoming
            DataInToggle(m_Vector3Node.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }

        /// <summary>
        //// Draws position and values of toggle and labels, draws green toggle if data incoming, red if no data incoming
        /// </summary>
        protected override void DrawFeatureValueTogglesAndLabels(GUIStyle style)
        {
            // If something is connected to the input port show incoming data
            if (m_Vector3Node.InputConnected)
            {
                //draws node data fields
                IMLNodeEditorMethods.DrawFeatureValueToggleAndLabel(m_Vector3Node, this, style);
            }
            // If there is nothing connected to the input port show editable fields
            else
            {
                //draws node editable fields
                DataTypeNodeEditorMethods.DrawEditableFieldsAndToggles(m_Vector3Node, this, style);
            }

        }

        
    }
}

