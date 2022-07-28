using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReusableMethods;
using System;
using System.Linq;
using XNode;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif
namespace InteractML
{
    [CustomNodeEditor(typeof(MLSystem))]
    public class MLSystemEditor : IMLNodeEditor
    {

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        protected MLSystem m_MLSystem;

        /// <summary>
        /// The label to show on the button port labels
        /// </summary>
        protected GUIContent m_ButtonPortLabel;
        /// <summary>
        /// NodePort for button. Loaded in OnHeaderHUI()
        /// </summary>
        protected NodePort m_ButtonPortToggleTrainInput;
        /// <summary>
        /// NodePort for button. Loaded in OnHeaderHUI()
        /// </summary>
        protected NodePort m_ButtonPortToggleRunInput;

        // Testing UI vars        
        Texture2D m_TestingBGColor;
        Vector2 m_TestingUIPosition;
        float m_TestingUIWidth;
        float m_TestingUIHeight; 
        Vector2 m_TestingUISize;
        Rect m_TestingUIRect;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_MLSystem = (target as MLSystem);
            //previous nodespace 10
            nodeSpace = 20;
            string arrayNo = "";
            if (m_MLSystem.numberInComponentList != -1)
                arrayNo = m_MLSystem.numberInComponentList.ToString();
            NodeName = "MACHINE LEARNING SYSTEM " + arrayNo;

            // Create inputport button label
            if (m_ButtonPortLabel == null)
                m_ButtonPortLabel = new GUIContent("");

            // Get button ports
            if (m_ButtonPortToggleTrainInput == null)
                m_ButtonPortToggleTrainInput = m_MLSystem.GetPort("ToggleTrainInputBoolPort");
            if (m_ButtonPortToggleRunInput == null)
                m_ButtonPortToggleRunInput = m_MLSystem.GetPort("ToggleRunInputBoolPort");


            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            InputPortsNamesOverride = new Dictionary<string, string>();
            InputPortsNamesOverride.Add("IMLTrainingExamplesNodes", "Recorded Data In");
            InputPortsNamesOverride.Add("InputFeatures", "Live Data In");
            base.nodeTips = m_MLSystem.tooltips;
            //previous nodespace 330
            m_BodyRect.height = 350;
            base.OnBodyGUI();
            ShowTestingPanel();
            //ShowRatingPanel();
        }

        protected override void ShowBodyFields()
        {
            ShowTrainingIcon(m_MLSystem.LearningType.ToString());
            ShowButtons(m_MLSystem);
            GUILayout.Space(20);
            ShowRunOnAwakeToggle(m_MLSystem as MLSystem);
            GUILayout.Space(10);
            ShowUniqueClassesLabel();
            GUILayout.Space(20);
            // if there is an error show the correct warning
            if (m_MLSystem.error)
            {

                nodeSpace = 60;
                m_BodyRect.height = m_BodyRect.height + HeaderRect.height + 35;
                ShowWarning(m_MLSystem.warning);
            }
        }

        /// <summary>
        /// Shows label with number of unique classes trained on
        /// </summary>
        protected void ShowUniqueClassesLabel()
        {
            int indentNumber = 5;
            EditorGUI.indentLevel += indentNumber;
            EditorGUILayout.LabelField($"Unique Classes Trained On: {m_MLSystem.TotalNumUniqueClasses}", m_NodeSkin.GetStyle("Node Local Space Label"));
            EditorGUI.indentLevel -= indentNumber;
            GUILayout.Space(5);
        }

