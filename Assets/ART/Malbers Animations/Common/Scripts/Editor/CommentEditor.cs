using UnityEngine;
using UnityEditor;

namespace MalbersAnimations.Utilities
{

    [CustomEditor(typeof(Comment))]
    public class CommentEditor : Editor
    {
        private Comment script { get { return target as Comment; } }
        GUIStyle style;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
           // EditorGUILayout.Space();
            style = new GUIStyle(EditorStyles.helpBox);
            style.fontSize =13;
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
