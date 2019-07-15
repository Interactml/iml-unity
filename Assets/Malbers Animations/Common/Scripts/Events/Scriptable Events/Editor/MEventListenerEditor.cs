using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MalbersAnimations.Events
{
    [CanEditMultipleObjects]
     [CustomEditor(typeof(MEventListener))]
    public class MEventListenerEditor : Editor
    {
        private ReorderableList list;
        private SerializedProperty eventsListeners , useFloat, useBool, useInt, useString, useVoid, useGo, useTransform, useVector;
        private MEventListener M;
        MonoScript script;


        private void OnEnable()
        {
            M = ((MEventListener)target);
            script = MonoScript.FromMonoBehaviour(M);


            //useFloat = serializedObject.FindProperty("useFloat");
            //useBool = serializedObject.FindProperty("useBool");
            //useInt = serializedObject.FindProperty("useInt");
            //useString = serializedObject.FindProperty("useString");
            //useVoid = serializedObject.FindProperty("useVoid");

            eventsListeners = serializedObject.FindProperty("Events");

            list = new ReorderableList(serializedObject, eventsListeners, true, true, true, true);
            list.drawElementCallback = drawElementCallback;
            list.drawHeaderCallback = HeaderCallbackDelegate;
            list.onAddCallback = OnAddCallBack;
        }


        void HeaderCallbackDelegate(Rect rect)
        {
            EditorGUI.LabelField(rect, "   Event Listeners");
        }

        void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 2;
            rect.height -= 5;
            SerializedProperty Element = eventsListeners.GetArrayElementAtIndex(index).FindPropertyRelative("Event");
            eventsListeners.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, Element, GUIContent.none);
        }

        void OnAddCallBack(ReorderableList list)
        {
            if (M.Events == null)
            {
                M.Events = new  List<MEventItemListener>();
            }

            M.Events.Add(new MEventItemListener());
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Events Listeners. They Response to the MEvents Assets\nThe MEvents should not repeat on the list", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.BeginDisabledGroup(true);
                script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    //EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"));
                    list.DoLayoutList();

                    if (list.index != -1)
                    {
                        if (list.index < list.count)
                        {
                            SerializedProperty Element = eventsListeners.GetArrayElementAtIndex(list.index);

                            if (M.Events[list.index].Event != null)
                            {

                                string Descp = M.Events[list.index].Event.Description;


                                if (Descp != string.Empty)
                                {
                                    EditorGUILayout.BeginVertical(MalbersEditor.StyleGreen);
                                    EditorGUILayout.HelpBox(M.Events[list.index].Event.Description, MessageType.None);
                                    EditorGUILayout.EndVertical();
                                }

                                EditorGUILayout.Space();

                                useFloat = Element.FindPropertyRelative("useFloat");
                                useBool = Element.FindPropertyRelative("useBool");
                                useInt = Element.FindPropertyRelative("useInt");
                                useString = Element.FindPropertyRelative("useString");
                                useVoid = Element.FindPropertyRelative("useVoid");
                                useGo = Element.FindPropertyRelative("useGO");
                                useTransform = Element.FindPropertyRelative("useTransform");
                                useVector = Element.FindPropertyRelative("useVector");

                                EditorGUILayout.BeginHorizontal();
                                useVoid.boolValue = GUILayout.Toggle(useVoid.boolValue, new GUIContent("Void", "No Parameters Response"), EditorStyles.toolbarButton);
                                useBool.boolValue = GUILayout.Toggle(useBool.boolValue, new GUIContent("Bool", "Bool Response"), EditorStyles.toolbarButton);
                                useFloat.boolValue = GUILayout.Toggle(useFloat.boolValue, new GUIContent("Float", "Float Response"), EditorStyles.toolbarButton);
                                useInt.boolValue = GUILayout.Toggle(useInt.boolValue, new GUIContent("Int", "Int Response"), EditorStyles.toolbarButton);
                                useString.boolValue = GUILayout.Toggle(useString.boolValue, new GUIContent("String", "String Response"), EditorStyles.toolbarButton);
                                useGo.boolValue = GUILayout.Toggle(useGo.boolValue, new GUIContent("GO", "Game Object Response"), EditorStyles.toolbarButton);
                                useTransform.boolValue = GUILayout.Toggle(useTransform.boolValue, new GUIContent("T", "Transform Response"), EditorStyles.toolbarButton);
                                useVector.boolValue = GUILayout.Toggle(useVector.boolValue, new GUIContent("V3", "Vector3 Response"), EditorStyles.toolbarButton);
                                EditorGUILayout.EndHorizontal();

                                if (useVoid.boolValue) EditorGUILayout.PropertyField(Element.FindPropertyRelative("Response"));

                                if (useBool.boolValue) EditorGUILayout.PropertyField(Element.FindPropertyRelative("ResponseBool"), new GUIContent("Response"));

                                if (useFloat.boolValue) EditorGUILayout.PropertyField(Element.FindPropertyRelative("ResponseFloat"), new GUIContent("Response"));

                                if (useInt.boolValue) EditorGUILayout.PropertyField(Element.FindPropertyRelative("ResponseInt"), new GUIContent("Response"));

                                if (useString.boolValue) EditorGUILayout.PropertyField(Element.FindPropertyRelative("ResponseString"), new GUIContent("Response"));

                                if (useGo.boolValue) EditorGUILayout.PropertyField(Element.FindPropertyRelative("ResponseGO"),new GUIContent("Response"));

                                if (useTransform.boolValue) EditorGUILayout.PropertyField(Element.FindPropertyRelative("ResponseTransform"), new GUIContent("Response"));

                                if (useVector.boolValue) EditorGUILayout.PropertyField(Element.FindPropertyRelative("ResponseVector"), new GUIContent("Response"));
                            }
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}