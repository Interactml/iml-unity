using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableMethods;
using System;
using System.Linq;
using XNode;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif
namespace InteractML
{
    [CustomNodeEditor(typeof(MLSystem))]
    public class MLSystemEditor : IMLNodeEditor
    {

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        protected MLSystem m_MLSystem;

        /// <summary>
        /// The label to show on the button port labels
        /// </summary>
        protected GUIContent m_ButtonPortLabel;
        /// <summary>
        /// NodePort for button. Loaded in OnHeaderHUI()
        /// </summary>
        protected NodePort m_ButtonPortToggleTrainInput;
        /// <summary>
        /// NodePort for button. Loaded in OnHeaderHUI()
        /// </summary>
        protected NodePort m_ButtonPortToggleRunInput;


        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_MLSystem = (target as MLSystem);
            //previous nodespace 10
            nodeSpace = 20;
            string arrayNo = "";
            if (m_MLSystem.numberInComponentList != -1)
                arrayNo = m_MLSystem.numberInComponentList.ToString();
            NodeName = "MACHINE LEARNING SYSTEM " + arrayNo;

            // Create inputport button label
            if (m_ButtonPortLabel == null)
                m_ButtonPortLabel = new GUIContent("");

            // Get button ports
            if (m_ButtonPortToggleTrainInput == null)
                m_ButtonPortToggleTrainInput = m_MLSystem.GetPort("ToggleTrainInputBoolPort");
            if (m_ButtonPortToggleRunInput == null)
                m_ButtonPortToggleRunInput = m_MLSystem.GetPort("ToggleRunInputBoolPort");


            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            InputPortsNamesOverride = new Dictionary<string, string>();
            InputPortsNamesOverride.Add("IMLTrainingExamplesNodes", "Recorded Data In");
            InputPortsNamesOverride.Add("InputFeatures", "Live Data In");
            base.nodeTips = m_MLSystem.tooltips;
            //previous nodespace 330
            m_BodyRect.height = 350;
            base.OnBodyGUI();
        }

        protected override void ShowBodyFields()
        {
            ShowTrainingIcon(m_MLSystem.LearningType.ToString());
            ShowButtons(m_MLSystem);
            GUILayout.Space(20);
            ShowRunOnAwakeToggle(m_MLSystem as MLSystem);
            GUILayout.Space(20);
            // if there is an error show the correct warning
            if (m_MLSystem.error)
            {

                nodeSpace = 60;
                m_BodyRect.height = m_BodyRect.height + HeaderRect.height + 40;
                ShowWarning(m_MLSystem.warning);
            }
        }

