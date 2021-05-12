using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


namespace MalbersAnimations
{
    [CustomEditor(typeof(LayersBehavior))]
    public class LayersBehaviorEd : Editor
    {
        private ReorderableList list;
        private LayersBehavior MlayerB;

        private void OnEnable()
        {
            MlayerB = ((LayersBehavior)target);

            list = new ReorderableList(serializedObject, serializedObject.FindProperty("layers"), false, true, true, true);
            list.drawElementCallback = drawElementCallback;
            list.drawHeaderCallback = HeaderCallbackDelegate;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("This Script Enable/Disable Layers in the Transition times", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();
            list.DoLayoutList();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
            serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// Reordable List Header
        /// </summary>
        void HeaderCallbackDelegate(Rect rect)
        {
            Rect R_1 = new Rect(rect.x, rect.y, (rect.width / 3), EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_1, "Layer");

            Rect R_2 = new Rect(rect.x + (((rect.width) / 3) + 2), rect.y, 25, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_2, "On");

            Rect R_3 = new Rect(rect.x + ((rect.width) / 3) + 25, rect.y, ((rect.width) / 3) - 25, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_3, "Transition");

            Rect R_4 = new Rect(rect.x + ((rect.width) / 3) * 2 + 2, rect.y, 25, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_4, "Off");

            Rect R_5 = new Rect(rect.x + ((rect.width) / 3) * 2 + 25, rect.y, ((rect.width) / 3) - 25, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_5, "Transition");
        }

        void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = MlayerB.layers[index];
            rect.y += 2;
            // element.active = EditorGUI.Toggle(new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight), element.active);

            Rect R_1 = new Rect(rect.x, rect.y, (rect.width / 3), EditorGUIUtility.singleLineHeight);
            element.layer = EditorGUI.TextField(R_1, element.layer);

            Rect R_2 = new Rect(rect.x + (((rect.width) / 3) + 5), rect.y, 15, EditorGUIUtility.singleLineHeight);
            element.activate = EditorGUI.Toggle(R_2, element.activate);

            if (element.activate)
            {
                Rect R_3 = new Rect(rect.x + ((rect.width) / 3) + 25, rect.y, ((rect.width) / 3) - 25, EditorGUIUtility.singleLineHeight);
                element.transA = (StateTransition)EditorGUI.EnumPopup(R_3, element.transA);
            }
            Rect R_4 = new Rect(rect.x + ((rect.width) / 3) * 2 + 5, rect.y, 15, EditorGUIUtility.singleLineHeight);
            element.deactivate = EditorGUI.Toggle(R_4, element.deactivate);

            if (element.deactivate)
            {
                Rect R_5 = new Rect(rect.x + ((rect.width) / 3) * 2 + 25, rect.y, ((rect.width) / 3) - 25, EditorGUIUtility.singleLineHeight);
                element.transD = (StateTransition)EditorGUI.EnumPopup(R_5, element.transD);
            }
        }

    }
}