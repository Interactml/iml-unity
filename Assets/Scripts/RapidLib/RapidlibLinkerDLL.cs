using System;
using System.Runtime.InteropServices;

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
        private static extern void runSeriesClassification(IntPtr model, IntPtr runningSeries, string outputString);
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
        public static void TrainSeriesClassification(IntPtr model, IntPtr trainingSeriesCollection)
        {
            trainSeriesClassification(model, trainingSeriesCollection);
        }

        /// <summary>
        /// Resets the rapidlib dtw model
        /// </summary>
        /// <param name="model"></param>
        /// <returns>True if succesful reset</returns>
        public static bool ResetSeriesClassification(IntPtr model)
        {
            return resetSeriesClassification(model);
        }

        /// <summary>
        /// Destroys the specified DTW model from memory
        /// </summary>
        /// <param name="model"></param>
        public static void DestroySeriesClassificationModel(IntPtr model)
        {
            destroySeriesClassificationModel(model);
        }
        
        /// <summary>
        /// [DEPRECATED] Adds a trainingSet as a new serie to the specified DTW model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="trainingSet"></param>
        /// <returns>True if succesfully added the serie</returns>
        public static bool AddSeries(IntPtr model, IntPtr trainingSet)
        {
            throw new NotImplementedException("Add series is no longer usable");
        }
        
        /// <summary>
        /// Runs the specified DTW model against the provided training examples serie
        /// </summary>
        /// <param name="model"></param>
        /// <param name="runningSeries"></param>
        /// <returns>The position of the closest known serie in the model </returns>
        public static void RunSeriesClassification(IntPtr model, IntPtr runningSeries, ref string outputString)
        {
            outputString = "";
            try
            {
                runSeriesClassification(model, runningSeries, outputString);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                throw new Exception("Error when running dtw model: " + e.Message);
            }            
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
            return process(model, input, numInputs, output, numOutputs);
        }
        
        /// <summary>
        /// Removes the specified model from memory
        /// </summary>
        /// <param name="model"></param>
        public static void DestroyModel(IntPtr model)
        {
            destroyModel(model);
        }

        /// <summary>
        /// Get a JSON representation of the model in the form of a styled string
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetJSON(IntPtr model)
        {
            return Marshal.PtrToStringAnsi(getJSON(model));
        }

        /// <summary>
        /// Configure empty model with string. See GetJSON()
        /// </summary>
        /// <param name="model"></param>
        /// <param name="jsonString"></param>
        public static void PutJSON(IntPtr model, string jsonString)
        {
            putJSON(model, jsonString);
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
            addTrainingExample(trainingSet, inputs, numInputs, outputs, numOutputs);
        }
        
        /// <summary>
        /// Returns the number of training examples in a specified rapidlib training set
        /// </summary>
        /// <param name="trainingSet"></param>
        /// <returns>Number of training examples</returns>
        public static int GetNumTrainingExamples(IntPtr trainingSet)
        {
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
            return createTrainingSeries();
        }

        /// <summary>
        /// Destroys a single training series instance in unmanaged memory
        /// </summary>
        /// <param name="seriesToDestroy"></param>
        public static void DestroyTrainingSeries(IntPtr seriesToDestroy)
        {
            destroyTrainingSeries(seriesToDestroy);
        }

        /// <summary>
        /// Adds a feature (or set of features) to a training series 
        /// </summary>
        /// <param name="series"></param>
        /// <param name="inputs"></param>
        /// <param name="numInputs"></param>
        public static void AddInputsToSeries(IntPtr series, double[] inputs, int numInputs)
        {
            addInputsToSeries(series, inputs, numInputs);
        }

        /// <summary>
        /// Adds label to a serie in unmanaged memory
        /// </summary>
        /// <param name="series"></param>
        /// <param name="label"></param>
        public static void AddLabelToSeries(IntPtr series, string label)
        {
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
            destroyTrainingSeriesCollection(seriesCollection);
        }

        /// <summary>
        /// Adds a series to a collection of series in unmanaged memory
        /// </summary>
        /// <param name="seriesCollection"></param>
        /// <param name="series"></param>
        public static void AddSeriesToSeriesCollection(IntPtr seriesCollection, IntPtr series)
        {
            addSeriesToSeriesCollection(seriesCollection, series);
        }


        #endregion
    }

}
