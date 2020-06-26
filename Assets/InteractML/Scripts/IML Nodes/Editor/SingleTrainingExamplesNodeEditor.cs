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
    [CustomNodeEditor(typeof(SingleTrainingExamplesNode))]
    public class SingleTrainingExamplesNodeEditor : TrainingExamplesNodeEditor
    {

        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private SingleTrainingExamplesNode m_SingleTrainingExamplesNode;



        /// <summary>
        /// Bool for buttontooltips output
        /// </summary>
        private bool buttonTip;
        private bool buttonTipHelper;

        private static GUIStyle editorLabelStyle;

        #endregion

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_SingleTrainingExamplesNode = (target as SingleTrainingExamplesNode);

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
            GUILayout.Label("Classification and Regression", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small"), GUILayout.MinWidth(NodeWidth - 10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));


        }

        public override void OnBodyGUI()
        {
            DrawPortLayout();
            ShowSingleTrainingExamplesNodePorts();
            PortTooltip(m_SingleTrainingExamplesNode.TrainingTips.PortTooltip);
            GUILayout.Space(50);

            DrawBodyLayoutInputs(m_SingleTrainingExamplesNode.DesiredInputFeatures.Count);
            DrawValues(m_SingleTrainingExamplesNode.DesiredInputFeatures, "Input Values");

            DrawBodyLayoutTargets(m_SingleTrainingExamplesNode.DesiredOutputFeatures.Count);
            GUILayout.Space(80);
            DrawValues(m_SingleTrainingExamplesNode.DesiredOutputFeatures, "Target Values");

            DrawBodyLayoutButtons();
            ShowButtons();

            DrawHelpButtonLayout(m_BodyRectButtons.y + m_BodyRectButtons.height);
            ShowTrainingExamplesDropdown();
            ShowHelpButton(m_HelpRect);

            if (showHelp)
            {
                ShowHelptip(m_HelpRect, m_SingleTrainingExamplesNode.TrainingTips.HelpTooltip);
            }
            if (showPort)
            {
                ShowTooltip(m_PortRect, TooltipText);
            }
            if (IsThisRectHovered(m_BodyRectInputs))
                ShowTooltip(m_BodyRectInputs, m_SingleTrainingExamplesNode.TrainingTips.BodyTooltip.Tips[0]);

            if (IsThisRectHovered(m_BodyRectTargets))
                ShowTooltip(m_BodyRectTargets, m_SingleTrainingExamplesNode.TrainingTips.BodyTooltip.Tips[1]);

            if (buttonTip)
            {
                ShowTooltip(m_BodyRectButtons, TooltipText);
            }

        }


        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowSingleTrainingExamplesNodePorts()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Live Data In");
            IMLNodeEditor.PortField(inputPortLabel, m_SingleTrainingExamplesNode.GetInputPort("InputFeatures"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Recorded\nData Out");
            IMLNodeEditor.PortField(outputPortLabel, m_SingleTrainingExamplesNode.GetOutputPort("TrainingExamplesNodeToOutput"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();

            GUIContent secondInputPortLabel = new GUIContent("Target Values");
            IMLNodeEditor.PortField(secondInputPortLabel, m_SingleTrainingExamplesNode.GetInputPort("TargetValues"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

        }


        /// <summary>
        /// Show the load, delete and record buttons
        /// </summary>
        protected override void ShowButtons()
        {
            int spacing = 75;
            GUILayout.BeginArea(m_BodyRectButtons);
            GUILayout.Space(20);
            // show record examples buttons 
            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing);
            if (GUILayout.Button(new GUIContent("Record One \n example"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record One Button")))
            {
                m_SingleTrainingExamplesNode.AddSingleTrainingExample();
            }
            //button tooltip code 
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                TooltipText = m_SingleTrainingExamplesNode.TrainingTips.BodyTooltip.Tips[2];
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

            GUILayout.Space(spacing);
            string recordNameButton = ShowRecordExamplesButton();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing -10);
            GUILayout.Label("record one \nexample", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green"));
            GUILayout.Label("");
            
            GUILayout.Label(recordNameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green"));
            GUILayout.Space(spacing-10);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing);
            ShowClearAllExamplesButton();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing-10);
            GUILayout.Label("delete all \n recordings", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button Pink"));
            GUILayout.Label("");
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing-10);
            GUILayout.Label("No of pairs: " + m_SingleTrainingExamplesNode.TrainingExamplesVector.Count, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small"));
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

        }

        private string ShowRecordExamplesButton()
        {
            string nameButton = "";

            //// Only run button logic when there are features to extract from
            //if (!Lists.IsNullOrEmpty(ref m_SeriesTrainingExamplesNode.InputFeatures))
            //{
            if (m_SingleTrainingExamplesNode.CollectingData)
            {
                nameButton = "stop \n recording";
            }
            else
            {
                nameButton = "start \n recording";
            }

            bool disableButton = false;

            // If there are any models connected we check some conditions
           /* if (!Lists.IsNullOrEmpty(ref m_SingleTrainingExamplesNode.IMLConfigurationNodesConnected))
            {
                for (int i = 0; i < m_SingleTrainingExamplesNode.IMLConfigurationNodesConnected.Count; i++)
                {
                    var IMLConfigNodeConnected = m_SingleTrainingExamplesNode.IMLConfigurationNodesConnected[i];
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
                m_SingleTrainingExamplesNode.ToggleCollectExamples();
            }

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                TooltipText = m_SingleTrainingExamplesNode.TrainingTips.BodyTooltip.Tips[3];
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
            // Always enable it back at the end
            GUI.enabled = true;
            return nameButton;
            //}
        }

        private void ShowClearAllExamplesButton()
        {

            bool disableButton = false;

            if (!Lists.IsNullOrEmpty(ref m_SingleTrainingExamplesNode.TrainingExamplesVector))
            {
                disableButton = false;
            }

            // Only run button logic when there are training examples to delete
            if (!disableButton)
            {
                // If there are any models connected we check some conditions
               /* if (!Lists.IsNullOrEmpty(ref m_SingleTrainingExamplesNode.IMLConfigurationNodesConnected))
                {
                    foreach (var IMLConfigNode in m_SingleTrainingExamplesNode.IMLConfigurationNodesConnected)
                    {
                        // Disable button if any of the models is runnning OR collecting data OR training
                        if (IMLConfigNode.Running || IMLConfigNode.Training || m_SingleTrainingExamplesNode.CollectingData)
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
                    m_SingleTrainingExamplesNode.ClearTrainingExamples();
                }
                //tooltipcode
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    buttonTipHelper = true;
                    TooltipText = m_SingleTrainingExamplesNode.TrainingTips.BodyTooltip.Tips[3];
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
                // Always enable it back at the end
                GUI.enabled = true;



            }
            // If there are no training examples to delete we draw a disabled button
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button("Delete Data", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button")))
                {
                    m_SingleTrainingExamplesNode.ToggleCollectExamples();
                }
                GUI.enabled = true;
            }
        }

        /// <summary>
        /// Show single training examples on foldout arrow
        /// </summary>
        protected override void ShowTrainingExamplesDropdown()
        {
            if (m_ShowTrainingDataDropdown)
            {
                m_Dropdown.x = m_HelpRect.x;
                m_Dropdown.y = m_HelpRect.y + m_HelpRect.height;
                m_Dropdown.width = m_HelpRect.width;
                m_Dropdown.height = 200;
                GUI.DrawTexture(m_Dropdown, NodeColor);

                GUILayout.BeginArea(m_Dropdown);

                EditorGUI.indentLevel++;
                

                if (ReusableMethods.Lists.IsNullOrEmpty(ref m_SingleTrainingExamplesNode.TrainingExamplesVector))
                {
                    EditorGUILayout.LabelField("Training Examples List is empty", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("foldoutempty"));
                }
                else
                {
                    // Begins Vertical Scroll
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUILayout.BeginVertical();
                    m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth()-10), GUILayout.Height(GetWidth() - 50));

                    for (int i = 0; i < m_SingleTrainingExamplesNode.TrainingExamplesVector.Count; i++)
                    {
                        EditorGUILayout.LabelField("Training Example " + (i + 1), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                        EditorGUI.indentLevel++;

                        var inputFeatures = m_SingleTrainingExamplesNode.TrainingExamplesVector[i].Inputs;
                        var outputFeatures = m_SingleTrainingExamplesNode.TrainingExamplesVector[i].Outputs;

                        // If the input features are not null...
                        if (inputFeatures != null)
                        {
                            // Draw inputs
                            for (int j = 0; j < inputFeatures.Count; j++)
                            {

                                if (inputFeatures[j].InputData == null)
                                {
                                    EditorGUILayout.LabelField("Inputs are null ");
                                    break;
                                }


                                EditorGUI.indentLevel++;

                                EditorGUILayout.LabelField("Input " + (j + 1) + " (" + inputFeatures[j].InputData.DataType + ")", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                for (int k = 0; k < inputFeatures[j].InputData.Values.Length; k++)
                                {
                                    EditorGUI.indentLevel++;

                                    EditorGUILayout.LabelField(inputFeatures[j].InputData.Values[k].ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                    EditorGUI.indentLevel--;
                                }

                                EditorGUI.indentLevel--;
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Inputs are null ", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                        }

                        // If the output features are not null...
                        if (outputFeatures != null)
                        {
                            // Draw outputs
                            for (int j = 0; j < outputFeatures.Count; j++)
                            {
                                if (outputFeatures[j].OutputData == null)
                                {
                                    EditorGUILayout.LabelField("Outputs are null ", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                                    break;
                                }


                                EditorGUI.indentLevel++;

                                EditorGUILayout.LabelField("Output " + (j + 1) + " (" + outputFeatures[j].OutputData.DataType + ")", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));


                                for (int k = 0; k < outputFeatures[j].OutputData.Values.Length; k++)
                                {

                                    EditorGUI.indentLevel++;

                                    EditorGUILayout.LabelField(outputFeatures[j].OutputData.Values[k].ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                    EditorGUI.indentLevel--;

                                }

                                EditorGUI.indentLevel--;

                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Outputs are null ", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                        }

                        EditorGUI.indentLevel--;

                    }

                    // Ends Vertical Scroll
                    EditorGUI.indentLevel = indentLevel;
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                    
                }

                EditorGUI.indentLevel--;
                GUILayout.EndArea();
            }
            
        }

    }
}
