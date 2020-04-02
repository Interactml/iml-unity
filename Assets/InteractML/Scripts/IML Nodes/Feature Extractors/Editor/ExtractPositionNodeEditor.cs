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
        private ExtractPosition m_ExtractPositionNode;

        private static GUIStyle editorLabelStyle;

        public override void OnBodyGUI()
        {
            // Get reference to the current node
            m_ExtractPositionNode = (target as ExtractPosition);

            if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
            EditorStyles.label.normal.textColor = Color.white;


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



        }

        #region Methods

        private void ShowExtractPositionNodePorts()
        {
            NodeEditorGUILayout.PortPair(m_ExtractPositionNode.GetInputPort("GameObjectDataIn"), m_ExtractPositionNode.GetOutputPort("LiveDataOut")); 
        }

        private void ShowExtractedPositionValues()
        {

            EditorGUILayout.LabelField(" x: " + m_ExtractPositionNode.FeatureValues.Values[0].ToString());
            EditorGUILayout.LabelField(" y: " + m_ExtractPositionNode.FeatureValues.Values[1].ToString());
            EditorGUILayout.LabelField(" z: " + m_ExtractPositionNode.FeatureValues.Values[2].ToString());
           
        }

        private void ShowLocalSpaceToggle()
        {
            m_ExtractPositionNode.LocalSpace = EditorGUILayout.ToggleLeft("Use local space for transform", m_ExtractPositionNode.LocalSpace);
        }

        #endregion

    }

}
