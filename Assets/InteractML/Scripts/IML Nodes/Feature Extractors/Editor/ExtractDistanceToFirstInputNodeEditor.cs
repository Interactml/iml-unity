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
    public class ExtractDistanceToFirstInputNodeEditor : NodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractDistanceToFirstInput m_ExtractDistanceToFirstInput;

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
            m_ExtractDistanceToFirstInput = (target as ExtractDistanceToFirstInput);

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
            GUILayout.Label("   LIVE DISTANCE DATA", skin.GetStyle("Header"), GUILayout.Height(headerSection.height));
        }

        public override void OnBodyGUI()
        {
            // Get reference to the current node
            m_ExtractDistanceToFirstInput = (target as ExtractDistanceToFirstInput);

            // Draw the ports
            DrawPortLayout();
            ShowDistanceBetweenInputsNodePorts();

            // Show extracted position values
            DrawBodyLayout();
            EditorGUILayout.Space();
            ShowDistanceBetweenInputsValue();
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
            bodySection.height = 60;

            // Draw body background purple rect below header
            GUI.DrawTexture(bodySection, nodeTexture);

            // Draw line above local space toggle
            GUI.DrawTexture(new Rect(bodySection.x, bodySection.y + bodySection.height - lineWeight, bodySection.width, lineWeight), lineTexture);

        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowDistanceBetweenInputsNodePorts()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("First Input");
            IMLNodeEditor.PortField(inputPortLabel, m_ExtractDistanceToFirstInput.GetInputPort("FirstInput"), skin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Live Data\n Out");
            IMLNodeEditor.PortField(outputPortLabel, m_ExtractDistanceToFirstInput.GetOutputPort("DistanceBetweenInputs"), skin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();

            GUIContent secondInputPortLabel = new GUIContent("Second Input");
            IMLNodeEditor.PortField(secondInputPortLabel, m_ExtractDistanceToFirstInput.GetInputPort("SecondInput"), skin.GetStyle("Port Label"), GUILayout.MinWidth(0));
        }

        /// <summary>
        /// Show the position value fields 
        /// </summary>
        private void ShowDistanceBetweenInputsValue()
        {
            EditorGUI.indentLevel++;
            GUILayout.BeginArea(bodySection);
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (m_ExtractDistanceToFirstInput.FeatureValues.Values == null || m_ExtractDistanceToFirstInput.FeatureValues.Values.Length == 0)
            {
                EditorGUILayout.LabelField("Distance between inputs: " + 0, skin.GetStyle("Node Body Label"));
            }
            else
            {
                // Go through the list output distances
                for (int i = 0; i < m_ExtractDistanceToFirstInput.FeatureValues.Values.Length; i++)
                {
                    EditorGUILayout.LabelField("Distance between inputs: " + m_ExtractDistanceToFirstInput.FeatureValues.Values[i], skin.GetStyle("Node Body Label"));
                }
            }
            GUILayout.EndArea();
            EditorGUI.indentLevel--;

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