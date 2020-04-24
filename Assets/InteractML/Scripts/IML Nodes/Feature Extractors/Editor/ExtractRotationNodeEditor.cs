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
        private Rect m_PortRect;
        private Rect m_BottomRect;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_ExtractRotation = (target as ExtractRotation);

            // Initialise header background Rects
            InitHeaderRects();

            // Draw header background Rect
            GUI.DrawTexture(HeaderRect, NodeColor);

            // Draw line below header
            GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#888EF7"));

            //Display Node name
            GUILayout.Label("LIVE ROTATION DATA", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(60));
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
            m_BottomRect.x = 5;
            m_BottomRect.y = HeaderRect.height + m_PortRect.height + m_BodyRect.height;
            m_BottomRect.width = NodeWidth - 10;
            m_BottomRect.height = 50;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_BottomRect, NodeColor);

            //Draw separator line
            GUI.DrawTexture(new Rect(m_BottomRect.x, HeaderRect.height + m_PortRect.height + m_BodyRect.height - WeightOfSeparatorLine, m_BottomRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }


        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowExtractRotationNodePorts()
        {
            EditorGUILayout.Space();
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
            GUILayout.BeginArea(m_BodyRect);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" x: " + System.Math.Round(new Quaternion(m_ExtractRotation.FeatureValues.Values[0], m_ExtractRotation.FeatureValues.Values[1], m_ExtractRotation.FeatureValues.Values[2], m_ExtractRotation.FeatureValues.Values[3]).eulerAngles.x, 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(" y: " + System.Math.Round(new Quaternion(m_ExtractRotation.FeatureValues.Values[0], m_ExtractRotation.FeatureValues.Values[1], m_ExtractRotation.FeatureValues.Values[2], m_ExtractRotation.FeatureValues.Values[3]).eulerAngles.y, 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(" z: " + System.Math.Round(new Quaternion(m_ExtractRotation.FeatureValues.Values[0], m_ExtractRotation.FeatureValues.Values[1], m_ExtractRotation.FeatureValues.Values[2], m_ExtractRotation.FeatureValues.Values[3]).eulerAngles.z, 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            EditorGUILayout.Space();
            GUILayout.EndArea();

        }

        /// <summary>
        /// Show the local space toggle 
        /// </summary>
        private void ShowLocalSpaceToggle()
        {
            EditorGUI.indentLevel++;
            GUILayout.BeginArea(m_BottomRect);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            m_ExtractRotation.LocalSpace = EditorGUILayout.ToggleLeft("Use local space for transform", m_ExtractRotation.LocalSpace, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Sub Label"));
            GUILayout.EndArea();
            EditorGUI.indentLevel--;
        }



        #endregion

    }

}
