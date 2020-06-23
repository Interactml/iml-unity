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
    [CustomNodeEditor(typeof(SeriesTrainingExamplesNode))]
    public class SeriesTrainingExamplesNodeEditor : TrainingExamplesNodeEditor
    {

        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private SeriesTrainingExamplesNode m_SeriesTrainingExamplesNode;


        /// <summary>
        /// Bool for add/remove output
        /// </summary>
        private bool m_AddOutput;
        private bool m_RemoveOutput;

        private static GUIStyle editorLabelStyle;
        #endregion

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_SeriesTrainingExamplesNode = (target as SeriesTrainingExamplesNode);

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
            GUILayout.Label("TEACH THE MACHINE", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header"), GUILayout.MinWidth(NodeWidth-10));
            GUILayout.Label("Dynamic Time Warping", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small"), GUILayout.MinWidth(NodeWidth-10));
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(60));


        }

        public override void OnBodyGUI()
        {
            DrawPortLayout();
            ShowSeriesTrainingExamplesNodePorts();
            GUILayout.Space(50);

            DrawBodyLayoutInputs(m_SeriesTrainingExamplesNode.DesiredInputFeatures.Count);
            DrawValues(m_SeriesTrainingExamplesNode.DesiredInputFeatures, "Input Values");

            DrawBodyLayoutTargets(m_SeriesTrainingExamplesNode.DesiredOutputFeatures.Count);
            GUILayout.Space(80);
            DrawValues(m_SeriesTrainingExamplesNode.DesiredOutputFeatures, "Target Values");

            DrawBodyLayoutButtons();
            ShowButtons();

            DrawHelpButtonLayout(m_BodyRectButtons.y + m_BodyRectButtons.height);
            ShowTrainingExamplesDropdown();
            ShowHelpButton(m_HelpRect);

            if (showHelp)
            {
                ShowHelptip(m_HelpRect, m_SeriesTrainingExamplesNode.TrainingTips.HelpTooltip);
            }

        }

  

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowSeriesTrainingExamplesNodePorts()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Live Data In");
            IMLNodeEditor.PortField(inputPortLabel, m_SeriesTrainingExamplesNode.GetInputPort("InputFeatures"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Recorded\nData Out");
            IMLNodeEditor.PortField(outputPortLabel, m_SeriesTrainingExamplesNode.GetOutputPort("TrainingExamplesNodeToOutput"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();

            GUIContent secondInputPortLabel = new GUIContent("Target Values");
            IMLNodeEditor.PortField(secondInputPortLabel, m_SeriesTrainingExamplesNode.GetInputPort("TargetValues"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

        }

        /// <summary>
        /// Show the load, delete and record buttons
        /// </summary>
        protected override void ShowButtons()
        {
            int spacing = 70;
            GUILayout.BeginArea(m_BodyRectButtons);
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing);
            string recordNameButton = ShowRecordExamplesButton();
            GUILayout.Space(spacing);
            ShowClearAllExamplesButton();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing-5);
            GUILayout.Label(recordNameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green"));
            GUILayout.Space(spacing-20);
            GUILayout.Label("delete all \n recordings", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button Pink"));
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();


        }

        private string ShowRecordExamplesButton()
        {
            string nameButton = "";

            //// Only run button logic when there are features to extract from
            //if (!Lists.IsNullOrEmpty(ref m_SeriesTrainingExamplesNode.InputFeatures))
            //{
            if (m_SeriesTrainingExamplesNode.CollectingData)
            {
                nameButton = "stop \nrecording";
            }
            else
            {
                nameButton = "start \nrecording";
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
                m_SeriesTrainingExamplesNode.ToggleCollectExamples();
            }
            // Always enable it back at the end
            GUI.enabled = true;
            return nameButton;
            //}
        }

        private void ShowClearAllExamplesButton()
        {
            
            bool disableButton = false;
            
            if (!Lists.IsNullOrEmpty(ref m_SeriesTrainingExamplesNode.TrainingExamplesVector))
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
                    m_SeriesTrainingExamplesNode.ClearTrainingExamples();
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
                    m_SeriesTrainingExamplesNode.ToggleCollectExamples();
                }
                GUI.enabled = true;
            }
        }

        /// <summary>
        /// Shows a dropdown with the training examples series
        /// </summary>
        protected override void ShowTrainingExamplesDropdown()
        {
            if (m_ShowTrainingDataDropdown)
            {
                m_Dropdown.x = m_HelpRect.x;
                m_Dropdown.y = m_HelpRect.y + m_HelpRect.height;
                m_Dropdown.width = m_HelpRect.width;
                m_Dropdown.height = 200;

                GUILayout.BeginArea(m_Dropdown);

                EditorGUI.indentLevel++;
                if (ReusableMethods.Lists.IsNullOrEmpty(ref m_SeriesTrainingExamplesNode.TrainingSeriesCollection))
                {
                    EditorGUILayout.LabelField("Training Series List is empty", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("foldoutempty"));
                }
                else
                {
                    // Begins Vertical Scroll
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUILayout.BeginVertical();
                    m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 25), GUILayout.Height(GetWidth() - 100));

                    // Go Series by Series
                    for (int i = 0; i < m_SeriesTrainingExamplesNode.TrainingSeriesCollection.Count; i++)
                    {
                        EditorGUILayout.LabelField("Training Series " + i, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                        EditorGUI.indentLevel++;

                        var inputFeaturesInSeries = m_SeriesTrainingExamplesNode.TrainingSeriesCollection[i].Series;
                        var labelSeries = m_SeriesTrainingExamplesNode.TrainingSeriesCollection[i].LabelSeries;

                        // If the input features are not null...
                        if (inputFeaturesInSeries != null)
                        {
                            EditorGUILayout.LabelField("No. Examples: " + inputFeaturesInSeries.Count, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                            // Draw inputs
                            for (int j = 0; j < inputFeaturesInSeries.Count; j++)
                            {
                                EditorGUI.indentLevel++;

                                EditorGUILayout.LabelField("Input Feature " + j, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                // Are there any examples in series?
                                if (inputFeaturesInSeries[j] == null)
                                {
                                    EditorGUILayout.LabelField("Inputs are null ", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                                    break;
                                }

                                EditorGUI.indentLevel++;
                                for (int k = 0; k < inputFeaturesInSeries[j].Count; k++)
                                {
                                    EditorGUILayout.LabelField("Input " + k + " (" + inputFeaturesInSeries[j][k].InputData.DataType + ")", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                    for (int w = 0; w < inputFeaturesInSeries[j][k].InputData.Values.Length; w++)
                                    {
                                        EditorGUI.indentLevel++;

                                        EditorGUILayout.LabelField(inputFeaturesInSeries[j][k].InputData.Values[w].ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                        EditorGUI.indentLevel--;
                                    }


                                }
                                EditorGUI.indentLevel--;
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.indentLevel--;

                        }
                        // If the input features are null...
                        else
                        {
                            EditorGUILayout.LabelField("Input Features in series are null", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                        }

                        // If the output features for the entire series are not null...
                        if (labelSeries != null)
                        {
                            // Draw output
                            EditorGUI.indentLevel++;

                            EditorGUILayout.TextArea(labelSeries, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                            //EditorGUILayout.LabelField("TEST");

                            EditorGUI.indentLevel--;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Series Target Values are null ", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
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
