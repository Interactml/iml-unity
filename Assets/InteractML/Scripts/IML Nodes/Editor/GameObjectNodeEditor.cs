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
        private Rect m_HelpRect;
        private Rect m_ToolRect;

        Editor gameObjectEditor;
        GUIStyle stylePreview;

        bool state;

        /// <summary>
        /// The texture displayed when the gameObject doesn't have a Mesh Renderer
        /// </summary>
        Texture2D m_NoMeshTexture;
        float m_TexHeightMultiplier = 0.45f;

        string tooltip = "";
        //bool tooltipOn = true;
        int counter = 0;
        int count = 2;

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
            GUILayout.BeginArea(HeaderRect);
            GUILayout.Space(10);
            GUILayout.Label("GAME OBJECT INPUT", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));

        }

        public override void OnBodyGUI()
        {

            if (toolTipOn)
            {
                ShowTooltip(m_ToolRect, m_HelpRect, m_GameObjectNode.tips.HelpTooltip);
            }
            // Draw Port Section
            DrawPortLayout();
            ShowGameObjectNodePorts();

            DrawBodyLayout();
            EditorGUI.indentLevel++;
            ShowGameObjectPreview();
            EditorGUI.indentLevel--;

            // Draw help button
            DrawHelpButtonLayout();
            ShowHelpButton(m_HelpRect);
            
                
            
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
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawHelpButtonLayout()
        {
            m_HelpRect.x = 5;
            m_HelpRect.y = HeaderRect.height + m_PortRect.height + m_BodyRect.height;
            m_HelpRect.width = NodeWidth - 10;
            m_HelpRect.height = 40;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_HelpRect, NodeColor);

            //Draw separator line
            GUI.DrawTexture(new Rect(m_HelpRect.x, HeaderRect.height + m_PortRect.height + m_BodyRect.height - WeightOfSeparatorLine, m_HelpRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowGameObjectNodePorts()
        {
            GUILayout.Space(5);
            GUIContent outputPortLabel = new GUIContent("GameObject\n Data Out", m_GameObjectNode.tips.PortTooltip[0]);
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

                // If the gameObject has a mesh filter... 
                var has3DMesh = gObj.GetComponentInChildren<MeshFilter>() == null ? false : true;
                if (has3DMesh)
                {
                    // Preview the 3d model in the node editor
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
                    }

                    // Draw preview of Game Object
                    if (stylePreview != null && gameObjectEditor != null)
                        gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(100, 100), stylePreview);
                    else
                        Debug.LogError("Null reference in the preview style of the GameObjectNode");

                }
                // If the gameObject doesn't have a mesh filter...
                else
                {
                    bool createNewTexture = false;
                    if (createNewTexture)
                    {
                        // Create new red texture for testing
                        m_NoMeshTexture = new Texture2D((int)NodeWidth, (int)(NodeWidth * m_TexHeightMultiplier));
                        var colors = new Color[m_NoMeshTexture.width * m_NoMeshTexture.height];
                        if (colors.Length > 0)
                        {
                            for (int i = 0; i < colors.Length; i++)
                            {
                                colors[i] = Color.yellow;
                            }
                        }
                        // Set red colour on all mipmaps
                        for (int m = 0; m < m_NoMeshTexture.mipmapCount; m++)
                            m_NoMeshTexture.SetPixels(colors, m);
                        m_NoMeshTexture.Apply();

                    }
                    // If we want to use a texture from assets...
                    else
                    {
                        // Only load once
                        if (m_NoMeshTexture == null)
                        {
                            // Create new empty texture
                            m_NoMeshTexture = new Texture2D(1,1);
                            // Read texture from memory
                            m_NoMeshTexture.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + "/InteractML/Resources/Icons/gameobject_transform_img.png"));
                            // Resize it
                            m_NoMeshTexture = TextureTools.ResampleAndCrop(m_NoMeshTexture, (int)NodeWidth, (int)(NodeWidth* m_TexHeightMultiplier));
                        }
                    }

                    // Draw texture
                    EditorGUI.DrawPreviewTexture(new Rect(0f, 35f, m_NoMeshTexture.width, m_NoMeshTexture.height), m_NoMeshTexture);
                }
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

