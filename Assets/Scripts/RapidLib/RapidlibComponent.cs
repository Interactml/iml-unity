using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InteractML
{
    /// <summary>
    /// Allows to do IML operations in the Unity Editor
    /// </summary>
    public class RapidlibComponent : MonoBehaviour
    {
        #region Variables

        private EasyRapidlib m_EasyRapidlib;

        public bool AllowKeyboardShortcuts;

        //This is what you need to show in the inspector.
        public EasyRapidlib.LearningType learningType;

        //public bool classification = false;
        [HideInInspector]
        public float startDelay = 0.0f;
        public float captureRate = 10.0f;
        [HideInInspector]
        public float recordTime = -1.0f;
        float timeToNextCapture = 0.0f;
        float timeToStopCapture = 0.0f;

        public Transform[] inputs;

        public double[] outputs;
        /// <summary>
        /// Last class predicted by the dtw algorithm
        /// </summary>
        public string OutputDTW;

        private bool run = false;
        public bool Running { get { return run; } }

        public bool collectData = false;
        public bool CollectingData { get { return collectData; } }

        public bool Training { get { return m_EasyRapidlib.Training; } }
        public bool Trained { get { return m_EasyRapidlib.Trained; } }

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

        /// <summary>
        /// Serie used to collect values and push them to EasyRapidlib
        /// </summary>
        private RapidlibTrainingSerie m_TrainingSerieDTW;
        /// <summary>
        /// Training examples serie used to run the model
        /// </summary>
        private RapidlibTrainingSerie m_RunningSerieDTW;

        #endregion

        #region Unity Messages

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            CalculateFeatures();

            RunModelLogic();

            CollectDataLogic();

#if UNITY_EDITOR
            if (AllowKeyboardShortcuts)
            {
                // Space collects data
                if (Input.GetKeyDown("space"))
                {
                    ToggleCollectingData();
                }
                // T key trains the model
                if (Input.GetKeyDown(KeyCode.T))
                {
                    Train();
                }
                // R key starts running the model
                if (Input.GetKeyDown(KeyCode.R))
                {
                    ToggleRunning();
                }
            }

#endif

            // Storing info for feature extraction at the end of a frame
            if (inputs != null && inputs.Length > 0)
            {
                // Storing this frame as the last and this velocity as the last to properly calculate velocity and accel every frame      
                m_LastFramePosition = inputs[0].position;
                m_LastFrameVelocity = m_Velocity;
            }

        }

        // Called when the script is loaded or a value is changed in the inspector (Called in the editor only).
        private void OnValidate()
        {
            // Make sure easy rapidlib instance exists
            if (m_EasyRapidlib == null)
                m_EasyRapidlib = new EasyRapidlib(learningType);

            // Did the learning type change in the editor?
            if (learningType != m_EasyRapidlib.LearningTypeModel)
            {
                // Override model
                m_EasyRapidlib.OverrideModel(learningType);
            }
            
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

        #endregion

        #region Public Methods

        /* COLLECTING DATA METHODS*/

        public void StartCollectingData()
        {

            collectData = true;
            if (!Running)
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
            if (learningType == EasyRapidlib.LearningType.DTW)
            {
                if (!Running)
                {
                    // Push collected training serie to easyRapidlib
                    m_EasyRapidlib.AddTrainingExampleSerie(m_TrainingSerieDTW);
                }
                // Make sure to clear the collected training serie 
                m_TrainingSerieDTW.ClearSerie();
            }
            collectData = false;
        }

        public void ToggleCollectingData()
        {
            if (collectData)
            {
                StopCollectingData();
            }
            else
            {
                StartCollectingData();
            }
        }

        /* RUNNING METHODS */

        public void StartRunning()
        {
            run = true;
        }

        public void StopRunning()
        {
            if (learningType == EasyRapidlib.LearningType.DTW)
            {
                // Runs dtw on the collected running example serie
                OutputDTW = m_EasyRapidlib.Run(m_RunningSerieDTW);
                // Make sure to clear the collected running serie 
                m_RunningSerieDTW.ClearSerie();

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

        /* TRAINING METHODS */

        public void Train()
        {
            m_EasyRapidlib.TrainModel();
        }

        /* TRAINING EXAMPLES METHODS */

        /// <summary>
        /// Adds a training example based on the configuration in the editor
        /// </summary>
        public void AddTrainingExample()
        {
            RapidlibTrainingExample newExample = new RapidlibTrainingExample();

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


            m_EasyRapidlib.AddTrainingExample(newExample);

        }

        /// <summary>
        /// Adds an already created training example to rapidlib
        /// </summary>
        /// <param name="newExample"></param>
        public void AddTrainingExample(RapidlibTrainingExample newExample)
        {
            m_EasyRapidlib.AddTrainingExample(newExample);
        }

        /// <summary>
        /// Adds a single example to a serie
        /// </summary>
        /// <param name="serieToReceiveExample"></param>
        public void AddExampleToSerie(ref RapidlibTrainingSerie serieToReceiveExample)
        {
            // Calculate size of the new example input vector [I CAN MOVE THIS TO THE START OF THE PROGRAM]
            int sizeNewExampleInput = 0;
            for (int i = 0; i < m_Features.Count; i++)
            {
                sizeNewExampleInput += (m_LengthsFeatureVector[i] * inputs.Length);
            }

            double[] singleInputExampleInSerie = new double[sizeNewExampleInput];

            m_ExtractFeatures(ref singleInputExampleInSerie, inputs);

            // Add a single example to the training serie dtw that we will push later on to easyRapidlib
            serieToReceiveExample.AddTrainingExample(singleInputExampleInSerie, OutputDTW.ToString());
        }

        [ContextMenu("Clear Training Examples")]
        public void ClearTrainingExamples()
        {
            if (learningType == EasyRapidlib.LearningType.DTW)
            {
                m_EasyRapidlib.TrainingExamplesSeries.Clear();
            }
            else
            {
                m_EasyRapidlib.TrainingExamples.Clear();
            }
        }

        public List<RapidlibTrainingExample> GetTrainingExamples()
        {
            if (m_EasyRapidlib == null)
                m_EasyRapidlib = new EasyRapidlib(learningType);
            if (m_EasyRapidlib.TrainingExamples ==  null)
                m_EasyRapidlib.TrainingExamples = new List<RapidlibTrainingExample>();
            return m_EasyRapidlib.TrainingExamples;
        }

        public List<RapidlibTrainingSerie> GetTrainingExamplesSeries()
        {
            if (m_EasyRapidlib == null)
                m_EasyRapidlib = new EasyRapidlib(learningType);
            if (m_EasyRapidlib.TrainingExamplesSeries == null)
                m_EasyRapidlib.TrainingExamplesSeries = new List<RapidlibTrainingSerie>();
            return m_EasyRapidlib.TrainingExamplesSeries;
        }

        public void SaveDataToDisk()
        {
            m_EasyRapidlib.SaveModelToDisk("RapidlibComponent_Model_" + this.name + "_" + SceneManager.GetActiveScene().name);
            m_EasyRapidlib.SaveTrainingExamplesToDisk("RapidlibComponent_TrainingExamples_" + this.name + "_" + SceneManager.GetActiveScene().name);
            m_EasyRapidlib.SaveTrainingSeriesToDisk("RapidlibComponent_TrainingSeriesCollection_" + this.name + "_" + SceneManager.GetActiveScene().name);
        }

        public void LoadDataFromDisk()
        {
            bool modelLoaded = m_EasyRapidlib.LoadModelFromDisk("RapidlibComponent_Model_" + this.name + "_" + SceneManager.GetActiveScene().name);

            if (modelLoaded)
            {
                // Do we need to update our learning type?
                if (learningType != m_EasyRapidlib.LearningTypeModel)
                {
                    learningType = m_EasyRapidlib.LearningTypeModel;
                }
            }

            // If we have a dtw model...
            if (learningType == EasyRapidlib.LearningType.DTW)
            {
                // We attempt to load a training series collection
                m_EasyRapidlib.LoadTrainingSeriesFromDisk("RapidlibComponent_TrainingSeriesCollection_" + this.name + "_" + SceneManager.GetActiveScene().name);

            }
            // If it is a classification or regression model...
            else
            {
                // We attempt to load a training examples set
                m_EasyRapidlib.LoadTrainigExamplesFromDisk("RapidlibComponent_TrainingExamples_" + this.name + "_" + SceneManager.GetActiveScene().name);
            }

        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            // Make sure inputs array is not null (to avoid null references when opening for the first time the project)
            if (inputs == null)
                inputs = new Transform[0];

            // Set size of outputs array to 1
            outputs = new double[1];

            // Storing the this frame as the last frame to properly calculate velocity every frame      
            m_LastFramePosition = this.transform.position;

            // Initialize array of distance of all inputs to first one based on number of inputs
            if (inputs.Length > 0)
            {
                // We will need to calculate the distance from everything to the first one
                m_DistancesToFirstInput = new float[inputs.Length - 1];
            }

            // Initialise instance of easy rapidlib
            if (m_EasyRapidlib == null)
                m_EasyRapidlib = new EasyRapidlib(learningType);

        }

        private void CalculateFeatures()
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

        private void RunModelLogic()
        {
            if (run && !m_EasyRapidlib.isModelEmpty)
            {
                // DTW
                if (learningType == EasyRapidlib.LearningType.DTW)
                {
                    // We init the running serie if it is null
                    if (m_RunningSerieDTW.ExampleSerie == null)
                        m_RunningSerieDTW = new RapidlibTrainingSerie();

                    // We start adding examples to the running serie until the user stops
                    AddExampleToSerie(ref m_RunningSerieDTW);                    
                }
                // Classification and Regression
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
                    //for (int i = 0; i < outputs.Length; i++)
                    //{
                    //    //Debug.Log(outputs[i]);
                    //}
                    
                    // Get the predictions based on the features we input
                    outputs = m_EasyRapidlib.Run(modelInputs, outputs.Length);

                }
            }

        }

        private void CollectDataLogic()
        {
            if (collectData)
            {
                if (Application.isPlaying && timeToStopCapture > 0 && Time.time >= timeToStopCapture)
                {
                    StopCollectingData();
                    //Debug.Log("end recording");
                }
                else if (!Application.isPlaying || Time.time >= timeToNextCapture)
                {
                    ////Debug.Log ("recording");
                    // Logic to gather training data is different for classification/regression and dtw
                    switch (learningType)
                    {
                        case EasyRapidlib.LearningType.Classification:
                            AddTrainingExample();
                            break;
                        case EasyRapidlib.LearningType.Regression:
                            AddTrainingExample();
                            break;
                        case EasyRapidlib.LearningType.DTW:
                            AddExampleToSerie(ref m_TrainingSerieDTW);
                            break;
                        default:
                            break;
                    }
                    timeToNextCapture = Time.time + 1.0f / captureRate;
                }

            }

        }

        private void ResetPointersFeatureVector(ref double[] inputForModel, Transform[] rawInputData)
        {
            m_PointerFeatureVector = 0;
        }

        private void ExtractPosition(ref double[] inputForModel, Transform[] rawInputData)
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



        #endregion
    }

}

