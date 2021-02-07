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
    [CustomNodeEditor(typeof(InputSetUp))]
    public class InputSetUpEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private InputSetUp m_InputSetUp;

        /// <summary>
        /// Position of scroll for dropdown
        /// </summary>
        protected Vector2 m_ScrollPos;

        IMLSides trainingSide;
        IMLSides mlsSide;

        int deleteLastButtonNo;
        IMLTriggerTypes deleteLastButtonTT;
        int deleteAllButtonNo;
        IMLTriggerTypes deleteAllButtonTT;
        int toggleRecordButtonNo;
        IMLTriggerTypes toggleRecordButtonTT;
        int trainButtonNo;
        IMLTriggerTypes trainButtonTT;
        int toggleRunButtonNo;
        IMLTriggerTypes toggleRunButtonTT;
       
        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_InputSetUp = (target as InputSetUp);

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
                trainingSide = (IMLSides)EditorGUILayout.EnumPopup(trainingSide);
            }
            if (GUI.changed)
            {
                m_InputSetUp.OnHandChange(trainingSide);
            }
            // choose delete last button
            GUI.changed = false;
            // set button choice for delete last
            // delete last
            ShowButtonChoice("deleteLast", deleteLastButtonNo, out deleteLastButtonNo, deleteLastButtonTT, out deleteLastButtonTT);
            // choose delete all button 
            ShowButtonChoice("deleteAll", deleteAllButtonNo, out deleteAllButtonNo, deleteAllButtonTT, out deleteAllButtonTT);
            // choose toggle record button 
            ShowButtonChoice("toggleRecord", toggleRecordButtonNo, out toggleRecordButtonNo, toggleRecordButtonTT, out toggleRecordButtonTT);
            // sets the controller side for the training
            GUI.changed = false;
            // if the input is from vr controllers or hands show choice for training examples related buttons to be on the left hand or right hand or both
            if (m_InputSetUp.device == IMLInputDevices.VRControllers || m_InputSetUp.device == IMLInputDevices.VRHands)
            {
                mlsSide = (IMLSides)EditorGUILayout.EnumPopup(mlsSide);
            }
            if (GUI.changed)
            {
                m_InputSetUp.OnHandChange(trainingSide);
            }
            // sets the button for training 
            ShowButtonChoice("train", trainButtonNo, out trainButtonNo, trainButtonTT, out trainButtonTT);
            // sets the button for running
            ShowButtonChoice("toggleRun", toggleRunButtonNo, out toggleRunButtonNo, toggleRunButtonTT, out toggleRunButtonTT);
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
            if (GUI.changed)
            {
                m_InputSetUp.OnButtonChange(handler, buttonNumOut);
            }
            triggerTypeOut = (IMLTriggerTypes)EditorGUILayout.EnumPopup(triggerType);
            if (GUI.changed)
            {
                m_InputSetUp.OnTriggerChange(handler, triggerType);
            }

        }

    }
}



