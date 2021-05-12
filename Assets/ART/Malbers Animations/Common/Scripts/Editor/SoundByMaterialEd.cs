using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace MalbersAnimations.Utilities
{
    [CustomEditor(typeof(SoundByMaterial))]
    public class SoundByMaterialEd : Editor
    {
        private ReorderableList list;
        SerializedProperty soundbymaterial;
        private SoundByMaterial mySBM;

        private void OnEnable()
        {
            mySBM = (SoundByMaterial)target;

            soundbymaterial = serializedObject.FindProperty("materialSounds");

            list = new ReorderableList(serializedObject, soundbymaterial, true, true, true, true);

            list.drawElementCallback = DrawElementCallback;
            list.drawHeaderCallback = HeaderCallbackDelegate;
        }

        /// <summary>
        /// Reordable List Header
        /// </summary>
        void HeaderCallbackDelegate(Rect rect)
        {
            //Rect R_0 = new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight);
            Rect R_1 = new Rect(rect.x + 14, rect.y, (rect.width - 10) / 2, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_1, "Sound by Material List ", EditorStyles.miniLabel);
        }

        void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = mySBM.materialSounds[index];
            rect.y += 2;

            Rect R_1 = new Rect(rect.x, rect.y, (rect.width ) , EditorGUIUtility.singleLineHeight);
            //Rect R_2 = new Rect(rect.x + 25 + ((rect.width - 30) / 2), rect.y, rect.width - ((rect.width) / 2) - 14, EditorGUIUtility.singleLineHeight);

            element.material = (PhysicMaterial)EditorGUI.ObjectField(R_1, element.material, typeof(PhysicMaterial), false);

        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Plays the sound matching the physic material on the hit object\nInvoke the method 'PlayMaterialSound'", MessageType.None);
            EditorGUILayout.EndVertical();


            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                mySBM.DefaultSound = (AudioClip)EditorGUILayout.ObjectField("Default Sound", mySBM.DefaultSound, typeof(AudioClip), false);
                EditorGUILayout.EndVertical();

                list.DoLayoutList();

                if (list.index != -1)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.indentLevel++;
                    SerializedProperty Element = soundbymaterial.GetArrayElementAtIndex(list.index);

                    SerializedProperty SoundElement = Element.FindPropertyRelative("Sounds");

                    EditorGUILayout.PropertyField(SoundElement, true);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }

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
