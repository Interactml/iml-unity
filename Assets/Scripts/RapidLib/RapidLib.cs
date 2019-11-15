using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using InteractML;


/// <summary>
/// [DEPRECATED] Use RapidlibTrainingExample instead
/// </summary>
[Serializable]
public class TrainingExample
{
    public double[] Input;
    public double[] Output;

}

/// <summary>
/// [DEPRECATED] Use RapidlibTrainingSerie instead
/// </summary>
[Serializable]
public class TrainingSeries
{
    public List<TrainingExample> examples;
}

[ExecuteInEditMode]
public class RapidLib : MonoBehaviour {

    #region Variables

    IntPtr model = (IntPtr)0;

    /// <summary>
    /// Flag to allow rapidlib to work with training data inputted from outside the class (required for InteractML)
    /// </summary>
    //[HideInInspector]
    public bool AllowExternalData;

    [SerializeField]
    private double[] m_ExternalInputFeatures;
    [SerializeField]
    private double[] m_ExternalOutputFeatures;

    /// <summary>
    /// The fileName used when saving/loading training examples from disk 
    /// </summary>
    private string m_ExternalFileNameForIO;

    public bool AllowKeyboardShortcuts;

    public enum LearningType { Classification, Regression, DTW };

    //This is what you need to show in the inspector.
    public LearningType learningType;

    //public bool classification = false;
    [HideInInspector]
    public float startDelay = 0.0f;
    public float captureRate = 10.0f;
    [HideInInspector]
    public float recordTime = -1.0f;
    float timeToNextCapture = 0.0f;
    float timeToStopCapture = 0.0f;

    public Transform[] inputs;

    [HideInInspector]
    public double[] outputs;

    //public double[] TrainingOutputs;

    [SerializeField]
    public List<TrainingExample> trainingExamples;

    [HideInInspector]
    public List<TrainingSeries> trainingSerieses;

    public bool run = false;
    public bool Running { get { return run; } }

    public bool collectData = false;
    public bool CollectingData { get { return collectData; } }

    private bool train = false;
    public bool Training { get { return train; } }
    public bool Trained { get; private set; }

    [HideInInspector]
    public string jsonString = "";

    // File saving
    string m_DataPathModel;
    string m_DataPathTrainingSet;
    string m_FileModelName;
    string m_FileTrainingSetName;
    string m_FolderDataPathName;
    string m_FileExtension;

    // Feature extraction
    public enum FeaturesEnum { Position, Rotation, Velocity, Acceleration, DistanceToFirstInput, Scale }
    [SerializeField] 
    private List<FeaturesEnum> m_Features;
    private int[] m_LengthsFeatureVector;

    private delegate void FeaturesDelegate(ref double[] inputForModel, Transform[] rawInputData);
    private FeaturesDelegate m_ExtractFeatures;
    private int m_PointerFeatureVector;

    // Used to calculate velocity and acceleration
    private Vector3 m_Velocity;
    private Vector3 m_Acceleration;
    private Vector3 m_LastFramePosition;
    private Vector3 m_LastFrameVelocity;

    // Used to calculate distance from all inputs to the first one (i.e. fingers to the hand palm)
    [SerializeField]
    private float[] m_DistancesToFirstInput;

    // Option to serialise with JSON.Net instead of Unity's built in one
    private bool SerializeWithJSONDotNet;
    #endregion

    #region DLL Calls

    //Lets make our calls from the Plugin

    [DllImport("RapidLibPlugin")]
    private static extern IntPtr createRegressionModel();

    [DllImport("RapidLibPlugin")]
    private static extern IntPtr createClassificationModel();

    [DllImport("RapidLibPlugin")]
    private static extern IntPtr createSeriesClassificationModel();

    [DllImport("RapidLibPlugin")]
    private static extern void destroyModel(IntPtr model);

    [DllImport("RapidLibPlugin")]
    private static extern void destroySeriesClassificationModel(IntPtr model);

    [DllImport("RapidLibPlugin")]
    //[return: MarshalAs(UnmanagedType.LPStr)]
    private static extern IntPtr getJSON(IntPtr model);

    [DllImport("RapidLibPlugin")]
    private static extern void putJSON(IntPtr model, string jsonString);

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

    [DllImport("RapidLibPlugin")]
    private static extern bool trainRegression(IntPtr model, IntPtr trainingSet);

    [DllImport("RapidLibPlugin")]
    private static extern bool trainClassification(IntPtr model, IntPtr trainingSet);

    [DllImport("RapidLibPlugin")]
    private static extern int process(IntPtr model, double [] input, int numInputs, double [] output, int numOutputs);

    [DllImport("RapidLibPlugin")]
    private static extern bool resetSeriesClassification(IntPtr model);

    [DllImport("RapidLibPlugin")]
    private static extern bool addSeries(IntPtr model, IntPtr trainingSet);

