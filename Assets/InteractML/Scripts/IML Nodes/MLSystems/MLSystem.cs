using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;
using ReusableMethods;
using System;
using System.IO;
using System.Threading.Tasks;

namespace InteractML
{
    [NodeWidth(420)]
    public class MLSystem : IMLNode
    {

        #region Variables

        /// <summary>
        /// The input list of TrainingExamplesNodes 
        /// </summary>
        [Input]
        public List<TrainingExamplesNode> IMLTrainingExamplesNodes;
        /// <summary>
        /// Input list of live features for running model 
        /// </summary>
        [Input]
        public List<Node> InputFeatures;

        /// <summary>
        /// The list of predicted outputs
        /// </summary>
        [SerializeField]
        public List<IMLBaseDataType> PredictedOutput;

        //[Input]
        //public List<Node> TriggerTrain;

        /// <summary>
        /// Toggle Train Button Input Customiser Boolean
        /// </summary>
        [Input]
        public bool ToggleTrainInputBool;

        /// <summary>
        /// Toggle Run Button Input Customiser Boolean
        /// </summary>
        [Input]
        public bool ToggleRunInputBool;

        /// <summary>
        /// Predicted Rapidlib Output 
        /// </summary>
        [HideInInspector]
        public double[] PredictedRapidlibOutput;
        // check with carlos what this is doing 
        [HideInInspector]
        public double[] DelayedPredictedOutput;

        /// <summary>
        /// Total updated number of training examples connected to this IML Configuration Node
        /// </summary>
        protected int m_TotalNumTrainingDataConnected;
        public int TotalNumTrainingDataConnected { get { return m_TotalNumTrainingDataConnected; } }

        [SerializeField, HideInInspector]
        protected int m_NumExamplesTrainedOn;
        /// <summary>
        /// Total number of training examples used to train the current model
        /// </summary>
        public int NumExamplesTrainedOn { get { return m_NumExamplesTrainedOn; } }

        /// <summary>
        /// List of expected inputs
        /// </summary>
        [SerializeField, HideInInspector]
        protected List<IMLSpecifications.InputsEnum> m_ExpectedInputList;
        public List<IMLSpecifications.InputsEnum> ExpectedInputList { get { return m_ExpectedInputList; } }

        /// <summary>
        /// List of expected outputs
        /// </summary>
        [SerializeField, HideInInspector]
        protected List<IMLSpecifications.OutputsEnum> m_ExpectedOutputList;
        public List<IMLSpecifications.OutputsEnum> ExpectedOutputList { get { return m_ExpectedOutputList; } }

        /// <summary>
        /// Dynamic list of ourput ports configured by traning examples set up 
        /// </summary>
        protected List<XNode.NodePort> m_DynamicOutputPorts;

        /// <summary>
        /// Bool have the outputs changed 
        /// </summary>
        public bool OutputPortsChanged { get; set; }
        
        /// <summary>
        /// The training example type of this model
        /// </summary>
        [SerializeField]
        protected IMLSpecifications.TrainingSetType m_trainingType;
        [HideInInspector]
        public IMLSpecifications.TrainingSetType TrainingType { get => m_trainingType; }

        /// <summary>
        /// The learning type of this model
        /// </summary>
        [SerializeField]
        protected IMLSpecifications.LearningType m_LearningType;
        [HideInInspector]
        public IMLSpecifications.LearningType LearningType { get => m_LearningType; }


        /// <summary>
        /// Keyboard flag to control node - will be legacy soon 
        /// </summary>
        public bool EnableKeyboardControl = true;
        [HideInInspector]
        public KeyCode TrainingKey = KeyCode.T;
        [HideInInspector]
        public KeyCode RunningKey;

