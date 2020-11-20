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
    [NodeWidth(300)]
    public class TrainingExamplesNode : IMLNode
    {

        #region Variables

        /// <summary>
        /// The input features passed in to the node
        /// </summary>
        [Input]
        public List<Node> InputFeatures;
        /// <summary>
        /// Target data types /features passed to node
        /// </summary>
        [Input]
        public List<Node> TargetValues;

        /// <summary>
        /// The training examples node that we are sending as output 
        /// </summary>
        [Output, SerializeField]
        public TrainingExamplesNode TrainingExamplesNodeToOutput;

        /// <summary>
        /// Enum types of collection 
        /// </summary>
        public enum CollectionMode { SingleExample, Series }
        /// <summary>
        /// Collection modes of the current training example node 
        /// </summary>
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
        public List<IMLSpecifications.OutputsEnum> DesiredOutputsConfig { get { return m_DesiredOutputsConfig; }}
        /// <summary>
        /// This one is kept to compare if the structure of the outputs has changed
        /// </summary>
        protected List<IMLSpecifications.OutputsEnum> m_LastKnownDesireOutputsConfig;

        /// <summary>
        /// IMLBaseDataTypes of input and output features
        /// </summary>
        protected List<IMLBaseDataType> m_DesiredOutputFeatures;
        protected List<IMLBaseDataType> m_DesiredInputFeatures;
        /// <summary>
        /// List of desired IMLBaseDataTypes outputs/inputs for this training set
        /// </summary>
        public List<IMLBaseDataType> DesiredOutputFeatures { get { return m_DesiredOutputFeatures; } }
        public List<IMLBaseDataType> DesiredInputFeatures { get { return m_DesiredInputFeatures; } }

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
        public List<MLSystem> MLSystemNodesConnected;
        /// <summary>
        /// Variables for setting delay in time for collecting data
        /// </summary>
        [HideInInspector]
        public float StartDelay = 0.0f;
        [HideInInspector]
        public float CaptureRate = 10.0f;
        [HideInInspector]
        public float RecordTime = -1.0f;
        protected float m_TimeToNextCapture = 0.0f;
        protected float m_TimeToStopCapture = 0.0f;

        /// <summary>
        /// boolean for whether collecting data or not 
        /// </summary>
        protected bool m_CollectingData;
        public bool CollectingData { get { return m_CollectingData; } }

        /// <summary>
        /// Flag to have a shortcut to collect data (CTRL + Space)
        /// </summary>
        public bool EnableKeyboardControl;
        /// <summary>
        /// What key willl record data 
        /// </summary>
        [HideInInspector]
        public KeyCode RecordDataKey;
        /// <summary>
        /// boolean for whether the MLS node connecting is a classification algorithm
        /// </summary>
        private bool MLSClassification = false;
        /// <summary>
        /// Lists of input ports 
        /// </summary>
        private List<NodePort> inputPortList;
        private List<NodePort> targetPortList;
        /// <summary>
        /// boolean to hold whether the user removed a target value/input port connection when there were trained examples
        /// </summary>
        private bool badRemove = false;
        private string portName = "";
        /// <summary>
        /// boolean for whether the node should be showing a warning 
        /// </summary>
        public bool showWarning;

        public Event Train;

        #endregion

        #region XNode Messages

        

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            if (this.DisconnectIfNotType<TrainingExamplesNode, IFeatureIML>(from, to))
            {
                return;
            }
            bool trainingExamplesRecorded = false;
            // if you have started training and this is not the system trying to stop you changing the connection and it is an output port 
            if (TrainingExamplesVector.Count > 0 || TrainingSeriesCollection.Count > 0 && !badRemove)
            {
                from.Disconnect(to);
                return;
            }
                
            // DIRTY CODE
            if (to.fieldName == "InputFeatures")
            {
                // DIRTY CODE
                // Since there is a bug in DTW rapidlib, we don't allow features of different size connected
                // If we are a training examples node for DTW...
                if (this is SeriesTrainingExamplesNode)
                {
                    // If we have any features...
                    if (!Lists.IsNullOrEmpty(ref InputFeatures))
                    {
                        // We check that the new feature connected is not of a different size
                        int newFeatureSize = (from.node as IFeatureIML).FeatureValues.Values.Length;
                        // Get the first feature connected as the template size
                        int knownFeatureSize = (InputFeatures[0] as IFeatureIML).FeatureValues.Values.Length;
                        // Disconnect if sizes don't match
                        if (newFeatureSize != knownFeatureSize)
                        {
                            from.Disconnect(to);
                            return;
                        }
                    }
                    
                 }
                inputPortList = this.GetInputPort("InputFeatures").GetConnections();
                IMLEventDispatcher.InputConfigChange?.Invoke();


            }
            //if connected to target values 
            if(to.fieldName == "TargetValues")
            {
                if (trainingExamplesRecorded)
                {
                    from.Disconnect(to);
                    return;
                }

                // Go through MLS systems connected if any MLS are classification or DTW set MLSclassification to true
                foreach (MLSystem MLS in MLSystemNodesConnected)
                {
                    if (MLS.Model.TypeOfModel == RapidlibModel.ModelType.DTW || MLS.Model.TypeOfModel == RapidlibModel.ModelType.kNN)
                    {
                        MLSClassification = true;
                    }
                }
                // if there is a mls classification connected and there is one or more target values break connection otherwise update list of connected wtarget values 
                if (MLSClassification && TargetValues.Count >= 1 )
                {
                    from.Disconnect(to);
                    return;
                }
                else
                {
                    UpdateTargetValueInput();
                }
                MLSClassification = false;
                targetPortList = this.GetInputPort("TargetValues").GetConnections();
                UpdateTargetValuesConfig();
                IMLEventDispatcher.LabelsConfigChange?.Invoke();
                return;
            }

            
        }

        public override void OnRemoveConnection(NodePort port)
        {
            if(TrainingExamplesVector.Count  > 0 || TrainingSeriesCollection.Count > 0)
            {
                badRemove = true;
                portName = port.fieldName;
            } else
            {
                inputPortList = this.GetInputPort("InputFeatures").GetConnections();
                targetPortList = this.GetInputPort("TargetValues").GetConnections();
            }
            base.OnRemoveConnection(port);
            UpdateTargetValueInput();
            UpdateTargetValuesConfig();
            
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
            var MLController = graph as IMLGraph;
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
        public override void Initialize()
        {
            
            SetDataCollection();
            // Make sure internal feature lists are initialized
            if (m_DesiredInputsConfig == null)
                m_DesiredInputsConfig = new List<IMLSpecifications.InputsEnum>();

            if (m_DesiredOutputsConfig == null)
                m_DesiredOutputsConfig = new List<IMLSpecifications.OutputsEnum>();

            if (m_DesiredOutputFeatures == null)
                m_DesiredOutputFeatures = new List<IMLBaseDataType>();

            if (m_DesiredInputFeatures == null)
                m_DesiredInputFeatures = new List<IMLBaseDataType>();

            if (Lists.IsNullOrEmpty(ref TrainingExamplesVector))
                TrainingExamplesVector = new List<IMLTrainingExample>();

            if (Lists.IsNullOrEmpty(ref TrainingSeriesCollection))
                TrainingSeriesCollection = new List<IMLTrainingSeries>();

            if (MLSystemNodesConnected == null)
                MLSystemNodesConnected = new List<MLSystem>();

            // set target value input list 
            UpdateTargetValueInput();
            //set up target values configuration list 
            UpdateTargetValuesConfig();
            // Load training data from disk
            LoadDataFromDisk();
            
            inputPortList = this.GetInputPort("InputFeatures").GetConnections();
            targetPortList = this.GetInputPort("TargetValues").GetConnections();
            CheckWarning();
            UpdateInputConfigList();
            
        }
        /// <summary>
        /// Starts/Stops collecting examples when called
        /// </summary>
        public void ToggleCollectExamples()
        {
            ToggleCollectingDataprotected();
        }

        /// <summary>
        /// Public method to start collecting data 
        /// Called by start recording 
        /// </summary>
        /// <returns>boolean on whether we have started</returns>
        public bool StartCollecting()
        {
            // if the node is set up with input features and it is not currently collecting data start collecting data
            if(InputFeatures.Count > 0 && TargetValues.Count > 0 && !m_CollectingData)
            {
                StartCollectingData();
                return true;
            } else
            {
                return false;
            }

        }
        /// <summary>
        /// Public method for stopping recording data 
        /// </summary>
        /// <returns></returns>
        public bool StopCollecting()
        {
            // if it is collecting data stop collecting data 
            if (m_CollectingData)
            {
                StopCollectingData();
                return true;
            } else
            {
                return false;
            }
        }


        // check move to single training examples node
        /// <summary>
        /// Adds a single training example to the node's list
        /// </summary>
        public bool AddSingleTrainingExample()
        {
            if(!m_CollectingData && InputFeatures.Count > 0 && TargetValues.Count > 0)
            {
                AddTrainingExampleprotected();
                CheckWarning();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Update logic per node
        /// </summary>
        public void UpdateLogic()
        {
            
            if (numberInComponentList == -1)
            {
                SetArrayNumber();
            }
            //Debug.Log(IMLConfigurationNodesConnected.Count);
            // Handle Input
            KeyboardInput();
            if (badRemove)
                StopDisconnection();
            // Keep input config list updated
            UpdateInputConfigList();

            //Update target values 
            UpdateDesiredOutputFeatures();
            //Update input values
            UpdateDesiredInputFeatures();
            //Update 
            UpdateInputConfigList();

            // Run examples logic in case we need to start/stop collecting examples
            CollectExamplesLogic();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ToggleCollectExamples();
            }
            
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
            //set false to show warning about training 
            CheckWarning();
            
            // I AM GOING TO KEEP THE DESIRED OUTPUTS UNCHANGED FOR THE MOMENT

            // Clear list of desired outputs (which is also fed to the training examples vector when adding a single training example)
            //DesiredOutputFeatures.Clear();

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
            // implement in subclass 
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

            } else
            {
                InputFeatures = new List<Node>();
            }
            
        }
        /// <summary>
        /// Updates the list of Feature Extractprs / Data type nodes connected to the Target Value port 
        /// </summary>
        protected void UpdateTargetValueInput()
        {
            // if the list exists get nodes connected 
            if (this.GetInputNodesConnected("TargetValues") != null)
            {
                TargetValues = this.GetInputNodesConnected("TargetValues");
            }
            else
            {
                TargetValues = new List<Node>();
            }
        }

        /// <summary>
        /// Updates the configuration list of target values 
        /// </summary>
        protected void UpdateTargetValuesConfig()
        {
            m_DesiredOutputsConfig.Clear();
            if (m_DesiredOutputsConfig.Count != TargetValues.Count)
            {
                for (int i = 0; i < TargetValues.Count; i++)
                {
                    // Cast the node checking if implements the feature interface (it is a featureExtractor)
                    IFeatureIML targetValue = TargetValues[i] as IFeatureIML;

                    // If it is a feature extractor...
                    if (targetValue != null)
                    {
                        // We add the feature to the desired inputs config
                        m_DesiredOutputsConfig.Add((IMLSpecifications.OutputsEnum)targetValue.FeatureValues.DataType);
                    }
                }
            }
            
        }



        /// <summary>
        /// Logic to collect examples. Needs to be called in an Update loop
        /// </summary>
        protected virtual void CollectExamplesLogic()
        {
            if (m_CollectingData)
            {
                if (Application.isPlaying && m_TimeToStopCapture > 0 && Time.time >= m_TimeToStopCapture)
                {
                    m_CollectingData = false;
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
            for (int i = 0; i < TargetValues.Count; i++)
            {
                newExample.AddOutputExample((TargetValues[i] as IFeatureIML).FeatureValues);
            }
            
            // Add the training example to the vector
            TrainingExamplesVector.Add(newExample);
            SaveDataToDisk();

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
            if (m_CollectingData)
            {
                StopCollectingData();
            }
            else
            {
                StartCollectingData();
            }
            CheckWarning();

        }

        /// <summary>
        /// Sets the collecting data flag to true and configures the class to collect data
        /// </summary>
        protected void StartCollectingData()
        {

            m_CollectingData = true;
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
        protected void StopCollectingData()
        {
            m_CollectingData = false;

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
            //implemented in subclass
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

        public void SetArrayNumber()
        {
            //Set array number
            IMLComponent MLComponent = this.FindController();
            List<TrainingExamplesNode> tNodes = new List<TrainingExamplesNode>();
            if (MLComponent.TrainingExamplesNodesList != null)
                tNodes = MLComponent.TrainingExamplesNodesList;
            FindComponentListNumber<TrainingExamplesNode>(tNodes, MLComponent);
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


        protected void UpdateDesiredOutputFeatures()
        {
            DesiredOutputFeatures.Clear();
            for(int i = 0; i<TargetValues.Count; i++)
            {
                IFeatureIML targetValue = TargetValues[i] as IFeatureIML;
                DesiredOutputFeatures.Add(targetValue.FeatureValues);
               
            }

        }

        protected void UpdateDesiredInputFeatures()
        {
            m_DesiredInputFeatures.Clear();
            for (int i = 0; i < InputFeatures.Count; i++)
            {
                IFeatureIML inputValue = InputFeatures[i] as IFeatureIML;
                m_DesiredInputFeatures.Add(inputValue.FeatureValues);

            }

        }

        protected void StopDisconnection()
        {
            if (portName == "TargetValues")
            {
                NodePort portConnect = GetInputPort(portName);
                for (int i = 0; i < targetPortList.Count; i++)
                {
                    portConnect.Connect(targetPortList[i]);
                    targetPortList[i].Connect(portConnect);
                }
            }

            if (portName == "InputFeatures")
            {
                NodePort portConnect = GetInputPort(portName);
                for (int i = 0; i < inputPortList.Count; i++)
                {
                    portConnect.Connect(inputPortList[i]);
                    inputPortList[i].Connect(portConnect);
                    
                }
            }

            badRemove = false;

        }
        /// <summary>
        /// Checks whether warning should be shown
        /// </summary>
        protected void CheckWarning()
        {
            if (TrainingExamplesVector.Count == 0 && TrainingSeriesCollection.Count == 0)
            {
                showWarning = false;

            } else
            {
                showWarning = true;
            }
        }

        #endregion

    }
}