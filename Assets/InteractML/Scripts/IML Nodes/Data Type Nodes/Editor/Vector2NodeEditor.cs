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
            m_Vector2Node = (target as Vector2Node);

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
            GUILayout.Label("LIVE VECTOR2 DATA", m_NodeSkin.GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));
        
        }

        public override void OnBodyGUI()
        {
            // Draw Port Section
            DrawPortLayout();
            ShowVector2NodePorts();

            // Draw Body Section
            DrawBodyLayout();
            //ShowVector2Values();
            DataInToggle(m_Vector2Node.ReceivingData, m_InnerBodyRect, m_BodyRect);

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
                ShowTooltip(m_HelpRect, m_Vector2Node.tips.HelpTooltip);
            }
            // if hovering over body rect
            if (IsThisRectHovered(m_BodyRect))
                ShowTooltip(m_BodyRect, m_Vector2Node.tips.BodyTooltip.Tips[0]);
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
            m_BodyRect.height = 130;

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
        private void ShowVector2NodePorts()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Vector2\nData In", m_Vector2Node.tips.PortTooltip[0]);
            IMLNodeEditor.PortField(inputPortLabel, m_Vector2Node.GetInputPort("m_In"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Vector2\nData Out", m_Vector2Node.tips.PortTooltip[1]);
            IMLNodeEditor.PortField(outputPortLabel, m_Vector2Node.GetOutputPort("m_Out"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
        }

        protected override void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            // If something is connected to the input port show incoming data
            if (m_Vector2Node.InputConnected)
            {
                GUILayout.BeginHorizontal();
                m_Vector2Node.x_switch = EditorGUILayout.Toggle(m_Vector2Node.x_switch, style);
                EditorGUILayout.LabelField("x: " + System.Math.Round(m_Vector2Node.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                m_Vector2Node.y_switch = EditorGUILayout.Toggle(m_Vector2Node.y_switch, style);
                EditorGUILayout.LabelField("y: " + System.Math.Round(m_Vector2Node.FeatureValues.Values[1], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
                GUILayout.EndHorizontal();

            }
            // If there is nothing connected to the input port show editable fields
            else
            {
                GUILayout.BeginHorizontal();
                m_Vector2Node.x_switch = EditorGUILayout.Toggle(m_Vector2Node.x_switch, style, GUILayout.MaxWidth(40));
                EditorGUILayout.LabelField("x: ", m_NodeSkin.GetStyle("Node Body Label"), GUILayout.MaxWidth(20));
                m_Vector2Node.m_UserInput.x = EditorGUILayout.FloatField(m_Vector2Node.m_UserInput.x, GUILayout.MaxWidth(60));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                m_Vector2Node.y_switch = EditorGUILayout.Toggle(m_Vector2Node.y_switch, style, GUILayout.MaxWidth(40));
                EditorGUILayout.LabelField("y: ", m_NodeSkin.GetStyle("Node Body Label"), GUILayout.MaxWidth(20));
                m_Vector2Node.m_UserInput.y = EditorGUILayout.FloatField(m_Vector2Node.m_UserInput.y, GUILayout.MaxWidth(60));
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

