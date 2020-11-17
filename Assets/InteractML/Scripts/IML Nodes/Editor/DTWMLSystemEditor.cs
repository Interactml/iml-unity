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
    [CustomNodeEditor(typeof(DTWMLSystem))]
    public class DTWMLSystemEditor : IMLNodeEditor
    {

        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private DTWMLSystem m_DTWMLSystem;

        /// <summary>
        /// Bool for add/remove output
        /// </summary>
        private bool m_AddOutput;
        private bool m_RemoveOutput;
        private bool m_DTWSwitch;

        

        #endregion

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_DTWMLSystem = (target as DTWMLSystem);
            nodeSpace = 20;
            string arrayNo = "";
            if (m_DTWMLSystem.numberInComponentList != -1)
                arrayNo = m_DTWMLSystem.numberInComponentList.ToString();
            NodeName = "MACHINE LEARNING SYSTEM " + arrayNo;
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            NodeWidth = 300;
            InputPortsNamesOverride = new Dictionary<string, string>();
            InputPortsNamesOverride.Add("IMLTrainingExamplesNodes", "Recorded Data In");
            InputPortsNamesOverride.Add("InputFeatures", "Live Data In");
            base.nodeTips = m_DTWMLSystem.tooltips;
            m_BodyRect.height = 330;
            base.OnBodyGUI();
        }

        protected override void ShowBodyFields()
        {
            ShowTrainingIcon("DTW");
            ShowButtons(m_DTWMLSystem);
            GUILayout.Space(320);
            ShowRunOnAwakeToggle(m_DTWMLSystem as MLSystem);
            GUILayout.Space(20);
            // if there is an error show the correct warning
            if (m_DTWMLSystem.error)
            {
                nodeSpace = 40;
                m_BodyRect.height = 430;
                ShowWarning(m_DTWMLSystem.warning);
            }

            
        }


        protected override void TrainModelButton()
        {
            Debug.Log(m_DTWMLSystem.TotalNumTrainingData);
            // Only run button logic when rapidlib reference not null and training examples are not null
            if (m_DTWMLSystem.Model != null && m_DTWMLSystem.TotalNumTrainingData > 0)
            {

                string nameButton = "";

                if (m_DTWMLSystem.Training)
                    nameButton = "STOP Training Model";
                if (m_DTWMLSystem.Trained)
                    nameButton = "Trained (" + m_DTWMLSystem.NumExamplesTrainedOn + " Examples)";
                else
                    nameButton = "Train Model";

                // Disable button if model is Running OR Trainig 
                if (m_DTWMLSystem.Running || m_DTWMLSystem.Training)
                    GUI.enabled = false;



                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Train")))
                {
                    IMLEventDispatcher.TrainMLSCallback(m_DTWMLSystem.id);
                    // Train model
                   /* if (m_DTWMLSystem.TrainModel())
                    {
                        // Save model if succesfully trained
                        m_DTWMLSystem.SaveModelToDisk();
                    }*/
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {

                GUI.enabled = false;
                if (m_DTWMLSystem.TotalNumTrainingData == 0)
                {
                    //EditorGUILayout.HelpBox("There are no training examples", MessageType.Error);
                }
                if (GUILayout.Button("Train Model", m_NodeSkin.GetStyle("Train")))
                {
                    //m_TrainingExamplesNode.ToggleCollectExamples();
                }
                //GUI.enabled = true;

            }

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                if (GUI.enabled)
                {
                    TooltipText = m_DTWMLSystem.tooltips.BodyTooltip.Tips[1];
                }
                else
                {
                    TooltipText = m_DTWMLSystem.tooltips.BodyTooltip.Error[0];
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

        protected override void RunModelButton()
        {
            bool enabled = false;
            if (m_DTWMLSystem.Model != null)
            {
                string nameButton = "";

                if (m_DTWMLSystem.Running)
                {
                    nameButton = "STOP";
                }
                else
                {
                    nameButton = "Populate";
                }

                // Disable button if model is Trainig OR Untrained
                if (m_DTWMLSystem.Training || m_DTWMLSystem.Untrained)
                    GUI.enabled = false;

                //Disable button if inputs don't match attached training examples node/s
                if (!m_DTWMLSystem.matchLiveDataInputs || !m_DTWMLSystem.matchVectorLength)
                {
                    Debug.Log("Number of live data nodes connected to input features do not match training examples live inputs input features");
                    GUI.enabled = false;
                }

                if (GUILayout.Button(nameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Run")))
                {
                    if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Run")))
                    {
                        if (m_DTWMLSystem.Running)
                        {
                            IMLEventDispatcher.StopRunCallback(m_DTWMLSystem.id);
                        }
                        else
                        {
                            IMLEventDispatcher.StartRunCallback(m_DTWMLSystem.id);
                        }
                    }
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {
                GUI.enabled = false;
            }

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                if (enabled)
                {
                    TooltipText = m_DTWMLSystem.tooltips.BodyTooltip.Tips[2];
                }
                else
                {
                    TooltipText = m_DTWMLSystem.tooltips.BodyTooltip.Error[1];
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

    }
}


