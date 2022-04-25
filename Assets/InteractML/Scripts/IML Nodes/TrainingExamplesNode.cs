using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;
using ReusableMethods;
using System;

namespace InteractML
{
    /// <summary>
    /// Holds the information and list of a training examples node
    /// </summary>
    [NodeWidth(300)]
    public class TrainingExamplesNode : IMLNode, IDataSetIML
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
        /// Record One Button Input Customiser Boolean
        /// </summary>
        [Input]
        public bool RecordOneInputBool;

        /// <summary>
        /// Toggle Record Button Input Customiser Boolean
        /// </summary>
        [Input]
        public bool ToggleRecordingInputBool;

        /// <summary>
        /// Delete Last Button Input Customiser Boolean
        /// </summary>
        [Input]
        public bool DeleteLastInputBool;

        /// <summary>
        /// Delet All Button Input Customiser Boolean
        /// </summary>
        [Input]
        public bool DeleteAllInputBool;


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
        public List<IMLTrainingExample> TrainingExamplesVector { get { return m_TrainingExamplesVector; } }
        protected List<IMLTrainingExample> m_TrainingExamplesVector;

        /// <summary>
        /// The output training examples series collection of this node
        /// </summary>
        [SerializeField, HideInInspector]
        public List<IMLTrainingSeries> TrainingSeriesCollection { get { return m_TrainingSeriesCollection; } }
        protected List<IMLTrainingSeries> m_TrainingSeriesCollection;

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
        protected int m_TotalNumberOfTrainingExamples { get { return m_TrainingExamplesVector.Count; } }
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
        /// boolean for whether the node should be showing a warning 
        /// </summary>
        public bool showWarning;

        private int lastNoOfRecordings = 0; // this needs a comment (what, why)
        private bool deleteLast = false; // this needs a comment (what, why)

        public bool canCollect; // this needs a comment (what, why)

        public int listNo; // this needs a comment (what, why)

        /// <summary>
        /// Specifies a subfolder to save the training examples
        /// </summary>
        public string SubFolderDataPath;

        #endregion

        #region XNode Messages



        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            if (m_TrainingExamplesVector == null) m_TrainingExamplesVector = new List<IMLTrainingExample>();
            if (m_TrainingSeriesCollection == null) m_TrainingSeriesCollection = new List<IMLTrainingSeries>();