    [DllImport("RapidLibPlugin")]
    private static extern int runSeriesClassification(IntPtr model, IntPtr trainingSet);

    [DllImport("RapidLibPlugin")]
    private static extern int getSeriesClassificationCosts(IntPtr model, double[] output, int numOutputs);

    #endregion

    #region Unity Messages

    void Start () {

        Initialize();

    }

    void OnDestroy()
    {
        if ((int)model != 0)
        {
            if (learningType == LearningType.DTW)
            {
                destroySeriesClassificationModel(model);
            } else
            {
                destroyModel(model);
            }
                
        }
        model = (IntPtr)0;
    }

    void Update()
    {
        // If rapidlib doesn't rely on any external data...
        if (!AllowExternalData)
        {
            // Calculate internal input features (user can use them to train a model)
            if (inputs != null && inputs.Length > 0)
            {
                // Calculate velocity and acceleration      
                m_Velocity = (inputs[0].position - m_LastFramePosition) / Time.deltaTime;
                m_Acceleration = (m_Velocity - m_LastFrameVelocity) / Time.deltaTime;

                // Calculate distances to first input if it is selected as a feature to extract
                if (m_Features.Contains(FeaturesEnum.DistanceToFirstInput) && inputs.Length > 1)
                {
                    // Fill the distances to first input array with the distances
                    for (int i = 0; i < m_DistancesToFirstInput.Length; i++)
                    {
                        m_DistancesToFirstInput[i] = Vector3.Distance(inputs[i + 1].position, inputs[0].position);
                    }
                }

            }
            else
            {
                Debug.LogError("Inputs in rapidlib component are null or empty!");
            }
        }

        ////Debug.Log(model);
        // Running Logic
        RunModelLogic();

        // Collect Data Logic
        CollectDataLogic();

#if UNITY_EDITOR
        if (AllowKeyboardShortcuts)
        {
            //// Space collects data
            //if (Input.GetKeyDown("space"))
            //{
            //    ToggleCollectingData();
            //}
            //// T key trains the model
            //if (Input.GetKeyDown(KeyCode.T))
            //{
            //    Train();
            //}
            // R key starts running the model
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    ToggleRunning();
            //}
        }

#endif

        // Storing info for feature extraction at the end of a frame
        if (!AllowExternalData && inputs != null && inputs.Length > 0)
        {
            // Storing this frame as the last and this velocity as the last to properly calculate velocity and accel every frame      
            m_LastFramePosition = inputs[0].position;
            m_LastFrameVelocity = m_Velocity;
        }

    }

    //void OnGUI()
    //{
    //    if (collectData)
    //    {
    //        GUI.Label(new Rect(20, 20, 100, 100), "time to capture " + (timeToNextCapture - Time.time));
    //    }
    //}

    // Called when the script is loaded or a value is changed in the inspector (Called in the editor only).
    private void OnValidate()
    {
        if (!AllowExternalData)
        {
            // Make sure that any of the lists to use are null
            if (m_LengthsFeatureVector == null)
                m_LengthsFeatureVector = new int[0];
            if (m_Features == null)
                m_Features = new List<FeaturesEnum>();

            // Empty feature extractor delegate and populate with corresponding method
            m_ExtractFeatures = null;
            m_LengthsFeatureVector = new int[m_Features.Count];
            for (int i = 0; i < m_Features.Count; i++)
            {
                switch (m_Features[i])
                {
                    case FeaturesEnum.Position:
                        m_ExtractFeatures += ExtractPosition;
                        m_LengthsFeatureVector[i] = 3;
                        break;
                    case FeaturesEnum.Rotation:
                        m_ExtractFeatures += ExtractRotation;
                        m_LengthsFeatureVector[i] = 4;
                        break;
                    case FeaturesEnum.Velocity:
                        m_ExtractFeatures += ExtractVelocity;
                        m_LengthsFeatureVector[i] = 3;
                        break;
                    case FeaturesEnum.Acceleration:
                        m_ExtractFeatures += ExtractAcceleration;
                        m_LengthsFeatureVector[i] = 3;
                        break;
                    case FeaturesEnum.DistanceToFirstInput:
                        m_ExtractFeatures += ExtractDistancesToFirstInput;
                        m_LengthsFeatureVector[i] = inputs.Length - 1;
                        break;
                    case FeaturesEnum.Scale:
                        m_ExtractFeatures += ExtractScale;
                        m_LengthsFeatureVector[i] = 3;
                        break;
                    default:
                        break;
                }
            }
            // The last method in the delegate needs to be to reset the pointer for next iteration
            m_ExtractFeatures += ResetPointersFeatureVector;

        }
    }

    private void OnApplicationQuit()
    {
        // Save model in the json
        if (learningType == LearningType.Regression || learningType == LearningType.Classification)
        {
            // Save JSON model and training to disk
            SaveDataToDisk();
        }
    }

