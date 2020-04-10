using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractVector2))]
    public class ExtractVector2NodeEditor : NodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractVector2 m_ExtractVector2;

        private GUISkin skin;

        private Color nodeColor;
        private Color lineColor;

        private Texture2D nodeTexture;
        private Texture2D lineTexture;

        private Rect headerSection;
        private Rect portSection;
        private Rect bodySection;
        private Rect subBodySection;

        private float nodeWidth;
        private float lineWeight;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_ExtractVector2 = (target as ExtractVector2);

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
            GUILayout.Label("   LIVE VECTOR2 DATA", skin.GetStyle("Header"), GUILayout.Height(headerSection.height));
        }

        public override void OnBodyGUI()
        {
            // Draw the ports
            DrawPortLayout();
            ShowExtractVector2NodePorts();

            // Draw the body
            EditorGUI.indentLevel++;
            DrawBodyLayout();
            ShowExtractedVector2Values();
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
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayout()
        {
            bodySection.x = 5;
            bodySection.y = headerSection.height + portSection.height;
            bodySection.width = nodeWidth - 10;
            bodySection.height = 90;

            // Draw body background purple rect below header
            GUI.DrawTexture(bodySection, nodeTexture);
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowExtractVector2NodePorts()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Vector2 \nData In");
            PortField(inputPortLabel, m_ExtractVector2.GetInputPort("inputVector2"), skin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Live Data\n Out");
            PortField(outputPortLabel, m_ExtractVector2.GetOutputPort("vector2FeatureExtracted"), skin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show the position value fields 
        /// </summary>
        private void ShowExtractedVector2Values()
        {
            GUILayout.BeginArea(bodySection);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(" 1: " + System.Math.Round(m_ExtractVector2.FeatureValues.Values[0], 3).ToString(), skin.GetStyle("Node Body Label"));
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(" 2: " + System.Math.Round(m_ExtractVector2.FeatureValues.Values[1], 3).ToString(), skin.GetStyle("Node Body Label"));
            EditorGUILayout.Space();

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

        /// <summary> Make a simple port field **** overriden from XNodeEditorGUILayout to edit GUIStyle of port label *****</summary>
        private static void PortField(GUIContent label, XNode.NodePort port, GUIStyle style, params GUILayoutOption[] options)
        {
            if (port == null) return;
            if (options == null) options = new GUILayoutOption[] { GUILayout.MinWidth(30) };
            Vector2 position = Vector3.zero;
            GUIContent content = label != null ? label : new GUIContent(ObjectNames.NicifyVariableName(port.fieldName));

            // If property is an input, display a regular property field and put a port handle on the left side
            if (port.direction == XNode.NodePort.IO.Input)
            {
                // Display a label
                EditorGUILayout.LabelField(content, style, options);

                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position - new Vector2(16, 0);
            }
            // If property is an output, display a text label and put a port handle on the right side
            else if (port.direction == XNode.NodePort.IO.Output)
            {
                // Display a label
                EditorGUILayout.LabelField(content, new GUIStyle(style) { alignment = TextAnchor.UpperRight }, options);

                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position + new Vector2(rect.width, 0);
            }
            NodeEditorGUILayout.PortField(position, port);
        }

        #endregion
    }

}
