using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
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

        protected override void ShowBodyFields()
        {
            DataInToggle(m_Vector4Node.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }

        protected override void DrawFeatureValueTogglesAndLabels(GUIStyle style)
        {
            // If something is connected to the input port show incoming data
            if (m_Vector4Node.InputConnected)
            {
                IMLNodeEditorMethods.DrawFeatureValueToggleAndLabel(m_Vector4Node, this, style);
            }
            // If there is nothing connected to the input port show editable fields
            else
            {
                DataTypeNodeEditorMethods.DrawEditableFieldsAndToggles(m_Vector4Node, this, style);
            }

            

            

        }

    }
}

