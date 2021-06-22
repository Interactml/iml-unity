using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


namespace MalbersAnimations.Utilities
{
    [CustomEditor(typeof(EffectManager))]
    public class EffectManagerEditor : Editor
    {

        private ReorderableList list;
        private SerializedProperty EffectList;
        private EffectManager _M;
        bool eventss = true, offsets = true , parent = true , general = true;

        private void OnEnable()
        {
            _M = ((EffectManager)target);

            EffectList = serializedObject.FindProperty("Effects");

            list = new ReorderableList(serializedObject, EffectList, true, true, true, true)
            {
                drawElementCallback = DrawElementCallback,
                drawHeaderCallback = HeaderCallbackDelegate,
                onAddCallback = OnAddCallBack
            };
        }


        void HeaderCallbackDelegate(Rect rect)
        {
            Rect R_1 = new Rect(rect.x + 14, rect.y, (rect.width - 10) / 2, EditorGUIUtility.singleLineHeight);
            Rect R_2 = new Rect(rect.x + 14 + ((rect.width - 30) / 2), rect.y, rect.width - ((rect.width) / 2), EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(R_1, "Effect List", EditorStyles.miniLabel);
            EditorGUI.LabelField(R_2, "ID", EditorStyles.centeredGreyMiniLabel);
        }

        void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _M.Effects[index];
            rect.y += 2;

            Rect R_0 = new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight);
            Rect R_1 = new Rect(rect.x + 16, rect.y, (rect.width - 10) / 2, EditorGUIUtility.singleLineHeight);
            Rect R_2 = new Rect(rect.x + 16 + ((rect.width - 30) / 2), rect.y, rect.width - ((rect.width) / 2), EditorGUIUtility.singleLineHeight);

            element.active = EditorGUI.Toggle(R_0, element.active);
            element.Name = EditorGUI.TextField(R_1, element.Name, EditorStyles.label);
            element.ID = EditorGUI.IntField(R_2, element.ID);

        }

        void OnAddCallBack(ReorderableList list)
        {
            if (_M.Effects == null)
            {
                _M.Effects = new System.Collections.Generic.List<Effect>();
            }
            _M.Effects.Add(new Effect());
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Manage all the Effects using the function (PlayEffect(int ID))", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                list.DoLayoutList();

                if (list.index != -1)
                {
                    Effect effect = _M.Effects[list.index];

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    SerializedProperty Element = EffectList.GetArrayElementAtIndex(list.index);
                    EditorGUILayout.LabelField("* " + effect.Name + " *", EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.indentLevel++;
                    general = EditorGUILayout.Foldout(general, "General");

                    if (general)
                    {
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("effect"), new GUIContent("Effect", "The Prefab or gameobject which holds the Effect(Particles, transforms)"));

                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("instantiate"), new GUIContent("Instantiate","True you want to make a copy of the effect, or if the effect is a Prefab!"));

                        if (!Element.FindPropertyRelative("instantiate").boolValue)

                        {
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("toggleable"), new GUIContent("Toggleable", "Everytime this effect is called it will turn on and off"));

                            if (Element.FindPropertyRelative("toggleable").boolValue)
                            {
                                EditorGUILayout.PropertyField(Element.FindPropertyRelative("On"), new GUIContent(Element.FindPropertyRelative("On").boolValue ? "On" : "Off", "if Toggleable is active this will set the first state"));
                            }
                        }

                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("life"), new GUIContent("Life","Duration of the Effect. The Effect will be destroyed if 'instantiate' is set to true"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("delay"), new GUIContent("Delay", "Time before playing the Effect"));

                        if (Element.FindPropertyRelative("life").floatValue <= 0)
                        {
                            EditorGUILayout.HelpBox("Life = 0  the effect will not be destroyed by this Script", MessageType.Info);
                        }
                    }
                    EditorGUI.indentLevel--;


                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.indentLevel++;
                    parent = EditorGUILayout.Foldout(parent, "Parent");

                    if (parent)
                    {
                        SerializedProperty root = Element.FindPropertyRelative("root");
                        EditorGUILayout.PropertyField(root, new GUIContent("Root","Uses the Root transform to position the Effect"));

                        if (root.objectReferenceValue != null)
                        {
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("isChild"), new GUIContent("is Child", "Set the Effect as a child of the Root transform"));
                            EditorGUILayout.PropertyField(Element.FindPropertyRelative("useRootRotation"), new GUIContent("Use Root Rotation", "Orient the Effect using the root rotation."));
                        }
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();



                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.indentLevel++;
                    offsets = EditorGUILayout.Foldout(offsets, "Offsets");
                    if (offsets)
                    {
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("RotationOffset"), new GUIContent("Rotation", "Add additional offset to the Effect position"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("PositionOffset"), new GUIContent("Position", "Add additional offset to the Effect rotation"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("ScaleMultiplier"), new GUIContent("Scale", "Add additional offset to the Effect Scale"));
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.PropertyField(Element.FindPropertyRelative("Modifier"), new GUIContent("Modifier",""));

                    if (effect.Modifier != null && effect.Modifier.Description != string.Empty)
                    {
                        SerializedObject modifier = new SerializedObject(effect.Modifier);
                        var property = modifier.GetIterator();

                        property.NextVisible(true);                 //Don't Paint the first "Base thing"
                        property.NextVisible(true);                 //Don't Paint the script
                        property.NextVisible(true);                 //Don't Paint the Description I already painted

                        EditorGUILayout.HelpBox(effect.Modifier.Description, MessageType.None);

                        EditorGUI.BeginChangeCheck();

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        do
                        {
                            EditorGUILayout.PropertyField(property, true);
                        } while (property.NextVisible(false));
                        EditorGUILayout.EndVertical();


                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(effect.Modifier, "Ability Changed");
                            modifier.ApplyModifiedProperties();
                            if (modifier != null)
                            {
                                MalbersEditor.SetObjectDirty(effect.Modifier);
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();


                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.indentLevel++;
                    eventss = EditorGUILayout.Foldout(eventss, "Events");
                    EditorGUI.indentLevel--;
                    if (eventss)
                    {
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnPlay"), new GUIContent("On Play"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnStop"), new GUIContent("On Stop"));
                    }
                    EditorGUILayout.EndVertical();


                    //EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    //SerializedProperty hasLODS = Element.FindPropertyRelative("HasLODs");
                    //EditorGUILayout.PropertyField(hasLODS, new GUIContent("LODs", "Has Level of Detail Meshes"));
                    //if (hasLODS.boolValue)
                    //{
                    //    EditorGUILayout.PropertyField(Element.FindPropertyRelative("LODs"), new GUIContent("Meshes", "Has Level of Detail Meshes"), true);
                    //}
                    //EditorGUILayout.EndVertical();

                }


                //  EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_M, "Effect Manager");
                EditorUtility.SetDirty(target);
            }
            serializedObject.ApplyModifiedProperties();
        }



        bool IsPrefab(GameObject gameObject)
        {
            bool isPrefabInstance = PrefabUtility.GetPrefabParent(gameObject) != null && PrefabUtility.GetPrefabObject(gameObject.transform) != null;
            bool isPrefabOriginal = PrefabUtility.GetPrefabParent(gameObject) == null && PrefabUtility.GetPrefabObject(gameObject.transform) != null;
            bool isDisconnectedPrefabInstance = PrefabUtility.GetPrefabParent(gameObject) != null && PrefabUtility.GetPrefabObject(gameObject.transform) == null;

            return isPrefabOriginal || isPrefabInstance || isDisconnectedPrefabInstance;
        }
    }
}