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

        Vector2 scrollWindow;
        string noteText;
        

        public override void OnBodyGUI()
        { 

            noteText = (target as TextNote).note;
            // Show how the text area is shown
            //scrollWindow = EditorGUILayout.BeginScrollView(scrollWindow);
            //EditorGUILayout.EndScrollView();

            GUIStyle textNoteGUIStyle = GUI.skin.textArea;

            textNoteGUIStyle.wordWrap = true;
            float dynamicSize = textNoteGUIStyle.CalcHeight(new GUIContent(noteText), 204f);

            noteText = EditorGUILayout.TextArea(noteText, textNoteGUIStyle, GUILayout.Height(dynamicSize*1.3f));

            (target as TextNote).note = noteText;

        }
    }

}

