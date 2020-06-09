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
    public class SeriesTrainingExamplesNodeEditor : IMLNodeEditor
    {

        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private SeriesTrainingExamplesNode m_SeriesTrainingExamplesNode;

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

        private static GUIStyle editorLabelStyle;

        bool help = false;
  

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

            DrawBodyLayoutOutputAddRemove();
            ShowOutputAddRemove();

            DrawBodyLayoutOutputs();
            GUILayout.Space(50);
            ShowOutputList();

            DrawBodyLayoutButtons();
            GUILayout.Space(60);
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
            m_BodyRectOutputs.height = 60 + (m_SeriesTrainingExamplesNode.DesiredOutputsConfig.Count * 62);

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
            m_BodyRectButtons.height = 95;

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
                int originalSize = m_SeriesTrainingExamplesNode.DesiredOutputsConfig.Count;
                int newSize = m_SeriesTrainingExamplesNode.DesiredOutputsConfig.Count + 1;
                    //EditorGUILayout.IntField("No. of Outputs", m_TrainingExamplesNode.DesiredOutputsConfig.Count);
                if (originalSize != newSize)
                {
                    m_SeriesTrainingExamplesNode.DesiredOutputsConfig.Resize<IMLSpecifications.OutputsEnum>(newSize);
                }

                m_AddOutput = false;
            }
            if (m_RemoveOutput)
            {
                //remove last item in output list and switch toggle back 
                // is there a way to select which output to delete?
                // Check if we are changing the size of the list 
                if (m_SeriesTrainingExamplesNode.DesiredOutputsConfig.Count > 1)
                {
                    int originalSize = m_SeriesTrainingExamplesNode.DesiredOutputsConfig.Count;
                    int newSize = m_SeriesTrainingExamplesNode.DesiredOutputsConfig.Count - 1;
                    //EditorGUILayout.IntField("No. of Outputs", m_TrainingExamplesNode.DesiredOutputsConfig.Count);
                    if (originalSize != newSize)
                    {
                        m_SeriesTrainingExamplesNode.DesiredOutputsConfig.Resize<IMLSpecifications.OutputsEnum>(newSize);
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
            if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
            EditorStyles.label.normal.textColor = Color.white;
            EditorStyles.label.font = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label").font;

            //Go through the list of outputs and show the correct kind of config editor tool
            for (int i = 0; i < m_SeriesTrainingExamplesNode.DesiredOutputsConfig.Count; i++)
            {
                
                int outputValueIndex = i + 1;
                string label = "Output " + outputValueIndex;
                if (m_SeriesTrainingExamplesNode.DesiredOutputsConfig.Count > 0 && i < m_SeriesTrainingExamplesNode.DesiredOutputsConfig.Count)
                {
                    var inputFeature = m_SeriesTrainingExamplesNode.DesiredOutputsConfig[i];
                    // We make sure that the desired output feature list captures the value inputted by the user
                    m_SeriesTrainingExamplesNode.DesiredOutputsConfig[i] = (IMLSpecifications.OutputsEnum)EditorGUILayout.EnumPopup(label, m_SeriesTrainingExamplesNode.DesiredOutputsConfig[i]);
                }
                if (m_SeriesTrainingExamplesNode.DesiredOutputFeatures.Count > 0 && i < m_SeriesTrainingExamplesNode.DesiredOutputFeatures.Count)
                {
                    string labelOutput = "Value " + outputValueIndex + ":";
                    var outputFeature = m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i];
                    // We make sure that the desired output feature list captures the value inputted by the user
                    switch (outputFeature.DataType)
                    {
                        case (IMLSpecifications.DataTypes)IMLSpecifications.OutputsEnum.Float:
                            (m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i] as IMLFloat).SetValue(EditorGUILayout.FloatField(labelOutput, m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i].Values[0]));
                            GUILayout.Space(15);
                            break;
                        case (IMLSpecifications.DataTypes)IMLSpecifications.OutputsEnum.Integer:
                            (m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i] as IMLInteger).SetValue(EditorGUILayout.IntField(labelOutput, (int)m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i].Values[0]));
                            GUILayout.Space(15);
                            break;
                        case (IMLSpecifications.DataTypes)IMLSpecifications.OutputsEnum.Vector2:
                            var vector2ToShow = new Vector2(m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i].Values[0],
                                m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i].Values[1]);
                            var valueVector2 = EditorGUILayout.Vector2Field(labelOutput, vector2ToShow);
                            (m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i] as IMLVector2).SetValues(valueVector2);
                            break;
                        case (IMLSpecifications.DataTypes)IMLSpecifications.OutputsEnum.Vector3:
                            var vector3ToShow = new Vector3(m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i].Values[0],
                                m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i].Values[1],
                                m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i].Values[2]);
                            var valueVector3 = EditorGUILayout.Vector3Field(labelOutput, vector3ToShow);
                            (m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i] as IMLVector3).SetValues(valueVector3);
                            break;
                        case (IMLSpecifications.DataTypes)IMLSpecifications.OutputsEnum.Vector4:
                            var vector4ToShow = new Vector4(m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i].Values[0],
                                m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i].Values[1],
                                m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i].Values[2],
                                m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i].Values[3]);
                            var valueVector4 = EditorGUILayout.Vector4Field(labelOutput, vector4ToShow);
                            (m_SeriesTrainingExamplesNode.DesiredOutputFeatures[i] as IMLVector4).SetValues(valueVector4);
                            break;
                        default:
                            break;
                    }
                    
                }
                GUILayout.Space(10);

            }
            EditorStyles.label.normal = editorLabelStyle.normal;
            EditorStyles.label.font = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Label").font;
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
                m_SeriesTrainingExamplesNode.LoadDataFromDisk();
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
            if (m_SeriesTrainingExamplesNode.CollectingData)
            {
                nameButton = "       STOP          ";
            }
            else
            {
                nameButton = "Record Series   ";
            }

            bool disableButton = false;

            // If there are any models connected we check some conditions
            if (!Lists.IsNullOrEmpty(ref m_SeriesTrainingExamplesNode.IMLConfigurationNodesConnected))
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
            }

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
                if (!Lists.IsNullOrEmpty(ref m_SeriesTrainingExamplesNode.IMLConfigurationNodesConnected))
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
                }

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

        private void ShowBottomSection()
        {
            m_BodyRectBottom.x = m_BodyRectBottom.x + 20;
            m_BodyRectBottom.y = m_BodyRectBottom.y + 10;
            m_BodyRectBottom.width = m_BodyRectBottom.width - 40;
            m_BodyRectBottom.height = 5000;

            GUILayout.BeginArea(m_BodyRectBottom);
            GUILayout.BeginHorizontal();
            GUILayout.Label("advanced options", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"));
            GUILayout.Label("");
            HelpButton(this.GetType().ToString());
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        

    }
}
