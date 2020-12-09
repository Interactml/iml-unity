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
    /// Editor class drawing a IMLFloat Feature - receiving a float or drawing editable float field 
    /// </summary>
    [CustomNodeEditor(typeof(KeyboardPress))]
    public class KeyboardPressEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private KeyboardPress m_KeyboardPress;

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
            m_KeyboardPress = (target as KeyboardPress);

            // Initialise node name
            NodeName = "KEYBOARD PRESS";

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("ControllerOutput", "Controller\nOut");

        }

        /// <summary>
        /// Draws node specific body fields
        /// </summary>
        protected override void ShowBodyFields()
        {

        }


    }
}

