using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(Vector2Node))]
    public class Vector2NodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private Vector2Node m_Vector2Node;

        public override void OnHeaderGUI()
        {
            m_Vector2Node = (target as Vector2Node);
            NodeName = "LIVE VECTOR 2 DATA";
            nodeSpace = 100;
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            InputPortsNamesOverride = new Dictionary<string, string>();
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Float\nData In");
            base.OutputPortsNamesOverride.Add("m_Out", "Float\nData Out");
            base.nodeTips = m_Vector2Node.tooltips;
            m_BodyRect.height = 100;
            base.OnBodyGUI();
        }

        protected override void ShowBodyFields()
        {
            DataInToggle(m_Vector2Node.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }

        protected override void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            // If something is connected to the input port show incoming data
            if (m_Vector2Node.InputConnected)
            {
                m_Vector2Node.x_switch = EditorGUILayout.Toggle(m_Vector2Node.x_switch, style);
                EditorGUILayout.LabelField("x: " + System.Math.Round(m_Vector2Node.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                GUILayout.Space(bodySpace);
                m_Vector2Node.y_switch = EditorGUILayout.Toggle(m_Vector2Node.y_switch, style);
                EditorGUILayout.LabelField("y: " + System.Math.Round(m_Vector2Node.FeatureValues.Values[1], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label Axis"));
                GUILayout.EndHorizontal();

            }
            // If there is nothing connected to the input port show editable fields
            else { 
                m_Vector2Node.x_switch = EditorGUILayout.Toggle(m_Vector2Node.x_switch, style, GUILayout.MaxWidth(30));
                EditorGUILayout.LabelField("x: ", m_NodeSkin.GetStyle("Node Body Label Axis"), GUILayout.MaxWidth(20));
                m_Vector2Node.m_UserInput.x = EditorGUILayout.FloatField(m_Vector2Node.m_UserInput.x, GUILayout.MaxWidth(inputWidth));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                GUILayout.Space(bodySpace);
                m_Vector2Node.y_switch = EditorGUILayout.Toggle(m_Vector2Node.y_switch, style, GUILayout.MaxWidth(30));
                EditorGUILayout.LabelField("y: ", m_NodeSkin.GetStyle("Node Body Label Axis"), GUILayout.MaxWidth(20));
                m_Vector2Node.m_UserInput.y = EditorGUILayout.FloatField(m_Vector2Node.m_UserInput.y, GUILayout.MaxWidth(inputWidth));
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

