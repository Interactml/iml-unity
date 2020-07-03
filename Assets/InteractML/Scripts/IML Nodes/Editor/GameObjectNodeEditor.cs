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

        Editor gameObjectEditor;
        GUIStyle stylePreview;
        /// <summary>
        /// The texture displayed when the gameObject doesn't have a Mesh Renderer
        /// </summary>
        Texture2D m_NoMeshTexture;
        float m_TexHeightMultiplier = 0.45f;



        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_GameObjectNode = (target as GameObjectNode);
            nodeSpace = 230;
            NodeName = "GAME OBJECT";
            base.OnHeaderGUI();

        }

        public override void OnBodyGUI()
        {
            InputPortsNamesOverride = new Dictionary<string, string>();
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("GameObjectDataIn", "Game Object\nData In");
            base.OutputPortsNamesOverride.Add("LiveDataOut", "Rotation\nData Out");
            base.nodeTips = m_GameObjectNode.tooltips;
            m_BodyRect.height = 170;
            base.OnBodyGUI();
        }

        protected override void ShowBodyFields()
        {
            ShowGameObjectPreview();
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
                            m_NoMeshTexture = new Texture2D(1, 1);
                            // Read texture from memory
                            m_NoMeshTexture.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + "/InteractML/Resources/Icons/gameobject_transform_img.png"));
                            // Resize it
                            m_NoMeshTexture = TextureTools.ResampleAndCrop(m_NoMeshTexture, (int)NodeWidth, (int)(NodeWidth * m_TexHeightMultiplier));
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

