using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableMethods;
using System.Linq;
using XNode;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeEditor(typeof(CIMLConfiguration))]
    public class CIMLConfigurationEditor : CRIMLConfigurationEditor
    {
        
        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private CIMLConfiguration m_CIMLConfiguration;
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
            m_CIMLConfiguration = (target as CIMLConfiguration);

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
            GUILayout.Label("Classification", m_NodeSkin.GetStyle("Header Small"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));
        }

        public override void OnBodyGUI()
        {
            // Unity specifically requires this to save/update any serial object.
            // serializedObject.Update(); must go at the start of an inspector gui, and
            // serializedObject.ApplyModifiedProperties(); goes at the end.
            serializedObject.Update();

            // Draw ports
            DrawPortLayout();
            //ShowSystemNodePorts();
            base.ShowNodePorts();
            //check if port is hovered over 
            PortTooltip(m_CIMLConfiguration.tips.PortTooltip);

            // Draw body Icons
            DrawBodyLayoutIcons();
            ShowIcon();

            // Draw body buttons
            DrawBodyLayoutButtons();
            ShowButtons();

            // Draw help button
            DrawHelpButtonLayout();
            ShowHelpButton(m_HelpRect);
            
            if (m_CIMLConfiguration.Model == null || m_CIMLConfiguration.TotalNumTrainingData < 1)
            {
                DrawWarningLayout(m_HelpRect);
                ShowWarning(m_CIMLConfiguration.tips.BottomError[0]);
            }


            if (showHelp)
            {
                ShowTooltip(m_HelpRect, m_CIMLConfiguration.tips.HelpTooltip);
            }
            if (showPort)
            {
                ShowTooltip(m_PortRect, TooltipText);
            }

            if (buttonTip)
            {
                ShowTooltip(m_ButtonsRect, TooltipText);
            }

            serializedObject.ApplyModifiedProperties();

        }

        /// <summary>
        /// Define rect values for port section and paint textures based on rects 
        /// </summary>
        private void DrawPortLayout()
        {
            // Add x units to height per extra port
            if (m_PortPairs == null)
                m_PortPairs = new List<IMLNodePortPair>();
            int numPortPairs = m_PortPairs.Count; ;
            int extraHeight = (numPortPairs * 10);

            // Draw body background purple rect below header
            m_PortRect.x = 5;
            m_PortRect.y = HeaderRect.height;
            m_PortRect.width = NodeWidth - 10;
            m_PortRect.height = 60 + extraHeight;
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
            m_ButtonsRect.height = 155;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_ButtonsRect, NodeColor);

            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_ButtonsRect.x, (HeaderRect.height + m_PortRect.height + m_IconRect.height + m_ButtonsRect.height) - WeightOfSeparatorLine, m_ButtonsRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for error message and paint textures based on rects 
        /// </summary>
        private void DrawHelpButtonLayout()
        {
            m_HelpRect.x = 5;
            m_ButtonsRect.height = m_ButtonsRect.height + 15;
            m_HelpRect.y = HeaderRect.height + m_PortRect.height + m_IconRect.height + m_ButtonsRect.height;
            m_HelpRect.width = NodeWidth - 10;
            m_HelpRect.height = 40;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_HelpRect, NodeColor);
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        
        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowSystemNodePorts()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Live Data In");
            IMLNodeEditor.PortField(inputPortLabel, m_CIMLConfiguration.GetInputPort("InputFeatures"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("ML Out");
            IMLNodeEditor.PortField(outputPortLabel, m_CIMLConfiguration.GetOutputPort("ModelOutput"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();

            GUIContent secondInputPortLabel = new GUIContent("Recorded Data In");
            IMLNodeEditor.PortField(secondInputPortLabel, m_CIMLConfiguration.GetInputPort("IMLTrainingExamplesNodes"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

        }


        private void ShowIcon()
        {

            m_IconCenter.x = (m_IconRect.width / 2) - 45;
            m_IconCenter.y = m_IconRect.y + 20;
            m_IconCenter.width = m_IconRect.width;
            m_IconCenter.height = m_IconRect.height - 20;

            GUILayout.BeginArea(m_IconCenter);
            m_ClassificationSwitch = EditorGUILayout.Toggle(m_ClassificationSwitch, m_NodeSkin.GetStyle("Classification Button"));
            GUILayout.EndArea();

            m_IconCenter.x = m_IconCenter.x + 5;
            GUILayout.BeginArea(m_IconCenter);
            GUILayout.Label("", GUILayout.MinHeight(80));
            GUILayout.Label("Classification", m_NodeSkin.GetStyle("Blue Classification Button"));
            GUILayout.EndArea();

        }

        /// <summary>
        /// Show the load, delete and record buttons
        /// </summary>
        private void ShowButtons()
        {
            m_ButtonsRect.x = m_ButtonsRect.x + 15;
            m_ButtonsRect.y = m_ButtonsRect.y + 15;
            m_ButtonsRect.width = m_ButtonsRect.width - 30;
            m_ButtonsRect.height = m_ButtonsRect.height - 15;

            GUILayout.BeginArea(m_ButtonsRect);
            // Init model button (to debug the model not working)
            if (GUILayout.Button("Reset Model", m_NodeSkin.GetStyle("Reset")))
            {
                m_CIMLConfiguration.ResetModel();
            }
            // if button contains mouse position
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                TooltipText = m_CIMLConfiguration.tips.BodyTooltip.Tips[0];
            }
            // if event type layout and mouse is not on butoon make buttonTip false
            else if (Event.current.type == EventType.Layout && !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTip = false;

            }
            // if current event is layout and mouse has been detected on button make buttontip true and button tip helper false
            if (Event.current.type == EventType.Layout && buttonTipHelper)
            {
                buttonTip = true;
                buttonTipHelper = false;
            }

            GUILayout.Space(15);
            TrainModelButton();
            GUILayout.Space(15);
            RunModelButton();
            GUILayout.EndArea();
        }

        private void TrainModelButton()
        {
            // Only run button logic when rapidlib reference not null and training examples are not null
            if (m_CIMLConfiguration.Model != null && m_CIMLConfiguration.TotalNumTrainingData > 0)
            {

                string nameButton = "";

                if (m_CIMLConfiguration.Training)
                    nameButton = "STOP Training Model";
                else
                    nameButton = "Train Model";

                // Disable button if model is Running OR Trainig 
                if (m_CIMLConfiguration.Running || m_CIMLConfiguration.Training)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Train")))
                {
                    m_CIMLConfiguration.TrainModel();
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {

                GUI.enabled = false;
                if (m_CIMLConfiguration.TotalNumTrainingData == 0)
                {
                    //EditorGUILayout.HelpBox("There are no training examples", MessageType.Error);
                }
                if (GUILayout.Button("Train Model", m_NodeSkin.GetStyle("Train")))
                {
                    //m_TrainingExamplesNode.ToggleCollectExamples();
                }
                //GUI.enabled = true;

            }
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                if (GUI.enabled)
                {
                    TooltipText = m_CIMLConfiguration.tips.BodyTooltip.Tips[1];
                } else
                {
                    TooltipText = m_CIMLConfiguration.tips.BodyTooltip.Error[0];
                }
            }
            else if (Event.current.type == EventType.MouseMove && !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTip = false;

            }

            if (Event.current.type == EventType.Layout && buttonTipHelper)
            {
                buttonTip = true;
                buttonTipHelper = false;
            }

        }

        private void RunModelButton()
        {
            bool enabled = false;
            if (m_CIMLConfiguration.Model != null)
            {
                string nameButton = "";

                if (m_CIMLConfiguration.Running)
                {
                    nameButton = "STOP";
                }
                else
                {
                    nameButton = "RUN";
                }

                // Disable button if model is Trainig OR Untrained
                if (m_CIMLConfiguration.Training || m_CIMLConfiguration.Untrained)
                {
                    GUI.enabled = false;
                    enabled = false;
                } else
                {
                    enabled = true;
                }
                    
                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Run")))
                {
                    m_CIMLConfiguration.ToggleRunning();
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button("Run", m_NodeSkin.GetStyle("Run")))
                {
                    //m_TrainingExamplesNode.ToggleCollectExamples();
                }
                GUI.enabled = true;
            }

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                if (enabled)
                {
                    TooltipText = m_CIMLConfiguration.tips.BodyTooltip.Tips[2];
                }
                else
                {
                    TooltipText = m_CIMLConfiguration.tips.BodyTooltip.Error[1];
                }
            }
            else if (Event.current.type == EventType.MouseMove && !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTip = false;

            }

            if (Event.current.type == EventType.Layout && buttonTipHelper)
            {
                buttonTip = true;
                buttonTipHelper = false;
            }

        }
    
    }
}

