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
    public class SingleTrainingExamplesNodeEditor : IMLNodeEditor
    {

        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private SingleTrainingExamplesNode m_SingleTrainingExamplesNode;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_BodyRectOutputs;
        private Rect m_BodyRectOutputAddRemove;
        private Rect m_BodyRectButtons;
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
            GUILayout.Label("", GUILayout.MinHeight(50));

            DrawBodyLayoutOutputAddRemove();
            ShowOutputAddRemove();

            DrawBodyLayoutOutputs();
            GUILayout.Label("", GUILayout.MinHeight(50));
            ShowOutputList();

            DrawBodyLayoutButtons();
            GUILayout.Label("", GUILayout.MinHeight(50));
            ShowButtons();

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
            GUI.DrawTexture(new Rect(m_PortRect.x, HeaderRect.height + m_PortRect.height - WeightOfSectionLine, m_PortRect.width, WeightOfSectionLine), GetColorTextureFromHexString("#74DF84"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayoutOutputAddRemove()
        {
            m_BodyRectOutputAddRemove.x = 5;
            m_BodyRectOutputAddRemove.y = HeaderRect.height + m_PortRect.height;
            m_BodyRectOutputAddRemove.width = NodeWidth - 10;
            m_BodyRectOutputAddRemove.height = 60;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_BodyRectOutputAddRemove, NodeColor);

            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_BodyRectOutputAddRemove.x, HeaderRect.height + m_PortRect.height + m_BodyRectOutputAddRemove.height - WeightOfSeparatorLine, m_BodyRectOutputAddRemove.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayoutOutputs()
        {
            m_BodyRectOutputs.x = 5;
            m_BodyRectOutputs.y = HeaderRect.height + m_PortRect.height + m_BodyRectOutputAddRemove.height;
            m_BodyRectOutputs.width = NodeWidth - 10;
            m_BodyRectOutputs.height = 60 + (m_SingleTrainingExamplesNode.DesiredOutputsConfig.Count * 36);

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_BodyRectOutputs, NodeColor);

            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_BodyRectOutputs.x, HeaderRect.height + m_PortRect.height + m_BodyRectOutputAddRemove.height + m_BodyRectOutputs.height - WeightOfSeparatorLine, m_BodyRectOutputs.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayoutButtons()
        {
            m_BodyRectButtons.x = 5;
            m_BodyRectButtons.y = HeaderRect.height + m_PortRect.height + m_BodyRectOutputAddRemove.height + m_BodyRectOutputs.height;
            m_BodyRectButtons.width = NodeWidth - 10;
            m_BodyRectButtons.height = 80;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_BodyRectButtons, NodeColor);

            // Draw line below add/remove buttons
            GUI.DrawTexture(new Rect(m_BodyRectButtons.x, (HeaderRect.height + m_PortRect.height + m_BodyRectOutputAddRemove.height + m_BodyRectOutputs.height + m_BodyRectButtons.height) - WeightOfSeparatorLine, m_BodyRectOutputs.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayoutBottom()
        {
            m_BodyRectBottom.x = 5;
            m_BodyRectBottom.y = HeaderRect.height + m_PortRect.height + m_BodyRectOutputAddRemove.height + m_BodyRectOutputs.height + m_BodyRectButtons.height;
            m_BodyRectBottom.width = NodeWidth - 10;
            m_BodyRectBottom.height = 40;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_BodyRectBottom, NodeColor);
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowSingleTrainingExamplesNodePorts()
        {
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();

            GUIContent inputPortLabel = new GUIContent("Live Data\nIn");
            IMLNodeEditor.PortField(inputPortLabel, m_SingleTrainingExamplesNode.GetInputPort("InputFeatures"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUIContent outputPortLabel = new GUIContent("Recorded\nData Out");
            IMLNodeEditor.PortField(outputPortLabel, m_SingleTrainingExamplesNode.GetOutputPort("TrainingExamplesNodeToOutput"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(0));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show the Output add or remove buttons
        /// </summary>
        private void ShowOutputAddRemove()
        {

            GUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Outputs", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));

            GUILayout.Label("", GUILayout.MinWidth(40));

            m_AddOutput = EditorGUILayout.Toggle(m_AddOutput, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Add Toggle"));
            m_RemoveOutput = EditorGUILayout.Toggle(m_RemoveOutput, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Remove Toggle"));

            if (m_AddOutput)
            {
                // add output slot and switch toggle back
                //m_SeriesTrainingExamplesNode.DesiredOutputFeatures.Add();

                // Check if we are changing the size of the list 
                int originalSize = m_SingleTrainingExamplesNode.DesiredOutputsConfig.Count;
                int newSize = m_SingleTrainingExamplesNode.DesiredOutputsConfig.Count + 1;
                //EditorGUILayout.IntField("No. of Outputs", m_TrainingExamplesNode.DesiredOutputsConfig.Count);
                if (originalSize != newSize)
                {
                    m_SingleTrainingExamplesNode.DesiredOutputsConfig.Resize<IMLSpecifications.OutputsEnum>(newSize);
                }

                m_AddOutput = false;
            }
            if (m_RemoveOutput)
            {
                //remove last item in output list and switch toggle back 
                // is there a way to select which output to delete?
                // Check if we are changing the size of the list 
                if (m_SingleTrainingExamplesNode.DesiredOutputsConfig.Count > 1)
                {
                    int originalSize = m_SingleTrainingExamplesNode.DesiredOutputsConfig.Count;
                    int newSize = m_SingleTrainingExamplesNode.DesiredOutputsConfig.Count - 1;
                    //EditorGUILayout.IntField("No. of Outputs", m_TrainingExamplesNode.DesiredOutputsConfig.Count);
                    if (originalSize != newSize)
                    {
                        m_SingleTrainingExamplesNode.DesiredOutputsConfig.Resize<IMLSpecifications.OutputsEnum>(newSize);
                    }
                }
                m_RemoveOutput = false;
            }
            GUILayout.EndHorizontal();

        }

        /// <summary>
        /// Show the Output list
        /// </summary>
        private void ShowOutputList()
        {
            //Go through the list of outputs and show the correct kind of config editor tool
            for (int i = 0; i < m_SingleTrainingExamplesNode.DesiredOutputsConfig.Count; i++)
            {

                int outputValueIndex = i + 1;
                string label = "Output " + outputValueIndex;
                if (m_SingleTrainingExamplesNode.DesiredOutputsConfig.Count > 0 && i < m_SingleTrainingExamplesNode.DesiredOutputsConfig.Count)
                {
                    var inputFeature = m_SingleTrainingExamplesNode.DesiredOutputsConfig[i];
                    // We make sure that the desired output feature list captures the value inputted by the user
                    m_SingleTrainingExamplesNode.DesiredOutputsConfig[i] = (IMLSpecifications.OutputsEnum)EditorGUILayout.EnumPopup(label, m_SingleTrainingExamplesNode.DesiredOutputsConfig[i]);
                }
                if (m_SingleTrainingExamplesNode.DesiredOutputFeatures.Count > 0 && i < m_SingleTrainingExamplesNode.DesiredOutputFeatures.Count)
                {
                    string labelOutput = "Value " + outputValueIndex + ":";
                    var outputFeature = m_SingleTrainingExamplesNode.DesiredOutputFeatures[i];
                    // We make sure that the desired output feature list captures the value inputted by the user
                    switch (outputFeature.DataType)
                    {
                        case (IMLSpecifications.DataTypes)IMLSpecifications.OutputsEnum.Float:
                            (m_SingleTrainingExamplesNode.DesiredOutputFeatures[i] as IMLFloat).SetValue(EditorGUILayout.FloatField(labelOutput, m_SingleTrainingExamplesNode.DesiredOutputFeatures[i].Values[0]));
                            break;
                        case (IMLSpecifications.DataTypes)IMLSpecifications.OutputsEnum.Integer:
                            (m_SingleTrainingExamplesNode.DesiredOutputFeatures[i] as IMLInteger).SetValue(EditorGUILayout.IntField(labelOutput, (int)m_SingleTrainingExamplesNode.DesiredOutputFeatures[i].Values[0]));
                            break;
                        case (IMLSpecifications.DataTypes)IMLSpecifications.OutputsEnum.Vector2:
                            var vector2ToShow = new Vector2(m_SingleTrainingExamplesNode.DesiredOutputFeatures[i].Values[0],
                                m_SingleTrainingExamplesNode.DesiredOutputFeatures[i].Values[1]);
                            var valueVector2 = EditorGUILayout.Vector2Field(labelOutput, vector2ToShow);
                            (m_SingleTrainingExamplesNode.DesiredOutputFeatures[i] as IMLVector2).SetValues(valueVector2);
                            break;
                        case (IMLSpecifications.DataTypes)IMLSpecifications.OutputsEnum.Vector3:
                            var vector3ToShow = new Vector3(m_SingleTrainingExamplesNode.DesiredOutputFeatures[i].Values[0],
                                m_SingleTrainingExamplesNode.DesiredOutputFeatures[i].Values[1],
                                m_SingleTrainingExamplesNode.DesiredOutputFeatures[i].Values[2]);
                            var valueVector3 = EditorGUILayout.Vector3Field(labelOutput, vector3ToShow);
                            (m_SingleTrainingExamplesNode.DesiredOutputFeatures[i] as IMLVector3).SetValues(valueVector3);
                            break;
                        case (IMLSpecifications.DataTypes)IMLSpecifications.OutputsEnum.Vector4:
                            var vector4ToShow = new Vector4(m_SingleTrainingExamplesNode.DesiredOutputFeatures[i].Values[0],
                                m_SingleTrainingExamplesNode.DesiredOutputFeatures[i].Values[1],
                                m_SingleTrainingExamplesNode.DesiredOutputFeatures[i].Values[2],
                                m_SingleTrainingExamplesNode.DesiredOutputFeatures[i].Values[3]);
                            var valueVector4 = EditorGUILayout.Vector4Field(labelOutput, vector4ToShow);
                            (m_SingleTrainingExamplesNode.DesiredOutputFeatures[i] as IMLVector4).SetValues(valueVector4);
                            break;
                        default:
                            break;
                    }

                }

            }
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
                m_SingleTrainingExamplesNode.LoadDataFromDisk();
            }

            GUILayout.Label("");
            ShowClearAllExamplesButton();
            GUILayout.Label("");
            string recordNameButton = ShowRecordExamplesButton();

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.Label("", GUILayout.MinHeight(20));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Load Data", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Load Button Yellow"));
            GUILayout.Label("");
            GUILayout.Label("Delete Data", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button Pink"));
            GUILayout.Label("");
            GUILayout.Label(recordNameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green"));
            GUILayout.EndHorizontal();
        }

        private string ShowRecordExamplesButton()
        {
            string nameButton = "";

            //// Only run button logic when there are features to extract from
            //if (!Lists.IsNullOrEmpty(ref m_SeriesTrainingExamplesNode.InputFeatures))
            //{
            if (m_SingleTrainingExamplesNode.CollectingData)
            {
                nameButton = "STOP";
            }
            else
            {
                nameButton = "Record Data";
            }

            bool disableButton = false;

            // If there are any models connected we check some conditions
            if (!Lists.IsNullOrEmpty(ref m_SingleTrainingExamplesNode.IMLConfigurationNodesConnected))
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
            }

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
                if (!Lists.IsNullOrEmpty(ref m_SingleTrainingExamplesNode.IMLConfigurationNodesConnected))
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
                }

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
