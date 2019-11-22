using System;
using System.Collections.Generic;

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

        private IMLSpecifications.ModelStatus m_ModelStatus;
        /// <summary>
        /// The current status of the model (running, trained, training)
        /// </summary>
        public IMLSpecifications.ModelStatus ModelStatus { get => m_ModelStatus; }


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
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;

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
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;

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

        /* RUNNING LOGIC */

        /// <summary>
        /// Runs the model and provides the output predictions (classification and regression)
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputPredictions">The </param>
        public void Run(double[] inputs, ref double[] outputPredictions)
        {
            // We run only classification and regression with the data passed in
            switch (m_TypeOfModel)
            {
                case ModelType.kNN:
                    RapidlibLinkerDLL.Process(m_ModelAddress,
                        inputs, inputs.Length,
                        ref outputPredictions, outputPredictions.Length);
                    break;
                case ModelType.NeuralNetwork:
                    RapidlibLinkerDLL.Process(m_ModelAddress,
                        inputs, inputs.Length,
                        ref outputPredictions, outputPredictions.Length);
                    break;
                case ModelType.DTW:
                    throw new Exception("Wrong format of data for DTW model! Are you trying to run a classification or regression?");
                case ModelType.None:
                    throw new Exception("You can't run an unitialised model!");
                default:
                    break;
            }

            // We mark the model as running
            m_ModelStatus = IMLSpecifications.ModelStatus.Running;
        }
        
        /// <summary>
        /// Runs the model and provides the closest trainingSerie the model is trained with
        /// </summary>
        /// <param name="inputSerie"></param>
        /// <returns></returns>
        public int Run(RapidlibTrainingSerie inputSerie)
        {
            int outputDTW = -1;
            // We only run DTW with the data passed in
            switch (m_TypeOfModel)
            {
                case ModelType.kNN:
                    throw new Exception("Wrong format of data for Classification model! Are you trying to run a DTW?");
                case ModelType.NeuralNetwork:
                    throw new Exception("Wrong format of data for Regression model! Are you trying to run a DTW?");
                case ModelType.DTW:
                    IntPtr dtwSerie = TransformTrainingSerieForRapidlib(this, inputSerie);
                    outputDTW = RapidlibLinkerDLL.RunSeriesClassification(m_ModelAddress, dtwSerie);
                    // Make sure to destroy the serie because it is outside of the GC scope
                    RapidlibLinkerDLL.DestroyTrainingSet(dtwSerie);
                    break;
                case ModelType.None:
                    throw new Exception("You can't run an unitialised model!");
                default:
                    break;
            }

            // If we got any output, we mark the model as running
            if (outputDTW != -1)
                m_ModelStatus = IMLSpecifications.ModelStatus.Running;

            return outputDTW;
        }

        /// <summary>
        /// Stops the model in case it is running
        /// </summary>
        /// <returns></returns>
        public bool StopRunning()
        {
            bool isStop = false;
            if (m_ModelStatus == IMLSpecifications.ModelStatus.Running)
            {
                m_ModelStatus = IMLSpecifications.ModelStatus.Trained;
                isStop = true;
            }
            return isStop;
        }

        /* TRAINING LOGIC */

        /// <summary>
        /// Trains the model with a list of training examples (classification or regression)
        /// </summary>
        /// <param name="trainingExamples">True if training succeeded</param>
        public bool Train(List<RapidlibTrainingExample> trainingExamples)
        {
            bool isTrained = false;
            IntPtr trainingSetAddress = (IntPtr)0;
            // Only allow training of classification and regression (because of the format of the training data)
            switch (m_TypeOfModel)
            {
                case ModelType.kNN:
                    trainingSetAddress = TransformTrainingExamplesForRapidlib(trainingExamples);
                    isTrained = RapidlibLinkerDLL.TrainClassification(m_ModelAddress, trainingSetAddress);
                    break;
                case ModelType.NeuralNetwork:
                    trainingSetAddress = TransformTrainingExamplesForRapidlib(trainingExamples);
                    isTrained = RapidlibLinkerDLL.TrainRegression(m_ModelAddress, trainingSetAddress);
                    break;
                case ModelType.DTW:
                    throw new Exception("A list of training series is required to train a DTW model!");
                case ModelType.None:
                    throw new Exception("You can't train on an unitialised model!");
                default:
                    break;
            }

            if (isTrained)
                m_ModelStatus = IMLSpecifications.ModelStatus.Trained;

            // Once the training is done, we need to destroy the c++ training list outside of the GC scope
            RapidlibLinkerDLL.DestroyTrainingSet(trainingSetAddress);

            return isTrained;
        }

        /// <summary>
        /// Trains the model with a list of training series (DTW)
        /// </summary>
        /// <param name="trainingSeries"></param>
        /// <returns>True if training succeeded</returns>
        public bool Train(List<RapidlibTrainingSerie> trainingSeries)
        {
            bool isTrained = true;
            // Only allow training of DTW (because of the format of the training data)
            switch (m_TypeOfModel)
            {
                case ModelType.kNN:
                    isTrained = false;
                    throw new Exception("A list of training examples, not series is required to train a classification model!");
                case ModelType.NeuralNetwork:
                    isTrained = false;
                    throw new Exception("A list of training examples, not series is required to train a regression model!");
                case ModelType.DTW:
                    // Reset model in dll memory
                    RapidlibLinkerDLL.ResetSeriesClassification(m_ModelAddress);
                    // Create rapidlib training examples series in rapidlib dll memory
                    IntPtr[] dtwSeriesAdresses = TransformTrainingSeriesForRapidlib(this, trainingSeries);
                    // Loop trhough the serieses to add them and then destroy them (they are out of the GC scope)
                    for (int i = 0; i < dtwSeriesAdresses.Length; i++)
                    {
                        bool trainedOnSerie = RapidlibLinkerDLL.AddSeries(m_ModelAddress, dtwSeriesAdresses[i]);
                        RapidlibLinkerDLL.DestroyTrainingSet(dtwSeriesAdresses[i]);
                        // If one of them fails, we record the training as failed
                        if (!trainedOnSerie)
                            isTrained = false;
                    }
                    break;
                case ModelType.None:
                    isTrained = false;
                    throw new Exception("You can't train on an unitialised model!");
                default:
                    break;
            }

            if (isTrained)
                m_ModelStatus = IMLSpecifications.ModelStatus.Trained;

            return isTrained;
        }

        /* MODEL CONFIGURATION */

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
            // We set the training status to untrained
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;
        }

        /// <summary>
        /// Configures the model with data from the json string
        /// </summary>
        /// <param name="jsonstring"></param>
        public void ConfigureModelWithJson(string jsonstring)
        {
            if (!String.IsNullOrEmpty(jsonstring))
            {
                // Configure the model in memory
                RapidlibLinkerDLL.PutJSON(m_ModelAddress, jsonstring);
                // Set the status to trained (since we assume the model we loaded was trained)
                m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;
            }
        }

        /* I/O FOR MODEL DATA */

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
            // Since it is a new model, the status of the model is now untrained
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;
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
            // Since it is a new model, the status of the model is now untrained
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;
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
            // Since it is a new model, the status of the model is now untrained
            m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;
        }

        /// <summary>
        /// Transforms the provided C# training examples list into a format suitable for the Rapidlib dll
        /// </summary>
        /// <param name="trainingExamplesToTransform"></param>
        /// <returns>The memory address to the newly created and configured training set</returns>
        private IntPtr TransformTrainingExamplesForRapidlib(List<RapidlibTrainingExample> trainingExamplesToTransform)
        {
            // Get the memory address for an empty c++ training set
            IntPtr rapidlibTrainingSetAddress = RapidlibLinkerDLL.CreateTrainingSet();
            // Configure the new training set in memory with the C# list of examples
            foreach (RapidlibTrainingExample example in trainingExamplesToTransform)
            {
                RapidlibLinkerDLL.AddTrainingExample(rapidlibTrainingSetAddress,
                    example.Input, example.Input.Length,
                    example.Output, example.Output.Length);
            }
            // Return the address for the training set in memory
            return rapidlibTrainingSetAddress;
        }

        /// <summary>
        /// Transforms the provided C# training series list into a format suitable for the rapidlib dll 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="trainingSeriesToTransform"></param>
        /// <returns>The memory addresses to the newly created and configured training series sets</returns>
        private IntPtr[] TransformTrainingSeriesForRapidlib(RapidlibModel model, List<RapidlibTrainingSerie> trainingSeriesToTransform)
        {
            // We check that we are transforming for the right kind of model
            if (model.TypeOfModel != ModelType.DTW)
                throw new Exception("You can't create a rapidlib training series set in memory for the rapidlib dll with a non DTW model!");
            // We first check that we have series to input, or we will get a c++ memory exception
            if (trainingSeriesToTransform.Count == 0)
                throw new Exception("You can't train a DTW model without any serie.");



            // Define the array of addresses to return
            IntPtr[] trainingSeriesSetAddress = new IntPtr[trainingSeriesToTransform.Count];
            for (int i = 0; i < trainingSeriesSetAddress.Length; i++)
            {
                // Avoid memory exceptions making sure that the input examples serie is not null or empty
                if (trainingSeriesToTransform[i].ExampleSerie == null || trainingSeriesToTransform[i].ExampleSerie.Count == 0 )
                    throw new Exception("You can't train a DTW model on an empty serie!");

                // Get the memory address for an empty c++ training set
                trainingSeriesSetAddress[i] = RapidlibLinkerDLL.CreateTrainingSet();

            }

            // Configure the new training set in memory with the C# list of examples
            for (int i = 0; i < trainingSeriesToTransform.Count; i++)
            {
                // We have index i to select each training series address and add all examples per serie to it
                for (int j = 0; j < trainingSeriesToTransform[i].ExampleSerie.Count; j++)
                {
                    RapidlibLinkerDLL.AddTrainingExample(trainingSeriesSetAddress[i],
                        trainingSeriesToTransform[i].ExampleSerie[j], trainingSeriesToTransform[i].ExampleSerie.Count,
                        trainingSeriesToTransform[i].ExampleSerie[j], trainingSeriesToTransform[i].ExampleSerie.Count); // We provide the serie as an output because it is ignored when running it and we need something

                }
            }
            // Return the address for the training series set in memory
            return trainingSeriesSetAddress;
        }

        /// <summary>
        /// Creates a training serie in the rapidlib dll memory based on one in C# memory and returns its address
        /// </summary>
        /// <param name="model"></param>
        /// <param name="trainingSeriesToTransform"></param>
        /// <returns>The memory address to the newly created and configured training series set</returns>
        private IntPtr TransformTrainingSerieForRapidlib(RapidlibModel model, RapidlibTrainingSerie trainingSeriesToTransform)
        {
            if (model.TypeOfModel != ModelType.DTW)
            {
                throw new Exception("You can't create a rapidlib training series set in memory for the rapidlib dll with a non DTW model!");
            }
            // Get the memory address for an empty c++ training set
            IntPtr trainingSeriesSetAddress = RapidlibLinkerDLL.CreateTrainingSet();
            // Configure the new training set in memory with the C# list of examples (we only need the inputs)
            foreach (var serie in trainingSeriesToTransform.ExampleSerie)
            {
                RapidlibLinkerDLL.AddTrainingExample(trainingSeriesSetAddress,
                    serie, serie.Length,
                    serie, serie.Length); // We provide the serie as an output because it is ignored when running it and we need something
            }
            // Return the address for the training series set in memory
            return trainingSeriesSetAddress;
        }


        #endregion
    }
}
