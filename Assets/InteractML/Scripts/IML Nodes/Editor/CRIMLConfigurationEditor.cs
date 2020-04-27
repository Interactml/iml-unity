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
    [CustomNodeEditor(typeof(CRIMLConfiguration))]
    public class CRIMLConfigurationEditor : IMLNodeEditor
    {

        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private CRIMLConfiguration m_CRIMLConfiguration;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_BodyRect;
        private Rect m_BodyRectButtons;
        private Rect m_BodyRectBigButtons;
        private Rect m_BodyRectBottom;
        private Rect m_PortRect;


        /// <summary>
        /// Bool for add/remove output
        /// </summary>
        private bool m_AddOutput;
        private bool m_RemoveOutput;

        #endregion

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_CRIMLConfiguration = (target as CRIMLConfiguration);

            NodeWidth = 300;

            // Initialise header background Rects
            InitHeaderRects();

            NodeColor = GetColorTextureFromHexString("#3A3B5B");

            // Draw header background Rect
            GUI.DrawTexture(HeaderRect, NodeColor);

            // Draw line below header
            GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#5EB3F9"));

            //Display Node name
            GUILayout.BeginArea(HeaderRect);
            GUILayout.Label("MACHINE LEARNING SYSTEM", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.Label("Classification and Regression", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));
        }

        public override void OnBodyGUI()
        {
            DrawPortLayout();
            ShowSystemNodePorts();
            //GUILayout.Label("", GUILayout.MinHeight(50));

            DrawBodyLayoutButtons();
            ShowButtons();

            DrawBodyLayoutBigButtons();
            ShowBigButtons();

            DrawBodyLayoutBottom();
            ShowBottomSection();

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
            GUI.DrawTexture(new Rect(m_PortRect.x, HeaderRect.height + m_PortRect.height - WeightOfSectionLine, m_PortRect.width, WeightOfSectionLine), GetColorTextureFromHexString("#5EB3F9"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayoutButtons()
        {
            m_BodyRectButtons.x = 5;
            m_BodyRectButtons.y = HeaderRect.height + m_PortRect.height;
            m_BodyRectButtons.width = NodeWidth - 10;
            m_BodyRectButtons.height = 120;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_BodyRectButtons, NodeColor);

            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_BodyRectButtons.x, (HeaderRect.height + m_PortRect.height + m_BodyRectButtons.height) - WeightOfSeparatorLine, m_BodyRectButtons.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayoutBigButtons()
        {
            m_BodyRectBigButtons.x = 5;
            m_BodyRectBigButtons.y = HeaderRect.height + m_PortRect.height + m_BodyRectButtons.height;
            m_BodyRectBigButtons.width = NodeWidth - 10;
            m_BodyRectBigButtons.height = 120;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_BodyRectBigButtons, NodeColor);

            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_BodyRectBigButtons.x, (HeaderRect.height + m_PortRect.height + m_BodyRectButtons.height + m_BodyRectBigButtons.height) - WeightOfSeparatorLine, m_BodyRectBigButtons.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayoutBottom()
        {
            m_BodyRectBottom.x = 5;
            m_BodyRectBottom.y = HeaderRect.height + m_PortRect.height + m_BodyRectButtons.height + m_BodyRectBigButtons.height;
            m_BodyRectBottom.width = NodeWidth - 10;
            m_BodyRectBottom.height = 40;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_BodyRectBottom, NodeColor);
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowSystemNodePorts()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Live Data In");
            IMLNodeEditor.PortField(inputPortLabel, m_CRIMLConfiguration.GetInputPort("InputFeatures"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("ML Out");
            IMLNodeEditor.PortField(outputPortLabel, m_CRIMLConfiguration.GetOutputPort("ModelOutput"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();

            GUIContent secondInputPortLabel = new GUIContent("Recorded Data In");
            IMLNodeEditor.PortField(secondInputPortLabel, m_CRIMLConfiguration.GetInputPort("IMLTrainingExamplesNodes"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));
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

            GUILayout.Button("Classification", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Classification Button"));
            GUILayout.Label("");
            GUILayout.Button("Regression", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Regression Button"));
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(100));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Classification", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Blue Classification Button"));
            EditorGUILayout.Space();
            GUILayout.Label("Regression", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Load Button Yellow"));
            GUILayout.EndHorizontal();
        }

        private void ShowBigButtons()
        {
            GUILayout.BeginArea(m_BodyRectBigButtons);
            // Init model button (to debug the model not working)
            if (GUILayout.Button("Reset Model"))
            {
                m_CRIMLConfiguration.ResetModel();
            }
            ShowTrainModelButton();
            ShowRunModelButton();
            GUILayout.EndArea();
        }

        private void ShowTrainModelButton()
        {
            // Only run button logic when rapidlib reference not null and training examples are not null
            if (m_CRIMLConfiguration.Model != null && m_CRIMLConfiguration.TotalNumTrainingData > 0)
            {

                string nameButton = "";

                if (m_CRIMLConfiguration.Training)
                    nameButton = "STOP Training Model";
                else
                    nameButton = "Train Model";

                // Disable button if model is Running OR Trainig 
                if (m_CRIMLConfiguration.Running || m_CRIMLConfiguration.Training)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton))
                {
                    m_CRIMLConfiguration.TrainModel();
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {

                GUI.enabled = false;
                if (m_CRIMLConfiguration.TotalNumTrainingData == 0)
                {
                    EditorGUILayout.HelpBox("There are no training examples", MessageType.Error);
                }
                if (GUILayout.Button("Train Model"))
                {
                    //m_TrainingExamplesNode.ToggleCollectExamples();
                }
                //GUI.enabled = true;

            }

        }

        private void ShowRunModelButton()
        {
            if (m_CRIMLConfiguration.Model != null)
            {
                string nameButton = "";

                if (m_CRIMLConfiguration.Running)
                {
                    nameButton = "STOP";
                }
                else
                {
                    nameButton = "Populate";
                }

                // Disable button if model is Trainig OR Untrained
                if (m_CRIMLConfiguration.Training || m_CRIMLConfiguration.Untrained)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton))
                {
                    m_CRIMLConfiguration.ToggleRunning();
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button("Run"))
                {
                    //m_TrainingExamplesNode.ToggleCollectExamples();
                }
                GUI.enabled = true;
            }

        }

        private void ShowBottomSection()
        {
            m_BodyRectBottom.x = m_BodyRectBottom.x + 20;
            m_BodyRectBottom.y = m_BodyRectBottom.y + 10;
            m_BodyRectBottom.width = m_BodyRectBottom.width - 40;

            GUILayout.BeginArea(m_BodyRectBottom);
            GUILayout.BeginHorizontal();
            GUILayout.Label("advanced options", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"));
            GUILayout.Label("");
            GUILayout.Button("Help", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Help Button"));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}

