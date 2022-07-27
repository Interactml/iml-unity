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
    public class TextNoteEditor: IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private TextNote m_TextNote;

        /// <summary>
        /// Rects for node layout
        /// </summary>
       // Rect m_BodyRect;

        private Texture2D m_YellowColor;

        Vector2 scrollWindow;
        string noteText;
        float dynamicSize = 30;



        public override void OnHeaderGUI()
        {

            // Get reference to the current node
            m_TextNote = (target as TextNote);

            // Initialise header background Rects
            InitHeaderRects();

            NodeColor = GetColorTextureFromHexString("#3A3B5B");
            if (m_YellowColor == null)
                m_YellowColor = GetColorTextureFromHexString("#F6C46F");

            // Set node width
            NodeWidth = 200;

            if (Event.current.type == EventType.Layout)
            {
                // Draw header background Rect
                GUI.DrawTexture(HeaderRect, NodeColor);
                // Draw line below header
                GUI.DrawTexture(LineBelowHeader, m_YellowColor);
            }

            //Display Node name
            GUILayout.BeginArea(HeaderRect);
            GUILayout.Space(10);
            GUILayout.Label("TEXT NOTE", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));
     
        }

        public override void OnBodyGUI()
        {

            // Draw body background Rect
            m_BodyRect.x = 5;
            m_BodyRect.y = HeaderRect.height;
            m_BodyRect.width = NodeWidth - 10;
            m_BodyRect.height = 30 + (dynamicSize * 1.3f);
            if (Event.current.type == EventType.Layout)
            {
                GUI.DrawTexture(m_BodyRect, NodeColor);
            }
            ShowTextNote();
        }

        #region Methods

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

        #endregion
    }

}

