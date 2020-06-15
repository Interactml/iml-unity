using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using System.Linq;
using ReusableMethods;
using System;

namespace InteractML
{
    [NodeWidth(420)]
    public class IMLConfiguration : IMLNode
    {

        #region Variables

        /// <summary>
        /// The input list of TrainingExamplesNodes 
        /// </summary>
        [Input]
        public List<TrainingExamplesNode> IMLTrainingExamplesNodes;
        [Input]
        public List<Node> InputFeatures;
        [Output, SerializeField]
        public IMLConfiguration ModelOutput;

        /// <summary>
        /// The list of predicted outputs
        /// </summary>
        [SerializeField]
        public List<IMLBaseDataType> PredictedOutput;

        [HideInInspector]
        public double[] PredictedRapidlibOutput;
        [HideInInspector]
        public double[] DelayedPredictedOutput;

        /// <summary>
        /// Total updated number of training examples connected to this IML Configuration Node
        /// </summary>
        protected int m_TotalNumTrainingData;
        public int TotalNumTrainingData { get { return m_TotalNumTrainingData; } }

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
        protected List<NodePort> m_DynamicOutputPorts;


        /// <summary>
        /// The learning type of this model
        /// </summary>
        [SerializeField]
        protected IMLSpecifications.LearningType m_LearningType;
        [HideInInspector]
        public IMLSpecifications.LearningType LearningType { get => m_LearningType; }

        /// <summary>
        /// Keyboard flag to control node
        /// </summary>
        public bool EnableKeyboardControl;
        [HideInInspector]
        public KeyCode TrainingKey;
        [HideInInspector]
        public KeyCode RunningKey;

        protected IMLSpecifications.ModelStatus m_ModelStatus { get { return m_Model != null ? m_Model.ModelStatus : IMLSpecifications.ModelStatus.Untrained; } }
        /// <summary>
        /// The current status of the model
        /// </summary>
        public IMLSpecifications.ModelStatus ModelStatus { get => m_ModelStatus; }
        /// <summary>
        /// Flags that controls if the model should run or not
        /// </summary>
        protected bool m_Running;
        public bool Running { get { return m_Running; } }
        public bool Training { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Training); } }
        public bool Trained { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Trained); } }
        public bool Untrained { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Untrained); } }
        
        [HideInInspector]
        public IMLNodeTooltips tips;

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
        /// Series to run DTW on
        /// </summary>
        protected IMLTrainingSeries m_RunningSeries;

        /// <summary>
        /// Vector used to compute the realtime predictions in rapidlib based on the training data
        /// </summary>
        protected double[] m_RapidlibInputVector;
        /// <summary>
        /// Vector used to output the realtime predictions from rapidlib
        /// </summary>
        protected double[] m_RapidlibOutputVector;

        protected bool m_NodeConnectionChanged;
        protected int m_LastKnownRapidlibOutputVectorSize;

        /// <summary>
        /// Flag that controls if the iml model should be trained when entering/leaving playmode
        /// </summary>
        [HideInInspector]
        public bool TrainOnPlaymodeChange;

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

        private List<TrainingExamplesNode> oldTrainingExamplesNodes;

        #endregion

        #region XNode Messages

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            Initialize();

            tips = IMLTooltipsSerialization.LoadTooltip("Classification_MLS");

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
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
                                    case IMLSpecifications.DataTypes.SerialVector:
                                        return (PredictedOutput[i] as IMLSerialVector).GetValues();
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

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);

            m_NodeConnectionChanged = true;

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
            if(from.fieldName == "TrainingExamplesNodeToOutput") 
            {
                TrainingExamplesNode temp = from.node as TrainingExamplesNode;
                temp.IMLConfigurationNodesConnected.Add(this);
            }
            
        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);

            m_NodeConnectionChanged = true;

            UpdateOutputConfigList();

            // IF expected outputs don't match with dynamic output ports, a training examples node was disconnected
            if (m_ExpectedOutputList.Count != m_DynamicOutputPorts.Count)
            {
                // Refresh all dynamic output ports to the right size
                UpdateDynamicOutputPorts(IMLTrainingExamplesNodes, m_ExpectedOutputList , ref m_DynamicOutputPorts);
            }

            // We call logic for adapting arrays and inputs/outputs for ML models
            // Create input feature vector for realtime rapidlib predictions
            CreateRapidlibInputVector();

            // Create rapidlib predicted output vector
            CreateRapidLibOutputVector();
        }

        #endregion

        #region Unity Messages

        public void OnValidate()
        {
            // Checks that the rapidlib model is instanced (only if model is null)
            if (m_Model == null)
            {
                m_Model = InstantiateRapidlibModel(m_LearningType);
            }

            // Did the learning type change in the editor?
            if ((int)m_LearningType != (int)m_Model.TypeOfModel)
            {
                // Override model
                OverrideModel(m_LearningType);
            }

            // We call logic for adapting arrays and inputs/outputs for ML models
            // Create input feature vector for realtime rapidlib predictions
            CreateRapidlibInputVector();

            // Create rapidlib predicted output vector
            CreateRapidLibOutputVector();

            // Set error flags to false
            SetErrorFlags(false);
        }

        public void OnDestroy()
        {
            // Remove reference of this node in the IMLComponent controlling this node (if any)
            var MLController = graph as IMLController;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.DeleteIMLConfigurationNode(this);
            }
        }

