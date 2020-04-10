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
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private GameObjectNode m_GameObjectNode;

        private GUISkin skin;

        private Color nodeColor;
        private Color lineColor;

        private Texture2D nodeTexture;
        private Texture2D lineTexture;

        private Rect headerSection;
        private Rect portSection;
        private Rect bodySection;

        GameObjectNode gObjNode;
        Editor gameObjectEditor;
        GUIStyle stylePreview;
        
        bool state;

        private float nodeWidth;
        private float lineWeight;

        private static void Init()
        {
            Debug.Log("load");
        }


        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_GameObjectNode = (target as GameObjectNode);

            // Get reference to GUIStyle
            skin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

            // Initatialize node textures
            InitTextures();

            //Set node dimensions
            nodeWidth = 250;

            //Set line width
            lineWeight = 2;

            //Draw header texture
            DrawHeaderLayout();

            //Display Node name
            GUILayout.Label("  GAME OBJECT INPUT", skin.GetStyle("Header"), GUILayout.Height(headerSection.height));
        }

        public override void OnBodyGUI()
        {

            DrawPortLayout();
            ShowGameObjectNodePorts();

            DrawBodyLayout();
            EditorGUI.indentLevel++;
            ShowGameObjectPreview();
            EditorGUI.indentLevel--;

        }

        /// <summary>
        /// Define rect values for node header and paint texture based on rect 
        /// </summary>
        private void DrawHeaderLayout()
        {
            // Set header rect dimensions
            headerSection.x = 5;
            headerSection.y = 5;
            headerSection.width = nodeWidth - 10;
            headerSection.height = 60;

            // Draw header background purple rect
            GUI.DrawTexture(headerSection, nodeTexture);
        }

        /// <summary>
        /// Define rect values for port section and paint textures based on rects 
        /// </summary>
        private void DrawPortLayout()
        {
            portSection.x = 5;
            portSection.y = headerSection.height;
            portSection.width = nodeWidth - 10;
            portSection.height = 60;

            // Draw body background purple rect below header
            GUI.DrawTexture(portSection, nodeTexture);

            // Draw line at top of body
            GUI.DrawTexture(new Rect(portSection.x, portSection.y - lineWeight, portSection.width, lineWeight), lineTexture);

            // Draw line below ports
            GUI.DrawTexture(new Rect(portSection.x, headerSection.height + portSection.height - lineWeight, portSection.width, lineWeight), lineTexture);
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayout()
        {
            bodySection.x = 5;
            bodySection.y = headerSection.height + portSection.height;
            bodySection.width = nodeWidth - 10;
            bodySection.height = 160;

            // Draw body background purple rect below header
            GUI.DrawTexture(bodySection, nodeTexture); 
        }


        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowGameObjectNodePorts()
        {
            EditorGUILayout.Space();

            GUIContent outputPortLabel = new GUIContent("GameObject\n Data Out");
            PortField(outputPortLabel, m_GameObjectNode.GetOutputPort("GameObjectDataOut"), skin.GetStyle("Port Label"), GUILayout.MinWidth(0));
        }

        /// <summary>
        /// Show the a preview image of the connected gameObject 
        /// </summary>
        private void ShowGameObjectPreview()
        {
            Rect previewBox = new Rect(bodySection.x + 15, bodySection.y, bodySection.width - 30, bodySection.height);
            GUILayout.BeginArea(previewBox);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Show label of which object is being fed to the node
            GameObject gObj = m_GameObjectNode.GameObjectDataOut;

            // Only draw the label if the object is not null
            if (gObj != null)
            {
                EditorGUILayout.LabelField(gObj.name, new GUIStyle(skin.GetStyle("Node Body Label")) { alignment = TextAnchor.MiddleLeft });
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


        /// <summary>
        /// Initatialize node textures
        /// </summary>
        private void InitTextures()
        {
            ColorUtility.TryParseHtmlString("#3A3B5B", out nodeColor);
            nodeTexture = new Texture2D(1, 1);
            nodeTexture.SetPixel(0, 0, nodeColor);
            nodeTexture.Apply();

            ColorUtility.TryParseHtmlString("#888EF7", out lineColor);
            lineTexture = new Texture2D(1, 1);
            lineTexture.SetPixel(0, 0, lineColor);
            lineTexture.Apply();
        }

        /// <summary> Make a simple port field **** overriden from XNodeEditorGUILayout to edit GUIStyle of port label *****</summary>
        private static void PortField(GUIContent label, XNode.NodePort port, GUIStyle style, params GUILayoutOption[] options)
        {
            if (port == null) return;
            if (options == null) options = new GUILayoutOption[] { GUILayout.MinWidth(30) };
            Vector2 position = Vector3.zero;
            GUIContent content = label != null ? label : new GUIContent(ObjectNames.NicifyVariableName(port.fieldName));

            // If property is an input, display a regular property field and put a port handle on the left side
            if (port.direction == XNode.NodePort.IO.Input)
            {
                // Display a label
                EditorGUILayout.LabelField(content, style, options);

                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position - new Vector2(16, 0);
            }
            // If property is an output, display a text label and put a port handle on the right side
            else if (port.direction == XNode.NodePort.IO.Output)
            {
                // Display a label
                EditorGUILayout.LabelField(content, new GUIStyle(style) { alignment = TextAnchor.UpperRight }, options);

                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position + new Vector2(rect.width, 0);
            }
            NodeEditorGUILayout.PortField(position, port);
        }
    }

}

