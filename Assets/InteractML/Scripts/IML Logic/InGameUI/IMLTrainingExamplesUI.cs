using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using ReusableMethods;

namespace InteractML
{
    /// <summary>
    /// Child class. Handles communication with a specific Training Examples Node
    /// </summary>
    public class IMLTrainingExamplesUI : IMLUI<TrainingExamplesNode>
    {
        #region Variables
        [Header("Training Examples Node Setup")]
        public TextMeshProUGUI CollectExamplesButtonText;
        public TextMeshProUGUI TrainingExamplesNodeIndexText;
        public TextMeshProUGUI NoTrainingExamplesText;
        public TextMeshProUGUI NoInputsText;
        public RectTransform InputsContentRect; // Should be a Parent for every other data type UI element for inputs
        public TextMeshProUGUI NoOutputsText;
        public RectTransform OutputsContentRect; // Should be a Parent for every other data type UI element for outputs

        [Header("Button Setup")]
        public Button MoveNextNodeButton;
        public Button MovePrevNodeButton;
        public Button RemoveOneOutputButton;
        public Button AddOneOutputButton;

        [Header("Data Type UI Prefabs")]
        [Tooltip("Populate with UI Prefabs of your data types")]
        public List<GameObject> DataTypeUIPrefabs;
        /// <summary>
        /// List containing input data from the training examples node
        /// </summary>
        private List<IMLDataTypeUI> m_InputData;
        /// <summary>
        /// List containing output data from the training examples node
        /// </summary>
        private List<IMLDataTypeUI> m_OutputData;

        /// <summary>
        /// Cache of input data from the training examples node
        /// </summary>
        private List<IMLBaseDataType> m_NodeInputData;
        /// <summary>
        /// Cache of the output data from the training examples node
        /// </summary>
        private List<IMLBaseDataType> m_NodeOutputData;


        [Header("Training Examples Node Vars")]
        [SerializeField]
        private int m_NoDesiredOutputsNode;

        #endregion

        #region Unity Messages
        // Start is called before the first frame update
        void Start()
        {
            // Find a reference of an IML Component to work with
            if (MLComponent == null)
                MLComponent = FindObjectOfType<IMLComponent>();

            if (MLComponent)
                m_CurrentNode = GetNode(m_NodeIndex, MLComponent.TrainingExamplesNodesList);

            // Init lists
            if (m_InputData == null)
                m_InputData = new List<IMLDataTypeUI>();

            if (m_OutputData == null)
                m_OutputData = new List<IMLDataTypeUI>();

            // Setup Buttons
            // Next node
            if (MoveNextNodeButton)
                AddOnClickButtonCall(MoveNextNodeButton, delegate { MoveToNextNode(); }, true);
            // Previous node
            if (MovePrevNodeButton)
                AddOnClickButtonCall(MovePrevNodeButton, delegate { MoveToPreviousNode(); }, true);
            // Add output
            if (AddOneOutputButton)
                AddOnClickButtonCall(AddOneOutputButton, delegate { AddOneOutput(); }, true);
            // Remove output
            if (RemoveOneOutputButton)
                AddOnClickButtonCall(RemoveOneOutputButton, delegate { RemoveOneOutput(); }, true);

        }

