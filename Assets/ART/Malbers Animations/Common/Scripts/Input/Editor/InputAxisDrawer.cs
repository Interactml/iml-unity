using UnityEngine;
using UnityEditor;

namespace MalbersAnimations
{
    [CustomPropertyDrawer(typeof(InputAxis))]
    public class InputAxisDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            property.FindPropertyRelative("name").stringValue = label.text;


            // Calculate rects
            var activeRect = new Rect(position.x, position.y, 15, position.height);
            var LabelRect = new Rect(position.x + 15, position.y, EditorGUIUtility.labelWidth - 15, position.height);
            var valueRect = new Rect(EditorGUIUtility.labelWidth + 15, position.y, position.width - EditorGUIUtility.labelWidth - 20, position.height);
            var RawRect = new Rect(position.width - 3, position.y, 17, position.height);


            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(activeRect, property.FindPropertyRelative("active"), GUIContent.none);
            EditorGUI.LabelField(LabelRect, label);

            EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("input"), GUIContent.none);

            var RawProp = property.FindPropertyRelative("raw");

            RawProp.boolValue = GUI.Toggle(RawRect, RawProp.boolValue , new GUIContent("R","Use Raw Values for the Axis"),EditorStyles.miniButton);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}