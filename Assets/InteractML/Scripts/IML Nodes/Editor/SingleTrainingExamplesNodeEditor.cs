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
    /// <summary>
    /// Editor for Single Training Examples Node known as Teach the Machine Classification and Regression
    /// </summary>
    [CustomNodeEditor(typeof(SingleTrainingExamplesNode))]
    public class SingleTrainingExamplesNodeEditor : TrainingExamplesNodeEditor
    {

        #region Variables 

        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private SingleTrainingExamplesNode m_SingleTrainingExamplesNode;

        #endregion

        public override void OnHeaderGUI()
        {
            baseNodeBodyHeight = 360;
            base.OnHeaderGUI();
            // Get reference to the current node
            m_SingleTrainingExamplesNode = m_TrainingExamplesNode as SingleTrainingExamplesNode;
            string arrayNo = "";
            //array number of training node 
            if (m_SingleTrainingExamplesNode.numberInComponentList != -1)
                arrayNo = m_SingleTrainingExamplesNode.numberInComponentList.ToString();
            NodeName = "TEACH THE MACHINE " + arrayNo;
            NodeSubtitle = "Classification & Regression Training Examples";
        }

        /// <summary>
        /// Show the load, delete and record buttons
        /// </summary>
        protected override void ShowButtons()
        {
            int spacing = 75;
            GUILayout.Space(40);

            // show record ONE example button
            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing);
            if (GUILayout.Button(new GUIContent("Record One \n example"), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record One Button")))
            {
                m_SingleTrainingExamplesNode.AddSingleTrainingExample();
            }
            //button tooltip code 
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                buttonTipHelper = true;
                TooltipText = m_SingleTrainingExamplesNode.tooltips.BodyTooltip.Tips[2];
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

            // show record examples button
            GUILayout.Space(spacing);
            string recordNameButton = ShowRecordExamplesButton();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing -10);
            GUILayout.Label("record one \nexample", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green"));
            GUILayout.Label("");
            
            GUILayout.Label(recordNameButton, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Record Button Green"));
            GUILayout.Space(spacing-10);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing);
            ShowClearAllExamplesButton();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing-10);
            GUILayout.Label("delete all \n recordings", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Delete Button Pink"));
            GUILayout.Label("");
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(spacing-10);
            GUILayout.Label("No of training pairs: " + m_SingleTrainingExamplesNode.TrainingExamplesVector.Count, Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Header Small"));
            GUILayout.EndHorizontal();


        }

     

        /// <summary>
        /// Show single training examples on foldout arrow
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
                

                if (ReusableMethods.Lists.IsNullOrEmpty(ref m_SingleTrainingExamplesNode.TrainingExamplesVector))
                {
                    EditorGUILayout.LabelField("Training Examples List is empty", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("foldoutempty"));
                }
                else
                {
                    // Begins Vertical Scroll
                    int indentLevel = EditorGUI.indentLevel;
                    EditorGUILayout.BeginVertical();
                    m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(GetWidth()-10), GUILayout.Height(GetWidth() - 50));

                    for (int i = 0; i < m_SingleTrainingExamplesNode.TrainingExamplesVector.Count; i++)
                    {
                        EditorGUILayout.LabelField("Training Example " + (i + 1), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                        EditorGUI.indentLevel++;

                        var inputFeatures = m_SingleTrainingExamplesNode.TrainingExamplesVector[i].Inputs;
                        var outputFeatures = m_SingleTrainingExamplesNode.TrainingExamplesVector[i].Outputs;

                        // If the input features are not null...
                        if (inputFeatures != null)
                        {
                            // Draw inputs
                            for (int j = 0; j < inputFeatures.Count; j++)
                            {

                                if (inputFeatures[j].InputData == null)
                                {
                                    EditorGUILayout.LabelField("Inputs are null ");
                                    break;
                                }


                                EditorGUI.indentLevel++;

                                EditorGUILayout.LabelField("Input " + (j + 1) + " (" + inputFeatures[j].InputData.DataType + ")", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                for (int k = 0; k < inputFeatures[j].InputData.Values.Length; k++)
                                {
                                    EditorGUI.indentLevel++;

                                    EditorGUILayout.LabelField(inputFeatures[j].InputData.Values[k].ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                    EditorGUI.indentLevel--;
                                }

                                EditorGUI.indentLevel--;
                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Inputs are null ", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                        }

                        // If the output features are not null...
                        if (outputFeatures != null)
                        {
                            // Draw outputs
                            for (int j = 0; j < outputFeatures.Count; j++)
                            {
                                if (outputFeatures[j].OutputData == null)
                                {
                                    EditorGUILayout.LabelField("Outputs are null ", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
                                    break;
                                }


                                EditorGUI.indentLevel++;

                                EditorGUILayout.LabelField("Output " + (j + 1) + " (" + outputFeatures[j].OutputData.DataType + ")", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));


                                for (int k = 0; k < outputFeatures[j].OutputData.Values.Length; k++)
                                {

                                    EditorGUI.indentLevel++;

                                    EditorGUILayout.LabelField(outputFeatures[j].OutputData.Values[k].ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));

                                    EditorGUI.indentLevel--;

                                }

                                EditorGUI.indentLevel--;

                            }
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Outputs are null ", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("scrollview"));
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
