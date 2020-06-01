using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using ReusableMethods;
using XNode.Examples.MathNodes;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

namespace InteractML
{
    /// <summary>
    /// Handles the logic of the different IML systems per graph
    /// </summary>
    [ExecuteInEditMode]
    public class IMLComponent : MonoBehaviour
    {

        #region Variables

        /// <summary>
        /// Reference to the IML Controller with nodes
        /// </summary>
        public IMLController MLController;
        private IMLController m_LastKnownIMLController;

        /// <summary>
        /// Scene where this IML Component belongs to
        /// </summary>
        private Scene m_OurScene;

        /// <summary>
        /// Collection of GameObjects that will be sent to the IML Controller
        /// </summary>
        [Header("GameObjects to Track")]
        [Tooltip("Add number of GameObjects to use in the IML Controller and what they are here")]
        [Rename("GameObject")]
        public List<GameObject> GameObjectsToUse;

        #region Private Lists of Nodes (Fields)
        /* Private Lists of nodes that we can have in the graph */
        private List<TextNote> m_TextNoteNodesList;
        private List<TrainingExamplesNode> m_TrainingExamplesNodesList;
        private List<IMLConfiguration> m_IMLConfigurationNodesList;
        private List<GameObjectNode> m_GameObjectNodeList;
        private List<RealtimeIMLOutputNode> m_RealtimeIMLOutputNodesList;
        public List<IFeatureIML> FeatureNodesList;
        #endregion

        #region Public Lists of Nodes (Properties)
        /// <summary>
        /// List of Training Example Nodes in the IML Controller
        /// </summary>
        public List<TrainingExamplesNode> TrainingExamplesNodesList
        {
            get
            {
                if (m_TrainingExamplesNodesList != null)
                    return m_TrainingExamplesNodesList;
                else
                    return new List<TrainingExamplesNode>();
            }
        }
        /// <summary>
        /// Lists of Model Nodes in the IML Controller
        /// </summary>
        public List<IMLConfiguration> IMLConfigurationNodesList
        {
            get
            {
                if (m_IMLConfigurationNodesList != null)
                    return m_IMLConfigurationNodesList;
                else
                    return new List<IMLConfiguration>();
            }
        }
        #endregion


        /// <summary>
        /// Have all the features been updated? (useful for features that need to update only once)
        /// </summary>
       [HideInInspector]
        public bool FeaturesUpdated;

        /// <summary>
        /// The Outputs from the IML Controller connected to this component
        /// </summary>
        [Header("IML Controller Realtime Outputs")]
        public List<double[]> IMLControllerOutputs;

        /// <summary>
        /// Add a Monobehaviour to this list and the IML Component will
        /// search for values marked with the "SendToIMLController" attribute
        /// </summary>
        //[Header("Components with IML Data to Fetch"), SerializeField]
        private List<MonoBehaviour> m_ComponentsWithIMLData;
        private Dictionary<MonoBehaviour, ScriptNode> m_MonoBehavioursPerScriptNode;
        private Dictionary<FieldInfo, IMLFieldInfoContainer> m_DataContainersPerFieldInfo;
        private Dictionary<FieldInfo, MonoBehaviour> m_DataMonobehavioursPerFieldInfo;



        #endregion

        #region Unity Messages

        // Called when something changes in the scene
        private void OnValidate()
        {
            IMLControllerOwnershipLogic();
        }

        // Awake is called before start
        private void Awake()
        {
            // We use the scene managers to make sure we flush the ownership of the controller when the scene loads/unloads
#if UNITY_EDITOR
            m_OurScene = EditorSceneManager.GetActiveScene();

            // Subscribe to the editor manager so that our update loop gets called
            IMLEditorManager.SubscribeIMLComponent(this);
#else
            m_OurScene = SceneManager.GetActiveScene();
#endif
        }

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            // If the app is running in the editor...
#if UNITY_EDITOR
            // Run logic only when the app is running. Otherwise the editor manager will handle it
            if (EditorApplication.isPlaying)
            {
                //Debug.Log("**RUNTIME**");
                UpdateLogic();
            }

            // If the app is running not in the editor...
#else
            // Run logic without troubles
            UpdateLogic();

#endif
        }

        // On Destroy gets called before the component is removed
        private void OnDestroy()
        {
            // We unsubscribe the component form the editor manager to avoid messing up with the list
            IMLEditorManager.UnsubscribeIMLComponent(this);
        }
#endregion

#region Private Methods

        private void Initialize()
        {
            // Initialise list of nodes for the IML Controller
            if (Lists.IsNullOrEmpty(ref GameObjectsToUse))
                GameObjectsToUse = new List<GameObject>();

            if (Lists.IsNullOrEmpty<TextNote>(ref m_TextNoteNodesList))
                m_TextNoteNodesList = new List<TextNote>();

            if (Lists.IsNullOrEmpty<TrainingExamplesNode>(ref m_TrainingExamplesNodesList))
                m_TrainingExamplesNodesList = new List<TrainingExamplesNode>();

            if (Lists.IsNullOrEmpty<IMLConfiguration>(ref m_IMLConfigurationNodesList))
                m_IMLConfigurationNodesList = new List<IMLConfiguration>();

            if (Lists.IsNullOrEmpty(ref m_GameObjectNodeList))
                m_GameObjectNodeList = new List<GameObjectNode>();

            if (Lists.IsNullOrEmpty(ref IMLControllerOutputs))
                IMLControllerOutputs = new List<double[]>();

            if (Lists.IsNullOrEmpty(ref FeatureNodesList))
                FeatureNodesList = new List<IFeatureIML>();


            // Get all th nodes which are in the graph
            GetAllNodes();

            // Init logic for training examples
            if (!Lists.IsNullOrEmpty(ref m_TrainingExamplesNodesList))
            {
                for (int i = 0; i < m_TrainingExamplesNodesList.Count; i++)
                {
                    var TrainingNode = m_TrainingExamplesNodesList[i];
                    if (TrainingNode == null)
                    {
                        Debug.LogError("Null reference in Training Examples list in IML System. The list is not calculated properly and has some null spaces!");
                    }
                    else
                    {
                        // Initialize Training Examples Node if not already initialized
                        m_TrainingExamplesNodesList[i].Initialize();
                    }


                }

            }

            // Init logic for IML Config node
            if (!Lists.IsNullOrEmpty(ref m_IMLConfigurationNodesList))
            {

                for (int i = 0; i < m_IMLConfigurationNodesList.Count; i++)
                {
                    var IMLConfigNode = m_IMLConfigurationNodesList[i];

                    if (IMLConfigNode == null)
                    {
                        Debug.LogError("Null reference in IML Config Node list in IML System. The list is not calculated properly and has some null spaces!");
                    }
                    else
                    {
                        // Initialize Training Examples Node if not already initialized
                        IMLConfigNode.Initialize();
                    }
                }
            }

            // Inject GameObjects to GameObject nodes
            SendGameObjectsToIMLController();

            updateGameObjectImage();

#if !UNITY_EDITOR
            // If we are not on the editor...
            if (Application.isPlaying)
            {               
                Debug.Log("RESET AND RETRAIN CALLED FROM IML COMPONENT");    
            
                // Reset all models
                ResetAllModels();

                // Retrain them
                ReTrainAllModels();

                // Run the models
                RunAllModels();
            }

#endif


            // We make sure that all null nodes in list are removed from the list
            if (MLController)
                MLController.nodes.RemoveAll(node => node == null);


        }

