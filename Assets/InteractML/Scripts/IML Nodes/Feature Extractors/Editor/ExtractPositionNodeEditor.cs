using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractPosition))]
    public class ExtractPositionNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractPosition m_ExtractPosition;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_BodyRect;
        private Rect m_PortRect;
        private Rect m_BottomRect;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_ExtractPosition = (target as ExtractPosition);

            // Initialise header background Rects
            InitHeaderRects();

            // Draw header background Rect
            GUI.DrawTexture(HeaderRect, NodeColor);

            // Draw line below header
            GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#888EF7"));

            //Display Node name
            GUILayout.Label("LIVE POSITION DATA", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(60));
        }
    

        public override void OnBodyGUI()
        {
            // Draw the ports
            DrawPortLayout();
            ShowExtractPositionNodePorts();

            // Draw the body
            DrawBodyLayout();
            ShowExtractedPositionValues();

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
        private void ShowExtractPositionNodePorts()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            
            GUIContent inputPortLabel = new GUIContent("GameObject \nData In");
            IMLNodeEditor.PortField(inputPortLabel, m_ExtractPosition.GetInputPort("GameObjectDataIn"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Live Data\n Out");
            IMLNodeEditor.PortField(outputPortLabel, m_ExtractPosition.GetOutputPort("LiveDataOut"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show the position value fields 
        /// </summary>
        private void ShowExtractedPositionValues()
        {
            GUILayout.BeginArea(m_BodyRect);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("x: " + System.Math.Round(m_ExtractPosition.FeatureValues.Values[0], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("y: " + System.Math.Round(m_ExtractPosition.FeatureValues.Values[1], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("z: " + System.Math.Round(m_ExtractPosition.FeatureValues.Values[2], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));

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
            m_ExtractPosition.LocalSpace = EditorGUILayout.ToggleLeft("Use local space for transform", m_ExtractPosition.LocalSpace, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Sub Label"));
            GUILayout.EndArea();
            EditorGUI.indentLevel--;
        }

       

        #endregion

    }

}
