using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace InteractML
{
    /// <summary>
    /// Child class. Handles communication with a specific Training Examples Node
    /// </summary>
    public class IMLTrainingExamplesUI : IMLUI<TrainingExamplesNode>
    {
        #region Variables
        [Header("Training Examples Node Setup")]
        public TextMeshProUGUI CollectExamplesButtonText;
        public TextMeshProUGUI TrainingExamplesNodeIndexText;
        public TextMeshProUGUI NoTrainingExamplesText;
        public TextMeshProUGUI NoInputsText;
        public TextMeshProUGUI NoOutputsText;

        [Header("Training Examples Node Vars")]
        [SerializeField]
        private int m_NoDesiredOutputsNode;

        #endregion

        #region Unity Messages
        // Start is called before the first frame update
        void Start()
        {
            // Find a reference of an IML Component to work with
            if (MLComponent == null)
                MLComponent = FindObjectOfType<IMLComponent>();

            if (MLComponent)
                m_CurrentNode = GetNode(m_NodeIndex, MLComponent.TrainingExamplesNodesList);
        }

        // Update is called once per frame
        void Update()
        {
            // Attempt to get node
            if (!m_CurrentNode)
                m_CurrentNode = GetNode(m_NodeIndex, MLComponent.TrainingExamplesNodesList);

            // Update UI that can change from the IML Controller
            if (m_CurrentNode)
            {
                // Node Index
                UpdateUIText(TrainingExamplesNodeIndexText, m_NodeIndex.ToString());
                // No Training Examples
                UpdateUIText(NoTrainingExamplesText, m_CurrentNode.TotalNumberOfTrainingExamples.ToString());
                // Inputs
                UpdateUIText(NoInputsText, m_CurrentNode.InputFeatures.Count.ToString());
                // No Outputs
                UpdateUIText(NoOutputsText, m_CurrentNode.DesiredOutputFeatures.Count.ToString());

                // Check if it is collecting examples to update button text
                if (m_CurrentNode.CollectingData)
                {
                    UpdateUIText(CollectExamplesButtonText, "STOP Recording Examples");
                }
                else
                {
                    UpdateUIText(CollectExamplesButtonText, "Record Examples");
                }
            }

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Toggles connection of examples
        /// </summary>
        public void ToggleCollectingTrainingExamples()
        {
            if (!MLComponent)
                return;

            if (!m_CurrentNode)
                m_CurrentNode = GetNode(m_NodeIndex, MLComponent.TrainingExamplesNodesList);

            ToggleCollectingExamples(m_CurrentNode);
        }

        /// <summary>
        /// Records only ONE example
        /// </summary>
        public void RecordSingleExample()
        {
            if (!MLComponent)
                return;

            if (!m_CurrentNode)
                m_CurrentNode = GetNode(m_NodeIndex, MLComponent.TrainingExamplesNodesList);

            m_CurrentNode.AddSingleTrainingExample();
        }

        /// <summary>
        /// Removes all training examples
        /// </summary>
        public void DeleteAllTrainingExamples()
        {
            if (!MLComponent)
                return;

            if (!m_CurrentNode)
                m_CurrentNode = GetNode(m_NodeIndex, MLComponent.TrainingExamplesNodesList);

            m_CurrentNode.ClearTrainingExamples();

        }

        /// <summary>
        /// Exposes publicly parent method to move to next node
        /// </summary>
        public void MoveToNextNode()
        {
            MoveToNextNode(MLComponent.TrainingExamplesNodesList);
        }

        /// <summary>
        /// Exposes publicly parent method to move to previous node
        /// </summary>
        public void MoveToPreviousNode()
        {
            MoveToPreviousNode(MLComponent.TrainingExamplesNodesList);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Toggles the collection of training examples in a specific node
        /// </summary>
        /// <param name="whichNode"></param>
        private void ToggleCollectingExamples(int whichNode)
        {
            if (MLComponent)
            {
                if (MLComponent.TrainingExamplesNodesList.Count > 0)
                {
                    MLComponent.TrainingExamplesNodesList[whichNode].ToggleCollectExamples();
                    if (CollectExamplesButtonText)
                    {
                        if (MLComponent.TrainingExamplesNodesList[whichNode].CollectingData)
                        {
                            CollectExamplesButtonText.text = "STOP Recording Examples";
                        }
                        else
                        {
                            CollectExamplesButtonText.text = "Record Examples";
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Toggles the collection of training examples in a specific node
        /// </summary>
        private void ToggleCollectingExamples(TrainingExamplesNode node)
        {
            node.ToggleCollectExamples();
        }
        #endregion
    }

}
