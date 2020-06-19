using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableMethods;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeEditor(typeof(TrainingExamplesNode))]
    public class TrainingExamplesNodeEditor : IMLNodeEditor
    {
        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        protected TrainingExamplesNode m_TrainingExamplesNode;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        protected Rect m_BodyRectInputs;
        protected Rect m_BodyRectTargets;
        protected Rect m_BodyRectButtons;
        protected Rect m_BodyRectBottom;
        protected Rect m_Dropdown;


        /// <summary>
        /// Bool for add/remove output
        /// </summary>
        private bool m_AddOutput;
        private bool m_RemoveOutput;

        private static GUIStyle editorLabelStyle;

        bool help = false;

        protected bool m_ShowTrainingDataDropdown;
        protected Vector2 m_ScrollPos;

        #endregion

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_TrainingExamplesNode = (target as TrainingExamplesNode);

            NodeWidth = 300;

            // Initialise header background Rects
            InitHeaderRects();

            NodeColor = GetColorTextureFromHexString("#3A3B5B");

            // Draw header background Rect
            GUI.DrawTexture(HeaderRect, NodeColor);

            // Draw line below header
            GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#74DF84"));

            //Display Node name
            GUILayout.BeginArea(HeaderRect);
            GUILayout.Space(5);
            GUILayout.Label("TEACH THE MACHINE", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.Label("Dynamic Time Warping", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));


        }

        public override void OnBodyGUI()
        {
            DrawPortLayout();
            ShowSeriesTrainingExamplesNodePorts();
            GUILayout.Space(50);

            DrawBodyLayoutInputs(m_TrainingExamplesNode.DesiredInputsConfig.Count);
            DrawValues(m_TrainingExamplesNode.DesiredInputFeatures, "Input Values");

            DrawBodyLayoutTargets(m_TrainingExamplesNode.DesiredOutputFeatures.Count);
            GUILayout.Space(80);
            DrawValues(m_TrainingExamplesNode.DesiredOutputFeatures, "Target Values");

            DrawBodyLayoutButtons();
            ShowButtons();

            DrawBodyLayoutBottom();
            ShowHelpButton(m_BodyRectBottom);

        }

        /// <summary>
        /// Define rect values for port section and paint textures based on rects 
        /// </summary>
        protected void DrawPortLayout()
        {
            // Draw body background purple rect below header
            m_PortRect.x = 5;
            m_PortRect.y = HeaderRect.height;
            m_PortRect.width = NodeWidth - 10;
            m_PortRect.height = 60;
            GUI.DrawTexture(m_PortRect, NodeColor);

            // Draw line below ports
            GUI.DrawTexture(new Rect(m_PortRect.x, HeaderRect.height + m_PortRect.height - WeightOfSectionLine, m_PortRect.width, WeightOfSectionLine), GetColorTextureFromHexString("#74DF84"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        protected void DrawBodyLayoutInputs(int connectedInputs)
        {
            m_BodyRectInputs.x = 5;
            m_BodyRectInputs.y = HeaderRect.height + m_PortRect.height;
            m_BodyRectInputs.width = NodeWidth - 10;
            m_BodyRectInputs.height = 80 + connectedInputs * 60;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_BodyRectInputs, NodeColor);


            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_BodyRectInputs.x, m_BodyRectInputs.y + m_BodyRectInputs.height - WeightOfSeparatorLine, m_BodyRectInputs.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        protected void DrawBodyLayoutTargets(int connectedTargets)
        {
            m_BodyRectTargets.x = 5;
            m_BodyRectTargets.y = m_BodyRectInputs.y + m_BodyRectInputs.height + WeightOfSeparatorLine;
            m_BodyRectTargets.width = NodeWidth - 10;
            m_BodyRectTargets.height = 80 + connectedTargets * 60;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_BodyRectTargets, NodeColor);

            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_BodyRectTargets.x, m_BodyRectTargets.y + m_BodyRectTargets.height - WeightOfSeparatorLine, m_BodyRectTargets.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }



        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        protected void DrawBodyLayoutButtons()
        {
            m_BodyRectButtons.x = 5;
            m_BodyRectButtons.y = m_BodyRectTargets.y + m_BodyRectTargets.height;
            m_BodyRectButtons.width = NodeWidth - 10;
            m_BodyRectButtons.height = 95;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_BodyRectButtons, NodeColor);

            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_BodyRectButtons.x, (m_BodyRectButtons.y + m_BodyRectButtons.height) - WeightOfSeparatorLine, m_BodyRectButtons.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        protected void DrawBodyLayoutBottom()
        {
            m_BodyRectBottom.x = 5;
            m_BodyRectBottom.y = m_BodyRectButtons.y + 40;
            m_BodyRectBottom.width = NodeWidth - 10;
            m_BodyRectBottom.height = 40;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_BodyRectBottom, NodeColor);
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowSeriesTrainingExamplesNodePorts()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Live Data In");
            IMLNodeEditor.PortField(inputPortLabel, m_TrainingExamplesNode.GetInputPort("InputFeatures"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Recorded\nData Out");
            IMLNodeEditor.PortField(outputPortLabel, m_TrainingExamplesNode.GetOutputPort("TrainingExamplesNodeToOutput"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();

            GUIContent secondInputPortLabel = new GUIContent("Target Values");
            IMLNodeEditor.PortField(secondInputPortLabel, m_TrainingExamplesNode.GetInputPort("TargetValues"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

        }

        /// <summary>
        /// Show the load, delete and record buttons
        /// </summary>
        private void ShowButtons()
        {
            m_BodyRectButtons.x = m_BodyRectButtons.x + 30;
            m_BodyRectButtons.y = m_BodyRectButtons.y + 20;
            m_BodyRectButtons.width = m_BodyRectButtons.width - 70;

            GUILayout.BeginArea(m_BodyRectButtons);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Load Data", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Load Button")))
            {
                m_TrainingExamplesNode.LoadDataFromDisk();
            }
            GUILayout.Label("");

            ShowClearAllExamplesButton();
            GUILayout.Label("");

            string recordNameButton = ShowRecordExamplesButton();

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            m_BodyRectButtons.x = m_BodyRectButtons.x - 10;
            m_BodyRectButtons.y = m_BodyRectButtons.y + 35;
            m_BodyRectButtons.width = m_BodyRectButtons.width + 40;
            GUILayout.BeginArea(m_BodyRectButtons);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Load Data", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Load Button Yellow"));
            GUILayout.Label("");
            GUILayout.Label("Delete Data", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button Pink"));
            GUILayout.Label("");
            GUILayout.Label(recordNameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green"));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();


        }

        private string ShowRecordExamplesButton()
        {
            string nameButton = "";

            //// Only run button logic when there are features to extract from
            //if (!Lists.IsNullOrEmpty(ref m_SeriesTrainingExamplesNode.InputFeatures))
            //{
            if (m_TrainingExamplesNode.CollectingData)
            {
                nameButton = "       STOP          ";
            }
            else
            {
                nameButton = "Record Series   ";
            }

            bool disableButton = false;

            // If there are any models connected we check some conditions
            /*if (!Lists.IsNullOrEmpty(ref m_SeriesTrainingExamplesNode.IMLConfigurationNodesConnected))
            {
                for (int i = 0; i < m_SeriesTrainingExamplesNode.IMLConfigurationNodesConnected.Count; i++)
                {
                    var IMLConfigNodeConnected = m_SeriesTrainingExamplesNode.IMLConfigurationNodesConnected[i];
                    // Disable button if model(s) connected are runnning or training
                    if (IMLConfigNodeConnected.Running || IMLConfigNodeConnected.Training)
                    {
                        disableButton = true;
                        break;
                    }

                }
            }*/

            // Draw button
            if (disableButton)
                GUI.enabled = false;
            if (GUILayout.Button("Record Data", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button")))
            {
                m_TrainingExamplesNode.ToggleCollectExamples();
            }
            // Always enable it back at the end
            GUI.enabled = true;
            return nameButton;
            //}
        }

        private void ShowClearAllExamplesButton()
        {

            bool disableButton = false;

            if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.TrainingExamplesVector))
            {
                disableButton = false;
            }

            // Only run button logic when there are training examples to delete
            if (!disableButton)
            {
                // If there are any models connected we check some conditions
                /*if (!Lists.IsNullOrEmpty(ref m_SeriesTrainingExamplesNode.IMLConfigurationNodesConnected))
                {
                    foreach (var IMLConfigNode in m_SeriesTrainingExamplesNode.IMLConfigurationNodesConnected)
                    {
                        // Disable button if any of the models is runnning OR collecting data OR training
                        if (IMLConfigNode.Running || IMLConfigNode.Training || m_SeriesTrainingExamplesNode.CollectingData)
                        {
                            disableButton = true;
                            break;
                        }
                    }
                }*/

                // Draw button
                if (disableButton)
                    GUI.enabled = false;
                if (GUILayout.Button("Delete Data", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button")))
                {
                    m_TrainingExamplesNode.ClearTrainingExamples();
                }
                // Always enable it back at the end
                GUI.enabled = true;



            }
            // If there are no training examples to delete we draw a disabled button
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button("Delete Data", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button")))
                {
                    m_TrainingExamplesNode.ToggleCollectExamples();
                }
                GUI.enabled = true;
            }
        }
        protected virtual void ShowTrainingExamplesDropdown()
        {
            Debug.Log("should be implemented in single or series node editor");
        }

        protected void SetDropdownStyle()
        {
            GUI.skin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");
            GUILayout.Space(140);
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
            myFoldoutStyle.fontStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview").fontStyle;
            m_ShowTrainingDataDropdown = EditorGUILayout.Foldout(m_ShowTrainingDataDropdown, "View Training Pairs", myFoldoutStyle);
        }




    }

}
