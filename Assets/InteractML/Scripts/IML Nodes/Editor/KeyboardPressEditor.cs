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
    [CustomNodeEditor(typeof(KeyboardPress))]
    public class KeyboardPressEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private KeyboardPress m_controller;

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
            m_controller = (target as KeyboardPress);

            NodeName = m_controller.name;

            // Initialise node height
            m_BodyRect.height = 100;
            nodeSpace = 80;

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("m_inputEvent", "Bool\nOut");
            Debug.Log("here" + m_controller.id);
            base.OnCreate();

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
            GUILayout.BeginHorizontal();
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



