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

        #endregion
        #region XNode messages
        public override void OnHeaderGUI()
        {
            baseNodeBodyHeight = 360;
            // Get reference to the current node
            m_TrainingExamplesNode = (target as TrainingExamplesNode);
            NodeName = "TEACH THE MACHINE ";
            if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
            {
                NodeSubtitle = "Classification and Regression trainging examples";
            }
            else
            {
                NodeSubtitle = "DTW training examples";
            }
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            OutputPortsNamesOverride = new Dictionary<string, string>();
            OutputPortsNamesOverride.Add("TrainingExamplesNodeToOutput", "Recorded\nExamples");

            InputPortsNamesOverride = new Dictionary<string, string>();
            InputPortsNamesOverride.Add("InputFeatures", "Live Data In");
            InputPortsNamesOverride.Add("TargetValues", "Target Values");

            base.nodeTips = m_TrainingExamplesNode.tooltips;
            if (m_TrainingExamplesNode.DesiredInputFeatures.Count != m_ConnectedInputs || m_ConnectedTargets != m_TrainingExamplesNode.DesiredOutputFeatures.Count || lastShowWarning != m_TrainingExamplesNode.showWarning)
                m_RecalculateRects = true;
            m_ConnectedInputs = m_TrainingExamplesNode.DesiredInputFeatures.Count;
            m_ConnectedTargets = m_TrainingExamplesNode.DesiredOutputFeatures.Count;
            lastShowWarning = m_TrainingExamplesNode.showWarning;
            base.OnBodyGUI();
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
                // height is the base node height plus the number of inputs/targets 
                m_BodyRect.height = baseNodeBodyHeight + ((m_ConnectedInputs + m_ConnectedTargets) * 80);
                // if showing warning increase height 
                if (m_TrainingExamplesNode.showWarning)
                {
                    m_BodyRect.height += 60;
                }
                nodeSpace = m_BodyRect.height + 50;
            }
        }
        /// <summary>
        /// GUI elements that show up in the body of the node
        /// </summary>
        protected override void ShowBodyFields()
        {
            GUILayout.BeginArea(m_BodyRect);
            GUILayout.Space(bodySpace);
            DrawValues(m_TrainingExamplesNode.DesiredInputFeatures, "Input Values");
            GUILayout.Space(bodySpace);
            DrawValues(m_TrainingExamplesNode.DesiredOutputFeatures, "Target Values");
            ShowButtons();
            //show warning if there are training examples 
            if (m_TrainingExamplesNode.showWarning)
            {
                if (m_TrainingExamplesNode.tooltips != null && m_TrainingExamplesNode.tooltips.BottomError.Length > 0)
                    ShowWarning(m_TrainingExamplesNode.tooltips.BottomError[0]);
                m_RecalculateRects = true;
            }
            GUILayout.EndArea();
            ShowTrainingExamplesDropdown();
        }

        protected string ShowRecordExamplesButton()
        {
            string nameButton = "";

            //// Only run button logic when there are features to extract from

            if (m_TrainingExamplesNode.CollectingData)
            {
                nameButton = "stop \n recording";
            }
            else
            {
                nameButton = "start \n recording";
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

            if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.TrainingExamplesVector))
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
                    IMLEventDispatcher.DeleteAllCallback(m_TrainingExamplesNode.id);               
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
            int spacing = 75;
            GUILayout.Space(40);

            // show record ONE example button
            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing);
            //if it is a single training examples node 
            if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
            {
                if (GUILayout.Button(new GUIContent("Record One \n example"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record One Button")))
                {
                    IMLEventDispatcher.RecordOneCallback?.Invoke(m_TrainingExamplesNode.id);
                }
                //button tooltip code 
                if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && m_TrainingExamplesNode.tooltips.BodyTooltip.Tips.Length > 2)
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
                // show record examples button
                GUILayout.Space(spacing);
            }
            
            string recordNameButton = ShowRecordExamplesButton();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing - 10);

            if(m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
            {
                GUILayout.Label("record one \nexample", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green"));
                GUILayout.Label("");
            }
            

            GUILayout.Label(recordNameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green"));
            GUILayout.Space(spacing - 10);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing);
            ShowClearAllExamplesButton();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing - 10);
            GUILayout.Label("delete all \n recordings", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button Pink"));
            GUILayout.Label("");
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing - 10);
            if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
            {
                GUILayout.Label("No of training pairs: " + m_TrainingExamplesNode.TrainingExamplesVector.Count, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small"));
            }
            else{
                GUILayout.Label("No of training examples: " + m_TrainingExamplesNode.TrainingSeriesCollection.Count, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small"));
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
                GUI.DrawTexture(m_Dropdown, NodeColor);

                GUILayout.BeginArea(m_Dropdown);

                EditorGUI.indentLevel++;


                if (ReusableMethods.Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.TrainingExamplesVector)&& ReusableMethods.Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.TrainingSeriesCollection))
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
    }

}
