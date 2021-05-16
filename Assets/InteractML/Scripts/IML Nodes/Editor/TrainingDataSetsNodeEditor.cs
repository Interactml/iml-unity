using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeEditor(typeof(TrainingDataSetsNode))]
    public class TrainingDataSetsNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Node to draw
        /// </summary>
        TrainingDataSetsNode m_NodeDataSet;

        /// <summary>
        /// Boolean that shows or hides training data sets
        /// </summary>
        protected bool m_ShowTrainingDataDropdown;
        /// <summary>
        /// Rect for dropdown layout
        /// </summary>
        protected Rect m_Dropdown;
        /// <summary>
        /// Position of scroll for dropdown
        /// </summary>
        protected Vector2 m_ScrollPos;



        public override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            // get refs
            if (m_NodeDataSet == null)
                m_NodeDataSet = target as TrainingDataSetsNode;
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            // Button LoadDataSets
            if (GUILayout.Button("Load Data Sets", m_NodeSkin.GetStyle("Run") ))
            {
                m_NodeDataSet.LoadDataSets(m_NodeDataSet.FolderPath, specificID: m_NodeDataSet.SpecificNodeID);

            }

            SetDropdownStyle();
            // Show data sets dropdown
            ShowDataSetsDropdown();
        }

        /// <summary>
        /// Set style for dropdown for training examples nodes
        /// </summary>
        protected void SetDropdownStyle()
        {
            GUI.skin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");
            GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
            Color myStyleColor = Color.white;
            myFoldoutStyle.fontStyle = FontStyle.Bold;
            myFoldoutStyle.normal.textColor = myStyleColor;
            myFoldoutStyle.onNormal.textColor = myStyleColor;
            myFoldoutStyle.hover.textColor = myStyleColor;
            myFoldoutStyle.onHover.textColor = myStyleColor;
            myFoldoutStyle.focused.textColor = myStyleColor;
            myFoldoutStyle.onFocused.textColor = myStyleColor;
            myFoldoutStyle.active.textColor = myStyleColor;
            myFoldoutStyle.onActive.textColor = myStyleColor;
            myFoldoutStyle.fontStyle = FontStyle.Bold;
            myFoldoutStyle.normal.textColor = myStyleColor;
            myFoldoutStyle.fontStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview").fontStyle;
            m_ShowTrainingDataDropdown = EditorGUILayout.Foldout(m_ShowTrainingDataDropdown, "View Training Pairs", myFoldoutStyle);
        }



        private void ShowDataSetsDropdown()
        {
            if (m_ShowTrainingDataDropdown)
            {
                //m_Dropdown = m_HelpRect;

                //m_Dropdown.height = 300;
                //if (Event.current.type == EventType.Layout)
                //{
                //    GUI.DrawTexture(m_Dropdown, NodeColor);
                //}

                //GUILayout.BeginArea(m_Dropdown);

                EditorGUI.indentLevel++;

                if (m_NodeDataSet.TrainingDataSets == null)
                    return;

                if (m_NodeDataSet.TrainingDataSets.Count < 0)
                {
                    EditorGUILayout.LabelField("Training Data Sets List is empty", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("foldoutempty"));
                }
                else
                {
                    // Begins Vertical Scroll
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUILayout.BeginVertical();

                    m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 25), GUILayout.Height(GetWidth() - 100));

                    // Go Series by Series
                    for (int i = 0; i < m_NodeDataSet.TrainingDataSets.Count; i++)
                    {
                        EditorGUILayout.LabelField("Training Data Sets " + i, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                        var inputFeaturesInSeries = m_NodeDataSet.TrainingDataSets[i];
                        IMLBaseDataType labelSeries = null;

                        EditorGUI.indentLevel++;

                        // If the input features are not null...
                        if (inputFeaturesInSeries != null && inputFeaturesInSeries.Count > 0)
                        {
                            labelSeries = m_NodeDataSet.TrainingDataSets[i][0].Outputs[0].OutputData;

                            EditorGUILayout.LabelField("No. Examples: " + inputFeaturesInSeries.Count, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                            // we only draw inputs if there are more than 50 examples
                            if (inputFeaturesInSeries.Count < 50)
                            {
                                //// Draw inputs
                                //for (int j = 0; j < inputFeaturesInSeries.Count; j++)
                                //{
                                //    EditorGUI.indentLevel++;

                                //    EditorGUILayout.LabelField("Input Feature " + j, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                //    // Are there any examples in series?
                                //    if (inputFeaturesInSeries[j] == null)
                                //    {
                                //        EditorGUILayout.LabelField("Inputs are null ", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                                //        break;
                                //    }

                                //    EditorGUI.indentLevel++;

                                //    for (int k = 0; k < inputFeaturesInSeries[j].Inputs.Count; k++)
                                //    {
                                //        EditorGUILayout.LabelField("Input " + k + " (" + inputFeaturesInSeries[j].Inputs[k].InputData.DataType + ")", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                //        for (int w = 0; w < inputFeaturesInSeries[j].Inputs[k].InputData.Values.Length; w++)
                                //        {
                                //            EditorGUI.indentLevel++;

                                //            EditorGUILayout.LabelField(inputFeaturesInSeries[j].Inputs[k].InputData.Values[w].ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                //            EditorGUI.indentLevel--;
                                //        }


                                //    }
                                //    EditorGUI.indentLevel--;
                                //    EditorGUI.indentLevel--;
                                //}
                                //EditorGUI.indentLevel--;


                            }

                        }
                        // If the input features are null...
                        else
                        {
                            EditorGUILayout.LabelField("Input Features in series are null or empty", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                        }

                        // If the output features for the entire series are not null...
                        if (labelSeries != null)
                        {
                            // Draw output
                            EditorGUI.indentLevel++;

                            EditorGUILayout.TextArea(labelSeries.ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
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



            }

            EditorGUI.indentLevel--;
            //GUILayout.EndArea();


        }
    }
}