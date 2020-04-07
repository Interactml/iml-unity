using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractPosition))]
    public class ExtractPositionNodeEditor : NodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractPosition m_ExtractPosition;

        private static GUIStyle editorLabelStyle;

        public override void OnBodyGUI()
        {
            // Get reference to the current node
            m_ExtractPosition = (target as ExtractPosition);

            if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
            EditorStyles.label.normal.textColor = Color.white;
            EditorStyles.label.wordWrap = true;

            //base.OnBodyGUI();

            // Draw the ports
            ShowExtractPositionNodePorts();

            // Show extracted position values
            EditorGUILayout.Space();
            ShowExtractedPositionValues();

            // Show local space toggle
            EditorGUILayout.Space();
            ShowLocalSpaceToggle();


            EditorStyles.label.normal = editorLabelStyle.normal;
            EditorStyles.label.wordWrap = false;


        }

        #region Methods

        private void ShowExtractPositionNodePorts()
        {
            
            NodeEditorGUILayout.PortPair(m_ExtractPosition.GetInputPort("GameObjectDataIn"), m_ExtractPosition.GetOutputPort("LiveDataOut"));
            
        }

        private void ShowExtractedPositionValues()
        {

            EditorGUILayout.LabelField(" x: " + m_ExtractPosition.FeatureValues.Values[0].ToString());
            EditorGUILayout.LabelField(" y: " + m_ExtractPosition.FeatureValues.Values[1].ToString());
            EditorGUILayout.LabelField(" z: " + m_ExtractPosition.FeatureValues.Values[2].ToString());
           
        }

        private void ShowLocalSpaceToggle()
        {
            m_ExtractPosition.LocalSpace = EditorGUILayout.ToggleLeft("Use local space for transform", m_ExtractPosition.LocalSpace);
        }

        #endregion

    }

}
