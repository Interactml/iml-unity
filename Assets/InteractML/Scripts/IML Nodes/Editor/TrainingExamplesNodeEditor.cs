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
        /// <summary>
        /// Boolean that shows or hides training data
        /// </summary>
        protected bool m_ShowTrainingDataDropdown;
        /// <summary>
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl0;
        /// <summary>
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl1;
        /// <summary>
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl2;
        /// <summary>
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl3;
        /// <summary>
        /// Rect for dropdown layout
        /// </summary>
        protected Rect m_Dropdown;
        /// <summary>
        /// Position of scroll for dropdown
        /// </summary>
        protected Vector2 m_ScrollPos;
        
        /// <summary>
        /// Boolean that shows or hides training UniqueClasses
        /// </summary>
        protected bool m_ShowUniqueClassesDropdown;
        /// <summary>
        /// List of dropdowns per UniqueClasses entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_UniqueClassesDropdownsLvl0;
        /// <summary>
        /// List of dropdowns per UniqueClasses entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_UniqueClassesDropdownsLvl1;
        /// <summary>
        /// List of dropdowns per UniqueClasses entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_UniqueClassesDropdownsLvl2;
        /// <summary>
        /// List of dropdowns per UniqueClasses entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_UniqueClassesDropdownsLvl3;
        /// <summary>
        /// Rect for unique classes dropdown layout
        /// </summary>
        protected Rect m_UniqueClassesDropdown;
        /// <summary>
        /// Position of scroll for unique classes dropdown
        /// </summary>
        protected Vector2 m_UniqueClassesScrollPos;

        // Styles
        /// <summary>
        /// Style of foldout
        /// </summary>
        GUIStyle m_FoldoutStyle;
        GUIStyle m_FoldoutEmptyStyle;
        GUIStyle m_ScrollViewStyle;
        GUIStyle m_RecordButtonStyle;
        GUIStyle m_DeleteButtonStyle;
        GUIStyle m_RecordOneButtonStyle;
        GUIStyle m_RecordButtonGreenStyle;
        GUIStyle m_DeleteButtonPinkStyle;
        GUIStyle m_HeaderSmallStyle;


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

            // get refs
            if (m_FoldoutEmptyStyle == null)
                m_FoldoutEmptyStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("foldoutempty");
            if (m_ScrollViewStyle == null)
                m_ScrollViewStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview");
            if (m_FoldoutStyle == null)
                SetDropdownStyle(out m_FoldoutStyle);
            if (m_RecordButtonStyle == null)
                m_RecordButtonStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button");
            if (m_DeleteButtonStyle == null)
                m_DeleteButtonStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button");
            if (m_RecordOneButtonStyle == null)
                m_RecordOneButtonStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record One Button");
            if (m_RecordButtonGreenStyle == null)
                m_RecordButtonGreenStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green");
            if (m_DeleteButtonPinkStyle == null)
                m_DeleteButtonPinkStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button Pink");
            if (m_HeaderSmallStyle == null)
                m_HeaderSmallStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small");

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
                if (m_TrainingExamplesNode.DesiredInputFeatures != null && m_TrainingExamplesNode.DesiredOutputFeatures != null)
                {
                    if (m_TrainingExamplesNode.DesiredInputFeatures.Count != m_ConnectedInputs
                        || m_ConnectedTargets != m_TrainingExamplesNode.DesiredOutputFeatures.Count
                        || lastShowWarning != m_TrainingExamplesNode.showWarning)
                    {
                        m_RecalculateRects = true;
                    }

                    m_ConnectedInputs = m_TrainingExamplesNode.DesiredInputFeatures.Count;
                    m_ConnectedTargets = m_TrainingExamplesNode.DesiredOutputFeatures.Count;
                }
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
                
                if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
                    m_BodyRect.height = baseNodeBodyHeight - 40; // Removed 40 instead of 30 since we need extra space for unique classes info shown
                else
                    m_BodyRect.height = baseNodeBodyHeight - 60;
                // if showing warning increase height 
                if (m_TrainingExamplesNode.showWarning)
                {
                    if(m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
                        m_BodyRect.height += 90;
                    else
                        m_BodyRect.height += 60;
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

            GUILayout.Space(40);
            ShowTrainingExamplesDropdown();
            GUILayout.Space(2);
            ShowUniqueTrainingClassesDropdown();
            
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

            if (GUILayout.Button("Record Data", m_RecordButtonStyle))
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
        protected void SetDropdownStyle(out GUIStyle myFoldoutStyle)
        {
            if (m_NodeSkin == null) m_NodeSkin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

            GUI.skin = m_NodeSkin;
            myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
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
            myFoldoutStyle.fontStyle = m_ScrollViewStyle.fontStyle;
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
                if (GUILayout.Button("Delete Data", m_DeleteButtonStyle))
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
            EditorStyles.foldout.fontStyle = m_ScrollViewStyle.fontStyle;

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
                if (GUILayout.Button(new GUIContent(""), m_RecordOneButtonStyle))
                {
                    IMLEventDispatcher.RecordOneCallback?.Invoke(m_TrainingExamplesNode.id);
                }
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                GUILayout.Label("record one example", m_RecordButtonGreenStyle);
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
            GUILayout.Label(recordNameButton, m_RecordButtonGreenStyle);
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
            GUILayout.Label("delete all recordings", m_DeleteButtonPinkStyle);
            GUILayout.Label("");
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            // Training pairs number
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
            {
                GUILayout.Label("Number of training pairs: " + m_TrainingExamplesNode.TrainingExamplesVector.Count, m_HeaderSmallStyle);
            }
            else
            {
                GUILayout.Label("Number of training examples: " + m_TrainingExamplesNode.TrainingSeriesCollection.Count, m_HeaderSmallStyle);
            }
            
            GUILayout.EndHorizontal();
            // Unique training classes number
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
            {
                EditorGUI.indentLevel++;
                GUILayout.Label("Number of unique classes: " + m_TrainingExamplesNode.NumUniqueClasses, m_HeaderSmallStyle);
                EditorGUI.indentLevel--;
            }
            // TO DO: Also show unique training classes for DTW
            GUILayout.EndHorizontal();

        }

        /// <summary>
        /// Show single training examples on foldout arrow
        /// </summary>
        private void ShowTrainingExamplesDropdown()
        {
            int numEntries = 0;
            int numEntriesSeries = 0;
            if (m_TrainingExamplesNode.TrainingExamplesVector != null && m_TrainingExamplesNode.TrainingExamplesVector.Count > 0) 
            {
                numEntries = m_TrainingExamplesNode.TrainingExamplesVector.Count;
            }
            if (m_TrainingExamplesNode.TrainingSeriesCollection != null && m_TrainingExamplesNode.TrainingSeriesCollection.Count > 0)
            {
                numEntriesSeries = m_TrainingExamplesNode.TrainingSeriesCollection.Count;
            }

            // only show the first 1000 entries for performance reasons...
            int entriesShown = (numEntries > 1000 || numEntriesSeries > 1000) ? entriesShown = 1000 : entriesShown = Mathf.Max(numEntries, numEntriesSeries);
            string dropdownLabel = (entriesShown == 1000) ? $"View Training Pairs ({Mathf.Max(numEntries, numEntriesSeries)}, showing 1000)" : $"View Training Pairs ({Mathf.Max(numEntries, numEntriesSeries)})";

            m_ShowTrainingDataDropdown = EditorGUILayout.Foldout(m_ShowTrainingDataDropdown, dropdownLabel, m_FoldoutStyle);

            // Save original indent level
            int originalIndentLevel = EditorGUI.indentLevel;

            if (m_ShowTrainingDataDropdown)
            {
                //m_Dropdown.x = m_HelpRect.x;
                //m_Dropdown.y = m_HelpRect.y + m_HelpRect.height;
                //m_Dropdown.width = m_HelpRect.width;
                //m_Dropdown.height = 200;
                //if (Event.current.type == EventType.Layout)
                //{
                //    GUI.DrawTexture(m_Dropdown, NodeColor);
                //}
                    

                //GUILayout.BeginArea(m_Dropdown);

                EditorGUI.indentLevel++;

                if (m_TrainingExamplesNode.TrainingExamplesVector.Count > 0 && m_TrainingExamplesNode.TrainingSeriesCollection.Count > 0)
                {
                    EditorGUILayout.LabelField("Training Examples List is empty", m_FoldoutEmptyStyle);
                }
                else
                {
                    // Begins Vertical Scroll
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUILayout.BeginVertical();


                    // Single training examples (classification, regression) (up to 1000 for performance reasons)
                    if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
                    {
                        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 20), GUILayout.Height(GetWidth() - 50));

                        for (int i = 0; i < entriesShown; i++)
                        {
                            EditorGUILayout.LabelField("Training Example " + (i + 1), m_ScrollViewStyle);

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

                                    EditorGUILayout.LabelField("Input " + (j + 1) + " (" + inputFeatures[j].InputData.DataType + ")", m_ScrollViewStyle);

                                    for (int k = 0; k < inputFeatures[j].InputData.Values.Length; k++)
                                    {
                                        EditorGUI.indentLevel++;

                                        EditorGUILayout.LabelField(inputFeatures[j].InputData.Values[k].ToString(), m_ScrollViewStyle);

                                        EditorGUI.indentLevel--;
                                    }

                                    EditorGUI.indentLevel--;
                                }
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Inputs are null ", m_ScrollViewStyle);
                            }

                            // If the output features are not null...
                            if (outputFeatures != null)
                            {
                                // Draw outputs
                                for (int j = 0; j < outputFeatures.Count; j++)
                                {
                                    if (outputFeatures[j].OutputData == null)
                                    {
                                        EditorGUILayout.LabelField("Outputs are null ", m_ScrollViewStyle);
                                        break;
                                    }


                                    EditorGUI.indentLevel++;

                                    EditorGUILayout.LabelField("Output " + (j + 1) + " (" + outputFeatures[j].OutputData.DataType + ")", m_ScrollViewStyle);


                                    for (int k = 0; k < outputFeatures[j].OutputData.Values.Length; k++)
                                    {

                                        EditorGUI.indentLevel++;

                                        EditorGUILayout.LabelField(outputFeatures[j].OutputData.Values[k].ToString(), m_ScrollViewStyle);

                                        EditorGUI.indentLevel--;

                                    }

                                    EditorGUI.indentLevel--;

                                }
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Outputs are null ", m_ScrollViewStyle);
                            }

                            EditorGUI.indentLevel--;

                        }

                        // Ends Vertical Scroll
                        EditorGUI.indentLevel = indentLevel;
                        EditorGUILayout.EndScrollView();
                    }
                    // Series training examples (DTW)
                    else
                    {
                        // init dropdowns
                        if (m_DataDropdownsLvl0 == null || m_DataDropdownsLvl0.Length != m_TrainingExamplesNode.TrainingSeriesCollection.Count)
                            m_DataDropdownsLvl0 = new bool[m_TrainingExamplesNode.TrainingSeriesCollection.Count];

                        m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 25), GUILayout.Height(GetWidth() - 100));

                        // Go Series by Series (only up to 1000)
                        for (int i = 0; i < entriesShown; i++)
                        {
                            string numSeries = (m_TrainingExamplesNode.TrainingSeriesCollection[i].Series == null) ? "null" : m_TrainingExamplesNode.TrainingSeriesCollection[i].Series.Count.ToString();                            
                            
                            //EditorGUILayout.LabelField("Training Series " + i, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                            m_DataDropdownsLvl0[i] = EditorGUILayout.Foldout(m_DataDropdownsLvl0[i], $"Training Series {i} ({numSeries} Entries)", m_FoldoutStyle);

                            EditorGUI.indentLevel++;

                            if (m_DataDropdownsLvl0[i])
                            {
                                var inputFeaturesInSeries = m_TrainingExamplesNode.TrainingSeriesCollection[i].Series;
                                var labelSeries = m_TrainingExamplesNode.TrainingSeriesCollection[i].LabelSeries;

                                // If the input features are not null...
                                if (inputFeaturesInSeries != null)
                                {
                                    // If features are empty...
                                    if (inputFeaturesInSeries.Count == 0)
                                    {
                                        EditorGUILayout.LabelField("Empty series, but not null!", m_ScrollViewStyle);
                                    }
                                    // If there are features...
                                    else
                                    {
                                        // init dropdowns
                                        if (m_DataDropdownsLvl1 == null || m_DataDropdownsLvl1.Length != inputFeaturesInSeries.Count)
                                            m_DataDropdownsLvl1 = new bool[inputFeaturesInSeries.Count];

                                        for (int k = 0; k < inputFeaturesInSeries.Count; i++)
                                        {
                                            //EditorGUILayout.LabelField("No. Examples: " + inputFeaturesInSeries.Count, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                                            m_DataDropdownsLvl1[k] = EditorGUILayout.Foldout(m_DataDropdownsLvl1[k], $"No. Examples: {inputFeaturesInSeries.Count}", m_FoldoutStyle);

                                            EditorGUI.indentLevel++;

                                            if (m_DataDropdownsLvl1[k])
                                            {
                                                // Draw inputs
                                                for (int j = 0; j < inputFeaturesInSeries.Count; j++)
                                                {
                                                    EditorGUI.indentLevel++;

                                                    EditorGUILayout.LabelField("Input Feature " + j, m_ScrollViewStyle);

                                                    // Are there any examples in series?
                                                    if (inputFeaturesInSeries[j] == null)
                                                    {
                                                        EditorGUILayout.LabelField("Inputs are null ", m_ScrollViewStyle);
                                                        break;
                                                    }

                                                    EditorGUI.indentLevel++;

                                                    for (int z = 0; z < inputFeaturesInSeries[j].Count; z++)
                                                    {
                                                        EditorGUILayout.LabelField("Input " + z + " (" + inputFeaturesInSeries[j][z].InputData.DataType + ")", m_ScrollViewStyle);

                                                        for (int w = 0; w < inputFeaturesInSeries[j][z].InputData.Values.Length; w++)
                                                        {
                                                            EditorGUI.indentLevel++;

                                                            EditorGUILayout.LabelField(inputFeaturesInSeries[j][z].InputData.Values[w].ToString(), m_ScrollViewStyle);

                                                            EditorGUI.indentLevel--;
                                                        }


                                                    }

                                                    EditorGUI.indentLevel--;
                                                    EditorGUI.indentLevel--;
                                                }

                                            }

                                            EditorGUI.indentLevel--;

                                        }

                                    }
                                }
                                // If the input features are null...
                                else
                                {
                                    EditorGUILayout.LabelField("Input Features in series are null", m_ScrollViewStyle);
                                }

                                // If the output features for the entire series are not null...
                                if (labelSeries != null)
                                {
                                    // Draw output
                                    EditorGUI.indentLevel++;

                                    EditorGUILayout.TextArea(labelSeries, m_ScrollViewStyle);
                                    //EditorGUILayout.LabelField("TEST");

                                    EditorGUI.indentLevel--;
                                }
                                else
                                {
                                    EditorGUILayout.LabelField("Series Target Values are null ", m_ScrollViewStyle);
                                }
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

                // Reset indent level
                EditorGUI.indentLevel = originalIndentLevel;

                //GUILayout.EndArea();
            }

        }

        /// <summary>
        /// Show unique training classes on foldout arrow
        /// </summary>
        private void ShowUniqueTrainingClassesDropdown()
        {
            int numEntries = 0;
            int numEntriesSeries = 0;
            if (m_TrainingExamplesNode.UniqueClasses != null && m_TrainingExamplesNode.UniqueClasses.Count > 0)
            {
                numEntries = m_TrainingExamplesNode.UniqueClasses.Count;
            }
            //if (m_TrainingExamplesNode.TrainingSeriesCollection != null && m_TrainingExamplesNode.TrainingSeriesCollection.Count > 0)
            //{
            //    numEntriesSeries = m_TrainingExamplesNode.TrainingSeriesCollection.Count;
            //}

            // only show the first 1000 entries for performance reasons...
            int entriesShown = (numEntries > 1000 || numEntriesSeries > 1000) ? entriesShown = 1000 : entriesShown = Mathf.Max(numEntries, numEntriesSeries);
            string dropdownLabel = (entriesShown == 1000) ? $"View Training Pairs ({Mathf.Max(numEntries, numEntriesSeries)}, showing 1000)" : $"View Unique Training Classes ({Mathf.Max(numEntries, numEntriesSeries)})";

            m_ShowUniqueClassesDropdown = EditorGUILayout.Foldout(m_ShowUniqueClassesDropdown, dropdownLabel, m_FoldoutStyle);

            // Save original indent level
            int originalIndentLevel = EditorGUI.indentLevel;

            if (m_ShowUniqueClassesDropdown)
            {
                //m_Dropdown.x = m_HelpRect.x;
                //m_Dropdown.y = m_HelpRect.y + m_HelpRect.height;
                //m_Dropdown.width = m_HelpRect.width;
                //m_Dropdown.height = 200;
                //if (Event.current.type == EventType.Layout)
                //{
                //    GUI.DrawTexture(m_Dropdown, NodeColor);
                //}


                //GUILayout.BeginArea(m_Dropdown);

                EditorGUI.indentLevel++;

                if (m_TrainingExamplesNode.UniqueClasses.Count == 0)
                {
                    EditorGUILayout.LabelField("Unique Training Classes List is empty", m_FoldoutEmptyStyle);
                }
                else
                {
                    // Begins Vertical Scroll
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUILayout.BeginVertical();


                    // Single training examples (classification, regression) (up to 1000 for performance reasons)
                    if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
                    {
                        m_UniqueClassesScrollPos = EditorGUILayout.BeginScrollView(m_UniqueClassesScrollPos, GUILayout.Width(GetWidth() - 20), GUILayout.Height(GetWidth() - 50));

                        for (int i = 0; i < entriesShown; i++)
                        {
                            EditorGUILayout.LabelField("Unique Class " + (i + 1), m_ScrollViewStyle);

                            EditorGUI.indentLevel++;

                            var inputFeatures = m_TrainingExamplesNode.UniqueClasses[i].Inputs;
                            var outputFeatures = m_TrainingExamplesNode.UniqueClasses[i].Outputs;

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

                                    EditorGUILayout.LabelField("Input " + (j + 1) + " (" + inputFeatures[j].InputData.DataType + ")", m_ScrollViewStyle);

                                    //for (int k = 0; k < inputFeatures[j].InputData.Values.Length; k++)
                                    //{
                                    EditorGUI.indentLevel++;

                                    //EditorGUILayout.LabelField(inputFeatures[j].InputData.Values[k].ToString(), m_ScrollViewStyle);
                                    EditorGUILayout.LabelField("Any", m_ScrollViewStyle);

                                    EditorGUI.indentLevel--;
                                    //}

                                    EditorGUI.indentLevel--;
                                }
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Inputs are null ", m_ScrollViewStyle);
                            }

                            // If the output features are not null...
                            if (outputFeatures != null)
                            {
                                // Draw outputs
                                for (int j = 0; j < outputFeatures.Count; j++)
                                {
                                    if (outputFeatures[j].OutputData == null)
                                    {
                                        EditorGUILayout.LabelField("Outputs are null ", m_ScrollViewStyle);
                                        break;
                                    }


                                    EditorGUI.indentLevel++;

                                    EditorGUILayout.LabelField("Output Class" + (j + 1) + " (" + outputFeatures[j].OutputData.DataType + ")", m_ScrollViewStyle);


                                    for (int k = 0; k < outputFeatures[j].OutputData.Values.Length; k++)
                                    {

                                        EditorGUI.indentLevel++;

                                        EditorGUILayout.LabelField(outputFeatures[j].OutputData.Values[k].ToString(), m_ScrollViewStyle);

                                        EditorGUI.indentLevel--;

                                    }

                                    EditorGUI.indentLevel--;

                                }
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Outputs are null ", m_ScrollViewStyle);
                            }

                            EditorGUI.indentLevel--;

                        }

                        // Ends Vertical Scroll
                        EditorGUI.indentLevel = indentLevel;
                        EditorGUILayout.EndScrollView();
                    }
                    // TO DO: Series unique training classes (DTW)
                    else
                    {
                        //// init dropdowns
                        //if (m_UniqueClassesDropdownsLvl0 == null || m_UniqueClassesDropdownsLvl0.Length != m_TrainingExamplesNode.TrainingSeriesCollection.Count)
                        //    m_UniqueClassesDropdownsLvl0 = new bool[m_TrainingExamplesNode.TrainingSeriesCollection.Count];

                        //m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 25), GUILayout.Height(GetWidth() - 100));

                        //// Go Series by Series (only up to 1000)
                        //for (int i = 0; i < entriesShown; i++)
                        //{
                        //    string numSeries = (m_TrainingExamplesNode.TrainingSeriesCollection[i].Series == null) ? "null" : m_TrainingExamplesNode.TrainingSeriesCollection[i].Series.Count.ToString();

                        //    //EditorGUILayout.LabelField("Training Series " + i, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                        //    m_UniqueClassesDropdownsLvl0[i] = EditorGUILayout.Foldout(m_UniqueClassesDropdownsLvl0[i], $"Training Series {i} ({numSeries} Entries)", m_FoldoutStyle);

                        //    EditorGUI.indentLevel++;

                        //    if (m_UniqueClassesDropdownsLvl0[i])
                        //    {
                        //        var inputFeaturesInSeries = m_TrainingExamplesNode.TrainingSeriesCollection[i].Series;
                        //        var labelSeries = m_TrainingExamplesNode.TrainingSeriesCollection[i].LabelSeries;

                        //        // If the input features are not null...
                        //        if (inputFeaturesInSeries != null)
                        //        {
                        //            // If features are empty...
                        //            if (inputFeaturesInSeries.Count == 0)
                        //            {
                        //                EditorGUILayout.LabelField("Empty series, but not null!", m_ScrollViewStyle);
                        //            }
                        //            // If there are features...
                        //            else
                        //            {
                        //                // init dropdowns
                        //                if (m_UniqueClassesDropdownsLvl1 == null || m_UniqueClassesDropdownsLvl1.Length != inputFeaturesInSeries.Count)
                        //                    m_UniqueClassesDropdownsLvl1 = new bool[inputFeaturesInSeries.Count];

                        //                for (int k = 0; k < inputFeaturesInSeries.Count; i++)
                        //                {
                        //                    //EditorGUILayout.LabelField("No. Examples: " + inputFeaturesInSeries.Count, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                        //                    m_UniqueClassesDropdownsLvl1[k] = EditorGUILayout.Foldout(m_UniqueClassesDropdownsLvl1[k], $"No. Examples: {inputFeaturesInSeries.Count}", m_FoldoutStyle);

                        //                    EditorGUI.indentLevel++;

                        //                    if (m_UniqueClassesDropdownsLvl1[k])
                        //                    {
                        //                        // Draw inputs
                        //                        for (int j = 0; j < inputFeaturesInSeries.Count; j++)
                        //                        {
                        //                            EditorGUI.indentLevel++;

                        //                            EditorGUILayout.LabelField("Input Feature " + j, m_ScrollViewStyle);

                        //                            // Are there any examples in series?
                        //                            if (inputFeaturesInSeries[j] == null)
                        //                            {
                        //                                EditorGUILayout.LabelField("Inputs are null ", m_ScrollViewStyle);
                        //                                break;
                        //                            }

                        //                            EditorGUI.indentLevel++;

                        //                            for (int z = 0; z < inputFeaturesInSeries[j].Count; z++)
                        //                            {
                        //                                EditorGUILayout.LabelField("Input " + z + " (" + inputFeaturesInSeries[j][z].InputData.DataType + ")", m_ScrollViewStyle);

                        //                                for (int w = 0; w < inputFeaturesInSeries[j][z].InputData.Values.Length; w++)
                        //                                {
                        //                                    EditorGUI.indentLevel++;

                        //                                    EditorGUILayout.LabelField(inputFeaturesInSeries[j][z].InputData.Values[w].ToString(), m_ScrollViewStyle);

                        //                                    EditorGUI.indentLevel--;
                        //                                }


                        //                            }

                        //                            EditorGUI.indentLevel--;
                        //                            EditorGUI.indentLevel--;
                        //                        }

                        //                    }

                        //                    EditorGUI.indentLevel--;

                        //                }

                        //            }
                        //        }
                        //        // If the input features are null...
                        //        else
                        //        {
                        //            EditorGUILayout.LabelField("Input Features in series are null", m_ScrollViewStyle);
                        //        }

                        //        // If the output features for the entire series are not null...
                        //        if (labelSeries != null)
                        //        {
                        //            // Draw output
                        //            EditorGUI.indentLevel++;

                        //            EditorGUILayout.TextArea(labelSeries, m_ScrollViewStyle);
                        //            //EditorGUILayout.LabelField("TEST");

                        //            EditorGUI.indentLevel--;
                        //        }
                        //        else
                        //        {
                        //            EditorGUILayout.LabelField("Series Target Values are null ", m_ScrollViewStyle);
                        //        }
                        //    }

                        //    EditorGUI.indentLevel--;
                        //}

                        //// Ends Vertical Scroll
                        //EditorGUI.indentLevel = indentLevel;
                        //EditorGUILayout.EndScrollView();
                    }

                    EditorGUILayout.EndVertical();

                }

                EditorGUI.indentLevel--;

                // Reset indent level
                EditorGUI.indentLevel = originalIndentLevel;

                //GUILayout.EndArea();
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
