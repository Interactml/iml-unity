using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif
using InteractML.DataTypeNodes;
using InteractML.FeatureExtractors;

namespace InteractML
{
    public static class IMLNodeEditorMethods
    {
        /// <summary>
        /// Draws toggle, feature label and value of input data for data type node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="node editor"></param>
        /// <param name="toggle GUIStyle"></param>
        public static void DrawFeatureValueToggleAndLabel<T>(this BaseDataTypeNode<T> node, IMLNodeEditor nodeEditor, GUIStyle toggle_style)
        {
            // for each of the features in the data type
            for (int i = 0; i < node.FeatureValues.Values.Length; i++)
            {
                // Draw each feature on a single line
                GUILayout.BeginHorizontal();
                GUILayout.Space(nodeEditor.bodySpace);
                
                //draw toggle
                node.ToggleSwitches[i] = EditorGUILayout.Toggle(node.ToggleSwitches[i], toggle_style);
                
                //draw label
                EditorGUILayout.LabelField(nodeEditor.feature_labels[i] + System.Math.Round(node.FeatureValues.Values[i], 3).ToString(), nodeEditor.m_NodeSkin.GetStyle("Node Body Label Axis"));
                
                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }

        /// <summary>
        /// Draws toggle, feature label and value of input data for extractor type node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="node editor"></param>
        /// <param name="toggle GUIStyle"></param>
        public static void DrawFeatureValueToggleAndLabel(this BaseExtractorNode node, IMLNodeEditor nodeEditor, GUIStyle toggle_style)
        {
            // for each of the features in the data type
            for (int i = 0; i < node.FeatureValues.Values.Length; i++)
            {
                // Draw each feature on a single line
                GUILayout.BeginHorizontal();
                GUILayout.Space(nodeEditor.bodySpace);

                //draw toggle
                node.ToggleSwitches[i] = EditorGUILayout.Toggle(node.ToggleSwitches[i], toggle_style);

                //draw label
                EditorGUILayout.LabelField(nodeEditor.feature_labels[i] + System.Math.Round(node.FeatureValues.Values[i], 3).ToString(), nodeEditor.m_NodeSkin.GetStyle("Node Body Label Axis"));

                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }
    }
}