    #endregion

    #region Public Methods

    public void Initialize()
    {
        // Make sure inputs array is not null (to avoid null references when opening for the first time the project)
        if (inputs == null)
            inputs = new Transform[0];

        // Set size of outputs array to 1
        outputs = new double[1];
        
        // Storing the this frame as the last frame to properly calculate velocity every frame      
        m_LastFramePosition = this.transform.position;

        // Get data path
        SetUpFileNamesAndPaths();

        SerializeWithJSONDotNet = true;

        // Initialize array of distance of all inputs to first one based on number of inputs
        if (inputs.Length > 0)
        {
            // We will need to calculate the distance from everything to the first one
            m_DistancesToFirstInput = new float[inputs.Length - 1];
        }

        // Set the trained flag to false
        Trained = false;


        // Code to initialize model (written by Marco)

        //model = (IntPtr)0;
        //Train();
        //jsonString = "";
        if ((int)model != 0)
        {
            destroyModel(model);
        }
        model = (IntPtr)0;

        if (learningType == LearningType.Regression)
        {
            model = createRegressionModel();

            // We load the model and the training set from disk
            jsonString = LoadDataFromDiskAndReturnModel();

            //Debug.Log("Training Examples list Count: " + trainingExamples.Count);

            putJSON(model, jsonString);
            //Debug.Log("PutJSON called");
        }
        else if (learningType == LearningType.Classification)
        {
            model = createClassificationModel();

            // We load the model and the training set from disk
            jsonString = LoadDataFromDiskAndReturnModel();

            //Debug.Log("Training Examples list Count: " + trainingExamples.Count);

            putJSON(model, jsonString);
            //Debug.Log("PutJSON called");

        }
        else
        {
            Train();
        }

    }

    public void AddTrainingExample()
    {
        TrainingExample newExample = new TrainingExample();

        // Calculate size of the new example input vector [I CAN MOVE THIS TO THE START OF THE PROGRAM]
        int sizeNewExampleInput = 0;
        for (int i = 0; i < m_Features.Count; i++)
        {
            sizeNewExampleInput += (m_LengthsFeatureVector[i] * inputs.Length);
        }

        newExample.Input = new double[sizeNewExampleInput];

        m_ExtractFeatures(ref newExample.Input, inputs);

        newExample.Output = new double[outputs.Length];
        for (int i = 0; i < outputs.Length; i++)
        {
            newExample.Output[i] = outputs[i];
        }

        //Array.Resize<TrainingExample>(ref trainingExamples, trainingExamples.Length + 1);
        //trainingExamples[trainingExamples.Length - 1] = newExample;
        trainingExamples.Add(newExample);
        
    }

    /// <summary>
    /// Adds an already created training example to rapidlib
    /// </summary>
    /// <param name="newExample"></param>
    public void AddTrainingExample(TrainingExample newExample)
    {
        // If the new examples is not null...
        if (newExample != null)
        {
            // We make sure that the inputs and outputs are not null
            if (newExample.Input == null || newExample.Output == null)
            {
                return;
            }
            // We make sure that the list is not null
            if (trainingExamples == null)
            {
                trainingExamples = new List<TrainingExample>();
            }
            // If everything is good, we add the example to the list
            trainingExamples.Add(newExample);
        }
    }

    /// <summary>
    /// Injects external data to run the model (required to work with InteractML)
    /// </summary>
    /// <param name="externalInputs"></param>
    /// <param name="externalOutputs"></param>
    public void InjectExternalData(double[] externalInputs, double[] externalOutputs, string externalFileName)
    {
        // Get reference to external inputs
        m_ExternalInputFeatures = externalInputs;
        // But make a copy of the external outputs into rapidlib
        m_ExternalOutputFeatures = new double[externalOutputs.Length];
        externalOutputs.CopyTo(m_ExternalOutputFeatures, 0);
        // Get data from the external file name
        m_ExternalFileNameForIO = externalFileName;
        // Make sure it is not empty
        if (m_ExternalFileNameForIO == "" || m_ExternalFileNameForIO == null)
        {
            m_ExternalFileNameForIO = "Rapidlib_Component";
        }
        // Add the bit to identify the file as rapidlib data
        m_ExternalFileNameForIO += "_RapidlibData";

        // Make sure that we warn the user if the component is not prepared to be injected data
        if (!AllowExternalData)
        {
            Debug.LogError("Injecting External Data into Rapidlib but AllowExternalData flag set to false! " + this.gameObject.name);            
        }
    }

    /// <summary>
    /// Returns a reference to the model output after processing, accounting whether rapidlib uses internal or external data
    /// </summary>
    /// <returns></returns>
    public double[] GetOutputs()
    {
        if (AllowExternalData)
        {
            if (m_ExternalOutputFeatures == null)
            {
                throw new UnityException("Trying to get model outputs from Rapidlib with external data, but external data is null!");
            }
            return m_ExternalOutputFeatures;
        }
        else
        {
            if (outputs == null)
            {
                throw new UnityException("Trying to get model outputs from Rapidlib with internal data, but internal data is null!");
            }
            return outputs;

        }
    }

