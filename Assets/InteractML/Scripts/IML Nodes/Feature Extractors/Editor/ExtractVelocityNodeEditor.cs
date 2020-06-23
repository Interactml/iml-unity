using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractVelocity))]
    public class ExtractVelocityNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractVelocity m_ExtractVelocity;

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
            m_ExtractVelocity = (target as ExtractVelocity);

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
            GUILayout.Label("LIVE VELOCITY DATA", m_NodeSkin.GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));
        }

        public override void OnBodyGUI()
        {
            // Draw the ports
            DrawPortLayout();
            ShowExtractVelocityNodePorts();

            //check if port is hovered over 
            PortTooltip(m_ExtractVelocity.tips.PortTooltip);

            // Draw the body
            DrawBodyLayout();
            //ShowExtractedVelocityValues();
            DataInToggle(m_ExtractVelocity.ReceivingData, m_InnerBodyRect, m_BodyRect);

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
                ShowTooltip(m_HelpRect, m_ExtractVelocity.tips.HelpTooltip);
            }
            // if hovering over body rect
            if (IsThisRectHovered(m_BodyRect))
                ShowTooltip(m_BodyRect, m_ExtractVelocity.tips.BodyTooltip.Tips[0]);
        }


        #region Methods

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
            m_BodyRect.height = 120;

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
        private void ShowExtractVelocityNodePorts()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Live\nData In", m_ExtractVelocity.tips.PortTooltip[0]);
            IMLNodeEditor.PortField(inputPortLabel, m_ExtractVelocity.GetInputPort("FeatureToInput"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Live Data\nOut", m_ExtractVelocity.tips.PortTooltip[1]);
            IMLNodeEditor.PortField(outputPortLabel, m_ExtractVelocity.GetOutputPort("VelocityExtracted"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show the position value fields 
        /// </summary>
        private void ShowExtractedVelocityValues()
        {
            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);

            double[] velocity1 = new double[m_ExtractVelocity.FeatureValues.Values.Length];

            for (int i = 0; i < m_ExtractVelocity.FeatureValues.Values.Length; i++)
            {

                velocity1[i] = System.Math.Round(m_ExtractVelocity.FeatureValues.Values[i], 3);
            }

            if (m_ExtractVelocity.FeatureValues.Values.Length != 0)
            {
                //EditorGUILayout.LabelField(" velocity: " + System.Math.Round(m_ExtractVelocity.FeatureValues.Values[0], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
                EditorGUILayout.LabelField(" velocity: " + string.Join(",", velocity1), m_NodeSkin.GetStyle("Node Body Label"));
            } else
            {
                EditorGUILayout.LabelField("Connect feature extractor", m_NodeSkin.GetStyle("Node Body Label"));
            }
            
            GUILayout.EndArea();

        }

        protected override void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            if (m_ExtractVelocity.FeatureValues.Values.Length != 0)
            {
                GUILayout.BeginHorizontal();
                m_ExtractVelocity.x_switch = EditorGUILayout.Toggle(m_ExtractVelocity.x_switch, style);
                EditorGUILayout.LabelField("x: " + System.Math.Round(m_ExtractVelocity.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                m_ExtractVelocity.y_switch = EditorGUILayout.Toggle(m_ExtractVelocity.y_switch, style);
                EditorGUILayout.LabelField("y: " + System.Math.Round(m_ExtractVelocity.FeatureValues.Values[1], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                m_ExtractVelocity.z_switch = EditorGUILayout.Toggle(m_ExtractVelocity.z_switch, style);
                EditorGUILayout.LabelField("z: " + System.Math.Round(m_ExtractVelocity.FeatureValues.Values[2], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("Connect a feature extractor", m_NodeSkin.GetStyle("Node Body Label"));
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
        #endregion
    }

}
