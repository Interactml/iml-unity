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
            ShowDeviceOptions();
            ShowHandChoice(m_InputSetUp.trainingHand);
            ShowButtonChoice(m_InputSetUp.DeleteLast);
            ShowButtonChoice(m_InputSetUp.DeleteAll);
            ShowButtonChoice(m_InputSetUp.ToggleRecord);
            ShowHandChoice(m_InputSetUp.mlsHand);
            ShowButtonChoice(m_InputSetUp.Train);
            ShowButtonChoice(m_InputSetUp.ToggleRun);
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

        private void ShowHandChoice(IMLSides side)
        {
            // if the input is from vr controllers or hands show choice for training examples related buttons to be on the left hand or right hand or both
            if (m_InputSetUp.device == IMLInputDevices.VRControllers || m_InputSetUp.device == IMLInputDevices.VRHands)
            {
                m_InputSetUp.trainingHand = (IMLSides)EditorGUILayout.EnumPopup(m_InputSetUp.trainingHand);
            }
        }

        private void ShowButtonChoice(InputHandler handler)
        {
            GUI.changed = false;
            // set button choice for delete last
            handler.buttonNo = EditorGUILayout.Popup(handler.buttonNo, m_InputSetUp.buttonOptions);
            if (GUI.changed)
            {
                handler.SetButton();
            }
            handler.triggerType = (IMLTriggerTypes)EditorGUILayout.EnumPopup(handler.triggerType);

        }

    }
}



