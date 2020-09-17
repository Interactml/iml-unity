using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableMethods;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeEditor(typeof(SeriesTrainingExamplesNode))]
    public class SeriesTrainingExamplesNodeEditor : TrainingExamplesNodeEditor
    {

        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private SeriesTrainingExamplesNode m_SeriesTrainingExamplesNode;

        #endregion

        public override void OnHeaderGUI()
        {
            baseNodeBodyHeight = 250;
            base.OnHeaderGUI();
            // Get reference to the current node
            m_SeriesTrainingExamplesNode = m_TrainingExamplesNode as SeriesTrainingExamplesNode;
            string arrayNo = "";
            if (m_SeriesTrainingExamplesNode.numberInComponentList != -1)
                arrayNo = m_SeriesTrainingExamplesNode.numberInComponentList.ToString();
            NodeName = "TEACH THE MACHINE " + arrayNo;
            NodeSubtitle = "DTW Training Examples";
        }

        /// <summary>
        /// Show the load, delete and record buttons
        /// </summary>
        protected override void ShowButtons()
        {
            int spacing = 75;
            GUILayout.Space(40);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing);
            string recordNameButton = ShowRecordExamplesButton();
            GUILayout.Space(spacing);
            ShowClearAllExamplesButton();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing-10);
            GUILayout.Label(recordNameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green"));
            GUILayout.Space(spacing-23);
            GUILayout.Label("delete all \n recordings", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button Pink"));
            GUILayout.EndHorizontal();

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing-15);
            GUILayout.Label("No of training pairs: " + m_SeriesTrainingExamplesNode.TrainingSeriesCollection.Count, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small"));
            GUILayout.EndHorizontal();

        }



        

        /// <summary>
        /// Shows a dropdown with the training examples series
        /// </summary>
        protected override void ShowTrainingExamplesDropdown()
        {
            if (m_ShowTrainingDataDropdown)
            {
                m_Dropdown.x = m_HelpRect.x;
                m_Dropdown.y = m_HelpRect.y + m_HelpRect.height;
                m_Dropdown.width = m_HelpRect.width;
                m_Dropdown.height = 200;
                GUI.DrawTexture(m_Dropdown, NodeColor);
                GUILayout.BeginArea(m_Dropdown);

                EditorGUI.indentLevel++;
                if (ReusableMethods.Lists.IsNullOrEmpty(ref m_SeriesTrainingExamplesNode.TrainingSeriesCollection))
                {
                    EditorGUILayout.LabelField("Training Series List is empty", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("foldoutempty"));
                }
                else
                {
                    // Begins Vertical Scroll
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUILayout.BeginVertical();
                    m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 25), GUILayout.Height(GetWidth() - 100));

                    // Go Series by Series
                    for (int i = 0; i < m_SeriesTrainingExamplesNode.TrainingSeriesCollection.Count; i++)
                    {
                        EditorGUILayout.LabelField("Training Series " + i, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                        EditorGUI.indentLevel++;

                        var inputFeaturesInSeries = m_SeriesTrainingExamplesNode.TrainingSeriesCollection[i].Series;
                        var labelSeries = m_SeriesTrainingExamplesNode.TrainingSeriesCollection[i].LabelSeries;

                        // If the input features are not null...
                        if (inputFeaturesInSeries != null)
                        {
                            EditorGUILayout.LabelField("No. Examples: " + inputFeaturesInSeries.Count, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                            // Draw inputs
                            for (int j = 0; j < inputFeaturesInSeries.Count; j++)
                            {
                                EditorGUI.indentLevel++;

                                EditorGUILayout.LabelField("Input Feature " + j, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                // Are there any examples in series?
                                if (inputFeaturesInSeries[j] == null)
                                {
                                    EditorGUILayout.LabelField("Inputs are null ", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                                    break;
                                }

                                EditorGUI.indentLevel++;
                                for (int k = 0; k < inputFeaturesInSeries[j].Count; k++)
                                {
                                    EditorGUILayout.LabelField("Input " + k + " (" + inputFeaturesInSeries[j][k].InputData.DataType + ")", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                    for (int w = 0; w < inputFeaturesInSeries[j][k].InputData.Values.Length; w++)
                                    {
                                        EditorGUI.indentLevel++;

                                        EditorGUILayout.LabelField(inputFeaturesInSeries[j][k].InputData.Values[w].ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                        EditorGUI.indentLevel--;
                                    }


                                }
                                EditorGUI.indentLevel--;
                                EditorGUI.indentLevel--;
                            }
                            EditorGUI.indentLevel--;

                        }
                        // If the input features are null...
                        else
                        {
                            EditorGUILayout.LabelField("Input Features in series are null", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                        }

                        // If the output features for the entire series are not null...
                        if (labelSeries != null)
                        {
                            // Draw output
                            EditorGUI.indentLevel++;

                            EditorGUILayout.TextArea(labelSeries, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                            //EditorGUILayout.LabelField("TEST");

                            EditorGUI.indentLevel--;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Series Target Values are null ", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                        }

                        EditorGUI.indentLevel--;

                    }

                    // Ends Vertical Scroll
                    EditorGUI.indentLevel = indentLevel;
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
                GUILayout.EndArea();
            }

        }



    }
}