        /// <summary>
        /// Show and control run on awake toggle for IMLConfiguration node
        /// </summary>
        /// <param name="configNode">Node to be controlled</param>
        protected void ShowRunOnAwakeToggle(MLSystem configNode)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            configNode.RunOnAwake = EditorGUILayout.Toggle(configNode.RunOnAwake, m_NodeSkin.GetStyle("Local Space Toggle"));
            EditorGUILayout.LabelField("Run Model On Play", m_NodeSkin.GetStyle("Node Local Space Label"));
            GUILayout.EndHorizontal();
        }
        /// <summary>
        /// Show MLS Algorithm icon 
        /// </summary>
        /// <param name="MLS">Name of Machine Learning System: Classification, Regression or DTW</param>
        protected void ShowTrainingIcon(string MLS)
        {
            m_IconCenter.x = m_BodyRect.x;
            m_IconCenter.y = m_BodyRect.y;
            m_IconCenter.width = m_BodyRect.width;
            m_IconCenter.height = 150;

            GUILayout.BeginArea(m_IconCenter);
            GUILayout.Space(bodySpace);
            GUILayout.BeginHorizontal();
            GUILayout.Box("", m_NodeSkin.GetStyle(MLS + " MLS Image"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(MLS, m_NodeSkin.GetStyle(MLS + " Label"));
            GUILayout.EndHorizontal();

            GUILayout.EndArea();

        }

        /// <summary>
        /// Show the load, delete and record buttons
        /// </summary>
        protected void ShowButtons(MLSystem node)
        {
            m_ButtonsRect.x = m_BodyRect.x -5 ;
            m_ButtonsRect.y = m_IconCenter.y + m_IconCenter.height;
            m_ButtonsRect.width = m_BodyRect.width -10;
            m_ButtonsRect.height = 150;
            GUILayout.Space(230);

            // DRAW BUTTONS AND PORTS OUTSIDE OF BEGIN AREA TO MAKE THEM WORK
            GUILayout.BeginHorizontal();
            // Draw port
            IMLNodeEditor.PortField(m_ButtonPortLabel, m_ButtonPortToggleTrainInput, m_NodeSkin.GetStyle("Port Label"), GUILayout.MaxWidth(10));

            // if button contains mouse position
            TrainModelButton();
            GUILayout.EndHorizontal();


            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            // Draw port
            IMLNodeEditor.PortField(m_ButtonPortLabel, m_ButtonPortToggleRunInput, m_NodeSkin.GetStyle("Port Label"), GUILayout.MaxWidth(10));

            RunModelButton();
            GUILayout.EndHorizontal();

            GUILayout.BeginArea(m_ButtonsRect);

            //GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(NodeWidth / 2 - 20);
            if (GUILayout.Button("", m_NodeSkin.GetStyle("Reset")))
            {
                node.ResetModel();
                numberOfExamplesTrained = 0;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.Label("reset model", m_NodeSkin.GetStyle("Reset Pink Label"));
            GUILayout.EndHorizontal();
            //ShowRunOnAwakeToggle(node);
            GUILayout.EndArea();
        }
        /// <summary>
        ///Training button for MLS node 
        /// </summary>
        protected virtual void TrainModelButton()
        {
            string nameButton = "";

            if (m_MLSystem.Trained)
                nameButton = "Trained (" + m_MLSystem.NumExamplesTrainedOn + " Examples)";
            else if (m_MLSystem.Training)
                nameButton = "Training";
            else
                nameButton = "Train Model";
            if ((m_MLSystem.Model != null && m_MLSystem.TotalNumTrainingDataConnected > 0 && !m_MLSystem.Running && !m_MLSystem.Training))
            {
                // If testing state used...
                if (m_MLSystem.UseTestingState)
                {
                    // If we are testing, disable gui
                    if (m_MLSystem.Testing) GUI.enabled = false;
                    else GUI.enabled = true;
                }
                // Default behaviour, not worried about testing logic at all
                else
                {
                    // Enable UI
                    GUI.enabled = true;
                }
            }
            // If rapidlib reference is null we draw a disabled button or if it is running or training (or testing if such state is being used)
            else
            {
                GUI.enabled = false;

            }
            if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Train")))
            {
                IMLEventDispatcher.TrainMLSCallback?.Invoke(m_MLSystem.id);
            }

            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                if (GUI.enabled)
                {
                    // Check for null
                    if (m_MLSystem.tooltips != null && m_MLSystem.tooltips.BodyTooltip != null && m_MLSystem.tooltips.BodyTooltip.Tips != null)
                        TooltipText = m_MLSystem.tooltips.BodyTooltip.Tips[1];
                }
                else
                {
                    //TooltipText = m_MLSystem.tooltips.BodyTooltip.Error[0];
                }
            }
            else if (Event.current.type == EventType.MouseMove && !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTip = false;

            }

            if (Event.current.type == EventType.Layout && buttonTipHelper)
            {
                buttonTip = true;
                buttonTipHelper = false;
            }

            // Reenable GUI afterwards
            GUI.enabled = true;
        }
        /// <summary>
        /// Run button for MLS node
        /// </summary>
        protected virtual void RunModelButton()
        {
            string nameButton = "";
            if (m_MLSystem.Running)
            {
                nameButton = "STOP";
                // In case we are using the testing state
                if (m_MLSystem.UseTestingState) nameButton = "STOP & RATE";
            }
            else
            {
                if (m_MLSystem.TrainingType == IMLSpecifications.TrainingSetType.SeriesTrainingExamples)
                    nameButton = "Populate";
                else
                {
                    if (m_MLSystem.UseTestingState && !m_MLSystem.AllTestingClassesCollected) nameButton = "Test & Run";
                    else nameButton = "Run";
                }
            }
            // If rapidlib reference is null we draw a disabled button
            if (((m_MLSystem.Model == null || m_MLSystem.Model.ModelAddress == (IntPtr)0 || m_MLSystem.Training || m_MLSystem.Untrained || !m_MLSystem.matchLiveDataInputs || !m_MLSystem.matchVectorLength)
                // And If the system isn't running as well
                && !m_MLSystem.Running)
                // OR if the node is testing!
                || (m_MLSystem.UseTestingState && m_MLSystem.Testing))
            {
               /* Debug.Log(m_MLSystem.Model == null);
                Debug.Log(m_MLSystem.Model.ModelAddress == (IntPtr)0);
                Debug.Log(m_MLSystem.Training);
                Debug.Log(m_MLSystem.Untrained);
                Debug.Log(!m_MLSystem.matchLiveDataInputs);
                Debug.Log(!m_MLSystem.matchVectorLength);*/
                // Disable button if model is Trainig OR Untrained 
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }

            if (GUILayout.Button(nameButton, m_NodeSkin.GetStyle("Run")))
            {
                IMLEventDispatcher.ToggleRunCallback?.Invoke(m_MLSystem.id);
            }

            
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                if (GUI.enabled)
                {
                    // Check for null
                    if (m_MLSystem.tooltips != null && m_MLSystem.tooltips.BodyTooltip != null && m_MLSystem.tooltips.BodyTooltip.Tips != null)
                        TooltipText = m_MLSystem.tooltips.BodyTooltip.Tips[2];
                }
                else
                {
                    // Check for null
                    if (m_MLSystem.tooltips != null && m_MLSystem.tooltips.BodyTooltip != null && m_MLSystem.tooltips.BodyTooltip.Tips != null)
                        TooltipText = m_MLSystem.tooltips.BodyTooltip.Error[1];
                }
            }
            else if (Event.current.type == EventType.MouseMove && !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTip = false;

            }
            GUI.enabled = true;

            if (Event.current.type == EventType.Layout && buttonTipHelper)
            {
                buttonTip = true;
                buttonTipHelper = false;
            }
        }

        #region Testing Interface 

        /// <summary>
        /// Shows the testing panel on top of node
        /// </summary>
        protected virtual void ShowTestingPanel()
        {
            // Only draw if the testing state is used and active
            if (m_MLSystem.UseTestingState && m_MLSystem.Testing)
            {
                // Avoid testing in a hotreload (needed vars are not properly populated)
                if (m_MLSystem.TotalUniqueTrainingClasses == null && m_MLSystem.TotalUniqueTrainingClasses.Count == 0 || m_MLSystem.CurrentTestingClassCollected > m_MLSystem.TotalUniqueTrainingClasses.Count - 1)
                {
                    m_MLSystem.StopTesting();
                    return;
                }

                // Create a new rect that occupies part of the node interface
                m_TestingUIPosition = m_BodyRect.position;
                m_TestingUIPosition.x -= 10;
                m_TestingUIWidth = HeaderRect.width + 20;
                m_TestingUIHeight = 380; // 380 is the height of the buttons in the MLSNode (roughly similar to bodyrect.height without the warning height)
                if (m_TestingUISize == null) m_TestingUISize = new Vector2();
                m_TestingUISize.x = m_TestingUIWidth;
                m_TestingUISize.y = m_TestingUIHeight;
                if (m_TestingUIRect == null) m_TestingUIRect = new Rect();
                m_TestingUIRect.position = m_TestingUIPosition;
                m_TestingUIRect.size = m_TestingUISize;

                GUILayout.BeginArea(m_TestingUIRect);
                
                if (m_TestingBGColor == null)
                    m_TestingBGColor = GetColorTextureFromHexString("#3b4675");
                // Draw background panel
                GUI.DrawTexture(m_TestingUIRect, m_TestingBGColor);
                //EditorGUI.DrawRect(TestingUIRect, Color.white);

                // Content of panel
                //GUILayout.Label("MACHINE LEARNING SYSTEM", m_NodeSkin.GetStyle("Header"), GUILayout.MinWidth(300));
                GUILayout.Space(120);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Provide Testing Examples", m_NodeSkin.GetStyle("Header"), GUILayout.MinWidth(275));
                // Cross to close the window (only when all the data is collected)
                if (!m_MLSystem.AllTestingClassesCollected) GUI.enabled = false;
                if (GUILayout.Button("", m_NodeSkin.GetStyle("Cross")))
                {
                    m_MLSystem.StopTesting();
                    m_MLSystem.StopRunning();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
                int space = 5;
                GUILayout.Space(space);


                // If there are classes not yet collected...
                if (!m_MLSystem.AllTestingClassesCollected)
                {
                    // Draw details of current class being collected
                    var expectedOutputClass = m_MLSystem.TotalUniqueTrainingClasses[m_MLSystem.CurrentTestingClassCollected].Outputs;
                    string expectedOutputClassString = "{ ";
                    foreach (var expectedOutput in expectedOutputClass)
                    {
                        if (expectedOutput == null || expectedOutput.OutputData == null || expectedOutput.OutputData.Values == null) return;
                        expectedOutputClassString = string.Concat(expectedOutputClassString, "{ ");
                        for (int i = 0; i < expectedOutput.OutputData.Values.Length; i++)
                        {
                            var value = expectedOutput.OutputData.Values[i];
                            expectedOutputClassString = string.Concat(expectedOutputClassString, value.ToString());
                            // there are more values after
                            if (i < expectedOutput.OutputData.Values.Length-1) expectedOutputClassString = string.Concat(expectedOutputClassString, ", ");
                            // this is the last value
                            else expectedOutputClassString = string.Concat(expectedOutputClassString, " } ");
                        }
                    }
                    expectedOutputClassString = string.Concat(expectedOutputClassString, "}");
                    int numTestingExamples = 0;
                    int classesCompleted = m_MLSystem.CurrentTestingClassCollected > 0 ? m_MLSystem.CurrentTestingClassCollected : 0;
                    if (m_MLSystem.TestingData != null && m_MLSystem.CurrentTestingClassCollected < m_MLSystem.TestingData.Count)
                    {
                        numTestingExamples = m_MLSystem.TestingData[m_MLSystem.CurrentTestingClassCollected].Count;
                        classesCompleted = 0;
                        foreach (var sublist in m_MLSystem.TestingData)
                        {
                            if (sublist != null && sublist.Count > 0)
                            {
                                classesCompleted++;
                            }
                        }
                    }


                    GUILayout.Space(space);
                    GUILayout.Label($"Class {m_MLSystem.CurrentTestingClassCollected}. Number of Testing Examples: {numTestingExamples}", m_NodeSkin.GetStyle("Header Small"), GUILayout.MinWidth(300));
                    GUILayout.Space(space); 
                    GUILayout.Label($"Expected output: {expectedOutputClassString}", m_NodeSkin.GetStyle("Header Small"), GUILayout.MinWidth(200));
                    GUILayout.Space(space);
                    GUILayout.Label($"{classesCompleted}/{m_MLSystem.TotalNumUniqueClasses} Classes Completed", m_NodeSkin.GetStyle("Header Small"), GUILayout.MinWidth(200));
                    GUILayout.Space(space);

                    // Record testing examples button
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    string nameButton = "";
                    GUIStyle buttonStyle = null;
                    if (m_MLSystem.CollectingTestingData)
                    {
                        nameButton = $"Stop Recording {numTestingExamples}";
                        buttonStyle = m_NodeSkin.GetStyle("Yellow Button Long");
                    }
                    else
                    {
                        nameButton = $"Record Testing Examples for Class {m_MLSystem.CurrentTestingClassCollected}";
                        // Green button if any examples collected 
                        if (numTestingExamples > 0) buttonStyle = m_NodeSkin.GetStyle("Green Button Long");
                        // White button if empty
                        else buttonStyle = m_NodeSkin.GetStyle("White Button Long");
                    }
                    Texture icon = Resources.Load("record_examples") as Texture;
                    GUIContent content = new GUIContent(nameButton, icon);
                    if (GUILayout.Button(content, buttonStyle))
                    {
                        m_MLSystem.ToggleCollectTestingData();
                    }
                    GUILayout.Box("", m_NodeSkin.GetStyle("Record Button Green"));
                    GUILayout.Space(20);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(10);

                    // Delete testing examples button
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    if (m_MLSystem.CollectingTestingData || numTestingExamples == 0) GUI.enabled = false;                        
                    // Delete button should be red when available and white when not available
                    if (numTestingExamples > 0 && GUI.enabled == true) buttonStyle = m_NodeSkin.GetStyle("White Button Long");
                    else buttonStyle = m_NodeSkin.GetStyle("Red Button Long");
                    if (GUILayout.Button($"Delete {numTestingExamples} Testing Examples", m_NodeSkin.GetStyle("Red Button Long")))
                    {
                        m_MLSystem.DeleteTestingDataForClass(m_MLSystem.CurrentTestingClassCollected);
                    }
                    GUILayout.Box("", m_NodeSkin.GetStyle("Delete Button"));
                    GUILayout.Space(20);
                    GUILayout.EndHorizontal();

                    GUILayout.Space(20);

                    // Check whether it is the last class and we finished collecting
                    if (m_MLSystem.CurrentTestingClassCollected == m_MLSystem.TotalNumUniqueClasses-1 && m_MLSystem.AllTestingClassesCollected)
                    {
                        if (GUILayout.Button("Run Model", m_NodeSkin.GetStyle("White Button Short")))
                        {
                            m_MLSystem.StopTesting();
                            m_MLSystem.StartRunning();
                        }
                    }
                    // We still have more classes to collect
                    else
                    {
                        // Next button
                        if (GUILayout.Button("Next Testing Class", m_NodeSkin.GetStyle("White Button Short")))
                        {
                            m_MLSystem.NextTestingClass();
                        }
                    }
                    GUI.enabled = true;


                }
                // All testing classes having collected
                else
                {
                    int numTestingExamples = 0;
                    int classesCompleted = m_MLSystem.CurrentTestingClassCollected > 0 ? m_MLSystem.CurrentTestingClassCollected : 0;
                    if (m_MLSystem.TestingData != null && m_MLSystem.CurrentTestingClassCollected < m_MLSystem.TestingData.Count)
                    {
                        numTestingExamples = m_MLSystem.TestingData[m_MLSystem.CurrentTestingClassCollected].Count;
                        classesCompleted = 0;
                        foreach (var sublist in m_MLSystem.TestingData)
                        {
                            if (sublist != null && sublist.Count > 0)
                            {
                                classesCompleted++;
                            }
                        }
                    }

                    GUILayout.Space(space);
                    GUILayout.Label($"{classesCompleted}/{m_MLSystem.TotalNumUniqueClasses} Classes Completed", m_NodeSkin.GetStyle("Header Small"), GUILayout.MinWidth(200));
                    GUILayout.Space(space);

                    if (GUILayout.Button("Run Model", m_NodeSkin.GetStyle("White Button Short")))
                    {
                        m_MLSystem.StopTesting();
                        m_MLSystem.StartRunning();
                    }
                }


                GUILayout.EndArea();


                //EditorGUI.DrawRect();
            }
        }

        protected virtual void ShowRatingPanel()
        {
            // Only draw if the rating state is used and active
            if (m_MLSystem.UseRatingState && m_MLSystem.Rating)
            {
                // Avoid rating in a hotreload (needed vars are not properly populated)
                if (m_MLSystem.AllRatingQuestionsAnswered)
                {
                    // Stop rating
                    m_MLSystem.StopRating();
                    return;
                }

                // Create a new rect that occupies part of the node interface
                m_TestingUIPosition = m_BodyRect.position;
                m_TestingUIPosition.x -= 10;
                m_TestingUIWidth = HeaderRect.width + 20;
                m_TestingUIHeight = 380; // 380 is the height of the buttons in the MLSNode (roughly similar to bodyrect.height without the warning height)
                if (m_TestingUISize == null) m_TestingUISize = new Vector2();
                m_TestingUISize.x = m_TestingUIWidth;
                m_TestingUISize.y = m_TestingUIHeight;
                if (m_TestingUIRect == null) m_TestingUIRect = new Rect();
                m_TestingUIRect.position = m_TestingUIPosition;
                m_TestingUIRect.size = m_TestingUISize;


                GUILayout.BeginArea(m_TestingUIRect);

                if (m_TestingBGColor == null)
                    m_TestingBGColor = GetColorTextureFromHexString("#3b4675");
                // Draw background panel
                GUI.DrawTexture(m_TestingUIRect, m_TestingBGColor);
                //EditorGUI.DrawRect(TestingUIRect, Color.white);

                // Content of panel
                //GUILayout.Label("MACHINE LEARNING SYSTEM", m_NodeSkin.GetStyle("Header"), GUILayout.MinWidth(300));
                GUILayout.Space(120);
                GUILayout.BeginHorizontal();
                GUILayout.Label("Rate Model", m_NodeSkin.GetStyle("Header"), GUILayout.MinWidth(275));
                // Cross to close the window (only when all the data is collected)
                if (!m_MLSystem.AllRatingQuestionsAnswered) GUI.enabled = false;
                if (GUILayout.Button("", m_NodeSkin.GetStyle("Cross")))
                {
                    m_MLSystem.StopRating();
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
                int space = 5;
                GUILayout.Space(space);


                // If there are questions not yet answered...
                if (!m_MLSystem.AllRatingQuestionsAnswered)
                {
                    // Draw details of each question 
                    // Q1
                    GUILayout.Space(space);
                    GUILayout.Label($"The movement interaction is enjoyable to use", m_NodeSkin.GetStyle("Header Small"), GUILayout.MinWidth(300));
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"Disagree");
                    bool q1Answer = false;
                    q1Answer = EditorGUILayout.Toggle(q1Answer);
                    q1Answer = EditorGUILayout.Toggle(q1Answer);
                    q1Answer = EditorGUILayout.Toggle(q1Answer);
                    q1Answer = EditorGUILayout.Toggle(q1Answer);
                    q1Answer = EditorGUILayout.Toggle(q1Answer);
                    q1Answer = EditorGUILayout.Toggle(q1Answer);
                    GUILayout.Label($"Agree");
                    GUILayout.EndHorizontal();
                    
                    // Q2
                    GUILayout.Space(space);
                    GUILayout.Label($"The interaction feels and behaves as I intended", m_NodeSkin.GetStyle("Header Small"), GUILayout.MinWidth(300));
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"Disagree");
                    bool q2Answer = false;
                    q2Answer = EditorGUILayout.Toggle(q1Answer);
                    q2Answer = EditorGUILayout.Toggle(q1Answer);
                    q2Answer = EditorGUILayout.Toggle(q1Answer);
                    q2Answer = EditorGUILayout.Toggle(q1Answer);
                    q2Answer = EditorGUILayout.Toggle(q1Answer);
                    q2Answer = EditorGUILayout.Toggle(q1Answer);
                    GUILayout.Label($"Agree");
                    GUILayout.EndHorizontal();

                    // Q3
                    GUILayout.Space(space);
                    GUILayout.Label($"The model recognises my movements accurately", m_NodeSkin.GetStyle("Header Small"), GUILayout.MinWidth(300));
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"Disagree");
                    bool q3Answer = false;
                    q3Answer = EditorGUILayout.Toggle(q1Answer);
                    q3Answer = EditorGUILayout.Toggle(q1Answer);
                    q3Answer = EditorGUILayout.Toggle(q1Answer);
                    q3Answer = EditorGUILayout.Toggle(q1Answer);
                    q3Answer = EditorGUILayout.Toggle(q1Answer);
                    q3Answer = EditorGUILayout.Toggle(q1Answer);
                    GUILayout.Label($"Agree");
                    GUILayout.EndHorizontal();

                    // Q4
                    GUILayout.Space(space);
                    GUILayout.Label($"I got the results I wanted after editing InteractML", m_NodeSkin.GetStyle("Header Small"), GUILayout.MinWidth(300));
                    GUILayout.Space(space);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"Disagree");
                    bool q4Answer = false;
                    q4Answer = EditorGUILayout.Toggle(q1Answer);
                    q4Answer = EditorGUILayout.Toggle(q1Answer);
                    q4Answer = EditorGUILayout.Toggle(q1Answer);
                    q4Answer = EditorGUILayout.Toggle(q1Answer);
                    q4Answer = EditorGUILayout.Toggle(q1Answer);
                    q4Answer = EditorGUILayout.Toggle(q1Answer);
                    GUILayout.Label($"Agree");
                    GUILayout.EndHorizontal();


                    // Check whether each question has an answer but isn't submitted yet
                    // TODO

                    // Submit answers button
                    if (GUILayout.Button("Submit", m_NodeSkin.GetStyle("White Button Short")))
                    {
                        // Stop rating
                        m_MLSystem.StopRating();
                    }


                }
                // All rating questions are answered
                else
                {
                }


                GUILayout.EndArea();


                //EditorGUI.DrawRect();
            }

        }

        #endregion

    }
}
