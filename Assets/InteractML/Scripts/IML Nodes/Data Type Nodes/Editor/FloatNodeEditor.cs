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

        private int counter = 0;
        private int count = 3;
        private int stop = 6;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_BodyRect;
        private Rect m_PortRect;
        private Rect m_InnerBodyRect;
        private Rect m_HelpRect;


        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_FloatNode = (target as FloatNode);

            // Load node skin
            if (m_NodeSkin == null)
                m_NodeSkin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

            // Initialise header background Rects
            InitHeaderRects();

            NodeColor = GetColorTextureFromHexString("#3A3B5B");

            // Draw header background Rect
            GUI.DrawTexture(HeaderRect, NodeColor);

            // Draw line below header
            GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#888EF7"));

            //Display Node name
            GUILayout.BeginArea(HeaderRect);
            GUILayout.Space(10);
            GUILayout.Label("LIVE FLOAT DATA", m_NodeSkin.GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));
        }

        public override void OnBodyGUI()
        {
            // Draw Port Section
            DrawPortLayout();
            ShowFloatNodePorts();

            //check if port is hovered over 
            PortTooltip(m_FloatNode.tips.PortTooltip);

            // Draw Body Section
            DrawBodyLayout();
            DataInToggle(m_FloatNode.ReceivingData, m_InnerBodyRect, m_BodyRect);
            //ShowFloatValue();

            // Draw help button
            DrawHelpButtonLayout();
            ShowHelpButton(m_HelpRect);

            // if hovering port show port tooltip
            if (showPort)
            {
                ShowTooltip(m_PortRect, TooltipText);
            }
            //if hovering over help show tooltip 
            if (showHelp)
            {
                ShowTooltip(m_HelpRect, m_FloatNode.tips.HelpTooltip);
            }
            // if hovering over body rect
            if (IsThisRectHovered(m_BodyRect))
                ShowTooltip(m_BodyRect, m_FloatNode.tips.BodyTooltip.Tips[0]);
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

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowFloatNodePorts()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Float\nData In", m_FloatNode.tips.PortTooltip[0]);
            IMLNodeEditor.PortField(inputPortLabel, m_FloatNode.GetInputPort("m_In"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Float\nData Out", m_FloatNode.tips.PortTooltip[1]);
            IMLNodeEditor.PortField(outputPortLabel, m_FloatNode.GetOutputPort("m_Out"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
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