    public void Train()
    {
        //Debug.Log("training");
        //Debug.Log("Model before training is: " + model);

        // Update training flag
        train = true;

        if (learningType == LearningType.DTW) {
            if (trainingSerieses.Count <= 0) { train = false;  return; }
        } else {
            if(trainingExamples.Count <= 0) { train = false; return; }
        }

        if ((int)model != 0)
        {
            destroyModel(model);
        }
        model = (IntPtr)0;

        if (learningType == LearningType.Classification)
        {
            model = createClassificationModel();
        } else if (learningType == LearningType.Regression)
        {
            model = createRegressionModel();
        } else if (learningType == LearningType.DTW)
        {
            model = createSeriesClassificationModel();
        } else
        {
            //Debug.Log("Error: unknown learning type");
        }

        //Debug.Log("created model");
        //Debug.Log("Model after training is: " + model);

        IntPtr trainingSet = createTrainingSet();
        //for(int i = 0; i < trainingExamples.Length; i++)
        if (learningType != LearningType.DTW)
        {
            foreach (TrainingExample example in trainingExamples)
            {
                addTrainingExample(trainingSet, example.Input, example.Input.Length, example.Output, example.Output.Length);
            }
        }

        //Debug.Log("created training set");

        if (learningType == LearningType.Classification)
        {
            if (!trainClassification(model, trainingSet))
            {
                //Debug.Log("training failed");
            }
        } else if (learningType == LearningType.Regression)
        {
            if (!trainRegression(model, trainingSet))
            {
                Debug.Log("training failed");
            }
        } else if (learningType == LearningType.DTW)
        {
            if(trainingSerieses.Count == 0)
            {
                destroyModel(model);
                model = (IntPtr)0;
                //Debug.Log("no training series, aborting learning and destroying model");
            } else {
                //Debug.Log(model);
                resetSeriesClassification(model);
                foreach (TrainingSeries series in trainingSerieses)
                {
                    trainingSet = createTrainingSet();
                    //for(int i = 0; i < trainingExamples.Length; i++)
                    foreach (TrainingExample example in series.examples)
                    {
                        //Debug.Log(example);
                        addTrainingExample(trainingSet, example.Input, example.Input.Length, example.Output, example.Output.Length);
                    }
                    //Debug.Log(model);
                    //Debug.Log(trainingSet);
                    if (!addSeries(model, trainingSet))
                    {
                        //Debug.Log("training failed");
                    }
                }
            }
            
                
        } else
        {
            //Debug.Log("Error: unknown learning type");
        }

        //Debug.Log("finished training");

        destroyTrainingSet(trainingSet);
        
        //Debug.Log("about to save");

        //jsonString = getJSON(model);
        if (learningType == LearningType.Regression)
        {
            // Needed to call GetJSON inside SaveDataToDisk
            SaveDataToDisk();
        }

        //Debug.Log("saved");

        //Debug.Log(jsonString);

        // Updating training flag
        train = false;
        Trained = true;
    }

    public void StartCollectingData()
    {
        
        collectData = true;
        if (!run)
        {
            //Debug.Log("starting recording in " + startDelay + " seconds");
            timeToNextCapture = Time.time + startDelay;
            if (recordTime > 0)
            {
                timeToStopCapture = Time.time + startDelay + recordTime;
            }
            else
            {
                timeToStopCapture = -1;
            }
        }

    }

    public void StopCollectingData()
    {
        if (learningType == LearningType.DTW)
        {
            if (!run)
            {
                trainingSerieses.Add(new TrainingSeries());
                trainingSerieses.Last().examples = new List<TrainingExample>(trainingExamples);
            }
            trainingExamples.Clear();
        }
        collectData = false;
    }

    public void ToggleCollectingData()
    {
        if (collectData)
        {
            StopCollectingData();
        } else
        {
            StartCollectingData();
        }
    }

    public void StartRunning()
    {
        run = true;
        if (learningType == LearningType.DTW)
        {
            StartCollectingData();
        } else
        {
            StopCollectingData();
        }
    }

    public void StopRunning()
    {
        if (learningType == LearningType.DTW)
        {
            StopCollectingData();
        } 
        run = false;
    }

    public void ToggleRunning()
    {
        if (run)
        {
            StopRunning();
        }
        else
        {
            StartRunning();
        }
    }

    [ContextMenu("Clear Training Examples")]
    public void ClearTrainingExamples()
    {
        trainingExamples.Clear();
    }

