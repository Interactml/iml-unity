using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace MalbersAnimations.Scriptables
{
    public class VariableEditor : Editor
    {
        public virtual void PaintInspectorGUI(string title)
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox(title, MessageType.None, true);
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.BeginDisabledGroup(true);
                MonoScript script =  MonoScript.FromScriptableObject(target as ScriptableObject);
                script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
                EditorGUI.EndDisabledGroup();

                //EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                //EditorGUILayout.PropertyField(serializedObject.FindProperty("Clone"), new GUIContent("Set to Clone", "The current value"));
                //EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("value"), new GUIContent("Value","The current value"));
                var useEvent = serializedObject.FindProperty("UseEvent");
                useEvent.boolValue = GUILayout.Toggle(useEvent.boolValue, new GUIContent("E", "Enable 'OnValueChanged' Event. It will be invoked only when the value changes"), EditorStyles.miniButton, GUILayout.Width(18));
                EditorGUILayout.EndHorizontal();
               // MalbersEditor.DrawSplitter();

                // EditorGUILayout.EndVertical();

                //  EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));
                //  EditorGUILayout.EndVertical();

                // EditorGUILayout.BeginVertical(EditorStyles.helpBox);


                //EditorGUILayout.PropertyField(useEvent, new GUIContent("Use Event", "If the value changes the On Value Changed Event will be invoked"));

                //EditorGUI.BeginDisabledGroup(!useEvent.boolValue);

                if (useEvent.boolValue)
                {
                    MalbersEditor.DrawSplitter();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnValueChanged"));
                }
                // EditorGUILayout.EndVertical();
               // EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(FloatVar))]
    public class FloatVarEditor : VariableEditor
    {
        public override void OnInspectorGUI()
        {
            PaintInspectorGUI("Float Variable");
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(StringVar))]
    public class StringVarEditor : VariableEditor
    {
        public override void OnInspectorGUI()
        {
            PaintInspectorGUI("String Variable");
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(BoolVar))]
    public class BoolVarEditor : VariableEditor
    {
        public override void OnInspectorGUI()
        {
            PaintInspectorGUI("Bool Variable");
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Vector3Var))]
    public class Vector3VarEditor : VariableEditor
    {
        public override void OnInspectorGUI()
        {
            PaintInspectorGUI("Vector3 Variable");
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(IntVar))]
    public class IntVarEditor : VariableEditor
    {
        public override void OnInspectorGUI()
        {
            PaintInspectorGUI("Int Variable");
        }
    }
}