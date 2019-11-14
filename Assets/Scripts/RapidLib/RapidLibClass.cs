using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices; 

namespace InteractML
{
    public class RapidLibClass
    {

        #region Variables

        /// <summary>
        /// Pointer to the rapidlib dll model
        /// </summary>
        RapidlibModel model;

        /// <summary>
        /// Options of learning type in Rapidlib
        /// </summary>
        public enum LearningType { Classification, Regression, DTW };

        /// <summary>
        /// The specific learning type selected
        /// </summary>
        public LearningType learningType;

        /// <summary>
        /// The list of training examples (for classification and regression)
        /// </summary>
        public List<RapidlibTrainingExample> trainingExamples;

        /// <summary>
        /// List of different training examples series (for DTW)
        /// </summary>
        public List<TrainingSeries> trainingSerieses;

        public bool run = false;
        /// <summary>
        /// Is the model running?
        /// </summary>
        public bool Running { get { return run; } }

        public bool collectData = false;
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
        public string jsonString = "";

        /// <summary>
        /// Data Path to stored model in disk
        /// </summary>
        private string modelDataPath;
        /// <summary>
        /// Data path to stored training examples in disk
        /// </summary>
        private string trainingExamplesDataPath;

        #endregion

        #region Constructors

        public RapidLibClass()
        {

        }

        #endregion

        #region Destructor

        ~RapidLibClass()
        {
            // We make sure to destroy the model when the class gets destroyed
            model.DestroyModel();
        }

        #endregion


        #region Public Methods

        public void LoadDataFromDiskAndReturnModel(ref string jsonModel, ref List<RapidlibTrainingExample> trainingExamplesList, string dataPathModel, string dataPathTrainingExamples)
        {
            // We load the model and the training set from disk
            jsonModel = LoadModelFromDisk(dataPathModel);
            trainingExamplesList = LoadTrainingSetFromDisk(dataPathTrainingExamples);
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

            model = CreateRapidlibModel(learningType);


            // We load the model and the training set from disk as json string
            LoadDataFromDiskAndReturnModel(ref jsonString, ref trainingExamples, modelDataPath, trainingExamplesDataPath);

            //Debug.Log("Training Examples list Count: " + trainingExamples.Count);

            // We configure the model with the data from the json string
            model.ConfigureModelWithJson(jsonString);

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