    public void SaveDataToDisk()
    {
        jsonString = Marshal.PtrToStringAnsi(getJSON(model));
        //Debug.Log("getJSON called");

        // Save JSON model and training to disk
        SaveModelToDisk(jsonString, m_DataPathModel);
        SaveTrainingSetToDisk(trainingExamples, m_DataPathTrainingSet);

    }

    public string LoadDataFromDiskAndReturnModel()
    {
        // We load the model and the training set from disk
        jsonString = LoadModelFromDisk(m_DataPathModel);
        trainingExamples = LoadTrainingSetFromDisk(m_DataPathTrainingSet);

        return jsonString;
    }

    #endregion

    #region Private Methods

    private void RunModelLogic()
    {
        if (run && (int)model != 0)
        {
            // DTW
            if (learningType == LearningType.DTW)
            {
                //Debug.Log("running");
                if (trainingExamples.Count > 0)
                {
                    IntPtr trainingSet = createTrainingSet();

                    foreach (TrainingExample example in trainingExamples)
                    {
                        addTrainingExample(trainingSet, example.Input, example.Input.Length, example.Output, example.Output.Length);
                    }
                    if (outputs.Length < 1)
                    {
                        outputs = new double[1];
                    }
                    ////Debug.Log("Model: " + model);
                    ////Debug.Log("Training set: " + trainingSet);
                    outputs[0] = runSeriesClassification(model, trainingSet);
                    //Debug.Log("Output:" + outputs[0]);
                }
            }
            // Classification and Regression
            else
            {
                // If rapidlib is using external data to run the model...
                if (AllowExternalData)
                {
                    // Check that expected I/O matches what we got
                    
                    // Make sure we have the external data to avoid errors
                    if ((m_ExternalInputFeatures == null || m_ExternalInputFeatures.Length == 0) || (m_ExternalOutputFeatures == null || m_ExternalOutputFeatures.Length == 0))
                    {
                        Debug.LogError("External data is null or empty when trying to run rapidlib model with external data! Stop running!");
                        return;
                    }

                    // Process inputs & outputs in rapidlib
                    process(model, m_ExternalInputFeatures, m_ExternalInputFeatures.Length, m_ExternalOutputFeatures, m_ExternalOutputFeatures.Length);
                }
                // If rapidlib is using internal data to run the model...
                else
                {
                    // Extract all the features from the input object
                    double[] modelInputs = null;
                    int sizeModelInput = 0;
                    // Calculate size of the model input vector [I CAN MOVE THIS TO THE START OF THE PROGRAM]
                    for (int i = 0; i < m_Features.Count; i++)
                    {
                        sizeModelInput += (m_LengthsFeatureVector[i] * inputs.Length);
                    }
                    modelInputs = new double[sizeModelInput];

                    if (modelInputs != null)
                        m_ExtractFeatures(ref modelInputs, inputs);

                    ////Debug.Log(input);
                    ////Debug.Log(input.Length);
                    //for (int i = 0; i < input.Length; i++)
                    //{
                    //    //Debug.Log(input[i]);
                    //}

                    ////Debug.Log(outputs);
                    ////Debug.Log(outputs.Length);
                    for (int i = 0; i < outputs.Length; i++)
                    {
                        //Debug.Log(outputs[i]);
                    }
                    process(model, modelInputs, modelInputs.Length, outputs, outputs.Length);

                }
            }
        }

    }

    private void CollectDataLogic()
    {
        if (collectData)
        {
            if (Application.isPlaying && timeToStopCapture > 0 && Time.time >= timeToStopCapture)
            {
                collectData = false;
                //Debug.Log("end recording");
            }
            else if (!Application.isPlaying || Time.time >= timeToNextCapture)
            {
                ////Debug.Log ("recording");
                AddTrainingExample();
                timeToNextCapture = Time.time + 1.0f / captureRate;
            }

        }

    }

    private void ResetPointersFeatureVector(ref double[] inputForModel, Transform[] rawInputData)
    {
        m_PointerFeatureVector = 0;
    }

    private void ExtractPosition(ref double[] inputForModel , Transform[] rawInputData)
    {
        // Don't run if we are outside vector boundaries
        if (m_PointerFeatureVector > inputForModel.Length)
            return;

        //Debug.Log("Extracting position...");

        /** Instead of length of rawInput it should be the feature length! But 
         * how to identify which feature length should be? I propose 
         * to create a class for each kind of feature, with info about it */
        for (int i = 0; i < rawInputData.Length; i++) 
        {
            inputForModel[m_PointerFeatureVector] = rawInputData[i].position.x;
            inputForModel[m_PointerFeatureVector + 1] = rawInputData[i].position.y;
            inputForModel[m_PointerFeatureVector + 2] = rawInputData[i].position.z;
        }

        // Move the pointer forward in the vector
        m_PointerFeatureVector += 3;

    }

