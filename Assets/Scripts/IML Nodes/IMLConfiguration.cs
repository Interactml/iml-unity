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

        private IMLSpecifications.ModelStatus m_ModelStatus;
        /// <summary>
        /// The current status of the model
        /// </summary>
        public IMLSpecifications.ModelStatus ModelStatus { get { return m_ModelStatus; } }
        public bool Running { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Running); } }
        public bool Training { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Training); } }
        public bool Trained { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Trained); } }
        public bool Untrained { get { return (m_ModelStatus == IMLSpecifications.ModelStatus.Untrained); } }

        /// <summary>
        /// Reference to the rapidlib component in this training examples node
        /// </summary>
        [SerializeField, HideInInspector]
        public RapidLib RapidLibComponent;
        /// <summary>
        /// The list of training examples that we will pass to rapidlib in the correct format
        /// </summary>
        private List<RapidlibTrainingExample> m_RapidlibExamples;
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
            InstantiateRapidlib();
        }

        public void OnDestroy()
        {
            // Destroy rapidlib connected to avoid stacking them in memory
            if (RapidLibComponent != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(RapidLibComponent);
#else
                Destroy(RapidLibComponent);
#endif
                RapidLibComponent = null;
            }

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
            // Make sure that rapidlib allows external data to be injected
            if (RapidLibComponent != null)
                RapidLibComponent.AllowExternalData = true;
            
            // Init lists
            if (Lists.IsNullOrEmpty(ref IMLTrainingExamplesNodes))
                IMLTrainingExamplesNodes = new List<TrainingExamplesNode>();

            if (Lists.IsNullOrEmpty(ref PredictedOutput))
                PredictedOutput = new List<IMLBaseDataType>();

            if (Lists.IsNullOrEmpty(ref m_ExpectedInputList))
                m_ExpectedInputList = new List<IMLSpecifications.InputsEnum>();

            if (Lists.IsNullOrEmpty(ref m_ExpectedOutputList))
                m_ExpectedOutputList = new List<IMLSpecifications.OutputsEnum>();

            if (Lists.IsNullOrEmpty(ref m_RapidlibExamples))
                m_RapidlibExamples = new List<RapidlibTrainingExample>();


            m_NodeConnectionChanged = false;

            m_LastKnownRapidlibOutputVectorSize = 0;
        }

        public void InstantiateRapidlib()
        {
            // If the rapidlib reference is not there, create it
            if (RapidLibComponent == null)
            {
                var IMLGraph = (graph as IMLController);
                // If we have an IML Component where to create the rapidlib component...
                if (IMLGraph.SceneComponent != null)
                {
                    // Create the rapidlib component in that gameobject
                    RapidLibComponent = IMLGraph.SceneComponent.gameObject.AddComponent<RapidLib>();
                    // Configure it to allow external data
                    RapidLibComponent.AllowExternalData = true;

                    //Debug.Log("CREATING RAPIDLIB REFERENCE");
                }
            }

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

            // Update learning type
            UpdateLearningType();

            // Update Training Status
            UpdateModelStatus();

            // Update Number of Training Examples Connected
            UpdateTotalNumberTrainingExamples();

            // Get the output from rapidlib
            UpdatePredictedOutput();

            // Transform rapidlib output to IMLTypes (needed to be called the first thing to work properly)
            TransformPredictedOuputToIMLTypes();

            // Update feature selection matrix
            // TO DO


        }

        public void TrainModel()
        {
            if (RapidLibComponent)
            {
                if (RapidLibComponent.trainingExamples != null)
                {
                    // Flush previous examples contained in the model
                    if (RapidLibComponent.trainingExamples.Count > 0)
                        RapidLibComponent.ClearTrainingExamples();
                }
                else
                {
                    // We make sure that the training examples are not null
                    RapidLibComponent.trainingExamples = new List<TrainingExample>();
                }

                // Create and add the training examples (all inside the method)
                CreateRapidlibTrainingExamples();

                // Trains rapidlib with the examples added
                RapidLibComponent.Train();

                //Debug.Log("***Retraining IML Config node with num Examples: " + RapidLibComponent.trainingExamples.Count + " Rapidlib training succesful: " + RapidLibComponent.Trained + "***");

            }
        }

        public void ToggleRunning()
        {
            if (m_RapidlibInputVector != null && m_RapidlibOutputVector != null)
            {
                UpdateInputVector();

                RapidLibComponent.InjectExternalData(m_RapidlibInputVector, m_RapidlibOutputVector, GetJSONFileName());

                RapidLibComponent.ToggleRunning();
            }
            else
            {
                Debug.LogError("Rapidlib vectors for realtime predictions are null!");
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
        /// Resets the rapidlib instance (by destroying all instances and creating a new one)
        /// </summary>
        public void ResetModel()
        {
            // TO DO: Take care of only the Rapidlib reference to this node (not working at the moment, relying on a general graph call from IML Component)            

            // Call IML Component reset all models 
            var controllerGraph = (graph as IMLController);
            if (controllerGraph)
            {
                controllerGraph.SceneComponent.ResetAllModels();
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
        /// Updates the predicted output from the rapidlib predictions
        /// </summary>
        private void UpdatePredictedOutput()
        {
            if (RapidLibComponent != null)
            {
                // Update input vector to inject
                UpdateInputVector();

                RapidLibComponent.InjectExternalData(m_RapidlibInputVector, m_RapidlibOutputVector, GetJSONFileName());

                if (DelayedPredictedOutput == null || DelayedPredictedOutput.Length != PredictedRapidlibOutput.Length)
                {
                    DelayedPredictedOutput = new double[PredictedRapidlibOutput.Length];
                }
                PredictedRapidlibOutput.CopyTo(DelayedPredictedOutput, 0);

                // Get Outputs from rapidlib
                PredictedRapidlibOutput = RapidLibComponent.GetOutputs();

            }
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

        private void UpdateLearningType()
        {
            if (RapidLibComponent)
            {
                switch (m_LearningType)
                {
                    case IMLSpecifications.LearningType.Classification:
                        RapidLibComponent.learningType = RapidLib.LearningType.Classification;
                        break;
                    case IMLSpecifications.LearningType.Regression:
                        RapidLibComponent.learningType = RapidLib.LearningType.Regression;
                        break;
                    case IMLSpecifications.LearningType.DTW:
                        RapidLibComponent.learningType = RapidLib.LearningType.DTW;
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateModelStatus()
        {

            if (RapidLibComponent)
            {

                if (RapidLibComponent.Running)
                {
                    m_ModelStatus = IMLSpecifications.ModelStatus.Running;
                }
                else if (RapidLibComponent.Training)
                {
                    m_ModelStatus = IMLSpecifications.ModelStatus.Training;
                }
                else if (RapidLibComponent.Trained)
                {
                    m_ModelStatus = IMLSpecifications.ModelStatus.Trained;
                }
                else
                {
                    m_ModelStatus = IMLSpecifications.ModelStatus.Untrained;
                }
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
        private void CreateRapidlibTrainingExamples()
        {
            // If the list to create is not null...
            if (m_RapidlibExamples != null)
            {
                // Go through all the IML Training Examples if we can
                if (!Lists.IsNullOrEmpty(ref IMLTrainingExamplesNodes))
                {
                    // Go through each node
                    for (int i = 0; i < IMLTrainingExamplesNodes.Count; i++)
                    {
                        // If there are training examples in this node...
                        if (!Lists.IsNullOrEmpty(ref IMLTrainingExamplesNodes[i].TrainingExamplesVector))
                        {
                            // Go through all the training examples
                            for (int j = 0; j < IMLTrainingExamplesNodes[i].TrainingExamplesVector.Count; j++)
                            {
                                // Check that individual example is not null
                                var IMLTrainingExample = IMLTrainingExamplesNodes[i].TrainingExamplesVector[j];
                                if (IMLTrainingExample != null)
                                {
                                    // Check that inputs/outputs are not null
                                    if (IMLTrainingExample.Inputs != null && IMLTrainingExample.Outputs != null)
                                    {
                                        // Add example to rapidlib
                                        if (RapidLibComponent != null)
                                        {
                                            TrainingExample newExample = new TrainingExample();

                                            // INPUT VECTOR RAPIDLIB
                                            int rapidlibInputVectorSize = 0;
                                            // Go through all input features in IML Training Example and get their size
                                            for (int k = 0; k < IMLTrainingExample.Inputs.Count; k++)
                                            {
                                                rapidlibInputVectorSize += IMLTrainingExample.Inputs[k].InputData.Values.Length;
                                            }
                                            // Create rapidlib input by constructing a vector size of all IML input features combined
                                            newExample.Input = new double[rapidlibInputVectorSize];
                                            // Create a pointer to know keep the boundaries in our input vector while we add IML features
                                            int pointerVector = 0;
                                            // Go through all IML input features and add their features to the rapidlib vector
                                            for (int k = 0; k < IMLTrainingExample.Inputs.Count; k++)
                                            {
                                                // Check that the boundaries are never surpassed
                                                if (pointerVector > rapidlibInputVectorSize)
                                                {
                                                    Debug.LogError("Trying to add input features to a rapidlib vector that is too small!");
                                                }
                                                var IMLInputFeature = IMLTrainingExample.Inputs[k];
                                                // Add IML data to rapidlib vector
                                                for (int w = 0; w < IMLInputFeature.InputData.Values.Length; w++)
                                                {
                                                    newExample.Input[w + pointerVector] = IMLInputFeature.InputData.Values[w];
                                                }
                                                // Move vector pointer forward
                                                pointerVector += IMLInputFeature.InputData.Values.Length;
                                            }

                                            // OUTPUT VECTOR RAPIDLIB
                                            int rapidlibOutputVectorSize = 0;
                                            // Go through all Output features in IML Training Example and get their size
                                            for (int k = 0; k < IMLTrainingExample.Outputs.Count; k++)
                                            {
                                                rapidlibOutputVectorSize += IMLTrainingExample.Outputs[k].OutputData.Values.Length;
                                            }
                                            // Create rapidlib Output by constructing a vector size of all IML Output features combined
                                            newExample.Output = new double[rapidlibOutputVectorSize];
                                            // Create a pointer to know keep the boundaries in our Output vector while we add IML features
                                            pointerVector = 0;
                                            // Go through all IML Output features and add their features to the rapidlib vector
                                            for (int k = 0; k < IMLTrainingExample.Outputs.Count; k++)
                                            {
                                                // Check that the boundaries are never surpassed
                                                if (pointerVector > rapidlibOutputVectorSize)
                                                {
                                                    Debug.LogError("Trying to add Output features to a rapidlib vector that is too small!");
                                                }
                                                var IMLOutputFeature = IMLTrainingExample.Outputs[k];
                                                // Add IML data to rapidlib vector
                                                for (int w = 0; w < IMLOutputFeature.OutputData.Values.Length; w++)
                                                {
                                                    newExample.Output[w + pointerVector] = IMLOutputFeature.OutputData.Values[w];
                                                }
                                                // Move vector pointer forward
                                                pointerVector += IMLOutputFeature.OutputData.Values.Length;
                                            }


                                            // ADD EXAMPLE TO RAPIDLIB
                                            RapidLibComponent.AddTrainingExample(newExample);

                                        }
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
            }
            // In case it is null, we make sure to initialize the class
            else
            {
                Initialize();
            }
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
