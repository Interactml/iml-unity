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
    [CreateNodeMenuAttribute("Interact ML/Machine Learning System/MLS DTW")]
    public class DTWIMLConfiguration : IMLConfiguration
    {

        #region Variables

        #endregion

        #region XNode Messages
        // Override Init to set learning type as dtw
        protected override void Init()
        {
            SetLearningType();
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

                m_RapidlibTrainingSeriesCollection = TransformIMLSeriesToRapidlib(IMLTrainingExamplesNodes, out m_NumExamplesTrainedOn);
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
                bool isNotTrainingExamplesNode = this.DisconnectIfNotType<DTWIMLConfiguration, SeriesTrainingExamplesNode>(from, to);

                // If we broke the connection...
                if (isNotTrainingExamplesNode)
                {
                    // Prepare flag to show error regarding training examples
                    m_ErrorWrongInputTrainingExamplesPort = true;
                }
                // If we accept the connection...
                else
                {
                    SeriesTrainingExamplesNode examplesNode = from.node as SeriesTrainingExamplesNode;

                    if(examplesNode.TargetValues.Count > 1)
                    {
                        from.Disconnect(to);
                        m_WrongNumberOfTargetValues = true; 
                    }

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