        /// <summary>
        /// The current status of the model
        /// </summary>
        protected IMLSpecifications.ModelStatus m_ModelStatus { get { return m_Model != null ? m_Model.ModelStatus : IMLSpecifications.ModelStatus.Untrained; } }
        public IMLSpecifications.ModelStatus ModelStatus { get => m_ModelStatus; }
        /// <summary>
        /// Public booleans to display the status of the mdoel 
        /// </summary>
        protected bool m_Running;
        public bool Running { get { return m_Running; } }
        public bool Training { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Training); } }
        public bool Trained { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Trained); } }
        public bool Untrained { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Untrained); } }

        /// <summary>
        /// Reference to the rapidlib model this node is holding
        /// </summary>
        protected RapidlibModel m_Model;
        /// <summary>
        /// Public reference to the rapidlib model this node is holding
        /// </summary>
        public RapidlibModel Model { get => m_Model; }

        /// <summary>
        /// The list of training examples that we will pass to rapidlib in the correct format
        /// </summary>
        protected List<RapidlibTrainingExample> m_RapidlibTrainingExamples;
        /// <summary>
        /// protected list of rapidlib training series collection (for dtw)
        /// </summary>
        protected List<RapidlibTrainingSerie> m_RapidlibTrainingSeriesCollection;

        
        /// <summary>
        /// Training series for MLSystem training in series 
        /// </summary>
        protected IMLTrainingSeries m_RunningSeries;

        /// <summary>
        /// How many different classes is the model trained on?
        /// </summary>
        [SerializeField, HideInInspector]
        protected int m_TotalNumUniqueClasses;
        public int TotalNumUniqueClasses { get { return m_TotalNumUniqueClasses; } }
        /// <summary>
        /// All the unique training classes for this model
        /// </summary>
        [SerializeField, HideInInspector]
        protected List<IMLTrainingExample> m_TotalUniqueTrainingClasses;
        public List<IMLTrainingExample> TotalUniqueTrainingClasses { get => m_TotalUniqueTrainingClasses; }

        /// <summary>
        /// Vector used to compute the realtime predictions in rapidlib based on the training data
        /// </summary>
        protected double[] m_RapidlibInputVector;
        /// <summary>
        /// Vector used to output the realtime predictions from rapidlib
        /// </summary>
        protected double[] m_RapidlibOutputVector;
        //to be deleted when new events system totally tested
        //private bool m_NodeConnectionChanged;

        /// <summary>
        /// Checks if label configuration changed - check with carlos if this is still needed 
        /// </summary>
        protected int m_LastKnownRapidlibOutputVectorSize;

        /// <summary>
        /// Flag that controls if the iml model should be trained when entering/leaving playmode
        /// </summary>
        [HideInInspector]
        public bool TrainOnPlaymodeChange = false;

        /// <summary>
        /// Flag that controls if the iml model should run when the game awakes 
        /// </summary>
        [HideInInspector]
        public bool RunOnAwake;

        /* NODEPORT NAMES */
        protected string m_TrainingExamplesNodeportName;
        protected string m_LiveFeaturesNodeportName;

        /* ERROR FLAGS */
        protected bool m_ErrorWrongInputTrainingExamplesPort;
        protected bool m_WrongNumberOfTargetValues = false;
        protected bool m_TrainingExamplesConflict = false;

        /// <summary>
        /// boolean whether the training nodes connected match the number of live data input nodes connected - to be deleted when new inputs implemented 
        /// </summary>
        public bool matchLiveDataInputs = true;

        /// <summary>
        /// boolean whether the vector of inputs matches the training nodes conencted 
        /// </summary>
        public bool matchVectorLength = true;

        /// <summary>
        /// string for the current warning tooltip displayed
        /// </summary>
        public string warning;

        /// <summary>
        /// boolean for whether there is an error in the set up of the model 
        /// </summary>
        public bool error = false;

        public bool trainOnLoad = true;

        protected bool  isKNN;

        /// <summary>
        /// Is the model training/running asynchronously?
        /// </summary>
        public bool UseAsync { get { return m_UseAsync; } set { m_UseAsync = value; } }
        /// <summary>
        /// Change this flag to use or not async methods (training, running models)
        /// </summary>
        private bool m_UseAsync = true;

        #region Testing Variables

        /// <summary>
        /// Are we using a testing state after running the model?
        /// </summary>
        public bool UseTestingState { get => m_UseTestingState; set => m_UseTestingState = value; }
        /// <summary>
        /// Change this flag to use or not the testing state after the running state
        /// </summary>
        private bool m_UseTestingState = true;
        /// <summary>
        /// Is the node collecting testing data for this model?
        /// </summary>
        protected bool m_Testing;
        /// <summary>
        /// Is the node collecting testing data for this model?
        /// </summary>
        public bool Testing { get { return m_Testing; } }
        /// <summary>
        /// Data used for testing (Classification/Regression only)
        /// </summary>
        public List<List<IMLTrainingExample>> TestingData { get { return m_TestingData; } }
        /// <summary>
        /// Data used for testing (Classification/Regression only)
        /// </summary>
        protected List<List<IMLTrainingExample>> m_TestingData;
        /// <summary>
        /// How many testing classes have been collected?
        /// </summary>
        public bool[] TestingClassesCollected { get => m_TestingClassesCollected; }
        private bool[] m_TestingClassesCollected;
        /// <summary>
        /// Which testing class is being collected?
        /// </summary>
        public int CurrentTestingClassCollected { get => m_CurrentTestingClassCollected; }
        private int m_CurrentTestingClassCollected;
        /// <summary>
        /// Are all testing classes collected?
        /// </summary>
        public bool AllTestingClassesCollected { get => m_CurrentTestingClassCollected >= m_TestingClassesCollected.Length; }
        /// <summary>
        /// Collecting testing data?
        /// </summary>
        private bool m_CollectingTestingData;
        /// <summary>
        /// Collecting testing data?
        /// </summary>
        public bool CollectingTestingData { get => m_CollectingTestingData; }
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


        #endregion

        #endregion

        #region XNode Messages



        // Return the correct value of an output port when requested
        public override object GetValue(XNode.NodePort port)
        {
            // If it is a dynamic output...
            if (port.IsDynamic)
            {
                // Only run when both lists are not null
                if (m_DynamicOutputPorts != null && PredictedOutput != null)
                {
                    // Only run if both lists have been properly populated
                    if (m_DynamicOutputPorts.Count == PredictedOutput.Count)
                    {
                        // Make sure we return the right value in the dynamic outputs
                        for (int i = 0; i < m_DynamicOutputPorts.Count; i++)
                        {
                            // If we are requested the value of a dynamic port...
                            if (port.fieldName == m_DynamicOutputPorts[i].fieldName)
                            {
                                // Since the dynamic port list length is the same as the predicted output, we get the corresponding predicted output
                                switch (PredictedOutput[i].DataType)
                                {
                                    case IMLSpecifications.DataTypes.Float:
                                        return (PredictedOutput[i] as IMLFloat).GetValue();
                                    case IMLSpecifications.DataTypes.Integer:
                                        return (PredictedOutput[i] as IMLInteger).GetValue();
                                    case IMLSpecifications.DataTypes.Vector2:
                                        return (PredictedOutput[i] as IMLVector2).GetValues();
                                    case IMLSpecifications.DataTypes.Vector3:
                                        return (PredictedOutput[i] as IMLVector3).GetValues();
                                    case IMLSpecifications.DataTypes.Vector4:
                                        return (PredictedOutput[i] as IMLVector4).GetValues();
                                    case IMLSpecifications.DataTypes.Array:
                                        return (PredictedOutput[i] as IMLArray).GetValues();
                                    default:
                                        break;
                                }
                            }
                        }

                    }

                }

            }
            // If we reach here, it is not a dynamic output. Return entire node (legacy output)
            return this;
        }
        /// <summary>
        /// Called when a connection is made from any of the ports 
        /// Checks whether the port that is connected to i the correct port 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public override void OnCreateConnection(XNode.NodePort from, XNode.NodePort to)
        {
            Debug.Log(from.GetType().ToString());
            base.OnCreateConnection(from, to);

            // If there is a connection to any of the button ports...
            if (to.fieldName == "ToggleTrainInputBoolPort")
            {
                // check incoming node type and port data type is accepted by input port
                System.Type[] portTypesAccept = new System.Type[] { typeof(bool) };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IMLNode) };
                this.DisconnectPortAndNodeIfANYTypes(from, to, portTypesAccept, nodeTypesAccept);
                // Exit any further checks to avoid unwanted disconnections
                return;

            }
            if (to.fieldName == "ToggleRunInputBoolPort")
            {
                // check incoming node type and port data type is accepted by input port
                System.Type[] portTypesAccept = new System.Type[] { typeof(bool) };
                System.Type[] nodeTypesAccept = new System.Type[] { typeof(IMLNode) };
                this.DisconnectPortAndNodeIfANYTypes(from, to, portTypesAccept, nodeTypesAccept);
                // Exit any further checks to avoid unwanted disconnections
                return;

            }

            m_WrongNumberOfTargetValues = false;
            m_TrainingExamplesConflict = false;

            // to be deleted when new events completely tested 
            //m_NodeConnectionChanged = true;

            // Evaluate the nodeport for training examples
            CheckTrainingExamplesConnections(from, to, m_TrainingExamplesNodeportName);

            // Evaluate nodeport for real-time features
            CheckInputFeaturesConnections(from, to, m_LiveFeaturesNodeportName);

            // We call logic for adapting arrays and inputs/outputs for ML models
            // Create input feature vector for realtime rapidlib predictions
            CreateRapidlibInputVector();

            // Create rapidlib predicted output vector
            CreateRapidLibOutputVector();
            // if adding a training examples node connection add MLS system to that training nodes list of connected MLS systems
            if (from.fieldName == "TrainingExamplesNodeToOutput" && !m_WrongNumberOfTargetValues)
            {
                IMLTrainingExamplesNodes = GetInputValues<TrainingExamplesNode>("IMLTrainingExamplesNodes").ToList();
                TrainingExamplesNode temp = from.node as TrainingExamplesNode;
                if (!temp.MLSystemNodesConnected.Contains(this))
                    temp.MLSystemNodesConnected.Add(this);
            }
            //if you connect a training examples or change live input 
            if (from.fieldName == "TrainingExamplesNodeToOutput" || from.fieldName == "m_Out" || from.fieldName == "LiveDataOut")
            {
                IMLEventDispatcher.ModelSetUpChangeCallback?.Invoke();
            }

        }

        public override void OnRemoveConnection(XNode.NodePort port)
        {
            base.OnRemoveConnection(port);

            // to be deleted when new events completely tested
            //m_NodeConnectionChanged = true;

            //maybe remove here 
            //UpdateOutputConfigList();

            // Check that lists are not null
            if (m_ExpectedOutputList == null)
                m_ExpectedOutputList = new List<IMLSpecifications.OutputsEnum>();
            if (m_DynamicOutputPorts == null)
                m_DynamicOutputPorts = new List<XNode.NodePort>();

               //look at how this happens in new events based system 
              // IF expected outputs don't match with dynamic output ports, a training examples node was disconnected
              if (m_ExpectedOutputList.Count != m_DynamicOutputPorts.Count)
              {
                  // Refresh all dynamic output ports to the right size
                  UpdateDynamicOutputPorts(IMLTrainingExamplesNodes, m_ExpectedOutputList, ref m_DynamicOutputPorts);
              }

            // We call logic for adapting arrays and inputs/outputs for ML models
            // Create input feature vector for realtime rapidlib predictions
            CreateRapidlibInputVector();

            // Create rapidlib predicted output vector
            CreateRapidLibOutputVector();

            // if we are diconnecting this from a training set remove this from the list of connected models
            if (port.fieldName == "IMLTrainingExamplesNodes")
            {
                TrainingNodeConnectedRemoved();
            } 

            if(port.fieldName == "InputFeatures")
            {
                InputFeatures = this.GetInputNodesConnected("InputFeatures");
            }
            CheckLiveDataInputMatchesTrainingExamples();
            CheckLengthInputsVector();
            UIErrors();

        }

        #endregion

        #region Unity Messages

        public void OnValidate()
        {
            // Checks that the rapidlib model is instanced (only if model is null)
            if (m_Model == null && (this.graph as IMLGraph).IsGraphRunning)
            {
                // Attempt to load model
                LoadModelFromDisk(reCreateModel: true);
            }

            // We call logic for adapting arrays and inputs/outputs for ML models
            // Create input feature vector for realtime rapidlib predictions
            CreateRapidlibInputVector();

            // Create rapidlib predicted output vector
            CreateRapidLibOutputVector();

            // Set error flags to false
            SetErrorFlags(false);
        }
        /// <summary>
        /// Method that subscribes relevant methods to events in the event dispatcher
        /// </summary>
        private void SubscribeDelegates(){

            // Methods for when the inputs of the training set are changed
            IMLEventDispatcher.InputConfigChangeCallback += OnDataInChanged;
            IMLEventDispatcher.InputConfigChangeCallback += UIErrors;

            // Methods for when the label of the training set is changed
            IMLEventDispatcher.LabelsConfigChangeCallback += OnLabelChanged;
            IMLEventDispatcher.LabelsConfigChangeCallback += UIErrors;

            // Methods for when the whole set up of the training set is changed 
            IMLEventDispatcher.ModelSetUpChangeCallback += OnDataInChanged;
            IMLEventDispatcher.ModelSetUpChangeCallback += OnLabelChanged;
            IMLEventDispatcher.ModelSetUpChangeCallback += UpdateTotalNumberTrainingExamples;
            IMLEventDispatcher.ModelSetUpChangeCallback += UIErrors;


            IMLEventDispatcher.LoadModelsCallback += LoadOrTrain;

            IMLEventDispatcher.listenText += ListenText;
        }
        /// <summary>
        /// Method that subscribes relevant methods to events in the event dispatcher
        /// </summary>
        private void UnsubscribeDelegates()
        {
            // Methods for when the inputs of the training set are changed
            IMLEventDispatcher.InputConfigChangeCallback -= OnDataInChanged;
            IMLEventDispatcher.InputConfigChangeCallback -= UIErrors;

            // Methods for when the label of the training set is changed
            IMLEventDispatcher.LabelsConfigChangeCallback -= OnLabelChanged;
            IMLEventDispatcher.LabelsConfigChangeCallback -= UIErrors;

            // Methods for when the whole set up of the training set is changed 
            IMLEventDispatcher.ModelSetUpChangeCallback -= OnDataInChanged;
            IMLEventDispatcher.ModelSetUpChangeCallback -= OnLabelChanged;
            IMLEventDispatcher.ModelSetUpChangeCallback -= UpdateTotalNumberTrainingExamples;
            IMLEventDispatcher.ModelSetUpChangeCallback -= UIErrors;

            IMLEventDispatcher.LoadModelsCallback -= LoadOrTrain;

            IMLEventDispatcher.listenText -= ListenText;
        }

        /// <summary>
        /// When MLSystem object is destroyed 
        /// </summary>
        public void OnDestroy()
        {
            // Unscibscribe from all events
            UnsubscribeDelegates();

            //remove this node from trainingexamplesnode list of MLSystem nodes when deleted
            foreach (TrainingExamplesNode tNode in IMLTrainingExamplesNodes) {
                tNode.MLSystemNodesConnected.Remove(this);
            }

            // Remove reference of this node in the IMLComponent controlling this node (if any)
            var MLController = graph as IMLGraph;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.DeleteMLSystemNode(this);
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize method called when a new MLSystem is created and when the graph this system is part of is enabled (when loading project/ switching edit play mode)
        /// </summary>
        public override void Initialize()
        {
            // Scubscribe to all events 
            SubscribeDelegates();

            // Set training type of the machine learning model
            SetTrainingType();

            // Set training type of the machine learning model
            SetLearningType();

            // Make sure that the scene component is aware of this node
            var imlGraph = graph as IMLGraph;
            if (imlGraph != null && imlGraph.SceneComponent != null && imlGraph.SceneComponent.MLSystemNodeList != null)
            {
                if (!imlGraph.SceneComponent.MLSystemNodeList.Contains(this))
                    imlGraph.SceneComponent.MLSystemNodeList.Add(this);
            }

            // need to clarify what this is doing 
            if (m_trainingType == IMLSpecifications.TrainingSetType.SeriesTrainingExamples)
                TrainOnPlaymodeChange = true;

            // Make sure the model is initialised properly
            if (m_Model == null && (this.graph as IMLGraph).IsGraphRunning)
                m_Model = InstantiateRapidlibModel();

            // Init lists
            if (Lists.IsNullOrEmpty(ref IMLTrainingExamplesNodes))
                IMLTrainingExamplesNodes = new List<TrainingExamplesNode>();

            if (Lists.IsNullOrEmpty(ref PredictedOutput))
                PredictedOutput = new List<IMLBaseDataType>();

            if (Lists.IsNullOrEmpty(ref m_ExpectedInputList))
                m_ExpectedInputList = new List<IMLSpecifications.InputsEnum>();

            if (Lists.IsNullOrEmpty(ref m_ExpectedOutputList))
                m_ExpectedOutputList = new List<IMLSpecifications.OutputsEnum>();

            if (Lists.IsNullOrEmpty(ref m_RapidlibTrainingExamples))
                m_RapidlibTrainingExamples = new List<RapidlibTrainingExample>();

            if (Lists.IsNullOrEmpty(ref m_DynamicOutputPorts))
                m_DynamicOutputPorts = new List<XNode.NodePort>();
            // to be deleted when new eevent system totally tested 
            //m_NodeConnectionChanged = false;

            m_LastKnownRapidlibOutputVectorSize = 0;

            // We call logic for adapting arrays and inputs/outputs for ML models
            // Create input feature vector for realtime rapidlib predictions
            CreateRapidlibInputVector();

            // Create rapidlib predicted output vector
            CreateRapidLibOutputVector();

            // Set number of training examples 
            UpdateTotalNumberTrainingExamples();

            //Add this node to list of MLsystem nodes in all training nodes attached 
            foreach (TrainingExamplesNode node in IMLTrainingExamplesNodes)
            {
                if (node != null && node.MLSystemNodesConnected != null && !node.MLSystemNodesConnected.Contains(this))
                    node.MLSystemNodesConnected.Add(this);
            }
            // Specify the names for the nodeports
            m_TrainingExamplesNodeportName = "IMLTrainingExamplesNodes";
            m_LiveFeaturesNodeportName = "InputFeatures";

            // Add all required dynamic ports
            // ToggleTrainInputBoolPort           
            this.GetOrCreateDynamicPort("ToggleTrainInputBoolPort", typeof(bool), NodePort.IO.Input);
            // ToggleRunInputBoolPort
            this.GetOrCreateDynamicPort("ToggleRunInputBoolPort", typeof(bool), NodePort.IO.Input);

            // Evaluate data in
            OnDataInChanged();
        }
        #region Subclass Instatiation Methods
        /// <summary>
        /// Instantiates a rapidlibmodel - this should be done is subclass 
        /// </summary>
        public virtual RapidlibModel InstantiateRapidlibModel()
        {
            RapidlibModel model;
            model = new RapidlibModel();
            Debug.LogError("No machine model set - implement InstantiateRapidlibModel in class");
            return model;
        }

        /// <summary>
        /// Sets training set type for the model - called in initialize - implemented in subclass 
        /// </summary>
        protected virtual void SetTrainingType()
        {
            m_trainingType = IMLSpecifications.TrainingSetType.SingleTrainingExamples;
            Debug.LogError("Needs to be implemented in MLSystem subclass");
        }
        #endregion

        /// <summary>
        /// Trains the ML model
        /// </summary>
        /// <returns></returns>
        public bool TrainModel(bool useAsync = false)
        {
            bool isTrained = false;
            //if the MLS is not running, training and the model is not null and the total number of training data is bigger than 0
            if (!Running && !Training && !Testing && Model != null && TotalNumTrainingDataConnected > 0)
            {
                
                //come back to this
                RunningLogic();
                // if there are no training examples in connected training nodes do not train 
                if (m_TotalNumTrainingDataConnected == 0)
                {
                    Debug.Log("no training examples");
                }
                else
                {
                    if (m_UseAsync || useAsync)
                    {
                        TrainModelPrivateAsync();
                        isTrained = true;
                    }
                    else
                    {
                        isTrained = TrainModelPrivate();
                    }
                }
                
            }
            return isTrained;
        }

        /// <summary>
        /// Trains the ML model
        /// </summary>
        /// <returns></returns>
        private bool TrainModelPrivate() {

            bool trained = false;
            switch (m_trainingType)
            {
                case IMLSpecifications.TrainingSetType.SingleTrainingExamples:
                    trained = SingleExamplesTrain();
                    break;
                case IMLSpecifications.TrainingSetType.SeriesTrainingExamples:
                    trained = SeriesExampleTrain();
                    break;
                default:
                    break;
            }
            return trained;
        }

        /// <summary>
        /// Trains the ML model
        /// </summary>
        /// <returns></returns>
        private void TrainModelPrivateAsync() 
        {

            switch (m_trainingType)
            {
                case IMLSpecifications.TrainingSetType.SingleTrainingExamples:
                    SingleExamplesTrainAsync();
                    break;
                case IMLSpecifications.TrainingSetType.SeriesTrainingExamples:
                    SeriesExampleTrainAsync();
                    break;
                default:
                    break;
            }
        }

        private bool SingleExamplesTrain()
        {
             bool isTrained = false;
            
             // Transform the IML Training Examples into a format suitable for Rapidlib
             m_RapidlibTrainingExamples = TransformIMLDataToRapidlib(IMLTrainingExamplesNodes, out m_NumExamplesTrainedOn);

             // Trains rapidlib with the examples added
             isTrained = m_Model.Train(m_RapidlibTrainingExamples);

            Debug.Log($"Model trained with {m_TotalNumUniqueClasses} unique classes");

            return isTrained;
        }

        private bool SeriesExampleTrain()
        {
            bool isTrained = false;
            if (m_RapidlibTrainingSeriesCollection == null)
                m_RapidlibTrainingSeriesCollection = new List<RapidlibTrainingSerie>();
            m_RapidlibTrainingSeriesCollection = TransformIMLSeriesToRapidlib(IMLTrainingExamplesNodes, out m_NumExamplesTrainedOn);
            isTrained = m_Model.Train(m_RapidlibTrainingSeriesCollection);
            return isTrained;
        }

        private void SingleExamplesTrainAsync()
        {
            // Transform the IML Training Examples into a format suitable for Rapidlib
            m_RapidlibTrainingExamples = TransformIMLDataToRapidlib(IMLTrainingExamplesNodes, out m_NumExamplesTrainedOn);

             // Trains rapidlib with the examples added
             m_Model.TrainAsync(m_RapidlibTrainingExamples);
        }

        private void SeriesExampleTrainAsync()
        {
            if (m_RapidlibTrainingSeriesCollection == null)
                m_RapidlibTrainingSeriesCollection = new List<RapidlibTrainingSerie>();
            m_RapidlibTrainingSeriesCollection = TransformIMLSeriesToRapidlib(IMLTrainingExamplesNodes, out m_NumExamplesTrainedOn);
            m_Model.TrainAsync(m_RapidlibTrainingSeriesCollection);
        }

        protected int CheckLengthTrainingVector(TrainingExamplesNode tNode)
        {
            int trainingVector = 0;
            // check null
            if (tNode == null)
                return trainingVector;

            // Attempt to trigger an update of desired input features on connected training examples node in case it hasn't been updated yet
            if (tNode.DesiredInputFeatures == null || tNode.DesiredInputFeatures.Count == 0) tNode.UpdateDesiredInputOutputConfigFromDataVector(updateDesiredFeatures: true);

            // First attempt to calculate training vector from recorded examples and desired input features (in case the tNode doesn't have any connected live features and it is just a data container)
            if (tNode.TrainingExamplesVector != null && tNode.TrainingExamplesVector.Count > 0 && tNode.DesiredInputFeatures != null && tNode.DesiredInputFeatures.Count > 0)
            {
                foreach (var feature in tNode.DesiredInputFeatures)
                {
                    trainingVector += feature.Values.Length; 
                }
            }
            // If there are no recorded examples, calculate from connected inputs
            else if (tNode.InputFeatures != null)
            {
                foreach (Node node in tNode.InputFeatures)
                {
                    IFeatureIML feature = node as IFeatureIML;
                    trainingVector += feature.FeatureValues.Values.Length;
                }

            }
            return trainingVector;
        }


        /// <summary>
        /// checks the length of input vector and checks if the same as the trained data set to be changed when new input system done
        /// </summary>
        protected void CheckLengthInputsVector()
        {
            IMLTrainingExamplesNodes = GetInputValues<TrainingExamplesNode>("IMLTrainingExamplesNodes").ToList();
            int lengthtrainingvector = 0;
            if (IMLTrainingExamplesNodes.Count == 0)
            {
                return;
            }
            TrainingExamplesNode tNode = IMLTrainingExamplesNodes[0];
            lengthtrainingvector = CheckLengthTrainingVector(tNode);

            int vectorSize = 0;
            if (InputFeatures != null)
            {
                for (int i = 0; i < InputFeatures.Count; i++)
                {
                    var inputFeature = InputFeatures[i] as IFeatureIML;
                    if (inputFeature != null)
                    {
                        if (inputFeature.FeatureValues != null)
                        {
                            if (inputFeature.FeatureValues.Values != null)
                            {
                                vectorSize += inputFeature.FeatureValues.Values.Length;
                            }
                        }
                    }
                }
                //Debug.Log(vectorSize);
                //Debug.Log(lengthtrainingvector);
                if (vectorSize == lengthtrainingvector)
                {
                    matchVectorLength = true;
                }
                else
                {
                    Debug.LogWarning("mismatch in live inputs to trained data");
                    matchVectorLength = false;
                }
            } else {
                matchVectorLength = false;

            }
            
        }

        public bool ToggleRunning()
        {
            bool success = false;
            // Is the testing state allowed 
            if (m_UseTestingState)
            {
                if (!m_Running && !Testing)
                {
                    success = StartRunning();
                }
                else if (m_Running && !Testing)
                {
                    StartTesting();
                    success = true;
                }
                //// Move to next testing class if not all testing data collected
                //else if (m_Running && Testing && !AllTestingClassesCollected)
                //{
                //    NextTestingClass();
                //}
                //// We can now stop testing and running
                //else if (m_Running && Testing && AllTestingClassesCollected)
                //{
                //    StopTesting();
                //    success = StopRunning();
                //}
            }
            // Default InteractML behaviour start/stop running
            else
            {
                if (!m_Running)
                {
                    success = StartRunning();
                }                
                else if (m_Running)
                {
                    success = StopRunning();
                }
            }
            return success; 
        }
        /// <summary>
        /// Sets running boolean to true 
        /// </summary>
        /// <returns>boolean on whether the model has started running</returns>
        public bool StartRunning()
        {
            if (!m_Running)
            {
                UpdateOutputFormat();
                // If the system is not running and it is trained, it is not traing and the vectors match 
                if (!m_Running && Trained && !Training && matchLiveDataInputs && matchVectorLength)
                {
                    // Set flag to true if running inputs/outputs are not null and the model is trained or it is a series 
                    if (((m_RapidlibInputVector != null && m_RapidlibOutputVector != null) || m_trainingType == IMLSpecifications.TrainingSetType.SeriesTrainingExamples))
                    {
                        m_Running = true;
                        return true;
                    }
                    else
                    {
                        Debug.LogError("Rapidlib vectors for realtime predictions are null!");
                        return false;
                    }
                }
            }
            
            return false; 
        }
        /// <summary>
        /// Stop running
        /// </summary>
        /// <returns>boolean on whether the model has stopped running</returns>
        public bool StopRunning()
        {
            if (m_Running)
            {
                // If we are on DTW, we run the iteration at the end of the data collection period
                if (m_trainingType == IMLSpecifications.TrainingSetType.SeriesTrainingExamples)
                {
                    string predictionDTW = RunModelDTW(m_RunningSeries);
                    // We clear running series for next run
                    m_RunningSeries.ClearSerie();
                    // We parse json into iml output
                    PredictedOutput = IMLDataSerialization.ParseJSONToIMLFeature(predictionDTW);
                    Debug.Log(predictionDTW.Length);
                    if (PredictedOutput != null && PredictedOutput.Count > 0)
                    {
                        Debug.Log("Predicted output is: " + PredictedOutput[0].Values[0]);
                    }
                }
                // Set flag to false
                m_Running = false;
                // Stop model
                m_Model.StopRunning();
                // For the moment, we consider the end of a running a model the end of a model steering iteration
                IMLEventDispatcher.ModelSteeringIterationFinished?.Invoke(this.id);
                return true;
            }
            return false; 
        }

        /// <summary>
        /// Returns the file name we want for the JSON, both for read and write
        /// </summary>
        /// <returns></returns>
        public string GetJSONFileName()
        {
            string fileName = "MLSystem" + this.id;
            return fileName;
        }

        /// <summary>
        /// Resets the rapidlibModel instance
        /// </summary>
        public void ResetModel()
        {
            // Take care of the RapidlibModel reference to this node     
            m_Model = InstantiateRapidlibModel();

            // Reset numExamplesTrainedOn
            m_NumExamplesTrainedOn = 0;

            // We reset the running flag
            m_Running = false;

        }

        /// <summary>
        /// Saves current model to disk (dataPath specified in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveModelToDisk()
        {
            m_Model.SaveModelToDisk(this.graph.name + "_IMLConfiguration" + this.id);
        }
        
        /// <summary>
        /// Saves current model to disk (dataPath specified in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        private void DeleteModelFromDisk()
        {
            IMLDataSerialization.DeleteRapidlibModelFromDisk(this.graph.name + "_IMLConfiguration" + this.id);
        }

        /// <summary>
        /// Loads the current model from disk (dataPath specified in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public virtual bool LoadModelFromDisk(bool reCreateModel = false)
        {
            bool success = false;
            Debug.Log("here load");
            // Make sure to re-instantiate the model if null or flag is true
            if (m_Model == null || reCreateModel || m_Model.ModelAddress == (IntPtr)0)
                m_Model = InstantiateRapidlibModel();
            success = m_Model.LoadModelFromDisk(this.graph.name + "_IMLConfiguration" + this.id, reCreateModel);
            // We update the node learning type to match the one from the loaded model
            if (success)
            {
                switch (m_Model.TypeOfModel)
                {
                    case RapidlibModel.ModelType.kNN:
                        // Configure inputs and outputs
                        PredictedOutput = new List<IMLBaseDataType>(m_Model.GetNumExpectedOutputs());
                        // Reload unique outputs
                        ReloadUniqueOutputs(IMLTrainingExamplesNodes, ref m_TotalUniqueTrainingClasses, ref m_TotalNumUniqueClasses);
                        // TO DO
                        // Still left to configure inputs
                        // Still left to configure the type of the inputs and outputs
                        break;
                    case RapidlibModel.ModelType.NeuralNetwork:
                        // Configure inputs and outputs
                        PredictedOutput = new List<IMLBaseDataType>(m_Model.GetNumExpectedOutputs());
                        // Reload unique outputs
                        ReloadUniqueOutputs(IMLTrainingExamplesNodes, ref m_TotalUniqueTrainingClasses, ref m_TotalNumUniqueClasses);
                        // TO DO
                        // Reload unique outputs for DTW
                        // Still left to configure inputs
                        // Still left to configure the type of the inputs and outputs
                        break;
                    case RapidlibModel.ModelType.DTW:
                        // DTW model will need to retrain!
                        Debug.Log("DTW RETRAINING WHEN LOADING MODEL NOT IMPLEMENTED YET!");
                        break;
                    case RapidlibModel.ModelType.None:
                        break;
                    default:
                        break;
                }
            }
            

            return success;
        }

        #endregion

        #region protected Methods
        // to be deleted if current implementation of composition works 
        protected virtual void SetLearningType()
        {
            Debug.LogError("Needs to be implemented in MLSystem subclass");
        }

       

        protected void KeyboardInput()
        {
            if (EnableKeyboardControl)
            {
                if (Input.GetKeyDown(TrainingKey))
                {
                    TrainModel();
                }

                if (Input.GetKeyDown(RunningKey))
                {
                    Debug.Log("running1");
                    ToggleRunning();
                }
            }
        }

        public void TransformPredictedOuputToIMLTypes(double[] rapidlibResults, ref List<IMLBaseDataType> IMLOutputsList)
        {
            int pointerRawOutputVector = 0;
            // Transform the raw vector output into list of iml types
            // Go through each output feature (preconfigured previously)
            foreach (var outputFeature in IMLOutputsList)
            {
                // Add corresponding values to that feature and move the pointerForward as many spaces as length the feature has
                for (int i = 0; i < outputFeature.Values.Length; i++)
                {
                    // DEBUG CODE
                    //if (Running && outputFeature.Values.Length > 0)
                    //{
                    //    Debug.Log("Output " + i + " : " + outputFeature.Values[i]);
                    //}
                    //
                    /* Debug.Log("i " + i);
                     Debug.Log("pointer " + (i + pointerRawOutputVector));
                     Debug.Log("predicted " + PredictedRapidlibOutput.Length);*/
                    if (i + pointerRawOutputVector >= PredictedRapidlibOutput.Length)
                    {
                        Debug.LogError("The predicted rapidlib output vector is too small when transforming to interactml types!");
                        break;
                    }
                    outputFeature.Values[i] = (float)PredictedRapidlibOutput[i + pointerRawOutputVector];

                }
                pointerRawOutputVector += outputFeature.Values.Length;
            }

        }



        /// <summary>
        /// Runs the model and updates the predicted output from the rapidlib predictions by 
        /// </summary>
        protected double[] RunModel()
        {
            // Only do calculations if running flag is true (useful for UI)
            if (m_Running)
            {
                // Only allow running if the model exists and it is trained or running
                if (m_Model != null && (m_ModelStatus == IMLSpecifications.ModelStatus.Trained || m_ModelStatus == IMLSpecifications.ModelStatus.Running))
                {
                    // Update input vector with latest features
                    UpdateInputVector();

                    // Update the delayed predicted output (seemed to fix some bug with the UI? MIGHT NOT NEED ANYMORE)
                    if (DelayedPredictedOutput == null || DelayedPredictedOutput.Length != PredictedRapidlibOutput.Length)
                    {
                        DelayedPredictedOutput = new double[PredictedRapidlibOutput.Length];
                    }
                    PredictedRapidlibOutput.CopyTo(DelayedPredictedOutput, 0);

                    // Run model
                    m_Model.Run(m_RapidlibInputVector, ref m_RapidlibOutputVector);
                }
            }

            // Return the rapidlib output vector if it is not null
            return m_RapidlibOutputVector != null ? m_RapidlibOutputVector : new double[0];
        }

        /// <summary>
        /// Runs an iteration of DTW
        /// </summary>
        /// <param name="seriesToRun"></param>
        /// <returns></returns>
        protected string RunModelDTW(IMLTrainingSeries seriesToRun)
        {
            string result = "";
            // If the seriesToRun is null or empty we don't allow to run DTW
            if (seriesToRun.Series == null || seriesToRun.Series.Count == 0)
            {
                Debug.LogError("Null or Empty serie used in DTW, aborting calculations!");
                return result;
            }
            // Only allow running if the model exists and it is trained or running
            if (m_Model != null && (m_ModelStatus == IMLSpecifications.ModelStatus.Trained || m_ModelStatus == IMLSpecifications.ModelStatus.Running))
            {
                // Run dtw
                result = m_Model.Run(new RapidlibTrainingSerie(seriesToRun.GetSeriesFeatures(), seriesToRun.LabelSeries));
            }
            return result;
        }

        /// <summary>
        /// Collects features frame by frame to the running series for DTW
        /// </summary>
        /// <param name="inputFeatures"></param>
        /// <param name="runningSeries"></param>
        protected void CollectFeaturesInRunningSeries(List<Node> inputFeatures, ref IMLTrainingSeries runningSeries)
        {
            // Only allow collection is model is marked as 'running' (althoug we will run the model only when toggleRunning is called again)
            if (m_Running)
            {
                // We don't run frame by frame, but instead we collect input features to run at the end 
                List<IMLInput> featuresToSeries = new List<IMLInput>(inputFeatures.Count);
                foreach (var item in inputFeatures)
                {
                    if (item is IFeatureIML feature)
                    {
                        featuresToSeries.Add(new IMLInput(feature.FeatureValues));
                    }
                }
                // Add all features to runnning series
                runningSeries.AddFeatures(featuresToSeries);

            }

        }

        protected virtual void RunningLogic()
        {
            // Account for all learning types now
            switch (m_trainingType)
            {
                // Classification and Regression
                case IMLSpecifications.TrainingSetType.SingleTrainingExamples:
                    // Get the output from rapidlib
                    PredictedRapidlibOutput = RunModel();
                    // Don't run when the predicteb rapidlib output is empty (this is a patch for some bug that broke the predictions)
                    if (PredictedRapidlibOutput.Length == 0)
                        break;
                    // Transform rapidlib output to IMLTypes (calling straight after getting the output so that the UI can show properly)
                    TransformPredictedOuputToIMLTypes(PredictedRapidlibOutput, ref PredictedOutput);
                    break;
                case IMLSpecifications.TrainingSetType.SeriesTrainingExamples:
                    CollectFeaturesInRunningSeries(InputFeatures, ref m_RunningSeries);
                    break;
                default:
                    break;
            }

        }
        //commented as not needed in new event based system - keeping yill thatis fully tested 
        /// <summary>
        /// Checks what is the output configuration 
        /// </summary>
       /* protected virtual bool CheckOutputConfiguration()
        {
            bool output = false;
            // In DTW, only update format if there is a node connection change or the predicted output is not correctly formatted
            if (m_LearningType == IMLSpecifications.LearningType.DTW)
            {
                if (m_NodeConnectionChanged
                || PredictedOutput.Any((i => (i == null || (i.Values == null || i.Values.Length == 0)))))
                {
                    output = true;
                }

            }
            // In classification and regression, only changed format when there is a change in nodes or formats don't match
            else
            {
                if (m_LastKnownRapidlibOutputVectorSize == 0
                || m_LastKnownRapidlibOutputVectorSize != PredictedRapidlibOutput.Length
                || m_NodeConnectionChanged
                || (m_LastKnownRapidlibOutputVectorSize > 0 && PredictedOutput.Count == 0))
                    output = true;
            }
            return true;
        }*/
        /// <summary>
        /// Checks what is the output configuration and creates a predicted output list in the correct format
        /// </summary>
        public void UpdateOutputFormat()
        {
            // Make sure that the list is not null
            if (PredictedOutput == null)
                PredictedOutput = new List<IMLBaseDataType>();
            
            //commented due to new event based system - keeping till 
            //bool updateOutFormat = CheckOutputConfiguration();

            // If we are meant to update format...
            //if (updateOutFormat)
           // {
                // Save size of rapidlib vectorsize to work with it 
                // DIRTY CODE.  THIS SHOULD CHECK IF THE OUTPUT CONFIGURATION ACTUALLY DID CHANGE OR NOT. YOU COULD HAVE 2 DIFF OUTPUTS CONFIGS WITH SAME VECTOR SIZE
                if (PredictedRapidlibOutput != null)
                    m_LastKnownRapidlibOutputVectorSize = PredictedRapidlibOutput.Length;
                // Adjust the desired outputs list based on configuration selected
                PredictedOutput.Clear();
                // Calculate required space for outputs
                for (int i = 0; i < m_ExpectedOutputList.Count; i++)
                {
                    // Generate data type for predicted output
                    switch (m_ExpectedOutputList[i])
                    {
                        case IMLSpecifications.OutputsEnum.Float:
                            PredictedOutput.Add(new IMLFloat());
                            break;
                        case IMLSpecifications.OutputsEnum.Integer:
                            PredictedOutput.Add(new IMLInteger());
                            break;
                        case IMLSpecifications.OutputsEnum.Vector2:
                            PredictedOutput.Add(new IMLVector2());
                            break;
                        case IMLSpecifications.OutputsEnum.Vector3:
                            PredictedOutput.Add(new IMLVector3());
                            break;
                        case IMLSpecifications.OutputsEnum.Vector4:
                            PredictedOutput.Add(new IMLVector4());
                            break;
                        case IMLSpecifications.OutputsEnum.Array:
                            PredictedOutput.Add(new IMLArray());
                            break;
                        default:
                            break;
                    }

                    /* DIRTY CODE */
                    // If we can, check if we need to update the corresponding output port
                    if (i < m_DynamicOutputPorts.Count)
                    {
                        var dynamicOutputPort = m_DynamicOutputPorts[i];
                        // Check if the output port needs and update or not
                        CheckOutputFormatMatchesDynamicPort(m_ExpectedOutputList[i], ref dynamicOutputPort);
                    }
                    else
                    {
                        // There was an unexpected change of training examples outputs, update list
                        UpdateDynamicOutputPorts(IMLTrainingExamplesNodes, m_ExpectedOutputList, ref m_DynamicOutputPorts);
                    }

                    
                //}

            }
            //Debug.Log(PredictedOutput.Count);
            //Debug.Log(PredictedRapidlibOutput.Length);
        }

        /// <summary>
        /// Updates the configuration list of inputs
        /// </summary>
        protected void UpdateInputConfigList()
        {
            // Get values from the input list
            InputFeatures = this.GetInputNodesConnected("InputFeatures");

            //check features have been connected 
            if (InputFeatures != null)
            {
                // Make sure that the list is initialised
                if (m_ExpectedInputList == null)
                    m_ExpectedInputList = new List<IMLSpecifications.InputsEnum>();

                // Adjust the desired inputs list based on nodes connected
                m_ExpectedInputList.Clear();
                // Go through all the nodes connected
                for (int i = 0; i < InputFeatures.Count; i++)
                {
                    // Cast the node checking if implements the feature interface (it is a featureExtractor)
                    IFeatureIML inputFeature = InputFeatures[i] as IFeatureIML;

                    // If it is a feature extractor...
                    if (inputFeature != null)
                    {
                        // We add the feature to the desired inputs config
                        m_ExpectedInputList.Add((IMLSpecifications.InputsEnum)inputFeature.FeatureValues.DataType);
                    }
                }
            }


        }
        /// <summary>
        /// Removes MLSystem node from list of MLSystem nodes connected in Training examples node that has been disconnected
        /// Called in remove connection 
        /// Done here as RemoveConnection method in xNode only gives information about the port ont that node
        /// </summary>
        private void TrainingNodeConnectedRemoved()
        {
            List<TrainingExamplesNode> oldTrainingExamplesNodes = IMLTrainingExamplesNodes;
            // Get values from the training example node
            IMLTrainingExamplesNodes = GetInputValues<TrainingExamplesNode>("IMLTrainingExamplesNodes").ToList();
            // commented out when moving deleting this node from training examples to remove connection or delete node
            
            if (IMLTrainingExamplesNodes.Count < oldTrainingExamplesNodes.Count)
            {
                List<TrainingExamplesNode> deletedTrainingExamplesNodes = oldTrainingExamplesNodes.Except(IMLTrainingExamplesNodes).ToList();
                foreach (TrainingExamplesNode node in deletedTrainingExamplesNodes)
                {
                    if (node != null)
                        node.MLSystemNodesConnected.Remove(this);
                }
            }

        }
        /// <summary>
        /// Updates the configuration list of outputs
        /// </summary>
        protected void UpdateOutputConfigList()
        {
            // Make sure that the output list is initialised
            if (m_ExpectedOutputList == null)
                m_ExpectedOutputList = new List<IMLSpecifications.OutputsEnum>();

            // Adjust the expected outputs list based on training node connected
            m_ExpectedOutputList.Clear();

            // Loop through all training examples nodes connected
            foreach (var trainingExamplesNode in IMLTrainingExamplesNodes)
            {
                // Access the list of expected outputs config from the training node connected
                if (trainingExamplesNode.DesiredOutputsConfig != null && trainingExamplesNode.DesiredOutputsConfig.Count > 0)
                {
                    // if there is no expected outputs cofgured yet populate expected type from the training examples node connected
                    if (m_ExpectedOutputList.Count == 0)
                    {
                        for (int i = m_ExpectedOutputList.Count; i < trainingExamplesNode.DesiredOutputsConfig.Count; i++)
                        {
                            m_ExpectedOutputList.Add(trainingExamplesNode.DesiredOutputsConfig[i]);
                        }
                    }
                    // if the number of training examples node desired output connected is bigger then the 
                    else if (trainingExamplesNode.DesiredOutputsConfig.Count > m_ExpectedOutputList.Count)
                    {
                        for (int i = m_ExpectedOutputList.Count - 1; i < trainingExamplesNode.DesiredOutputsConfig.Count - 1; i++)
                        {
                            m_ExpectedOutputList.Add(trainingExamplesNode.DesiredOutputsConfig[i]);
                        }
                    }
                    
                    if (m_ExpectedOutputList.Count <= trainingExamplesNode.DesiredOutputsConfig.Count)
                    {
                        for (int i = 0; i < m_ExpectedOutputList.Count - 1; i++)
                        {
                            if (m_ExpectedOutputList[i] != trainingExamplesNode.DesiredOutputsConfig[i])
                            {
                                Debug.Log("i");
                                m_ExpectedOutputList.Add(trainingExamplesNode.DesiredOutputsConfig[i]);
                            }
                        }
                    } else
                    {
                        for (int i = 0; i < trainingExamplesNode.DesiredOutputsConfig.Count; i++)
                        {
                            if (m_ExpectedOutputList[i] != trainingExamplesNode.DesiredOutputsConfig[i])
                            {
                                m_ExpectedOutputList.Add(trainingExamplesNode.DesiredOutputsConfig[i]);
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Adds output ports to the list of dynamic output ports (drawn in the editor class)
        /// </summary>
        /// <param name="trainingExamples"></param>
        /// <param name="outputPorts"></param>
        protected void AddDynamicOutputPorts(TrainingExamplesNode trainingExamples, ref List<XNode.NodePort> outputPorts)
        {
            // Make sure the dynamic port list is initialised
            if (outputPorts == null)
                outputPorts = new List<XNode.NodePort>();

            bool matchConfig = true;
            // Do we already have the same number of ports?
            if (outputPorts.Count == trainingExamples.DesiredOutputsConfig.Count)
            {
                // Are they ordered in the same way?
                for (int i = 0; i < outputPorts.Count; i++)
                {
                    switch (trainingExamples.DesiredOutputsConfig[i])
                    {
                        case IMLSpecifications.OutputsEnum.Float:
                            if (outputPorts[i].ValueType != typeof(float)) matchConfig = false;
                            break;
                        case IMLSpecifications.OutputsEnum.Integer:
                            if (outputPorts[i].ValueType != typeof(int)) matchConfig = false;
                            break;
                        case IMLSpecifications.OutputsEnum.Vector2:
                            if (outputPorts[i].ValueType != typeof(Vector2)) matchConfig = false;
                            break;
                        case IMLSpecifications.OutputsEnum.Vector3:
                            if (outputPorts[i].ValueType != typeof(Vector3)) matchConfig = false;
                            break;
                        case IMLSpecifications.OutputsEnum.Vector4:
                            if (outputPorts[i].ValueType != typeof(Vector4)) matchConfig = false;
                            break;
                        case IMLSpecifications.OutputsEnum.Array:
                            if (outputPorts[i].ValueType != typeof(float[])) matchConfig = false;
                            break;
                        case IMLSpecifications.OutputsEnum.Boolean:
                            if (outputPorts[i].ValueType != typeof(bool)) matchConfig = false;
                            break;
                        default:
                            break;
                    }

                }

            }

            // If there is no matching configuration, we need to rebuild the output ports list. Let's rebuild it
            if (!matchConfig)
            {
                // Clear dynamic ports
                ClearDynamicPorts();
                outputPorts.Clear();

                // Add as many output ports as we have output types in the training examples node. It will be drawn in the Editor class
                for (int i = 0; i < trainingExamples.DesiredOutputsConfig.Count; i++)
                {
                    // Get reference to current expected output
                    var expectedOutput = trainingExamples.DesiredOutputsConfig[i];

                    // Add output port based on the type
                    AddDynamicOutputPort(expectedOutput, ref outputPorts);
                }

            }


        }

        /// <summary>
        /// Adds one dynamic output port based on its type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="outputPorts"></param>
        protected void AddDynamicOutputPort(IMLSpecifications.OutputsEnum type, ref List<XNode.NodePort> outputPorts)
        {
            // Make sure the dynamic port list is initialised
            if (outputPorts == null)
                outputPorts = new List<XNode.NodePort>();

            // Prepare output port
            XNode.NodePort dynamicOutputPort;
            // Add a specific kind of type for the output node depending on the expected type. The index will be the current length of the outputPorts list (since we are adding new ones, it will constantly increase)
            switch (type)
            {
                case IMLSpecifications.OutputsEnum.Float:
                    dynamicOutputPort = AddDynamicOutput(typeof(float), fieldName: $"Out {outputPorts.Count} (Float)");
                    break;
                case IMLSpecifications.OutputsEnum.Integer:
                    dynamicOutputPort = AddDynamicOutput(typeof(int), fieldName: $"Out {outputPorts.Count} (Int)");
                    break;
                case IMLSpecifications.OutputsEnum.Vector2:
                    dynamicOutputPort = AddDynamicOutput(typeof(Vector2), fieldName: $"Out {outputPorts.Count} (V2)");
                    break;
                case IMLSpecifications.OutputsEnum.Vector3:
                    dynamicOutputPort = AddDynamicOutput(typeof(Vector3), fieldName: $"Out {outputPorts.Count} (V3)");
                    break;
                case IMLSpecifications.OutputsEnum.Vector4:
                    dynamicOutputPort = AddDynamicOutput(typeof(Vector4), fieldName: $"Out {outputPorts.Count} (V4)");
                    break;
                case IMLSpecifications.OutputsEnum.Array:
                    dynamicOutputPort = AddDynamicOutput(typeof(float[]), fieldName: $"Out {outputPorts.Count} (Array)");
                    break;
                default:
                    dynamicOutputPort = null;
                    break;
            }

            // If we got one output port to add, we add it
            if (dynamicOutputPort != null && !outputPorts.Contains(dynamicOutputPort))
                outputPorts.Add(dynamicOutputPort);

        }

        /// <summary>
        /// Removes output ports from the list of dynamic output ports (the last batch added)
        /// </summary>
        /// <param name="traininingExamples"></param>
        /// <param name="outputPorts"></param>
        protected void RemoveDynamicOutputPorts(TrainingExamplesNode trainingExamples, ref List<XNode.NodePort> outputPorts)
        {
            // Make sure the dynamic port list is initialised
            if (outputPorts == null)
            {
                outputPorts = new List<XNode.NodePort>();
                // If we don't have any to remove, then just exit the method
                return;
            }

            // Select ports to remove. Assume that we will be removing the last output ports we added
            List<XNode.NodePort> portsToRemove = new List<XNode.NodePort>(outputPorts.GetRange(outputPorts.Count - (trainingExamples.DesiredOutputsConfig.Count) - 1, trainingExamples.DesiredOutputsConfig.Count));

            // Remove those ports from local list
            outputPorts.RemoveRange(outputPorts.Count - (trainingExamples.DesiredOutputsConfig.Count) - 1, trainingExamples.DesiredOutputsConfig.Count);

            // Remove ports from node parent class
            for (int i = 0; i < portsToRemove.Count; i++)
            {
                // Get reference
                var port = portsToRemove[i];
                // Remove from list first
                portsToRemove.Remove(port);
                // Remove from node parent class
                RemoveDynamicPort(port);

            }

        }

        protected void UpdateDynamicOutputPorts(List<TrainingExamplesNode> trainingExamplesNodes, List<IMLSpecifications.OutputsEnum> expectedOutputs, ref List<XNode.NodePort> outputPorts)
        {
            // Make sure the dynamic port list is initialised
            if (outputPorts == null)
                outputPorts = new List<XNode.NodePort>();
            if (expectedOutputs == null)
                expectedOutputs = new List<IMLSpecifications.OutputsEnum>();

            // Calculate how many output ports we need to amend
            int totalExpectedOutputs = expectedOutputs.Count;
            // If we have expected outputs, proceed as planned...
            if (totalExpectedOutputs > 0)
            {
                int diff = totalExpectedOutputs - outputPorts.Count;
                // If we need to add more output ports...
                if (diff > 0)
                {
                    // Add to the end
                    for (int i = 0; i < diff; i++)
                    {
                        int index = (totalExpectedOutputs - 1) - i;
                        AddDynamicOutputPort(expectedOutputs[index], ref outputPorts);
                    }
                }
                // If we need to remove output ports...
                else if (diff < 0)
                {
                    int diffAbs = Mathf.Abs(diff);
                    // We will try to reuse as many ports as we can
                    // start from the end
                    int index = (outputPorts.Count - 1);
                    // Destroy ports
                    for (int i = 0; i < diffAbs; i++)
                    {

                        var port = outputPorts[index - i];
                        // remove port from parent list
                        RemoveDynamicPort(port);
                    }
                    // Now reduce size of local list
                    int indexToRemove = 0;
                    // Attempt to remove from the end
                    if (diffAbs < outputPorts.Count && diffAbs * 2 < outputPorts.Count) indexToRemove = diffAbs;
                    // if not, remove from... beginning?
                    else indexToRemove = (outputPorts.Count) - diffAbs;
                    outputPorts.RemoveRange(indexToRemove, diffAbs);
                }

                // Make sure that all port types and names are matching (expectedOutputs and outputPorts are now the same size)
                for (int i = 0; i < outputPorts.Count; i++)
                {
                    var port = outputPorts[i];
                    // Format
                    CheckOutputFormatMatchesDynamicPort(expectedOutputs[i], ref port);
                    // fieldName index
                    outputPorts[i] = CheckOutputIndexMatchesDynamicPort(i, port);
                }


            }
            // If we don't have exptected outputs, clear ports
            else
            {
                // Clear entire local list
                outputPorts.Clear();
                // Use local list as a copy of parent class list
                outputPorts = DynamicOutputs.ToList();
                // Clear all dynamic outputs for a fresh start
                for (int i = 0; i < outputPorts.Count(); i++)
                {
                    var outputPort = outputPorts[i];
                    RemoveDynamicPort(outputPort);
                }
                // Clear entire local list again for a fresh start
                outputPorts.Clear();

            }
            // Update flag so that the editor can be drawn properly
            OutputPortsChanged = true;

            //// Clear entire local list
            //outputPorts.Clear();
            //// Use local list as a copy of parent class list
            //outputPorts = DynamicOutputs.ToList();
            //// Clear all dynamic outputs for a fresh start
            //for (int i = 0; i < outputPorts.Count(); i++)
            //{
            //    var outputPort = outputPorts[i];
            //    RemoveDynamicPort(outputPort);
            //}
            //// Clear entire local list again for a fresh start
            //outputPorts.Clear();

            //// Add output ports per training examples
            //foreach (var trainingExamples in trainingExamplesNodes)
            //{
            //    AddDynamicOutputPorts(trainingExamples, ref outputPorts);
            //}
        }

        /// <summary>
        /// Checks if the expected output type matches the port output type
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="outputPort"></param>
        protected void CheckOutputFormatMatchesDynamicPort(IMLSpecifications.OutputsEnum dataType, ref XNode.NodePort outputPort)
        {
            // Adjusts the value type of a nodeport
            switch (dataType)
            {
                case IMLSpecifications.OutputsEnum.Float:
                    if (outputPort.ValueType != typeof(float))
                        outputPort.ValueType = typeof(float);
                    break;
                case IMLSpecifications.OutputsEnum.Integer:
                    if (outputPort.ValueType != typeof(int))
                        outputPort.ValueType = typeof(int);
                    break;
                case IMLSpecifications.OutputsEnum.Vector2:
                    if (outputPort.ValueType != typeof(Vector2))
                        outputPort.ValueType = typeof(Vector2);
                    break;
                case IMLSpecifications.OutputsEnum.Vector3:
                    if (outputPort.ValueType != typeof(Vector3))
                        outputPort.ValueType = typeof(Vector3);
                    break;
                case IMLSpecifications.OutputsEnum.Vector4:
                    if (outputPort.ValueType != typeof(Vector4))
                        outputPort.ValueType = typeof(Vector4);
                    break;
                case IMLSpecifications.OutputsEnum.Array:
                    if (outputPort.ValueType != typeof(float[]))
                        outputPort.ValueType = typeof(float[]);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Checks if the expected output index matches the one in the port output name
        /// </summary>
        /// <param name="expectedIndex"></param>
        /// <param name="outputPort"></param>
        protected XNode.NodePort CheckOutputIndexMatchesDynamicPort(int expectedIndex, XNode.NodePort outputPort)
        {
            // Get port index
            string portIndexString = new string(outputPort.fieldName.Where(char.IsDigit).ToArray());
            int portIndex = 0;
            // If we got the index from the field name...
            if (int.TryParse(portIndexString, out portIndex))
            {
                // If indexes are not the same...
                if (expectedIndex != portIndex)
                {
                    // Amend port name index, we will need to create a new port and use in place
                    XNode.NodePort newPort = new XNode.NodePort(
                        "Output " + expectedIndex.ToString(),
                        outputPort.ValueType,
                        XNode.NodePort.IO.Output,
                        outputPort.connectionType,
                        outputPort.typeConstraint,
                        outputPort.node);
                    // Return new port
                    return newPort;

                }
                // If indexes are the same...
                else
                {
                    // Return original port
                    return outputPort;
                }
            }
            else
            {
                throw new Exception("The output port fieldName doesn't contain an index!");
            }
        }
        /// <summary>
        /// Updates number of training examples 
        /// </summary>
        public void UpdateTotalNumberTrainingExamples()
        {
            // Get training examples from the connected examples nodes
            IMLTrainingExamplesNodes = GetInputValues<TrainingExamplesNode>("IMLTrainingExamplesNodes").ToList();

            // The total number will start from 0 and keep adding the total amount of training examples from nodes connected
            m_TotalNumTrainingDataConnected = 0;
            if (!Lists.IsNullOrEmpty(ref IMLTrainingExamplesNodes))
            {
                
                for (int i = 0; i < IMLTrainingExamplesNodes.Count; i++)
                {
                    if (IMLTrainingExamplesNodes[i] != null)
                    {
                        switch (m_trainingType)
                        {
                            case IMLSpecifications.TrainingSetType.SingleTrainingExamples:
                                m_TotalNumTrainingDataConnected += IMLTrainingExamplesNodes[i].TotalNumberOfTrainingExamples;
                                break;
                            case IMLSpecifications.TrainingSetType.SeriesTrainingExamples:
                                m_TotalNumTrainingDataConnected += IMLTrainingExamplesNodes[i].TrainingSeriesCollection.Count;
                                break;
                            default:
                                Debug.LogWarning("Training type not set");
                                break;
                        }
                            
                    }
                    else
                    {
                        Debug.Log("Training Examples Node [" + i + "] is null!");
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the number of nodes connected is correct to be changed when new input system implemented
        /// </summary>
        protected void CheckLiveDataInputMatchesTrainingExamples()
        {
            // Get training examples from the connected examples nodes
            IMLTrainingExamplesNodes = GetInputValues<TrainingExamplesNode>("IMLTrainingExamplesNodes").ToList();
            // get the number of input features for the first training examples
            int numberOfTrainingDataInputs = 0;
            int numOfDesiredTrainingDataInputs = 0;
            int numOfLiveDataInputs = 0;
            bool alreadyChecked = false;
            // if the list has objects
            if(!Lists.IsNullOrEmpty(ref IMLTrainingExamplesNodes))
            {
                if (IMLTrainingExamplesNodes[0] != null )
                {
                    var liveFeaturesFromTrainingExamples = IMLTrainingExamplesNodes[0].GetInputPort("InputFeatures");
                    if (liveFeaturesFromTrainingExamples != null)
                    {
                        // set the numnber of training data inputs as the first connected training example (which should be the same as the rest connected)
                        numberOfTrainingDataInputs = liveFeaturesFromTrainingExamples.GetConnections().Count();
                        // If inputs are 0, we need a different way of calculating if the live inputs match the training examples
                        if (numberOfTrainingDataInputs == 0)
                        {
                            if (IMLTrainingExamplesNodes[0].DesiredInputFeatures != null && InputFeatures != null)
                            {
                                foreach (var desiredInputFeature in IMLTrainingExamplesNodes[0].DesiredInputFeatures)
                                {
                                    if (desiredInputFeature != null && desiredInputFeature.Values != null)
                                        numOfDesiredTrainingDataInputs += desiredInputFeature.Values.Length;
                                }

                                foreach (var liveInputFeature in InputFeatures)
                                {
                                    if (liveInputFeature != null)
                                    {
                                        var liveFeatureValues = (liveInputFeature as IFeatureIML).FeatureValues;
                                        if (liveFeatureValues != null && liveFeatureValues.Values != null)
                                            numOfLiveDataInputs += liveFeatureValues.Values.Length;
                                    }
                                }
                                // Check that the sequence of inputs is equal in the training examples node and the MLSystem
                                if (numOfDesiredTrainingDataInputs == numOfLiveDataInputs)
                                {
                                    matchLiveDataInputs = true;
                                    alreadyChecked = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("There is a null reference in training examples node connected!");
                }
            }

            if (!alreadyChecked)
            {
                // if the number of input features connected is the same to that of the connected training examples set matching to true
                matchLiveDataInputs = (numberOfTrainingDataInputs == GetInputPort("InputFeatures").GetConnections().Count()) ?  true : false;
            }                

        }

        /// <summary>
        /// Create the rapidlib training examples list in the required format
        /// </summary>
        protected List<RapidlibTrainingExample> TransformIMLDataToRapidlib(List<TrainingExamplesNode> trainingNodesIML, out int numExamples)
        {
            // Create list to return
            List<RapidlibTrainingExample> rapidlibExamples = new List<RapidlibTrainingExample>();

            // Reset counter examples trained on
            numExamples = 0;

            // Make sure unique training classes is init
            if (m_TotalUniqueTrainingClasses == null) m_TotalUniqueTrainingClasses = new List<IMLTrainingExample>();
            // Empty unique training classes (we are going to retrain the model)
            m_TotalUniqueTrainingClasses.Clear();
            m_TotalNumUniqueClasses = 0;

            // Go through all the IML Training Examples if we can
            if (!Lists.IsNullOrEmpty(ref trainingNodesIML))
            {
                //comeback
                // Reset counter examples trained on
                numExamples = 0;
                // Go through each node
                for (int i = 0; i < trainingNodesIML.Count; i++)
                {
                    // If there are training examples in this node...
                    if (trainingNodesIML[i].TrainingExamplesVector.Count > 0)
                    {
                        // Pull all unique classes the model will be trained on
                        AddUniqueOutputs(trainingNodesIML[i], ref m_TotalUniqueTrainingClasses, ref m_TotalNumUniqueClasses);

                        // Go through all the training examples
                        for (int j = 0; j < trainingNodesIML[i].TrainingExamplesVector.Count; j++)
                        {
                            // Check that individual example is not null
                            var IMLTrainingExample = trainingNodesIML[i].TrainingExamplesVector[j];
                            if (IMLTrainingExample != null)
                            {
                                // Check that inputs/outputs are not null
                                if (IMLTrainingExample.Inputs != null && IMLTrainingExample.Outputs != null)
                                {
                                    // Add a new rapidlib example to list
                                    rapidlibExamples.Add(new RapidlibTrainingExample(IMLTrainingExample.GetInputs(), IMLTrainingExample.GetOutputs()));
                                }
                                // If there are null outputs we debug an error
                                else
                                {
                                    Debug.LogError("Null inputs/outputs found when training IML model. Training aborted!");
                                }
                            }
                        }
                        // Update counter examples trained on
                        numExamples += trainingNodesIML[i].TrainingExamplesVector.Count;
                    }
                }
            }

            // Return list
            return rapidlibExamples;
        }

        /// <summary>
        /// Create a rapidlib training series list in the required format for DTW training
        /// </summary>
        /// <param name="trainingSeriesIML"></param>
        /// <returns></returns>
        protected List<RapidlibTrainingSerie> TransformIMLSeriesToRapidlib(List<TrainingExamplesNode> trainingNodesIML, out int numSeries)
        {
            List<RapidlibTrainingSerie> seriesToReturn = new List<RapidlibTrainingSerie>();
            // Reset number of series
            numSeries = 0;
            // Go through all the IML Training Examples if we can
            if (!Lists.IsNullOrEmpty(ref trainingNodesIML))
            {
                // Go through each node
                for (int i = 0; i < trainingNodesIML.Count; i++)
                {
                    // If there are training series in this node...
                    if (trainingNodesIML[i].TrainingSeriesCollection.Count > 0)
                    {
                        foreach (var IMLSeries in trainingNodesIML[i].TrainingSeriesCollection)
                        {
                            // Add each series to the rapidlib series list to return
                            seriesToReturn.Add(new RapidlibTrainingSerie(IMLSeries.GetSeriesFeatures(), IMLSeries.LabelSeries));
                        }
                        // Increase counter of series
                        numSeries += trainingNodesIML[i].TrainingSeriesCollection.Count;
                        Debug.Log(trainingNodesIML[i].TrainingSeriesCollection.Count);
                    }
                }
            }
            
            return seriesToReturn;
        }

        /// <summary>
        /// Creates the rapidlib input vector (input for the realtime predictions)
        /// </summary>
        protected void CreateRapidlibInputVector()
        {
            // If we have some input connected...
            if (!Lists.IsNullOrEmpty(ref InputFeatures))
            {
                int vectorSize = 0;
                // Create a vector based on the amount of input features to predict
                // Calculate vector size
                for (int i = 0; i < InputFeatures.Count; i++)
                {
                    var inputFeature = InputFeatures[i] as IFeatureIML;
                    if (inputFeature != null)
                    {
                        if (inputFeature.FeatureValues != null)
                        {
                            if (inputFeature.FeatureValues.Values != null)
                            {
                                vectorSize += inputFeature.FeatureValues.Values.Length;
                            }
                        }
                    }
                }

                // Create vector
                m_RapidlibInputVector = new double[vectorSize];

            }

        }

        /// <summary>
        /// Creates the rapidlib output vector (output from the realtime predictions)
        /// </summary>
        protected void CreateRapidLibOutputVector()
        {
            // If we have some expected output...
            if (!Lists.IsNullOrEmpty(ref PredictedOutput))
            {
                int vectorSize = 0;
                // Create a vector based on the amount of output features to predict
                // Calculate vector size
                for (int i = 0; i < PredictedOutput.Count; i++)
                {
                    if (PredictedOutput[i] != null)
                    {
                        if (PredictedOutput[i].Values != null)
                        {
                            vectorSize += PredictedOutput[i].Values.Length;
                        }
                    }
                }

                // Create vector
                m_RapidlibOutputVector = new double[vectorSize];
            }
        }

        protected void UpdateRapidLibOutputVector()
        {
            if (!Lists.IsNullOrEmpty(ref PredictedOutput))
            {
                int vectorSize = 0;
                // Create a vector based on the amount of output features to predict
                // Calculate vector size
                for (int i = 0; i < PredictedOutput.Count; i++)
                {
                    if (PredictedOutput[i] != null)
                    {
                        if (PredictedOutput[i].Values != null)
                        {
                            vectorSize += PredictedOutput[i].Values.Length;
                        }
                    }
                }

                // Create vector
                System.Array.Resize(ref m_RapidlibOutputVector, vectorSize);
            }

        }
        /// <summary>
        /// Updates the input vector to send to rapidlib with the input features in the IML Config node
        /// </summary>
        protected void UpdateInputVector()
        {
            CreateRapidlibInputVector();
            // If we have some input connected...
            if (!Lists.IsNullOrEmpty(ref InputFeatures))
            {
                int vectorPointer = 0;
                // Go thorugh all the features to add their input
                for (int i = 0; i < InputFeatures.Count; i++)
                {
                    var inputFeature = InputFeatures[i] as IFeatureIML;
                    if (inputFeature != null)
                    {
                        if (inputFeature.FeatureValues != null)
                        {
                            if (inputFeature.FeatureValues.Values != null)
                            {
                                // Make sure that the vector pointer doesn't go too far away
                                if (vectorPointer > m_RapidlibInputVector.Length)
                                    throw new UnityException("Vector size too small when updating input features in IML Config Node!");


                                // Go through all the values of a specific input feature
                                for (int j = 0; j < inputFeature.FeatureValues.Values.Length; j++)
                                {
                                    // Copy each value into the input vector
                                    m_RapidlibInputVector[j + vectorPointer] = inputFeature.FeatureValues.Values[j];
                                }

                                // Move forward the vectorPointer to avoid overriding the previous input feature
                                vectorPointer += inputFeature.FeatureValues.Values.Length;

                            }
                        }
                    }
                }

            }

        }

        /// <summary>
        /// Sets all error flags to the value passed in
        /// </summary>
        /// <param name="value"></param>
        protected void SetErrorFlags(bool value)
        {
            m_ErrorWrongInputTrainingExamplesPort = value;
        }

        protected void CheckTrainingExamplesConnections(XNode.NodePort from, XNode.NodePort to, string portName)
        {
            // Evaluate the nodeport for training examples
            if (to.fieldName == portName)
            {
                bool isNotTrainingExamplesNode = false;
                if (m_trainingType == IMLSpecifications.TrainingSetType.SingleTrainingExamples)
                {
                    // Check if the node connected was a single training examples node OR a node implementing IDataSetIML
                    Type[] typesAccepted = { typeof(SingleTrainingExamplesNode), typeof(IDataSetIML) };
                    isNotTrainingExamplesNode = this.DisconnectFROMPortIsNotTypes(from, to, typesAccepted);
                    //isNotTrainingExamplesNode = this.DisconnectIfNotType<MLSystem, SingleTrainingExamplesNode>(from, to);
                    Debug.Log(isNotTrainingExamplesNode);
                }
                else
                {
                    // Check if the node connected was a training examples node
                    isNotTrainingExamplesNode = this.DisconnectIfNotType<MLSystem, SeriesTrainingExamplesNode>(from, to);

                }

                // If we broke the connection...
                if (isNotTrainingExamplesNode)
                {
                    // Prepare flag to show error regarding training examples
                    m_ErrorWrongInputTrainingExamplesPort = true;
                }
                // If we accept the connection...
                else
                {
                    TrainingExamplesNode examplesNode = from.node as TrainingExamplesNode;
                    // if there is already a training examples node attached
                    if (IMLTrainingExamplesNodes.Count > 0)
                    {
                        TrainingExamplesNode connectedNode = IMLTrainingExamplesNodes[0];
                        // if the desired
                        if (!Enumerable.SequenceEqual(connectedNode.DesiredInputsConfig, examplesNode.DesiredInputsConfig) && !Enumerable.SequenceEqual(connectedNode.DesiredInputsConfig, examplesNode.DesiredInputsConfig))
                        {
                            m_TrainingExamplesConflict = true;
                            from.Disconnect(to);
                        }
                    }

                    // Disconnect if there is a wrong number of target values
                    if (examplesNode.TargetValues != null && examplesNode.TargetValues.Count > 1 && isKNN)
                    {
                        from.Disconnect(to);
                        m_WrongNumberOfTargetValues = true;
                    }
                    // We check that the connection is from a training examples node
                    if (examplesNode != null && !m_WrongNumberOfTargetValues && !m_TrainingExamplesConflict)
                    {
                        // Update dynamic ports for output
                        AddDynamicOutputPorts(examplesNode, ref m_DynamicOutputPorts);
                    }
                }

            }
        }

        protected virtual void CheckInputFeaturesConnections(XNode.NodePort from, XNode.NodePort to, string portName)
        {
            // Evaluate nodeport for real-time features
            if (to.fieldName == portName)
            {
                // Only accept IFeaturesIML
                bool connectionBroken = this.DisconnectIfNotType<MLSystem, IFeatureIML>(from, to);

                if (connectionBroken)
                {
                    // TO DO
                    // DISPLAY ERROR, not a feature connected
                }
            }
        }
/*
        protected bool CheckTrainingExamplesConfigMatch(List<TrainingExamplesNode> trainingNodesIML)
        {
            bool match = true;
            //if there are more than one training node connected check there is an equal vector
            if (trainingNodesIML.Count > 1)
            {
                int vectorSize = CheckLengthTrainingVector(trainingNodesIML[0]);
                int targetVector = 0;
                for (int i = 1; i < trainingNodesIML.Count; i++)
                {
                    //make this method for target values too 
                    if (vectorSize != CheckLengthTrainingVector(trainingNodesIML[i]))
                    {
                        Debug.LogWarning("Can't train mismatch in Inputs");

                    }
                }

            }
            return match;
        }*/
        /// <summary>
        /// Checks whether the lives data matches or the application is currently playing and sets errors boolean to true
        /// also sets what the tooltips should be for the error. The boolean and tooltip are used by the editor script to show and set error message
        /// This is called when the inputs or labels to attached training nodes are changed and when we move between play modes 
        /// </summary>
        public void UIErrors()
        {
            //in the inputs don't match or the input vector is a different lentgh show error
            if (!matchLiveDataInputs || !matchVectorLength)
            {
                if (tooltips.BottomError != null && tooltips.BottomError.Length > 3)
                {
                    warning = tooltips.BottomError[3];
                } else
                {
                    //Debug.Log("tooltip not found");
                }

                error = true;
                // if the applicsation is not playing show error about running in edit mode 
            } else if (!Application.isPlaying)
            {
                if (tooltips.BottomError != null && tooltips.BottomError.Length > 3)
                    warning = tooltips.BottomError[4];
                error = true;
            }
            else
            {
                error = false;
            }
        }

        public void OnLabelChanged()
        {
            UpdateOutputConfigList();
            // Make sure that current output format matches the expected output format
            UpdateOutputFormat();
            // Make sure dynamic output ports match the expected output format
            UpdateDynamicOutputPorts(IMLTrainingExamplesNodes, m_ExpectedOutputList, ref m_DynamicOutputPorts);
            // COMEBACK TO THIS IF THERE ARE BIG PROBLEMS 
            UpdateRapidLibOutputVector();

        }

        public void OnDataInChanged()
        {
            
            // Update Input Config List
            UpdateInputConfigList();

            //Check if the vector length of connected inputs matches the training examples connected
            CheckLengthInputsVector();
            //Check if number of inputs connected matches training examples 
            CheckLiveDataInputMatchesTrainingExamples();
        }

        public void UpdateLogic()
        {
            //test
            //Debug.Log(Model.ModelAddress);
            //Debug.Log(Model.ModelStatus);
            //Debug.Log(Trained);
            //Debug.Log(m_trainingType);
            //Debug.Log(m_Model.TypeOfModel);
            
            //UpdateDynamicOutputPorts(IMLTrainingExamplesNodes, m_ExpectedOutputList, ref m_DynamicOutputPorts);
            // Pull inputs from bool event nodeports
            if (GetInputValue<bool>("ToggleTrainInputBoolPort"))
                IMLEventDispatcher.TrainMLSCallback(this.id);
            if (GetInputValue<bool>("ToggleRunInputBoolPort"))
                IMLEventDispatcher.ToggleRunCallback(this.id);

            // Perform running logic (it will account for DTW and Classification/Regression) only if there is a predicted output            
            RunningLogic();

            // Do testing logic 
            TestingLogic();


        }

        private bool LoadOrTrain()
        {
            Debug.Log("here");
            ResetModel();
            //Debug.Log(trainOnLoad);
            //Debug.Log(Model.TypeOfModel);
            if (Model.TypeOfModel == RapidlibModel.ModelType.DTW)
            {
                if (NumExamplesTrainedOn > 0)
                {
                    return TrainModel();
                } else
                {
                    return true;
                }
                

            } else if(Model.TypeOfModel == RapidlibModel.ModelType.NeuralNetwork || Model.TypeOfModel == RapidlibModel.ModelType.kNN)  
            {
                m_NumExamplesTrainedOn = m_TotalNumTrainingDataConnected;
                return LoadModelFromDisk();
            }
            else
            {
                return true;
            }
            
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
                if (Trained)
                {
                    status = "Trained " + m_NumExamplesTrainedOn.ToString() + "\n";
                }
                
                if (Running)
                {
                    status += "Running \n";
                    if(LearningType != IMLSpecifications.LearningType.DTW)
                    {
                        int i = 0;
                        foreach (IMLBaseDataType dataType in PredictedOutput)
                        {
                            status += "Output: " + i;
                            i++;
                            foreach (float f in dataType.Values)
                                status += f.ToString() + "\n";
                        }

                    }
                    
                } else
                {
                    if(LearningType == IMLSpecifications.LearningType.DTW)
                    {
                        int i = 0;
                        foreach (IMLBaseDataType dataType in PredictedOutput)
                        {
                            status += "Output: " + i;
                            i++;
                            foreach (float f in dataType.Values)
                                status += f.ToString() + "\n";
                        }
                    }
                }

                return status;
            }
            return null;
        }

        #endregion

        #region Unique Training Classes Methods

        /// <summary>
        /// Adds all unique training classes from the outputs in a training dataset
        /// </summary>
        /// <param name="trainingExamplesNode"></param>
        /// <param name="uniqueClassesKnown"></param>
        protected void AddUniqueOutputs(TrainingExamplesNode trainingExamplesNode, ref List<IMLTrainingExample> uniqueClassesKnown, ref int totalNumUniqueClasses)
        {
            if (uniqueClassesKnown == null)
                uniqueClassesKnown = new List<IMLTrainingExample>();

            // Pull all unique classes the model will be trained on
            if (trainingExamplesNode.UniqueClasses != null && trainingExamplesNode.UniqueClasses.Count > 0)
            {
                foreach (var trainingClass in trainingExamplesNode.UniqueClasses)
                {
                    // Is there a new unique trainingClass in this node? 
                    if (!uniqueClassesKnown.Where(uniqueClassKnown => NodeExtensionMethods.OutputsEqual(uniqueClassKnown.Outputs, trainingClass.Outputs)).Any())
                    {
                        // If so, add it to our list of unique trainingClasses
                        var newUniqueTrainingClass = new IMLTrainingExample();
                        newUniqueTrainingClass.Outputs = trainingClass.Outputs;
                        uniqueClassesKnown.Add(newUniqueTrainingClass);
                        totalNumUniqueClasses++;
                    }
                }
            }

        }

        /// <summary>
        /// Reloads all unique outputs based on training examples nodes passed in
        /// </summary>
        /// <param name="trainingExamplesNodes"></param>
        /// <param name="uniqueClassesKnown"></param>
        protected void ReloadUniqueOutputs(List<TrainingExamplesNode> trainingExamplesNodes, ref List<IMLTrainingExample> uniqueClassesKnown, ref int totalNumUniqueClasses)
        {
            // Make sure unique training classes is init
            if (uniqueClassesKnown == null) uniqueClassesKnown = new List<IMLTrainingExample>();
            // Empty unique training classes (we are going to retrain the model)
            uniqueClassesKnown.Clear();
            totalNumUniqueClasses = 0;

            // Go through all the IML Training Examples if we can
            if (!Lists.IsNullOrEmpty(ref trainingExamplesNodes))
            {
                // Go through each node
                for (int i = 0; i < trainingExamplesNodes.Count; i++)
                {
                    // If there are training examples in this node...
                    if (trainingExamplesNodes[i].TrainingExamplesVector.Count > 0)
                    {
                        // Pull all unique classes the model will be trained on
                        AddUniqueOutputs(trainingExamplesNodes[i], ref uniqueClassesKnown, ref totalNumUniqueClasses);
                    }
                }
            }

        }

        #endregion

        #region Collecting Testing Data Methods

        public void StartTesting()
        {
            // Set model status to testing
            m_Testing = true;
            // How many classes do we need to collect?
            m_TestingClassesCollected = new bool[m_TotalNumUniqueClasses];
            // Reset index to start collecting testing classes
            m_CurrentTestingClassCollected = 0;
        }

        public void StopTesting()
        {
            m_Testing = false;
        }

        public void NextTestingClass()
        {
            // Flag current testing class as collected
            m_TestingClassesCollected[m_CurrentTestingClassCollected] = true;
            // Move index collecting testing classes forward
            m_CurrentTestingClassCollected++;
            // Make sure we don't pass limit
            if (m_CurrentTestingClassCollected >= m_TestingClassesCollected.Length)
                m_CurrentTestingClassCollected = m_TestingClassesCollected.Length - 1;

        }

        /// <summary>
        /// Collects testing data for a specified class label
        /// </summary>
        /// <param name="classLabel"></param>
        protected void CollectTestExample (List<IMLOutput> classLabel, int indexClass)
        {
            if (classLabel != null)
            {
                // Make sure list is init
                if (m_TestingData == null) m_TestingData = new List<List<IMLTrainingExample>>();
                List<IMLTrainingExample> ourTrainingDataList = null;
                // Make we have a sublist ready, if not init it
                if (indexClass >= m_TestingData.Count) m_TestingData.Add(new List<IMLTrainingExample>());
                // get sublist if already exists
                if (indexClass <= m_TestingData.Count - 1) ourTrainingDataList = m_TestingData[indexClass];
                else Debug.LogError($"Index Class is out of bounds! Index: {indexClass}");

                var newTestingExample = new IMLTrainingExample();
                // output is the classlabel passed in
                foreach (var label in classLabel)
                {
                    newTestingExample.AddOutputExample(label.OutputData);
                }

                // Add all the live input features to the testing example being recorded
                for (int i = 0; i < InputFeatures.Count; i++)
                {
                    newTestingExample.AddInputExample((InputFeatures[i] as IFeatureIML).FeatureValues);
                }

                // Add to testing data list
                ourTrainingDataList.Add(newTestingExample);
            }
        }

        protected void TestingLogic()
        {
            if (m_Testing)
            {
                // Only allow testing if the model is running
                if (!m_Running)
                {
                    m_Testing = false;
                    return;
                }

                if (m_CollectingTestingData)
                {
                    if (Application.isPlaying && m_TimeToStopCapture > 0 && Time.time >= m_TimeToStopCapture)
                    {
                        //Debug.Log("collecting false");
                        m_CollectingTestingData = false;
                    }
                    else if (!Application.isPlaying || Time.time >= m_TimeToNextCapture)
                    {
                        // Collect testing data for the current testing class
                        CollectTestExample(m_TotalUniqueTrainingClasses[m_CurrentTestingClassCollected].Outputs, m_CurrentTestingClassCollected);
                        m_TimeToNextCapture = Time.time + 1.0f / CaptureRate;
                    }

                }

                // DO SOMETHING
                Debug.Log($"MODEL TESTING!! ID: {id}");
            }
        }

        /// <summary>
        /// Toggles collect testing data flag for a specific class
        /// </summary>
        /// <param name="indexClass"></param>
        public void ToggleCollectTestingData()
        {
            m_CollectingTestingData = !m_CollectingTestingData;
        }

        /// <summary>
        /// Deletes the testing dataset for a specific class
        /// </summary>
        /// <param name="indexClass"></param>
        public void DeleteTestingDataForClass (int indexClass)
        {
            // Make sure list is init
            if (m_TestingData == null) m_TestingData = new List<List<IMLTrainingExample>>();
            List<IMLTrainingExample> ourTrainingDataList = null;
            // Make we have a sublist ready, if not init it
            if (indexClass >= m_TestingData.Count) m_TestingData.Add(new List<IMLTrainingExample>());
            // get sublist if already exists
            if (indexClass <= m_TestingData.Count - 1) ourTrainingDataList = m_TestingData[indexClass];
            else Debug.LogError($"Index Class is out of bounds! Index: {indexClass}");

            ourTrainingDataList.Clear();
        }

        #endregion

    }
}
