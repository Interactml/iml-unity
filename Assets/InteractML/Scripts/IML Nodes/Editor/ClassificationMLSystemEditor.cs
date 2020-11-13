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
    [CustomNodeEditor(typeof(ClassificationMLSystem))]
    public class ClassificationMLSystemEditor : IMLNodeEditor
    {
        
        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ClassificationMLSystem m_ClassificationMLSystem;
        #endregion


        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_ClassificationMLSystem = (target as ClassificationMLSystem);
            nodeSpace = 20;
            string arrayNo = "";
            if (m_ClassificationMLSystem.numberInComponentList != -1)
                arrayNo = m_ClassificationMLSystem.numberInComponentList.ToString();
            NodeName = "MACHINE LEARNING SYSTEM " + arrayNo;
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            InputPortsNamesOverride = new Dictionary<string, string>();
            InputPortsNamesOverride.Add("IMLTrainingExamplesNodes", "Recorded Data In");
            InputPortsNamesOverride.Add("InputFeatures", "Live Data In");
            base.nodeTips = m_ClassificationMLSystem.tooltips;
            m_BodyRect.height = 330;
            base.OnBodyGUI();
        }

        protected override void ShowBodyFields()
        {
            ShowTrainingIcon("Classification");
            ShowButtons(m_ClassificationMLSystem);
            GUILayout.Space(m_BodyRect.height + HeaderRect.height - 50);
            ShowRunOnAwakeToggle(m_ClassificationMLSystem as MLSystem);
            GUILayout.Space(20);
            // if there is an error show the correct warning
            if (m_ClassificationMLSystem.error)
            {
                nodeSpace = 40;
                m_BodyRect.height = m_BodyRect.height + HeaderRect.height + 60;
                ShowWarning(m_ClassificationMLSystem.warning);
            }
        }




        protected override void TrainModelButton()
        {
            
            // Only run button logic when rapidlib reference not null and training examples are not null
            if (m_ClassificationMLSystem.Model != null && m_ClassificationMLSystem.TotalNumTrainingData > 0)
            {
                string nameButton = "";

                if (m_ClassificationMLSystem.Training)
                    nameButton = "STOP Training Model";
                if (m_ClassificationMLSystem.Trained)
                    nameButton = "Trained (" + m_ClassificationMLSystem.NumExamplesTrainedOn + " Examples)";
                else
                    nameButton = "Train Model";


                // Disable button if model is Running OR Training 
                if (m_ClassificationMLSystem.Running || m_ClassificationMLSystem.Training)
                    GUI.enabled = false;
                
                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Train")))
                {
                    IMLEventDispatcher.TrainMLSCallback(m_ClassificationMLSystem.id);
                    // old training model can delete when tested
                   /* if (m_CIMLConfiguration.TrainModel())
                    {
                        // Save model if succesfully trained
                        m_CIMLConfiguration.SaveModelToDisk();
                    }*/
                }
                // Always enable it back at the end
                GUI.enabled = true;

                

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {

                GUI.enabled = false;
                if (m_ClassificationMLSystem.TotalNumTrainingData == 0)
                {
                    //EditorGUILayout.HelpBox("There are no training examples", MessageType.Error);
                }
                if (GUILayout.Button("Train Model", m_NodeSkin.GetStyle("Train")))
                {
                    //m_TrainingExamplesNode.ToggleCollectExamples();
                }
                //GUI.enabled = true;

            }
            // if teh rect contains the mouse and the toolstips have loaded show tooltips 
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && m_ClassificationMLSystem.tooltips.BodyTooltip.Tips.Length != 0)
            {
                buttonTipHelper = true;
                if (GUI.enabled)
                {
                    TooltipText = m_ClassificationMLSystem.tooltips.BodyTooltip.Tips[1];

                } else
                {
                    TooltipText = m_ClassificationMLSystem.tooltips.BodyTooltip.Error[0];
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
            if (m_ClassificationMLSystem.Model != null)
            {
                string nameButton = "";

                if (m_ClassificationMLSystem.Running)
                {
                    nameButton = "STOP";
                }
                else
                {
                    nameButton = "RUN";
                }

                // Disable button if model is Training OR Untrained
                if (m_ClassificationMLSystem.Training || m_ClassificationMLSystem.Untrained)
                {
                    GUI.enabled = false;
                    enabled = false;
                } else
                {
                    enabled = true;
                }
                //Disable button if inputs don't match attached training examples node/s
                if (!m_ClassificationMLSystem.matchLiveDataInputs || !m_ClassificationMLSystem.matchVectorLength)
                {
                    // commented as firing all the time can uncomment for future debugging
                    //Debug.Log("Number of live data nodes connected to input features do not match training examples live inputs input features");
                    GUI.enabled = false;
                }
                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Run")))
                {
                    if (m_ClassificationMLSystem.Running)
                    {
                        IMLEventDispatcher.StopRunCallback(m_ClassificationMLSystem.id);
                    } else
                    {
                        IMLEventDispatcher.StartRunCallback(m_ClassificationMLSystem.id);
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
            // if the rect contains the mouse and the toltips have loaded 
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && m_ClassificationMLSystem.tooltips.BodyTooltip.Tips.Length != 0)
            {
                buttonTipHelper = true;
                if (enabled)
                {
                    TooltipText = m_ClassificationMLSystem.tooltips.BodyTooltip.Tips[2];
                }
                else
                {
                    TooltipText = m_ClassificationMLSystem.tooltips.BodyTooltip.Error[1];
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

