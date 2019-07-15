using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;
using ReusableMethods;

namespace InteractML
{
    /// <summary>
    /// Holds the information and list of a training examples node
    /// </summary>
    [NodeWidth(500)]
    public class TrainingExamplesNode : Node
    {

        #region Variables

        /// <summary>
        /// The input features passed in to the node
        /// </summary>
        [Input]
        public List<Node> InputFeatures;

        /// <summary>
        /// The training examples node that we are sending as output 
        /// </summary>
        [Output, SerializeField]
        public TrainingExamplesNode TrainingExamplesNodeToOutput;

        /// <summary>
        /// The output training examples vector of this node
        /// </summary>
        [SerializeField, HideInInspector]
        public List<IMLTrainingExample> TrainingExamplesVector;

        public List<IMLBaseDataType> testList;

        public List<IMLBaseDataType> testListReadFromDisk;

        /// <summary>
        /// Configuration of desired inputs for the Training Examples node 
        /// </summary>
        [SerializeField]
        private List<IMLSpecifications.InputsEnum> m_DesiredInputsConfig;
        public List<IMLSpecifications.InputsEnum> DesiredInputsConfig { get { return m_DesiredInputsConfig; } }

        /// <summary>
        /// Configuration of desired outputs for a specific Training Set
        /// </summary>
        [SerializeField]
        private List<IMLSpecifications.OutputsEnum> m_DesiredOutputsConfig;
        public List<IMLSpecifications.OutputsEnum> DesiredOutputsConfig { get { return m_DesiredOutputsConfig; } }
        /// <summary>
        /// This one is kept to compare if the structure of the outputs has changed
        /// </summary>
        private List<IMLSpecifications.OutputsEnum> m_LastKnownDesireOutputsConfig;


        private List<IMLBaseDataType> m_DesiredOutputFeatures;
        /// <summary>
        /// List of desired outputs for this training set
        /// </summary>
        public List<IMLBaseDataType> DesiredOutputFeatures { get { return m_DesiredOutputFeatures; } }

        /// <summary>
        /// Private member, returns total number of training examples from vector (if reference not null)
        /// </summary>
        private int m_TotalNumberOfTrainingExamples { get { return (TrainingExamplesVector != null ? TrainingExamplesVector.Count : 0); } }
        /// <summary>
        /// Total number of training examples
        /// </summary>
        public int TotalNumberOfTrainingExamples { get { return m_TotalNumberOfTrainingExamples; } }

        /// <summary>
        /// Used to store the different lengths of features during feature extraction
        /// </summary>
        private int[] m_LengthsFeatureVector;

        /// <summary>
        /// The list of IML Config nodes connected
        /// </summary>
        [HideInInspector]
        public List<IMLConfiguration> IMLConfigurationNodesConnected;

        // Variables for collecting data
        public float StartDelay = 0.0f;
        public float CaptureRate = 10.0f;
        [HideInInspector]
        public float RecordTime = -1.0f;
        float m_TimeToNextCapture = 0.0f;
        float m_TimeToStopCapture = 0.0f;

        private bool m_CollectData;
        /// <summary>
        /// Is the Node Collecting Data?
        /// </summary>
        public bool CollectingData { get { return m_CollectData; } }

        /// <summary>
        /// Flag to have a shortcut to collect data (CTRL + Space)
        /// </summary>
        public bool EnableCollectDataShortcut;

        #endregion

        #region XNode Messages

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            Initialize();
         
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            TrainingExamplesNodeToOutput = this;

            return this; 
        }

        #endregion

        #region Unity Messages

        public void OnValidate()
        {
            // Check that the output list is being updated properly
            //UpdateOutputsList();

        }

