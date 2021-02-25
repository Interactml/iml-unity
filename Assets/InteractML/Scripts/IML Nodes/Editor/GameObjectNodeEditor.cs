using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    /// <summary>
    /// Editor class drawing a Game Object Feature - receiving the transform from a game object in the scene.
    /// </summary>
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

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_GameObjectNode = (target as GameObjectNode);

            // Initialise node name
            NodeName = "GAME OBJECT";

            // Initialise node height
            m_BodyRect.height = 170;
            nodeSpace = 230;

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("GameObjectDataOut", "Game Object\nData Out");
            
            // Initialise node tooltips
            base.nodeTips = m_GameObjectNode.tooltips;

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
            // Set size of preview box
            Rect previewBox = new Rect(m_BodyRect.x + 20, m_BodyRect.y, m_BodyRect.width - 40, m_BodyRect.height);

            GUILayout.BeginArea(previewBox);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Show label of which object is being fed to the node
            GameObject gObj = m_GameObjectNode.GameObjectDataOut;

            // Only draw the preview if the object is not null
            if (gObj != null)
            {

                EditorGUILayout.LabelField(gObj.name, m_NodeSkin.GetStyle("Node Body Label"));

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
                    {
                        Rect auxRect = GUILayoutUtility.GetRect(100, 100);
                        auxRect.width = 200;
                        auxRect.height = 100;
                        gameObjectEditor.OnPreviewGUI(auxRect, stylePreview);
                    }
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
                            try
                            {
                                m_NoMeshTexture.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + "/InteractML/Resources/Icons/gameobject_transform_img.png"));
                            }
                            catch (System.Exception e)
                            {

                                Debug.LogError(e.Message);
                            }
                            // Only resize it if the loading was successful
                            if (m_NoMeshTexture.width != 1)
                                // Resize it
                                m_NoMeshTexture = TextureTools.ResampleAndCrop(m_NoMeshTexture, (int)NodeWidth, (int)(NodeWidth * m_TexHeightMultiplier));

                        }
                    }

                    // Draw texture
                    EditorGUI.DrawPreviewTexture(new Rect(0f, 35f, m_NoMeshTexture.width, m_NoMeshTexture.height), m_NoMeshTexture);
                }
            }
            // If it is null, we warn the user
            else
            {
                EditorGUILayout.LabelField("Connect a GameObject", m_NodeSkin.GetStyle("Node Body Label"));
                //EditorGUILayout.HelpBox("Connect a GameObject", MessageType.Error);
            }

            GUILayout.EndArea();

        }





    }

}

