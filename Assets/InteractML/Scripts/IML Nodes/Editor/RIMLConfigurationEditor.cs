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
    [CustomNodeEditor(typeof(RIMLConfiguration))]
    public class RIMLConfigurationEditor : CRIMLConfigurationEditor
    {
        
        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private RIMLConfiguration m_RIMLConfiguration;
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
            m_RIMLConfiguration = (target as RIMLConfiguration);

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
            GUILayout.Label("MACHINE LEARNING SYSTEM", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.Label("Regression", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small"), GUILayout.MinWidth(NodeWidth - 10));
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
            PortTooltip(m_RIMLConfiguration.tips.PortTooltip);

            // Draw body Icons
            DrawBodyLayoutIcons();
            ShowIcon();

            // Draw body buttons
            DrawBodyLayoutButtons();
            ShowButtons();

            // Draw help button
            DrawHelpButtonLayout();
            ShowHelpButton(m_HelpRect);
            
            if (m_RIMLConfiguration.Model == null || m_RIMLConfiguration.TotalNumTrainingData < 1)
            {
                DrawWarningLayout(m_HelpRect);
                ShowWarning(m_RIMLConfiguration.tips.BottomError[0]);
            }


            if (showHelp)
            {
                ShowTooltip(m_HelpRect, m_RIMLConfiguration.tips.HelpTooltip);
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
            m_IconsRect.x = 5;
            m_IconsRect.y = HeaderRect.height + m_PortRect.height;
            m_IconsRect.width = NodeWidth - 10;
            m_IconsRect.height = 140;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_IconsRect, NodeColor);

            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_IconsRect.x, (HeaderRect.height + m_PortRect.height + m_IconsRect.height) - WeightOfSeparatorLine, m_IconsRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayoutButtons()
        {
            m_ButtonsRect.x = 5;
            m_ButtonsRect.y = HeaderRect.height + m_PortRect.height + m_IconsRect.height;
            m_ButtonsRect.width = NodeWidth - 10;
            m_ButtonsRect.height = 155;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_ButtonsRect, NodeColor);

            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_ButtonsRect.x, (HeaderRect.height + m_PortRect.height + m_IconsRect.height + m_ButtonsRect.height) - WeightOfSeparatorLine, m_ButtonsRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for error message and paint textures based on rects 
        /// </summary>
        private void DrawHelpButtonLayout()
        {
            m_HelpRect.x = 5;
            m_ButtonsRect.height = m_ButtonsRect.height + 15;
            m_HelpRect.y = HeaderRect.height + m_PortRect.height + m_IconsRect.height + m_ButtonsRect.height;
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
            IMLNodeEditor.PortField(inputPortLabel, m_RIMLConfiguration.GetInputPort("InputFeatures"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("ML Out");
            IMLNodeEditor.PortField(outputPortLabel, m_RIMLConfiguration.GetOutputPort("ModelOutput"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();

            GUIContent secondInputPortLabel = new GUIContent("Recorded Data In");
            IMLNodeEditor.PortField(secondInputPortLabel, m_RIMLConfiguration.GetInputPort("IMLTrainingExamplesNodes"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

        }


        private void ShowIcon()
        {
            
            m_LeftIconRect.x = m_IconsRect.x + 40;
            m_LeftIconRect.y = m_IconsRect.y + 20;
            m_LeftIconRect.width = m_IconsRect.width / 2;
            m_LeftIconRect.height = m_IconsRect.height;

            m_RightIconRect.x = (m_IconsRect.width / 2) + 30;
            m_RightIconRect.y = m_IconsRect.y + 20;
            m_RightIconRect.width = m_IconsRect.width;
            m_RightIconRect.height = m_IconsRect.height;


            GUILayout.BeginArea(m_RightIconRect);
            m_RegressionSwitch = EditorGUILayout.Toggle(m_RegressionSwitch, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Regression Button"));
            GUILayout.EndArea();

            m_RightIconRect.x = m_RightIconRect.x + 10;
            GUILayout.BeginArea(m_RightIconRect);
            GUILayout.Label("", GUILayout.MinHeight(80));
            GUILayout.Label("Regression", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Load Button Yellow"));
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
            if (GUILayout.Button("Reset Model", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Reset")))
            {
                m_RIMLConfiguration.ResetModel();
            }
            // if button contains mouse position
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                TooltipText = m_RIMLConfiguration.tips.BodyTooltip.Tips[0];
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
            if (m_RIMLConfiguration.Model != null && m_RIMLConfiguration.TotalNumTrainingData > 0)
            {

                string nameButton = "";

                if (m_RIMLConfiguration.Training)
                    nameButton = "STOP Training Model";
                else
                    nameButton = "Train Model";

                // Disable button if model is Running OR Trainig 
                if (m_RIMLConfiguration.Running || m_RIMLConfiguration.Training)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Train")))
                {
                    m_RIMLConfiguration.TrainModel();
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {

                GUI.enabled = false;
                if (m_RIMLConfiguration.TotalNumTrainingData == 0)
                {
                    //EditorGUILayout.HelpBox("There are no training examples", MessageType.Error);
                }
                if (GUILayout.Button("Train Model", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Train")))
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
                    TooltipText = m_RIMLConfiguration.tips.BodyTooltip.Tips[1];
                } else
                {
                    TooltipText = m_RIMLConfiguration.tips.BodyTooltip.Error[0];
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
            if (m_RIMLConfiguration.Model != null)
            {
                string nameButton = "";

                if (m_RIMLConfiguration.Running)
                {
                    nameButton = "STOP";
                }
                else
                {
                    nameButton = "RUN";
                }

                // Disable button if model is Trainig OR Untrained
                if (m_RIMLConfiguration.Training || m_RIMLConfiguration.Untrained)
                {
                    GUI.enabled = false;
                    enabled = false;
                } else
                {
                    enabled = true;
                }
                    
                if (GUILayout.Button(nameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Run")))
                {
                    m_RIMLConfiguration.ToggleRunning();
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

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                if (enabled)
                {
                    TooltipText = m_RIMLConfiguration.tips.BodyTooltip.Tips[2];
                }
                else
                {
                    TooltipText = m_RIMLConfiguration.tips.BodyTooltip.Error[1];
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

