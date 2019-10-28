using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace MalbersAnimations
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MalbersInput))]
    public class MalbersInputEditor : Editor
    {
        private ReorderableList list;
        private SerializedProperty inputs;
        private MalbersInput M;
        MonoScript script;


        private void OnEnable()
        {
            M = ((MalbersInput)target);
            script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);

            inputs = serializedObject.FindProperty("inputs");

            list = new ReorderableList(serializedObject, inputs, true, true, true, true);
            list.drawElementCallback = drawElementCallback;
            list.drawHeaderCallback = HeaderCallbackDelegate;
            list.onAddCallback = OnAddCallBack;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleBlue);
            EditorGUILayout.HelpBox("Connects the INPUTS to the Locomotion System. The 'Name' is actually the Properties to access", MessageType.None);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(MalbersEditor.StyleGray);
            {
                EditorGUI.BeginDisabledGroup(true);
                script = (MonoScript)EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginChangeCheck();
#if REWIRED
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("PlayerID"), new GUIContent("Player ID", "Rewired Player ID"));
                EditorGUILayout.EndVertical();
#endif

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Horizontal"), new GUIContent("Horizontal", "Axis for the Horizontal Movement"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Vertical"), new GUIContent("Vertical", "Axis for the Forward/Backward Movement"));
                    //EditorGUILayout.PropertyField(serializedObject.FindProperty("UpDown"), new GUIContent("UpDown", "Axis for the Up and Down Movement"));
                }
                EditorGUILayout.EndVertical();

                list.DoLayoutList();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cameraBaseInput"), new GUIContent("Camera Input", "The Character will follow the camera forward axis"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("alwaysForward"), new GUIContent("Always Forward", "The Character will move forward forever"));
                EditorGUILayout.EndVertical();

                if (list.index != -1)
                {
                    if (list.index < list.count)
                    {
                        SerializedProperty Element = inputs.GetArrayElementAtIndex(list.index);
                        DrawInputEvents(Element, true);
                    }
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUI.indentLevel++;
                M.showInputEvents = EditorGUILayout.Foldout(M.showInputEvents, "Events (Enable/Disable Malbers Input)");
                EditorGUI.indentLevel--;

                if (M.showInputEvents)
                {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnInputEnabled"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnInputDisabled"));
 }
                EditorGUILayout.EndVertical();


                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(target);
                }
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndVertical();
        }


        public static void DrawInputEvents(SerializedProperty Element, bool showEvents)
        {
            if (Element.FindPropertyRelative("active").boolValue && showEvents)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                InputButton GetPressed = (InputButton)Element.FindPropertyRelative("GetPressed").enumValueIndex;


                switch (GetPressed)
                {
                    case InputButton.Press:
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputPressed"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputChanged"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputDown"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputUp"));
                        break;
                    case InputButton.Down:
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputDown"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputChanged"));
                        break;
                    case InputButton.Up:
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputUp"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputChanged"));
                        break;
                    case InputButton.LongPress:
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("LongPressTime"), new GUIContent("On Long Press", "Time the Input Should be Pressed"));
                        EditorGUILayout.Space();
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnLongPress"), new GUIContent("On Long Press"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnPressedNormalized"), new GUIContent("On Pressed Time Normalized"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputDown"), new GUIContent("On Pressed Down"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputUp"), new GUIContent("On Pressed Interrupted"));
                        break;
                    case InputButton.DoubleTap:
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("DoubleTapTime"));
                        EditorGUILayout.Space();
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnInputDown"), new GUIContent("On First Tap"));
                        EditorGUILayout.PropertyField(Element.FindPropertyRelative("OnDoubleTap"));
                        break;
                    default:
                        break;
                }
                EditorGUILayout.EndVertical();
            }
        }

/// <summary>
/// Reordable List Header
/// </summary>
        void HeaderCallbackDelegate(Rect rect)
        {
            Rect R_1 = new Rect(rect.x + 20, rect.y, (rect.width - 20) / 4 - 23, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_1, "Name");

            Rect R_2 = new Rect(rect.x + (rect.width - 20) / 4 + 15, rect.y, (rect.width - 20) / 4, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_2, "Type");

            Rect R_3 = new Rect(rect.x + ((rect.width - 20) / 4) * 2 + 18, rect.y, ((rect.width - 30) / 4) + 11, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_3, "Value");

            Rect R_4 = new Rect(rect.x + ((rect.width) / 4) * 3 + 15, rect.y, ((rect.width) / 4) - 15, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(R_4, "Button");
        }

        void drawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = M.inputs[index];
            rect.y += 2;
            element.active = EditorGUI.Toggle(new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight), element.active);

            Rect R_1 = new Rect(rect.x + 20, rect.y, (rect.width - 20) / 4 - 23, EditorGUIUtility.singleLineHeight);
            GUIStyle a = new GUIStyle();

            //This make the name a editable label
            a.fontStyle = FontStyle.Normal;
            element.name = EditorGUI.TextField(R_1, element.name, a);

            Rect R_2 = new Rect(rect.x + (rect.width - 20) / 4+15, rect.y, (rect.width - 20) / 4, EditorGUIUtility.singleLineHeight);
            element.type = (InputType)EditorGUI.EnumPopup(R_2, element.type);

            Rect R_3 = new Rect(rect.x + ((rect.width - 20) / 4) * 2 + 18, rect.y, ((rect.width - 30) / 4)+11 , EditorGUIUtility.singleLineHeight);
            if (element.type != InputType.Key)
                element.input = EditorGUI.TextField(R_3, element.input);
            else
                element.key = (KeyCode)EditorGUI.EnumPopup(R_3, element.key);

            Rect R_4 = new Rect(rect.x + ((rect.width) / 4) * 3 +15, rect.y, ((rect.width) / 4)-15 , EditorGUIUtility.singleLineHeight);
            element.GetPressed = (InputButton)EditorGUI.EnumPopup(R_4, element.GetPressed);
        }

        void OnAddCallBack(ReorderableList list)
        {
            if (M.inputs == null)
            {
                M.inputs = new System.Collections.Generic.List<InputRow>();
            }
            M.inputs.Add(new  InputRow("New","InputValue", KeyCode.Alpha0, InputButton.Press, InputType.Input));
        }
    }
}