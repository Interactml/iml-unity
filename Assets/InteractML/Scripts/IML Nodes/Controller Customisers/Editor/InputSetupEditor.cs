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

        private InputHelpers.Button selectedButton;

        

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
            // set gui changed to false so thar we know if they have selected a new input 
            GUI.changed = false;
            // set the device to user chosen input
            m_InputSetUp.device = (IMLInputDevices)EditorGUILayout.EnumPopup(m_InputSetUp.device);
            // if the user changes then alert the node 
            if (GUI.changed)
            {
                m_InputSetUp.OnInputDeviceChange();
            }
            // set gui changed back to false
            GUI.changed = false;
            // if the input is from vr controllers or hands show choice for training examples related buttons to be on the left hand or right hand or both
            if(m_InputSetUp.device == IMLInputDevices.VRControllers || m_InputSetUp.device == IMLInputDevices.VRHands)
            {
                m_InputSetUp.trainingHand = (IMLSides)EditorGUILayout.EnumPopup(m_InputSetUp.trainingHand);
            }
            // set button choice for delete last
            m_InputSetUp.deleteLastButtonNo = EditorGUILayout.Popup(m_InputSetUp.deleteLastButtonNo, m_InputSetUp.buttonOptions);
            // set trigger type for delete last
            m_InputSetUp.deleteLastTriggerType = (IMLTriggerTypes)EditorGUILayout.EnumPopup(m_InputSetUp.deleteLastTriggerType);
            // set choice for delete all
            m_InputSetUp.deleteAllButtonNo = EditorGUILayout.Popup(m_InputSetUp.deleteAllButtonNo, m_InputSetUp.buttonOptions);
            // set trigger type for delete all
            m_InputSetUp.deleteAllTriggerType = (IMLTriggerTypes)EditorGUILayout.EnumPopup(m_InputSetUp.deleteAllTriggerType);
            // set choice for toggle record 
            m_InputSetUp.toggleRecordButtonNo = EditorGUILayout.Popup(m_InputSetUp.toggleRecordButtonNo, m_InputSetUp.buttonOptions);
            // set trigger type for toggle record
            m_InputSetUp.toggleRecordTriggerType = (IMLTriggerTypes)EditorGUILayout.EnumPopup(m_InputSetUp.toggleRecordTriggerType);
            // if the input is from vr controllers or hands show choice for machine learning system related buttons to be on the left hand or right hand or both
            if (m_InputSetUp.device == IMLInputDevices.VRControllers || m_InputSetUp.device == IMLInputDevices.VRHands)
            {
                m_InputSetUp.mlsHand = (IMLSides)EditorGUILayout.EnumPopup(m_InputSetUp.mlsHand);
            }
            // set button choice for train
            m_InputSetUp.trainButtonNo = EditorGUILayout.Popup(m_InputSetUp.trainButtonNo, m_InputSetUp.buttonOptions);
            // set trigger type for train
            m_InputSetUp.trainTriggerType = (IMLTriggerTypes)EditorGUILayout.EnumPopup(m_InputSetUp.trainTriggerType);
            // set button choice for toggle run 
            m_InputSetUp.toggleRunButtonNo = EditorGUILayout.Popup(m_InputSetUp.toggleRunButtonNo, m_InputSetUp.buttonOptions);
            // set trigger type for delete last
            m_InputSetUp.toggleRunTriggerType= (IMLTriggerTypes)EditorGUILayout.EnumPopup(m_InputSetUp.toggleRunTriggerType);




        }


    }
}



