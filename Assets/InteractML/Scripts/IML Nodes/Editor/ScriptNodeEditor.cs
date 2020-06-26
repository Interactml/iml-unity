using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeEditor(typeof(ScriptNode))]
    public class ScriptNodeEditor : IMLNodeEditor
    {
        #region Variables
        private ScriptNode m_SriptNode;
        #endregion


        #region XNode GUI Messages

        public override void OnHeaderGUI()
        {
            // Get reference to our node
            if (m_SriptNode == null)
                m_SriptNode = target as ScriptNode;
            
            // Attempt to get name of script to change node name
            if (m_SriptNode.GetScript() != null && string.IsNullOrEmpty(NodeName))
                NodeName = $"{m_SriptNode.GetScript().GetType().ToString()} (Script)";

            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            bodyheight = 100;            
            base.OnBodyGUI();
        }

        #endregion


    }


}