    private void ExtractRotation(ref double[] inputForModel, Transform[] rawInputData)
    {
        // Don't run if we are outside vector boundaries
        if (m_PointerFeatureVector > inputForModel.Length)
            return;

        //Debug.Log("Extracting rotation...");

        for (int i = 0; i < rawInputData.Length; i++)
        {
            inputForModel[m_PointerFeatureVector] = rawInputData[i].rotation.x;
            inputForModel[m_PointerFeatureVector + 1] = rawInputData[i].rotation.y;
            inputForModel[m_PointerFeatureVector + 2] = rawInputData[i].rotation.z;
            inputForModel[m_PointerFeatureVector + 3] = rawInputData[i].rotation.w;
        }

        // Move the pointer forward in the vector
        m_PointerFeatureVector += 4;

    }

    private void ExtractVelocity(ref double[] inputForModel, Transform[] rawInputData)
    {
        // Don't run if we are outside vector boundaries
        if (m_PointerFeatureVector > inputForModel.Length)
            return;

        for (int i = 0; i < rawInputData.Length; i++)
        {
            inputForModel[m_PointerFeatureVector] = m_Velocity.x;
            inputForModel[m_PointerFeatureVector + 1] = m_Velocity.y;
            inputForModel[m_PointerFeatureVector + 2] = m_Velocity.z;
        }

        // Move the pointer forward in the vector
        m_PointerFeatureVector += 3;

    }

    private void ExtractAcceleration(ref double[] inputForModel, Transform[] rawInputData)
    {
        // Don't run if we are outside vector boundaries
        if (m_PointerFeatureVector > inputForModel.Length)
            return;

        for (int i = 0; i < rawInputData.Length; i++)
        {
            
            inputForModel[m_PointerFeatureVector] = m_Acceleration.x;
            inputForModel[m_PointerFeatureVector + 1] = m_Acceleration.y;
            inputForModel[m_PointerFeatureVector + 2] = m_Acceleration.z;

        }

        // Move the pointer forward in the vector
        m_PointerFeatureVector += 3;

    }

    private void ExtractDistancesToFirstInput(ref double[] inputForModel, Transform[] rawInputData)
    {
        // Don't run if we are outside vector boundaries
        if (m_PointerFeatureVector > inputForModel.Length)
            return;

        // We go through all inputs - 1 which will be the length of m_distancesToFirstInput
        for (int i = 0; i < m_DistancesToFirstInput.Length; i++)
        {
            inputForModel[m_PointerFeatureVector + i] = m_DistancesToFirstInput[i];
        }

        // Move the pointer forward in the vector
        m_PointerFeatureVector += m_DistancesToFirstInput.Length;
    }

    private void ExtractScale(ref double[] inputForModel, Transform[] rawInputData)
    {
        // Don't run if we are outside vector boundaries
        if (m_PointerFeatureVector > inputForModel.Length)
            return;

        for (int i = 0; i < rawInputData.Length; i++)
        {
            inputForModel[m_PointerFeatureVector] = rawInputData[i].localScale.x;
            inputForModel[m_PointerFeatureVector + 1] = rawInputData[i].localScale.y;
            inputForModel[m_PointerFeatureVector + 2] = rawInputData[i].localScale.z;
        }

        // Move the pointer forward in the vector
        m_PointerFeatureVector += 3;

    }

    /// <summary>
    /// Saves the model to disk
    /// </summary>
    /// <param name="modelJSON">The model in JSON format</param>
    /// <param name="filePath">The full file path (including the filename itself)</param>
    private void SaveModelToDisk (string modelJSON, string filePath)
    {

        if (AllowExternalData)
        {
            IMLDataSerialization.SaveRapidlibModelToDisk(modelJSON, m_ExternalFileNameForIO);
        }
        else
        {
            //Debug.Log("Save model disk called! " + filePath);

            // Check if there is NOT a folder with the folder name
            if (!Directory.Exists(Path.Combine(Application.dataPath, m_FolderDataPathName)))
            {
                // If there is not, we create it
                Directory.CreateDirectory(Path.Combine(Application.dataPath, m_FolderDataPathName));
            }

            // Check if there is NOT already a JSON file created
            if (File.Exists(filePath))
            {
                File.Delete(filePath);

            }

            // We create a text file
            //File.CreateText(filePath);

            // Write on the path
            File.WriteAllText(filePath, modelJSON);

        }

    }

    /// <summary>
    /// Loads a model from disk at the specified path
    /// </summary>
    /// <param name="filePath">The complete file path, including the name of the file</param>
    /// <returns>Returns a string JSON containing the model </returns>
    private string LoadModelFromDisk (string filePath)
    {
        if (AllowExternalData)
        {
            return IMLDataSerialization.LoadRapidlibModelFromDisk(m_ExternalFileNameForIO);
        }
        else
        {
            //Debug.Log("Load model disk called! " + filePath);

            // Check if there is NOT a folder with the folder name
            if (!Directory.Exists(Path.Combine(Application.dataPath, m_FolderDataPathName)))
            {
                // If there is not, we create it
                Directory.CreateDirectory(Path.Combine(Application.dataPath, m_FolderDataPathName));
            }


            // Check if there is NOT already a JSON file created
            if (File.Exists(filePath))
            {
                var file = File.ReadAllText(filePath);
                if (file != null && file != "")
                {
                    // We read from the file
                    return file;
                }
            }

            return "";

        }

    }

