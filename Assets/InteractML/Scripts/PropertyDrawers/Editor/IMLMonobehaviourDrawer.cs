using UnityEditor;
using UnityEngine;
using InteractML;

[CustomPropertyDrawer(typeof(IMLMonobehaviourAttribute))]
public class IMLMonobehaviourDrawer : PropertyDrawer
{
    Texture2D m_ScriptTexture;
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
        if (!m_TextureLoaded || m_ScriptTexture == null)
        {
            // Load texture
            m_ScriptTexture = EditorGUIUtility.FindTexture("cs Script Icon");
            m_TextureLoaded = true;
        }

        // Draw label
        // Attempt to get name of script class
        Object SerializedScript = property.objectReferenceValue;
        string scriptClassName = null;
        if (SerializedScript != null)
            scriptClassName = SerializedScript.name;
        else
            scriptClassName = "Null";
        // Assign extra text to label
        label.text = string.Concat(label.text, ": ", scriptClassName);
        // Assign icon to label
        label.image = m_ScriptTexture;

        // Draw label with extra info and label
        EditorGUI.LabelField(position, label);

        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);


        // Draw icon 
        //EditorGUI.DrawTextureTransparent(position, m_ScriptTexture);
        //EditorGUI.PropertyField(position, property, GUIContent.none);

        EditorGUI.EndProperty();

    }
}
