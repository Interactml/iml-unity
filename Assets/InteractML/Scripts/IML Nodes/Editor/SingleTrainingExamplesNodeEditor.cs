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
        /// Bool for add/remove output
        /// </summary>
        private bool m_AddOutput;
        private bool m_RemoveOutput;

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
            GUILayout.Space(50);

            DrawBodyLayoutInputs(m_SingleTrainingExamplesNode.DesiredInputsConfig.Count);
            DrawValues(m_SingleTrainingExamplesNode.DesiredInputFeatures, "Input Values");

            DrawBodyLayoutTargets(m_SingleTrainingExamplesNode.DesiredOutputFeatures.Count);
            GUILayout.Space(80);
            DrawValues(m_SingleTrainingExamplesNode.DesiredOutputFeatures, "Target Values");

            DrawBodyLayoutButtons();
            ShowButtons();

            DrawBodyLayoutBottom();
            ShowHelpButton(m_BodyRectBottom);

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
        private void ShowButtons()
        {
            m_BodyRectButtons.x = m_BodyRectButtons.x + 30;
            m_BodyRectButtons.y = m_BodyRectButtons.y + 20;
            m_BodyRectButtons.width = m_BodyRectButtons.width - 70;

            GUILayout.BeginArea(m_BodyRectButtons);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Load Data", "This is a tooltip"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Load Button")))
            {
                m_SingleTrainingExamplesNode.LoadDataFromDisk();
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
            if (m_SingleTrainingExamplesNode.CollectingData)
            {
                nameButton = "    STOP          ";
            }
            else
            {
                nameButton = "Record Data   ";
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

    }
}
