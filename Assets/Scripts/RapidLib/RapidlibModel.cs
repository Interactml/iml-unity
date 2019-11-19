using System;

namespace InteractML
{
    /// <summary>
    /// Holds address and configuration for a rapidlibmodel in memory. 
    /// Very basic and clean. Use if you are implementing training logic yourself
    /// </summary>
    public class RapidlibModel
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
        public string ModelJSONString { get => m_ModelJSONString; }

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

        #region Constructor

        /// <summary>
        /// Creates an empty instance without any model
        /// </summary>
        public RapidlibModel()
        {
            // Set default values
            m_ModelAddress = (IntPtr)0;
            m_ModelJSONString = "";
            m_TypeOfModel = ModelType.None;

        }

        /// <summary>
        /// Creates a specific model type
        /// </summary>
        /// <param name="modelToCreate"></param>
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

        #region Destructor

        ~RapidlibModel()
        {
            // Make sure to destroy the model when the class is collected by the GC
            DestroyModel();
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

        /// <summary>
        /// Saves the current model with the desired fileName (filePath configured in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveModelToDisk(string fileName)
        {
            // Transforms the model into a stylised json string
            m_ModelJSONString = RapidlibLinkerDLL.GetJSON(m_ModelAddress);
            // Saves the model as a stylised json with the specified fileName
            IMLDataSerialization.SaveRapidlibModelToDisk(m_ModelJSONString, fileName);
        }

        /// <summary>
        /// Loads the model with the specified name into this instance
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadModelFromDisk(string fileName)
        {
            // Attempt to load model
            string stringLoaded = IMLDataSerialization.LoadRapidlibModelFromDisk(fileName);

            // If we loaded something...
            if (!String.IsNullOrEmpty(stringLoaded))
            {
                // We configure our model with the configuration loaded
                ConfigureModelWithJson(stringLoaded);
                // We store the loaded string 
                m_ModelJSONString = stringLoaded;
            }
            // If we couldn't load anything, throw exception
            else
            {
                throw new Exception("Couldn't load rapidlib model because nothing was found in provided path: " + fileName);
            }
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
