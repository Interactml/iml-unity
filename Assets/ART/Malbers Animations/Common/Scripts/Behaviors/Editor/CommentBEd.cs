using UnityEngine;
using UnityEditor;

namespace MalbersAnimations.Utilities
{
    [CustomEditor(typeof(CommentB))]
    public class CommentBEd : Editor
    {
        private CommentB script { get { return target as CommentB; } }
        GUIStyle style;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            // EditorGUILayout.Space();
            style = new GUIStyle(EditorStyles.helpBox);
            style.fontSize = 13;
            //style.fontStyle = FontStyle.Bold;
            //style.font.
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGreen);
            string text = EditorGUILayout.TextArea(script.text, style);
            EditorGUILayout.EndVertical();
            if (text != script.text)
            {
                Undo.RecordObject(script, "Edit Comments");
                script.text = text;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}