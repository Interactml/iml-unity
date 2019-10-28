using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;


namespace MalbersAnimations
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AnimalAIControl))]
    public class AnimalAIControlEd : Editor
    {
        AnimalAIControl M;
        MonoScript script;

        SerializedProperty stoppingDistance;

        bool EventHelp;

        private void OnEnable()
        {
            M = (AnimalAIControl)target;
            script = MonoScript.FromMonoBehaviour(M);

            stoppingDistance = serializedObject.FindProperty("stoppingDistance");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Basic AI system for Animal Script", MessageType.None, true);
            EditorGUILayout.EndVertical();


            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            EditorGUI.BeginDisabledGroup(true);
            script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("target"), new GUIContent("Target","Target to follow"));

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(stoppingDistance, new GUIContent("Stopping Distance", "Agent Stopping Distance"));
            if (EditorGUI.EndChangeCheck())
            {
                if (M.Agent)
                {
                    M.Agent.stoppingDistance = stoppingDistance.floatValue;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            EditorGUI.BeginChangeCheck();
          
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

             EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoSpeed"), new GUIContent("Auto Speed", "The speed changes  are handled by this script"));
            if (serializedObject.FindProperty("AutoSpeed").boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ToTrot"), new GUIContent("To Trot", "Distance to start trotting"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ToRun"), new GUIContent("To Run", "Distance to start running"));
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            M.showevents= EditorGUILayout.Foldout(M.showevents, "Events");
            EditorGUI.indentLevel--;



            if (M.showevents)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTargetPositionArrived"), new GUIContent("On Position Arrived"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTargetArrived"), new GUIContent("On Target Arrived"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnActionStart"), new GUIContent("On Action Start"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnActionEnd"), new GUIContent("On Action End"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnDebug"), new GUIContent("On Debug"));
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("debug"), new GUIContent("Debug"));
            EditorGUILayout.EndVertical();

            if (!M.Agent)
            {
                EditorGUILayout.HelpBox("There's no Agent found on the hierarchy on this gameobject\nPlease add a NavMesh Agent Component", MessageType.Error);
            }


            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Animal AI Control Changed");
                EditorUtility.SetDirty(target);

              

            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}