        /// <summary>
        /// Show and control run on awake toggle for IMLConfiguration node
        /// </summary>
        /// <param name="configNode">Node to be controlled</param>
        protected void ShowRunOnAwakeToggle(MLSystem configNode)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            configNode.RunOnAwake = EditorGUILayout.Toggle(configNode.RunOnAwake, m_NodeSkin.GetStyle("Local Space Toggle"));
            EditorGUILayout.LabelField("Run Model On Play", m_NodeSkin.GetStyle("Node Local Space Label"));
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// Show MLS Algorithm icon 
        /// </summary>
        /// <param name="MLS">Name of Machine Learning System: Classification, Regression or DTW</param>
        protected void ShowTrainingIcon(string MLS)
        {
            m_IconCenter.x = m_BodyRect.x;
            m_IconCenter.y = m_BodyRect.y;
            m_IconCenter.width = m_BodyRect.width;
            m_IconCenter.height = 150;

            GUILayout.BeginArea(m_IconCenter);
            GUILayout.Space(bodySpace);
            GUILayout.BeginHorizontal();
            GUILayout.Box("", m_NodeSkin.GetStyle(MLS + " MLS Image"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(MLS, m_NodeSkin.GetStyle(MLS + " Label"));
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

        }

        /// <summary>
        /// Show the load, delete and record buttons
        /// </summary>
        protected void ShowButtons(MLSystem node)
        {
            m_ButtonsRect.x = m_BodyRect.x -5 ;
            m_ButtonsRect.y = m_IconCenter.y + m_IconCenter.height;
            m_ButtonsRect.width = m_BodyRect.width -10;
            m_ButtonsRect.height = 150;
            GUILayout.Space(230);

            // DRAW BUTTONS AND PORTS OUTSIDE OF BEGIN AREA TO MAKE THEM WORK
            GUILayout.BeginHorizontal();
            // Draw port
            IMLNodeEditor.PortField(m_ButtonPortLabel, m_ButtonPortToggleTrainInput, m_NodeSkin.GetStyle("Port Label"), GUILayout.MaxWidth(10));

            // if button contains mouse position
            TrainModelButton();
            GUILayout.EndHorizontal();


            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            // Draw port
            IMLNodeEditor.PortField(m_ButtonPortLabel, m_ButtonPortToggleRunInput, m_NodeSkin.GetStyle("Port Label"), GUILayout.MaxWidth(10));

            RunModelButton();
            GUILayout.EndHorizontal();

            GUILayout.BeginArea(m_ButtonsRect);

            //GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(NodeWidth / 2 - 20);
            if (GUILayout.Button("", m_NodeSkin.GetStyle("Reset")))
            {
                node.ResetModel();
                numberOfExamplesTrained = 0;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("reset model", m_NodeSkin.GetStyle("Reset Pink Label"));
            GUILayout.EndHorizontal();
            //ShowRunOnAwakeToggle(node);
            GUILayout.EndArea();
        }
        /// <summary>
        ///Training button for MLS node 
        /// </summary>
        protected virtual void TrainModelButton()
        {
            string nameButton = "";

            if (m_MLSystem.Trained)
                nameButton = "Trained (" + m_MLSystem.NumExamplesTrainedOn + " Examples)";
            else if (m_MLSystem.Training)
                nameButton = "Training";
            else
                nameButton = "Train Model";

            if (m_MLSystem.Model != null && m_MLSystem.TotalNumTrainingDataConnected > 0 && !m_MLSystem.Running && !m_MLSystem.Training)
            {
                // Enable UI
                GUI.enabled = true;
            }
            // If rapidlib reference is null we draw a disabled button or if it is running or training
            else
            {
                GUI.enabled = false;

            }
            if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Train")))
            {
                IMLEventDispatcher.TrainMLSCallback?.Invoke(m_MLSystem.id);
            }

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                if (GUI.enabled)
                {
                    // Check for null
                    if (m_MLSystem.tooltips != null && m_MLSystem.tooltips.BodyTooltip != null && m_MLSystem.tooltips.BodyTooltip.Tips != null)
                        TooltipText = m_MLSystem.tooltips.BodyTooltip.Tips[1];
                }
                else
                {
                    //TooltipText = m_MLSystem.tooltips.BodyTooltip.Error[0];
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
        /// <summary>
        /// Run button for MLS node
        /// </summary>
        protected virtual void RunModelButton()
        {
            string nameButton = "";
            if (m_MLSystem.Running)
            {
                nameButton = "STOP";
            }
            else
            {
                if (m_MLSystem.TrainingType == IMLSpecifications.TrainingSetType.SeriesTrainingExamples)
                    nameButton = "Populate";
                else
                    nameButton = "Run";
            }
            // If rapidlib reference is null we draw a disabled button
            if ((m_MLSystem.Model == null || m_MLSystem.Model.ModelAddress == (IntPtr)0 || m_MLSystem.Training || m_MLSystem.Untrained || !m_MLSystem.matchLiveDataInputs || !m_MLSystem.matchVectorLength) && !m_MLSystem.Running)
            {
               /* Debug.Log(m_MLSystem.Model == null);
                Debug.Log(m_MLSystem.Model.ModelAddress != (IntPtr)0);
                Debug.Log(m_MLSystem.Training);
                Debug.Log(m_MLSystem.Untrained);
                Debug.Log(!m_MLSystem.matchLiveDataInputs);
                Debug.Log(!m_MLSystem.matchVectorLength);*/
                // Disable button if model is Trainig OR Untrained 
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }

            if (GUILayout.Button(nameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Run")))
            {
                IMLEventDispatcher.ToggleRunCallback(m_MLSystem.id);
            }

            
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                if (GUI.enabled)
                {
                    // Check for null
                    if (m_MLSystem.tooltips != null && m_MLSystem.tooltips.BodyTooltip != null && m_MLSystem.tooltips.BodyTooltip.Tips != null)
                        TooltipText = m_MLSystem.tooltips.BodyTooltip.Tips[2];
                }
                else
                {
                    // Check for null
                    if (m_MLSystem.tooltips != null && m_MLSystem.tooltips.BodyTooltip != null && m_MLSystem.tooltips.BodyTooltip.Tips != null)
                        TooltipText = m_MLSystem.tooltips.BodyTooltip.Error[1];
                }
            }
            else if (Event.current.type == EventType.MouseMove && !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTip = false;

            }
            GUI.enabled = true;

            if (Event.current.type == EventType.Layout && buttonTipHelper)
            {
                buttonTip = true;
                buttonTipHelper = false;
            }
        }

    }
}
