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
    [CustomNodeEditor(typeof(TrainingExamplesNode))]
    public class TrainingExamplesNodeEditor : NodeEditor
    {
        #region Variables
        private static GUIStyle editorLabelStyle;

        private TrainingExamplesNode m_TrainingExamplesNode;
        private int m_RequiredOutputListSlots = 0;

        // Ints used to keep track of how many feautures existed in the node to display warnings
        private int m_LastKnownNumInputFeatures;
        private int m_LastKnownNumOutputFeatures;

        /// <summary>
        /// Flag that will contorl the training examples dropdown
        /// </summary>
        private bool m_ShowTrainingDataDropdown;

        /// <summary>
        /// Scroll for data dropdown
        /// </summary>
        private Vector2 m_ScrollPos;

        #endregion

        #region Unity Messages


        public override void OnBodyGUI()
        {
            if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
            EditorStyles.label.normal.textColor = Color.white;
            // Override label width for this node (since some names are quite long)
            LabelWidth = 184;

            base.OnBodyGUI();

            // Get reference to the current node
            m_TrainingExamplesNode = (target as TrainingExamplesNode);

            // SHOW KEYBOARD SHORTCUTS
            // Show information about runtime keys for interaction
            GUIStyle styleGUIBox = new GUIStyle(GUI.skin.box);
            styleGUIBox.richText = true;
            if (m_TrainingExamplesNode.EnableKeyboardControl)
            {
                GUILayout.Box("<b>Keyboard Shortcuts:</b>\n <b>Record:</b> " + m_TrainingExamplesNode.RecordDataKey.ToString(),
                    styleGUIBox);

                // Show key configs
                m_TrainingExamplesNode.RecordDataKey = (KeyCode)EditorGUILayout.EnumFlagsField(m_TrainingExamplesNode.RecordDataKey);
                EditorGUILayout.Space();
            }



            // SHOW INPUT CONFIG
            ShowDesiredInputsConfigLogic();
            EditorGUILayout.Space();

            // SHOW OUTPUT CONFIG
            ShowDesiredOutputsConfigLogic();
            EditorGUILayout.Space();

            // TOTAL NUMBER OF TRAINING EXAMPLES
            ShowTotalNumberOfTrainingData();

            // DESIRED OUTPUT CONFIG AND BUILD OUTPUT LIST
            EditorGUILayout.Space();
            ShowDesiredOutputFeaturesLogic();
            EditorGUILayout.Space();

            // RECORD EXAMPLES BUTTON 
            ShowRecordExamplesButton();

            // RECORD ONE SINGLE EXAMPLE BUTTON
            ShowRecordOneExampleButton();

            // CLEAR ALL TRAINING EXAMPLES BUTTON
            ShowClearAllExamplesButton();

            // Debug buttons loading stuff
            if (GUILayout.Button("Load All Data From Disk"))
            {
                m_TrainingExamplesNode.LoadDataFromDisk();
            }

            // WARNINGS IF FEATURES CHANGE
            CheckInputFeaturesChanges();
            CheckOutputFeaturesChanges();

            // TRAINING DATA DROPDOWN
            ShowDataDropdown();

            // Error with output configuration
            var nodeTraining = target as TrainingExamplesNode;
            if (nodeTraining.DesiredOutputsConfig.Count == 0)
            {
                EditorGUILayout.HelpBox("There are no Desired Outputs Configured!", MessageType.Error);
            }

            EditorStyles.label.normal = editorLabelStyle.normal;
        }

        #endregion

        #region Private Methods

        private void ShowTotalNumberOfTrainingData()
        {
            string numberExamplesText = "000";
            // Switch data collection mode
            switch (m_TrainingExamplesNode.ModeOfCollection)
            {
                case TrainingExamplesNode.CollectionMode.SingleExample:
                    numberExamplesText = m_TrainingExamplesNode.TotalNumberOfTrainingExamples.ToString();
                    EditorGUILayout.LabelField("Total No. Training Examples: ", numberExamplesText);
                    EditorGUILayout.LabelField("Total No. Desired Outputs: ", m_TrainingExamplesNode.DesiredOutputFeatures.Count.ToString());
                    break;
                case TrainingExamplesNode.CollectionMode.Series:
                    numberExamplesText = m_TrainingExamplesNode.TrainingSeriesCollection.Count.ToString();
                    EditorGUILayout.LabelField("Total No. Training Series: ", numberExamplesText);
                    EditorGUILayout.LabelField("Total No. Desired Outputs: ", m_TrainingExamplesNode.DesiredOutputFeatures.Count.ToString());
                    break;
                default:
                    break;
            }


        }

        private void ShowRecordOneExampleButton()
        {
            // Only draw this in single example collection mode
            if (m_TrainingExamplesNode.ModeOfCollection == TrainingExamplesNode.CollectionMode.SingleExample)
            {
                // Only run button logic when there are features to extract from
                if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.InputFeatures))
                {
                    string nameButton = "";

                    if (m_TrainingExamplesNode.CollectingData)
                        nameButton = "STOP Recording Example";
                    else
                        nameButton = "Record ONE Example";

                    bool disableButton = false;

                    // If there are any models connected we check some conditions
                    if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.IMLConfigurationNodesConnected))
                    {
                        for (int i = 0; i < m_TrainingExamplesNode.IMLConfigurationNodesConnected.Count; i++)
                        {
                            var IMLConfigNodeConnected = m_TrainingExamplesNode.IMLConfigurationNodesConnected[i];
                            // Disable button if model(s) connected are runnning or training
                            if (IMLConfigNodeConnected.Running || IMLConfigNodeConnected.Training)
                            {
                                disableButton = true;
                                break;
                            }

                        }
                    }

                    // Draw button
                    if (disableButton)
                        GUI.enabled = false;
                    if (GUILayout.Button(nameButton))
                    {
                        m_TrainingExamplesNode.AddSingleTrainingExample();
                    }
                    // Always enable it back at the end
                    GUI.enabled = true;


                }
                // If there are no features to extract from we draw a disabled button
                else
                {
                    GUI.enabled = false;
                    if (GUILayout.Button("Record ONE Example"))
                    {
                        m_TrainingExamplesNode.ToggleCollectExamples();
                    }
                    GUI.enabled = true;

                }

            }

        }

        private void ShowRecordExamplesButton()
        {
            
            // Only run button logic when there are features to extract from
            if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.InputFeatures))
            {
                string nameButton = "";

                if (m_TrainingExamplesNode.CollectingData )
                {
                    switch (m_TrainingExamplesNode.ModeOfCollection)
                    {
                        case TrainingExamplesNode.CollectionMode.SingleExample:
                            nameButton = "STOP Recording Examples";
                            break;
                        case TrainingExamplesNode.CollectionMode.Series:
                            nameButton = "STOP Recording Series";
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (m_TrainingExamplesNode.ModeOfCollection)
                    {
                        case TrainingExamplesNode.CollectionMode.SingleExample:
                            nameButton = "Record Examples";                        
                            break;
                        case TrainingExamplesNode.CollectionMode.Series:
                            nameButton = "Record Series";
                            break;
                        default:
                            break;
                    }

                }

                bool disableButton = false;

                // If there are any models connected we check some conditions
                if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.IMLConfigurationNodesConnected))
                {
                    for (int i = 0; i < m_TrainingExamplesNode.IMLConfigurationNodesConnected.Count; i++)
                    {
                        var IMLConfigNodeConnected = m_TrainingExamplesNode.IMLConfigurationNodesConnected[i];
                        // Disable button if model(s) connected are runnning or training
                        if (IMLConfigNodeConnected.Running || IMLConfigNodeConnected.Training)
                        { 
                            disableButton = true;
                            break;
                        }

                    }
                }

                // Draw button
                if (disableButton)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton))
                {
                    m_TrainingExamplesNode.ToggleCollectExamples();
                }
                // Always enable it back at the end
                GUI.enabled = true;


            }
            // If there are no features to extract from we draw a disabled button
            else
            {
                string nameButton = "";
                switch (m_TrainingExamplesNode.ModeOfCollection)
                {
                    case TrainingExamplesNode.CollectionMode.SingleExample:
                        nameButton = "Record Examples";
                        break;
                    case TrainingExamplesNode.CollectionMode.Series:
                        nameButton = "Record Series";
                        break;
                    default:
                        break;
                }

                GUI.enabled = false;
                if (GUILayout.Button(nameButton))
                {
                    m_TrainingExamplesNode.ToggleCollectExamples();
                }
                GUI.enabled = true;

            }

        }

        private void ShowClearAllExamplesButton()
        {
            string nameButton = "";
            bool disableButton = false;
            switch (m_TrainingExamplesNode.ModeOfCollection)
            {
                case TrainingExamplesNode.CollectionMode.SingleExample:
                    nameButton = "Delete All Training Examples";
                    // Only run button logic when there are training examples to delete
                    if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.TrainingExamplesVector))
                    {
                        disableButton = false;
                    }
                    break;
                case TrainingExamplesNode.CollectionMode.Series:
                    nameButton = "Delete All Training Series Collected";
                    // Only run button logic when there are training examples to delete
                    if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.TrainingSeriesCollection))
                    {
                        disableButton = false;
                    }
                    break;
                default:
                    break;
            }


            // Only run button logic when there are training examples to delete
            if (!disableButton)
            {
                // If there are any models connected we check some conditions
                if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.IMLConfigurationNodesConnected))
                {
                    foreach (var IMLConfigNode in m_TrainingExamplesNode.IMLConfigurationNodesConnected)
                    {
                        // Disable button if any of the models is runnning OR collecting data OR training
                        if (IMLConfigNode.Running || IMLConfigNode.Training || m_TrainingExamplesNode.CollectingData)
                        {
                            disableButton = true;
                            break;
                        }
                    }
                }

                // Draw button
                if (disableButton)
                    GUI.enabled = false;
                if (GUILayout.Button(nameButton))
                {
                    m_TrainingExamplesNode.ClearTrainingExamples();
                }
                // Always enable it back at the end
                GUI.enabled = true;



            }
            // If there are no training examples to delete we draw a disabled button
            else
            {
                GUI.enabled = false;
                if (GUILayout.Button(nameButton))
                {
                    m_TrainingExamplesNode.ToggleCollectExamples();
                }
                GUI.enabled = true;
            }

        }

        private void ShowDesiredOutputFeaturesLogic ()
        {
            // SHOW DESIRED OUTPUT CONFIG AND BUILD OUTPUT LIST
            EditorGUILayout.LabelField("Desired Output Value: ");
            // Go through the list of desired outputs and show the correct kind of config editor tool
            for (int i = 0; i < m_TrainingExamplesNode.DesiredOutputFeatures.Count; i++)
            {
                int outputValueIndex = i + 1;
                string labelOutput = "Value " + outputValueIndex + ":";
                var outputFeature = m_TrainingExamplesNode.DesiredOutputFeatures[i];
                // We make sure that the desired output feature list captures the value inputted by the user
                switch (outputFeature.DataType)
                {
                    case (IMLSpecifications.DataTypes) IMLSpecifications.OutputsEnum.Float:
                        (m_TrainingExamplesNode.DesiredOutputFeatures[i] as IMLFloat).SetValue(EditorGUILayout.FloatField(labelOutput, m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[0]) );
                        break;
                    case (IMLSpecifications.DataTypes) IMLSpecifications.OutputsEnum.Integer:
                        (m_TrainingExamplesNode.DesiredOutputFeatures[i] as IMLInteger).SetValue(EditorGUILayout.IntField(labelOutput, (int)m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[0]));
                        break;
                    case (IMLSpecifications.DataTypes) IMLSpecifications.OutputsEnum.Vector2:
                        var vector2ToShow = new Vector2(m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[0], 
                            m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[1]);
                        var valueVector2 = EditorGUILayout.Vector2Field(labelOutput, vector2ToShow);
                        (m_TrainingExamplesNode.DesiredOutputFeatures[i] as IMLVector2).SetValues(valueVector2);
                        break;
                    case (IMLSpecifications.DataTypes) IMLSpecifications.OutputsEnum.Vector3:
                        var vector3ToShow = new Vector3(m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[0], 
                            m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[1], 
                            m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[2]);
                        var valueVector3 = EditorGUILayout.Vector3Field(labelOutput, vector3ToShow);
                        (m_TrainingExamplesNode.DesiredOutputFeatures[i] as IMLVector3).SetValues(valueVector3);
                        break;
                    case (IMLSpecifications.DataTypes) IMLSpecifications.OutputsEnum.Vector4:
                        var vector4ToShow = new Vector4(m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[0],
                            m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[1],
                            m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[2],
                            m_TrainingExamplesNode.DesiredOutputFeatures[i].Values[3]);
                        var valueVector4 = EditorGUILayout.Vector4Field(labelOutput, vector4ToShow);
                        (m_TrainingExamplesNode.DesiredOutputFeatures[i] as IMLVector4).SetValues(valueVector4);
                        break;
                    default:
                        break;
                }
            }


        }

        private void ShowDesiredInputsConfigLogic()
        {
            // SHOW DESIRED INPUT CONFIG AND BUILD INPUT LIST
            EditorGUILayout.LabelField("Inputs: ");
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("No. of Inputs ", m_TrainingExamplesNode.DesiredInputsConfig.Count.ToString());

            // Go through the list of inputs and show the correct kind of config editor tool
            for (int i = 0; i < m_TrainingExamplesNode.DesiredInputsConfig.Count; i++)
            {
                int inputValueIndex = i + 1;
                string label = "Input " + inputValueIndex;
                var inputFeature = m_TrainingExamplesNode.DesiredInputsConfig[i];
                // We make sure that the desired output feature list captures the value inputted by the user
                EditorGUILayout.EnumFlagsField(label, m_TrainingExamplesNode.DesiredInputsConfig[i]);

            }
            EditorGUI.indentLevel--;


        }

        private void ShowDesiredOutputsConfigLogic()
        {
            // SHOW DESIRED INPUT CONFIG AND BUILD INPUT LIST
            EditorGUILayout.LabelField("Outputs: ");
            EditorGUI.indentLevel++;

            //EditorGUILayout.IntField("No. of Outputs ", m_TrainingExamplesNode.DesiredInputsConfig.Count.ToString());

            // Check if we are changing the size of the list 
            int originalSize = m_TrainingExamplesNode.DesiredOutputsConfig.Count;
            int newSize = EditorGUILayout.IntField("No. of Outputs", m_TrainingExamplesNode.DesiredOutputsConfig.Count);
            if (originalSize != newSize)
            {
                m_TrainingExamplesNode.DesiredOutputsConfig.Resize<IMLSpecifications.OutputsEnum>(newSize);
            }
            //Go through the list of inputs and show the correct kind of config editor tool
            for (int i = 0; i < m_TrainingExamplesNode.DesiredOutputsConfig.Count; i++)
            {
                int outputValueIndex = i + 1;
                string label = "Output " + outputValueIndex;
                if (m_TrainingExamplesNode.DesiredInputsConfig.Count > 0 && i < m_TrainingExamplesNode.DesiredInputsConfig.Count)
                {
                    var inputFeature = m_TrainingExamplesNode.DesiredInputsConfig[i];
                    // We make sure that the desired output feature list captures the value inputted by the user
                    EditorGUILayout.EnumFlagsField(label, m_TrainingExamplesNode.DesiredOutputsConfig[i]);
                }

            }
            EditorGUI.indentLevel--;


        }

        /// <summary>
        /// Checks if the num of input features differ from what we last know to show warnings
        /// </summary>
        private void CheckInputFeaturesChanges()
        {
            // Get current number of input features
            TrainingExamplesNode trainingExamplesNode = target as TrainingExamplesNode;
            int currentNumFeatures = trainingExamplesNode.DesiredInputsConfig.Count;

            // Show debug info
            //EditorGUILayout.LabelField("LastKnownInputFeatures: " + m_LastKnownNumInputFeatures);

            // Style for warning box
            GUIStyle tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(24, 10, 10, 10);
            tableStyle.margin.left = 10;


            // If we have no knowledge of input features
            if (m_LastKnownNumInputFeatures == 0)
            {
                m_LastKnownNumInputFeatures = currentNumFeatures;
            }
            // Warning for feature removed
            else if (currentNumFeatures < m_LastKnownNumInputFeatures)
            {
                EditorGUILayout.BeginVertical(tableStyle);
                EditorGUILayout.HelpBox("Deleting a Input/Output Feature deletes all the data recorded with it in all the Examples in this node!", MessageType.Warning);
                if (GUILayout.Button("Undo Action"))
                {
                    m_LastKnownNumInputFeatures = currentNumFeatures;
                }
                EditorGUILayout.EndVertical();

            }
            // Warning for feature added
            else if (currentNumFeatures > m_LastKnownNumInputFeatures)
            {
                EditorGUILayout.BeginVertical(tableStyle);
                EditorGUILayout.HelpBox("Adding a new Input Feature deletes all the previous Examples recorded in this node.", MessageType.Warning);
                if (GUILayout.Button("Undo Action"))
                {
                    m_LastKnownNumInputFeatures = currentNumFeatures;
                }
                EditorGUILayout.EndVertical();
            }
            // If there is no change performed
            else
            {
                // Update value of lasknown input features
                m_LastKnownNumInputFeatures = currentNumFeatures;
            }

        }

        /// <summary>
        /// Checks if the num of output features differ from what we last know to show warnings
        /// </summary>
        private void CheckOutputFeaturesChanges()
        {
            // Get current number of input features
            TrainingExamplesNode trainingExamplesNode = target as TrainingExamplesNode;
            int currentNumFeatures = trainingExamplesNode.DesiredOutputsConfig.Count;

            // Show debug info
            //EditorGUILayout.LabelField("LastKnownOutputFeatures: " + m_LastKnownNumOutputFeatures);

            // Style for warning box
            GUIStyle tableStyle = new GUIStyle("box");
            tableStyle.padding = new RectOffset(24, 10, 10, 10);
            tableStyle.margin.left = 10;


            // If we have no knowledge of output features
            if (m_LastKnownNumOutputFeatures == 0)
            {
                m_LastKnownNumOutputFeatures = currentNumFeatures;
            }
            // Warning for feature removed
            else if (currentNumFeatures < m_LastKnownNumOutputFeatures)
            {
                EditorGUILayout.BeginVertical(tableStyle);
                EditorGUILayout.HelpBox("Deleting a Input/Output Feature deletes all the data recorded with it in all the Examples in this node!", MessageType.Warning);
                if (GUILayout.Button("Undo Action"))
                {
                    m_LastKnownNumOutputFeatures = currentNumFeatures;
                }
                EditorGUILayout.EndVertical();

            }
            // Warning for feature added
            else if (currentNumFeatures > m_LastKnownNumOutputFeatures)
            {
                EditorGUILayout.BeginVertical(tableStyle);
                EditorGUILayout.HelpBox("Adding a new Output Feature deletes all the previous Examples recorded in this node.", MessageType.Warning);
                if (GUILayout.Button("Undo Action"))
                {
                    m_LastKnownNumOutputFeatures = currentNumFeatures;
                }
                EditorGUILayout.EndVertical();
            }
            // If there is no change performed
            else
            {
                // Update value of lasknown input features
                m_LastKnownNumOutputFeatures = currentNumFeatures;
            }

        }

        /// <summary>
        /// Shows training data dropdown accounting for differences in data
        /// </summary>
        private void ShowDataDropdown()
        {
            switch (m_TrainingExamplesNode.ModeOfCollection)
            {
                case TrainingExamplesNode.CollectionMode.SingleExample:
                    ShowTrainingExamplesDropdown();
                    break;
                case TrainingExamplesNode.CollectionMode.Series:
                    ShowTrainingSeriesCollectedDropdown();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Shows a dropdown with the training examples
        /// </summary>
        private void ShowTrainingExamplesDropdown()
        {
            EditorGUILayout.Space();

            m_ShowTrainingDataDropdown = EditorGUILayout.Foldout(m_ShowTrainingDataDropdown, "Inspect Training Examples ");
            if (m_ShowTrainingDataDropdown)
            {
                EditorGUI.indentLevel++;

                if (ReusableMethods.Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.TrainingExamplesVector))
                {
                    EditorGUILayout.LabelField("Training Examples List is empty");
                }
                else
                {
                    // Begins Vertical Scroll
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUILayout.BeginVertical();
                    m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 25), GUILayout.Height(GetWidth() - 100));

                    for (int i = 0; i < m_TrainingExamplesNode.TrainingExamplesVector.Count; i++)
                    {
                        EditorGUILayout.LabelField("Training Example " + (i+1));

                        EditorGUI.indentLevel++;

                        var inputFeatures = m_TrainingExamplesNode.TrainingExamplesVector[i].Inputs;
                        var outputFeatures = m_TrainingExamplesNode.TrainingExamplesVector[i].Outputs;

                        // If the input features are not null...
                        if (inputFeatures != null)
                        {
                            // Draw inputs
                            for (int j = 0; j < inputFeatures.Count; j++)
                            {

                                if (inputFeatures[j].InputData == null )
                                {
                                    EditorGUILayout.LabelField("Inputs are null ");
                                    break;
                                }


                                EditorGUI.indentLevel++;

                                EditorGUILayout.LabelField("Input " + (j+1) + " (" + inputFeatures[j].InputData.DataType + ")");

                                for (int k = 0; k < inputFeatures[j].InputData.Values.Length; k++)
                                {
                                    EditorGUI.indentLevel++;

                                    EditorGUILayout.LabelField(inputFeatures[j].InputData.Values[k].ToString());

                                    EditorGUI.indentLevel--;
                                }

                                EditorGUI.indentLevel--;
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Inputs are null ");
                        }

                        // If the output features are not null...
                        if (outputFeatures != null)
                        {
                            // Draw outputs
                            for (int j = 0; j < outputFeatures.Count; j++)
                            {
                                if (outputFeatures[j].OutputData == null)
                                {
                                    EditorGUILayout.LabelField("Outputs are null ");
                                    break;
                                }


                                EditorGUI.indentLevel++;

                                EditorGUILayout.LabelField("Output " + (j+1) + " (" + outputFeatures[j].OutputData.DataType + ")");


                                for (int k = 0; k < outputFeatures[j].OutputData.Values.Length; k++)
                                {

                                    EditorGUI.indentLevel++;

                                    EditorGUILayout.LabelField(outputFeatures[j].OutputData.Values[k].ToString());

                                    EditorGUI.indentLevel--;

                                }

                                EditorGUI.indentLevel--;

                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Outputs are null ");
                        }

                        EditorGUI.indentLevel--;

                    }

                    // Ends Vertical Scroll
                    EditorGUI.indentLevel = indentLevel;
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();

                }

                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// Shows a dropdown with the training examples series
        /// </summary>
        private void ShowTrainingSeriesCollectedDropdown()
        {
            EditorGUILayout.Space();

            m_ShowTrainingDataDropdown = EditorGUILayout.Foldout(m_ShowTrainingDataDropdown, "Inspect Training Series");
            if (m_ShowTrainingDataDropdown)
            {
                EditorGUI.indentLevel++;

                if (ReusableMethods.Lists.IsNullOrEmpty(ref m_TrainingExamplesNode.TrainingSeriesCollection))
                {
                    EditorGUILayout.LabelField("Training Series List is empty");
                }
                else
                {
                    // Begins Vertical Scroll
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUILayout.BeginVertical();
                    m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth() - 25), GUILayout.Height(GetWidth() - 100));

                    // Go Series by Series
                    for (int i = 0; i < m_TrainingExamplesNode.TrainingSeriesCollection.Count; i++)
                    {
                        EditorGUILayout.LabelField("Training Series " + i);

                        EditorGUI.indentLevel++;

                        var inputFeaturesInSeries = m_TrainingExamplesNode.TrainingSeriesCollection[i].Series;
                        var labelSeries = m_TrainingExamplesNode.TrainingSeriesCollection[i].LabelSeries;

                        // If the input features are not null...
                        if (inputFeaturesInSeries != null)
                        {
                            EditorGUILayout.LabelField("No. Examples: " + inputFeaturesInSeries.Count);

                            // Draw inputs
                            for (int j = 0; j < inputFeaturesInSeries.Count; j++)
                            {
                                EditorGUI.indentLevel++;

                                EditorGUILayout.LabelField("Input Feature " + j);                             

                                // Are there any examples in series?
                                if (inputFeaturesInSeries[j] == null)
                                {
                                    EditorGUILayout.LabelField("Inputs are null ");
                                    break;
                                }

                                EditorGUI.indentLevel++;
                                for (int k = 0; k < inputFeaturesInSeries[j].Count; k++)
                                {
                                    EditorGUILayout.LabelField("Input " + k + " (" + inputFeaturesInSeries[j][k].InputData.DataType + ")");

                                    for (int w = 0; w < inputFeaturesInSeries[j][k].InputData.Values.Length; w++)
                                    {
                                        EditorGUI.indentLevel++;

                                        EditorGUILayout.LabelField(inputFeaturesInSeries[j][k].InputData.Values[w].ToString());

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
                            EditorGUILayout.LabelField("Input Features in series are null");
                        }

                        // If the output features for the entire series are not null...
                        if (labelSeries != null)
                        {
                            // Draw output
                            EditorGUI.indentLevel++;

                            EditorGUILayout.TextArea(labelSeries);
                            //EditorGUILayout.LabelField("TEST");

                            EditorGUI.indentLevel--;
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Series Output is null ");
                        }

                        EditorGUI.indentLevel--;

                    }

                    // Ends Vertical Scroll
                    EditorGUI.indentLevel = indentLevel;
                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
            }

        }

        #endregion

    }

}
