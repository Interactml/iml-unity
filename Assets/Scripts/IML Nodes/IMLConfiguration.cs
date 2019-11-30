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
    public class IMLConfiguration : Node
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
        public List<IMLBaseDataType> PredictedOutput;

        [HideInInspector]
        public double[] PredictedRapidlibOutput;
        [HideInInspector]
        public double[] DelayedPredictedOutput;

        /// <summary>
        /// Total updated number of training examples connected to this IML Configuration Node
        /// </summary>
        private int m_TotalNumTrainingExamples;
        public int TotalNumTrainingExamples { get { return m_TotalNumTrainingExamples; } }

        /// <summary>
        /// List of expected inputs
        /// </summary>
        [SerializeField]
        private List<IMLSpecifications.InputsEnum> m_ExpectedInputList;
        public List<IMLSpecifications.InputsEnum> ExpectedInputList { get { return m_ExpectedInputList; } }

        /// <summary>
        /// List of expected outputs
        /// </summary>
        [SerializeField]
        private List<IMLSpecifications.OutputsEnum> m_ExpectedOutputList;
        public List<IMLSpecifications.OutputsEnum> ExpectedOutputList { get { return m_ExpectedOutputList; } }


        /// <summary>
        /// The learning type of this model
        /// </summary>
        [SerializeField]
        private IMLSpecifications.LearningType m_LearningType;

        private IMLSpecifications.ModelStatus m_ModelStatus { get { return m_Model != null ? m_Model.ModelStatus : IMLSpecifications.ModelStatus.Untrained; } }
        /// <summary>
        /// The current status of the model
        /// </summary>
        public IMLSpecifications.ModelStatus ModelStatus { get => m_ModelStatus; }
        /// <summary>
        /// Flags that controls if the model should run or not
        /// </summary>
        private bool m_Running;
        public bool Running { get { return m_Running; } }
        public bool Training { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Training); } }
        public bool Trained { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Trained); } }
        public bool Untrained { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Untrained); } }


        /// <summary>
        /// Reference to the rapidlib model this node is holding
        /// </summary>
        private RapidlibModel m_Model;
        /// <summary>
        /// Public reference to the rapidlib model this node is holding
        /// </summary>
        public RapidlibModel Model { get => m_Model; }

        /// <summary>
        /// The list of training examples that we will pass to rapidlib in the correct format
        /// </summary>
        private List<RapidlibTrainingExample> m_RapidlibTrainingExamples;
        /// <summary>
        /// Private list of rapidlib training series collection (for dtw)
        /// </summary>
        private List<RapidlibTrainingSerie> m_RapidlibTrainingSeriesCollection;
        /// <summary>
        /// Vector used to compute the realtime predictions in rapidlib based on the training data
        /// </summary>
        private double[] m_RapidlibInputVector;
        /// <summary>
        /// Vector used to output the realtime predictions from rapidlib
        /// </summary>
        private double[] m_RapidlibOutputVector;

        private bool m_NodeConnectionChanged;
        private int m_LastKnownRapidlibOutputVectorSize;

        /// <summary>
        /// Flag that controls if the iml model should be trained when entering/leaving playmode
        /// </summary>
        public bool TrainOnPlaymodeChange;

        /// <summary>
        /// Flag that controls if the iml model should run when the game awakes 
        /// </summary>
        public bool RunOnAwake;

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
            return this; 
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);

            m_NodeConnectionChanged = true;
        }

        public override void OnRemoveConnection(NodePort port)
        {
            base.OnRemoveConnection(port);

            m_NodeConnectionChanged = true;
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
        }

        public void OnDestroy()
        {
            // Remove reference of this node in the IMLComponent controlling this node (if any)
            var MLController = graph as IMLController;
            if (MLController.SceneComponent != null)
            {
                MLController.SceneComponent.IMLConfigurationNodesList.Remove(this);
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

            m_NodeConnectionChanged = false;

            m_LastKnownRapidlibOutputVectorSize = 0;
        }

        /// <summary>
        /// Instantiates a rapidlibmodel
        /// </summary>
        /// <param name="learningType"></param>
        public RapidlibModel InstantiateRapidlibModel(IMLSpecifications.LearningType learningType)
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
            // Transform rapidlib output to IMLTypes (needed to be called the first thing to work properly)
            TransformPredictedOuputToIMLTypes();

            // Update Input Config List
            UpdateInputConfigList();

            // Create input feature vector for realtime rapidlib predictions
            CreateRapidlibInputVector();

            // Update Output Format
            UpdateOutputFormat();

            // Create rapidlib predicted output vector
            CreateRapidLibOutputVector();

            // Update Number of Training Examples Connected
            UpdateTotalNumberTrainingExamples();

            // Get the output from rapidlib
            PredictedRapidlibOutput = RunModel();

            // Transform rapidlib output to IMLTypes (needed to be called the first thing to work properly)
            TransformPredictedOuputToIMLTypes();

            // Update feature selection matrix
            // TO DO


        }

        public void TrainModel()
        {
            if (m_RapidlibTrainingSeriesCollection == null)
                m_RapidlibTrainingSeriesCollection = new List<RapidlibTrainingSerie>();

            // Transform the IML Training Examples into a format suitable for Rapidlib
            m_RapidlibTrainingExamples = TransformIMLDataToRapidlib(IMLTrainingExamplesNodes);

            // Trains rapidlib with the examples added
            m_Model.Train(m_RapidlibTrainingExamples);

            //Debug.Log("***Retraining IML Config node with num Examples: " + RapidLibComponent.trainingExamples.Count + " Rapidlib training succesful: " + RapidLibComponent.Trained + "***");

  
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
        public void SaveModelToDisk(string fileName)
        {
            m_Model.SaveModelToDisk(this.name + fileName);
        }

        /// <summary>
        /// Loads the current model from disk (dataPath specified in IMLDataSerialization)
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadModelFromDisk(string fileName)
        {
            m_Model.LoadModelFromDisk(this.name + fileName);
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
                    Debug.LogError("DTW RETRAINING WHEN LOADING MODEL NOT IMPLEMENTED YET!");
                    break;
                case RapidlibModel.ModelType.None:
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Private Methods

        public void TransformPredictedOuputToIMLTypes()
        {
            int pointerRawOutputVector = 0;
            // Transform the raw vector output into list of iml types
            // Go through each output feature (preconfigured previously)
            foreach (var outputFeature in PredictedOutput)
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

                    if (i + pointerRawOutputVector > PredictedRapidlibOutput.Length)
                    {
                        Debug.LogError("The rapidlib output vector is too small!");
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
        private double[] RunModel()
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
        /// Checks what is the output configuration and creates a predicted output list in the correct format
        /// </summary>
        private void UpdateOutputFormat()
        {
            // Make sure that the list is not null
            if (PredictedOutput ==  null)
                PredictedOutput = new List<IMLBaseDataType>();

            // Only changed this when there is a change in nodes
            if (m_LastKnownRapidlibOutputVectorSize == 0 || m_LastKnownRapidlibOutputVectorSize != PredictedRapidlibOutput.Length || m_NodeConnectionChanged)
            {
                // Save size of rapidlib vectorsize to work with it 
                // DIRTY CODE.  THIS SHOULD CHECK IF THE OUTPUT CONFIGURATION ACTUALLY DID CHANGE OR NOT. YOU COULD HAVE 2 DIFF OUTPUTS CONFIGS WITH SAME VECTOR SIZE
                m_LastKnownRapidlibOutputVectorSize = PredictedRapidlibOutput.Length;
                // Adjust the desired outputs list based on configuration selected
                PredictedOutput.Clear();
                // Calculate required space for outputs
                for (int i = 0; i < m_ExpectedOutputList.Count; i++)
                {
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
                }

            }

        }

        /// <summary>
        /// Updates the configuration list of inputs
        /// </summary>
        private void UpdateInputConfigList()
        {
            // Get values from the input list
            InputFeatures = GetInputValues<Node>("InputFeatures").ToList();

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

        private void OverrideModel(IMLSpecifications.LearningType learningType)
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

        private void UpdateTotalNumberTrainingExamples()
        {
            // Get training examples from the connected examples nodes
            IMLTrainingExamplesNodes = GetInputValues<TrainingExamplesNode>("IMLTrainingExamplesNodes").ToList();

            // The total number will start from 0 and keep adding the total amount of training examples from nodes connected
            m_TotalNumTrainingExamples = 0;
            if (!Lists.IsNullOrEmpty(ref IMLTrainingExamplesNodes))
            {
                for (int i = 0; i < IMLTrainingExamplesNodes.Count; i++)
                {
                    if (IMLTrainingExamplesNodes[i] != null)
                    {
                        m_TotalNumTrainingExamples += IMLTrainingExamplesNodes[i].TotalNumberOfTrainingExamples;
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
        private List<RapidlibTrainingExample> TransformIMLDataToRapidlib(List<TrainingExamplesNode> trainingNodesIML)
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

                                    //// INPUT VECTOR RAPIDLIB
                                    //int rapidlibInputVectorSize = 0;
                                    //// Go through all input features in IML Training Example and get their size
                                    //for (int k = 0; k < IMLTrainingExample.Inputs.Count; k++)
                                    //{
                                    //    rapidlibInputVectorSize += IMLTrainingExample.Inputs[k].InputData.Values.Length;
                                    //}
                                    //// Create rapidlib input by constructing a vector size of all IML input features combined
                                    //newExample.Input = new double[rapidlibInputVectorSize];
                                    //// Create a pointer to know keep the boundaries in our input vector while we add IML features
                                    //int pointerVector = 0;
                                    //// Go through all IML input features and add their features to the rapidlib vector
                                    //for (int k = 0; k < IMLTrainingExample.Inputs.Count; k++)
                                    //{
                                    //    // Check that the boundaries are never surpassed
                                    //    if (pointerVector > rapidlibInputVectorSize)
                                    //    {
                                    //        Debug.LogError("Trying to add input features to a rapidlib vector that is too small!");
                                    //    }
                                    //    var IMLInputFeature = IMLTrainingExample.Inputs[k];
                                    //    // Add IML data to rapidlib vector
                                    //    for (int w = 0; w < IMLInputFeature.InputData.Values.Length; w++)
                                    //    {
                                    //        newExample.Input[w + pointerVector] = IMLInputFeature.InputData.Values[w];
                                    //    }
                                    //    // Move vector pointer forward
                                    //    pointerVector += IMLInputFeature.InputData.Values.Length;
                                    //}

                                    //// OUTPUT VECTOR RAPIDLIB
                                    //int rapidlibOutputVectorSize = 0;
                                    //// Go through all Output features in IML Training Example and get their size
                                    //for (int k = 0; k < IMLTrainingExample.Outputs.Count; k++)
                                    //{
                                    //    rapidlibOutputVectorSize += IMLTrainingExample.Outputs[k].OutputData.Values.Length;
                                    //}
                                    //// Create rapidlib Output by constructing a vector size of all IML Output features combined
                                    //newExample.Output = new double[rapidlibOutputVectorSize];
                                    //// Create a pointer to know keep the boundaries in our Output vector while we add IML features
                                    //pointerVector = 0;
                                    //// Go through all IML Output features and add their features to the rapidlib vector
                                    //for (int k = 0; k < IMLTrainingExample.Outputs.Count; k++)
                                    //{
                                    //    // Check that the boundaries are never surpassed
                                    //    if (pointerVector > rapidlibOutputVectorSize)
                                    //    {
                                    //        Debug.LogError("Trying to add Output features to a rapidlib vector that is too small!");
                                    //    }
                                    //    var IMLOutputFeature = IMLTrainingExample.Outputs[k];
                                    //    // Add IML data to rapidlib vector
                                    //    for (int w = 0; w < IMLOutputFeature.OutputData.Values.Length; w++)
                                    //    {
                                    //        newExample.Output[w + pointerVector] = IMLOutputFeature.OutputData.Values[w];
                                    //    }
                                    //    // Move vector pointer forward
                                    //    pointerVector += IMLOutputFeature.OutputData.Values.Length;
                                    //}

                                    //// ADD EXAMPLE TO list to return
                                    //rapidlibExamples.Add(newExample);

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
        /// Creates the rapidlib input vector (input for the realtime predictions)
        /// </summary>
        private void CreateRapidlibInputVector()
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
        private void CreateRapidLibOutputVector()
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

        /// <summary>
        /// Updates the input vector to send to rapidlib with the input features in the IML Config node
        /// </summary>
        private void UpdateInputVector()
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

#endregion

    }
}
