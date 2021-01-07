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
            // Get reference to the current node
            m_TrainingExamplesNode = (target as TrainingExamplesNode);
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
            if (m_TrainingExamplesNode.DesiredInputFeatures.Count != m_ConnectedInputs || m_ConnectedTargets != m_TrainingExamplesNode.DesiredOutputFeatures.Count|| lastShowWarning != m_TrainingExamplesNode.showWarning)
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

            // Draw button record examples
            if (disableButton)
                GUI.enabled = false;
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
                    m_TrainingExamplesNode.ClearTrainingExamples();
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


    }

}
