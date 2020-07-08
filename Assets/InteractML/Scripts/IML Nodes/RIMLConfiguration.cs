using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;
using ReusableMethods;
using System;

namespace InteractML
{
    [CreateNodeMenuAttribute("Interact ML/Machine Learning System/MLS Regression")]
    [NodeWidth(300)]
    public class RIMLConfiguration: CRIMLConfiguration
    {

        #region Variables


        #endregion

        #region XNode Messages
        // Override Init to set learning type as regression
        protected override void Init()
        {
            SetLearningType();
            tooltips = IMLTooltipsSerialization.LoadTooltip("Regression_MLS");
            base.Init();
        }
        #endregion

        #region Unity Messages


        #endregion

        #region Public Methods

        /// <summary>
        /// Instantiates a rapidlibmodel
        /// </summary>
        /// <param name="learningType"></param>
        public override RapidlibModel InstantiateRapidlibModel(IMLSpecifications.LearningType learningType)
        {
            SetLearningType();
            return base.InstantiateRapidlibModel(learningType);
        }

        /// <summary>
        /// Loads the current model from disk (dataPath specified in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public override void LoadModelFromDisk(bool reCreateModel = false)
        {
            m_Model.LoadModelFromDisk(this.graph.name + "_IMLConfiguration" + this.id, reCreateModel);
            // We update the node learning type to match the one from the loaded model
            m_LearningType = IMLSpecifications.LearningType.Regression;
            // Configure inputs and outputs
            PredictedOutput = new List<IMLBaseDataType>(m_Model.GetNumExpectedOutputs());
            // TO DO
            // Still left to configure inputs
            // Still left to configure the type of the inputs and outputs
        }

        #endregion

        #region Protected Methods
        protected override void SetLearningType()
        {            
            m_LearningType = IMLSpecifications.LearningType.Regression;
            learningChoice = CR_LearningChoice.Regression;            
        }

        protected override void OverrideModel(IMLSpecifications.LearningType learningType)
        {
            m_Model = new RapidlibModel(RapidlibModel.ModelType.NeuralNetwork);
        }

        /// <summary>
        /// Override training Examples to only check for the single training examples type
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="portName"></param>
        protected override void CheckTrainingExamplesConnections(NodePort from, NodePort to, string portName)
        {
            // Evaluate the nodeport for training examples
            if (to.fieldName == portName)
            {
                // Check if the node connected was a training examples node
                bool isNotTrainingExamplesNode = this.DisconnectIfNotType<RIMLConfiguration, SingleTrainingExamplesNode>(from, to);

                // If we broke the connection...
                if (isNotTrainingExamplesNode)
                {
                    // Prepare flag to show error regarding training examples
                    m_ErrorWrongInputTrainingExamplesPort = true;
                }
                // If we accept the connection...
                else
                {
                    SingleTrainingExamplesNode examplesNode = from.node as SingleTrainingExamplesNode;
                    // We check that the connection is from a training examples node
                    if (examplesNode != null)
                    {
                        // Update dynamic ports for output
                        AddDynamicOutputPorts(examplesNode, ref m_DynamicOutputPorts);
                    }

                }

            }

        }

        #endregion

    }
}
