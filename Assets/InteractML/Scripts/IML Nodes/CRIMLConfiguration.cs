using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;
using ReusableMethods;
using System;

namespace InteractML
{
    [NodeWidth(420)]
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
            RapidlibModel model = new RapidlibModel();
            switch (learningType)
            {
                case IMLSpecifications.LearningType.Classification:
                    model = new RapidlibModel(RapidlibModel.ModelType.kNN);
                    break;
                case IMLSpecifications.LearningType.Regression:
                    model = new RapidlibModel(RapidlibModel.ModelType.NeuralNetwork);
                    break;
                default:
                    break;
            }
            return model;
        }


        public override void TrainModel()
        {
            RunningLogic();
            // if there are no training examples in connected training nodes do not train 
           if(m_TotalNumTrainingData == 0)
            {
                Debug.Log("no training examples");
            }
            else
            {

                // Transform the IML Training Examples into a format suitable for Rapidlib
                m_RapidlibTrainingExamples = TransformIMLDataToRapidlib(IMLTrainingExamplesNodes);

                // Trains rapidlib with the examples added
                m_Model.Train(m_RapidlibTrainingExamples);

                //Debug.Log("***Retraining IML Config node with num Examples: " + RapidLibComponent.trainingExamples.Count + " Rapidlib training succesful: " + RapidLibComponent.Trained + "***");
            
            }
          
        }

        /// <summary>
        /// Loads the current model from disk (dataPath specified in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public override void LoadModelFromDisk()
        {
            m_Model.LoadModelFromDisk(this.graph.name + "_IMLConfiguration" + this.id);
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

#endregion

    }
}