            // If there is a connection to any of the button ports...
            if (to.fieldName == "RecordOneInputBoolPort")
            {
                // check incoming node type and port data type is accepted by input port
                System.Type[] portTypesAccept = new System.Type[] { typeof(bool) };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IMLNode) };
                this.DisconnectPortAndNodeIfANYTypes(from, to, portTypesAccept, nodeTypesAccept);
                // Exit any further checks to avoid unwanted disconnections
                return;

            }
            if (to.fieldName == "ToggleRecordingInputBoolPort")
            {
                // check incoming node type and port data type is accepted by input port
                System.Type[] portTypesAccept = new System.Type[] { typeof(bool) };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IMLNode) };
                this.DisconnectPortAndNodeIfANYTypes(from, to, portTypesAccept, nodeTypesAccept);
                // Exit any further checks to avoid unwanted disconnections
                return;

            }
            if (to.fieldName == "DeleteAllExamplesBoolPort")
            {
                // check incoming node type and port data type is accepted by input port
                System.Type[] portTypesAccept = new System.Type[] { typeof(bool) };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IMLNode) };
                this.DisconnectPortAndNodeIfANYTypes(from, to, portTypesAccept, nodeTypesAccept);
                // Exit any further checks to avoid unwanted disconnections
                return;
            }
            if (to.fieldName == "SubFolderDataPathStringPort")
            {
                // check incoming node type and port data type is accepted by input port
                System.Type[] portTypesAccept = new System.Type[] { typeof(string) };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IMLNode) };
                this.DisconnectPortAndNodeIfANYTypes(from, to, portTypesAccept, nodeTypesAccept);
                // Exit any further checks to avoid unwanted disconnections
                return;
            }



            // If there is a connection in one of the features ports...

            // bool for tracking whether there are training examples recorded
            bool trainingExamplesExist = false;
            // if you are not connecting a ifeatureiml node then disconnect
            if (to.fieldName == "MovementData")
            {
                // check incoming node type and port data type is accepted by input port
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IFeatureIML) }; // discriminate based on type of node connected
                this.DisconnectFROMPortIsNotTypes(from, to, nodeTypesAccept);                

            }
            //if (this.DisconnectIfNotType<TrainingExamplesNode, IFeatureIML>(from, to) && from.fieldName != "TrainingExamplesNodeToOutput")
            //{
            //    from.Disconnect(to);
            //    return;
            //}
            
            // if there are training examples set the bool to true 
            if (m_TrainingExamplesVector.Count > 0 || m_TrainingSeriesCollection.Count > 0)
            {
                trainingExamplesExist = true;
            }
            // if you are changing the input features or target values check if the connected MLSystemNodes have more then one training example and don't allow if they do
            if(to.fieldName == "InputFeatures" || to.fieldName == "TargetValues")
            {
                foreach (MLSystem MLS in MLSystemNodesConnected)
                {
                    if (MLS.IMLTrainingExamplesNodes.Count > 1)
                    {
                        from.Disconnect(to);
                    }
                }
            }

            // if you are connecting to input port 
            if (to.fieldName == "InputFeatures")
            {
                // if there are training examples
                if (trainingExamplesExist)
                {
                    // int for number of input features currently connected
                    int noOfInputFeatures = 0;
                    // if there are features set tp that number if there are none leave as 0 
                    if (!Lists.IsNullOrEmpty(ref InputFeatures))
                    {
                        noOfInputFeatures = InputFeatures.Count;
                    }

                    // get the data type of the feature being connected
                    IFeatureIML node = from.node as IFeatureIML;
                    IMLSpecifications.InputsEnum datatype = (IMLSpecifications.InputsEnum)node.FeatureValues.DataType;
                    
                    if (DesiredInputsConfig.Count == noOfInputFeatures||datatype != DesiredInputsConfig[noOfInputFeatures])
                    {
                        from.Disconnect(to);
                        return;
                    }
                    
                }
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

                UpdateInputConfigList();
                UpdateDesiredInputFeatures();
                IMLEventDispatcher.InputConfigChangeCallback?.Invoke();


            }
            //if connected to target values 
            if(to.fieldName == "TargetValues")
            {
                if (trainingExamplesExist)
                {
                    int noOfTargetValues = 0;
                    if (!Lists.IsNullOrEmpty(ref TargetValues))
                    {
                        noOfTargetValues = TargetValues.Count;
                    }
                    IFeatureIML node = from.node as IFeatureIML;
                    IMLSpecifications.OutputsEnum datatype = (IMLSpecifications.OutputsEnum)node.FeatureValues.DataType;
                    if (datatype != DesiredOutputsConfig[noOfTargetValues])
                    {
                        from.Disconnect(to);
                        return;
                    }
                    
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
                //targetPortList = this.GetInputPort("TargetValues").GetConnections();
                UpdateTargetValuesConfig();
                UpdateDesiredOutputFeatures();
                IMLEventDispatcher.LabelsConfigChangeCallback?.Invoke();
                
            }

            CheckSetUp();

        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);

            if (m_TrainingExamplesVector == null) m_TrainingExamplesVector = new List<IMLTrainingExample>();
            if (m_TrainingSeriesCollection == null) m_TrainingSeriesCollection = new List<IMLTrainingSeries>();

            if (m_TrainingExamplesVector.Count == 0 && m_TrainingSeriesCollection.Count == 0)
            {
                UpdateTargetValuesConfig();
                UpdateInputConfigList();
                UpdateTargetValueInput();

                UpdateDesiredInputFeatures();
                UpdateDesiredOutputFeatures();
            } else
            {
                InputFeatures = this.GetInputNodesConnected("InputFeatures");
                TargetValues = this.GetInputNodesConnected("TargetValues");
            }

            CheckSetUp();
        }

        /// <summary>
        /// Checks if the training examples node has the right set up of features to collect training examples
        /// </summary>
        private void CheckSetUp()
        {
            canCollect = true;
            if (Lists.IsNullOrEmpty(ref InputFeatures) || Lists.IsNullOrEmpty(ref TargetValues))
            {
                canCollect = false;
                return;
            }
            if (m_TrainingExamplesVector.Count > 0 || m_TrainingSeriesCollection.Count > 0)
            {
                if (DesiredInputsConfig.Count != InputFeatures.Count || DesiredOutputsConfig.Count != TargetValues.Count)
                {
                    // DIRTY CODE
                    // Update desired input features in case is an unwanted mismatch (init event not triggering due to an unhandled exception)
                    UpdateInputConfigList();
                    UpdateTargetValuesConfig();
                    UpdateDesiredInputFeatures();
                    UpdateDesiredOutputFeatures();
                    // If the mismatch is still present...
                    if (DesiredInputsConfig.Count != InputFeatures.Count || DesiredOutputsConfig.Count != TargetValues.Count)
                        canCollect = false;
                    // If not, allow collection of data
                    else
                        canCollect = true;
                }
            }
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

            if (((IMLGraph)graph).IsGraphRunning && m_TrainingExamplesVector != null && m_TrainingSeriesCollection!= null)
            {
                // Attempt to load data if needed
                if (m_TrainingExamplesVector.Count == 0 && m_TrainingSeriesCollection.Count == 0)
                {
                    LoadDataFromDisk();
                }

            }


        }
        //check 
        protected void OnDestroy()
        {
            UnsubscribeDelegates();
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
            SubscribeDelegates();
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

            if (Lists.IsNullOrEmpty(ref m_TrainingExamplesVector))
            {
                m_TrainingExamplesVector = new List<IMLTrainingExample>();
            }
                

            if (Lists.IsNullOrEmpty(ref m_TrainingSeriesCollection))
                m_TrainingSeriesCollection = new List<IMLTrainingSeries>();

            if (MLSystemNodesConnected == null)
                MLSystemNodesConnected = new List<MLSystem>();

            // set target value input list 
            UpdateTargetValueInput();
            //set up target values configuration list 
            UpdateTargetValuesConfig();
            // Load training data from disk
            LoadDataFromDisk();
            CheckWarning();
            if (m_TrainingSeriesCollection.Count == 0 && m_TrainingExamplesVector.Count == 0)
           {
                UpdateInputConfigList();
                UpdateTargetValuesConfig();
           }
            
            UpdateDesiredInputFeatures();
            UpdateDesiredOutputFeatures();
            CheckSetUp();

            // Add all required dynamic ports
            // RecordOneInputBoolPort           
            this.GetOrCreateDynamicPort("RecordOneInputBoolPort", typeof(bool), NodePort.IO.Input, ConnectionType.Override, TypeConstraint.Inherited);
            // ToggleRecordingInputBool
            this.GetOrCreateDynamicPort("ToggleRecordingInputBoolPort", typeof(bool), NodePort.IO.Input, ConnectionType.Override, TypeConstraint.Inherited);
            // DeleteAllExamplesBoolPort
            this.GetOrCreateDynamicPort("DeleteAllExamplesBoolPort", typeof(bool), NodePort.IO.Input, ConnectionType.Override, TypeConstraint.Inherited);
            // SubFolderDataPathStringPort
            this.GetOrCreateDynamicPort("SubFolderDataPathStringPort", typeof(string), NodePort.IO.Input, ConnectionType.Override, TypeConstraint.Inherited);

            // Pull inputs from bool event nodeports
            if (GetInputValue<bool>("RecordOneInputBoolPort"))
                IMLEventDispatcher.RecordOneCallback(this.id);
            if (GetInputValue<bool>("ToggleRecordingInputBoolPort"))
                IMLEventDispatcher.ToggleRecordCallback(this.id);
            if (GetInputValue<bool>("DeleteAllExamplesBoolPort"))
                IMLEventDispatcher.DeleteAllExamplesInNodeCallback(this.id);
            // Pull input from string subfolderDataPath nodeport
            SubFolderDataPath = GetInputValue<string>("SubFolderDataPathStringPort");

            // Attempt to load data
            if (m_TrainingExamplesVector.Count == 0 && m_TrainingSeriesCollection.Count == 0)
            {
                LoadDataFromDisk();
            }

        }


        /// <summary>
        /// Starts/Stops collecting examples when called
        /// </summary>
        public bool ToggleCollectExamples()
        {
            return ToggleCollectingDataprotected();
        }

        /// <summary>
        /// Public method to start collecting data 
        /// Called by start recording 
        /// </summary>
        /// <returns>boolean on whether we have started</returns>
        public bool StartCollecting()
        {
            // Refresh the canCollect flag inside CheckSetUp to ensure that we can collect if needed
            CheckSetUp();
            // if the node is set up with input features and it is not currently collecting data start collecting data
            if (InputFeatures.Count > 0 && TargetValues.Count > 0 && !m_CollectingData && canCollect)
            {                
                StartCollectingData();
                // Callbacks from event dispatcher
                IMLEventDispatcher.StartRecordCallback?.Invoke(this.id);
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
                // Callbacks from event dispatcher
                IMLEventDispatcher.StopRecordCallback?.Invoke(this.id);
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
            if(!m_CollectingData && InputFeatures.Count > 0 && TargetValues.Count > 0 && canCollect)
            {
                AddTrainingExampleprotected();
                CheckWarning();
                SaveDataToDisk();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Update logic per node
        /// </summary>
        public virtual void UpdateLogic()
        {
            // Pull inputs from bool event nodeports
            if (GetInputValue<bool>("RecordOneInputBoolPort"))
                IMLEventDispatcher.RecordOneCallback(this.id); 
            if (GetInputValue<bool>("ToggleRecordingInputBoolPort"))
                IMLEventDispatcher.ToggleRecordCallback(this.id);
            if (GetInputValue<bool>("DeleteAllExamplesBoolPort"))
                IMLEventDispatcher.DeleteAllExamplesInNodeCallback(this.id);
            // Pull input from string subfolderDataPath nodeport
            SubFolderDataPath = GetInputValue<string>("SubFolderDataPathStringPort");


            // loading if empty moved to init and onvalidate
            //// Attempt to load data
            //if (m_TrainingExamplesVector.Count == 0 && m_TrainingSeriesCollection.Count == 0)
            //{
            //    LoadDataFromDisk();
            //}

            // Run examples logic in case we need to start/stop collecting examples
            CollectExamplesLogic();

        }

        /// <summary>
        /// Clears all the training examples stored in the node
        /// </summary>
        public virtual void ClearTrainingExamples(bool deleteFromDisk = true)
        {
            // Account for mode
            switch (ModeOfCollection)
            {
                case CollectionMode.SingleExample:
                    // Clear examples in node
                    m_TrainingExamplesVector.Clear();
                    break;
                case CollectionMode.Series:
                    // Clear series in node
                    m_TrainingSeriesCollection.Clear();
                    break;
                default:
                    break;
            }
            
            if (deleteFromDisk)
                // clear data on disk
                SaveDataToDisk();
            //set false to show warning about training 
            CheckWarning();
            //Update MLsystems connected with new number of training data 
            UpdateConnectMLSystems();

            UpdateTargetValuesConfig();
            UpdateInputConfigList();

            // I AM GOING TO KEEP THE DESIRED OUTPUTS UNCHANGED FOR THE MOMENT

            // Clear list of desired outputs (which is also fed to the training examples vector when adding a single training example)
            //DesiredOutputFeatures.Clear();
            CheckSetUp();
        }

        public void DeleteLastRecording()
        {
            if (deleteLast)
            {

            }
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

            // Make sure that the list is initialised
            if (m_DesiredInputsConfig == null)
                m_DesiredInputsConfig = new List<IMLSpecifications.InputsEnum>();

            // Adjust the desired inputs list based on nodes connected
            m_DesiredInputsConfig.Clear();
            // if there are inputfestures connected 
            if (InputFeatures != null)
            {
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
        /// Updates the configuration list of desired inputs and outputs from the internal training examples list
        /// </summary>
        /// <param name="updateDesiredFeatures">Update list of desired features as well? </param>
        public void UpdateDesiredInputOutputConfigFromDataVector(bool updateDesiredFeatures = false)
        {
            // Make sure that the list is initialised
            // Expected configuration
            if (m_DesiredInputsConfig == null)
                m_DesiredInputsConfig = new List<IMLSpecifications.InputsEnum>();
            if (m_DesiredOutputsConfig == null)
                m_DesiredOutputsConfig = new List<IMLSpecifications.OutputsEnum>();
            // Expected features
            if (updateDesiredFeatures && m_DesiredInputFeatures == null)
                m_DesiredInputFeatures = new List<IMLBaseDataType>();
            if (updateDesiredFeatures && m_DesiredOutputFeatures == null)
                m_DesiredOutputFeatures = new List<IMLBaseDataType>();


            // Adjust the desired inputs list based on nodes connected
            // Expected Configs
            m_DesiredInputsConfig.Clear();
            m_DesiredOutputsConfig.Clear();
            // Expected Features
            if (updateDesiredFeatures) 
            {
                m_DesiredInputFeatures.Clear();
                m_DesiredOutputFeatures.Clear();
            }
            // if there are training examples loaded...
            if ((m_TrainingSeriesCollection != null && m_TrainingSeriesCollection.Count > 0) 
                || (m_TrainingExamplesVector != null && m_TrainingExamplesVector.Count > 0))
            {
                // Training Series
                if (this is SeriesTrainingExamplesNode)
                {                    
                    // Check for null
                    if ((m_TrainingSeriesCollection.Count > 0) 
                        && (m_TrainingSeriesCollection[0].Series == null || m_TrainingSeriesCollection[0].LabelSeries == null) 
                        && (m_TrainingSeriesCollection[0].Series[0] == null || m_TrainingSeriesCollection[0].LabelSeries == null))
                    {
                        NodeDebug.LogWarning("Null Reference in Series! Can't configure dataset for model.", this, debugToConsole: true);
                        return;
                    }
                    // Check for empty series
                    if (m_TrainingSeriesCollection[0].Series.Count == 0)
                    {
                        NodeDebug.LogWarning("Series is empty! Can't configure dataset for model.", this, debugToConsole: true);
                        return;
                    }
                    // Inputs
                    for (int i = 0; i < m_TrainingSeriesCollection[0].Series[0].Count; i++)
                    {
                        Debug.Log($"i is {i}");
                        // Check for null
                        if (m_TrainingSeriesCollection[0].Series[0][i] == null || m_TrainingSeriesCollection[0].Series[0][i].InputData == null)
                        {
                            NodeDebug.LogWarning("Null Reference in Input for Series! Can't configure dataset for model.", this, debugToConsole: true);
                            return;
                        }

                        // Expected config
                        m_DesiredInputsConfig.Add((IMLSpecifications.InputsEnum)m_TrainingSeriesCollection[0].Series[0][i].InputData.DataType);
                        // Expected features
                        if (updateDesiredFeatures)
                            m_DesiredInputFeatures.Add(m_TrainingSeriesCollection[0].Series[0][i].InputData);
                    }
                    //inputFeaturesInSeries[j][k].InputData.Values.Length
                    
                    // Outputs
                    //Debug.Log(TrainingSeriesCollection[0].LabelSeries);
                    List<IMLBaseDataType> labels = IMLDataSerialization.ParseJSONToIMLFeature(m_TrainingSeriesCollection[0].LabelSeries);
                    //Debug.Log(labels.Count);
                    for (int i = 0; i < labels.Count; i++)
                    {
                        // Check for null
                        if (labels[i] == null)
                        {
                            NodeDebug.LogWarning("Null Reference in Series Label! Can't configure dataset for model.", this, debugToConsole: true);
                            return;
                        }
                        // Expected config
                        m_DesiredOutputsConfig.Add((IMLSpecifications.OutputsEnum)labels[i].DataType);
                        // Expected features
                        if (updateDesiredFeatures)
                            m_DesiredOutputFeatures.Add(labels[i]);
                    }
                }
                // Single training examples
                else if (m_TrainingExamplesVector.Count > 0)
                {
                    // Check for null
                    if (m_TrainingExamplesVector[0] == null || m_TrainingExamplesVector[0].Inputs == null || m_TrainingExamplesVector[0].Outputs == null)
                    {
                        NodeDebug.LogWarning("Null Reference in Training Examples! Can't configure dataset for model.", this, debugToConsole: true);
                        return;
                    }
                    // Inputs
                    var recordedInputFeatures = m_TrainingExamplesVector[0].Inputs;
                    for (int i = 0; i < m_TrainingExamplesVector[0].Inputs.Count; i++)
                    {
                        // Check for null
                        if (recordedInputFeatures[i].InputData == null || recordedInputFeatures[i].InputData.Values == null)
                        {
                            NodeDebug.LogWarning("Null Reference in Training Examples Input! Can't configure dataset for model.", this, debugToConsole: true);
                            return;
                        }
                        // Expected config
                        m_DesiredInputsConfig.Add((IMLSpecifications.InputsEnum)recordedInputFeatures[i].InputData.DataType);
                        // Expected features
                        if (updateDesiredFeatures)
                            m_DesiredInputFeatures.Add(recordedInputFeatures[i].InputData);
                    }
                    // Outputs
                    var outputFeatures = m_TrainingExamplesVector[0].Outputs;
                    for (int i = 0; i < outputFeatures.Count; i++)
                    {
                        // Check for null
                        if (outputFeatures[i] == null || outputFeatures[i].OutputData == null)
                        {
                            NodeDebug.LogWarning("Null Reference in Training Examples Output! Can't configure dataset for model.", this, debugToConsole: true);
                            return;
                        }
                        // Expected config
                        m_DesiredOutputsConfig.Add((IMLSpecifications.OutputsEnum)outputFeatures[i].OutputData.DataType);
                        // Expected features
                        if (updateDesiredFeatures)
                            m_DesiredOutputFeatures.Add(outputFeatures[i].OutputData);
                    }
                }
                
            }
            else
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
        protected void CollectExamplesLogic()
        {
            if (m_CollectingData)
            {
                if (Application.isPlaying && m_TimeToStopCapture > 0 && Time.time >= m_TimeToStopCapture)
                {
                    //Debug.Log("collecting false");
                    m_CollectingData = false;
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
                            {
                                AddInputsToSeries(InputFeatures,
                                                IMLDataSerialization.ParseIMLFeatureToJSON(DesiredOutputFeatures),
                                                ref m_SingleSeries);
                            }
                            break;
                        default:
                            break;
                    }

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
            m_TrainingExamplesVector.Add(newExample);
            
            // Commented this as it is slowing down released builds
            //SaveDataToDisk();
            //update connected MLSystem node with no of training examples
            UpdateConnectMLSystems();
            
            //Debug.Log("TrainingExamples Saved!");
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
        protected bool ToggleCollectingDataprotected()
        {
            bool success = false;
            if (m_CollectingData)
            {
                success = StopCollecting();
            }
            else
            {
                success = StartCollecting();
            }
            CheckWarning();

            return success;

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
                m_TrainingSeriesCollection.Add(new IMLTrainingSeries(m_SingleSeries));
                m_SingleSeries.ClearSerie();
                //update the number of training examples connected in mlsystem
                UpdateConnectMLSystems();

            }

            // Save data to disk
            SaveDataToDisk();
        }


        public virtual void SaveDataToDisk()
        {
            //implemented in subclass
        }

        

        private void LoadDataFromDisk()
        {
            if (this is SeriesTrainingExamplesNode)
            {
                // Load IML Series Collection from disk
                var auxTrainingSeriesCollection = IMLDataSerialization.LoadTrainingSeriesCollectionFromDisk(GetJSONFileName());
                if (!Lists.IsNullOrEmpty(ref auxTrainingSeriesCollection))
                {
                    m_TrainingSeriesCollection = auxTrainingSeriesCollection;
                }
            }
            else
            {
                //Load training data from disk
                var auxTrainingExamplesVector = IMLDataSerialization.LoadTrainingSetFromDisk(GetJSONFileName());
                if (!Lists.IsNullOrEmpty(ref auxTrainingExamplesVector))
                {
                    m_TrainingExamplesVector = auxTrainingExamplesVector;
                    //Debug.Log("Training Examples Vector loaded!");
                }
            }
            //Debug.Log(TrainingExamplesVector.Count);

            if (m_TrainingExamplesVector.Count > 0 || m_TrainingSeriesCollection.Count > 0)
            {
                UpdateDesiredInputOutputConfigFromDataVector();
            }
        }

        /// <summary>
        /// Returns the file name we want for the regular training examples list in JSON format, both for read and write
        /// </summary>
        /// <returns></returns>
        public virtual string GetJSONFileName ()
        {
            if(this.graph != null)
            {
                string fileName = "TrainingExamplesNode" + this.id;
                
                // If we have a subfolder specified for the data...
                if (!String.IsNullOrEmpty(SubFolderDataPath))
                    fileName = String.Concat(SubFolderDataPath, "/", fileName);
                
                return fileName;
            }
            return null;
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
            if (DesiredOutputFeatures == null)
                m_DesiredOutputFeatures = new List<IMLBaseDataType>();
            else
                m_DesiredOutputFeatures.Clear();

            if (TargetValues == null) return;
            for(int i = 0; i<TargetValues.Count; i++)
            {
                IFeatureIML targetValue = TargetValues[i] as IFeatureIML;
                m_DesiredOutputFeatures.Add(targetValue.FeatureValues);
               
            }

        }

        protected void UpdateDesiredInputFeatures()
        {
            if (m_DesiredInputFeatures == null)
                m_DesiredInputFeatures = new List<IMLBaseDataType>();
            else
                m_DesiredInputFeatures.Clear();
            
            if (InputFeatures == null) return;
            for (int i = 0; i < InputFeatures.Count; i++)
            {
                IFeatureIML inputValue = InputFeatures[i] as IFeatureIML;
                m_DesiredInputFeatures.Add(inputValue.FeatureValues);

            }

        }

        protected void StopDisconnection()
        {
           /* if (portName == "InputFeatures")
            {
                NodePort portConnect = GetInputPort(portName);
                for (int i = 0; i < inputPortList.Count; i++)
                {
                    portConnect.Connect(inputPortList[i]);
                    inputPortList[i].Connect(portConnect);

                }
            }

            if (portName == "TargetValues")
            {
                NodePort portConnect = GetInputPort(portName);
                for (int i = 0; i < targetPortList.Count; i++)
                {
                    portConnect.Connect(targetPortList[i]);
                    targetPortList[i].Connect(portConnect);
                }
            }*/

            

            //badRemove = false;

        }
        /// <summary>
        /// Checks whether warning should be shown
        /// </summary>
        protected void CheckWarning()
        {
            if (m_TrainingExamplesVector.Count == 0 && m_TrainingSeriesCollection.Count == 0)
            {
                showWarning = false;

            } else
            {
                showWarning = true;
            }
        }

        private void UpdateConnectMLSystems()
        {
            foreach (MLSystem MLN in MLSystemNodesConnected)
            {
                if(MLN != null)
                    MLN.UpdateTotalNumberTrainingExamples();
            }
        }

        private void SubscribeDelegates()
        {
            IMLEventDispatcher.listenText += ListenText;
        }

        private void UnsubscribeDelegates()
        {
            IMLEventDispatcher.listenText -= ListenText;
        }

        private bool ListenText(string nodeid)
        {
            bool listening;
            if (this.id == nodeid)
                IMLEventDispatcher.getText += GetStatus;
            else
                IMLEventDispatcher.getText -= GetStatus;
            return true;
        }
        public string GetStatus(string nodeid)
        {
            
            if (nodeid == this.id)
            {
                string status = "";
                int i = 0;
                foreach (IFeatureIML node in TargetValues)
                {
                    status += "Label: " + i;
                    foreach (float value in node.FeatureValues.Values)
                    {
                        status += value + ", ";
                    }
                    i++;
                }
                if (CollectingData)
                    status += "Recording \n";

                status += "Examples: ";
                if (ModeOfCollection == CollectionMode.SingleExample)
                {

                   status += TotalNumberOfTrainingExamples.ToString();
                } else
                {
                    status += m_TrainingSeriesCollection.Count.ToString();
                }
                return status;
            }
            return "here";
        }

        #endregion

    }
}