    /// <summary>
    /// Saves Training Data Set to disk
    /// </summary>
    /// <param name="listToSave">The list of training examples</param>
    /// <param name="filePath">File path without file extension</param>
    private void SaveTrainingSetToDisk (List<TrainingExample> listToSave, string filePath)
    {
        // If external data is being injected, we use the IMLDataSerialization tool (to avoid inconsistencies in files and folders)
        if (AllowExternalData)
        {
            List<RapidlibTrainingExample> auxList = new List<RapidlibTrainingExample>(listToSave.Count);          
            for (int i = 0; i < listToSave.Count; i++)
            {
                auxList.Add(new RapidlibTrainingExample(listToSave[i].Input, listToSave[i].Output));
            }
            IMLDataSerialization.SaveTrainingSetToDiskRapidlib(auxList, m_ExternalFileNameForIO);

        }
        // If internal data is being used, we used deprecated serialization from rapidlib (we know it works)
        else
        {
            //Debug.Log("Save training set to disk called! " + filePath);

            // Check if there is NOT a folder with the folder name
            if (!Directory.Exists(Path.Combine(Application.dataPath, m_FolderDataPathName)))
            {
                // If there is not, we create it
                Directory.CreateDirectory(Path.Combine(Application.dataPath, m_FolderDataPathName));
            }

            string subFolderPath = Path.Combine(Application.dataPath, m_FolderDataPathName + this.name);
            //Debug.Log("SUBFOLDER PATH IS: " + subFolderPath);

            // Check if there is NOT a subfolder with the component name
            if (!Directory.Exists(subFolderPath))
            {
                // If there is not, we create it
                Directory.CreateDirectory(subFolderPath);
            }

            // If the option to serialize witht JSON dot net is active...
            if (SerializeWithJSONDotNet)
            {
                // We save the entire input/output list as a JSON
                string auxFilePath = subFolderPath + "/" + m_FileTrainingSetName + "_Inputs_Outputs" + m_FileExtension;
                // Check if there is already a JSON file created for this training example
                if (File.Exists(auxFilePath))
                {
                    // We delete it to make sure we override it
                    File.Delete(auxFilePath);
                }
                // Generate JSON string from the entire list
                var jsonTrainingeExamplesList = JsonConvert.SerializeObject(listToSave);
                // Write on the path
                File.WriteAllText(auxFilePath, jsonTrainingeExamplesList);
            }
            // If not, We keep default behaviour with Unity's built in JSON serializer
            else
            {
                // We iterate through the list to save individual JSON files
                for (int i = 0; i < listToSave.Count; i++)
                {
                    // INPUT
                    // We save each input and output array as a JSON
                    string auxFilePath = subFolderPath + "/" + m_FileTrainingSetName + "_" + i.ToString() + "_INPUT" + m_FileExtension;
                    //Debug.Log("PATH TO SAVE INPUT TRAINING SET IS: " + auxFilePath);
                    // Check if there is already a JSON file created for this training example
                    if (File.Exists(auxFilePath))
                    {
                        // We delete it to make sure we override it
                        File.Delete(auxFilePath);
                    }
                    //Debug.Log("Input_" + i.ToString() + ": " + JsonHelper.ToJson(listToSave[i].input));
                    //Debug.Log("PATH: " + auxFilePath);
                    // Write on the path
                    File.WriteAllText(auxFilePath, JsonHelper.ToJson(listToSave[i].Input));

                    // OUTPUT
                    // We save each input and output array as a JSON
                    auxFilePath = subFolderPath + "/" + m_FileTrainingSetName + "_" + i.ToString() + "_OUTPUT" + m_FileExtension;
                    //Debug.Log("PATH TO SAVE OUTPUT TRAINING SET IS: " + auxFilePath);
                    // Check if there is already a JSON file created for this training example
                    if (File.Exists(auxFilePath))
                    {
                        // We delete it to make sure we override it
                        File.Delete(auxFilePath);
                    }
                    //Debug.Log("Output_" + i.ToString() + ": " + JsonHelper.ToJson(listToSave[i].input));
                    //Debug.Log("PATH: " + auxFilePath);
                    // Write on the path
                    File.WriteAllText(auxFilePath, JsonHelper.ToJson(listToSave[i].Input));

                }

            }


        }


    }

