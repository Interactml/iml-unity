using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RenameAttribute))]
public class RenameEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        EditorGUI.PropertyField(position, property, new GUIContent((attribute as RenameAttribute).NewName));
        
    }
}