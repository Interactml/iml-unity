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
    [CustomNodeEditor(typeof(CIMLConfiguration))]
    public class CIMLConfigurationEditor : IMLNodeEditor
    {
        
        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private CIMLConfiguration m_CIMLConfiguration;
        #endregion


        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_CIMLConfiguration = (target as CIMLConfiguration);
            nodeSpace = 380;
            string arrayNo = "";
            if (m_CIMLConfiguration.numberInComponentList != -1)
                arrayNo = m_CIMLConfiguration.numberInComponentList.ToString();
            NodeName = "MACHINE LEARNING SYSTEM " + arrayNo;
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            NodeWidth = 300;
            InputPortsNamesOverride = new Dictionary<string, string>();
            InputPortsNamesOverride.Add("IMLTrainingExamplesNodes", "Recorded Data In");
            InputPortsNamesOverride.Add("InputFeatures", "Live Data In");
            base.nodeTips = m_CIMLConfiguration.tooltips;
            m_BodyRect.height = 320;
            base.OnBodyGUI();
        }

        protected override void ShowBodyFields()
        {
            ShowTrainingIcon("Classification");
            ShowButtons(m_CIMLConfiguration);
            ShowRunOnAwakeToggle(m_CIMLConfiguration as IMLConfiguration);
        }




        protected override void TrainModelButton()
        {
            
            // Only run button logic when rapidlib reference not null and training examples are not null
            if (m_CIMLConfiguration.Model != null && m_CIMLConfiguration.TotalNumTrainingData > 0)
            {


                string nameButton = "";

                if (m_CIMLConfiguration.Training)
                    nameButton = "STOP Training Model";
                if (m_CIMLConfiguration.Trained)
                    nameButton = "Trained (" + m_CIMLConfiguration.NumExamplesTrainedOn + " Examples)";
                else
                    nameButton = "Train Model";


                // Disable button if model is Running OR Training 
                if (m_CIMLConfiguration.Running || m_CIMLConfiguration.Training)
                    GUI.enabled = false;
                
                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Train")))
                {
                    // Train model
                    if (m_CIMLConfiguration.TrainModel())
                    {
                        // Save model if succesfully trained
                        m_CIMLConfiguration.SaveModelToDisk();
                    }
                }
                // Always enable it back at the end
                GUI.enabled = true;

                

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {

                GUI.enabled = false;
                if (m_CIMLConfiguration.TotalNumTrainingData == 0)
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
                    TooltipText = m_CIMLConfiguration.tooltips.BodyTooltip.Tips[1];

                    //TooltipText = m_CIMLConfiguration.tips.BodyTooltip.Tips[1];
                } else
                {
                    TooltipText = m_CIMLConfiguration.tooltips.BodyTooltip.Error[0];
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
            if (m_CIMLConfiguration.Model != null)
            {
                string nameButton = "";

                if (m_CIMLConfiguration.Running)
                {
                    nameButton = "STOP";
                }
                else
                {
                    nameButton = "RUN";
                }

                // Disable button if model is Training OR Untrained
                if (m_CIMLConfiguration.Training || m_CIMLConfiguration.Untrained)
                {
                    GUI.enabled = false;
                    enabled = false;
                } else
                {
                    enabled = true;
                }
                //Disable button if inputs don't match attached training examples node/s
                if (!m_CIMLConfiguration.matchLiveDataInputs || !m_CIMLConfiguration.matchVectorLength)
                {
                    Debug.Log("Number of live data nodes connected to input features do not match training examples live inputs input features");
                    GUI.enabled = false;
                }
                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Run")))
                {
                    m_CIMLConfiguration.ToggleRunning();
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
                    TooltipText = m_CIMLConfiguration.tooltips.BodyTooltip.Tips[2];
                }
                else
                {
                    TooltipText = m_CIMLConfiguration.tooltips.BodyTooltip.Error[1];
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

