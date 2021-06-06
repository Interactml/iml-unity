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
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl0;
        /// <summary>
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl1;
        /// <summary>
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl2;
        /// <summary>
        /// List of dropdowns per data entry (lvl 0, highest)
        /// </summary>
        protected bool[] m_DataDropdownsLvl3;
        /// <summary>
        /// Rect for dropdown layout
        /// </summary>
        protected Rect m_Dropdown;
        /// <summary>
        /// Position of scroll for dropdown
        /// </summary>
        protected Vector2 m_ScrollPos;

        /// <summary>
        /// Style of foldout
        /// </summary>
        GUIStyle m_FoldoutStyle;
        GUIStyle m_FoldoutEmptyStyle;
        GUIStyle m_ScrollViewStyle;


        public override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            // get refs
            if (m_NodeDataSet == null)
                m_NodeDataSet = target as TrainingDataSetsNode;
            if (m_FoldoutStyle == null)
                SetDropdownStyle(out m_FoldoutStyle);
            if (m_FoldoutEmptyStyle == null)
                m_FoldoutEmptyStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("foldoutempty");
            if (m_ScrollViewStyle == null)
                m_ScrollViewStyle = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview");

        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            // Button LoadDataSets
            string buttonText = "";
            if (m_NodeDataSet.LoadingStarted)
                buttonText = $"Loading... ({m_NodeDataSet.DataSetSize} Loaded)";
            else
                buttonText = "Load Data Sets";
            if (GUILayout.Button(buttonText, m_NodeSkin.GetStyle("Run") ))
            {
                m_NodeDataSet.LoadDataSets(m_NodeDataSet.FolderPath, specificID: m_NodeDataSet.SpecificNodeID);

            }

            // Show data sets dropdown
            ShowDataSetsDropdown();
        }

        /// <summary>
        /// Set style for dropdown for training examples nodes
        /// </summary>
        protected void SetDropdownStyle(out GUIStyle myFoldoutStyle)
        {
            GUI.skin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");
            myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
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
        }

        private void ShowDataSetsDropdown()
        {
            int numEntries = 0;
            if (m_NodeDataSet.TrainingDataSets != null && m_NodeDataSet.TrainingDataSets.Count > 0)
                numEntries = m_NodeDataSet.TrainingDataSets.Count;

            m_ShowTrainingDataDropdown = EditorGUILayout.Foldout(m_ShowTrainingDataDropdown, $"View Training Pairs ({numEntries} Entries)", m_FoldoutStyle);

            // Save original indent level
            int originalIndentLevel = EditorGUI.indentLevel;

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

                if (m_NodeDataSet.TrainingDataSets == null || m_NodeDataSet.TrainingDataSets.Count == 0)
                {
                    EditorGUILayout.LabelField("Training Data Sets List is empty", m_FoldoutEmptyStyle);
                }
                else
                {
                    // Begins Vertical Scroll

                    // init dropdowns
                    if (m_DataDropdownsLvl0 == null || m_DataDropdownsLvl0.Length != m_NodeDataSet.TrainingDataSets.Count)
                        m_DataDropdownsLvl0 = new bool[m_NodeDataSet.TrainingDataSets.Count];

                    EditorGUILayout.BeginVertical();

                    m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 25), GUILayout.Height(GetWidth() - 100));

                    // Training Examples List iteration (only draw 1% of data)
                    for (int i = 0; i < m_NodeDataSet.TrainingDataSets.Count; i++)
                    {
                        var trainingExamplesList = m_NodeDataSet.TrainingDataSets[i];
                        IMLBaseDataType labelSeries = null;

                        m_DataDropdownsLvl0[i] = EditorGUILayout.Foldout(m_DataDropdownsLvl0[i], $"Training Data Set {i}", m_FoldoutStyle);

                        EditorGUI.indentLevel++;

                        if (m_DataDropdownsLvl0[i])
                        {
                            // If the input features are not null...
                            if (trainingExamplesList != null && trainingExamplesList.Count > 0)
                            {
                                // init dropdowns
                                if (m_DataDropdownsLvl1 == null || m_DataDropdownsLvl1.Length != trainingExamplesList.Count)
                                    m_DataDropdownsLvl1 = new bool[trainingExamplesList.Count];


                                labelSeries = m_NodeDataSet.TrainingDataSets[i][0].Outputs[0].OutputData;

                                m_DataDropdownsLvl1[i] = EditorGUILayout.Foldout(m_DataDropdownsLvl1[i], $"No. Examples: {trainingExamplesList.Count}", m_FoldoutStyle);

                                if (m_DataDropdownsLvl1[i])
                                {
                                    // Each Training Example iteration
                                    for (int j = 0; j < trainingExamplesList.Count * 0.01; j++)
                                    {
                                        EditorGUI.indentLevel++;

                                        // Are there any examples in series?
                                        if (trainingExamplesList[j] == null)
                                        {
                                            EditorGUILayout.LabelField("Inputs are null ", m_ScrollViewStyle);
                                            break;
                                        }

                                        EditorGUI.indentLevel++;

                                        // init dropdowns
                                        if (m_DataDropdownsLvl2 == null || m_DataDropdownsLvl2.Length != trainingExamplesList.Count)
                                            m_DataDropdownsLvl2 = new bool[trainingExamplesList.Count];

                                        m_DataDropdownsLvl2[j] = EditorGUILayout.Foldout(m_DataDropdownsLvl2[j], $"Input Feature {j}", m_FoldoutStyle);

                                        if (m_DataDropdownsLvl2[j])
                                        {
                                            // Each input feature in training examples
                                            for (int k = 0; k < trainingExamplesList[j].Inputs.Count * 0.01; k++)
                                            {

                                                // init dropdowns
                                                if (m_DataDropdownsLvl3 == null || m_DataDropdownsLvl3.Length != trainingExamplesList[j].Inputs.Count)
                                                    m_DataDropdownsLvl3 = new bool[trainingExamplesList[j].Inputs.Count];

                                                m_DataDropdownsLvl3[k] = EditorGUILayout.Foldout(m_DataDropdownsLvl3[k], $"Input {k} ({trainingExamplesList[j].Inputs[k].InputData.DataType})", m_FoldoutStyle);

                                                if (m_DataDropdownsLvl3[k])
                                                {
                                                    for (int w = 0; w < trainingExamplesList[j].Inputs[k].InputData.Values.Length; w++)
                                                    {
                                                        EditorGUI.indentLevel++;

                                                        EditorGUILayout.LabelField(trainingExamplesList[j].Inputs[k].InputData.Values[w].ToString(), m_ScrollViewStyle);

                                                        EditorGUI.indentLevel--;
                                                    }

                                                }


                                            }

                                        }
                                        EditorGUI.indentLevel--;
                                        EditorGUI.indentLevel--;
                                    }
                                    EditorGUI.indentLevel--;


                                }

                            }
                            // If the input features are null...
                            else
                            {
                                EditorGUILayout.LabelField("Input Features in series are null or empty", m_ScrollViewStyle);
                            }

                            // If the output features for the entire series are not null...
                            if (labelSeries != null)
                            {
                                // Draw output
                                EditorGUI.indentLevel++;

                                EditorGUILayout.TextArea($"Label series: {labelSeries.Values[0]}", m_ScrollViewStyle);
                                //EditorGUILayout.LabelField("TEST");

                                EditorGUI.indentLevel--;
                            }
                            else
                            {
                                EditorGUILayout.LabelField("Series Target Values are null ", m_ScrollViewStyle);
                            }


                        }

                        EditorGUI.indentLevel--;

                    }
                    

                    // End Vertical Scroll
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();

                }

                EditorGUI.indentLevel--;

            }

            // Reset indent level
            EditorGUI.indentLevel = originalIndentLevel;


            //GUILayout.EndArea();


        }
    }
}