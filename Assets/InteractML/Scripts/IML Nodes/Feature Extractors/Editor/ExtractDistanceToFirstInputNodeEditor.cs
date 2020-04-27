using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractDistanceToFirstInput))]
    public class ExtractDistanceToFirstInputNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractDistanceToFirstInput m_ExtractDistanceToFirstInput;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_BodyRect;
        private Rect m_PortRect;
        private Rect m_InnerBodyRect;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_ExtractDistanceToFirstInput = (target as ExtractDistanceToFirstInput);

            // Initialise header background Rects
            InitHeaderRects();

            NodeColor = GetColorTextureFromHexString("#3A3B5B");

            // Draw header background Rect
            GUI.DrawTexture(HeaderRect, NodeColor);

            // Draw line below header
            GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#888EF7"));

            //Display Node name
            GUILayout.Label("LIVE DISTANCE DATA", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(NodeWidth), GUILayout.MinHeight(60));

        }

        public override void OnBodyGUI()
        {
            // Draw the ports
            DrawPortLayout();
            ShowDistanceBetweenInputsNodePorts();

            // Draw the body
            DrawBodyLayout();
            ShowDistanceBetweenInputsValue();
 
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
            m_BodyRect.height = 70;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_BodyRect, NodeColor);
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowDistanceBetweenInputsNodePorts()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("First Input");
            IMLNodeEditor.PortField(inputPortLabel, m_ExtractDistanceToFirstInput.GetInputPort("FirstInput"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Live Data\n Out");
            IMLNodeEditor.PortField(outputPortLabel, m_ExtractDistanceToFirstInput.GetOutputPort("DistanceBetweenInputs"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();

            GUIContent secondInputPortLabel = new GUIContent("Second Input");
            IMLNodeEditor.PortField(secondInputPortLabel, m_ExtractDistanceToFirstInput.GetInputPort("SecondInput"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));
        }

        /// <summary>
        /// Show the position value fields 
        /// </summary>
        private void ShowDistanceBetweenInputsValue()
        {
            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);


            if (m_ExtractDistanceToFirstInput.FeatureValues.Values == null || m_ExtractDistanceToFirstInput.FeatureValues.Values.Length == 0)
            {
                EditorGUILayout.LabelField("distance between inputs: " + 0, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            }
            else
            {
                // Go through the list output distances
                for (int i = 0; i < m_ExtractDistanceToFirstInput.FeatureValues.Values.Length; i++)
                {
                    EditorGUILayout.LabelField("distance between inputs: " + m_ExtractDistanceToFirstInput.FeatureValues.Values[i], Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
                }
            }
            GUILayout.EndArea();
            
        }

        #endregion

    }

}