using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.ControllerCustomisers
{
    /// <summary>
    /// 
    /// </summary>
    [CustomNodeEditor(typeof(VRTrigger))]
    public class VRTriggerEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private VRTrigger m_VRTrigger;

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
            m_VRTrigger = (target as VRTrigger);

            // Initialise node name
            NodeName = "VR TRIGGER";

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("ControllerOutput", "Controller\nOut");

        }

        /// <summary>
        /// Draws node specific body fields
        /// </summary>
        protected override void ShowBodyFields()
        {
            GUI.changed = false;
            m_VRTrigger.hand = (IMLSides)EditorGUILayout.EnumPopup(m_VRTrigger.hand);
            
            if (GUI.changed)
            {
                m_VRTrigger.OnHandChange();
            }
            // show hand choice method from inputsetupeditor using m_VRTrigger.hand
            GUI.changed = false;
            // set button choice for delete last
            m_VRTrigger.inputs = (IMLControllerInputs)EditorGUILayout.EnumPopup(m_VRTrigger.inputs);
            //Event.current.type == EventType.Repaint
            if (GUI.changed)
            {
                m_VRTrigger.OnButtonChange();
                //mark as changed
                EditorUtility.SetDirty(m_VRTrigger);
            }
            m_VRTrigger.triggerType = (IMLTriggerTypes)EditorGUILayout.EnumPopup(m_VRTrigger.triggerType);
            if (GUI.changed)
            {
                m_VRTrigger.OnTriggerChange();
                EditorUtility.SetDirty(m_VRTrigger);
            }
            //show button method from InputSetUpEditor using m_VRTrigger.triggerButton*/

        }


    }
}



