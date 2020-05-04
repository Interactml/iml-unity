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

        private int counter = 0;
        private int count = 3;
        private int stop = 6;

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
            m_ExtractPosition = (target as ExtractPosition);

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
            GUILayout.Label("LIVE POSITION DATA", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));
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
            DrawLocalSpaceLayout();
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

            GUI.DrawTexture(new Rect(m_PortRect.x, HeaderRect.height + m_PortRect.height - WeightOfSeparatorLine, m_PortRect.width, WeightOfSeparatorLine), SeparatorLineColor);

            
            if (m_ExtractPosition.ReceivingData)
            {
                foreach (var port in m_ExtractPosition.Ports)
                {
                    if (counter >= count)
                    {
                        if (port.IsInput)
                        {
                            Vector2 positionOut = IMLNodeEditor.GetPortPosition(port);
                            positionOut = positionOut - new Vector2(2, -1);
                            Rect circle = new Rect(positionOut, new Vector2(20, 20));
                            Color col = GUI.color;
                            GUI.color = Color.white;
                            GUI.DrawTexture(circle, NodeEditorResources.dotOuter);

                            GUI.color = col;
                        }

                        if (counter == stop)
                        {
                            counter = 0;
                        }
                    } 
                }
                counter++;
            } 

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
        private void DrawLocalSpaceLayout()
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
        private void ShowExtractPositionNodePorts()
        {
            GUILayout.Space(5);
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
            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);

            if (m_ExtractPosition.ReceivingData)
            {
                ShowPositionValueTogglesAndLabels(Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Green Toggle"));
            }
            else
            {
                ShowPositionValueTogglesAndLabels(Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Red Toggle"));
            }

            GUILayout.EndArea();

        }

        /// <summary>
        /// Show the local space toggle 
        /// </summary>
        private void ShowPositionValueTogglesAndLabels(GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            m_ExtractPosition.x_switch = EditorGUILayout.Toggle(m_ExtractPosition.x_switch, style);
            EditorGUILayout.LabelField(new GUIContent("x: " + System.Math.Round(m_ExtractPosition.FeatureValues.Values[0], 3).ToString(), "TOOLTIP"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            m_ExtractPosition.y_switch = EditorGUILayout.Toggle(m_ExtractPosition.y_switch, style);
            EditorGUILayout.LabelField("y: " + System.Math.Round(m_ExtractPosition.FeatureValues.Values[1], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            m_ExtractPosition.z_switch = EditorGUILayout.Toggle(m_ExtractPosition.z_switch, style);
            EditorGUILayout.LabelField("z: " + System.Math.Round(m_ExtractPosition.FeatureValues.Values[2], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
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
            m_ExtractPosition.LocalSpace = EditorGUILayout.Toggle(m_ExtractPosition.LocalSpace, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Local Space Toggle"));
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
