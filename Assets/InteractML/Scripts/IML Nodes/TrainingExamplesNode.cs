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
        [Input]
        public List<Node> TargetValues;

        /// <summary>
        /// The training examples node that we are sending as output 
        /// </summary>
        [Output, SerializeField]
        public TrainingExamplesNode TrainingExamplesNodeToOutput;

        
        public enum CollectionMode { SingleExample, Series }
        [HideInInspector]
        public CollectionMode ModeOfCollection;

        /// <summary>
        /// The output training examples vector of this node
        /// </summary>
        [SerializeField, HideInInspector]
        public List<IMLTrainingExample> TrainingExamplesVector;

        /// <summary>
        /// The output training examples series collection of this node
        /// </summary>
        [SerializeField, HideInInspector]
        public List<IMLTrainingSeries> TrainingSeriesCollection;

        /// <summary>
        /// Series use to add information while we collect data. 
        /// It will be later added to the training series collection
        /// </summary>
        protected IMLTrainingSeries m_SingleSeries;

        public List<IMLBaseDataType> testList;

        public List<IMLBaseDataType> testListReadFromDisk;

        /// <summary>
        /// Configuration of desired inputs for the Training Examples node 
        /// </summary>
        [SerializeField, HideInInspector]
        protected List<IMLSpecifications.InputsEnum> m_DesiredInputsConfig;
        public List<IMLSpecifications.InputsEnum> DesiredInputsConfig { get { return m_DesiredInputsConfig; } set { this.m_DesiredInputsConfig = value; } }

        /// <summary>
        /// Configuration of desired outputs for a specific Training Set
        /// </summary>
        [SerializeField, HideInInspector]
        protected List<IMLSpecifications.OutputsEnum> m_DesiredOutputsConfig;
        public List<IMLSpecifications.OutputsEnum> DesiredOutputsConfig { get { return m_DesiredOutputsConfig; } set { this.m_DesiredOutputsConfig = value; } }
        /// <summary>
        /// This one is kept to compare if the structure of the outputs has changed
        /// </summary>
        protected List<IMLSpecifications.OutputsEnum> m_LastKnownDesireOutputsConfig;


        protected List<IMLBaseDataType> m_DesiredOutputFeatures;
        /// <summary>
        /// List of desired outputs for this training set
        /// </summary>
        public List<IMLBaseDataType> DesiredOutputFeatures { get { return m_DesiredOutputFeatures; } }

        /// <summary>
        /// protected member, returns total number of training examples from vector (if reference not null)
        /// </summary>
        protected int m_TotalNumberOfTrainingExamples { get { return (TrainingExamplesVector != null ? TrainingExamplesVector.Count : 0); } }
        /// <summary>
        /// Total number of training examples
        /// </summary>
        public int TotalNumberOfTrainingExamples { get { return m_TotalNumberOfTrainingExamples; } }

        /// <summary>
        /// Used to store the different lengths of features during feature extraction
        /// </summary>
        protected int[] m_LengthsFeatureVector;

        /// <summary>
        /// The list of IML Config nodes connected
        /// </summary>
        [HideInInspector]
        public List<IMLConfiguration> IMLConfigurationNodesConnected;

        // Variables for collecting data
        [HideInInspector]
        public float StartDelay = 0.0f;
        [HideInInspector]
        public float CaptureRate = 10.0f;
        [HideInInspector]
        public float RecordTime = -1.0f;
        protected float m_TimeToNextCapture = 0.0f;
        protected float m_TimeToStopCapture = 0.0f;

        protected bool m_CollectData;
        /// <summary>
        /// Is the Node Collecting Data?
        /// </summary>
        public bool CollectingData { get { return m_CollectData; } }

        /// <summary>
        /// Flag to have a shortcut to collect data (CTRL + Space)
        /// </summary>
        public bool EnableKeyboardControl;
        [HideInInspector]
        public KeyCode RecordDataKey;

        public IMLNodeTooltips tips;

        #endregion

        #region XNode Messages

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            Initialize();

            tips = IMLTooltipsSerialization.LoadTooltip("TrainingExamples");
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            this.DisconnectIfNotType<TrainingExamplesNode, IFeatureIML>(from, to);
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            TrainingExamplesNodeToOutput = this;

            return this; 
        }

        #endregion

        #region Unity Messages

        //public void OnValidate()
        //{
        //    // Check that the output list is being updated properly
        //    //UpdateOutputsList();

        //}
        //check 
        protected void OnDestroy()
        {
            // Remove this node from IML Component controlling it (if any)
            var MLController = graph as IMLController;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.DeleteTrainingExamplesNode(this);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialises nodes values and vars
        /// </summary>
        public void Initialize()
        {
            SetDataCollection();
            // Make sure internal feature lists are initialized
            if (m_DesiredInputsConfig == null)
                m_DesiredInputsConfig = new List<IMLSpecifications.InputsEnum>();

            if (m_DesiredOutputsConfig == null)
                m_DesiredOutputsConfig = new List<IMLSpecifications.OutputsEnum>();

            if (m_DesiredOutputFeatures == null)
                m_DesiredOutputFeatures = new List<IMLBaseDataType>();

            if (Lists.IsNullOrEmpty(ref TrainingExamplesVector))
                TrainingExamplesVector = new List<IMLTrainingExample>();

            if (Lists.IsNullOrEmpty(ref TrainingSeriesCollection))
                TrainingSeriesCollection = new List<IMLTrainingSeries>();

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
            ToggleCollectingDataprotected();
        }

        // check move to single training examples node
        /// <summary>
        /// Adds a single training example to the node's list
        /// </summary>
        public void AddSingleTrainingExample()
        {
            AddTrainingExampleprotected();
        }

        /// <summary>
        /// Update logic per node
        /// </summary>
        public void UpdateLogic()
        {
            // Handle Input
            KeyboardInput();

            // Keep input config list updated
            UpdateInputConfigList();

            // Make sure the output list is properly update 
            // TO DO: Potentially makes sense to move this to OnValidate to avoid calling it all the time
            UpdateOutputsList();

            // Run examples logic in case we need to start/stop collecting examples
            CollectExamplesLogic();

            // Check input for keyboard shortcut in case is pressed
            //if (EnableKeyboardShortcut)
            //{
            //    if (Input.GetKeyDown(KeyCode.Space) )
            //    {
            //        ToggleCollectExamples();
            //    }
            //}
        }

        /// <summary>
        /// Clears all the training examples stored in the node
        /// </summary>
        public virtual void ClearTrainingExamples()
        {

            // Account for mode
            switch (ModeOfCollection)
            {
                case CollectionMode.SingleExample:
                    // Clear examples in node
                    TrainingExamplesVector.Clear();
                    break;
                case CollectionMode.Series:
                    // Clear series in node
                    TrainingSeriesCollection.Clear();
                    break;
                default:
                    break;
            }



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

        #region protected Methods

        /// <summary>
        /// Sets Data Collection Type 
        /// </summary>
        protected virtual void SetDataCollection()
        {

        }

        /// <summary>
        /// Updates the configuration list of inputs
        /// </summary>
        protected void UpdateInputConfigList()
        {
            // Get values from the input list
            InputFeatures = this.GetInputNodesConnected("InputFeatures");
            // if there are inputfestures connected 
             if (InputFeatures != null)
            {
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
            
        }

        /// <summary>
        /// Makes sure the list of outputs is properly configured
        /// </summary>
        protected void UpdateOutputsList()
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
        protected void BuildOutputFeaturesFromOutputConfig()
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
        protected virtual void CollectExamplesLogic()
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
                    //check 
                    // We check which modality of collection is selected
                    switch (ModeOfCollection)
                    {
                        case CollectionMode.SingleExample:
                            AddTrainingExampleprotected();
                            break;
                        case CollectionMode.Series:
                            AddInputsToSeries(InputFeatures, 
                                IMLDataSerialization.ParseIMLFeatureToJSON(DesiredOutputFeatures), 
                                ref m_SingleSeries);
                            break;
                        default:
                            break;
                    }

                    ////Debug.Log ("recording");
                    m_TimeToNextCapture = Time.time + 1.0f / CaptureRate;
                }

            }

        }

        /// <summary>
        /// Adds a single training example 
        /// </summary>
        protected void AddTrainingExampleprotected()
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

        protected void AddInputsToSeries(List<Node> inputs, string label, ref IMLTrainingSeries series)
        {
            // Only add if inputs are not null or empty (the series can be empty)
            if (!Lists.IsNullOrEmpty(ref inputs))
            {
                // We get the Input Features from the n
                List<IMLInput> featuresInput = new List<IMLInput>(inputs.Count);
                // Add all the input features to the series being recorded
                for (int i = 0; i < inputs.Count; i++)
                {
                    if (inputs[i] is IFeatureIML feature)
                    {
                       featuresInput.Add(new IMLInput(feature.FeatureValues));
                    }
                        
                }

                // Add features to series
                m_SingleSeries.AddFeatures(featuresInput, label);
            }
        }

        /// <summary>
        /// Starts/Stops Collecting data
        /// </summary>
        protected void ToggleCollectingDataprotected()
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
        protected void StartCollectingData()
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
        protected virtual void StopCollectingData()
        {
            m_CollectData = false;

            // If we are in collect series mode...
            if (ModeOfCollection == CollectionMode.Series)
            {
                // We add our series to the series collection
                TrainingSeriesCollection.Add(new IMLTrainingSeries(m_SingleSeries));
                m_SingleSeries.ClearSerie();

            }

            // Save data to disk
            SaveDataToDisk();
        }


        public virtual void SaveDataToDisk()
        {
            if (ModeOfCollection == CollectionMode.SingleExample)
            {
                IMLDataSerialization.SaveTrainingSetToDisk(TrainingExamplesVector, GetJSONFileName());
            } else if (ModeOfCollection == CollectionMode.Series)
            {
                IMLDataSerialization.SaveTrainingSeriesCollectionToDisk(TrainingSeriesCollection, GetJSONFileName());
            }else
            {
                Debug.LogWarning("No data collection set");
            }
            
            
        }

        public virtual void LoadDataFromDisk()
        {
            //Load training data from disk
            var auxTrainingExamplesVector = IMLDataSerialization.LoadTrainingSetFromDisk(GetJSONFileName());
            if (!Lists.IsNullOrEmpty(ref auxTrainingExamplesVector))
            {
                TrainingExamplesVector = auxTrainingExamplesVector;
                //Debug.Log("Training Examples Vector loaded!");
            }

            // Load IML Series Collection from disk
            var auxTrainingSeriesCollection = IMLDataSerialization.LoadTrainingSeriesCollectionFromDisk(GetJSONFileName());
            if (!Lists.IsNullOrEmpty(ref auxTrainingSeriesCollection))
            {
                TrainingSeriesCollection = auxTrainingSeriesCollection;
            }
        }

        /// <summary>
        /// Returns the file name we want for the regular training examples list in JSON format, both for read and write
        /// </summary>
        /// <returns></returns>
        public virtual string GetJSONFileName ()
        {
            string graphName = this.graph.name;
            string fileName = graphName + "TrainingExamplesNode" + this.id;
            return fileName;
        }

        protected void KeyboardInput()
        {
            if (EnableKeyboardControl)
            {
                if (Input.GetKeyDown(RecordDataKey))
                {
                    ToggleCollectExamples();
                }

            }
        }

        #endregion

    }
}