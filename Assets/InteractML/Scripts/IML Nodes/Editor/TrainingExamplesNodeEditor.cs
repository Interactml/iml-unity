using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableMethods;
using XNode;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeEditor(typeof(TrainingExamplesNode))]
    public class TrainingExamplesNodeEditor : IMLNodeEditor
    {
        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        protected TrainingExamplesNode m_TrainingExamplesNode;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        protected Rect m_BodyRectInputs;
        protected Rect m_BodyRectTargets;
        protected Rect m_BodyRectButtons;
        protected Rect m_BodyRectBottom;
        protected Rect m_Dropdown;
        /// <summary>
        /// Boolean that shows or hides training data
        /// </summary>
        protected bool m_ShowTrainingDataDropdown;
        /// <summary>
        /// Position of scroll for dropdown
        /// </summary>
        protected Vector2 m_ScrollPos;
        /// <summary>
        /// holds the base height of the node for recalculating height
        /// </summary>
        protected float baseNodeBodyHeight;
        /// <summary>
        /// Holds whether show warning was changed in the last frame
        /// </summary>
        protected bool lastShowWarning = false;

        /// <summary>
        /// The label to show on the button port labels
        /// </summary>
        protected GUIContent m_ButtonPortLabel;
        /// <summary>
        /// NodePort for button. Loaded in OnHeaderHUI()
        /// </summary>
        protected NodePort m_ButtonPortRecordOneInput;
        /// <summary>
        /// NodePort for button. Loaded in OnHeaderHUI()
        /// </summary>
        protected NodePort m_ButtonPortToggleRecord;
        /// <summary>
        /// NodePort for button. Loaded in OnHeaderHUI()
        /// </summary>
        protected NodePort m_ButtonPortDeleteExamples;
        /// <summary>
        /// Used to specify subfolder where to save/load data. Loaded in OnHeaderHUI()
        /// </summary>
        protected NodePort m_PortSubFolderDataPath;

        #endregion
        #region XNode messages
        public override void OnHeaderGUI()
        {
            baseNodeBodyHeight = 250;
            // Get reference to the current node
            m_TrainingExamplesNode = (target as TrainingExamplesNode);
            NodeName = "TEACH THE MACHINE " + m_TrainingExamplesNode.listNo;
            if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
            {
                NodeSubtitle = "Classification and Regression trainging examples";
            }
            else
            {
                NodeSubtitle = "DTW training examples";
            }

            // Create inputport button label
            if (m_ButtonPortLabel == null)
                m_ButtonPortLabel = new GUIContent("");

            // Get button ports
            if (m_ButtonPortRecordOneInput == null)
                m_ButtonPortRecordOneInput = m_TrainingExamplesNode.GetPort("RecordOneInputBoolPort");
            if (m_ButtonPortToggleRecord == null)
                m_ButtonPortToggleRecord = m_TrainingExamplesNode.GetPort("ToggleRecordingInputBoolPort");
            if (m_ButtonPortDeleteExamples == null)
                m_ButtonPortDeleteExamples = m_TrainingExamplesNode.GetPort("DeleteAllExamplesBoolPort");
            // Get subfolderdatapath port
            if (m_PortSubFolderDataPath == null)
                m_PortSubFolderDataPath = m_TrainingExamplesNode.GetPort("SubFolderDataPathStringPort");


            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            IMLGraph graph = this.target.graph as IMLGraph;
            if (graph.IsGraphRunning)
            {
                OutputPortsNamesOverride = new Dictionary<string, string>();
                OutputPortsNamesOverride.Add("TrainingExamplesNodeToOutput", "Recorded\nExamples");

                InputPortsNamesOverride = new Dictionary<string, string>();
                InputPortsNamesOverride.Add("InputFeatures", "Live Data In");
                InputPortsNamesOverride.Add("TargetValues", "Labels");

                base.nodeTips = m_TrainingExamplesNode.tooltips;
                if (m_TrainingExamplesNode.DesiredInputFeatures.Count != m_ConnectedInputs || m_ConnectedTargets != m_TrainingExamplesNode.DesiredOutputFeatures.Count || lastShowWarning != m_TrainingExamplesNode.showWarning)
                    m_RecalculateRects = true;
                m_ConnectedInputs = m_TrainingExamplesNode.DesiredInputFeatures.Count;
                m_ConnectedTargets = m_TrainingExamplesNode.DesiredOutputFeatures.Count;
                lastShowWarning = m_TrainingExamplesNode.showWarning;
                base.OnBodyGUI();
            }
            
        }

        #endregion
        /// <summary>
        /// Initialise body layout 
        /// </summary>
        protected override void InitBodyLayout()
        {
            if (m_RecalculateRects)
            {
                m_BodyRect.x = 5;
                m_BodyRect.y = HeaderRect.height + m_PortRect.height;
                m_BodyRect.width = NodeWidth - 10;
                // height is the base node height plus the number of inputs/targets + extra offset after moving buttons with nodeports out of begin area
                //m_BodyRect.height = baseNodeBodyHeight + ((m_ConnectedInputs + m_ConnectedTargets) * 20) + 225;
                m_BodyRect.height = baseNodeBodyHeight - 30;
                // if showing warning increase height 
                if (m_TrainingExamplesNode.showWarning)
                {
                    m_BodyRect.height += 90;
                }
                // NodeSpace makes the node longer/shorter if there is extra space needed or lacking at the end of the node
                //nodespace = m_BodyRect.height * 0.4f;
                nodeSpace = 65;
            }
        }
        /// <summary>
        /// GUI elements that show up in the body of the node
        /// </summary>
        protected override void ShowBodyFields()
        {
            GUILayout.Space(bodySpace);

            // ShowButtons needs to be called outside of the BeginArea in order for the button nodeports to work
            ShowButtons();

            // commented out for ual students
            // Moved the draw values out of begin area to draw them after showbuttons and not on top
            /*GUILayout.Space(bodySpace);
            DrawValues(m_TrainingExamplesNode.DesiredInputFeatures, "Input Values");
            GUILayout.Space(bodySpace);
            DrawValues(m_TrainingExamplesNode.DesiredOutputFeatures, "Target Values");*/


            
            //show warning if there are training examples 
            if (m_TrainingExamplesNode.showWarning)
            {
                //UAL 
                GUILayout.Space(25);
                if (m_TrainingExamplesNode.tooltips != null && m_TrainingExamplesNode.tooltips.BottomError != null && m_TrainingExamplesNode.tooltips.BottomError.Length > 0)
                {
                    //Changed for UAL students 
                    nodeSpace = 55;
                    ShowWarning(m_TrainingExamplesNode.tooltips.BottomError[0]);
                }
                    
                m_RecalculateRects = true;
            }
            
            // Added an option to specify a subfolder to save/load the data
            // commented out for UAL students 
            //ShowSubFolderDataField();

            ShowTrainingExamplesDropdown();
        }

        protected string ShowRecordExamplesButton()
        {
            string nameButton = "";

            //// Only run button logic when there are features to extract from

            if (m_TrainingExamplesNode.CollectingData)
            {
                nameButton = "stop recording";
            }
            else
            {
                nameButton = "start recording";
            }

            if (!m_TrainingExamplesNode.canCollect)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Record Data", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button")))
            {
                IMLEventDispatcher.ToggleRecordCallback(m_TrainingExamplesNode.id);
            }

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                //TooltipText = m_TrainingExamplesNode.TrainingTips.BodyTooltip.Tips[3];
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
        }

        /// <summary>
        /// Set style for dropdown for training examples nodes
        /// </summary>
        protected void SetDropdownStyle()
        {
            GUI.skin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");
            GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
            Color myStyleColor = Color.white;
            myFoldoutStyle.fontStyle = FontStyle.Bold;
            myFoldoutStyle.normal.textColor = myStyleColor;
            myFoldoutStyle.onNormal.textColor = myStyleColor;
            myFoldoutStyle.hover.textColor = myStyleColor;
            myFoldoutStyle.onHover.textColor = myStyleColor;
            myFoldoutStyle.focused.textColor = myStyleColor;
            myFoldoutStyle.onFocused.textColor = myStyleColor;
            myFoldoutStyle.active.textColor = myStyleColor;
            myFoldoutStyle.onActive.textColor = myStyleColor;
            myFoldoutStyle.fontStyle = FontStyle.Bold;
            myFoldoutStyle.normal.textColor = myStyleColor;
            myFoldoutStyle.fontStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview").fontStyle;
            m_ShowTrainingDataDropdown = EditorGUILayout.Foldout(m_ShowTrainingDataDropdown, "View Training Pairs", myFoldoutStyle);
        }

        protected void ShowClearAllExamplesButton()
        {

            bool disableButton = false;

            if (m_TrainingExamplesNode.TrainingExamplesVector.Count > 0)
            {
                disableButton = false;
            }

            // Only run button logic when there are training examples to delete
            if (!disableButton)
            {

                // Draw button
                if (disableButton)
                    GUI.enabled = false;
                if (GUILayout.Button("Delete Data", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button")))
                {
                    IMLEventDispatcher.DeleteAllExamplesInNodeCallback(m_TrainingExamplesNode.id);               
                }
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    buttonTipHelper = true;
                    //TooltipText = m_SeriesTrainingExamplesNode.toooltips.BodyTooltip.Tips[3];
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
            }
        }

        // <summary>
        /// Draws help button and tells whether mouse is over the tooltip
        /// </summary>
        public override void ShowHelpButton(Rect m_HelpRect)
        {
            // Load node skin
            if (m_NodeSkin == null)
                m_NodeSkin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

            m_HelpRect.x = m_HelpRect.x + 20;
            m_HelpRect.y = m_HelpRect.y + 10;
            m_HelpRect.width = m_HelpRect.width - 40;


            GUILayout.BeginArea(m_HelpRect);
            GUILayout.BeginHorizontal();
            SetDropdownStyle();
            EditorStyles.foldout.fontStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview").fontStyle;

            if (GUILayout.Button(new GUIContent("Help"), m_NodeSkin.GetStyle("Help Button")))
            {
                if (showHelp)
                {
                    showHelp = false;
                }
                else
                {
                    showHelp = true;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Used in Training examples nodes to draw values of inuts and target values 
        /// </summary>
        /// <param name="values">Either desired input values or desired output values from training examples node </param>
        /// <param name="name">Either target vaue or input values </param>
        protected void DrawValues(List<IMLBaseDataType> values, string name)
        {
            //check there are values
            if (values != null)
            {
                // for each set of values create space in the node and name them 
                for (int i = 0; i < values.Count; i++)
                {
                    if (m_NodeSkin == null)
                        m_NodeSkin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");
                    GUIStyle style = m_NodeSkin.GetStyle("TE Text");
                    var x = 30;
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(x), GUILayout.MaxHeight(x) };
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(bodySpace);
                    GUILayout.TextArea(name + " " + i, style, options);
                    GUILayout.EndHorizontal();
                    // set text area dependent on DataType 
                    switch (values[i].DataType)
                    {
                        case IMLSpecifications.DataTypes.Float:
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(bodySpace);
                            GUILayout.TextArea("float: " + System.Math.Round(values[i].Values[0], 3).ToString(), style, options);
                            GUILayout.EndHorizontal();
                            break;
                        case IMLSpecifications.DataTypes.Integer:
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(bodySpace);
                            GUILayout.TextArea("int: " + System.Math.Round(values[i].Values[0], 3).ToString(), style, options);
                            GUILayout.EndHorizontal();
                            break;
                        case IMLSpecifications.DataTypes.Vector2:
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(bodySpace);
                            GUILayout.TextArea("x: " + System.Math.Round(values[i].Values[0], 3).ToString(), style, options);
                            GUILayout.TextArea("y: " + System.Math.Round(values[i].Values[1], 3).ToString(), style, options);
                            GUILayout.EndHorizontal();
                            break;
                        case IMLSpecifications.DataTypes.Vector3:
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(bodySpace);
                            GUILayout.TextArea("x: " + System.Math.Round(values[i].Values[0], 3).ToString(), style, options);
                            GUILayout.TextArea("y: " + System.Math.Round(values[i].Values[1], 3).ToString(), style, options);
                            GUILayout.TextArea("z: " + System.Math.Round(values[i].Values[2], 3).ToString(), style, options);
                            GUILayout.EndHorizontal();
                            break;
                        case IMLSpecifications.DataTypes.Vector4:
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(bodySpace);
                            GUILayout.TextArea("x: " + System.Math.Round(values[i].Values[0], 3).ToString(), style, options);
                            GUILayout.TextArea("y: " + System.Math.Round(values[i].Values[1], 3).ToString(), style, options);
                            GUILayout.TextArea("z: " + System.Math.Round(values[i].Values[2], 3).ToString(), style, options);
                            GUILayout.TextArea("w: " + System.Math.Round(values[i].Values[2], 3).ToString(), style, options);
                            GUILayout.EndHorizontal();
                            break;
                        // need to implement for serial vector 
                        default:
                            break;
                    }
                }
            }
            EditorGUILayout.Space();

        }

        private void ShowButtons()
        {
            int offset = 1;
            GUILayout.Space(20);

            // show record ONE example button

            //if it is a single training examples node 
            if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
            {

                GUILayout.BeginHorizontal();
                // Draw port                
                IMLNodeEditor.PortField(m_ButtonPortLabel, m_ButtonPortRecordOneInput, m_NodeSkin.GetStyle("Port Label"), GUILayout.MaxWidth(10));
                GUILayout.Space(offset);
                if (GUILayout.Button(new GUIContent(""), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record One Button")))
                {
                    IMLEventDispatcher.RecordOneCallback?.Invoke(m_TrainingExamplesNode.id);
                }
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.Label("record one example", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green"));
                GUILayout.Label("");
                GUILayout.EndHorizontal();
                
                //button tooltip code 
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)
                    && m_TrainingExamplesNode.tooltips != null
                    && m_TrainingExamplesNode.tooltips.BodyTooltip != null
                    && m_TrainingExamplesNode.tooltips.BodyTooltip.Tips != null
                    && m_TrainingExamplesNode.tooltips.BodyTooltip.Tips.Length > 2)
                {
                    buttonTipHelper = true;
                    TooltipText = m_TrainingExamplesNode.tooltips.BodyTooltip.Tips[2];
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

                GUILayout.EndHorizontal();
            }

            // show record examples button
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            // Draw port
            IMLNodeEditor.PortField(m_ButtonPortLabel, m_ButtonPortToggleRecord, m_NodeSkin.GetStyle("Port Label"), GUILayout.MaxWidth(10));
            GUILayout.Space(offset);

            // draw record button
            string recordNameButton = ShowRecordExamplesButton();
            GUILayout.Space(5);
            // draw record label
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(recordNameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green"));
            GUILayout.Label("");
            GUILayout.EndHorizontal();


            GUILayout.EndHorizontal();

            // show delete all button
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            //GUILayout.Space(spacing);
            // Draw port
            IMLNodeEditor.PortField(m_ButtonPortLabel, m_ButtonPortDeleteExamples, m_NodeSkin.GetStyle("Port Label"), GUILayout.MaxWidth(10));
            GUILayout.Space(offset);
            // draw delete all button

            ShowClearAllExamplesButton();
            
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("delete all recordings", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button Pink"));
            GUILayout.Label("");
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
            {
                GUILayout.Label("Number of training pairs: " + m_TrainingExamplesNode.TrainingExamplesVector.Count, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small"));
            }
            else{
                GUILayout.Label("Number of training examples: " + m_TrainingExamplesNode.TrainingSeriesCollection.Count, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small"));
            }
            
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Show single training examples on foldout arrow
        /// </summary>
        private void ShowTrainingExamplesDropdown()
        {
            if (m_ShowTrainingDataDropdown)
            {
                m_Dropdown.x = m_HelpRect.x;
                m_Dropdown.y = m_HelpRect.y + m_HelpRect.height;
                m_Dropdown.width = m_HelpRect.width;
                m_Dropdown.height = 200;
                if (Event.current.type == EventType.Layout)
                {
                    GUI.DrawTexture(m_Dropdown, NodeColor);
                }
                    

                GUILayout.BeginArea(m_Dropdown);

                EditorGUI.indentLevel++;


                if (m_TrainingExamplesNode.TrainingExamplesVector.Count > 0 && m_TrainingExamplesNode.TrainingSeriesCollection.Count > 0)
                {
                    EditorGUILayout.LabelField("Training Examples List is empty", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("foldoutempty"));
                }
                else
                {
                    // Begins Vertical Scroll
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUILayout.BeginVertical();

                    if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
                    {
                        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 10), GUILayout.Height(GetWidth() - 50));

                        for (int i = 0; i < m_TrainingExamplesNode.TrainingExamplesVector.Count; i++)
                        {
                            EditorGUILayout.LabelField("Training Example " + (i + 1), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                            EditorGUI.indentLevel++;

                            var inputFeatures = m_TrainingExamplesNode.TrainingExamplesVector[i].Inputs;
                            var outputFeatures = m_TrainingExamplesNode.TrainingExamplesVector[i].Outputs;

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
                    }
                    else
                    {
                        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 25), GUILayout.Height(GetWidth() - 100));

                        // Go Series by Series
                        for (int i = 0; i < m_TrainingExamplesNode.TrainingSeriesCollection.Count; i++)
                        {
                            EditorGUILayout.LabelField("Training Series " + i, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                            EditorGUI.indentLevel++;

                            var inputFeaturesInSeries = m_TrainingExamplesNode.TrainingSeriesCollection[i].Series;
                            var labelSeries = m_TrainingExamplesNode.TrainingSeriesCollection[i].LabelSeries;

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
                    }

                    EditorGUILayout.EndVertical();

                }

                EditorGUI.indentLevel--;
                GUILayout.EndArea();
            }

        }

        /// <summary>
        /// Shows the subfolder data field to specify and optional name where to save/load data
        /// </summary>
        private void ShowSubFolderDataField()
        {
            GUILayout.BeginHorizontal();
            // Draw port
            IMLNodeEditor.PortField(m_ButtonPortLabel, m_PortSubFolderDataPath, m_NodeSkin.GetStyle("Port Label"), GUILayout.MaxWidth(10));
            GUILayout.Space(15);
            EditorGUILayout.LabelField("SubFolder Data Path");
            m_TrainingExamplesNode.SubFolderDataPath = EditorGUILayout.TextField(m_TrainingExamplesNode.SubFolderDataPath);
            GUILayout.EndHorizontal();
        }
    }

}