        /// <summary>
        /// Checks if an IMLController is owned and properly updates it when needed
        /// </summary>
        private void IMLControllerOwnershipLogic()
        {
            // Get reference to current scene
            Scene currentScene;
#if UNITY_EDITOR
            currentScene = EditorSceneManager.GetActiveScene();
#else
            currentScene = SceneManager.GetActiveScene();
#endif


            // We run this code only when the current scene is open
            if (m_OurScene == currentScene)
            {
                //Debug.Log("Our Scene is open!");
            }
            
            // If there is an IML Controller assigned
            if (MLController != null)
            {
                // If we don't have a memory a previous controller, we remember this one
                if (m_LastKnownIMLController == null)
                {
                    m_LastKnownIMLController = MLController;
                    // We also make sure to assign this IML Component as the one referenced in the IML controller
                    // So that nodes in the graph can know who updates them in the scene
                    MLController.SceneComponent = this;
                }
                // If the controller has changed, we make sure to flush wrong information
                else if (m_LastKnownIMLController != MLController)
                {
                    // We make sure we free the scene component reference in the previous controller to avoid information sent to the wrong place
                    m_LastKnownIMLController.SceneComponent = null;
                    // We remember the current controller
                    m_LastKnownIMLController = MLController;
                }
                // If the controller matches what we remember...
                else if (m_LastKnownIMLController == MLController)
                {
                    // We make sure the current controller is the right one
                    if (MLController.SceneComponent != this)
                    {
                        // Warn in the editor that the controller is being used by several IMLComponents
                        Debug.LogError("The referenced IML Controller is being used by more than one IML Component!");
                        MLController.SceneComponent = this;
                    }
                }
            }
            // If the MLController is null
            else
            {
                // We flush all the lists
                ClearLists();
            }
        }

        // this should only happen when a node is added 
        /// <summary>
        /// Finds all nodes in the IML Controller and puts them in lists of their types
        /// </summary>
        public void GetAllNodes ()
        {
            // Keep lists of nodes found updated
            if (MLController != null)
            {
                foreach (var node in MLController.nodes)
                {
                    // Feature nodes
                    CheckNodeIsFeature(node, ref FeatureNodesList);
                    
                    // GameObject nodes
                    CheckTypeAddNodeToList(node, ref m_GameObjectNodeList);

                    // Training Examples nodes
                    CheckNodeIsTraining(node, ref m_TrainingExamplesNodesList);

                    // IML Config Node
                    CheckNodeIsConfiguration(node, ref m_IMLConfigurationNodesList);

                    // Export output node
                    CheckTypeAddNodeToList(node, ref m_RealtimeIMLOutputNodesList);

                }

            }

        }

        /// <summary>
        /// Adds a node to the list passed in
        /// </summary>
        /// <param name="NodeType"></param>
        /// <param name="listToAddTo"></param>
        private void CheckTypeAddNodeToList<T>(XNode.Node nodeToAdd, ref List<T> listToAddTo)
        {

            // We don't update if the node is null
            if (nodeToAdd == null)
            {
                return;
            }
            Type p = nodeToAdd.GetType();
            // We first check that the node matches the type we want
            if (nodeToAdd.GetType() == typeof(T))
            {
                // Conver the node to that type
                T nodeToAddTyped = (T)System.Convert.ChangeType(nodeToAdd, typeof(T));
                // Make sure the list is not null
                if (listToAddTo == null)
                    listToAddTo = new List<T>();
                // If the list doesn't contain that specific node, we add it
                if (!listToAddTo.Contains(nodeToAddTyped))
                {
                    listToAddTo.Add(nodeToAddTyped);
                }
            }
        }

        private void CheckNodeIsTraining(XNode.Node nodeToAdd, ref List<TrainingExamplesNode> listToAddTo)
        {
            // We first check that the node ref is not null
            if (nodeToAdd != null)
            {
                // Then check that the node is a training examples node
                var trainingNode = nodeToAdd as TrainingExamplesNode;
                if (trainingNode != null)
                {
                    // Make sure the list is init
                    if (listToAddTo == null)
                        listToAddTo = new List<TrainingExamplesNode>();

                    // If we got a feature, we add it to the list (if it is not there already)
                    if (!listToAddTo.Contains(trainingNode))
                    {
                        listToAddTo.Add(trainingNode);
                    }

                }
            }

        }

        private void CheckNodeIsConfiguration(XNode.Node nodeToAdd, ref List<IMLConfiguration> listToAddTo)
        {
            // We first check that the node ref is not null
            if (nodeToAdd != null)
            {
                // Then check that the node is a configuratiton
                var configNode = nodeToAdd as IMLConfiguration;
                if (configNode != null)
                {
                    // Make sure the list is init
                    if (listToAddTo == null)
                        listToAddTo = new List<IMLConfiguration>();

                    // If we got a feature, we add it to the list (if it is not there already)
                    if (!listToAddTo.Contains(configNode))
                    {
                        listToAddTo.Add(configNode);
                    }

                }
            }

        }

