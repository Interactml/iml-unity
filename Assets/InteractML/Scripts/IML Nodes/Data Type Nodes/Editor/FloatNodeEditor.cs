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
            GUILayout.Label("LIVE FLOAT DATA", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));
        }

        public override void OnBodyGUI()
        {
            // Draw Port Section
            DrawPortLayout();
            ShowFloatNodePorts();

            // Draw Body Section
            DrawBodyLayout();
            DataInToggle(m_FloatNode.ReceivingData, m_InnerBodyRect, m_BodyRect);
            //ShowFloatValue();

            // Draw help button
            DrawHelpButtonLayout();
            ShowHelpButton();
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

            GUIContent inputPortLabel = new GUIContent("Float\nData In", "Input float");
            IMLNodeEditor.PortField(inputPortLabel, m_FloatNode.GetInputPort("m_In"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Float\nData Out", "Output float");
            IMLNodeEditor.PortField(outputPortLabel, m_FloatNode.GetOutputPort("m_Out"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show the float value fields with attribute name
        /// </summary>
        //private void ShowFloatValue()
        //{
        //    m_InnerBodyRect.x = m_BodyRect.x + 20;
        //    m_InnerBodyRect.y = m_BodyRect.y + 20;
        //    m_InnerBodyRect.width = m_BodyRect.width - 20;
        //    m_InnerBodyRect.height = m_BodyRect.height - 20;

        //    GUILayout.BeginArea(m_InnerBodyRect);
        //    EditorGUILayout.Space();
        //    EditorGUILayout.LabelField(new GUIContent("Name: " + m_FloatNode.ValueName, "Attribute name"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
        //    EditorGUILayout.Space();
        //    EditorGUILayout.LabelField(new GUIContent("Float: " + System.Math.Round(m_FloatNode.FeatureValues.Values[0], 3).ToString(), "Float Value"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
        //    GUILayout.EndArea();

        //}

        protected override void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Name: " + m_FloatNode.ValueName, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            GUILayout.Space(10);
            GUILayout.BeginHorizontal(); 
            m_FloatNode.float_switch = EditorGUILayout.Toggle(m_FloatNode.float_switch, style);
            EditorGUILayout.LabelField(new GUIContent("Float: " + System.Math.Round(m_FloatNode.FeatureValues.Values[0], 3).ToString(), "Float Value"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            GUILayout.EndHorizontal(); 
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
            GUILayout.Button(new GUIContent("Help", "What does this node do?"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Help Button"));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

    }
}
