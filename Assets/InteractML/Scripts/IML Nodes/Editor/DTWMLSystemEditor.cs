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
    public class DTWMLSystemEditor : MLSystemEditor
    {

        #region Variables 

        /// <summary>
        /// Bool for add/remove output
        /// </summary>
        private bool m_AddOutput;
        private bool m_RemoveOutput;
        private bool m_DTWSwitch;

        

        #endregion


       /* protected override void TrainModelButton()
        {
            // Only run button logic when rapidlib reference not null and training examples are not null
            if (m_MLSystem.Model != null && m_MLSystem.TotalNumTrainingData > 0)
            {

                string nameButton = "";

                if (m_MLSystem.Training)
                    nameButton = "STOP Training Model";
                if (m_MLSystem.Trained)
                    nameButton = "Trained (" + m_MLSystem.NumExamplesTrainedOn + " Examples)";
                else
                    nameButton = "Train Model";

                // Disable button if model is Running OR Trainig 
                if (m_MLSystem.Running || m_MLSystem.Training)
                    GUI.enabled = false;



                if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Train")))
                {
                    IMLEventDispatcher.TrainMLSCallback(m_MLSystem.id);
                  
                }
                // Always enable it back at the end
                GUI.enabled = true;

            }
            // If rapidlib reference is null we draw a disabled button
            else
            {

                GUI.enabled = false;
                if (m_MLSystem.TotalNumTrainingData == 0)
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
                    TooltipText = m_MLSystem.tooltips.BodyTooltip.Tips[1];
                }
                else
                {
                    TooltipText = m_MLSystem.tooltips.BodyTooltip.Error[0];
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

        }*/

        /*protected override void RunModelButton()
        {
            bool enabled = false;
            if (m_MLSystem.Model != null)
            {
                string nameButton = "";

                if (m_MLSystem.Running)
                {
                    nameButton = "STOP";
                }
                else
                {
                    nameButton = "Populate";
                }

                // Disable button if model is Trainig OR Untrained
                if (m_MLSystem.Training || m_MLSystem.Untrained)
                    GUI.enabled = false;

                //Disable button if inputs don't match attached training examples node/s
                if (!m_MLSystem.matchLiveDataInputs || !m_MLSystem.matchVectorLength)
                {
                    Debug.Log("Number of live data nodes connected to input features do not match training examples live inputs input features");
                    GUI.enabled = false;
                }

                if (GUILayout.Button(nameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Run")))
                {
                    IMLEventDispatcher.ToggleRunCallback(m_MLSystem.id);
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
                    TooltipText = m_MLSystem.tooltips.BodyTooltip.Tips[2];
                }
                else
                {
                    TooltipText = m_MLSystem.tooltips.BodyTooltip.Error[1];
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

        }*/

    }
}


