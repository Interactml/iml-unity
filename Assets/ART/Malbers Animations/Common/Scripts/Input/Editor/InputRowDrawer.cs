using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

namespace MalbersAnimations
{
    [CustomPropertyDrawer(typeof(InputRow))]
    public class InputRowDrawer : PropertyDrawer
    {

        //static float EventHeight = 83;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // EditorGUI.HelpBox(position, "",MessageType.None);

            EditorGUI.BeginProperty(position, label, property);

            var DefaultPosition = position;

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            property.FindPropertyRelative("name").stringValue = label.text;

            var height = EditorGUIUtility.singleLineHeight;
            // Calculate rects
            var activeRect = new Rect(position.x, position.y, 15, height);
            var LabelRect = new Rect(position.x + 17, position.y, 100, height);


            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(activeRect, property.FindPropertyRelative("active"), GUIContent.none);
            EditorGUI.LabelField(LabelRect, label, EditorStyles.miniBoldLabel);

            //Set Rect to the Parameters Values
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(" "));

            // Calculate rects
            var typeRect = new Rect(position.x - 30, position.y, 44, height);
            var valueRect = new Rect(position.x - 30 + 45, position.y, position.width / 2 - 11, height);
            var ActionRect = new Rect(position.width / 2 + position.x - 30 + 40 - 5, position.y, (position.width / 2 - 7) - 16 , height);
            var ShowRect = new Rect(DefaultPosition.width+6 , position.y, 16, height-1);

            EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("type"), GUIContent.none);

            InputType current = (InputType)property.FindPropertyRelative("type").enumValueIndex;
            switch (current)
            {
                case InputType.Input:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("input"), GUIContent.none);
                    break;
                case InputType.Key:
                    EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("key"), GUIContent.none);
                    break;
                default:
                    break;
            }

              EditorGUI.PropertyField(ActionRect, property.FindPropertyRelative("GetPressed"), GUIContent.none);

            SerializedProperty show = property.FindPropertyRelative("ShowEvents");


            show.boolValue = GUI.Toggle(ShowRect, show.boolValue, new GUIContent("" /*!show.boolValue ? "▼" : "▲"*/, "Show Events for the " +property.FindPropertyRelative("name").stringValue +" Input"), EditorStyles.foldout);

            //if (show.boolValue)
            //{
            //    DefaultPosition.y += EditorGUIUtility.singleLineHeight+3;



            //    InputButton enumValueIndex = (InputButton)property.FindPropertyRelative("GetPressed").enumValueIndex;

              

            //    switch (enumValueIndex)
            //    {
            //        case InputButton.Press:
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnInputPressed"));

            //            DefaultPosition.y += EventHeight;
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnInputChanged"));

            //            DefaultPosition.y += EventHeight;
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnInputUp"));

            //            DefaultPosition.y += EventHeight;
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnInputDown"));
            //            break;
            //        case InputButton.Down:
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnInputDown"));

            //            DefaultPosition.y += EventHeight;
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnInputChanged"));
            //            break;
            //        case InputButton.Up:
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnInputUp"));

            //            DefaultPosition.y += EventHeight;
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnInputChanged"));
            //            break;
            //        case InputButton.LongPress:
            //            Rect LonRect = DefaultPosition;
            //            LonRect.height = EditorGUIUtility.singleLineHeight;
            //            EditorGUI.PropertyField(LonRect, property.FindPropertyRelative("LongPressTime"), new GUIContent("On Long Press", "Time the Input Should be Pressed"));
            //            DefaultPosition.y += EditorGUIUtility.singleLineHeight + 3;
                      
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnLongPress"), new GUIContent("On Long Press"));
            //            DefaultPosition.y += EventHeight;
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnPressedNormalized"), new GUIContent("On Pressed Time Normalized"));
            //            DefaultPosition.y += EventHeight;
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnInputDown"), new GUIContent("On Pressed Down"));
            //            DefaultPosition.y += EventHeight;
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnInputUp"), new GUIContent("On Pressed Interrupted"));
            //            DefaultPosition.y += EventHeight;
            //            break;
            //        case InputButton.DoubleTap:
            //            Rect LonRect1 = DefaultPosition;
            //            LonRect1.height = EditorGUIUtility.singleLineHeight;
            //            EditorGUI.PropertyField(LonRect1, property.FindPropertyRelative("DoubleTapTime"), new GUIContent("Double Tap Time", "Time between the double tap"));
            //            DefaultPosition.y += EditorGUIUtility.singleLineHeight + 3;

            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnInputDown"), new GUIContent("On First Tap"));
            //            DefaultPosition.y += EventHeight;
            //            EditorGUI.PropertyField(DefaultPosition, property.FindPropertyRelative("OnDoubleTap"));
            //            break;
            //        default:
            //            break;
            //    }
            //}

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }


        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        //{

        //    SerializedObject childObj = new UnityEditor.SerializedObject(property.objectReferenceValue as InputRow);
        //    SerializedProperty ite = childObj.GetIterator();

        //    float totalHeight = EditorGUI.GetPropertyHeight(property, label);

        //    while (ite.NextVisible(true))
        //    {
        //        totalHeight += EditorGUI.GetPropertyHeight(ite, label);
        //    }


        //    return totalHeight;
        //}


        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        //{
        //    bool ShowEvents = property.FindPropertyRelative("ShowEvents").boolValue;

        //    InputButton enumValueIndex = (InputButton)property.FindPropertyRelative("GetPressed").enumValueIndex;

        //    float NewHeight = base.GetPropertyHeight(property, label);

        //    if (ShowEvents)
        //    {
        //        switch (enumValueIndex)
        //        {
        //            case InputButton.Press:



        //                return NewHeight + 332;

        //            case InputButton.Down:



        //                 //float DownHeight = property.FindPropertyRelative("OnInputDown").rectValue.height +
        //                 //                   property.FindPropertyRelative("OnInputChanged").rectValue.height;

        //              //Debug.Log(property.FindPropertyRelative("OnInputDown").ex.height);

        //                return NewHeight + 166;

        //            case InputButton.Up:
        //                return NewHeight + 166;
        //            case InputButton.LongPress:
        //                return NewHeight + 353;
        //            case InputButton.DoubleTap:
        //                return NewHeight + 186;
        //            default:
        //                break;
        //        }
        //    }

        //    return base.GetPropertyHeight(property, label);
        //}
    }
   
}