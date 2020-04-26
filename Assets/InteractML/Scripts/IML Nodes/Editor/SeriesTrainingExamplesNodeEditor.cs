using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeEditor(typeof(SeriesTrainingExamplesNode))]
    public class SeriesTrainingExamplesNodeEditor : IMLNodeEditor
    {

        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private SeriesTrainingExamplesNode m_SeriesTrainingExamplesNode;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_BodyRectOutputAddRemove;
        private Rect m_PortRect;

        /// <summary>
        /// Bool for add/remove output
        /// </summary>
        private bool m_AddOutput;
        private bool m_RemoveOutput;

        #endregion

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_SeriesTrainingExamplesNode = (target as SeriesTrainingExamplesNode);

            NodeWidth = 300;

            // Initialise header background Rects
            InitHeaderRects();

            NodeColor = GetColorTextureFromHexString("#3A3B5B");

            // Draw header background Rect
            GUI.DrawTexture(HeaderRect, NodeColor);

            // Draw line below header
            GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#74DF84"));

            //Display Node name
            GUILayout.Label("TEACH THE MACHINE DTW", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(60));
        }

        public override void OnBodyGUI()
        {
            DrawPortLayout();
            ShowSeriesTrainingExamplesNodePorts();

            DrawBodyLayoutOutputAddRemove();
            ShowOutputAddRemove();

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
            GUI.DrawTexture(new Rect(m_PortRect.x, HeaderRect.height + m_PortRect.height - WeightOfSectionLine, m_PortRect.width, WeightOfSectionLine), GetColorTextureFromHexString("#74DF84"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayoutOutputAddRemove()
        {
            m_BodyRectOutputAddRemove.x = 5;
            m_BodyRectOutputAddRemove.y = HeaderRect.height + m_PortRect.height;
            m_BodyRectOutputAddRemove.width = NodeWidth - 10;
            m_BodyRectOutputAddRemove.height = 80;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_BodyRectOutputAddRemove, NodeColor);
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowSeriesTrainingExamplesNodePorts()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Live Data\nIn");
            IMLNodeEditor.PortField(inputPortLabel, m_SeriesTrainingExamplesNode.GetInputPort("InputFeatures"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Recorded\nData Out");
            IMLNodeEditor.PortField(outputPortLabel, m_SeriesTrainingExamplesNode.GetOutputPort("TrainingExamplesNodeToOutput"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show the position value fields 
        /// </summary>
        private void ShowOutputAddRemove()
        {
            GUILayout.BeginArea(m_BodyRectOutputAddRemove, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));



            GUILayout.BeginHorizontal();


            EditorGUILayout.LabelField("Outputs", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));


            EditorGUILayout.Space();


            m_AddOutput = EditorGUILayout.Toggle(m_AddOutput, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Toggle"));
            m_RemoveOutput = EditorGUILayout.Toggle(m_RemoveOutput, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Toggle"));


            GUILayout.EndHorizontal();

            GUILayout.EndArea();

        }
    }
}
