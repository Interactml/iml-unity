using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace MalbersAnimations.Utilities
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TriggerProxy))]
    public class TriggerProxyEditor : Editor
    {
        MonoScript script;
        private void OnEnable()
        {
            script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Used for catch when a gameobject enters, stays, or Exit A collider/Trigger", MessageType.None);
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.BeginDisabledGroup(true);
                script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("active"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Ignore"), new GUIContent("Ignore","GameObjects to Ignore with this layers"));
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTrigger_Enter"), new GUIContent("On Trigger Enter"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTrigger_Stay"), new GUIContent("On Trigger Stay"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTrigger_Exit"), new GUIContent("On Trigger Exit"));
                EditorGUILayout.EndVertical();


            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }

}