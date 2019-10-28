using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

namespace MalbersAnimations.Utilities
{
    [CustomEditor(typeof(ActiveMeshes))]
    public class ActiveMeshesEditor : Editor
    {
        private ReorderableList list;
        SerializedProperty ToggleMeshes;
        private ActiveMeshes MyMeshes;
        private MonoScript script;

        private void OnEnable()
        {
            MyMeshes = (ActiveMeshes)target;
            script = MonoScript.FromMonoBehaviour(MyMeshes);

            ToggleMeshes = serializedObject.FindProperty("Meshes");

            list = new ReorderableList(serializedObject, ToggleMeshes, true, true, true, true);
            {
                list.drawElementCallback = DrawElementCallback;
                list.drawHeaderCallback = HeaderCallbackDelegate;
                list.onAddCallback = OnAddCallBack;
            }
        }

        /// <summary>
        /// Reordable List Header
        /// </summary>
        void HeaderCallbackDelegate(Rect rect)
        {
            Rect R_0 = new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight);
            Rect R_01 = new Rect(rect.x + 14, rect.y, 35, EditorGUIUtility.singleLineHeight);
            Rect R_1 = new Rect(rect.x + 14 + 25, rect.y, (rect.width - 10) / 2, EditorGUIUtility.singleLineHeight);
            Rect R_2 = new Rect(rect.x  + 35 + ((rect.width - 30) / 2), rect.y, rect.width - ((rect.width) / 2) - 25, EditorGUIUtility.singleLineHeight);

            MyMeshes.showMeshesList  = EditorGUI.ToggleLeft(R_0,"", MyMeshes.showMeshesList);
            EditorGUI.LabelField(R_01, new GUIContent(" #", "Index"), EditorStyles.miniLabel);
            EditorGUI.LabelField(R_1, "Active Meshes", EditorStyles.miniLabel);
            EditorGUI.LabelField(R_2, "CURRENT", EditorStyles.centeredGreyMiniLabel);

            Rect R_3 = new Rect(rect.width + 5, rect.y + 1, 20, EditorGUIUtility.singleLineHeight - 2);
            MyMeshes.random = GUI.Toggle(R_3, MyMeshes.random, new GUIContent("R", "On Start Assigns a Random Mesh"), EditorStyles.miniButton);
        }

        void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = MyMeshes.Meshes[index];
            rect.y += 2;

            Rect R_0 = new Rect(rect.x, rect.y, (rect.width - 65) / 2, EditorGUIUtility.singleLineHeight);
            Rect R_1 = new Rect(rect.x + 25, rect.y, (rect.width - 65) / 2, EditorGUIUtility.singleLineHeight);
            Rect R_2 = new Rect(rect.x + 25 + ((rect.width - 30) / 2), rect.y, rect.width - ((rect.width) / 2) - 8, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(R_0, "(" + index.ToString() + ")", EditorStyles.label);

            element.Name = EditorGUI.TextField(R_1, element.Name,EditorStyles.label);

            string ButtonName = "Empty";

            if (element.meshes != null  && element.meshes.Length > element.Current)
            {
                ButtonName = (element.meshes[element.Current] == null ? "Empty" : element.meshes[element.Current].name ) + " (" + element.Current + ")";
            } 
            

            if (GUI.Button(R_2, ButtonName,EditorStyles.miniButton))
            {
                ToggleButton(index);
            }
        }

        void ToggleButton(int index)
        {
            if (MyMeshes.Meshes[index].meshes != null)
            {
                MyMeshes.Meshes[index].ChangeMesh();
            }
        }

        void OnAddCallBack(ReorderableList list)
        {
            if (MyMeshes.Meshes == null)
            {
                MyMeshes.Meshes = new System.Collections.Generic.List<ActiveSMesh>();
            }

            MyMeshes.Meshes.Add(new ActiveSMesh()); 
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Toggle || Swap Meshes", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            EditorGUI.BeginDisabledGroup(true);
            script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
            {
                list.DoLayoutList();
                EditorGUI.indentLevel++;
                if (MyMeshes.showMeshesList)
                {
                    if (list.index != -1)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        SerializedProperty Element = ToggleMeshes.GetArrayElementAtIndex(list.index);

                        EditorGUILayout.PropertyField(Element, true);


                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUI.indentLevel--;
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
