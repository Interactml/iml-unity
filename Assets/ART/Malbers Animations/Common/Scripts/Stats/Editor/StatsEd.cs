using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MalbersAnimations
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Stats))]
    public class StatsEd : Editor
    {
        MonoScript script;
        private ReorderableList list;
        private Stats M;
        private SerializedProperty statList;

        private void OnEnable()
        {
            M = (Stats)target;
            script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);

            statList = serializedObject.FindProperty("stats");


            list = new ReorderableList(serializedObject, statList, true, true, true, true);
            list.drawElementCallback = DrawElementCallback;
            list.drawHeaderCallback = HeaderCallbackDelegate;
            list.onAddCallback = OnAddCallBack;
        }



        void HeaderCallbackDelegate(Rect rect)
        {
            Rect R_1 = new Rect(rect.x + 25, rect.y, (rect.width - 10) / 2, EditorGUIUtility.singleLineHeight);
            Rect R_2 = new Rect(rect.width / 2 + 25, rect.y, rect.x + (rect.width / 4) - 5, EditorGUIUtility.singleLineHeight);
            // Rect R_3 = new Rect((rect.width / 2) + (rect.width / 4)+30, rect.y+1, (rect.width / 4), EditorGUIUtility.singleLineHeight - 2);

            EditorGUI.LabelField(R_1, "Name", EditorStyles.miniLabel);
            EditorGUI.LabelField(R_2, "Value", EditorStyles.centeredGreyMiniLabel);
            //EditorGUI.LabelField(R_3, "Default", EditorStyles.centeredGreyMiniLabel);
        }

        void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = statList.GetArrayElementAtIndex(index);
            var name = element.FindPropertyRelative("name");
            var active = element.FindPropertyRelative("active");
            var Value = element.FindPropertyRelative("value");
            var MaxValue = element.FindPropertyRelative("maxValue");

            rect.y += 2;

            Rect R_0 = new Rect(rect.x - 3, rect.y, 15, EditorGUIUtility.singleLineHeight);
            Rect R_1 = new Rect(rect.x + 13, rect.y, (rect.width - 65) / 2 + 5, EditorGUIUtility.singleLineHeight);
            Rect R_2 = new Rect(rect.x + 25 + ((rect.width - 30) / 2), rect.y, rect.width - ((rect.width) / 2) - 8, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(R_0, active, new GUIContent("", "Is the Stat Enabled? when Disable no modification can be done"));
            EditorGUI.PropertyField(R_1, name, GUIContent.none);
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(R_2, Value, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                MaxValue.FindPropertyRelative("ConstantValue").floatValue = Value.FindPropertyRelative("ConstantValue").floatValue;
            }

        }


        void OnAddCallBack(ReorderableList list)
        {
            if (M.stats == null)
            {
                M.stats = new List<Stat>();
            }
            M.stats.Add(new Stat());
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Stats Manager", MessageType.None);
            EditorGUILayout.EndVertical();


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.BeginDisabledGroup(true);
                script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
                EditorGUI.EndDisabledGroup();


                list.DoLayoutList();

                if (list.index != -1)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    var element = statList.GetArrayElementAtIndex(list.index);
                    var id = element.FindPropertyRelative("ID");
                    var name = element.FindPropertyRelative("name").stringValue;
                    var ShowEvents = element.FindPropertyRelative("ShowEvents");
                    var BelowValue = element.FindPropertyRelative("Below");
                    var AboveValue = element.FindPropertyRelative("Above");

                    var Value = element.FindPropertyRelative("value");
                    var MaxValue = element.FindPropertyRelative("maxValue");
                    var MinValue = element.FindPropertyRelative("minValue");
                    // var useConstant = Value.FindPropertyRelative("UseConstant");
                    //  var variable = Value.FindPropertyRelative("Variable");

                    EditorGUILayout.PropertyField(id, new GUIContent("ID", "ID to identify the Stat"));

                    EditorGUI.BeginChangeCheck();

                    EditorGUILayout.PropertyField(Value, new GUIContent("Value", "Current Value of the Stat"));

                    //float newValue = Value.floatValue;
                    //if (M.stats[list.index].Value != newValue) M.stats[list.index].Value != newValue;


                    if (EditorGUI.EndChangeCheck())
                    {
                        MaxValue.FindPropertyRelative("ConstantValue").floatValue = Value.FindPropertyRelative("ConstantValue").floatValue;
                    }

                    EditorGUILayout.PropertyField(MaxValue, new GUIContent("Max Value", "Default/Reset value of the Stat"));
                    EditorGUILayout.PropertyField(MinValue, new GUIContent("Min Value", "Minimun value of the Stat"));

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    var Regenerate = element.FindPropertyRelative("regenerate");
                    EditorGUILayout.PropertyField(Regenerate, new GUIContent("Regenerate", "Can the Stat Regenerate over time?"));

                    bool regen = Regenerate.boolValue;
                    if (M.stats[list.index].Regenerate != regen)
                    {
                        M.stats[list.index].Regenerate = regen;
                        serializedObject.ApplyModifiedProperties();
                    }

                    // if (Regenerate.boolValue)
                    {
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("RegenRate"), new GUIContent("Rate", "Regeneration Rate, how fast/Slow the Stat will regenerate"));
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("RegenWaitTime"), new GUIContent("Wait Time", "After the Stat is modified, the time to wait to start regenerating"));
                    }
                    EditorGUILayout.EndVertical();


                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    var Degenerate = element.FindPropertyRelative("degenerate");
                    EditorGUILayout.PropertyField(Degenerate, new GUIContent("Degenerate", "Can the Stat Degenerate over time?"));
                    bool degen = Degenerate.boolValue;


                    Degenerate.boolValue = degen;
                    M.stats[list.index].Degenerate = degen;
                    serializedObject.ApplyModifiedProperties();

                    EditorGUILayout.PropertyField(element.FindPropertyRelative("DegenRate"), new GUIContent("Rate", "Degeneration Rate, how fast/Slow the Stat will Degenerate"));

                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.indentLevel++;
                    ShowEvents.boolValue = EditorGUILayout.Foldout(ShowEvents.boolValue, "Events");
                    EditorGUI.indentLevel--;

                    if (ShowEvents.boolValue)
                    {

                        EditorGUILayout.PropertyField(element.FindPropertyRelative("OnValueChange"));
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("OnValueChangeNormalized"));
                        MalbersEditor.DrawSplitter();
                        EditorGUILayout.Space();
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("OnStatFull"), new GUIContent("On " + name + " Full "));
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("OnStatEmpty"), new GUIContent("On " + name + " Empty "));
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("OnDegenereate"), new GUIContent("On " + name + " Degenerate "));

                        MalbersEditor.DrawSplitter();
                        EditorGUILayout.Space();
                        EditorGUILayout.BeginHorizontal(/*EditorStyles.helpBox*/);
                        EditorGUIUtility.labelWidth = 55;
                        EditorGUILayout.PropertyField(BelowValue, new GUIContent("Below", "Used to Check when the Stat is below this value"));
                        EditorGUILayout.PropertyField(AboveValue, new GUIContent("Above", "Used to Check when the Stat is Above this value"));
                        EditorGUIUtility.labelWidth = 0;
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("OnStatBelow"), new GUIContent("On " + name + " Below " + BelowValue.floatValue.ToString("F1")));
                        EditorGUILayout.PropertyField(element.FindPropertyRelative("OnStatAbove"), new GUIContent("On " + name + " Above " + AboveValue.floatValue.ToString("F1")));

                    }

                    EditorGUILayout.EndVertical();

                }
            }
            EditorGUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(M, "Stats Modified");
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
            }
            serializedObject.ApplyModifiedProperties();

        }
    }
}