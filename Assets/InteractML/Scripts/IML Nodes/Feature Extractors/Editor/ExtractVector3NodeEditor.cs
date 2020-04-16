using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractVector3))]
    public class ExtractVector3NodeEditor : NodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractVector3 m_ExtractVector3;

        private GUISkin skin;

        private Color nodeColor;
        private Color lineColor;

        private Texture2D nodeTexture;
        private Texture2D lineTexture;

        private Rect headerSection;
        private Rect portSection;
        private Rect bodySection;

        private float nodeWidth;
        private float lineWeight;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_ExtractVector3 = (target as ExtractVector3);

            // Get reference to GUIStyle
            skin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

            // Initatialize node textures
            InitTextures();

            //Set node dimensions
            nodeWidth = 250;

            //Set line width
            lineWeight = 2;

            //Draw header texture
            DrawHeaderLayout();

            //Display Node name
            GUILayout.Label("   LIVE VECTOR3 DATA", skin.GetStyle("Header"), GUILayout.Height(headerSection.height));
        }

        public override void OnBodyGUI()
        {
            // Draw the ports
            DrawPortLayout();
            ShowExtractVector3NodePorts();

            // Draw the body
            EditorGUI.indentLevel++;
            DrawBodyLayout();
            ShowExtractedVector3Values();
            EditorGUI.indentLevel--;

        }

        #region Methods

        /// <summary>
        /// Define rect values for node header and paint texture based on rect 
        /// </summary>
        private void DrawHeaderLayout()
        {
            // Set header rect dimensions
            headerSection.x = 5;
            headerSection.y = 5;
            headerSection.width = nodeWidth - 10;
            headerSection.height = 60;

            // Draw header background purple rect
            GUI.DrawTexture(headerSection, nodeTexture);
        }

        /// <summary>
        /// Define rect values for port section and paint textures based on rects 
        /// </summary>
        private void DrawPortLayout()
        {
            portSection.x = 5;
            portSection.y = headerSection.height;
            portSection.width = nodeWidth - 10;
            portSection.height = 60;

            // Draw body background purple rect below header
            GUI.DrawTexture(portSection, nodeTexture);

            // Draw line at top of body
            GUI.DrawTexture(new Rect(portSection.x, portSection.y - lineWeight, portSection.width, lineWeight), lineTexture);

            // Draw line below ports
            GUI.DrawTexture(new Rect(portSection.x, headerSection.height + portSection.height - lineWeight, portSection.width, lineWeight), lineTexture);
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowExtractVector3NodePorts()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Vector3 \nData In");
            IMLNodeEditor.PortField(inputPortLabel, m_ExtractVector3.GetInputPort("inputVector3"), skin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Live Data\n Out");
            IMLNodeEditor.PortField(outputPortLabel, m_ExtractVector3.GetOutputPort("vector3FeatureExtracted"), skin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayout()
        {
            bodySection.x = 5;
            bodySection.y = headerSection.height + portSection.height;
            bodySection.width = nodeWidth - 10;
            bodySection.height = 120;

            // Draw body background purple rect below header
            GUI.DrawTexture(bodySection, nodeTexture);

            // Draw line above local space toggle
            GUI.DrawTexture(new Rect(bodySection.x, bodySection.y + bodySection.height - lineWeight, bodySection.width, lineWeight), lineTexture);

        }

        /// <summary>
        /// Show the position value fields 
        /// </summary>
        private void ShowExtractedVector3Values()
        {
            GUILayout.BeginArea(bodySection);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" x: " + System.Math.Round(m_ExtractVector3.FeatureValues.Values[0], 3).ToString(), skin.GetStyle("Node Body Label"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(" y: " + System.Math.Round(m_ExtractVector3.FeatureValues.Values[1], 3).ToString(), skin.GetStyle("Node Body Label"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(" z: " + System.Math.Round(m_ExtractVector3.FeatureValues.Values[2], 3).ToString(), skin.GetStyle("Node Body Label"));
            EditorGUILayout.Space();
            GUILayout.EndArea();

        }
        /// <summary>
        /// Initatialize node textures
        /// </summary>
        private void InitTextures()
        {
            ColorUtility.TryParseHtmlString("#3A3B5B", out nodeColor);
            nodeTexture = new Texture2D(1, 1);
            nodeTexture.SetPixel(0, 0, nodeColor);
            nodeTexture.Apply();

            ColorUtility.TryParseHtmlString("#888EF7", out lineColor);
            lineTexture = new Texture2D(1, 1);
            lineTexture.SetPixel(0, 0, lineColor);
            lineTexture.Apply();
        } 

        #endregion
    }

}
