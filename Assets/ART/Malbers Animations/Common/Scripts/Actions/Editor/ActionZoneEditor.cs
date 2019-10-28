using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace MalbersAnimations
{
    [CustomEditor(typeof(ActionZone))]
    [CanEditMultipleObjects]
    public class ActionZoneEditor : Editor
    {
        private ActionZone M;

        string[] actionNames;
        MonoScript script;
        private void OnEnable()
        {
            M = ((ActionZone)target);
            script = MonoScript.FromMonoBehaviour((ActionZone)target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Actions && Emotions for activating the Zones\nJust for gameObjects with the Animal Script ", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.BeginDisabledGroup(true);
                script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ID"), new GUIContent("Action"));
               // M.actionsToUse = (Actions)EditorGUILayout.ObjectField("Actions Pack", M.actionsToUse, typeof(Actions), false);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("HeadOnly"), new GUIContent("Head Only", "Enable only when the 'Head' bone enter the trigger zone"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ActiveOnJump"), new GUIContent("Active on Jump", "Can be used while jumping, (Grab Ledge, WallJumping)"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MoveToExitAction"), new GUIContent("Move to exit Action", "This Action exits only when the animal moves... Useful for AI animal Control"));
                   // M.ActiveOnJump = EditorGUILayout.Toggle(new GUIContent("Active on Jump", "Can be used while jumping, (Grab Ledge, WallJumping)"), M.ActiveOnJump);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                var auto = serializedObject.FindProperty("automatic");

                EditorGUILayout.PropertyField(auto, new GUIContent("Automatic", "As soon as the animal enters the zone play the action"));
                //M.automatic = EditorGUILayout.Toggle(new GUIContent("Automatic", "As soon as the animal enters the zone play the action"), M.automatic);

                if (auto.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AutomaticDisabled"), new GUIContent("Disabled", "if true the Trigger will be disabled for this value in seconds"));

                    //M.AutomaticDisabled = EditorGUILayout.FloatField(new GUIContent("Disabled", "if true the Trigger will be disabled for this value in seconds"), M.AutomaticDisabled);
                    if (M.AutomaticDisabled < 0)
                    {
                        M.AutomaticDisabled = 0;
                        serializedObject.ApplyModifiedProperties();
                    }

                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();

                    var aling = serializedObject.FindProperty("Align");
                   // M.Align = EditorGUILayout.Toggle(new GUIContent("Align", "Aligns the Animal to the Align Position and Rotation of the AlignPoint"), M.Align);
                    EditorGUILayout.PropertyField(aling, new GUIContent("Align", "Aligns the Animal to the Align Position and Rotation of the AlignPoint"));
                     

                    if (aling.boolValue)
                    {
                        M.AlignPos = GUILayout.Toggle(M.AlignPos, new GUIContent("P", "Align Position"), EditorStyles.miniButton, GUILayout.MaxWidth(25));
                        M.AlignRot = GUILayout.Toggle(M.AlignRot, new GUIContent("R", "Align Rotation"), EditorStyles.miniButton, GUILayout.MaxWidth(25));
                        if (M.AlignPos || M.AlignRot)
                        {
                            M.AlignLookAt = false;
                        }

                        M.AlignLookAt = GUILayout.Toggle(M.AlignLookAt, new GUIContent("L", "Align Looking at the Zone"), EditorStyles.miniButton, GUILayout.MaxWidth(25));
                        if (M.AlignLookAt)
                        {
                            M.AlignPos = M.AlignRot = false;
                        }


                        if (M.AlingPoint== null)
                        {
                            M.AlingPoint = M.transform;
                        }
                        serializedObject.ApplyModifiedProperties();
                    }

                    EditorGUILayout.EndHorizontal();
                    if (M.Align)
                    {
                        M.AlingPoint =(Transform) EditorGUILayout.ObjectField("Aling Point", M.AlingPoint, typeof(Transform), true);
                        M.AlingPoint2 =(Transform) EditorGUILayout.ObjectField("Aling Point End", M.AlingPoint2, typeof(Transform), true);
                        M.AlignTime = EditorGUILayout.FloatField(new GUIContent("Align Time", "Time to aling"), M.AlignTime);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("AlignCurve"), new GUIContent("Align Curve"));

                        if (M.AlignTime < 0)  M.AlignTime = 0;
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                M.EditorShowEvents = EditorGUILayout.Foldout(M.EditorShowEvents, "Events");
                EditorGUI.indentLevel--;

                if (M.EditorShowEvents)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnEnter"), new GUIContent("On Animal Enter"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnExit"), new GUIContent("On Animal Exit"));
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ActionDelay"), new GUIContent("Action Delay Event"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("OnAction"), new GUIContent("On Animal Action"));
                }
                EditorGUILayout.EndVertical();


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.indentLevel++;
                M.EditorAI = EditorGUILayout.Foldout(M.EditorAI, "AI");
                if (M.EditorAI)
                {
                    EditorGUI.indentLevel--;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("stoppingDistance"), new GUIContent("Stopping Distance"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("waitTime"), new GUIContent("Wait Time", "Wait Time after the Action ended"));
                     EditorGUILayout.PropertyField(serializedObject.FindProperty("pointType"), new GUIContent("Zone Type", "Which type is this action zone"));
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("nextTargets"), new GUIContent("Next Targets","Posible Targets to go after finishing the Action"),true);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}