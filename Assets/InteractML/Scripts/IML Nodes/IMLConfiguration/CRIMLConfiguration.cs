using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;
using ReusableMethods;
using System;

namespace InteractML
{
    [NodeWidth(300)]
    [CreateNodeMenuAttribute("Interact ML/Machine Learning System/MLS Classification Regression")]
    public class CRIMLConfiguration: IMLConfiguration
    {

        #region Variables
        
        public enum CR_LearningChoice { Classification, Regression};
        public CR_LearningChoice learningChoice;

        #endregion

        #region XNode Messages

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
            // Switch local learning type
            switch (learningType)
            {
                case IMLSpecifications.LearningType.Classification:
                    learningChoice = CR_LearningChoice.Classification;
                    break;
                case IMLSpecifications.LearningType.Regression:
                    learningChoice = CR_LearningChoice.Regression;
                    break;
                default:
                    break;
            }
            // Update local and parent learning type
            SetLearningType();
            // Return new model
            return base.InstantiateRapidlibModel(learningType);
        }

        /// <summary>
        /// Loads the current model from disk (dataPath specified in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public override bool LoadModelFromDisk(bool reCreateModel = false)
        {
            bool success = false;

            // Make sure to re-instantiate the model if null or flag is true
            if (m_Model == null || reCreateModel)
                m_Model = InstantiateRapidlibModel(LearningType);

            success = m_Model.LoadModelFromDisk(this.graph.name + "_IMLConfiguration" + this.id, reCreateModel);
            // We update the node learning type to match the one from the loaded model
            switch (m_Model.TypeOfModel)
            {
                case RapidlibModel.ModelType.kNN:
                    m_LearningType = IMLSpecifications.LearningType.Classification;
                    // Configure inputs and outputs
                    PredictedOutput = new List<IMLBaseDataType>(m_Model.GetNumExpectedOutputs());
                    // TO DO
                    // Still left to configure inputs
                    // Still left to configure the type of the inputs and outputs
                    break;
                case RapidlibModel.ModelType.NeuralNetwork:
                    m_LearningType = IMLSpecifications.LearningType.Regression;
                    // Configure inputs and outputs
                    PredictedOutput = new List<IMLBaseDataType>(m_Model.GetNumExpectedOutputs());
                    // TO DO
                    // Still left to configure inputs
                    // Still left to configure the type of the inputs and outputs
                    break;
                case RapidlibModel.ModelType.DTW:
                    // DTW model will need to retrain!
                    Debug.Log("This node is meant to only configure classificationa and regression. Loaded data is DTW");
                    break;
                case RapidlibModel.ModelType.None:
                    break;
                default:
                    break;
            }

            return success;
        }

        #endregion

        #region Protected Methods
        protected override void SetLearningType()
        {
            if (learningChoice == CR_LearningChoice.Classification)
            {
                m_LearningType = IMLSpecifications.LearningType.Classification;

            } else
            {
                m_LearningType = IMLSpecifications.LearningType.Regression;
            }
            
        }


        protected override void RunningLogic()
        {
            PredictedRapidlibOutput = RunModel();
            // Don't run when the predicteb rapidlib output is empty (this is a patch for some bug that broke the predictions)
            if (PredictedRapidlibOutput.Length != 0)
                // Transform rapidlib output to IMLTypes (calling straight after getting the output so that the UI can show properly)
                TransformPredictedOuputToIMLTypes(PredictedRapidlibOutput, ref PredictedOutput);
        }

        protected override bool CheckOutputConfiguration()
        {
            bool output = false;
            if (m_LastKnownRapidlibOutputVectorSize == 0
            || m_LastKnownRapidlibOutputVectorSize != PredictedRapidlibOutput.Length
            || m_NodeConnectionChanged
            || (m_LastKnownRapidlibOutputVectorSize > 0 && PredictedOutput.Count == 0))
               output = true;
            return output;
        }
        protected override void OverrideModel(IMLSpecifications.LearningType learningType)
        {
            switch (learningType)
            {
                case IMLSpecifications.LearningType.Classification:
                    m_Model = new RapidlibModel(RapidlibModel.ModelType.kNN);
                    break;
                case IMLSpecifications.LearningType.Regression:
                    m_Model = new RapidlibModel(RapidlibModel.ModelType.NeuralNetwork);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Override training Examples to only check for the single training examples type
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="portName"></param>
        protected override void CheckTrainingExamplesConnections(XNode.NodePort from, XNode.NodePort to, string portName)
        {
            // Evaluate the nodeport for training examples
            if (to.fieldName == portName)
            {
                // Check if the node connected was a training examples node
                bool isNotTrainingExamplesNode = this.DisconnectIfNotType<CRIMLConfiguration, SingleTrainingExamplesNode>(from, to);

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
