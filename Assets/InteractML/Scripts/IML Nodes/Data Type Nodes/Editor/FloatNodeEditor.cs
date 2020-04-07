using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(FloatNode))]
    public class FloatNodeEditor : NodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private FloatNode m_FloatNode;

        private static GUIStyle editorLabelStyle;

        public override void OnBodyGUI()
        {
            // Get reference to the current node
            m_FloatNode = (target as FloatNode);

            if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
            EditorStyles.label.normal.textColor = Color.white;

            //Show float value
            ShowFloatValue();

            EditorStyles.label.normal = editorLabelStyle.normal;

        }

        private void ShowFloatValue()
        {
            EditorGUILayout.FloatField(m_FloatNode.FloatToOutput);          
        }
    }

}
