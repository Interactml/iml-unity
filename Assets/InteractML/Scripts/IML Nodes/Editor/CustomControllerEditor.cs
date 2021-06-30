using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.ControllerCustomisers
{
    /// <summary>
    /// 
    /// </summary>
    [CustomNodeEditor(typeof(CustomController))]
    public class CustomControllerEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private CustomController m_controller;



        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
           m_controller = (target as CustomController);

            NodeName = m_controller.name;

            // Initialise node height
            m_BodyRect.height = 100;
            nodeSpace = -10;

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("inputValue", "Bool\nOut");

        }

        /// <summary>
        /// Draws node specific body fields
        /// </summary>
        protected override void ShowBodyFields()
        {
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            // show hand choice method from inputsetupeditor using m_controller.hand
            GUI.changed = false;
            GUILayout.Space(10);
            GUILayout.Label("Button", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(100));
            // set button choice for delete last
            m_controller.inputNo = EditorGUILayout.Popup(m_controller.inputNo, m_controller.buttonOptions);
            //Event.current.type == EventType.Repaint
            if (GUI.changed)
            {
                m_controller.OnButtonChange();
                //mark as changed
                EditorUtility.SetDirty(m_controller);
            }
            GUI.changed = false;
            GUILayout.Space(10);
            m_controller.trigger = (IMLTriggerTypes)EditorGUILayout.EnumPopup(m_controller.trigger);
            if (GUI.changed)
            {
                m_controller.OnTriggerChange();
                EditorUtility.SetDirty(m_controller);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(100);
            //show button method from InputSetUpEditor using m_controller.triggerButton*/

        }


    }
}



