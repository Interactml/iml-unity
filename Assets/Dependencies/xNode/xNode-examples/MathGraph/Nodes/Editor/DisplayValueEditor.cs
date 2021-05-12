using UnityEditor;
using UnityEngine;
using XNode.Examples.MathNodes;

namespace XNodeEditor.Examples {

    /// <summary> 
    /// NodeEditor functions similarly to the Editor class, only it is xNode specific.
    /// Custom node editors should have the CustomNodeEditor attribute that defines which node type it is an editor for.
    /// </summary>
    [CustomNodeEditor(typeof(DisplayValue))]
    public class DisplayValueEditor : NodeEditor {

        private Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        private bool drawCustomTexture = false;

        /// <summary> Called whenever the xNode editor window is updated </summary>
        public override void OnBodyGUI() {

            // Attempt to draw a texture as the base color
            if (tex == null)
                tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, GetTint());
            tex.Apply();

            if (drawCustomTexture)
            {
                GUI.DrawTexture(new Rect(0, 0, GetWidth(), GetWidth() * 0.5f), tex, ScaleMode.ScaleAndCrop);

            }

            // Draw the default GUI first, so we don't have to do all of that manually.
            base.OnBodyGUI();            

            // `target` points to the node, but it is of type `Node`, so cast it.
            DisplayValue displayValueNode = target as DisplayValue;

            // Get the value from the node, and display it
            object obj = displayValueNode.GetValue();
            if (obj != null) EditorGUILayout.LabelField(obj.ToString());

            //// CODE TO CHANGE NODE COLOR
            //customNodeColor = EditorGUILayout.ColorField(customNodeColor);

            // Checkbox to allow the drawing of custom texture
            drawCustomTexture = EditorGUILayout.Toggle("Custom node texture", drawCustomTexture);

        }

    }
}