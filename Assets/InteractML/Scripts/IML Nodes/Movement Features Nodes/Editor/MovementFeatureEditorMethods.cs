using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.MovementFeatures
{
    public static class MovementFeatureEditorMethods
    {
        /// <summary>
        /// Draws toggle, feature label and value of input data for feature extractor and data type nodes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="node editor"></param>
        /// <param name="int number of features"></param>
        /// <param name="toggle GUIStyle"></param>
        public static void DrawFeatureValueToggleAndLabel(this IMLNodeEditor nodeEditor, BaseMovementFeatureNode node)
        {
            GUILayout.Space(10);

            if (node.FeatureValues.Values.Length == node.ToggleSwitches.Length)
            {
                // for each of the features in the data type
                for (int i = 0; i < node.FeatureValues.Values.Length; i++)
                {
                    // Draw each feature on a single line
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(nodeEditor.bodySpace);

                    //draw toggle
                    node.ToggleSwitches[i] = EditorGUILayout.Toggle(node.ToggleSwitches[i], IMLNodeEditorMethods.DataInToggle(nodeEditor, node.FeatureValueReceivingData[i]));

                    //draw label
                    EditorGUILayout.LabelField(nodeEditor.feature_labels[i] + System.Math.Round(node.FeatureValues.Values[i], 3).ToString(), nodeEditor.m_NodeSkin.GetStyle("Node Body Label Axis"));

                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
        }


        /// <summary>
        /// Draws toggle, feature label and value of input data for feature extractor and data type nodes
        /// </summary>
        /// <param name="node"></param>
        /// <param name="node editor"></param>
        /// <param name="int number of features"></param>
        /// <param name="toggle GUIStyle"></param>
        public static void DrawFeatureValueToggleAndLabelDynamic(this IMLNodeEditor nodeEditor, BaseMovementFeatureNode node)
        {
            // check number of feature values sent as parameter matches the amount of feature values in the node    
            if (node.ToggleSwitches.Length == node.FeatureValues.Values.Length)
            {
                // for each of the features in the data type
                for (int i = 0; i < node.FeatureValues.Values.Length; i++)
                {
                    // Draw each feature on a single line
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(nodeEditor.bodySpace);

                    //draw toggle
                    node.ToggleSwitches[i] = EditorGUILayout.Toggle(node.ToggleSwitches[i], IMLNodeEditorMethods.DataInToggle(nodeEditor, node.FeatureValueReceivingData[i]));

                    //draw label
                    EditorGUILayout.LabelField(System.Math.Round(node.FeatureValues.Values[i], 3).ToString(), nodeEditor.m_NodeSkin.GetStyle("Node Body Label Axis"));

                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
            else
            {
                Debug.Log("Number of feature values sent as parameter does not match the amount of feature values in the node");
            }
            
        }

        /// <summary>
        /// Draws toggle to choose whether extractor is to use local space or not
        /// </summary>
        /// <param name="node editor"></param>
        /// <param name="int number of features"></param>
        /// <return>localspace flag</return>
        public static bool DrawLocalSpaceToggle(this IMLNodeEditor nodeEditor, bool localSpace)
        {
            // Draw toggle and label on a single line
            GUILayout.BeginHorizontal();

            // line up spacing from side
            GUILayout.Space(25);

            // draw toggle
            localSpace = EditorGUILayout.Toggle(localSpace, nodeEditor.m_NodeSkin.GetStyle("Local Space Toggle"));
            
            // add space between label and toggle
            //GUILayout.Space(15);

            // draw label
            EditorGUILayout.LabelField("Use local space for transform", nodeEditor.m_NodeSkin.GetStyle("Node Local Space Label"));
            
            GUILayout.EndHorizontal();

            return localSpace;
        }
        
    }
}
