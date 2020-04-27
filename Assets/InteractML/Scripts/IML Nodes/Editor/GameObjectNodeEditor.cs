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
    public class GameObjectNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private GameObjectNode m_GameObjectNode;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_BodyRect;
        private Rect m_PortRect;

        Editor gameObjectEditor;
        GUIStyle stylePreview;

        bool state;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_GameObjectNode = (target as GameObjectNode);

            // Initialise header background Rects
            InitHeaderRects();

            NodeColor = GetColorTextureFromHexString("#3A3B5B");

            // Draw header background Rect
            GUI.DrawTexture(HeaderRect, NodeColor);

            // Draw line below header
            GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#E24680"));

            //Display Node name
            GUILayout.Label("GAME OBJECT INPUT", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 20), GUILayout.MinHeight(60));

        }

        public override void OnBodyGUI()
        {
            // Draw Port Section
            DrawPortLayout();
            ShowGameObjectNodePorts();

            DrawBodyLayout();
            EditorGUI.indentLevel++;
            ShowGameObjectPreview();
            EditorGUI.indentLevel--;

        }

        /// <summary>
        /// Define rect values for port section and paint textures based on rects 
        /// </summary>
        private void DrawPortLayout()
        {
            // Draw body background purple rect below header
            m_PortRect.x = 5;
            m_PortRect.y = HeaderRect.height;
            m_PortRect.width = NodeWidth - 10;
            m_PortRect.height = 60;
            GUI.DrawTexture(m_PortRect, NodeColor);

            // Draw line below ports
            GUI.DrawTexture(new Rect(m_PortRect.x, HeaderRect.height + m_PortRect.height - WeightOfSectionLine, m_PortRect.width, WeightOfSectionLine), GetColorTextureFromHexString("#E24680"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayout()
        {
            m_BodyRect.x = 5;
            m_BodyRect.y = HeaderRect.height + m_PortRect.height;
            m_BodyRect.width = NodeWidth - 10;
            m_BodyRect.height = 160;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_BodyRect, NodeColor);
        }


        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowGameObjectNodePorts()
        {
            EditorGUILayout.Space();
            GUIContent outputPortLabel = new GUIContent("GameObject\n Data Out");
            PortField(outputPortLabel, m_GameObjectNode.GetOutputPort("GameObjectDataOut"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));
            

        }

        /// <summary>
        /// Show the a preview image of the connected gameObject 
        /// </summary>
        private void ShowGameObjectPreview()
        {
            Rect previewBox = new Rect(m_BodyRect.x + 15, m_BodyRect.y, m_BodyRect.width - 30, m_BodyRect.height);
            GUILayout.BeginArea(previewBox);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Show label of which object is being fed to the node
            GameObject gObj = m_GameObjectNode.GameObjectDataOut;

            // Only draw the label if the object is not null
            if (gObj != null)
            {
                EditorGUILayout.LabelField(gObj.name, new GUIStyle(Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label")) { alignment = TextAnchor.MiddleLeft });
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                // Code to create a preview of the gObj in the node
                if (gameObjectEditor == null || m_GameObjectNode.state)
                {
                    gameObjectEditor = Editor.CreateEditor(gObj);
                    m_GameObjectNode.state = false;
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
                EditorGUILayout.HelpBox("Connect a GameObject", MessageType.Error);
            }

            GUILayout.EndArea();

        }
    }

}