        // Update is called once per frame
        void Update()
        {
            // Attempt to get node
            if (!m_CurrentNode)
                m_CurrentNode = GetNode(m_NodeIndex, MLComponent.TrainingExamplesNodesList);

            // Update UI that can change from the IML Controller
            if (m_CurrentNode)
            {
                // Node Index
                UpdateUIText(TrainingExamplesNodeIndexText, m_NodeIndex.ToString());
                // No Training Examples
                UpdateUIText(NoTrainingExamplesText, m_CurrentNode.TotalNumberOfTrainingExamples.ToString());
                // Inputs
                UpdateUIText(NoInputsText, m_CurrentNode.InputFeatures.Count.ToString());
                // No Outputs
                UpdateUIText(NoOutputsText, m_CurrentNode.DesiredOutputFeatures.Count.ToString());

                // Check if it is collecting examples to update button text
                if (m_CurrentNode.CollectingData)
                {
                    UpdateUIText(CollectExamplesButtonText, "STOP Recording Examples");
                }
                else
                {
                    UpdateUIText(CollectExamplesButtonText, "Record Examples");
                }

                // Populate inputs in List
                UpdateDataList(ref m_InputData, m_CurrentNode.InputFeatures, ref m_NodeInputData, DataTypeUIPrefabs, "Input", InputsContentRect);

                // Populate outputs in List
                UpdateDataList(ref m_OutputData, m_CurrentNode.DesiredOutputFeatures, ref m_NodeOutputData, DataTypeUIPrefabs, "Output", OutputsContentRect);
            }

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Toggles connection of examples
        /// </summary>
        public void ToggleCollectingTrainingExamples()
        {
            if (!MLComponent)
                return;

            if (!m_CurrentNode)
                m_CurrentNode = GetNode(m_NodeIndex, MLComponent.TrainingExamplesNodesList);

            ToggleCollectingExamples(m_CurrentNode);
        }

        /// <summary>
        /// Records only ONE example
        /// </summary>
        public void RecordSingleExample()
        {
            if (!MLComponent)
                return;

            if (!m_CurrentNode)
                m_CurrentNode = GetNode(m_NodeIndex, MLComponent.TrainingExamplesNodesList);

            m_CurrentNode.AddSingleTrainingExample();
        }

        /// <summary>
        /// Removes all training examples
        /// </summary>
        public void DeleteAllTrainingExamples()
        {
            if (!MLComponent)
                return;

            if (!m_CurrentNode)
                m_CurrentNode = GetNode(m_NodeIndex, MLComponent.TrainingExamplesNodesList);

            m_CurrentNode.ClearTrainingExamples();

        }

        /// <summary>
        /// Exposes publicly parent method to move to next node
        /// </summary>
        public void MoveToNextNode()
        {
            MoveToNextNode(MLComponent.TrainingExamplesNodesList);
        }

        /// <summary>
        /// Exposes publicly parent method to move to previous node
        /// </summary>
        public void MoveToPreviousNode()
        {
            MoveToPreviousNode(MLComponent.TrainingExamplesNodesList);
        }

        /// <summary>
        /// Adds one item to the outputs list
        /// </summary>
        public void AddOneOutput()
        {
            if (m_CurrentNode)
            {
                if (m_CurrentNode.DesiredOutputsConfig != null)
                {
                    // Adds a new desired output (for the moment a float)
                    IMLSpecifications.OutputsEnum newOutput = new IMLSpecifications.OutputsEnum();
                    newOutput = IMLSpecifications.OutputsEnum.Float;
                    m_CurrentNode.DesiredOutputsConfig.Add(newOutput);
                }
            }
        }

        /// <summary>
        /// Removes last item in the desired output list
        /// </summary>
        public void RemoveOneOutput()
        {
            if (m_CurrentNode)
            {
                if (m_CurrentNode.DesiredOutputsConfig != null && m_CurrentNode.DesiredOutputsConfig.Count > 0)
                    // Removes last item added to the desired outputs list
                    m_CurrentNode.DesiredOutputsConfig.RemoveAt(m_CurrentNode.DesiredOutputsConfig.Count - 1);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Toggles the collection of training examples in a specific node
        /// </summary>
        /// <param name="whichNode"></param>
        private void ToggleCollectingExamples(int whichNode)
        {
            if (MLComponent)
            {
                if (MLComponent.TrainingExamplesNodesList.Count > 0)
                {
                    MLComponent.TrainingExamplesNodesList[whichNode].ToggleCollectExamples();
                    if (CollectExamplesButtonText)
                    {
                        if (MLComponent.TrainingExamplesNodesList[whichNode].CollectingData)
                        {
                            CollectExamplesButtonText.text = "STOP Recording Examples";
                        }
                        else
                        {
                            CollectExamplesButtonText.text = "Record Examples";
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Toggles the collection of training examples in a specific node
        /// </summary>
        private void ToggleCollectingExamples(TrainingExamplesNode node)
        {
            node.ToggleCollectExamples();
        }

        /// <summary>
        /// Updates a list of data in the in-game UI
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="nodeDataList"></param>
        private void UpdateDataList<T>(ref List<IMLDataTypeUI> dataList, List<T> externalDataList, ref List<IMLBaseDataType> nodeDataListCache, List<GameObject> dataUIPrefabs, string label, Transform parent)
        {
            // Don't run the method if the external data list is null
            if (externalDataList == null)
                return;

            // Do we need to update the nodeDataListCache?
            if (nodeDataListCache == null || nodeDataListCache.Count != externalDataList.Count)
            {
                // Clear previous node list
                nodeDataListCache = new List<IMLBaseDataType>();

                // Check what type the external data list is, to cast a new list of the right type we need            
                // IMLBaseDataType
                if (externalDataList is List<IMLBaseDataType>)
                {
                    nodeDataListCache = externalDataList.CastNewList<T, IMLBaseDataType>();
                }
                // XNode.Node
                else if (externalDataList is List<XNode.Node>)
                {
                    List<XNode.Node> xnodeList = externalDataList.CastNewList<T, XNode.Node>();
                    List<IFeatureIML> featureDataList = xnodeList.CastNewList<XNode.Node, IFeatureIML>();
                    // Loop through features to get the right information
                    foreach (var item in featureDataList)
                    {
                        nodeDataListCache.Add(item.FeatureValues);
                    }
                }
                // Anything else
                else
                {
                    throw new System.Exception("Class " + typeof(T).ToString() + " is not yet supported in the UI!");
                }
            }


            // Make sure local data list is init
            if (dataList == null)
                dataList = new List<IMLDataTypeUI>();

            // If we need to resize the local data list...
            if (dataList.Count != nodeDataListCache.Count)
            {
                dataList.Resize(nodeDataListCache.Count, destroyItems: true);
            }

            // Make sure we are both lists configurations and structures are matching
            for (int i = 0; i < nodeDataListCache.Count; i++)
            {
                var externalData = nodeDataListCache[i];
                var internalData = dataList[i];                

                // We break the method if external data is null or any prefabs are null
                if (externalData == null )
                    return;

                // Flag that will trigger the reconfigure local data type event
                bool reconfigureData = false;

                // Check for null first
                if (internalData == null)
                {
                    reconfigureData = true;
                }
                // Check for mismatch of data type
                else if (internalData.DataType != externalData.DataType)
                {
                    reconfigureData = true;
                }

                // If we need to reconfigure the local data...
                if (reconfigureData)
                {

                    GameObject prefabClone = null;
                    // Create the right kind of data and update values
                    switch (externalData.DataType)
                    {
                        case IMLSpecifications.DataTypes.Float:
                            prefabClone = Instantiate(dataUIPrefabs[0], parent);
                            break;
                        case IMLSpecifications.DataTypes.Integer:
                            prefabClone = Instantiate(dataUIPrefabs[1], parent);
                            break;
                        case IMLSpecifications.DataTypes.Vector2:
                            prefabClone = Instantiate(dataUIPrefabs[2], parent);
                            break;
                        case IMLSpecifications.DataTypes.Vector3:
                            prefabClone = Instantiate(dataUIPrefabs[3], parent);
                            break;
                        case IMLSpecifications.DataTypes.Vector4:
                            prefabClone = Instantiate(dataUIPrefabs[4], parent);
                            break;
                        case IMLSpecifications.DataTypes.SerialVector:
                            throw new System.Exception("Serial Vector not yet supported in in-game UI!");
                            break;
                        default:
                            break;
                    }

                    // If we managed to clone the prefab correctly, get internal data from prefab clone
                    if (prefabClone)
                    {
                        // If there is anything in the internalData slot, we destroy it
                        if (internalData)
                            Destroy(internalData.gameObject);
                        // Override the internal data slot with the new prefab clone
                        internalData = prefabClone.GetComponent<IMLDataTypeUI>();

                    }
                    // If not, abort method
                    else
                    {
                        throw new System.Exception("Error when reconfiguring data type on UI, a prefab was null!");
                        return;
                    }

                    // Update label
                    internalData.Label.text = label + " " + i.ToString();

                    // Replace element in local list
                    dataList[i] = internalData;
                }

                // Update local values
                UpdateUIDataFields(internalData.InputField, externalData.Values);


            }

        }

        #endregion
    }

}
