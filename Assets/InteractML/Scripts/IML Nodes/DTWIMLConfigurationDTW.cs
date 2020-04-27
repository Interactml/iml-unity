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
    public class DTWIMLConfiguration : IMLConfiguration
    {

        #region Variables

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
            model = new RapidlibModel(RapidlibModel.ModelType.DTW);
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
                if (m_RapidlibTrainingSeriesCollection == null)
                    m_RapidlibTrainingSeriesCollection = new List<RapidlibTrainingSerie>();

                m_RapidlibTrainingSeriesCollection = TransformIMLSeriesToRapidlib(IMLTrainingExamplesNodes);
                m_Model.Train(m_RapidlibTrainingSeriesCollection);

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
                    Debug.Log("This node is meant to only be DTW you are trying to implement classification");
                    break;
                case RapidlibModel.ModelType.NeuralNetwork:
                    m_LearningType = IMLSpecifications.LearningType.Regression;
                    Debug.Log("This node is meant to only be DTW you are trying to implement regression");
                    break;
                case RapidlibModel.ModelType.DTW:
                    m_LearningType = IMLSpecifications.LearningType.DTW;
                    // DTW model will need to retrain!
                    Debug.Log("DTW RETRAINING WHEN LOADING MODEL NOT IMPLEMENTED YET!");
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
            m_LearningType = IMLSpecifications.LearningType.DTW;
        }

        protected override void RunningLogic()
        {
            //RunModelDTW(m_RunningSeries);
            CollectFeaturesInRunningSeries(InputFeatures, ref m_RunningSeries);
        }

        protected override bool CheckOutputConfiguration()
        {
            bool output = false;
            if (m_NodeConnectionChanged
                || PredictedOutput.Any((i => (i == null || (i.Values == null || i.Values.Length == 0)))))
            {
                output = true;
            }
            return output;
        }

        protected override void OverrideModel(IMLSpecifications.LearningType learningType)
        {
            m_Model = new RapidlibModel(RapidlibModel.ModelType.DTW);
        }

#endregion

    }
}