        /// <summary>
        /// Adds a feauture node to the list of features
        /// </summary>
        /// <param name="nodeToAdd"></param>
        /// <param name="listToAddTo"></param>
        private void CheckNodeIsFeature(XNode.Node nodeToAdd, ref List<IFeatureIML> listToAddTo)
        {
            // We first check that the node ref is not null
            if (nodeToAdd != null)
            {
                // Then check that the node is a feature
                var featureNode = nodeToAdd as IFeatureIML;
                if (featureNode != null)
                {
                    // Make sure the list is init
                    if (listToAddTo == null)
                        listToAddTo = new List<IFeatureIML>();

                    // If we got a feature, we add it to the list (if it is not there already)
                    if (!listToAddTo.Contains(featureNode))
                    {
                        listToAddTo.Add(featureNode);
                    }
                    
                }
            }

        
        }

        private void RunFeaturesLogic()
        {
            if (FeatureNodesList == null)
                return;

            for (int i = 0; i < FeatureNodesList.Count; i++)
            {
                // Call the update logic per node ONLY IF THEY ARE UPDATABLE (for performance reasons)
                if (FeatureNodesList[i].isExternallyUpdatable)
                {
                    // Unlock the isUpdated flag so that all fields will be properly updated
                    FeatureNodesList[i].isUpdated = false;
                    // Update the feature
                    FeatureNodesList[i].UpdateFeature();
                    // Lock isUpdated so that later on calls don't recalculate certain values (as the velocity for instance)
                    FeatureNodesList[i].isUpdated = true;
                }
            }
        }

        /// <summary>
        /// Runs the logic for all the trainin examples nodes
        /// </summary>
        private void RunTraininExamplesLogic()
        {
            // Only run if we have the list of training examples
            if (m_TrainingExamplesNodesList == null)
                return;

            for (int i = 0; i < m_TrainingExamplesNodesList.Count; i++)
            {
                // Call the update logic per node
                m_TrainingExamplesNodesList[i].UpdateLogic();
            }
        }

        private void RunIMLConfigurationsLogic()
        {
            for (int i = 0; i < m_IMLConfigurationNodesList.Count; i++)
            {
                // If the node is null...
                if (m_IMLConfigurationNodesList[i] == null)
                {
                    // We call again GetAllNodes to make sure our list is updated
                    GetAllNodes();
                    // Break the for loop
                    break;
                }
                else
                {
                    // Call the update logic per node
                    m_IMLConfigurationNodesList[i].UpdateLogic();
                }
            }

        }

