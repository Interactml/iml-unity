using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(ArrayNode))]
    public class ArrayNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ArrayNode m_ArrayNode;

        /// <summary>
        /// boolean for dropdown
        /// </summary>
        private bool m_ShowArrayDataDropdown;

        /// <summary>
        /// rect for elements dropdown
        /// </summary>
        protected Rect m_Dropdown;

        /// <summary>
        /// Position of scroll for dropdown
        /// </summary>
        protected Vector2 m_ScrollPos;

        /// <summary>
        /// Initialise node specific interface values
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_ArrayNode = (target as ArrayNode);

            // Initialise node name
            NodeName = "ARRAY";

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Array\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("m_Out", "Array \nData Out");

            // Initialise node tooltips
            base.nodeTips = m_ArrayNode.tooltips;

            // Initialise axis labels
            feature_labels = new string[0];

        }


        protected override void ShowBodyFields()
        {
            
            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);

            UpdateFeatureLabelArray();

            // check if there is a connection
            if (!m_ArrayNode.IsInputConnected())
            {
                // set node length
                nodeSpace = 120;
                m_BodyRect.height = 60;
                // alert to connect an array to input
                EditorGUILayout.LabelField("Connect an array", m_NodeSkin.GetStyle("Node Body Label"));
            }
            // check if there are any features connected
            else 
            {
                // check there are features to show
                if (m_ArrayNode.FeatureValues.Values != null && m_ArrayNode.FeatureValues.Values.Length != 0)
                {
                    // show toggles and values for number of elements up to 6
                    if (m_ArrayNode.FeatureValues.Values.Length < 7)
                    {
                        // dynamically adjust node length based on amount of velocity features
                        nodeSpace = 120 + (m_ArrayNode.FeatureValues.Values.Length * 24);
                        m_BodyRect.height = 60 + (m_ArrayNode.FeatureValues.Values.Length * 24);

                        // draws each feature in the node
                        DataTypeNodeEditorMethods.DrawFeatureValueToggleAndLabel(this, m_ArrayNode);
                    }
                    else if (m_ArrayNode.FeatureValues.Values.Length > m_ArrayNode.m_MaximumArraySize)
                    {
                        // set node length
                        nodeSpace = 160;
                        m_BodyRect.height = 100;
                        // alert that array length is too long
                        ShowWarning(m_ArrayNode.tooltips.BottomError[0]);
                    }  
                    else
                    {
                        // set node length
                        nodeSpace = 120;
                        m_BodyRect.height = 60;
                        // show dropdown scroll view of elements
                        SetDropdownStyle();
                        ShowArrayDataDropdown();
                    }

                }
                else
                {
                    // set node length
                    nodeSpace = 120;
                    m_BodyRect.height = 60;

                    // if input is detect without features alert user there is connection 
                    if (m_ArrayNode.GetInputNodesConnected("m_In")[0].GetType() == typeof(InteractML.ScriptNode))
                        EditorGUILayout.LabelField("Script connection detected", m_NodeSkin.GetStyle("Node Body Label"));
                    else
                        EditorGUILayout.LabelField("Connection detected", m_NodeSkin.GetStyle("Node Body Label"));
                }
            }

            GUILayout.EndArea();
            



        }

        /// <summary>
        /// Set style for dropdown for training examples nodes
        /// </summary>
        protected void SetDropdownStyle()
        {
            GUI.skin = m_NodeSkin;
            GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
            Color myStyleColor = Color.white;
            myFoldoutStyle.fontStyle = FontStyle.Bold;
            myFoldoutStyle.normal.textColor = myStyleColor;
            myFoldoutStyle.onNormal.textColor = myStyleColor;
            myFoldoutStyle.hover.textColor = myStyleColor;
            myFoldoutStyle.onHover.textColor = myStyleColor;
            myFoldoutStyle.focused.textColor = myStyleColor;
            myFoldoutStyle.onFocused.textColor = myStyleColor;
            myFoldoutStyle.active.textColor = myStyleColor;
            myFoldoutStyle.onActive.textColor = myStyleColor;
            myFoldoutStyle.fontStyle = FontStyle.Bold;
            myFoldoutStyle.normal.textColor = myStyleColor;
            myFoldoutStyle.fontStyle = m_NodeSkin.GetStyle("scrollview").fontStyle;
            
            m_ShowArrayDataDropdown = EditorGUILayout.Foldout(m_ShowArrayDataDropdown, "View Array Values", myFoldoutStyle);
        }

        protected void UpdateFeatureLabelArray()
        {
            if (feature_labels.Length != m_ArrayNode.FeatureValues.Values.Length)
            {
                feature_labels = new string[m_ArrayNode.FeatureValues.Values.Length];
                for (int i = 0; i < feature_labels.Length; i++)
                    feature_labels[i] = " ";           
            }
        }

        /// <summary>
        /// Show single training examples on foldout arrow
        /// </summary>
        private void ShowArrayDataDropdown()
        {
            if (m_ShowArrayDataDropdown)
            {
                // dynamically adjust node length based on amount of velocity features
                nodeSpace = 280;
                m_BodyRect.height = 220;

                m_Dropdown.x = m_InnerBodyRect.x - 30;
                m_Dropdown.y = m_InnerBodyRect.y - m_BodyRect.height + 110;
                m_Dropdown.width = m_InnerBodyRect.width - 10;
                m_Dropdown.height = 150;

                GUI.DrawTexture(m_Dropdown, NodeColor);

                GUILayout.BeginArea(m_Dropdown);
                

                // Begins Vertical Scroll
                EditorGUILayout.BeginVertical();

                m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos); //GUILayout.Width(GetWidth() - 10), GUILayout.Height(GetWidth() - 50));
                // draws each feature in the node

                EditorGUILayout.Space();
                // for each of the features in the data type
                for (int i = 0; i < m_ArrayNode.FeatureValues.Values.Length; i++)
                {
                    // Draw each feature on a single line
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    //draw toggle
                    m_ArrayNode.ToggleSwitches[i] = EditorGUILayout.Toggle(m_ArrayNode.ToggleSwitches[i], IMLNodeEditorMethods.DataInToggle(this, m_ArrayNode.FeatureValueReceivingData[i]));
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    //draw label
                    EditorGUILayout.LabelField(feature_labels[i] + System.Math.Round(m_ArrayNode.FeatureValues.Values[i], 3).ToString(), m_NodeSkin.GetStyle("scrollview"));

                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                }
                EditorGUILayout.Space();
                // Ends Vertical Scroll
                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
                
                GUILayout.EndArea();
            }

        }
    }
}

