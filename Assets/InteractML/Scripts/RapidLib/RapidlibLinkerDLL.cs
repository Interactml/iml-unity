﻿using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace InteractML
{
    /// <summary>
    /// Links to the Rapidlib dll exposing its functionality to C#. Doesn't hold any data
    /// </summary>
    public class RapidlibLinkerDLL
    {
        #region DLL Calls
        //Lets make our calls from the Plugin

        // REGRESSION 
        [DllImport("RapidLibPlugin")]
        private static extern IntPtr createRegressionModel();
        [DllImport("RapidLibPlugin")]
        private static extern bool trainRegression(IntPtr model, IntPtr trainingSet);

        // CLASSIFICATION
        [DllImport("RapidLibPlugin")]
        private static extern IntPtr createClassificationModel();
        [DllImport("RapidLibPlugin")]
        private static extern bool trainClassification(IntPtr model, IntPtr trainingSet);

        // DTW
        [DllImport("RapidLibPlugin")]
        private static extern IntPtr createSeriesClassificationModel();
        [DllImport("RapidLibPlugin")]
        private static extern bool resetSeriesClassification(IntPtr model);
        [DllImport("RapidLibPlugin")]
        private static extern void destroySeriesClassificationModel(IntPtr model);
        [DllImport("RapidLibPlugin")]
        private static extern bool trainSeriesClassification(IntPtr model, IntPtr trainingSeriesCollection);
        [DllImport("RapidLibPlugin")]
        private static extern IntPtr runSeriesClassification(IntPtr model, IntPtr runningSeries);
        [DllImport("RapidLibPlugin")]
        private static extern int getSeriesClassificationCosts(IntPtr model, double[] output, int numOutputs);

        // GENERIC MODEL
        [DllImport("RapidLibPlugin")]
        private static extern int process(IntPtr model, double[] input, int numInputs, double[] output, int numOutputs);
        [DllImport("RapidLibPlugin")]
        private static extern void destroyModel(IntPtr model);
        [DllImport("RapidLibPlugin")]
        //[return: MarshalAs(UnmanagedType.LPStr)]
        private static extern IntPtr getJSON(IntPtr model);
        [DllImport("RapidLibPlugin")]
        private static extern void putJSON(IntPtr model, string jsonString);

        // TRAINING EXAMPLES
        [DllImport("RapidLibPlugin")]
        private static extern IntPtr createTrainingSet();
        [DllImport("RapidLibPlugin")]
        private static extern void destroyTrainingSet(IntPtr trainingSet);
        [DllImport("RapidLibPlugin")]
        private static extern void addTrainingExample(IntPtr trainingSet, double[] inputs, int numInputs, double[] outputs, int numOutputs);
        [DllImport("RapidLibPlugin")]
        private static extern int getNumTrainingExamples(IntPtr trainingSet);
        [DllImport("RapidLibPlugin")]
        private static extern double getInput(IntPtr trainingSet, int i, int j);
        [DllImport("RapidLibPlugin")]
        private static extern double getOutput(IntPtr trainingSet, int i, int j);

        // TRAINING SERIES
        [DllImport("RapidLibPlugin")]
        private static extern IntPtr createTrainingSeries();
        [DllImport("RapidLibPlugin")]
        private static extern void destroyTrainingSeries(IntPtr series);
        [DllImport("RapidLibPlugin")]
        private static extern void addInputsToSeries(IntPtr series, double[] inputs, int numInputs);
        [DllImport("RapidLibPlugin")]
        private static extern void addLabelToSeries(IntPtr series, string labelString);
        [DllImport("RapidLibPlugin")]
        private static extern IntPtr createTrainingSeriesCollection();
        [DllImport("RapidLibPlugin")]
        private static extern void destroyTrainingSeriesCollection(IntPtr seriesCollection);
        [DllImport("RapidLibPlugin")]
        private static extern void addSeriesToSeriesCollection(IntPtr seriesCollection, IntPtr series);


        #endregion

        #region Public Methods

        /* Exposing the rapidlib methods publicly so that other C# classes can use them */

        /* === REGRESSION === */
        /// <summary>
        /// Creates a rapidlib regression model (neural network)
        /// </summary>
        /// <returns>Pointer to model</returns>
        public static IntPtr CreateRegressionModel()
        {
            try
            {
                return createRegressionModel();
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                throw new Exception("Error when creating regression model: " + e.Message);
            }
        }
        
        /// <summary>
        /// Trains a rapidlib regression model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="trainingSet"></param>
        /// <returns>True if succesfully trained</returns>
        public static bool TrainRegression(IntPtr model, IntPtr trainingSet)
        {
            if (model == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting trainRegression.");

            return trainRegression(model, trainingSet);
        }

        /// <summary>
        /// Trains a rapidlib regression model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="trainingSet"></param>
        /// <returns>True if succesfully trained</returns>
        public static async Task<bool> TrainRegressionAsync(IntPtr model, IntPtr trainingSet)
        {
            if (model == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting trainRegression.");

            return trainRegression(model, trainingSet);
        }

        /* === CLASSIFICATION === */
        /// <summary>
        /// Creates a rapidlib classification model (kNN)
        /// </summary>
        /// <returns>Pointer to model</returns>
        public static IntPtr CreateClassificationModel()
        {
            try
            {
                return createClassificationModel();
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                throw new Exception("Error when creating classification model: " + e.Message);
            }
        }

        /// <summary>
        /// Trains a rapidlib classification model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="trainingSeriesCollection"></param>
        /// <returns>True if succesfully trained</returns>
        public static bool TrainClassification(IntPtr model, IntPtr trainingSeriesCollection)
        {
            if (model == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting trainClassification.");

            return trainClassification(model, trainingSeriesCollection);
        }

        /// <summary>
        /// Trains a rapidlib classification model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="trainingSeriesCollection"></param>
        /// <returns>True if succesfully trained</returns>
        public static async Task<bool> TrainClassificationAsync(IntPtr model, IntPtr trainingSeriesCollection)
        {
            if (model == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting trainClassification.");
            
            return trainClassification(model, trainingSeriesCollection);
        }

        /* === DTW === */
        /// <summary>
        /// Creates a rapidlib time series classification model (DTW)
        /// </summary>
        /// <returns>Pointer to model</returns>
        public static IntPtr CreateSeriesClassificationModel()
        {
            try
            {
                return createSeriesClassificationModel();
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                throw new Exception("Error when creating DTW model: " + e.Message);
            }
        }

        /// <summary>
        /// Trains a rapidlib trime series classification model (DTW) with a series collection in unmanaged memory
        /// </summary>
        /// <param name="model"></param>
        /// <param name="trainingSeriesCollection"></param>
        public static bool TrainSeriesClassification(IntPtr model, IntPtr trainingSeriesCollection)
        {
            if (model == IntPtr.Zero || trainingSeriesCollection == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting trainingSeriesClassification.");

            return trainSeriesClassification(model, trainingSeriesCollection);
        }

        /// <summary>
        /// Trains a rapidlib trime series classification model (DTW) with a series collection in unmanaged memory
        /// </summary>
        /// <param name="model"></param>
        /// <param name="trainingSeriesCollection"></param>
        public static async Task<bool> TrainSeriesClassificationAsync(IntPtr model, IntPtr trainingSeriesCollection)
        {
            if (model == IntPtr.Zero || trainingSeriesCollection == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting trainingSeriesClassification.");

            return trainSeriesClassification(model, trainingSeriesCollection);
        }

        /// <summary>
        /// Resets the rapidlib dtw model
        /// </summary>
        /// <param name="model"></param>
        /// <returns>True if succesful reset</returns>
        public static bool ResetSeriesClassification(IntPtr model)
        {
            if (model == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting resetSeriesClassification.");

            return resetSeriesClassification(model);
        }

        /// <summary>
        /// Destroys the specified DTW model from memory
        /// </summary>
        /// <param name="model"></param>
        public static void DestroySeriesClassificationModel(IntPtr model)
        {
            if (model == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting seriesClassificationModel destruction.");

            destroySeriesClassificationModel(model);
        }
        
        /// <summary>
        /// Runs the specified DTW model against the provided training examples serie
        /// </summary>
        /// <param name="model"></param>
        /// <param name="runningSeries"></param>
        /// <returns>The position of the closest known serie in the model </returns>
        public static string RunSeriesClassification(IntPtr model, IntPtr runningSeries)
        {
            if (model == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting runSeriesClassification.");

            string outputString = "";
            try
            {
                outputString = Marshal.PtrToStringAnsi(runSeriesClassification(model, runningSeries));
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                throw new Exception("Error when running dtw model: " + e.Message);
            }
            return outputString;
        }
        
        /// <summary>
        /// Returns the costs for each known output in the DTW model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="output"></param>
        /// <param name="numOutputs"></param>
        /// <returns></returns>
        public static int GetSeriesClassificationCosts(IntPtr model, double[] output, int numOutputs)
        {
            if (model == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting getSeriesClassification.");

            return getSeriesClassificationCosts(model, output, numOutputs);
        }

        /* === GENERIC MODEL === */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="input"></param>
        /// <param name="numInputs"></param>
        /// <param name="output"></param>
        /// <param name="numOutputs"></param>
        /// <returns></returns>
        public static int Process(IntPtr model, double[] input, int numInputs, ref double[] output, int numOutputs)
        {
            if (model == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting model process.");

            return process(model, input, numInputs, output, numOutputs);
        }

        /// <summary>
        /// Removes the specified model from memory
        /// </summary>
        /// <param name="model"></param>
        public static void DestroyModel(IntPtr model)
        {
            if (model == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting model destruction.");

            destroyModel(model);
        }

        /// <summary>
        /// Get a JSON representation of the model in the form of a styled string
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetJSON(IntPtr model)
        {
            if (model == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting GetJSON.");

            return Marshal.PtrToStringAnsi(getJSON(model));
        }

        /// <summary>
        /// Configure empty model with string. See GetJSON()
        /// </summary>
        /// <param name="model"></param>
        /// <param name="jsonString"></param>
        public static void PutJSON(IntPtr model, string jsonString)
        {
            if (jsonString.Contains("\"modelSet\" : null"))
                throw new Exception("We can't configure a null rapidlib model with a json config file!");
            else if (jsonString.Contains("\"modelType\" : \"Series Classification\""))
                throw new NotImplementedException("DTW model configuration from file not implemented yet, you will need to manually retrain it!");
            if (model == IntPtr.Zero)
                throw new Exception("The address to the model is zero, aborting configuration.");

            try
            {
                putJSON(model, jsonString);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                throw new Exception("Error when configuring model with json config file: " + e.Message);
            }
        }

        // TRAINING EXAMPLES
        /// <summary>
        /// Creates a training set in rapidlib and returns a pointer to it
        /// </summary>
        /// <returns></returns>
        public static IntPtr CreateTrainingSet()
        {
            return createTrainingSet();
        }
        
        /// <summary>
        /// Destroys a specified training set in rapidlib
        /// </summary>
        /// <param name="trainingSet"></param>
        public static void DestroyTrainingSet(IntPtr trainingSet)
        {
            if (trainingSet == IntPtr.Zero)
                throw new Exception("The address to the trainingSet is zero, aborting destruction.");

            destroyTrainingSet(trainingSet);
        }
        
        /// <summary>
        /// Adds a single training example into the specified trainingSet
        /// </summary>
        /// <param name="trainingSet"></param>
        /// <param name="inputs"></param>
        /// <param name="numInputs"></param>
        /// <param name="outputs"></param>
        /// <param name="numOutputs"></param>
        public static void AddTrainingExample(IntPtr trainingSet, double[] inputs, int numInputs, double[] outputs, int numOutputs)
        {
            if (trainingSet == IntPtr.Zero)
                throw new Exception("The address to the trainingSet is zero, aborting additionTrainingExample.");

            addTrainingExample(trainingSet, inputs, numInputs, outputs, numOutputs);
        }
        
        /// <summary>
        /// Returns the number of training examples in a specified rapidlib training set
        /// </summary>
        /// <param name="trainingSet"></param>
        /// <returns>Number of training examples</returns>
        public static int GetNumTrainingExamples(IntPtr trainingSet)
        {
            if (trainingSet == IntPtr.Zero)
                throw new Exception("The address to the trainingSet is zero, aborting getNumTrainingExamples.");

            return getNumTrainingExamples(trainingSet);
        }

        /// <summary>
        /// Gets the input from a specific training example in a training set
        /// </summary>
        /// <param name="trainingSet"></param>
        /// <param name="trainingExamplePos">Position in the training set list</param>
        /// <param name="inputPos">Position in the input list</param>
        /// <returns>Value of the input</returns>
        public static double GetInput(IntPtr trainingSet, int trainingExamplePos, int inputPos)
        {
            if (trainingSet == IntPtr.Zero)
                throw new Exception("The address to the trainingSet is zero, aborting getInput.");

            return getInput(trainingSet, trainingExamplePos, inputPos);
        }

        /// <summary>
        /// Gets the output from a specific training example in a training set
        /// </summary>
        /// <param name="trainingSet"></param>
        /// <param name="trainingExamplePos">Position in the training set list</param>
        /// <param name="outputPos">Position in the output list</param>
        /// <returns>Value of the output</returns>
        public static double GetOutput(IntPtr trainingSet, int trainingExamplePos, int outputPos)
        {
            if (trainingSet == IntPtr.Zero)
                throw new Exception("The address to the trainingSet is zero, aborting getOutput.");

            return getOutput(trainingSet, trainingExamplePos, outputPos);
        }

        /* ==== TRAINING SERIES ==== */

        // TRAINING SERIES

        /// <summary>
        /// Returns a single training series instance in unmanaged memory
        /// </summary>
        /// <returns></returns>
        public static IntPtr CreateTrainingSeries()
        {
            try
            {
                return createTrainingSeries();
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                throw new Exception("Error when creating unmanaged training series: " + e.Message);
            }

        }

        /// <summary>
        /// Destroys a single training series instance in unmanaged memory
        /// </summary>
        /// <param name="seriesToDestroy"></param>
        public static void DestroyTrainingSeries(IntPtr seriesToDestroy)
        {
            if (seriesToDestroy == IntPtr.Zero)
                throw new Exception("The address to the seriesToDestroy is zero, aborting destruction.");

            destroyTrainingSeries(seriesToDestroy);
        }

        /// <summary>
        /// Adds a single feature to a training series in unmanaged memory
        /// </summary>
        /// <param name="series"></param>
        /// <param name="inputs"></param>
        /// <param name="numInputs"></param>
        public static void AddInputsToSeries(IntPtr series, double[] inputs, int numInputs)
        {
            if (series == IntPtr.Zero)
                throw new Exception("The address to the series is zero, aborting addInputsToSeries.");

            addInputsToSeries(series, inputs, numInputs);
        }

        /// <summary>
        /// Adds label to a serie in unmanaged memory
        /// </summary>
        /// <param name="series"></param>
        /// <param name="label"></param>
        public static void AddLabelToSeries(IntPtr series, string label)
        {
            if (series == IntPtr.Zero)
                throw new Exception("The address to the series is zero, aborting addLabelToSeries.");

            addLabelToSeries(series, label);
        }

        /// <summary>
        /// Creates a collection of training series in unmanaged memory (useful when training dtw models)
        /// </summary>
        /// <returns>Pointer to series collection in unmanaged memory</returns>
        public static IntPtr CreateTrainingSeriesCollection()
        {
            return createTrainingSeriesCollection();
        }

        /// <summary>
        /// Destroys collection of training series in unmanaged memory
        /// </summary>
        /// <param name="seriesCollection"></param>
        public static void DestroyTrainingSeriesCollection(IntPtr seriesCollection)
        {
            if (seriesCollection == IntPtr.Zero)
                throw new Exception("The address to the seriesCollection is zero, aborting destroyTrainingSeriesCollection.");

            try
            {
                destroyTrainingSeriesCollection(seriesCollection);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                throw new Exception("Error when destroying unmanaged training series collection: " + e.Message);
            }
        }

        /// <summary>
        /// Adds a series to a collection of series in unmanaged memory
        /// </summary>
        /// <param name="seriesCollection"></param>
        /// <param name="series"></param>
        public static void AddSeriesToSeriesCollection(IntPtr seriesCollection, IntPtr series)
        {
            if (seriesCollection == IntPtr.Zero || series == IntPtr.Zero)
                throw new Exception("The address to the seriesCollection is zero, aborting addSeriesToSeriesCollection.");

            addSeriesToSeriesCollection(seriesCollection, series);
        }


        #endregion
    }

}