        /// <summary>
        /// Send all the gameobjects in the list to the 
        /// </summary>
        private void SendGameObjectsToIMLController()
        {
            //Debug.Log("GameObjects being injected to IML Controller");
            if (!Lists.IsNullOrEmpty(ref m_GameObjectNodeList) && !Lists.IsNullOrEmpty(ref GameObjectsToUse))
            {
                // Compare distance between both lists
                int distanceLists = GameObjectsToUse.Count - m_GameObjectNodeList.Count;
                // If they are the same length
                if (distanceLists == 0)
                {
                    // Assign each node a gameobject
                    for (int i = 0; i < m_GameObjectNodeList.Count; i++)
                    {
                        m_GameObjectNodeList[i].GameObjectDataOut = GameObjectsToUse[i];
                       // Debug.Log("Injecting GObject " + GameObjectsToUse[i].name);

                    }

                }
                // If there are more gObjects than nodes, we spawn a few extract nodes and add the gameobjects to them
                else if (distanceLists > 0)
                {
                    // Spawn as many nodes as distance we have
                    for (int i = 0; i < distanceLists; i++)
                    {
                        MLController.AddNode<GameObjectNode>();
                    }

                    // Refresh list of nodes
                    GetAllNodes();

                    // Assign each node a gameobject
                    for (int i = 0; i < m_GameObjectNodeList.Count; i++)
                    {
                        m_GameObjectNodeList[i].GameObjectDataOut = GameObjectsToUse[i];
                    }

                }
                // If there are more nodes than gameObjects
                else if (distanceLists < 0)
                {
                    // We warn the user
                    Debug.LogWarning("There are currently more GameObject Nodes in " + MLController.name + " than GameObjects referenced in the IML Component of " + this.name);

                    // Add as many GameObjects as we can to the list
                    for (int i = 0; i < GameObjectsToUse.Count; i++)
                    {
                        m_GameObjectNodeList[i].GameObjectDataOut = GameObjectsToUse[i];
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the outputs coming from the list of ExportOutputNodes
        /// </summary>
        private void ExtractOutputsIMLController()
        {
            // If the list is not created, we create one
            if (IMLControllerOutputs == null)
            {
                IMLControllerOutputs = new List<double[]>();
            }

            // If the list is not null or empty...
            if (!Lists.IsNullOrEmpty(ref m_RealtimeIMLOutputNodesList))
            {
                // If the size of the IML Controller outputs and the nodes found doesn't match, we make it match
                if (IMLControllerOutputs.Count != m_RealtimeIMLOutputNodesList.Count)
                {
                    // We clear all contents of the list
                    IMLControllerOutputs.Clear();
                    // We go through all the nodes exporting outputs
                    foreach (var outputNode in m_RealtimeIMLOutputNodesList)
                    {
                        var output = outputNode.GetIMLControllerOutputs();
                        // If the node has an output...
                        if (output != null)
                        {
                            // We add that output to the list
                            IMLControllerOutputs.Add(output);
                        }
                    }
                }
                // If it matches, we make sure to call the output node get method to update values
                else
                {
                    // We go through all the nodes exporting outputs
                    for (int i = 0; i < m_RealtimeIMLOutputNodesList.Count; i++)
                    {
                        var outputNode = m_RealtimeIMLOutputNodesList[i];
                        IMLControllerOutputs[i] = outputNode.GetIMLControllerOutputs();

                    }
                }
            }
            // If the output nodes list is null...
            else if (m_RealtimeIMLOutputNodesList == null)
            {
                // We create a new one
                m_RealtimeIMLOutputNodesList = new List<RealtimeIMLOutputNode>();
            }
        }

        /// <summary>
        /// Gets and sets the data marked with the "SendToIMLController" and "PullFromIMLController" attributes in Monobehaviours subscribed
        /// </summary>
        private void FetchDataFromMonobehavioursSubscribed()
        {
            if (m_ComponentsWithIMLData == null || m_ComponentsWithIMLData.Count == 0)
            {
                return;
            }
            foreach (var gameComponent in m_ComponentsWithIMLData)
            {
                /* ADD SCRIPT NODE */
                if (m_MonoBehavioursPerScriptNode == null)
                    m_MonoBehavioursPerScriptNode = new Dictionary<MonoBehaviour, ScriptNode>();

                // Check if the dictionary DOESN'T contain a fieldInfo for this reflected value, and then create nodes and dictionary values
                if (!m_MonoBehavioursPerScriptNode.ContainsKey(gameComponent))
                {
                    ScriptNode scriptNode = null;

                    // First, we try and see if the graph already contains a node we can use
                    foreach (var node in MLController.nodes)
                    {
                        // We see if this node is of the right type
                        if (node.GetType() == typeof(ScriptNode))
                        {
                            // We check if the node is available to use
                            var isTaken = m_MonoBehavioursPerScriptNode.Values.Any(container => container.Script == gameComponent);
                            // If the node is not taken...
                            if (!isTaken)
                            {
                                // This will be our node!
                                scriptNode = (ScriptNode)node;
                                // Stop searching for nodes
                                break;
                            }
                        }
                    }

                    // If we didn't find a suitable existing node...
                    if (scriptNode == null)
                    {
                        // Create a new script node into the graph
                        scriptNode = MLController.AddNode<ScriptNode>();
#if UNITY_EDITOR
                        // Save newnode to graph on disk                              
                        AssetDatabase.AddObjectToAsset(scriptNode, MLController);
                        // Reload graph into memory since we have modified it on disk
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(MLController));
#endif
                    }

                    // Configure our node appropiately
                    scriptNode.Script = gameComponent;
                    scriptNode.name = gameComponent.name + "(Script)";


                    // Add that to the dictionary            
                    m_MonoBehavioursPerScriptNode.Add(gameComponent, scriptNode);

                }

                // Gets all fields information from the game component (using System.Reflection)
                FieldInfo[] objectFields = gameComponent.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

                // Go through all the fields
                for (int i = 0; i < objectFields.Length; i++)
                {
                    var fieldToUse = objectFields[i];

                    // Check if the field is marked with the "SendToIMLController" attribute
                    SendToIMLController dataForIMLController = Attribute.GetCustomAttribute(fieldToUse, typeof(SendToIMLController)) as SendToIMLController;
                    // We check now if the field is marked with the "PullFromIMLController" attribute
                    PullFromIMLController dataFromIMLController = Attribute.GetCustomAttribute(fieldToUse, typeof(PullFromIMLController)) as PullFromIMLController;
                    // Define flags to identify attribute behaviour
                    bool isInputData = false, isOutputData = false;
                    // Update flags
                    if (dataForIMLController != null)
                        isInputData = true;
                    if (dataFromIMLController != null)
                        isOutputData = true;

                    // If the field is marked as either input or output...
                    if (isInputData || isOutputData)
                    {
                        // Debug type of that value in console
                        //Debug.Log(fieldToUse.Name + " Used in IMLComponent, With Type: " + fieldToUse.FieldType + ", With Value: " + fieldToUse.GetValue(gameComponent).ToString());

                        // Make sure that the dictionaries are initialised
                        if (m_DataMonobehavioursPerFieldInfo == null)
                            m_DataMonobehavioursPerFieldInfo = new Dictionary<FieldInfo, MonoBehaviour>();
                        if (m_DataContainersPerFieldInfo == null)
                            m_DataContainersPerFieldInfo = new Dictionary<FieldInfo, IMLFieldInfoContainer>();

                        // Check if the dictionary DOESN'T contain a fieldInfo for this reflected value, and then create nodes and dictionary values
                        if (!m_DataMonobehavioursPerFieldInfo.ContainsKey(fieldToUse))
                        {
                            // If it doesn't contain it, then we add it to both dictionaries
                            // Firstly, to keep track which gamecomponent belongs to which field info
                            m_DataMonobehavioursPerFieldInfo.Add(fieldToUse, gameComponent);
                            // Secondly, we create a node (based on its type) for this fieldInfo and add it to the dictionary
                            if (fieldToUse.FieldType == typeof(Single))
                            {
                                DataTypeNodes.FloatNode newNode = null;
                                // First, we try and see if the graph already contains a node we can use
                                foreach (var node in MLController.nodes)
                                {
                                    // We see if this node is of the right type
                                    if (node.GetType() == typeof(DataTypeNodes.FloatNode))
                                    {
                                        // We check if the node is available to use
                                        var isTaken = m_DataContainersPerFieldInfo.Values.Any(container => container.nodeForField == node);
                                        // If the node is not taken...
                                        if (!isTaken)
                                        {
                                            // This will be our node!
                                            newNode = (DataTypeNodes.FloatNode)node;
                                            // Stop searching for nodes
                                            break;
                                        }
                                    }
                                }

                                // If we didn't find a suitable existing node...
                                if (newNode == null)
                                {
                                    // Create a new Serial Vector node into the graph
                                    newNode = MLController.AddNode<DataTypeNodes.FloatNode>();
#if UNITY_EDITOR
                                    // Save newnode to graph on disk                              
                                    AssetDatabase.AddObjectToAsset(newNode, MLController);
                                    // Reload graph into memory since we have modified it on disk
                                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(MLController));
#endif                              
                                }

                                // Configure our node appropiately
                                newNode.Value = (float)fieldToUse.GetValue(gameComponent);
                                newNode.ValueName = fieldToUse.Name;
                                newNode.name = fieldToUse.Name + " (Float Node)";

                                // Add all info to a container
                                IMLFieldInfoContainer infoContainer = new IMLFieldInfoContainer(fieldToUse, newNode, IMLSpecifications.DataTypes.Float, gameComponent);
                                // Add that to the dictionary
                                m_DataContainersPerFieldInfo.Add(fieldToUse, infoContainer);

                            }
                            else if (fieldToUse.FieldType == typeof(Int32))
                            {
                                DataTypeNodes.IntegerNode newNode = null;
                                // First, we try and see if the graph already contains a node we can use
                                foreach (var node in MLController.nodes)
                                {
                                    // We see if this node is of the right type
                                    if (node.GetType() == typeof(DataTypeNodes.IntegerNode))
                                    {
                                        // We check if the node is available to use
                                        var isTaken = m_DataContainersPerFieldInfo.Values.Any(container => container.nodeForField == node);
                                        // If the node is not taken...
                                        if (!isTaken)
                                        {
                                            // This will be our node!
                                            newNode = (DataTypeNodes.IntegerNode)node;
                                            // Stop searching for nodes
                                            break;
                                        }
                                    }
                                }

                                // If we didn't find a suitable existing node...
                                if (newNode == null)
                                {
                                    // Create a new Serial Vector node into the graph
                                    newNode = MLController.AddNode<DataTypeNodes.IntegerNode>();
#if UNITY_EDITOR
                                    // Save newnode to graph on disk                              
                                    AssetDatabase.AddObjectToAsset(newNode, MLController);
                                    // Reload graph into memory since we have modified it on disk
                                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(MLController));
#endif
                                }

                                // Configure our node appropiately
                                newNode.Value = (int)fieldToUse.GetValue(gameComponent);
                                newNode.ValueName = fieldToUse.Name;
                                newNode.name = fieldToUse.Name + " (Int Node)";

                                // Add all info to a container
                                IMLFieldInfoContainer infoContainer = new IMLFieldInfoContainer(fieldToUse, newNode, IMLSpecifications.DataTypes.Integer, gameComponent);
                                // Add that to the dictionary
                                m_DataContainersPerFieldInfo.Add(fieldToUse, infoContainer);

                            }
                            else if (fieldToUse.FieldType == typeof(Vector2))
                            {
                                DataTypeNodes.Vector2Node newNode = null;
                                // First, we try and see if the graph already contains a node we can use
                                foreach (var node in MLController.nodes)
                                {
                                    // We see if this node is of the right type
                                    if (node.GetType() == typeof(DataTypeNodes.Vector2Node))
                                    {
                                        // We check if the node is available to use
                                        var isTaken = m_DataContainersPerFieldInfo.Values.Any(container => container.nodeForField == node);
                                        // If the node is not taken...
                                        if (!isTaken)
                                        {
                                            // This will be our node!
                                            newNode = (DataTypeNodes.Vector2Node)node;
                                            // Stop searching for nodes
                                            break;
                                        }
                                    }
                                }

                                // If we didn't find a suitable existing node...
                                if (newNode == null)
                                {
                                    // Create a new Serial Vector node into the graph
                                    newNode = MLController.AddNode<DataTypeNodes.Vector2Node>();
#if UNITY_EDITOR
                                    // Save newnode to graph on disk                              
                                    AssetDatabase.AddObjectToAsset(newNode, MLController);
                                    // Reload graph into memory since we have modified it on disk
                                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(MLController));
#endif

                                }

                                // Configure our node appropiately
                                newNode.Value = (Vector2)fieldToUse.GetValue(gameComponent);
                                newNode.ValueName = fieldToUse.Name;
                                newNode.name = fieldToUse.Name + " (Vector2 Node)";

                                // Add all info to a container
                                IMLFieldInfoContainer infoContainer = new IMLFieldInfoContainer(fieldToUse, newNode, IMLSpecifications.DataTypes.Vector2, gameComponent);
                                // Add that to the dictionary
                                m_DataContainersPerFieldInfo.Add(fieldToUse, infoContainer);

                            }
                            else if (fieldToUse.FieldType == typeof(Vector3))
                            {
                                DataTypeNodes.Vector3Node newNode = null;
                                // First, we try and see if the graph already contains a node we can use
                                foreach (var node in MLController.nodes)
                                {
                                    // We see if this node is of the right type
                                    if (node.GetType() == typeof(DataTypeNodes.Vector3Node))
                                    {
                                        // We check if the node is available to use
                                        var isTaken = m_DataContainersPerFieldInfo.Values.Any(container => container.nodeForField == node);
                                        // If the node is not taken...
                                        if (!isTaken)
                                        {
                                            // This will be our node!
                                            newNode = (DataTypeNodes.Vector3Node)node;
                                            // Stop searching for nodes
                                            break;
                                        }
                                    }
                                }

                                // If we didn't find a suitable existing node...
                                if (newNode == null)
                                {
                                    // Create a new Serial Vector node into the graph
                                    newNode = MLController.AddNode<DataTypeNodes.Vector3Node>();
#if UNITY_EDITOR
                                    // Save newnode to graph on disk                              
                                    AssetDatabase.AddObjectToAsset(newNode, MLController);
                                    // Reload graph into memory since we have modified it on disk
                                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(MLController));
#endif

                                }

                                // Configure our node appropiately
                                newNode.Value = (Vector3)fieldToUse.GetValue(gameComponent);
                                newNode.ValueName = fieldToUse.Name;
                                newNode.name = fieldToUse.Name + " (Vector3 Node)";

                                // Add all info to a container
                                IMLFieldInfoContainer infoContainer = new IMLFieldInfoContainer(fieldToUse, newNode, IMLSpecifications.DataTypes.Vector3, gameComponent);
                                // Add that to the dictionary
                                m_DataContainersPerFieldInfo.Add(fieldToUse, infoContainer);

                            }
                            else if (fieldToUse.FieldType == typeof(Vector4) || fieldToUse.FieldType == typeof(Quaternion))
                            {
                                DataTypeNodes.Vector4Node newNode = null;
                                // First, we try and see if the graph already contains a node we can use
                                foreach (var node in MLController.nodes)
                                {
                                    // We see if this node is of the right type
                                    if (node.GetType() == typeof(DataTypeNodes.Vector4Node))
                                    {
                                        // We check if the node is available to use
                                        var isTaken = m_DataContainersPerFieldInfo.Values.Any(container => container.nodeForField == node);
                                        // If the node is not taken...
                                        if (!isTaken)
                                        {
                                            // This will be our node!
                                            newNode = (DataTypeNodes.Vector4Node)node;
                                            // Stop searching for nodes
                                            break;
                                        }
                                    }
                                }

                                // If we didn't find a suitable existing node...
                                if (newNode == null)
                                {
                                    // Create a new Serial Vector node into the graph
                                    newNode = MLController.AddNode<DataTypeNodes.Vector4Node>();
#if UNITY_EDITOR
                                    // Save newnode to graph on disk                              
                                    AssetDatabase.AddObjectToAsset(newNode, MLController);
                                    // Reload graph into memory since we have modified it on disk
                                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(MLController));
#endif

                                }

                                // Configure our node appropiately
                                newNode.Value = (Vector4)fieldToUse.GetValue(gameComponent);
                                newNode.ValueName = fieldToUse.Name;
                                newNode.name = fieldToUse.Name + " (Vector4 Node)";

                                // Add all info to a container
                                IMLFieldInfoContainer infoContainer = new IMLFieldInfoContainer(fieldToUse, newNode, IMLSpecifications.DataTypes.Vector4, gameComponent);
                                // Add that to the dictionary
                                m_DataContainersPerFieldInfo.Add(fieldToUse, infoContainer);

                            }
                            else if (fieldToUse.FieldType == typeof(float[]))
                            {
                                DataTypeNodes.SerialVectorNode newNode = null;
                                // First, we try and see if the graph already contains a node we can use
                                foreach (var node in MLController.nodes)
                                {
                                    // We see if this node is of the right type
                                    if (node.GetType() == typeof(DataTypeNodes.SerialVectorNode))
                                    {
                                        // We check if the node is available to use
                                        var isTaken = m_DataContainersPerFieldInfo.Values.Any(container => container.nodeForField == node);
                                        // If the node is not taken...
                                        if (!isTaken)
                                        {
                                            // This will be our node!
                                            newNode = (DataTypeNodes.SerialVectorNode)node;
                                            // Stop searching for nodes
                                            break;
                                        }
                                    }
                                }

                                // If we didn't find a suitable existing node...
                                if (newNode == null)
                                {
                                    // Create a new Serial Vector node into the graph
                                    newNode = MLController.AddNode<DataTypeNodes.SerialVectorNode>();
#if UNITY_EDITOR
                                    // Save newnode to graph on disk                              
                                    AssetDatabase.AddObjectToAsset(newNode, MLController);
                                    // Reload graph into memory since we have modified it on disk
                                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(MLController));
#endif
                                }

                                // Configure our node appropiately
                                newNode.Value = (float[])fieldToUse.GetValue(gameComponent);
                                newNode.ValueName = fieldToUse.Name;
                                newNode.name = fieldToUse.Name + " (Serial Vector Node)";

                                // Add all info to a container
                                IMLFieldInfoContainer infoContainer = new IMLFieldInfoContainer(fieldToUse, newNode, IMLSpecifications.DataTypes.SerialVector, gameComponent);
                                // Add that to the dictionary
                                m_DataContainersPerFieldInfo.Add(fieldToUse, infoContainer);


                            }
                        }
                        // If the dictionary already contains a fieldInfo, update it
                        else if (true)
                        {
                            // Get the node linked to that field info
                            var dataContainer = m_DataContainersPerFieldInfo[fieldToUse];

                            // Detect the type of the node
                            switch (dataContainer.DataType)
                            {
                                // FLOAT
                                case IMLSpecifications.DataTypes.Float:
                                    // If it is input...
                                    if (isInputData)
                                        (dataContainer.nodeForField as DataTypeNodes.FloatNode).Value = (float)fieldToUse.GetValue(gameComponent);
                                    // If it is output...
                                    if (isOutputData)
                                        fieldToUse.SetValue(gameComponent, (dataContainer.nodeForField as DataTypeNodes.FloatNode).Value);
                                    break;
                                // INTEGER
                                case IMLSpecifications.DataTypes.Integer:
                                    // If it is input...
                                    if (isInputData)
                                        (dataContainer.nodeForField as DataTypeNodes.IntegerNode).Value = (int)fieldToUse.GetValue(gameComponent);
                                    // If it is output...
                                    if (isOutputData)
                                        fieldToUse.SetValue(gameComponent, (dataContainer.nodeForField as DataTypeNodes.IntegerNode).Value);
                                    break;
                                // VECTOR 2
                                case IMLSpecifications.DataTypes.Vector2:
                                    // If it is input...
                                    if (isInputData)
                                        (dataContainer.nodeForField as DataTypeNodes.Vector2Node).Value = (Vector2)fieldToUse.GetValue(gameComponent);
                                    // If it is output...
                                    if (isOutputData)
                                        fieldToUse.SetValue(gameComponent, (dataContainer.nodeForField as DataTypeNodes.Vector2Node).Value);
                                    break;
                                // VECTOR 3
                                case IMLSpecifications.DataTypes.Vector3:
                                    // If it is input...
                                    if (isInputData)
                                        (dataContainer.nodeForField as DataTypeNodes.Vector3Node).Value = (Vector3)fieldToUse.GetValue(gameComponent);
                                    // If it is output...
                                    if (isOutputData)
                                        fieldToUse.SetValue(gameComponent, (dataContainer.nodeForField as DataTypeNodes.Vector3Node).Value);
                                    break;
                                // VECTOR 4
                                case IMLSpecifications.DataTypes.Vector4:
                                    // If it is input...
                                    if (isInputData)
                                        (dataContainer.nodeForField as DataTypeNodes.Vector4Node).Value = (Vector4)fieldToUse.GetValue(gameComponent);
                                    // If it is output...
                                    if (isOutputData)
                                        fieldToUse.SetValue(gameComponent, (dataContainer.nodeForField as DataTypeNodes.Vector4Node).Value);
                                    break;
                                // SERIAL VECTOR
                                case IMLSpecifications.DataTypes.SerialVector:
                                    // If it is input...
                                    if (isInputData)
                                        (dataContainer.nodeForField as DataTypeNodes.SerialVectorNode).Value = (float[])fieldToUse.GetValue(gameComponent);
                                    // If it is output...
                                    if (isOutputData)
                                        fieldToUse.SetValue(gameComponent, (dataContainer.nodeForField as DataTypeNodes.SerialVectorNode).Value);
                                    break;
                                // DEFAULT SWITCH
                                default:
                                    break;
                            }
                        }

                    }
                }
            }
        }

        #endregion

        #region Public Methods

        [ContextMenu("Clear Lists (Use in Case of null ref errors)")]
        public void ClearLists()
        {
            // Training Examples list
            if (m_TrainingExamplesNodesList != null)
                m_TrainingExamplesNodesList.Clear();
            else
                m_TrainingExamplesNodesList = new List<TrainingExamplesNode>();

            // IML Config node List
            if (m_IMLConfigurationNodesList != null)
                m_IMLConfigurationNodesList.Clear();
            else
                m_IMLConfigurationNodesList = new List<IMLConfiguration>();

            // RealtimeIMLOutPutNodes List
            if (m_RealtimeIMLOutputNodesList != null)
                m_RealtimeIMLOutputNodesList.Clear();
            else
                m_RealtimeIMLOutputNodesList = new List<RealtimeIMLOutputNode>();

        }

        /// <summary>
        /// Update logic loop that will be called inside Unity's Update event
        /// </summary>
        public void UpdateLogic()
        {
            //Debug.Log("Running IMLComponent update...");


            //            Debug.Log("Update loop running...");

            //            // Get reference to current scene
            //            Scene currentScene;
            //#if UNITY_EDITOR
            //            currentScene = EditorSceneManager.GetActiveScene();
            //#else
            //            currentScene = SceneManager.GetActiveScene();
            //#endif

            //#if UNITY_EDITOR
            //            m_OurScene = EditorSceneManager.GetActiveScene();
            //#else
            //                m_OurScene = SceneManager.GetActiveScene();
            //#endif



            //            Debug.Log("Current scene is: " + currentScene.name);
            //            Debug.Log("Our Scene is: " + m_OurScene.name);



            //            // We run this code only when the current scene is open
            //            if (m_OurScene == currentScene)
            //            {
            //                Debug.Log("Our Scene is open!");
            //            }


            // Keep lists of nodes found updated
            GetAllNodes();

            if (MLController != null)
            {
                // Fetch data from the Monobehaviours we have subscribed into and out of the IML Controller
                FetchDataFromMonobehavioursSubscribed();

                // Run logic for all feature nodes
                RunFeaturesLogic();

                // Run logic for all training example nodes
                RunTraininExamplesLogic();

                // Run logic for all IML Config nodes
                RunIMLConfigurationsLogic();

                // Get all IML Controller outputs
                ExtractOutputsIMLController();

            }

        }

        /// <summary>
        /// Public function that updates the game objects in the graph
        /// </summary>
        public void UpdateGameObjectsInIMLController()
        {
            SendGameObjectsToIMLController();
        }

        [ContextMenu("Delete All Models")]
        public void DeleteAllModels()
        {
            //Debug.Log("Delete All Models Called");

            // I tried deleting the IML Config nodes, but that gave errors when controlling Rapidlib! Avoiding to do that at all.

//            // Destroy all rapidlib components attached to this gameobject (since we don't need them any more)
//            var rapidlibsInGameObject = this.GetComponents<RapidLib>();

//            foreach (var rapidlibComponent in rapidlibsInGameObject)
//            {
//#if UNITY_EDITOR
//                DestroyImmediate(rapidlibComponent);
//#else
//                    Destroy(rapidlibComponent);
//#endif

//            }
        }

        /// <summary>
        /// Reload all models from disk (when possible)
        /// </summary>
        public void LoadAllModelsFromDisk()
        {
            foreach (var IMLConfigNode in m_IMLConfigurationNodesList)
            {
                // Loads the model in the IMLConfigNode
                IMLConfigNode.LoadModelFromDisk();
            }
            
        }

        /// <summary>
        /// Saves all models to disk (when possible)
        /// </summary>
        public void SaveAllModels()
        {
            foreach (var IMLConfigNode in m_IMLConfigurationNodesList)
            {
                // Save model to disk
                IMLConfigNode.SaveModelToDisk();
            }

        }

        /// <summary>
        /// Stops all models if they are running
        /// </summary>
        public void StopAllModels()
        {
            foreach (var IMLConfigNode in m_IMLConfigurationNodesList)
            {
                // Stop model if they are running
                if (IMLConfigNode.Running)
                    IMLConfigNode.ToggleRunning();
            }
        }

        /// <summary>
        /// Resets all models for the IML Config nodes (by destroying and re-creating them)
        /// </summary>
        public void ResetAllModels()
        {
            //Debug.Log("Resetting all models...");

            DeleteAllModels();

            //// Go through the list of iml config nodes and instantiate new rapidlibs
            //foreach (var imlConfigNode in IMLConfigurationNodesList)
            //{
            //    if (imlConfigNode)
            //    {
            //        imlConfigNode.InstantiateRapidlibModel();                    
            //    }
            //}
        }

        /// <summary>
        /// Retrains all the models (currently launching a coroutine)
        /// </summary>
        public void ReTrainAllModels()
        {
            Debug.Log("**Retraining all models...**");

            //// COROUTINE APPROACH
            IEnumerator coroutineToStart = ReTrainAllModelsCoroutine();
            StartCoroutine(coroutineToStart);
        }

        /// <summary>
        /// Runs all models that are marked with RunOnAwake
        /// </summary>
        public void RunAllModels()
        {
            foreach (var imlConfigNode in m_IMLConfigurationNodesList)
            {
                if (imlConfigNode)
                {
                    // Only eun if the flag is marked to do so
                    if (imlConfigNode.RunOnAwake)
                        imlConfigNode.ToggleRunning();
                }
            }

        }

        /// <summary>
        /// Coroutine that will retrain all the models (one per frame, because training is expensive)
        /// </summary>
        /// <returns></returns>
        IEnumerator ReTrainAllModelsCoroutine()
        {
            yield return new WaitForSeconds(0.05f);
            foreach (var imlConfigNode in m_IMLConfigurationNodesList)
            {
                if (imlConfigNode)
                {
                    // Only retrains if the flag is marked to do so
                    if (imlConfigNode.TrainOnPlaymodeChange)
                        imlConfigNode.TrainModel();
                    yield return null;
                }
            }
            
        }

        /// <summary>
        /// Pass a Monobehaviour and mark any field with "SendToIMLController" or "PullFromIMLController" attribute to use it with the IML Component
        /// </summary>
        /// <param name="gameComponent"></param>
        public void SubscribeToIMLController(MonoBehaviour gameComponent)
        {
            if (gameComponent == null)
            {
                Debug.LogError("Monobehaviour passed is null!");
                return;
            }
            else
            {
                if (m_ComponentsWithIMLData == null)
                    m_ComponentsWithIMLData = new List<MonoBehaviour>();

                if (!m_ComponentsWithIMLData.Contains(gameComponent))
                {
                    m_ComponentsWithIMLData.Add(gameComponent);
                }
            }
        }

        /// <summary>
        /// Unsubscribes Monobehaviour from the list to use with the IMLController
        /// </summary>
        /// <param name="gameComponent"></param>
        public void UnsubscribeFromIMLController(MonoBehaviour gameComponent)
        {
            if (gameComponent == null)
            {
                Debug.LogError("Monobehaviour passed is null!");
                return;
            }
            else
            {
                if (m_ComponentsWithIMLData == null)
                    m_ComponentsWithIMLData = new List<MonoBehaviour>();

                if (m_ComponentsWithIMLData.Contains(gameComponent))
                {
                    // Before removing, make sure that the dictionaries and the IML Controller are updated with the changes
                    if (m_DataMonobehavioursPerFieldInfo.ContainsValue(gameComponent))
                    {                        
                        foreach (var entry in m_DataMonobehavioursPerFieldInfo)
                        {
                            if (entry.Value == gameComponent)
                            {
                                FieldInfo fieldInfoToDelete = entry.Key;
                                // Use the value of this entry to remove data from other dictionary
                                if (m_DataContainersPerFieldInfo.ContainsKey(fieldInfoToDelete))
                                {
                                    // Go through container dictionary and remove entries that match with our field info to delete
                                    foreach (var entry2 in m_DataContainersPerFieldInfo)
                                    {
                                        if (entry2.Key == fieldInfoToDelete)
                                        {
                                            // Remove Node from IML Controller before deleting the entry
                                            MLController.RemoveNode(entry2.Value.nodeForField);
                                            // Remove fieldInfo/DataContainer entry from dictionary
                                            m_DataContainersPerFieldInfo.Remove(entry2.Key);
                                        }
                                    }
                                }
                                // Remove Monobehaviour/fieldInfo entry from dictionary
                                m_DataMonobehavioursPerFieldInfo.Remove(entry.Key);
                            }
                        }
                    }
                    
                    // After all dictionaries are updated (and the ML grpah as well), remove gameComponent from list
                    m_ComponentsWithIMLData.Remove(gameComponent);
                }
            }

        }

        /// <summary>
        /// Removes GameObjectNode From GameObjectNodeList 
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteGameObjectNode(GameObjectNode nodeToDelete)
        {
            if (m_GameObjectNodeList.Contains(nodeToDelete))
                m_GameObjectNodeList.Remove(nodeToDelete);

        }
        /// <summary>
        /// Removes IMLConfigurationNode From IMLConfigurationNodeList 
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteIMLConfigurationNode(IMLConfiguration nodeToDelete)
        {
            if (m_IMLConfigurationNodesList.Contains(nodeToDelete))
                m_IMLConfigurationNodesList.Remove(nodeToDelete);
        }
        /// <summary>
        /// Removes RealtimeOutputNode From RealtimeOutputNodeList 
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteRealtimeIMLOutputNode(RealtimeIMLOutputNode nodeToDelete)
        {
            if (m_RealtimeIMLOutputNodesList.Contains(nodeToDelete))
                m_RealtimeIMLOutputNodesList.Remove(nodeToDelete);
        }
        /// <summary>
        /// Removes TrainingExamplesNode From TrainingExamplesNodeList 
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteTrainingExamplesNode(TrainingExamplesNode nodeToDelete)
        {
            if (m_TrainingExamplesNodesList.Contains(nodeToDelete))
                m_TrainingExamplesNodesList.Remove(nodeToDelete);
        }
        /// <summary>
        /// Removes TextNoteNode From TextNoteNodeList 
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteTextNoteNode(TextNote nodeToDelete)
        {
            if (m_TextNoteNodesList.Contains(nodeToDelete))
                m_TextNoteNodesList.Remove(nodeToDelete);
        }
        /// <summary>
        /// Removes TextNoteNode From TextNoteNodeList 
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteFeatureNode(IFeatureIML nodeToDelete)
        {
            if (FeatureNodesList.Contains(nodeToDelete))
                FeatureNodesList.Remove(nodeToDelete);
        }
        //Very dirty code need to sort it out in a better way - updates the node pointer when exiting or entering play mode 
        public void updateGameObjectImage()
        {
            
            foreach (GameObjectNode GONode in m_GameObjectNodeList)
            {
                GONode.state = true;
            }
        }

        #endregion

        #region SceneLoading

#if UNITY_EDITOR
        private void SceneOpenedLogic(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
        {
            Debug.Log("SceneOpened detected by IMLComponent");
        }      

#endif

#endregion

    }

}
