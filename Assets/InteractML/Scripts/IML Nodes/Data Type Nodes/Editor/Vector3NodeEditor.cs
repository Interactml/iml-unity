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
        /// Reference to the node itself
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
        }

        protected override void ShowBodyFields()
        {
            DataInToggle(m_Vector3Node.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }
        protected override void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            
            // If something is connected to the input port show incoming data
            if (m_Vector3Node.InputConnected)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(bodySpace);
                m_Vector3Node.x_switch = EditorGUILayout.Toggle(m_Vector3Node.x_switch, style);
                EditorGUILayout.LabelField("x: " + System.Math.Round(m_Vector3Node.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                GUILayout.Space(bodySpace);
                m_Vector3Node.y_switch = EditorGUILayout.Toggle(m_Vector3Node.y_switch, style);
                EditorGUILayout.LabelField("y: " + System.Math.Round(m_Vector3Node.FeatureValues.Values[1], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                GUILayout.Space(bodySpace);
                m_Vector3Node.z_switch = EditorGUILayout.Toggle(m_Vector3Node.z_switch, style);
                EditorGUILayout.LabelField("z: " + System.Math.Round(m_Vector3Node.FeatureValues.Values[2], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
                GUILayout.EndHorizontal();

            }
            // If there is nothing connected to the input port show editable fields
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(bodySpace);
                m_Vector3Node.x_switch = EditorGUILayout.Toggle(m_Vector3Node.x_switch, style, GUILayout.MaxWidth(40));
                EditorGUILayout.LabelField("x: ", m_NodeSkin.GetStyle("Node Body Label Axis"), GUILayout.MaxWidth(20));
                m_Vector3Node.m_UserInput.x = EditorGUILayout.FloatField(m_Vector3Node.m_UserInput.x, GUILayout.MaxWidth(60));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                GUILayout.Space(bodySpace);
                m_Vector3Node.y_switch = EditorGUILayout.Toggle(m_Vector3Node.y_switch, style, GUILayout.MaxWidth(40));
                EditorGUILayout.LabelField("y: ", m_NodeSkin.GetStyle("Node Body Label Axis"), GUILayout.MaxWidth(20));
                m_Vector3Node.m_UserInput.y = EditorGUILayout.FloatField(m_Vector3Node.m_UserInput.y, GUILayout.MaxWidth(60));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                GUILayout.Space(bodySpace);
                m_Vector3Node.z_switch = EditorGUILayout.Toggle(m_Vector3Node.z_switch, style, GUILayout.MaxWidth(40));
                EditorGUILayout.LabelField("z: ", m_NodeSkin.GetStyle("Node Body Label Axis"), GUILayout.MaxWidth(20));
                m_Vector3Node.m_UserInput.z = EditorGUILayout.FloatField(m_Vector3Node.m_UserInput.z, GUILayout.MaxWidth(60));
                GUILayout.EndHorizontal();
   
            }

           
        }

        /// <summary>
        /// Display help button
        /// </summary>
        private void ShowHelpButton()
        {
            m_HelpRect.x = m_HelpRect.x + 20;
            m_HelpRect.y = m_HelpRect.y + 10;
            m_HelpRect.width = m_HelpRect.width - 30;

            GUILayout.BeginArea(m_HelpRect);
            GUILayout.BeginHorizontal();
            GUILayout.Label("");
            GUILayout.Button("Help", m_NodeSkin.GetStyle("Help Button"));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}

