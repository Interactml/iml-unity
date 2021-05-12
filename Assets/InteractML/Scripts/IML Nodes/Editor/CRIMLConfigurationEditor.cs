﻿using System.Collections;
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
        protected Rect m_PortRect;
        protected Rect m_BodyRect;
        protected Rect m_IconRect;
        protected Rect m_IconCenter;
        protected Rect m_RightIconRect;
        protected Rect m_ButtonsRect;
        protected Rect m_BottomButtonsRect;
        protected Rect m_CenterBottomButtonsRect;
        protected Rect m_HelpRect;
        protected Rect m_ToolRect;



        /// <summary>
        /// Bool for add/remove output
        /// </summary>
        protected bool m_AddOutput;
        protected bool m_MinusOutput;

        protected bool m_ClassificationSwitch;
        protected bool m_RegressionSwitch;

        protected bool buttonTip;
        protected bool buttonTipHelper;

        protected bool bottomButtonTip;
        protected bool bottomButtonTipHelper;

        #endregion

      

     /*   public override void OnBodyGUI()
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
            base.ShowNodePorts();
            //check if port is hovered over 


            // Draw body Icons
            DrawBodyLayoutIcons();
            ShowIcon();

            // Draw body buttons
            DrawBodyLayoutButtons();
            ShowButtons();

            // Draw help button
            DrawHelpButtonLayout();
            ShowHelpButton(m_HelpRect);
            
            if (m_CRIMLConfiguration.Model == null || m_CRIMLConfiguration.TotalNumTrainingData < 1)
            {
                DrawWarningLayout(m_HelpRect);
                ShowWarning(m_CRIMLConfiguration.tips.BottomError[0]);
            }


            if (showHelp)
            {
                ShowTooltip(m_HelpRect, m_CRIMLConfiguration.tips.HelpTooltip);
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

        }*/

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
            m_BottomButtonsRect.height = m_BottomButtonsRect.height + 15;
            m_HelpRect.y = HeaderRect.height + m_PortRect.height + m_IconRect.height + m_ButtonsRect.height + m_BottomButtonsRect.height;
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
            IMLNodeEditor.PortField(inputPortLabel, m_CRIMLConfiguration.GetInputPort("InputFeatures"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("ML Out");
            IMLNodeEditor.PortField(outputPortLabel, m_CRIMLConfiguration.GetOutputPort("ModelOutput"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();

            GUIContent secondInputPortLabel = new GUIContent("Recorded Data In");
            IMLNodeEditor.PortField(secondInputPortLabel, m_CRIMLConfiguration.GetInputPort("IMLTrainingExamplesNodes"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

        }


        private void ShowIcon()
        {
            
            m_IconCenter.x = m_IconRect.x + 40;
            m_IconCenter.y = m_IconRect.y + 20;
            m_IconCenter.width = m_IconRect.width / 2;
            m_IconCenter.height = m_IconRect.height;

            m_RightIconRect.x = (m_IconRect.width / 2) + 30;
            m_RightIconRect.y = m_IconRect.y + 20;
            m_RightIconRect.width = m_IconRect.width;
            m_RightIconRect.height = m_IconRect.height;

            GUILayout.BeginArea(m_IconCenter);
            m_ClassificationSwitch = EditorGUILayout.Toggle(m_ClassificationSwitch, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Classification Button"));
            GUILayout.EndArea();

            GUILayout.BeginArea(m_RightIconRect);
            m_RegressionSwitch = EditorGUILayout.Toggle(m_RegressionSwitch, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Regression Button"));
            GUILayout.EndArea();

            m_IconCenter.x = m_IconCenter.x + 5;
            GUILayout.BeginArea(m_IconCenter);
            GUILayout.Label("", GUILayout.MinHeight(80));
            GUILayout.Label("Classification", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Blue Classification Button"));
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
                m_CRIMLConfiguration.ResetModel();
            }
            // if button contains mouse position
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                TooltipText = m_CRIMLConfiguration.tips.BodyTooltip.Tips[0];
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
                if (GUILayout.Button(nameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Train")))
                {
                    // Train model
                    if (m_CRIMLConfiguration.TrainModel())
                    {
                        // Save model if succesfully trained
                        m_CRIMLConfiguration.SaveModelToDisk();
                    }
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
                    TooltipText = m_CRIMLConfiguration.tips.BodyTooltip.Tips[1];
                } else
                {
                    TooltipText = m_CRIMLConfiguration.tips.BodyTooltip.Error[0];
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
            if (m_CRIMLConfiguration.Model != null)
            {
                string nameButton = "";

                if (m_CRIMLConfiguration.Running)
                {
                    nameButton = "STOP";
                }
                else
                {
                    nameButton = "RUN";
                }

                // Disable button if model is Trainig OR Untrained
                if (m_CRIMLConfiguration.Training || m_CRIMLConfiguration.Untrained)
                {
                    GUI.enabled = false;
                    enabled = false;
                } else
                {
                    enabled = true;
                }
                    
                if (GUILayout.Button(nameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Run")))
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
                    TooltipText = m_CRIMLConfiguration.tips.BodyTooltip.Tips[2];
                }
                else
                {
                    TooltipText = m_CRIMLConfiguration.tips.BodyTooltip.Error[1];
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

