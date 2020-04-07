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
    public class ExtractRotationNodeEditor : NodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractRotation m_ExtractRotation;

        private GUISkin skin;

        private Color bgColor;
        private Color lineColor;

        private Texture2D nodeTexture;
        private Texture2D lineTexture;

        private float nodeWidth;
        private float nodeHeaderHeight;

        public override void OnCreate()
        {
            base.OnCreate();

            ColorUtility.TryParseHtmlString("#3A3B5B", out bgColor);
            nodeTexture = new Texture2D(1, 1);
            nodeTexture.SetPixel(0, 0, bgColor);
            nodeTexture.Apply();

            ColorUtility.TryParseHtmlString("#888EF7", out lineColor);
            lineTexture = new Texture2D(1, 1);
            lineTexture.SetPixel(0, 0, lineColor);
            lineTexture.Apply();

            nodeWidth = 250;
            nodeHeaderHeight = 60;

        }

        public override void OnHeaderGUI()
        {
            // Draw header background purple rect
            GUI.DrawTexture(new Rect(5, 0, nodeWidth-10, nodeHeaderHeight), nodeTexture);

            // Get reference to GUIStyle
            skin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

            //Display Node name
            GUILayout.Label("LIVE ROTATION DATA", skin.GetStyle("Header"), GUILayout.Height(nodeHeaderHeight));

        }
        
        public override void OnBodyGUI()
        {

            // Get reference to the current node
            m_ExtractRotation = (target as ExtractRotation);

            // Draw header background purple rect
            GUI.DrawTexture(new Rect(5, nodeHeaderHeight, nodeWidth - 10, 262), nodeTexture);

            // Draw line below header
            GUI.DrawTexture(new Rect(5, nodeHeaderHeight, nodeWidth - 10, 1), lineTexture);

            // Draw the ports
            EditorGUILayout.Space();
            ShowExtractRotationNodePorts();

            // Draw line below nodes
            GUI.DrawTexture(new Rect(5, 100, nodeWidth - 10, 1), lineTexture);

            // Show extracted position values
            ShowExtractedRotationValues();
            EditorGUILayout.Space();
            // Draw line below values
            GUI.DrawTexture(new Rect(5, 100, nodeWidth - 10, 1), lineTexture);

            // Show local space toggle
            EditorGUILayout.Space();
            ShowLocalSpaceToggle();


        }

        

        #region Methods

        private void ShowExtractRotationNodePorts()
        {
            GUILayout.BeginHorizontal();
            
            NodeEditorGUILayout.PortField(m_ExtractRotation.GetInputPort("GameObjectDataIn"), GUILayout.MinWidth(0));
            NodeEditorGUILayout.PortField(m_ExtractRotation.GetOutputPort("LiveDataOut"), GUILayout.MinWidth(0));
            GUILayout.EndHorizontal();

           
        }

        private void ShowExtractedRotationValues()
        {
            Quaternion quaternion = new Quaternion(m_ExtractRotation.FeatureValues.Values[0], m_ExtractRotation.FeatureValues.Values[1], m_ExtractRotation.FeatureValues.Values[2], m_ExtractRotation.FeatureValues.Values[3]);
            Vector3 euler = new Vector3();
            quaternion.eulerAngles = euler;

            EditorGUILayout.LabelField(" x: " + euler.x, skin.GetStyle("Node Body Label"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(" y: " + euler.y, skin.GetStyle("Node Body Label"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(" z: " + euler.z, skin.GetStyle("Node Body Label"));
            EditorGUILayout.Space();

        }

        private void ShowLocalSpaceToggle()
        {
            m_ExtractRotation.LocalSpace = EditorGUILayout.ToggleLeft("Use local space for transform", m_ExtractRotation.LocalSpace, skin.GetStyle("Node Sub Label"));
        }


        #endregion

    }

}
