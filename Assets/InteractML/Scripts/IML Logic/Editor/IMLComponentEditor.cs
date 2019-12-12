using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InteractML
{
    [CustomEditor(typeof(IMLComponent))]
    [CanEditMultipleObjects]
    public class IMLComponentEditor : Editor
    {
        IMLComponent imlComponent;

        private bool m_ShowIMLControllerOutputsDropdown = true;
        private bool[] m_ShowSubIMLCtrlrOutputs;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            imlComponent = target as IMLComponent;

            // LIST OF IML CONTROLLER OUTPUTS
            ShowIMLControllerOutputs();

            // BUTTON GOBJECTS UPDATE
            if (GUILayout.Button("Update IML Controller"))
            {
                imlComponent.UpdateGameObjectsInIMLController();
                imlComponent.GetAllNodes();
            }
        }

        private void ShowIMLControllerOutputs()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("IML Controller Outputs", EditorStyles.boldLabel);

            m_ShowIMLControllerOutputsDropdown = EditorGUILayout.Foldout(m_ShowIMLControllerOutputsDropdown, "IML Controller Outputs");

            if (m_ShowIMLControllerOutputsDropdown)
            {
                // If there are outputs...
                if (imlComponent.IMLControllerOutputs != null)
                {
                    if (m_ShowSubIMLCtrlrOutputs == null)
                        m_ShowSubIMLCtrlrOutputs = new bool[0];

                    // Make sure dropdown array is same size
                    if (m_ShowSubIMLCtrlrOutputs.Length != imlComponent.IMLControllerOutputs.Count)
                    {
                        m_ShowSubIMLCtrlrOutputs = new bool[imlComponent.IMLControllerOutputs.Count];
                    }

                    // Go through all the outputs
                    for (int i = 0; i < imlComponent.IMLControllerOutputs.Count; i++)
                    {
                        var output = imlComponent.IMLControllerOutputs[i];
                        EditorGUI.indentLevel++;

                        m_ShowSubIMLCtrlrOutputs[i] = true;

                        m_ShowSubIMLCtrlrOutputs[i] = EditorGUILayout.Foldout(m_ShowSubIMLCtrlrOutputs[i], "IML Controller Output " + i);

                        if (m_ShowSubIMLCtrlrOutputs[i])
                        {
                            for (int j = 0; j < output.Length; j++)
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.DoubleField("Output " + j + " : ", output[j]);
                                EditorGUI.indentLevel--;
                            }
                        }

                        EditorGUI.indentLevel--;

                    }
                }
                // If there are no outputs
                else
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.LabelField("List is empty");

                    EditorGUI.indentLevel--;
                }

            }

        }


    }

}
