using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices; 

namespace InteractML
{
    /// <summary>
    /// Higher level class that holds a model, its training set and methods to deal with both. 
    /// Use this if you want more functionalities than just a model
    /// </summary>
    public class EasyRapidlib
    {

        #region Variables

        /// <summary>
        /// Pointer to the rapidlib dll model
        /// </summary>
        RapidlibModel m_Model;

        /// <summary>
        /// Options of learning type in Rapidlib
        /// </summary>
        public enum LearningType { Classification, Regression, DTW };

        /// <summary>
        /// The specific learning type selected
        /// </summary>
        private LearningType m_LearningType;

        /// <summary>
        /// The list of training examples (for classification and regression)
        /// </summary>
        private List<RapidlibTrainingExample> m_TrainingExamples;

        /// <summary>
        /// List of different training examples series (for DTW)
        /// </summary>
        private List<RapidlibTrainingSerie> m_TrainingExamplesSeries;

        private bool run = false;
        /// <summary>
        /// Is the model running?
        /// </summary>
        public bool Running { get { return run; } }

        private bool collectData = false;
        /// <summary>
        /// Can the model collect data?
        /// </summary>
        public bool CollectingData { get { return collectData; } }

        private bool train = false;
        /// <summary>
        /// Is the model training?
        /// </summary>
        public bool Training { get { return train; } }
        /// <summary>
        /// Is the model trained?
        /// </summary>
        public bool Trained { get; private set; }

        /// <summary>
        /// String containing the model in a JSON format
        /// </summary>
        public string jsonModelString = "";

        /// <summary>
        /// Data Path to stored model in disk
        /// </summary>
        private string m_ModelDataPath;
        /// <summary>
        /// Data path to stored training examples in disk
        /// </summary>
        private string m_TrainingExamplesDataPath;

        #endregion

        #region Constructors

        public EasyRapidlib()
        {
            Initialize();
        }

        public EasyRapidlib(LearningType learningType)
        {
            m_LearningType = learningType;
            Initialize();
        }

        public EasyRapidlib(LearningType learningType, string dataPathModel)
        {
            m_LearningType = learningType;
            m_ModelDataPath = dataPathModel;            
            Initialize();
        }

        public EasyRapidlib(LearningType learningType, string dataPathModel, string dataPathTrainingSet)
        {
            m_LearningType = learningType;
            m_ModelDataPath = dataPathModel;
            m_TrainingExamplesDataPath = dataPathTrainingSet;
            Initialize();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads both model and training set from disk (will only load the ones that have a datapath)
        /// </summary>
        /// <param name="jsonModel"></param>
        /// <param name="trainingExamplesList"></param>
        /// <param name="dataPathModel"></param>
        /// <param name="dataPathTrainingExamples"></param>
        /// <returns>True if managed to load everything, false otherwise</returns>
        public bool LoadModelAndTrainingDataFromDisk(ref string jsonModel, ref List<RapidlibTrainingExample> trainingExamplesList, string dataPathModel, string dataPathTrainingExamples)
        {
            bool successLoadingAll = true;
            // Only load data if we have the datapaths
            if (!String.IsNullOrEmpty(dataPathModel))
                jsonModel = LoadModelFromDisk(dataPathModel);
            else
                successLoadingAll = false;

            if (!String.IsNullOrEmpty(dataPathTrainingExamples))
                trainingExamplesList = LoadTrainingSetFromDisk(dataPathTrainingExamples);
            else
                successLoadingAll = false;

            // Succesfully loaded everything?
            return successLoadingAll;

        }

        public RapidlibModel CreateClassificationModel()
        {
            return new RapidlibModel(RapidlibModel.ModelType.kNN); ;
        }

        public RapidlibModel CreateRegressionModel()
        {
            return new RapidlibModel(RapidlibModel.ModelType.NeuralNetwork);
        }

        public RapidlibModel CreateTimeSeriesClassificationModel()
        {
            return new RapidlibModel(RapidlibModel.ModelType.DTW);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialises rapidlib class to default values
        /// </summary>
        private void Initialize()
        {
            // Set the trained flag to false
            Trained = false;

            // Code to initialize model 

            m_Model = CreateRapidlibModel(m_LearningType);

            // We load the model and the training set from disk as json string
            LoadModelAndTrainingDataFromDisk(ref jsonModelString, ref m_TrainingExamples, m_ModelDataPath, m_TrainingExamplesDataPath);

            //Debug.Log("Training Examples list Count: " + trainingExamples.Count);

            // We configure the model with the data from the json string
            m_Model.ConfigureModelWithJson(jsonModelString);

        }
        
        /// <summary>
        /// Creates a rapidlib model based on the learning type specified
        /// </summary>
        /// <param name="learningType"></param>
        /// <returns></returns>
        private RapidlibModel CreateRapidlibModel(LearningType learningType)
        {
            RapidlibModel tempModel;

            // We create a different model depending on what kind of learning type is selected
            switch (learningType)
            {
                case LearningType.Classification:
                    tempModel = CreateClassificationModel(); ;
                    break;
                case LearningType.Regression:
                    tempModel = CreateRegressionModel(); ;
                    break;
                case LearningType.DTW:
                    tempModel = CreateTimeSeriesClassificationModel(); ;
                    break;
                default:
                    throw new Exception("Error: Unkown learning type in RapidlibClass");
                    break;
            }

            return tempModel;

        }

        /* === Model IO Methods === */

        /// <summary>
        /// Saves the model to disk
        /// </summary>
        /// <param name="modelJSON">The model in JSON format</param>
        /// <param name="filePath">The full file path (including the filename itself)</param>
        private void SaveModelToDisk(string modelJSON, string filePath)
        {
            IMLDataSerialization.SaveRapidlibModelToDisk(modelJSON, filePath);
        }

        /// <summary>
        /// Loads a model from disk at the specified path
        /// </summary>
        /// <param name="filePath">The complete file path, including the name of the file</param>
        /// <returns>Returns a string JSON containing the model configuration </returns>
        private string LoadModelFromDisk(string filePath)
        {
            return IMLDataSerialization.LoadRapidlibModelFromDisk(filePath);
        }

        /* === Training Examples IO Methods === */

        /// <summary>
        /// Saves Training Data Set to disk
        /// </summary>
        /// <param name="listToSave">The list of training examples</param>
        /// <param name="filePath">File path without file extension</param>
        private void SaveTrainingSetToDisk(List<RapidlibTrainingExample> listToSave, string filePath)
        {
            IMLDataSerialization.SaveTrainingSetToDiskRapidlib(listToSave, filePath);
        }


        /// <summary>
        /// Loads Training Data Set from Disk
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Returns a list with training set</returns>
        private List<RapidlibTrainingExample> LoadTrainingSetFromDisk(string filePath)
        {
            return IMLDataSerialization.LoadTrainingSetFromDiskRapidlib(filePath);
        }




        #endregion

    }

}