    /// <summary>
    /// Loads Training Data Set from Disk
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>Returns a list with training set</returns>
    private List<TrainingExample> LoadTrainingSetFromDisk (string filePath)
    {
        // If external data is being injected, we use the IMLDataSerialization tool (to avoid inconsistencies in files and folders)
        if (AllowExternalData)
        {
            // Convert to deprecated data type
            List<RapidlibTrainingExample> loadedList = IMLDataSerialization.LoadTrainingSetFromDiskRapidlib(m_ExternalFileNameForIO);
            List<TrainingExample> returnList = new List<TrainingExample>(loadedList.Count);
            for (int i = 0; i < loadedList.Count; i++)
            {

                // Only translate values if they are not null or empty
                if (loadedList[i].Input != null && loadedList[i].Input.Length > 0 
                    && loadedList[i].Output != null && loadedList[i].Output.Length > 0)
                {
                    TrainingExample auxExample = new TrainingExample();
                    auxExample.Input = loadedList[i].Input;
                    auxExample.Output = loadedList[i].Output;
                    // Add each example one by one 
                    returnList.Add(auxExample);
                }
            }
            return returnList;
        }
        // If internal data is being used, we used deprecated serialization from rapidlib (we know it works)
        else
        {
            //Debug.Log("Load training set from disk called! " + filePath);

            // Check if there is NOT a folder with the folder name
            if (!Directory.Exists(Path.Combine(Application.dataPath, m_FolderDataPathName)))
            {
                // If there is not, we create it
                Directory.CreateDirectory(Path.Combine(Application.dataPath, m_FolderDataPathName));
            }

            string subFolderPath = Path.Combine(Application.dataPath, m_FolderDataPathName + this.name);
            //Debug.Log("SUBFOLDER PATH IS: " + subFolderPath);

            // Check if there is NOT a subfolder with the component name
            if (!Directory.Exists(subFolderPath))
            {
                // If there is not, we create it
                Directory.CreateDirectory(subFolderPath);
            }

            List<TrainingExample> auxList = new List<TrainingExample>();

            // If the option to serialize witht JSON dot net is active...
            if (SerializeWithJSONDotNet)
            {
                // We calculate the entire input/output list file name
                string auxFilePath = subFolderPath + "/" + m_FileTrainingSetName + "_Inputs_Outputs" + m_FileExtension;
                //Debug.Log("File name to read is: " + auxFilePath);
                // We check if the file is there before reading from it
                if (File.Exists(auxFilePath))
                {
                    //Debug.Log("The file exists and we read from it!");
                    string jsonTrainingExamplesList = File.ReadAllText(auxFilePath);
                    if (jsonTrainingExamplesList != null)
                        auxList = JsonConvert.DeserializeObject<List<TrainingExample>>(jsonTrainingExamplesList);

                    //Debug.Log("What we read is: " + jsonTrainingExamplesList);
                }
            }
            // If not, We keep default behaviour with Unity's built in JSON serializer
            else
            {
                //INPUT & OUTPUT
                string[] inputFiles = Directory.GetFiles(subFolderPath, "*_INPUT.txt");
                string[] outputFiles = Directory.GetFiles(subFolderPath, "*_OUTPUT.txt");
                //Debug.Log("FILE PATH TO EXTRACT FILES IS: " + subFolderPath);
                //Debug.Log( inputFiles.Length + " Training Files found");
                if (inputFiles != null && inputFiles.Length > 0)
                {
                    double[] contents = new double[inputFiles.Length];
                    for (int i = 0; i < inputFiles.Length; i++)
                    {
                        // Input deserialisation
                        contents = JsonHelper.FromJson<double>(File.ReadAllText(inputFiles[i]));
                        //Debug.Log(contents);
                        auxList.Add(new TrainingExample());
                        auxList[i].Input = contents;
                        // Output deserialisation
                        contents = JsonHelper.FromJson<double>(File.ReadAllText(outputFiles[i]));
                        //Debug.Log(contents.ToString());
                        auxList[i].Output = contents;

                    }
                }
            }

            return auxList;

        }


    }

    private void SetUpFileNamesAndPaths ()
    {
        // Set up file extension type
        m_FileExtension = ".txt";
        
        // Set up file names
        m_FileModelName = gameObject.name + "_Model";
        m_FileTrainingSetName = gameObject.name + "_TrainingSet";

        // Set up file folder name
        m_FolderDataPathName = "RapidLibData/";

        // Set up data path (Application.dataPath + FolderName + FileName + FileExtension)
        m_DataPathModel = Path.Combine(Application.dataPath, m_FolderDataPathName + m_FileModelName + m_FileExtension);
        // Training set is not having the extension added yet
        m_DataPathTrainingSet = Path.Combine(Application.dataPath, m_FolderDataPathName + m_FileTrainingSetName);
        //Debug.Log("datapath TRAINING SET IS: " + m_DataPathTrainingSet);

    }

    #endregion

}
