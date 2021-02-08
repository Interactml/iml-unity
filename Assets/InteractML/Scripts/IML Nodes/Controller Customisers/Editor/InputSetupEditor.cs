using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
using InteractML;
using InteractML.ControllerCustomisers;

#endif

namespace InteractML.ControllerCustomisers
{
    /// <summary>
    /// 
    /// </summary>
    [CustomNodeEditor(typeof(InteractML.CustomControllers.InputSetUp))]
    public class InputSetUpEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private InteractML.CustomControllers.InputSetUp m_InputSetUp;

        /// <summary>
        /// Position of scroll for dropdown
        /// </summary>
        protected Vector2 m_ScrollPos;

        IMLSides trainingSide;
        IMLSides mlsSide;
       
        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_InputSetUp = (target as InteractML.CustomControllers.InputSetUp);

            // Initialise node name
            NodeName = "Input Set Up";
        }

        /// <summary>
        /// Draws node specific body fields
        /// </summary>
        protected override void ShowBodyFields()
        {
            // sets the device
            ShowDeviceOptions();
            //sets the controller side for the training examples whether left right or both 
            GUI.changed = false;
            // if the input is from vr controllers or hands show choice for training examples related buttons to be on the left hand or right hand or both
            if (m_InputSetUp.device == IMLInputDevices.VRControllers || m_InputSetUp.device == IMLInputDevices.VRHands)
            {
                m_InputSetUp.trainingHand = (IMLSides)EditorGUILayout.EnumPopup(m_InputSetUp.trainingHand);
            }
            if (GUI.changed)
            {
                m_InputSetUp.OnHandChange(m_InputSetUp.trainingHand, "trainingSide");
                EditorUtility.SetDirty(m_InputSetUp);
            }
            // choose delete last button
            GUI.changed = false;
            // set button choice for delete last
            // delete last
            ShowButtonChoice(m_InputSetUp.DeleteLast.buttonName, m_InputSetUp.deleteLastButtonNo, out m_InputSetUp.deleteLastButtonNo, m_InputSetUp.deleteLastButtonTT, out m_InputSetUp.deleteLastButtonTT);
            // choose delete all button 
            ShowButtonChoice(m_InputSetUp.DeleteAll.buttonName, m_InputSetUp.deleteAllButtonNo, out m_InputSetUp.deleteAllButtonNo, m_InputSetUp.deleteAllButtonTT, out m_InputSetUp.deleteAllButtonTT);
            // choose toggle record button 
            ShowButtonChoice(m_InputSetUp.ToggleRecord.buttonName, m_InputSetUp.toggleRecordButtonNo, out m_InputSetUp.toggleRecordButtonNo, m_InputSetUp.toggleRecordButtonTT, out m_InputSetUp.toggleRecordButtonTT);
            // sets the controller side for the training
            GUI.changed = false;
            // if the input is from vr controllers or hands show choice for training examples related buttons to be on the left hand or right hand or both
            if (m_InputSetUp.device == IMLInputDevices.VRControllers || m_InputSetUp.device == IMLInputDevices.VRHands)
            {
                m_InputSetUp.mlsHand = (IMLSides)EditorGUILayout.EnumPopup(m_InputSetUp.mlsHand);
            }
            if (GUI.changed)
            {
                m_InputSetUp.OnHandChange(m_InputSetUp.mlsHand, "mlsSide");
                EditorUtility.SetDirty(m_InputSetUp);
            }
            // sets the button for training 
            ShowButtonChoice(m_InputSetUp.Train.buttonName, m_InputSetUp.trainButtonNo, out m_InputSetUp.trainButtonNo, m_InputSetUp.trainButtonTT, out m_InputSetUp.trainButtonTT);
            // sets the button for running
            ShowButtonChoice(m_InputSetUp.ToggleRun.buttonName, m_InputSetUp.toggleRunButtonNo, out m_InputSetUp.toggleRunButtonNo, m_InputSetUp.toggleRunButtonTT, out m_InputSetUp.toggleRunButtonTT);
        }

        private void ShowDeviceOptions()
        {
            // set gui changed to false so thar we know if they have selected a new input 
            GUI.changed = false;
            // set the device to user chosen input
            m_InputSetUp.device = (IMLInputDevices)EditorGUILayout.EnumPopup(m_InputSetUp.device);


            // if the user changes then alert the node 
            if (GUI.changed)
            {
                m_InputSetUp.OnInputDeviceChange();
            }
        }

        private void ShowButtonChoice(string handler, int buttonnum, out int buttonNumOut, IMLTriggerTypes triggerType, out IMLTriggerTypes triggerTypeOut)
        {
            GUI.changed = false;
            // set button choice for delete last
            buttonNumOut = EditorGUILayout.Popup(buttonnum, m_InputSetUp.buttonOptions);
            //Event.current.type == EventType.Repaint
            if (GUI.changed)
            {
                 m_InputSetUp.OnButtonChange(handler, buttonNumOut);
                //mark as changed
                EditorUtility.SetDirty(m_InputSetUp);
            }
            triggerTypeOut = (IMLTriggerTypes)EditorGUILayout.EnumPopup(triggerType);
            if (GUI.changed)
            {
                m_InputSetUp.OnTriggerChange(handler, triggerType);
            }

        }

    }
}



