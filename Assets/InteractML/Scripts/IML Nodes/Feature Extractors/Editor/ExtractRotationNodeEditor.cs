using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractRotation))]
    public class ExtractRotationNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractRotation m_ExtractRotation;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_BodyRect;
        private Rect m_InnerBodyRect;
        private Rect m_PortRect;
        private Rect m_LocalSpaceRect;
        private Rect m_InnerLocalSpaceRect;
        private Rect m_HelpRect;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_ExtractRotation = (target as ExtractRotation);

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
            GUILayout.Label("LIVE ROTATION DATA", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));

        }


        public override void OnBodyGUI()
        {
            // Draw the ports
            DrawPortLayout();
            ShowExtractRotationNodePorts();

            // Draw the body
            DrawBodyLayout();
            ShowExtractedRotationValues();

            // Draw local space toggle
            DrawBottomLayout();
            ShowLocalSpaceToggle();

            // Draw help button
            DrawHelpButtonLayout();
            ShowHelpButton();
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
        private void DrawBottomLayout()
        {
            m_LocalSpaceRect.x = 5;
            m_LocalSpaceRect.y = HeaderRect.height + m_PortRect.height + m_BodyRect.height;
            m_LocalSpaceRect.width = NodeWidth - 10;
            m_LocalSpaceRect.height = 50;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_LocalSpaceRect, NodeColor);

            //Draw separator line
            GUI.DrawTexture(new Rect(m_LocalSpaceRect.x, HeaderRect.height + m_PortRect.height + m_BodyRect.height - WeightOfSeparatorLine, m_LocalSpaceRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawHelpButtonLayout()
        {
            m_HelpRect.x = 5;
            m_HelpRect.y = HeaderRect.height + m_PortRect.height + m_BodyRect.height + m_LocalSpaceRect.height;
            m_HelpRect.width = NodeWidth - 10;
            m_HelpRect.height = 40;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_HelpRect, NodeColor);

            //Draw separator line
            GUI.DrawTexture(new Rect(m_HelpRect.x, HeaderRect.height + m_PortRect.height + m_BodyRect.height + m_LocalSpaceRect.height - WeightOfSeparatorLine, m_HelpRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowExtractRotationNodePorts()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("GameObject \nData In");
            IMLNodeEditor.PortField(inputPortLabel, m_ExtractRotation.GetInputPort("GameObjectDataIn"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Live Data\n Out");
            IMLNodeEditor.PortField(outputPortLabel, m_ExtractRotation.GetOutputPort("LiveDataOut"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show the Rotation value fields 
        /// </summary>
        private void ShowExtractedRotationValues()
        {
            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);

            if (m_ExtractRotation.ReceivingData)
            {
                DrawPositionValueTogglesAndLabels(Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Green Toggle"));
            }
            else
            {
                DrawPositionValueTogglesAndLabels(Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Red Toggle"));
            }

            GUILayout.EndArea();

        }

        /// <summary>
        /// Show the local space toggle 
        /// </summary>
        private void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            m_ExtractRotation.x_switch = EditorGUILayout.Toggle(m_ExtractRotation.x_switch, style);
            EditorGUILayout.LabelField("x: " + System.Math.Round(m_ExtractRotation.FeatureValues.Values[0], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            m_ExtractRotation.y_switch = EditorGUILayout.Toggle(m_ExtractRotation.y_switch, style);
            EditorGUILayout.LabelField("y: " + System.Math.Round(m_ExtractRotation.FeatureValues.Values[1], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            m_ExtractRotation.z_switch = EditorGUILayout.Toggle(m_ExtractRotation.z_switch, style);
            EditorGUILayout.LabelField("z: " + System.Math.Round(m_ExtractRotation.FeatureValues.Values[2], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show the local space toggle 
        /// </summary>
        private void ShowLocalSpaceToggle()
        {
            m_InnerLocalSpaceRect.x = m_LocalSpaceRect.x + 24;
            m_InnerLocalSpaceRect.y = m_LocalSpaceRect.y + 16;
            m_InnerLocalSpaceRect.width = m_LocalSpaceRect.width;
            m_InnerLocalSpaceRect.height = m_LocalSpaceRect.height;

            GUILayout.BeginArea(m_InnerLocalSpaceRect);
            GUILayout.BeginHorizontal();
            m_ExtractRotation.LocalSpace = EditorGUILayout.Toggle(m_ExtractRotation.LocalSpace, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Local Space Toggle"));
            EditorGUILayout.LabelField("Use local space for transform", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Local Space Label"));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            
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
            GUILayout.Button("Help", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Help Button"));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        #endregion

    }

}
