using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractVector4))]
    public class ExtractVector4NodeEditor : NodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractVector4 m_ExtractVector4;

        private static GUIStyle editorLabelStyle;

        public override void OnBodyGUI()
        {
            // Get reference to the current node
            m_ExtractVector4 = (target as ExtractVector4);

            if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
            EditorStyles.label.normal.textColor = Color.white;
            base.OnBodyGUI();
            EditorStyles.label.normal = editorLabelStyle.normal;

        }
    }

}