#endregion

        #region Public Methods

        public void Initialize()
        {
            // Make sure the model is initialised properly
            if (m_Model == null)
            m_Model = InstantiateRapidlibModel(m_LearningType);

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
                m_DynamicOutputPorts = new List<NodePort>();

            m_NodeConnectionChanged = false;

            m_LastKnownRapidlibOutputVectorSize = 0;

            // We call logic for adapting arrays and inputs/outputs for ML models
            // Create input feature vector for realtime rapidlib predictions
            CreateRapidlibInputVector();

            // Create rapidlib predicted output vector
            CreateRapidLibOutputVector();

            //Add this node to list of IMLConfig nodes in all training nodes attached 
            foreach(TrainingExamplesNode node in IMLTrainingExamplesNodes)
            {
                if(!node.IMLConfigurationNodesConnected.Contains(this))
                    node.IMLConfigurationNodesConnected.Add(this);
            }
            // Specify the names for the nodeports
            m_TrainingExamplesNodeportName = "IMLTrainingExamplesNodes";
            m_LiveFeaturesNodeportName = "InputFeatures";
        }

        /// <summary>
        /// Instantiates a rapidlibmodel
        /// </summary>
        /// <param name="learningType"></param>
        public virtual RapidlibModel InstantiateRapidlibModel(IMLSpecifications.LearningType learningType)
        {
            RapidlibModel model = new RapidlibModel();
            switch (learningType)
            {
                case IMLSpecifications.LearningType.Classification:
                    model = new RapidlibModel(RapidlibModel.ModelType.kNN);
                    break;
                case IMLSpecifications.LearningType.Regression:
                    model = new RapidlibModel(RapidlibModel.ModelType.NeuralNetwork);
                    break;
                case IMLSpecifications.LearningType.DTW:
                    model = new RapidlibModel(RapidlibModel.ModelType.DTW);
                    break;
                default:
                    break;
            }
            return model;
        }

        public void UpdateLogic()
        {
            //Set Learning Type 
            SetLearningType();

            // Handle Input
            KeyboardInput();
            
            // Update Input Config List
            UpdateInputConfigList();

            // Update Output Config List from Training Examples Node
            UpdateOutputConfigList();

            // Update Number of Training Examples Connected
            UpdateTotalNumberTrainingExamples();

            // Make sure that current output format matches the expected output format
            UpdateOutputFormat();

            // Make sure dynamic output ports match the expected output format
            UpdateDynamicOutputPorts(IMLTrainingExamplesNodes, m_ExpectedOutputList, ref m_DynamicOutputPorts);

            // Perform running logic (it will account for DTW and Classification/Regression) only if there is a predicted output            
            RunningLogic();

            // Update feature selection matrix
            // TO DO
            /*Debug.Log("expected output" + m_ExpectedOutputList.Count);
            Debug.Log("predicted rap lib output" + PredictedRapidlibOutput.Length);
            Debug.Log("predicted output" + PredictedOutput.Count);*/
            // TO DO add some logic
            UpdateRapidLibOutputVector();

        }

        public virtual void TrainModel()
        {
            RunningLogic();
            // if there are no training examples in connected training nodes do not train 
           if(m_TotalNumTrainingData == 0)
            {
                Debug.Log("no training examples");
            }
            else
            {
                if (m_RapidlibTrainingSeriesCollection == null)
                    m_RapidlibTrainingSeriesCollection = new List<RapidlibTrainingSerie>();

                
                //TD Turn into two methods to be overriden in subclass 
                // If we have a dtw model...
                if (m_LearningType == IMLSpecifications.LearningType.DTW)
                {
                    m_RapidlibTrainingSeriesCollection = TransformIMLSeriesToRapidlib(IMLTrainingExamplesNodes);
                    m_Model.Train(m_RapidlibTrainingSeriesCollection);
                }
                // If it is a classification/regression model
                else
                {
                    // Transform the IML Training Examples into a format suitable for Rapidlib
                    m_RapidlibTrainingExamples = TransformIMLDataToRapidlib(IMLTrainingExamplesNodes);

                    // Trains rapidlib with the examples added
                    m_Model.Train(m_RapidlibTrainingExamples);

                    //Debug.Log("***Retraining IML Config node with num Examples: " + RapidLibComponent.trainingExamples.Count + " Rapidlib training succesful: " + RapidLibComponent.Trained + "***");
                }
            }
          
        }

        public void ToggleRunning()
        {
            // If the system is not running...
            if (!m_Running)
            {
                // Set flag to true if running inputs/outputs are not null and the model is trained!
                if (m_RapidlibInputVector != null && m_RapidlibOutputVector != null && Trained)
                {
                    UpdateInputVector();

                    m_Running = true;
                }
                else
                {
                    Debug.LogError("Rapidlib vectors for realtime predictions are null!");
                }
            }
            // If the system is already running...
            else
            {
                // If we are on DTW, we run the iteration at the end of the data collection period
                if (m_LearningType == IMLSpecifications.LearningType.DTW)
                {
                    string predictionDTW = RunModelDTW(m_RunningSeries);
                    // We clear running series for next run
                    m_RunningSeries.ClearSerie();
                    // We parse json into iml output
                    PredictedOutput = IMLDataSerialization.ParseJSONToIMLFeature(predictionDTW);

                    if (PredictedOutput != null && PredictedOutput.Count > 0)
                        Debug.Log("Predicted output is: " + PredictedOutput[0].Values[0]);
                }
                // Set flag to false
                m_Running = false;
                // Stop model
                m_Model.StopRunning();
            }
        }

        /// <summary>
        /// Returns the file name we want for the JSON, both for read and write
        /// </summary>
        /// <returns></returns>
        public string GetJSONFileName()
        {
            string graphName = this.graph.name;
            string nodeIndex = this.graph.nodes.FindIndex(a => a == this).ToString();
            string fileName = graphName + "_node_" + nodeIndex + "_" + "IMLConfiguration";
            return fileName;
        }

        /// <summary>
        /// Resets the rapidlibModel instance
        /// </summary>
        public void ResetModel()
        {
            // Take care of the RapidlibModel reference to this node            
            m_Model = InstantiateRapidlibModel(m_LearningType);

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
        /// Loads the current model from disk (dataPath specified in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public virtual void LoadModelFromDisk()
        {
            m_Model.LoadModelFromDisk(this.graph.name + "_IMLConfiguration" + this.id);
            // We update the node learning type to match the one from the loaded model
            switch (m_Model.TypeOfModel)
            {
                case RapidlibModel.ModelType.kNN:
                    m_LearningType = IMLSpecifications.LearningType.Classification;
                    // Configure inputs and outputs
                    PredictedOutput = new List<IMLBaseDataType>(m_Model.GetNumExpectedOutputs());
                    // TO DO
                    // Still left to configure inputs
                    // Still left to configure the type of the inputs and outputs
                    break;
                case RapidlibModel.ModelType.NeuralNetwork:
                    m_LearningType = IMLSpecifications.LearningType.Regression;
                    // Configure inputs and outputs
                    PredictedOutput = new List<IMLBaseDataType>(m_Model.GetNumExpectedOutputs());
                    // TO DO
                    // Still left to configure inputs
                    // Still left to configure the type of the inputs and outputs
                    break;
                case RapidlibModel.ModelType.DTW:
                    m_LearningType = IMLSpecifications.LearningType.DTW;
                    // DTW model will need to retrain!
                    Debug.Log("DTW RETRAINING WHEN LOADING MODEL NOT IMPLEMENTED YET!");
                    break;
                case RapidlibModel.ModelType.None:
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region protected Methods

        protected virtual void SetLearningType()
        {
            
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
            switch (m_LearningType)
            {
                // Classification and Regression
                case IMLSpecifications.LearningType.Classification: case IMLSpecifications.LearningType.Regression:                  
                    // Get the output from rapidlib
                    PredictedRapidlibOutput = RunModel();
                    // Don't run when the predicteb rapidlib output is empty (this is a patch for some bug that broke the predictions)
                    if (PredictedRapidlibOutput.Length == 0)
                        break;
                    // Transform rapidlib output to IMLTypes (calling straight after getting the output so that the UI can show properly)
                    TransformPredictedOuputToIMLTypes(PredictedRapidlibOutput, ref PredictedOutput);
                    break;
                case IMLSpecifications.LearningType.DTW:
                    //RunModelDTW(m_RunningSeries);
                    CollectFeaturesInRunningSeries(InputFeatures, ref m_RunningSeries);
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// Checks what is the output configuration 
        /// </summary>
        protected virtual bool CheckOutputConfiguration()
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
        }
        /// <summary>
        /// Checks what is the output configuration and creates a predicted output list in the correct format
        /// </summary>
        protected void UpdateOutputFormat()
        {
            // Make sure that the list is not null
            if (PredictedOutput ==  null)
                PredictedOutput = new List<IMLBaseDataType>();

            bool updateOutFormat = CheckOutputConfiguration();

            // If we are meant to update format...
            if (updateOutFormat)
            {
                // Save size of rapidlib vectorsize to work with it 
                // DIRTY CODE.  THIS SHOULD CHECK IF THE OUTPUT CONFIGURATION ACTUALLY DID CHANGE OR NOT. YOU COULD HAVE 2 DIFF OUTPUTS CONFIGS WITH SAME VECTOR SIZE
                if(PredictedRapidlibOutput != null)
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
                        case IMLSpecifications.OutputsEnum.SerialVector:
                            PredictedOutput.Add(new IMLSerialVector());
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
                    /* DIRTY CODE */
                }

            }

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
        /// Updates the configuration list of outputs
        /// </summary>
        protected void UpdateOutputConfigList()
        {
            oldTrainingExamplesNodes = IMLTrainingExamplesNodes;
            // Get values from the training example node
            IMLTrainingExamplesNodes = GetInputValues<TrainingExamplesNode>("IMLTrainingExamplesNodes").ToList();

            if (IMLTrainingExamplesNodes.Count < oldTrainingExamplesNodes.Count)
            {
                List<TrainingExamplesNode> list3 = oldTrainingExamplesNodes.Except(IMLTrainingExamplesNodes).ToList();
                foreach (TrainingExamplesNode node in list3)
                {
                    node.IMLConfigurationNodesConnected.Remove(this);
                }
            }

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
                    // Populate expected type from the trainin examples node connected
                    // dirty code needs rewriting 
                    if (m_ExpectedOutputList.Count == 0)
                    {
                        for (int i = m_ExpectedOutputList.Count; i < trainingExamplesNode.DesiredOutputsConfig.Count; i++)
                        {
                            m_ExpectedOutputList.Add(trainingExamplesNode.DesiredOutputsConfig[i]);
                        }
                    }
                     else if (trainingExamplesNode.DesiredOutputsConfig.Count > m_ExpectedOutputList.Count)
                    {
                        for (int i = m_ExpectedOutputList.Count-1; i < trainingExamplesNode.DesiredOutputsConfig.Count-1; i++)
                        {
                            m_ExpectedOutputList.Add(trainingExamplesNode.DesiredOutputsConfig[i]);
                        }
                    }

                    if (m_ExpectedOutputList.Count <= trainingExamplesNode.DesiredOutputsConfig.Count)
                    {
                        for (int i = 0; i < m_ExpectedOutputList.Count-1; i++)
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
        protected void AddDynamicOutputPorts(TrainingExamplesNode trainingExamples, ref List<NodePort> outputPorts)
        {
            // Make sure the dynamic port list is initialised
            if (outputPorts == null)
                outputPorts = new List<NodePort>();

            // Add as many output ports as we have output types in the training examples node. It will be drawn in the Editor class
            for (int i = 0; i < trainingExamples.DesiredOutputsConfig.Count; i++)
            {
                // Get reference to current expected output
                var expectedOutput = trainingExamples.DesiredOutputsConfig[i];

                // Add output port based on the type
                AddDynamicOutputPort(expectedOutput, ref outputPorts);
            }
        }
        
        /// <summary>
        /// Adds one dynamic output port based on its type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="outputPorts"></param>
        protected void AddDynamicOutputPort(IMLSpecifications.OutputsEnum type, ref List<NodePort> outputPorts)
        {
            // Make sure the dynamic port list is initialised
            if (outputPorts == null)
                outputPorts = new List<NodePort>();

            // Prepare output port
            NodePort dynamicOutputPort;
            // Add a specific kind of type for the output node depending on the expected type. The index will be the current length of the outputPorts list (since we are adding new ones, it will constantly increase)
            switch (type)
            {
                case IMLSpecifications.OutputsEnum.Float:
                    dynamicOutputPort = AddDynamicOutput(typeof(float), fieldName: "Output " + outputPorts.Count);
                    break;
                case IMLSpecifications.OutputsEnum.Integer:
                    dynamicOutputPort = AddDynamicOutput(typeof(int), fieldName: "Output " + outputPorts.Count);
                    break;
                case IMLSpecifications.OutputsEnum.Vector2:
                    dynamicOutputPort = AddDynamicOutput(typeof(Vector2), fieldName: "Output " + outputPorts.Count);
                    break;
                case IMLSpecifications.OutputsEnum.Vector3:
                    dynamicOutputPort = AddDynamicOutput(typeof(Vector3), fieldName: "Output " + outputPorts.Count);
                    break;
                case IMLSpecifications.OutputsEnum.Vector4:
                    dynamicOutputPort = AddDynamicOutput(typeof(Vector4), fieldName: "Output " + outputPorts.Count);
                    break;
                case IMLSpecifications.OutputsEnum.SerialVector:
                    dynamicOutputPort = AddDynamicOutput(typeof(float[]), fieldName: "Output " + outputPorts.Count);
                    break;
                default:
                    dynamicOutputPort = null;
                    break;
            }

            // If we got one output port to add, we add it
            if (dynamicOutputPort != null)
                outputPorts.Add(dynamicOutputPort);

        }

        /// <summary>
        /// Removes output ports from the list of dynamic output ports (the last batch added)
        /// </summary>
        /// <param name="traininingExamples"></param>
        /// <param name="outputPorts"></param>
        protected void RemoveDynamicOutputPorts(TrainingExamplesNode trainingExamples, ref List<NodePort> outputPorts)
        {
            // Make sure the dynamic port list is initialised
            if (outputPorts == null)
            {
                outputPorts = new List<NodePort>();
                // If we don't have any to remove, then just exit the method
                return;
            }

            // Select ports to remove. Assume that we will be removing the last output ports we added
            List<NodePort> portsToRemove = new List<NodePort>(outputPorts.GetRange(outputPorts.Count - (trainingExamples.DesiredOutputsConfig.Count) - 1, trainingExamples.DesiredOutputsConfig.Count));
            
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

        protected void UpdateDynamicOutputPorts(List<TrainingExamplesNode> trainingExamplesNodes, List<IMLSpecifications.OutputsEnum> expectedOutputs, ref List<NodePort> outputPorts)
        {
            // Make sure the dynamic port list is initialised
            if (outputPorts == null)
                outputPorts = new List<NodePort>();
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
                    outputPorts.RemoveRange((outputPorts.Count) - diffAbs, diffAbs);
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
        protected void CheckOutputFormatMatchesDynamicPort(IMLSpecifications.OutputsEnum dataType, ref NodePort outputPort)
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
                case IMLSpecifications.OutputsEnum.SerialVector:
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
        protected NodePort CheckOutputIndexMatchesDynamicPort(int expectedIndex, NodePort outputPort)
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
                    NodePort newPort = new NodePort(
                        "Output " + expectedIndex.ToString(),
                        outputPort.ValueType,
                        NodePort.IO.Output,
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

        protected virtual void OverrideModel(IMLSpecifications.LearningType learningType)
        {
            switch (learningType)
            {
                case IMLSpecifications.LearningType.Classification:
                    m_Model = new RapidlibModel(RapidlibModel.ModelType.kNN);
                    break;
                case IMLSpecifications.LearningType.Regression:
                    m_Model = new RapidlibModel(RapidlibModel.ModelType.NeuralNetwork);
                    break;
                case IMLSpecifications.LearningType.DTW:
                    m_Model = new RapidlibModel(RapidlibModel.ModelType.DTW);
                    break;
                default:
                    break;
            }
        }

        protected void UpdateTotalNumberTrainingExamples()
        {
            // Get training examples from the connected examples nodes
            IMLTrainingExamplesNodes = GetInputValues<TrainingExamplesNode>("IMLTrainingExamplesNodes").ToList();

            // The total number will start from 0 and keep adding the total amount of training examples from nodes connected
            m_TotalNumTrainingData = 0;
            if (!Lists.IsNullOrEmpty(ref IMLTrainingExamplesNodes))
            {
                for (int i = 0; i < IMLTrainingExamplesNodes.Count; i++)
                {
                    if (IMLTrainingExamplesNodes[i] != null)
                    {
                        // DTW
                        if (m_LearningType == IMLSpecifications.LearningType.DTW)
                            m_TotalNumTrainingData += IMLTrainingExamplesNodes[i].TrainingSeriesCollection.Count;
                        // classification/regression
                        else
                            m_TotalNumTrainingData += IMLTrainingExamplesNodes[i].TotalNumberOfTrainingExamples;
                    }
                    else
                    {
                        Debug.Log("Training Examples Node [" + i + "] is null!" );
                    }
                }
            }
        }

        /// <summary>
        /// Create the rapidlib training examples list in the required format
        /// </summary>
        protected List<RapidlibTrainingExample> TransformIMLDataToRapidlib(List<TrainingExamplesNode> trainingNodesIML)
        {
            // Create list to return
            List<RapidlibTrainingExample> rapidlibExamples = new List<RapidlibTrainingExample>();

            // Go through all the IML Training Examples if we can
            if (!Lists.IsNullOrEmpty(ref trainingNodesIML))
            {
                // Go through each node
                for (int i = 0; i < trainingNodesIML.Count; i++)
                {
                    // If there are training examples in this node...
                    if (!Lists.IsNullOrEmpty(ref trainingNodesIML[i].TrainingExamplesVector))
                    {
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
        protected List<RapidlibTrainingSerie> TransformIMLSeriesToRapidlib(List<TrainingExamplesNode> trainingNodesIML)
        {
            List<RapidlibTrainingSerie> seriesToReturn = new List<RapidlibTrainingSerie>();

            // Go through all the IML Training Examples if we can
            if (!Lists.IsNullOrEmpty(ref trainingNodesIML))
            {
                // Go through each node
                for (int i = 0; i < trainingNodesIML.Count; i++)
                {
                    // If there are training series in this node...
                    if (!Lists.IsNullOrEmpty(ref trainingNodesIML[i].TrainingSeriesCollection))
                    {
                        foreach (var IMLSeries in trainingNodesIML[i].TrainingSeriesCollection)
                        {
                            // Add each series to the rapidlib series list to return
                            seriesToReturn.Add(new RapidlibTrainingSerie(IMLSeries.GetSeriesFeatures(), IMLSeries.LabelSeries));
                        }
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

        protected virtual void CheckTrainingExamplesConnections(NodePort from, NodePort to, string portName)
        {            
            // Evaluate the nodeport for training examples
            if (to.fieldName == portName)
            {
                // Check if the node connected was a training examples node
                bool isNotTrainingExamplesNode = this.DisconnectIfNotType<IMLConfiguration, TrainingExamplesNode>(from, to);

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
                    // We check that the connection is from a training examples node
                    if (examplesNode != null)
                    {
                        // Update dynamic ports for output
                        AddDynamicOutputPorts(examplesNode, ref m_DynamicOutputPorts);
                    }
                }
            }
        }

        protected virtual void CheckInputFeaturesConnections(NodePort from, NodePort to, string portName)
        {
            // Evaluate nodeport for real-time features
            if (to.fieldName == portName)
            {
                // Only accept IFeaturesIML
                bool connectionBroken = this.DisconnectIfNotType<IMLConfiguration, IFeatureIML>(from, to);

                if (connectionBroken)
                {
                    // TO DO
                    // DISPLAY ERROR, not a feature connected
                }
            }
        }
       

        #endregion

    }
}
