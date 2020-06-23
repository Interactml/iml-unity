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
        private int numberOfExamplesTrained = 0;
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
            GUILayout.Label("MACHINE LEARNING SYSTEM", m_NodeSkin.GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.Label("Regression", m_NodeSkin.GetStyle("Header Small"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));
        }

        public override void OnBodyGUI()
        {
            // Specify any overrides to port fieldNames
            if (OverridePortNames == false)
            {
                // Input ports names
                InputPortsNamesOverride = new Dictionary<string, string>();
                InputPortsNamesOverride.Add("IMLTrainingExamplesNodes", "Recorded Data In");
                InputPortsNamesOverride.Add("InputFeatures", "Live Data In");
                OverridePortNames = true;
            }

            // Unity specifically requires this to save/update any serial object.
            // serializedObject.Update(); must go at the start of an inspector gui, and
            // serializedObject.ApplyModifiedProperties(); goes at the end.
            serializedObject.Update();

            // Draw ports
            DrawPortLayout();
            //ShowSystemNodePorts();
            base.ShowNodePorts(InputPortsNamesOverride, showOutputType: true);
            //check if port is hovered over 
            PortTooltip(m_RIMLConfiguration.tips.PortTooltip);

            // Draw body Icons
            DrawBodyLayoutIcons();
            ShowIcon();

            // Draw body buttons
            DrawBodyLayoutButtons();
            ShowButtons();

            // Draw body buttons
            DrawBodyLayoutBottomButtons();
            ShowBottomButtons();

            // Draw help button
            DrawHelpButtonLayout();
            ShowHelpButton(m_HelpRect);
            
            if (m_RIMLConfiguration.Model == null || m_RIMLConfiguration.TotalNumTrainingData < 1)
            {
                DrawWarningLayout(m_HelpRect);
                ShowWarning(m_RIMLConfiguration.tips.BottomError[0]);
            }
            if (numberOfExamplesTrained != m_RIMLConfiguration.TotalNumTrainingData)
            {
                DrawWarningLayout(m_HelpRect);
                ShowWarning(m_RIMLConfiguration.tips.BottomError[2]);
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

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_ButtonsRect, NodeColor);

            // Draw line below add/remove buttons
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
        /// Define rect values for error message and paint textures based on rects 
        /// </summary>
        private void DrawHelpButtonLayout()
        {
            m_HelpRect.x = 5;
            m_HelpRect.y = HeaderRect.height + m_PortRect.height + m_IconRect.height + m_ButtonsRect.height + m_BottomButtonsRect.height;
            m_HelpRect.width = NodeWidth - 10;
            m_HelpRect.height = 40;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_HelpRect, NodeColor);
        }
        
        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowSystemNodePorts()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Live Data In");
            IMLNodeEditor.PortField(inputPortLabel, m_RIMLConfiguration.GetInputPort("InputFeatures"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("ML Out");
            IMLNodeEditor.PortField(outputPortLabel, m_RIMLConfiguration.GetOutputPort("ModelOutput"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();

            GUIContent secondInputPortLabel = new GUIContent("Recorded Data In");
            IMLNodeEditor.PortField(secondInputPortLabel, m_RIMLConfiguration.GetInputPort("IMLTrainingExamplesNodes"), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

        }


        private void ShowIcon()
        {

            m_IconCenter.x = (m_IconRect.width / 2) - 45;
            m_IconCenter.y = m_IconRect.y + 20;
            m_IconCenter.width = m_IconRect.width;
            m_IconCenter.height = m_IconRect.height - 20;

            GUILayout.BeginArea(m_IconCenter);
            m_RegressionSwitch = EditorGUILayout.Toggle(m_RegressionSwitch, m_NodeSkin.GetStyle("Regression Button"));
            GUILayout.EndArea();

            m_IconCenter.x = m_IconCenter.x + 10;
            GUILayout.BeginArea(m_IconCenter);
            GUILayout.Label("", GUILayout.MinHeight(80));
            GUILayout.Label("Regression", m_NodeSkin.GetStyle("Load Button Yellow"));
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

            TrainModelButton();
            GUILayout.Space(15);
            RunModelButton();
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
                m_RIMLConfiguration.ResetModel();
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

        private void TrainModelButton()
        {
            // Only run button logic when rapidlib reference not null and training examples are not null
            if (m_RIMLConfiguration.Model != null && m_RIMLConfiguration.TotalNumTrainingData > 0)
            {

                string nameButton = "";

                if (m_RIMLConfiguration.Training)
                    nameButton = "STOP Training Model";
                if (m_RIMLConfiguration.Trained)
                    nameButton = "Trained (" + numberOfExamplesTrained + " Examples)";
                else
                    nameButton = "Train Model";

                // Disable button if model is Running OR Trainig 
                if (m_RIMLConfiguration.Running || m_RIMLConfiguration.Training)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Train")))
                {
                    m_RIMLConfiguration.TrainModel();
                    numberOfExamplesTrained = m_RIMLConfiguration.TotalNumTrainingData;
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
                    
                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Run")))
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

