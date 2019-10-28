using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

namespace MalbersAnimations.Utilities
{
    [CustomEditor(typeof(BlendShapes))]
    public class BlendShapesEditor : Editor
    {
        BlendShapes MyBlendShapes;
        private ReorderableList list;
        protected int index = 0;

        private void OnEnable()
        {
            MyBlendShapes = (BlendShapes)target;
            list = new ReorderableList(serializedObject, serializedObject.FindProperty("Shapes"), true, true, true, true)
            {
                drawElementCallback = DrawElementCallback,
                drawHeaderCallback = HeaderCallbackDelegate,
                onAddCallback = OnAddCallBack,
                onSelectCallback = OnSelectedCB
            };
        }

        void OnSelectedCB(ReorderableList list)
        {
            var item = MyBlendShapes.Shapes[list.index];
            if ((item.NameID == null || item.NameID == string.Empty) && item.mesh != null)
            {
                item.NameID = item.mesh.name;
            }
        }

        /// <summary>
        /// Reordable List Header
        /// </summary>
        void HeaderCallbackDelegate(Rect rect)
        {
            Rect R_1 = new Rect(rect.x + 14, rect.y, (rect.width - 10) / 2, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_1, "Name");

            Rect R_2 = new Rect(rect.x + 14 + ((rect.width - 30) / 2), rect.y, rect.width - ((rect.width - 10) / 2), EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_2, "Skin Mesh Renderer");

            Rect R_3 = new Rect(rect.width + 5, rect.y + 1, 20, EditorGUIUtility.singleLineHeight - 2);
            MyBlendShapes.random = GUI.Toggle(R_3, MyBlendShapes.random, new GUIContent("R", "On Start Randomize the BlendShapes"), EditorStyles.miniButton);

        }


        void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = MyBlendShapes.Shapes[index];
            rect.y += 2;

            Rect R_1 = new Rect(rect.x, rect.y, (rect.width-45) / 2, EditorGUIUtility.singleLineHeight);

            element.NameID = EditorGUI.TextField(R_1, element.NameID,EditorStyles.label);

            Rect R_2 = new Rect(rect.x + ((rect.width - 30) / 2), rect.y, rect.width - ((rect.width - 30) / 2), EditorGUIUtility.singleLineHeight);
            element.mesh = (SkinnedMeshRenderer)EditorGUI.ObjectField(R_2, element.mesh, typeof(SkinnedMeshRenderer), true);
        }


        void OnAddCallBack(ReorderableList list)
        {
            if (MyBlendShapes.Shapes == null)
            {
                MyBlendShapes.Shapes = new System.Collections.Generic.List<MeshBlendShapes>();
            }

            MyBlendShapes.Shapes.Add(new MeshBlendShapes());
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Adjust the Blend Shapes on the Meshes", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                list.DoLayoutList();
                EditorGUI.indentLevel++;

                if (list.index != -1 && MyBlendShapes.Shapes.Count > list.index)
                {
                    if (MyBlendShapes.Shapes[list.index].mesh != null)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUI.indentLevel--;
                        MeshBlendShapes item = MyBlendShapes.Shapes[list.index];

                        if (item.mesh.sharedMesh.blendShapeCount > 0)
                        {
                            if (item.blendShapes == null)
                            {
                                item.blendShapes = item.GetBlendShapeValues();
                            }

                            for (int i = 0; i < item.mesh.sharedMesh.blendShapeCount; i++)
                            {
                                item.blendShapes[i] = EditorGUILayout.Slider(item.mesh.sharedMesh.GetBlendShapeName(i), item.blendShapes[i], 0, 100);
                            }

                            EditorGUI.indentLevel++;
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("Reset"))
                            {
                                item.blendShapes = new float[item.mesh.sharedMesh.blendShapeCount];
                                Undo.RecordObject(MyBlendShapes, "Reset Blend Shape");
                            }
                            if (GUILayout.Button("Randomize"))
                            {

                               float[] BlendRandom =  new float[item.mesh.sharedMesh.blendShapeCount];
                                for (int i = 0; i < BlendRandom.Length; i++)
                                {
                                    BlendRandom[i] = Random.Range(0, 100);
                                }
                                item.blendShapes = BlendRandom;

                                Undo.RecordObject(MyBlendShapes, "Randomize Blend Shape");
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("No BlendShapes found", MessageType.Info);
                        }
                            EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndVertical();


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(MyBlendShapes, "Blend Shapes Changed");
                EditorUtility.SetDirty(target);
                MyBlendShapes.UpdateBlendShapes();
            }
               
            serializedObject.ApplyModifiedProperties();
        }
    }
}
