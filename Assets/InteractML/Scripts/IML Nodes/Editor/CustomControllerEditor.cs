using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableMethods;
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
        /// Timer for toggle visualizaton
        /// </summary>
        private Timer m_ToggleTimer;
        /// <summary>
        /// How long will the visual toggle remain active?
        /// </summary>
        private float m_TimeToggleTrue = 0.3f;
        /// <summary>
        /// Flag to keep track of when input was true
        /// </summary>
        private bool m_InputDetected;

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

            if (m_controller.inputValue) m_InputDetected = true;
            //draw toggle and show active for more than a frame (for visualitation purposes)
            if (m_InputDetected)
            {
                if (m_ToggleTimer == null) m_ToggleTimer = new Timer();
                // Take a some time until we set the flag to false (shows true toggle for a bit)
                if (m_ToggleTimer.GenericCountDown(m_TimeToggleTrue)) m_InputDetected = false;              
            }
            // Draw toggle
            EditorGUILayout.Toggle(m_controller.InputChange, IMLNodeEditorMethods.DataInToggle(this, m_InputDetected));

            // Label
            GUILayout.Label("Button", m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(50));
            // set button choice for delete last
            m_controller.inputNo = EditorGUILayout.Popup(m_controller.inputNo, m_controller.buttonOptions);
            //Event.current.type == EventType.Repaint
                m_controller.OnButtonChange();
                //mark as changed
                EditorUtility.SetDirty(m_controller);
            GUI.changed = false;
            GUILayout.Space(10);
            m_controller.trigger = (IMLTriggerTypes)EditorGUILayout.EnumPopup(m_controller.trigger);
            if (GUI.changed)
            {
                m_controller.OnTriggerChange();
                EditorUtility.SetDirty(m_controller);
            }
            GUILayout.EndHorizontal();

            //// Show visualization of when the button is pressed
            //GUILayout.BeginHorizontal();
            //GUILayout.Space(10);
            ////draw toggle
            //EditorGUILayout.Toggle(m_controller.inputValue, IMLNodeEditorMethods.DataInToggle(this, m_controller.inputValue));
            ////draw label
            //EditorGUILayout.LabelField($"Bool out: {m_controller.inputValue}", this.m_NodeSkin.GetStyle("Node Body Label Axis"));
            //GUILayout.EndHorizontal();
            //EditorGUILayout.Space();

            GUILayout.Space(100);
            //show button method from InputSetUpEditor using m_controller.triggerButton*/
        }


    }
}



