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
    [CustomNodeEditor(typeof(DTWIMLConfiguration))]
    public class CDTWIMLConfigurationEditor : IMLNodeEditor
    {

        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private DTWIMLConfiguration m_DTWIMLConfiguration;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_PortRect;
        private Rect m_BodyRect;
        private Rect m_IconRect;
        private Rect m_IconCenter;
        private Rect m_ButtonsRect;
        private Rect m_BottomButtonsRect;
        private Rect m_CenterBottomButtonsRect;
        private Rect m_HelpRect;
        private Rect m_WarningRect;
        private Rect m_InnerWarningRect;
        private Rect m_InnerInnerWarningRect;

        /// <summary>
        /// Bool for add/remove output
        /// </summary>
        private bool m_AddOutput;
        private bool m_RemoveOutput;
        private bool m_DTWSwitch; 

        #endregion

        public override void OnHeaderGUI()
        {
            // Load node skin
            if (m_NodeSkin == null)
                m_NodeSkin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

            // Get references
            m_IMLNode = target as IMLNode;
            m_IMLNodeSerialized = new SerializedObject(m_IMLNode);

            // Get reference to the current node
            m_DTWIMLConfiguration = (target as DTWIMLConfiguration);

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
            GUILayout.Space(5);
            GUILayout.Label("MACHINE LEARNING SYSTEM", m_NodeSkin.GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.Label("Dynamic Time Warping", m_NodeSkin.GetStyle("Header Small"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));
        }

        public override void OnBodyGUI()
        {
            DrawPortLayout();
            ShowSystemNodePorts();

            DrawBodyLayoutIcons();
            ShowIcons();

            DrawBodyLayoutButtons();
            ShowButtons();

            DrawBodyLayoutBottomButtons();
            ShowBottomButtons();
            
            // Draw help button
            DrawHelpButtonLayout();
            ShowHelpButton(m_HelpRect);

            DrawWarningLayout();
            ShowWarning();
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
        private void DrawBodyLayoutIcons()
        {
            m_IconRect.x = 5;
            m_IconRect.y = HeaderRect.height + m_PortRect.height;
            m_IconRect.width = NodeWidth - 10;
            m_IconRect.height = 140;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_IconRect, NodeColor);

            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_IconRect.x, (HeaderRect.height + m_PortRect.height + m_IconRect.height) - WeightOfSeparatorLine, m_IconRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayoutButtons()
        {
            m_ButtonsRect.x = 5;
            m_ButtonsRect.y = HeaderRect.height + m_PortRect.height + m_IconRect.height;
            m_ButtonsRect.width = NodeWidth - 10;
            m_ButtonsRect.height = 115;

            // Draw body background purple rect
            GUI.DrawTexture(m_ButtonsRect, NodeColor);

            // Draw line below buttons
            GUI.DrawTexture(new Rect(m_ButtonsRect.x, (HeaderRect.height + m_PortRect.height + m_IconRect.height + m_ButtonsRect.height) - WeightOfSeparatorLine, m_ButtonsRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayoutBottomButtons()
        {
            m_BottomButtonsRect.x = 5;
            m_ButtonsRect.height = m_ButtonsRect.height + 15;
            m_BottomButtonsRect.y = HeaderRect.height + m_PortRect.height + m_IconRect.height + m_ButtonsRect.height;
            m_BottomButtonsRect.width = NodeWidth - 10;
            m_BottomButtonsRect.height = 100;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_BottomButtonsRect, NodeColor);

            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_BottomButtonsRect.x, (HeaderRect.height + m_PortRect.height + m_IconRect.height + m_ButtonsRect.height + m_BottomButtonsRect.height) - WeightOfSeparatorLine, m_BottomButtonsRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawHelpButtonLayout()
        {
            m_HelpRect.x = 5;
            m_HelpRect.y = HeaderRect.height + m_PortRect.height + m_IconRect.height + m_ButtonsRect.height + m_BottomButtonsRect.height;
            m_HelpRect.width = NodeWidth - 10;
            m_HelpRect.height = 40;

            // Draw body background purple rect
            GUI.DrawTexture(m_HelpRect, NodeColor);
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawWarningLayout()
        {
            m_WarningRect.x = 5;
            m_WarningRect.y = HeaderRect.height + m_PortRect.height + m_IconRect.height + m_ButtonsRect.height + m_BottomButtonsRect.height + m_HelpRect.height;
            m_WarningRect.width = NodeWidth - 10;
            m_WarningRect.height = 120;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_WarningRect, NodeColor);
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowSystemNodePorts()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Live Data In");
            IMLNodeEditor.PortField(inputPortLabel, m_DTWIMLConfiguration.GetInputPort("InputFeatures"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("ML Out");
            IMLNodeEditor.PortField(outputPortLabel, m_DTWIMLConfiguration.GetOutputPort("ModelOutput"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();

            GUIContent secondInputPortLabel = new GUIContent("Recorded Data In");
            IMLNodeEditor.PortField(secondInputPortLabel, m_DTWIMLConfiguration.GetInputPort("IMLTrainingExamplesNodes"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));
        }

        /// <summary>
        /// Show the load, delete and record buttons
        /// </summary>
        private void ShowIcons()
        {
            m_IconCenter.x = (m_IconRect.width/2) - 45;
            m_IconCenter.y = m_IconRect.y + 20;
            m_IconCenter.width = m_IconRect.width;
            m_IconCenter.height = m_IconRect.height - 20;

            GUILayout.BeginArea(m_IconCenter);
            m_DTWSwitch = EditorGUILayout.Toggle(m_DTWSwitch, m_NodeSkin.GetStyle("DTW Button"));
            GUILayout.EndArea();

            m_IconCenter.x = m_IconCenter.x - 15;
            GUILayout.BeginArea(m_IconCenter);
            GUILayout.Label("", GUILayout.MinHeight(90));
            GUILayout.Label("Dynamic Time Warping", m_NodeSkin.GetStyle("Purple DTW Button"));
            GUILayout.EndArea();

        }

        private void ShowButtons()
        {
            m_ButtonsRect.x = m_ButtonsRect.x + 15;
            m_ButtonsRect.y = m_ButtonsRect.y + 15;
            m_ButtonsRect.width = m_ButtonsRect.width - 30;
            m_ButtonsRect.height = m_ButtonsRect.height - 15;

            GUILayout.BeginArea(m_ButtonsRect);

            ShowTrainModelButton();
            GUILayout.Space(15);
            ShowRunModelButton();
            GUILayout.EndArea();
        }

        private void ShowBottomButtons()
        {
            m_CenterBottomButtonsRect.x = (m_BottomButtonsRect.width / 2) - 15;
            m_CenterBottomButtonsRect.y = m_BottomButtonsRect.y + 20;
            m_CenterBottomButtonsRect.width = m_BottomButtonsRect.width;
            m_CenterBottomButtonsRect.height = m_BottomButtonsRect.height - 20;

            GUILayout.BeginArea(m_CenterBottomButtonsRect);
            GUILayout.BeginHorizontal();
            // Init model button (to debug the model not working)
            if (GUILayout.Button("", m_NodeSkin.GetStyle("Reset")))
            {
                m_DTWIMLConfiguration.ResetModel();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.Space(10);

            m_CenterBottomButtonsRect.x = m_CenterBottomButtonsRect.x - 15;
            GUILayout.BeginArea(m_CenterBottomButtonsRect);
            GUILayout.Label("", GUILayout.MinHeight(40));
            GUILayout.BeginHorizontal();
            GUILayout.Label("reset model", m_NodeSkin.GetStyle("Reset Pink Label"));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }
        private void ShowTrainModelButton()
        {
            // Only run button logic when rapidlib reference not null and training examples are not null
            if (m_DTWIMLConfiguration.Model != null && m_DTWIMLConfiguration.TotalNumTrainingData > 0)
            {

                string nameButton = "";

                if (m_DTWIMLConfiguration.Training)
                    nameButton = "STOP Training Model";
                else
                    nameButton = "Train Model";

                // Disable button if model is Running OR Trainig 
                if (m_DTWIMLConfiguration.Running || m_DTWIMLConfiguration.Training)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Train")))
                {
                    m_DTWIMLConfiguration.TrainModel();
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {

                GUI.enabled = false;
                if (m_DTWIMLConfiguration.TotalNumTrainingData == 0)
                {
                    //EditorGUILayout.HelpBox("There are no training examples", MessageType.Error);
                }
                if (GUILayout.Button("Train Model", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Train")))
                {
                    //m_TrainingExamplesNode.ToggleCollectExamples();
                }
                //GUI.enabled = true;

            }

        }

        private void ShowRunModelButton()
        {
            if (m_DTWIMLConfiguration.Model != null)
            {
                string nameButton = "";

                if (m_DTWIMLConfiguration.Running)
                {
                    nameButton = "STOP";
                }
                else
                {
                    nameButton = "Populate";
                }

                // Disable button if model is Trainig OR Untrained
                if (m_DTWIMLConfiguration.Training || m_DTWIMLConfiguration.Untrained)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Run")))
                {
                    m_DTWIMLConfiguration.ToggleRunning();
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button("Run", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Run")))
                {
                    //m_TrainingExamplesNode.ToggleCollectExamples();
                }
                GUI.enabled = true;
            }

        }

        private void ShowHelpButton()
        {
            m_HelpRect.x = m_HelpRect.x + 20;
            m_HelpRect.y = m_HelpRect.y + 10;
            m_HelpRect.width = m_HelpRect.width - 40;

            GUILayout.BeginArea(m_HelpRect);
            GUILayout.BeginHorizontal();
            //GUILayout.Label("advanced options", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"));
            GUILayout.Label("");
            HelpButton(this.GetType().ToString());
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void ShowWarning()
        {
            m_InnerWarningRect.x = m_WarningRect.x + 20;
            m_InnerWarningRect.y = m_WarningRect.y + 20;
            m_InnerWarningRect.width = m_WarningRect.width - 40;
            m_InnerWarningRect.height = m_WarningRect.height - 40;

            // Draw darker purple rect
            GUI.DrawTexture(m_InnerWarningRect, GetColorTextureFromHexString("#1C1D2E"));

            m_InnerInnerWarningRect.x = m_InnerWarningRect.x + 10;
            m_InnerInnerWarningRect.y = m_InnerWarningRect.y + 10;
            m_InnerInnerWarningRect.width = m_InnerWarningRect.width - 20;
            m_InnerInnerWarningRect.height = m_InnerWarningRect.height - 20;

            GUILayout.BeginArea(m_InnerInnerWarningRect);
            GUILayout.BeginHorizontal();
            GUILayout.Button("", m_NodeSkin.GetStyle("Warning"));
            GUILayout.Space(5);
            GUILayout.Label("Danger Will Robinson", m_NodeSkin.GetStyle("Warning Header"));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.Label("What the heck do you think you're doing to your nodes partner?", m_NodeSkin.GetStyle("Warning Label"));

            GUILayout.EndArea();

        }
    }
}


