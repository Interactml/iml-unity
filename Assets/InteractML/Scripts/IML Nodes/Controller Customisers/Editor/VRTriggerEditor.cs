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

            // Initialise node height
            m_BodyRect.height = 100;
            nodeSpace = 80;

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("m_inputEvent", "Controller\nOut");

        }

        /// <summary>
        /// Draws node specific body fields
        /// </summary>
        protected override void ShowBodyFields()
        {
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hand", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(100));
            GUI.changed = false;
            m_VRTrigger.hand = (IMLSides)EditorGUILayout.EnumPopup(m_VRTrigger.hand);
            GUILayout.EndHorizontal();
            if (GUI.changed)
            {
                m_VRTrigger.OnHandChange();
            }
            // show hand choice method from inputsetupeditor using m_VRTrigger.hand
            GUI.changed = false;
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Button", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(100));
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
            GUILayout.EndHorizontal();
            //show button method from InputSetUpEditor using m_VRTrigger.triggerButton*/

        }


    }
}



