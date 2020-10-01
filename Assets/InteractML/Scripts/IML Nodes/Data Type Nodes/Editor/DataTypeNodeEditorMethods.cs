using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    /// </summary>
    /// Contains methods for data type node editor classes
    /// </summary>
    public static class DataTypeNodeEditorMethods
    {

        /// <summary>
        /// Draws editable float field with toggle for each feature
        /// </summary>
        /// <param name="data type node"></param>
        /// <param name="data type node editor"></param>
        /// <param name="GUIStyle of toggle"></param>
        public static void DrawEditableFieldsAndToggles<T>(this BaseDataTypeNode<T> node, IMLNodeEditor nodeEditor, GUIStyle toggleStyle)
        {
            // for each of the features in the data type
            for (int i = 0; i < node.FeatureValues.Values.Length; i++)
            {
                // Draw each feature on a single line
                GUILayout.BeginHorizontal();
                GUILayout.Space(nodeEditor.bodySpace);

                //draw toggle
                node.ToggleSwitches[i] = EditorGUILayout.Toggle(node.ToggleSwitches[i], toggleStyle, GUILayout.MaxWidth(40));
                
                //draw label
                EditorGUILayout.LabelField(nodeEditor.feature_labels[i], nodeEditor.m_NodeSkin.GetStyle("Node Body Label Axis"), GUILayout.MaxWidth(20));
                
                // draw editable float field for each feature
                node.UserInput.Values[i] = EditorGUILayout.FloatField(node.UserInput.Values[i], GUILayout.MaxWidth(60));
                
                GUILayout.EndHorizontal(); 
                EditorGUILayout.Space();

            }

        }
    }
}
