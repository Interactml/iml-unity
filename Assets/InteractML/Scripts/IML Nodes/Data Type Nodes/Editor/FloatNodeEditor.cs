using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(FloatNode))]
    public class FloatNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private FloatNode m_FloatNode;


        public override void OnHeaderGUI()
        {
            m_FloatNode = (target as FloatNode);
            NodeName = "LIVE FLOAT DATA";
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            InputPortsNamesOverride = new Dictionary<string, string>();
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Float\nData In");
            base.OutputPortsNamesOverride.Add("m_Out", "Float\nData Out");
            base.nodeTips = m_FloatNode.tooltips;
            bodyheight = 100;
            base.OnBodyGUI();
        }

        protected override void ShowBodyFields()
        {
            DataInToggle(m_FloatNode.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }

        /// <summary>
        /// Define rect values for port section and paint textures based on rects 
        /// </summary>
        private void DrawPortLayout()
        {
            // Draw body background purple rect below header
            m_PortRect.x = 5;
            m_PortRect.y = HeaderRect.height;
            m_PortRect.width = NodeWidth - 10;
            m_PortRect.height = 60;
            GUI.DrawTexture(m_PortRect, NodeColor);

            // Draw line below ports
            GUI.DrawTexture(new Rect(m_PortRect.x, HeaderRect.height + m_PortRect.height - WeightOfSectionLine, m_PortRect.width, WeightOfSectionLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayout()
        {
            m_BodyRect.x = 5;
            m_BodyRect.y = HeaderRect.height + m_PortRect.height;
            m_BodyRect.width = NodeWidth - 10;
            m_BodyRect.height = 100;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_BodyRect, NodeColor);
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawHelpButtonLayout()
        {
            m_HelpRect.x = 5;
            m_HelpRect.y = HeaderRect.height + m_PortRect.height + m_BodyRect.height;
            m_HelpRect.width = NodeWidth - 10;
            m_HelpRect.height = 40;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_HelpRect, NodeColor);

            //Draw separator line
            GUI.DrawTexture(new Rect(m_HelpRect.x, HeaderRect.height + m_PortRect.height + m_BodyRect.height - WeightOfSeparatorLine, m_HelpRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

     
        protected override void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            // If something is connected to the input port show incoming data
            if (m_FloatNode.InputConnected)
            {
                GUILayout.BeginHorizontal();
                m_FloatNode.float_switch = EditorGUILayout.Toggle(m_FloatNode.float_switch, style);
                EditorGUILayout.LabelField("Float: " + System.Math.Round(m_FloatNode.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
                GUILayout.EndHorizontal();
            }
            // If there is nothing connected to the input port show editable fields
            else
            {
                GUILayout.BeginHorizontal();
                m_FloatNode.float_switch = EditorGUILayout.Toggle(m_FloatNode.float_switch, style, GUILayout.MaxWidth(40));
                EditorGUILayout.LabelField("Float: ", m_NodeSkin.GetStyle("Node Body Label"), GUILayout.MaxWidth(40));
                m_FloatNode.m_UserInput = EditorGUILayout.FloatField(m_FloatNode.m_UserInput, GUILayout.MaxWidth(60));
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
