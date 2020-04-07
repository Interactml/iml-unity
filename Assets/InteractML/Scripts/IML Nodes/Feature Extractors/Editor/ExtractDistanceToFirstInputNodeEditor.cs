using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractDistanceToFirstInput))]
    public class ExtractDistanceToFirstInputNodeEditor : NodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractDistanceToFirstInput m_ExtractDistanceToFirstInput;

        private static GUIStyle editorLabelStyle;

        public override void OnBodyGUI()
        {
            // Get reference to the current node
            m_ExtractDistanceToFirstInput = (target as ExtractDistanceToFirstInput);

            if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
            EditorStyles.label.normal.textColor = Color.white;
            EditorStyles.label.wordWrap = true;

            // Draw the ports
            ShowDistanceBetweenInputsNodePorts();

            // Show extracted position values
            EditorGUILayout.Space();
            ShowDistanceBetweenInputsValue();

            EditorStyles.label.normal = editorLabelStyle.normal;
            EditorStyles.label.wordWrap = false;

        }

        #region Methods

        private void ShowDistanceBetweenInputsNodePorts()
        {
            NodeEditorGUILayout.PortPair(m_ExtractDistanceToFirstInput.GetInputPort("FirstInput"), m_ExtractDistanceToFirstInput.GetOutputPort("DistanceBetweenInputs"));
            NodeEditorGUILayout.PortField(m_ExtractDistanceToFirstInput.GetInputPort("SecondInput"));
        }

        private void ShowDistanceBetweenInputsValue()
        {
            if (m_ExtractDistanceToFirstInput.FeatureValues.Values == null || m_ExtractDistanceToFirstInput.FeatureValues.Values.Length == 0)
            {
                EditorGUILayout.FloatField("Distance between inputs: ", 0);
            }
            else
            {
                // Go through the list output distances
                for (int i = 0; i < m_ExtractDistanceToFirstInput.FeatureValues.Values.Length; i++)
                {
                    EditorGUILayout.FloatField("Distance between inputs: ", m_ExtractDistanceToFirstInput.FeatureValues.Values[i]);
                }
            }


        }

        #endregion
    }

}