        private void OnDestroy()
        {
            // Remove this node from IML Component controlling it (if any)
            var MLController = graph as IMLController;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.TrainingExamplesNodesList.Remove(this);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialises nodes values and vars
        /// </summary>
        public void Initialize()
        {
            // Make sure internal feature lists are initialized
            if (m_DesiredInputsConfig == null)
                m_DesiredInputsConfig = new List<IMLSpecifications.InputsEnum>();

            if (m_DesiredOutputsConfig == null)
                m_DesiredOutputsConfig = new List<IMLSpecifications.OutputsEnum>();

            if (m_DesiredOutputFeatures == null)
                m_DesiredOutputFeatures = new List<IMLBaseDataType>();

            if (Lists.IsNullOrEmpty(ref TrainingExamplesVector))
                TrainingExamplesVector = new List<IMLTrainingExample>();

            if (IMLConfigurationNodesConnected == null)
                IMLConfigurationNodesConnected = new List<IMLConfiguration>();

            // Load training data from disk
            LoadDataFromDisk();
        }

        /// <summary>
        /// Starts/Stops collecting examples when called
        /// </summary>
        public void ToggleCollectExamples()
        {
            ToggleCollectingDataPrivate();
        }

        /// <summary>
        /// Update logic per node
        /// </summary>
        public void UpdateLogic()
        {
            // Keep input config list updated
            UpdateInputConfigList();

            // Make sure the output list is properly update 
            // TO DO: Potentially makes sense to move this to OnValidate to avoid calling it all the time
            UpdateOutputsList();

            // Run examples logic in case we need to start/stop collecting examples
            CollectExamplesLogic();

            // Check input for keyboard shortcut in case is pressed
            if (EnableCollectDataShortcut)
            {
                if (Input.GetKeyDown(KeyCode.Space) )
                {
                    ToggleCollectExamples();
                }
            }
        }

        /// <summary>
        /// Clears all the training examples stored in the node
        /// </summary>
        public void ClearTrainingExamples()
        {
            // Clear examples in node
            TrainingExamplesVector.Clear();

            // I AM GOING TO KEEP THE DESIRED OUTPUTS UNCHANGED FOR THE MOMENT

            // Clear list of desired outputs (which is also fed to the training examples vector when adding a single training example)
            //DesiredOutputFeatures.Clear();

            // Make sure the outputs are populated properly after clearing them out
            UpdateOutputsList();
        }

        public void Terminate()
        {
            SaveDataToDisk();
            Debug.Log("Training Examples Vector Saved On Terminate!");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the configuration list of inputs
        /// </summary>
        private void UpdateInputConfigList()
        {
            // Get values from the input list
            InputFeatures = GetInputValues<Node>("InputFeatures").ToList();

            // Make sure that the list is initialised
            if (m_DesiredInputsConfig == null)
                m_DesiredInputsConfig = new List<IMLSpecifications.InputsEnum>();

            // Adjust the desired inputs list based on nodes connected
            m_DesiredInputsConfig.Clear();
            // Go through all the nodes connected
            for (int i = 0; i < InputFeatures.Count; i++)
            {
                // Cast the node checking if implements the feature interface (it is a featureExtractor)
                IFeatureIML inputFeature = InputFeatures[i] as IFeatureIML;

                // If it is a feature extractor...
                if (inputFeature != null)
                {
                    // We add the feature to the desired inputs config
                    DesiredInputsConfig.Add((IMLSpecifications.InputsEnum)inputFeature.FeatureValues.DataType);
                }
            }
        }

        /// <summary>
        /// Makes sure the list of outputs is properly configured
        /// </summary>
        private void UpdateOutputsList()
        {
            // Check if we actually need to rebuild the desired output features
            // If we have no record of the structure OR current and last known structures doesn't match
            if ( m_LastKnownDesireOutputsConfig == null || ! m_LastKnownDesireOutputsConfig.SequenceEqual(m_DesiredOutputsConfig) )
            {
                //Debug.Log("[UPDATE] REBUILDING OUTPUT LIST");
                // Build Output List
                BuildOutputFeaturesFromOutputConfig();
                m_LastKnownDesireOutputsConfig = m_DesiredOutputsConfig;
            }

            // If the size of the output configuration doesn't match the output features size, something is wrong
            if (m_DesiredOutputsConfig.Count != m_DesiredOutputFeatures.Count)
            {
                // Re-Build Output List
                BuildOutputFeaturesFromOutputConfig();
            }
            // If both lists are the same size...
            else
            {
                // Double making sure that each of the outputs specified in the configuration matches the required type
                for (int i = 0; i < m_DesiredOutputFeatures.Count; i++)
                {
                    // If the data type doesn't match the one specified in the configuration...
                    if (m_DesiredOutputFeatures[i].DataType != (IMLSpecifications.DataTypes) m_DesiredOutputsConfig[i])
                    {
                        //Debug.LogError("DataType not equal!");
                        // Correct that output feature with the right type
                        switch (m_DesiredOutputsConfig[i])
                        {
                            case IMLSpecifications.OutputsEnum.Float:
                                m_DesiredOutputFeatures[i] = new IMLFloat();
                                break;
                            case IMLSpecifications.OutputsEnum.Integer:
                                m_DesiredOutputFeatures[i] = new IMLInteger();
                                break;
                            case IMLSpecifications.OutputsEnum.Vector2:
                                m_DesiredOutputFeatures[i] = new IMLVector2();
                                break;
                            case IMLSpecifications.OutputsEnum.Vector3:
                                m_DesiredOutputFeatures[i] = new IMLVector3();
                                break;
                            case IMLSpecifications.OutputsEnum.Vector4:
                                m_DesiredOutputFeatures[i] = new IMLVector4();
                                break;
                            default:
                                break;
                        }
                        
                    }
                }

            }

        }

        /// <summary>
        /// Clears the output features and rebuilds them from the configuration specified
        /// </summary>
        private void BuildOutputFeaturesFromOutputConfig()
        {
            // Adjust the desired outputs list based on configuration selected
            m_DesiredOutputFeatures.Clear();
            // Calculate required space for outputs
            for (int i = 0; i < m_DesiredOutputsConfig.Count; i++)
            {
                switch (m_DesiredOutputsConfig[i])
                {
                    case IMLSpecifications.OutputsEnum.Float:
                        m_DesiredOutputFeatures.Add(new IMLFloat());
                        break;
                    case IMLSpecifications.OutputsEnum.Integer:
                        m_DesiredOutputFeatures.Add(new IMLInteger());
                        break;
                    case IMLSpecifications.OutputsEnum.Vector2:
                        m_DesiredOutputFeatures.Add(new IMLVector2());
                        break;
                    case IMLSpecifications.OutputsEnum.Vector3:
                        m_DesiredOutputFeatures.Add(new IMLVector3());
                        break;
                    case IMLSpecifications.OutputsEnum.Vector4:
                        m_DesiredOutputFeatures.Add(new IMLVector4());
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// Logic to collect examples. Needs to be called in an Update loop
        /// </summary>
        private void CollectExamplesLogic()
        {
            if (m_CollectData)
            {
                if (Application.isPlaying && m_TimeToStopCapture > 0 && Time.time >= m_TimeToStopCapture)
                {
                    m_CollectData = false;
                    //Debug.Log("end recording");
                }
                else if (!Application.isPlaying || Time.time >= m_TimeToNextCapture)
                {
                    ////Debug.Log ("recording");
                    AddTrainingExample();
                    m_TimeToNextCapture = Time.time + 1.0f / CaptureRate;
                }

            }

        }

        /// <summary>
        /// Adds a single training example 
        /// </summary>
        private void AddTrainingExample()
        {
            // Declare new example to add to vector
            IMLTrainingExample newExample = new IMLTrainingExample();

            // Add all the input features to the training example being recorded
            for (int i = 0; i < InputFeatures.Count; i++)
            {
                newExample.AddInputExample((InputFeatures[i] as IFeatureIML).FeatureValues);
            }

            // Add all the output features to the training example being recorded
            for (int i = 0; i < m_DesiredOutputFeatures.Count; i++)
            {
                newExample.AddOutputExample(m_DesiredOutputFeatures[i]);
            }

            // Add the training example to the vector
            TrainingExamplesVector.Add(newExample);

        }

        /// <summary>
        /// Starts/Stops Collecting data
        /// </summary>
        private void ToggleCollectingDataPrivate()
        {
            if (m_CollectData)
            {
                StopCollectingData();
            }
            else
            {
                StartCollectingData();
            }

        }

        /// <summary>
        /// Sets the collecting data flag to true and configures the class to collect data
        /// </summary>
        private void StartCollectingData()
        {

            m_CollectData = true;
            Debug.Log("starting recording in " + StartDelay + " seconds");
            m_TimeToNextCapture = Time.time + StartDelay;
            if (RecordTime > 0)
            {
                m_TimeToStopCapture = Time.time + StartDelay + RecordTime;
            }
            else
            {
                m_TimeToStopCapture = -1;
            }

        }

        /// <summary>
        /// Sets the collect data flag to false to stop collecting data
        /// </summary>
        private void StopCollectingData()
        {
            m_CollectData = false;

            // Save data to disk
            SaveDataToDisk();
        }


        public void SaveDataToDisk()
        {
            IMLDataSerialization.SaveTrainingSetToDisk(TrainingExamplesVector, GetJSONFileName());

        }

        public void LoadDataFromDisk()
        {
            //Load training data from disk
            var auxTrainingExamplesVector = IMLDataSerialization.LoadTrainingSetFromDisk(GetJSONFileName());
            if (!Lists.IsNullOrEmpty(ref auxTrainingExamplesVector))
            {
                TrainingExamplesVector = auxTrainingExamplesVector;
                //Debug.Log("Training Examples Vector loaded!");
            }

        }

        /// <summary>
        /// Returns the file name we want for the JSON, both for read and write
        /// </summary>
        /// <returns></returns>
        public string GetJSONFileName ()
        {
            string graphName = this.graph.name;
            string nodeIndex = this.graph.nodes.FindIndex(a => a == this).ToString();
            string fileName = graphName + "_node_" + nodeIndex + "_" + "TrainingExamplesNode";
            return fileName;
        }

        #endregion

    }
}