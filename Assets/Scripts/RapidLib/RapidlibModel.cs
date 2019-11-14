using System;

namespace InteractML
{
    /// <summary>
    /// Holds address and configuration for a rapidlibmodel in memory
    /// </summary>
    public struct RapidlibModel
    {
        #region Variables

        IntPtr m_ModelAddress;
        /// <summary>
        /// The address in memory of the rapidlib model (used to interface with the rapidlib DLL)
        /// </summary>
        public IntPtr ModelAddress { get => m_ModelAddress; }

        string m_ModelJSONString;
        /// <summary>
        /// The json string containing all info of a model
        /// </summary>
        public string ModelJSONString { get => m_ModelJSONString; set => m_ModelJSONString = value; }

        /// <summary>
        /// Different types of model available
        /// </summary>
        public enum ModelType { kNN, NeuralNetwork, DTW, None }

        private ModelType m_TypeOfModel;
        /// <summary>
        /// Which model is this one?
        /// </summary>
        public ModelType TypeOfModel { get => m_TypeOfModel; set => m_TypeOfModel = value; }

        #endregion

        #region Struct Constructor

        public RapidlibModel(ModelType modelToCreate)
        {
            // Set default values
            m_ModelAddress = (IntPtr)0;
            m_ModelJSONString = "";
            m_TypeOfModel = modelToCreate;

            // Creates the specific model type
            switch (modelToCreate)
            {
                case ModelType.kNN:
                    CreateClassificationModel();
                    break;
                case ModelType.NeuralNetwork:
                    CreateRegressionModel();
                    break;
                case ModelType.DTW:
                    CreateTimeSeriesClassificationModel();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Destroys the model we have a reference for and set the address to none
        /// </summary>
        public void DestroyModel()
        {
            // If we have an address to destroy...
            if ((int)m_ModelAddress != 0)
            {
                // If our model is dtw, we need to call the specific destructor
                if (m_TypeOfModel == ModelType.DTW)
                {
                    RapidlibLinkerDLL.DestroySeriesClassificationModel(m_ModelAddress);
                }
                // If our model is either classification or regression, we call the other generic destructor
                else
                {
                    RapidlibLinkerDLL.DestroyModel(m_ModelAddress);
                }
            }
            // We set the pointer to none
            m_ModelAddress = (IntPtr)0;
            // We set the json string to nothing
            m_ModelJSONString = "";
            // We set the model type to none
            m_TypeOfModel = ModelType.None;

        }

        /// <summary>
        /// Configures the model with data from the json string
        /// </summary>
        /// <param name="jsonstring"></param>
        public void ConfigureModelWithJson(string jsonstring)
        {
            RapidlibLinkerDLL.PutJSON(m_ModelAddress, jsonstring);
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a classification model in rapidlib and holds its memory address
        /// </summary>
        public void CreateClassificationModel()
        {
            // We first make sure that we don't have any model in memory...
            DestroyModel();
            // We create the new model in memory and get its address
            m_ModelAddress = RapidlibLinkerDLL.CreateClassificationModel();
            // We set the type of model to kNN
            m_TypeOfModel = ModelType.kNN;
        }

        /// <summary>
        /// Creates a regression model in rapidlib and holds its memory address
        /// </summary>
        public void CreateRegressionModel()
        {
            // We first make sure that we don't have any model in memory...
            DestroyModel();
            // We create the new model in memory and get its address
            m_ModelAddress = RapidlibLinkerDLL.CreateRegressionModel();
            // We set the type of model to kNN
            m_TypeOfModel = ModelType.NeuralNetwork;
        }

        /// <summary>
        /// Creates a timeSeriesClassification (DTW) model in rapidlib and holds its memory address
        /// </summary>
        public void CreateTimeSeriesClassificationModel()
        {
            // We first make sure that we don't have any model in memory...
            DestroyModel();
            // We create the new model in memory and get its address
            m_ModelAddress = RapidlibLinkerDLL.CreateSeriesClassificationModel();
            // We set the type of model to kNN
            m_TypeOfModel = ModelType.DTW;
        }


        #endregion
    }
}
