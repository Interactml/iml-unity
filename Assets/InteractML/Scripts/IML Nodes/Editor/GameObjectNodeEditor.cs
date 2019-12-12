using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeEditor(typeof(GameObjectNode))]
    public class GameObjectNodeEditor : NodeEditor
    {
        GameObjectNode gObjNode;
        Editor gameObjectEditor;
        GUIStyle stylePreview;

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            gObjNode = (target as GameObjectNode);

            // Show label of which object is being fed to the node
            GameObject gObj = gObjNode.GameObjectFromScene;

            // Only draw the label if the object is not null
            if (gObj != null)
            {
                EditorGUILayout.LabelField("GameObject: " + gObj.name);

                // Code to create a preview of the gObj in the node
                if (gameObjectEditor == null)
                {
                    gameObjectEditor = Editor.CreateEditor(gObj);
                }
                
                // Defines the style for the gameObject preview
                if (stylePreview == null)
                {
                    // Base our style from the helpBox one (other styles might throw null reference in texture field)
                    stylePreview = new GUIStyle(EditorStyles.textArea);
                    // Change color of background texture (NOT WORKING CURRENTLY) (Even when creating a totally new style, it throws null error)
                    //if (stylePreview.normal.background.isReadable)
                    //{
                    //    Texture2D textureStyle = new Texture2D(32, 32);
                    //    Color[] styleBgTextureColors = textureStyle.GetPixels();
                    //    // Define the color of the preview background texture 
                    //    for (int i = 0; i < styleBgTextureColors.Length; i++)
                    //    {
                    //        // We want a black color texture
                    //        styleBgTextureColors[i] = Color.black;
                    //    }
                    //    textureStyle.SetPixels(styleBgTextureColors);
                    //    textureStyle.Apply();
                    //    // Apply changes to style
                    //    // Normal Style
                    //    stylePreview.normal.background = textureStyle;
                    //    stylePreview.onNormal.background = textureStyle;
                    //    // Hover
                    //    stylePreview.hover.background = textureStyle;
                    //    stylePreview.onHover.background = textureStyle;
                    //    // Active
                    //    stylePreview.active.background = textureStyle;
                    //    stylePreview.onActive.background = textureStyle;
                    //    // Focused
                    //    stylePreview.focused.background = textureStyle;
                    //    stylePreview.onFocused.background = textureStyle;
                    //}
                    //else
                    //{
                    //    //Debug.Log("Style texture is not readable!");
                    //}

                }

                // Draw preview of Game Object
                if (stylePreview != null)
                    gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(100, 100), stylePreview);
                else
                    Debug.LogError("Null reference in the preview style of the GameObjectNode");
            }
            // If it is null, we warn it
            else
            {
                EditorGUILayout.LabelField("GameObject: " + "NULL");
                EditorGUILayout.HelpBox("No GameObject being fed to this node from an IML Component in the Scene", MessageType.Error);
            }
        }
    }

}

