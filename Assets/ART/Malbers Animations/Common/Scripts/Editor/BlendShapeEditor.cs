using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

namespace MalbersAnimations.Utilities
{
    [CustomEditor(typeof(BlendShape))]
    public class BlendShapeEditor : Editor
    {
        BlendShape M;
        private MonoScript script;
        protected int index = 0;

        private void OnEnable()
        {
            M = (BlendShape)target;
            script = MonoScript.FromMonoBehaviour(M);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Adjust the Blend Shapes on the Mesh", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {

                EditorGUI.BeginDisabledGroup(true);
                script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mesh"));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    M.SetShapesCount();
                    EditorUtility.SetDirty(target);
                }

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("LODs"),new GUIContent("LODs","Other meshes with Blend Shapes to change"),true);
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();

                var Length = M.mesh.sharedMesh.blendShapeCount;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                if (Length > 0)
                {
                    int pin = serializedObject.FindProperty("PinnedShape").intValue;
                    EditorGUILayout.LabelField(new GUIContent("Pin Shape:              (" + pin + ") |" + M.mesh.sharedMesh.GetBlendShapeName(pin) + "|", "Current Shape Store to modigy When accesing public methods from other scripts"));
                }
                EditorGUILayout.EndVertical();

                if (Length > 0)
                {
                    if (M.blendShapes == null)
                    {
                        M.blendShapes = M.GetBlendShapeValues();
                        serializedObject.ApplyModifiedProperties();
                    }

                    SerializedProperty blendShapes = serializedObject.FindProperty("blendShapes");


                    for (int i = 0; i < Length; i++)
                    {
                        if (blendShapes.arraySize == Length)
                        {
                            blendShapes.GetArrayElementAtIndex(i).floatValue = EditorGUILayout.Slider("(" + i.ToString("D2") + ") " + M.mesh.sharedMesh.GetBlendShapeName(i), blendShapes.GetArrayElementAtIndex(i).floatValue, 0, 100);
                            //M.blendShapes[i] = EditorGUILayout.Slider(M.mesh.sharedMesh.GetBlendShapeName(i), M.blendShapes[i], 0, 100);
                        }
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("Reset"))
                    {
                        M.blendShapes = new float[M.mesh.sharedMesh.blendShapeCount];
                        Undo.RecordObject(M, "Reset Blend Shape");
                    }
                    if (GUILayout.Button("Randomize"))
                    {
                        float[] BlendRandom = new float[M.mesh.sharedMesh.blendShapeCount];
                        for (int i = 0; i < Length; i++)
                        {
                            BlendRandom[i] = Random.Range(0, 100);
                        }
                        M.blendShapes = BlendRandom;
                        Undo.RecordObject(M, "Randomize Blend Shape");
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No BlendShapes found", MessageType.Info);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();


            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(M, "Blend Shapes Changed");
                EditorUtility.SetDirty(target);
                M.UpdateBlendShapes();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
