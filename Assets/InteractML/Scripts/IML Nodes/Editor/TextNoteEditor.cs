using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeEditor(typeof(TextNote))]
    public class TextNoteEditor: NodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private TextNote m_TextNote;

        Vector2 scrollWindow;
        string noteText;

        private GUISkin skin;

        private Color nodeColor;
        private Color lineColor;

        private Texture2D nodeTexture;
        private Texture2D lineTexture;

        private Rect headerSection;
        private Rect bodySection;

        private float nodeWidth;
        private float lineWeight;

        float dynamicSize;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_TextNote = (target as TextNote);

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
            GUILayout.Label("  TEXT NOTE", skin.GetStyle("Header"), GUILayout.Height(headerSection.height));
        }

        public override void OnBodyGUI()
        {
            DrawBodyLayout();
            dynamicSize = 30;
            ShowTextNote();
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
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayout()
        {
            bodySection.x = 5;
            bodySection.y = headerSection.height;
            bodySection.width = nodeWidth - 10;
            bodySection.height = 30 + (dynamicSize * 1.3f);

            // Draw body background purple rect below header
            GUI.DrawTexture(bodySection, nodeTexture);

            // Draw line at top of body
            GUI.DrawTexture(new Rect(bodySection.x, bodySection.y - lineWeight, bodySection.width, lineWeight), lineTexture);
        }

        /// <summary>
        /// Show the text note
        /// </summary>
        private void ShowTextNote()
        {
            EditorGUILayout.Space();
            noteText = (target as TextNote).note;
            // Show how the text area is shown
            //scrollWindow = EditorGUILayout.BeginScrollView(scrollWindow);
            //EditorGUILayout.EndScrollView();

            GUIStyle textNoteGUIStyle = GUI.skin.textArea;

            textNoteGUIStyle.wordWrap = true;
            dynamicSize = textNoteGUIStyle.CalcHeight(new GUIContent(noteText), 204f);

            noteText = EditorGUILayout.TextArea(noteText, textNoteGUIStyle, GUILayout.Height(dynamicSize * 1.3f));

            (target as TextNote).note = noteText;
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

