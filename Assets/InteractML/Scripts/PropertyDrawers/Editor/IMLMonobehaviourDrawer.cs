using UnityEditor;
using UnityEngine;
using InteractML;

[CustomPropertyDrawer(typeof(IMLMonobehaviourAttribute))]
public class IMLMonobehaviourDrawer : PropertyDrawer
{
    Texture2D m_ScriptTexture;
    Texture2D m_GOTexture;
    bool m_TextureLoaded = false;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Cast reference to the monobehaviour attribute
        IMLMonobehaviourAttribute IMLDrawer = attribute as IMLMonobehaviourAttribute;
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);
        
        // Load texture once
        if (!m_TextureLoaded)
        {
            // Load texture
            m_ScriptTexture = EditorGUIUtility.FindTexture("cs Script Icon");
            m_GOTexture = EditorGUIUtility.FindTexture("Prefab Icon");
            m_TextureLoaded = true;
        }

        // Draw label
        // Attempt to get name of script class
        Object serializedScript = property.objectReferenceValue;
        string scriptClassName = null;
        string goClassName = null;
        if (serializedScript != null)
        {
            scriptClassName = serializedScript.GetType().ToString();
            goClassName = serializedScript.name;
        }
        else
        {
            scriptClassName = "Null";
            goClassName = "Null";
        }

        // Assign extra text to label
        label.text = string.Concat("Script: ", scriptClassName);
        // Assign icon to label
        label.image = m_ScriptTexture;
        // Increase original position height to account for extra drawing
        position.y = position.y + 20;
        position.height = position.height + 10;
        // Draw label for script
        EditorGUI.LabelField(position, label);

        // Change text and icon for gameobject
        label.text = string.Concat("GameObject : ", goClassName);
        label.image = m_GOTexture;
        // Decrease original position height to account for extra drawing
        position.y = position.y - 20;
        // Draw label for gameObject
        EditorGUI.LabelField(position, label);

        EditorGUI.EndProperty();


    }
}
