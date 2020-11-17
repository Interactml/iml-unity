using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableMethods;
using System.Linq;
using XNode;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeEditor(typeof(RegressionMLSystem))]
    public class RegressionMLSystemEditor : IMLNodeEditor
    {

        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private RegressionMLSystem m_RegressionMLSystem;
        #endregion

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_RegressionMLSystem = (target as RegressionMLSystem);
            nodeSpace = 60;
            string arrayNo = "";
            if (m_RegressionMLSystem.numberInComponentList != -1)
                arrayNo = m_RegressionMLSystem.numberInComponentList.ToString();
            NodeName = "MACHINE LEARNING SYSTEM " + arrayNo;
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            NodeWidth = 300;
            InputPortsNamesOverride = new Dictionary<string, string>();
            InputPortsNamesOverride.Add("IMLTrainingExamplesNodes", "Recorded Data In");
            InputPortsNamesOverride.Add("InputFeatures", "Live Data In");
            base.nodeTips = m_RegressionMLSystem.tooltips;
            m_BodyRect.height = 360;
            base.OnBodyGUI();

        }

        protected override void ShowBodyFields()
        {
            ShowTrainingIcon("Regression");
            GUILayout.Space(320);
            ShowButtons(m_RegressionMLSystem);
            ShowRunOnAwakeToggle(m_RegressionMLSystem as MLSystem);
            // if there is an error show the correct warning
            if (m_RegressionMLSystem.error)
            {
                nodeSpace = 70;
                m_BodyRect.height = m_BodyRect.height + HeaderRect.height + 40;
                ShowWarning(m_RegressionMLSystem.warning);
            }
        }


        protected override void TrainModelButton()
        {
            // Only run button logic when rapidlib reference not null and training examples are not null
            if (m_RegressionMLSystem.Model != null && m_RegressionMLSystem.TotalNumTrainingData > 0)
            {

                string nameButton = "";

                if (m_RegressionMLSystem.Training)
                    nameButton = "STOP Training Model";
                if (m_RegressionMLSystem.Trained)
                    nameButton = "Trained (" + m_RegressionMLSystem.NumExamplesTrainedOn + " Examples)";
                else
                    nameButton = "Train Model";

                // Disable button if model is Running OR Trainig 
                if (m_RegressionMLSystem.Running || m_RegressionMLSystem.Training)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Train")))
                {
                    IMLEventDispatcher.TrainMLSCallback(m_RegressionMLSystem.id);
                    // Train model old implementation to be deleted when tested
                   /*if (m_RIMLConfiguration.TrainModel())
                    {
                        // Save model if succesfully trained
                        m_RIMLConfiguration.SaveModelToDisk();
                    }*/
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {

                GUI.enabled = false;
                if (m_RegressionMLSystem.TotalNumTrainingData == 0)
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
                    TooltipText = m_RegressionMLSystem.tooltips.BodyTooltip.Tips[1];
                } else
                {
                    TooltipText = m_RegressionMLSystem.tooltips.BodyTooltip.Error[0];
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
            if (m_RegressionMLSystem.Model != null)
            {
                string nameButton = "";

                if (m_RegressionMLSystem.Running)
                {
                    nameButton = "STOP";
                }
                else
                {
                    nameButton = "RUN";
                }

                // Disable button if model is Trainig OR Untrained
                if (m_RegressionMLSystem.Training || m_RegressionMLSystem.Untrained)
                {
                    GUI.enabled = false;
                    enabled = false;
                } else
                {
                    enabled = true;
                }
                //Disable button if inputs don't match attached training examples node/s
                if (!m_RegressionMLSystem.matchLiveDataInputs || !m_RegressionMLSystem.matchVectorLength)
                {
                    Debug.Log("Number of live data nodes connected to input features do not match training examples live inputs input features");
                    GUI.enabled = false;
                }
                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Run")))
                {
                    Debug.Log("run");
                    if (m_RegressionMLSystem.Running)
                    {
                        IMLEventDispatcher.StopRunCallback(m_RegressionMLSystem.id);
                    }
                    else
                    {
                        IMLEventDispatcher.StartRunCallback(m_RegressionMLSystem.id);
                    }
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button("Run", m_NodeSkin.GetStyle("Run")))
                {
                    //m_TrainingExamplesNode.ToggleCollectExamples();
                }
                GUI.enabled = true;
            }

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                if (enabled)
                {
                    TooltipText = m_RegressionMLSystem.tooltips.BodyTooltip.Tips[2];
                }
                else
                {
                    TooltipText = m_RegressionMLSystem.tooltips.BodyTooltip.Error[1];
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

