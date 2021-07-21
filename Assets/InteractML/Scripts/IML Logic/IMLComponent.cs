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
using InteractML.ControllerCustomisers;
#endif

namespace InteractML
{
    /// <summary>
    /// Handles the logic of the different IML systems per graph
    /// </summary>
    public class IMLComponent : MonoBehaviour
    {

        #region Variables

        /// <summary>
        /// Reference to the IML Controller with nodes
        /// </summary>
        public IMLGraph graph;
        private IMLGraph m_LastKnownGraph;

        /// <summary>
        /// Scene where this IML Component belongs to
        /// </summary>
        private Scene m_OurScene;

        /// <summary>
        /// Collection of GameObjects that will be sent to the IML Graph
        /// </summary>
        [Header("GameObjects to Track")]
        [Tooltip("Add number of GameObjects to use in the IML Graph and what they are here")]
        [Rename("GameObject")]
        public List<GameObject> GameObjectsToUse;
        /// <summary>
        /// Dictionary to hold references of GameObjects and which GONode manages them
        /// </summary>
        [SerializeField, HideInInspector]
        private GOPerGONodeDictionary m_GOsPerGONodes;

        /// <summary>
        /// bool to check whether MLS nodes are loaded before running
        /// </summary>
        private bool nodesLoaded;

        #region Private Lists of Nodes (Fields)
        /* Private Lists of nodes that we can have in the graph */
        private List<TextNote> m_TextNoteNodesList;
        private List<TrainingExamplesNode> m_TrainingExamplesNodesList;
        private List<MLSystem> m_MLSystemNodeList;
        [SerializeField, HideInInspector]
        private List<GameObjectNode> m_GameObjectNodeList;
        public List<IFeatureIML> FeatureNodesList;
        [SerializeField, HideInInspector]
        private List<ScriptNode> m_ScriptNodesList;
        //[SerializeField, HideInInspector]
        private InteractML.ControllerCustomisers.InputSetUp m_inputSetUp;
        [SerializeField, HideInInspector]
        private List<CustomController> m_CustomControllerList;
        public List<Type> inputTypes;

        #endregion

        #region Public Lists of Nodes (Properties)
        [HideInInspector]
        public InteractML.ControllerCustomisers.InputSetUp inputSetUp { get => m_inputSetUp; }
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
        public List<MLSystem> MLSystemNodeList
        {
            get
            {
                if (m_MLSystemNodeList != null)
                    return m_MLSystemNodeList;
                else
                    return new List<MLSystem>();
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
        [Header("Scripts to Track")]
        [Tooltip("Add number of Scripts to use in the IML Graph and what they are here")]
        public List<IMLMonoBehaviourContainer> ComponentsWithIMLData;
        /// <summary>
        /// Dictionary to hold references of components with IML Data and which scriptNode manages them
        /// </summary>
        [SerializeField, HideInInspector]
        private MonobehaviourScriptNodeDictionary m_MonoBehavioursPerScriptNode;
        /// <summary>
        /// Clones of a monobehaviour subscribed to ComponentsWithIMLData that is marked as "controlClones'
        /// </summary>
        [SerializeField, HideInInspector]
        private List<MonoBehaviour> m_MonobehaviourClones;
        private Dictionary<FieldInfo, IMLFieldInfoContainer> m_DataContainersPerFieldInfo;
        private Dictionary<FieldInfo, MonoBehaviour> m_DataMonobehavioursPerFieldInfo;

        private bool universalInputEnabled;
        [HideInInspector]
        public bool universalInputActive = false;


        private IMLGrab icon;
        private bool isSubscribed = false;

        #endregion

        #region Unity Messages

        void Reset()
        {
            
        }

        private void OnEnable()
        {

#if !UNITY_EDITOR
            SubscribeToDelegates();
            Initialize();

#endif
            //InitializeIMLIndicator();
        }

        // Called when something changes in the scene
        private void OnValidate()
        {
#if UNITY_EDITOR
           // Debug.Log("validating");
            // Subscribe to the editor manager so that our update loop gets called
            IMLEditorManager.SubscribeIMLComponent(this);

            IMLControllerOwnershipLogic();
            //SubscribeToDelegates();
            if (IMLEventDispatcher.TrainMLSCallback == null)
            {
                SubscribeToDelegates();
                
            }
            Initialize();
            
#endif
        }

        // Awake is called before start
        private void Awake()
        {
            //Debug.Log("awake");
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
#if !UNITY_EDITOR
                // Run Models on Play if we are on a build
                RunModelsOnPlay();
#endif
            
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
            // Stop running all models and stop collecting examples (if any)
            StopAllModels();
            StopAllCollectingExamples();
            // We unsubscribe the component from the editor manager to avoid messing up with the list
            IMLEditorManager.UnsubscribeIMLComponent(this);
            //Unsubscribe this from the event dispatcher 
            UnsubscribeToDelegates();
        }
#endregion

#region Private Methods


        private void Initialize()
        {
            // ensure universal input in inactive on open
            universalInputActive = false;
            
            // Initialise list of nodes for the IML Controller
            if (Lists.IsNullOrEmpty(ref GameObjectsToUse))
                GameObjectsToUse = new List<GameObject>();

            if (Lists.IsNullOrEmpty<TextNote>(ref m_TextNoteNodesList))
                m_TextNoteNodesList = new List<TextNote>();

            if (Lists.IsNullOrEmpty<TrainingExamplesNode>(ref m_TrainingExamplesNodesList))
                m_TrainingExamplesNodesList = new List<TrainingExamplesNode>();

            if (Lists.IsNullOrEmpty<MLSystem>(ref m_MLSystemNodeList))
                m_MLSystemNodeList = new List<MLSystem>();

            if (Lists.IsNullOrEmpty(ref m_GameObjectNodeList))
                m_GameObjectNodeList = new List<GameObjectNode>();

            if (Lists.IsNullOrEmpty(ref IMLControllerOutputs))
                IMLControllerOutputs = new List<double[]>();

            if (Lists.IsNullOrEmpty(ref FeatureNodesList))
                FeatureNodesList = new List<IFeatureIML>();
            
            if (Lists.IsNullOrEmpty(ref m_CustomControllerList))
                m_CustomControllerList = new List<CustomController>();


            // Get all th nodes which are in the graph
            GetAllNodes();

            /*  // Init logic for training examples
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

              }*/

            // Init logic for MLSystem nodes
            if (!Lists.IsNullOrEmpty(ref m_MLSystemNodeList))
            {

                for (int i = 0; i < m_MLSystemNodeList.Count; i++)
                {
                    var MLSystemNode = m_MLSystemNodeList[i];

                    if (MLSystemNode == null)
                    {
                        Debug.LogError("Null reference in MLSystem Node list in IML System. The list is not calculated properly and has some null spaces!");
                    }
                }
            }

            

            // In case the user is reusing the same IML graph (with the same number of GO or Script nodes) in more than one scene
            // We need to make sure that we are reusing the correct GameObjectNodes and ScriptNodes
            TryRecycleGameObjectNodes();
            TryRecycleScriptNodes();

            // commented to fix opening window problem - delete when sure this has not caused issues 
            // Inject GameObjects to GameObject nodes
            // SendGameObjectsToIMLController();
            // UpdateGameObjectImage();

#if !UNITY_EDITOR
            // If we are not on the editor...
            if (Application.isPlaying)
            {
                LoadDataForModels();
            }

#endif


            // We make sure that all null nodes in list are removed from the list
            if (graph)
                graph.nodes.RemoveAll(node => node == null);

            // initialize all nodes 
            InitializeAllNodes();
            InitializeEvent();
            // train models
            LoadDataForModels();


        }

        /// <summary>
        /// Initialize all nodes in the graph called OnEnable
        /// </summary>
        private void InitializeAllNodes()
        {
            //Initialise GameObjectNodes
            InitializeNodeType(m_GameObjectNodeList);
            //Initialise Features
            InitializeFeatureNode(FeatureNodesList);
            //Initialise Training Examples
            InitializeTrainingNodes(TrainingExamplesNodesList);
            //Initialise MLSystemList
            InitializeNodeType(MLSystemNodeList);
            //Initialise Script nodes
            InitializeNodeType(m_ScriptNodesList);
            //Initialise Script nodes
            InitializeNodeType(m_CustomControllerList);
            //Initialize input set up
            if (m_inputSetUp != null)
            {
                //initialize node
                m_inputSetUp.NodeInitalize();
            } 
            
        }
        /// <summary>
        /// Event to set up models when loading
        /// </summary>
        private void InitializeEvent()
        {
            IMLEventDispatcher.ModelSetUpChangeCallback?.Invoke();
        }

        /// <summary>
        /// Goes through all IMLnodes in list and initialises. Called in InitializeAllNodes
        /// </summary>
        /// <param name="ListToInitalize">IMLNode List to initiliaze</param>
        private void InitializeNodeType(IEnumerable<IMLNode> ListToInitalize)
        {
            if (ListToInitalize != null)
            {
                // loop through all nodes in list
                foreach (IMLNode node in ListToInitalize)
                {
                    if (node != null)
                    {
                        //Initialize node 
                        node.NodeInitalize();
                    } 

                }
            }

        }
        /// <summary>
        /// Goes through all IMLnodes in list and initialises. Called in InitializeAllNodes
        /// </summary>
        /// <param name="ListToInitalize">IMLNode List to initiliaze</param>
        private void InitializeTrainingNodes(List<TrainingExamplesNode> ListToInitalize)
        {
            if (ListToInitalize != null)
            {
                // loop through all nodes in list
                for(int i = 0; i < ListToInitalize.Count; i++)
                {
                    if (ListToInitalize[i] != null)
                    {
                        //Initialize node 
                        ListToInitalize[i].NodeInitalize();
                        ListToInitalize[i].listNo = i;
                    } 

                }
            }

        }

        private void InitializeFeatureNode(List<IFeatureIML> ListToInitalize)
        {
            // loop through all nodes in list
            foreach (IMLNode node in ListToInitalize)
            {
                //Initialize node 
                node.NodeInitalize();
            }
        }

        private void InitializeIMLIndicator()
        {
            //Debug.Log("icon initialize it");
            //comeback
            if (transform.childCount == 0 && icon == null)
            {
                icon = GameObject.Instantiate(Resources.Load("Prefabs/IMLIcon") as GameObject, this.transform).GetComponent<IMLGrab>();
                // avoiding null reference error in case the icon didn't load from resources
                if (icon != null && icon.graph != null)
                    icon.graph = this;
                else
                    Debug.LogWarning("Failed to load Prefabs/IMLIcon in IMLComponent.InitializeIMLIndicator()");
            } 
            if (icon == null)
            {
                icon = this.transform.Find("IMLIcon(Clone)").GetComponent<IMLGrab>();
            }
            
        }
        /// <summary>
        /// Subscribe to all delegates called in initialize
        /// </summary>
        private void SubscribeToDelegates() {
            //Debug.Log("subscribe");
            // DIRTY CODE
            // I am unsubscribing from all delegates first since there are issue with ToggleRecordCallback having the same method twice
           // UnsubscribeToDelegates();
            
            // dispatchers for MLSystem node events
            IMLEventDispatcher.TrainMLSCallback += Train;
            IMLEventDispatcher.ToggleRunCallback += ToggleRunning;
            IMLEventDispatcher.ResetModelCallback += ResetModel;

            // dispatchers for training examples node events
            IMLEventDispatcher.RecordOneCallback += RecordOne;
            IMLEventDispatcher.ToggleRecordCallback += ToggleRecording;
            IMLEventDispatcher.StartRecordCallback += StartRecording;
            IMLEventDispatcher.StopRecordCallback += StopRecording;
            IMLEventDispatcher.DeleteAllExamplesInNodeCallback += DeleteAllTrainingExamplesInNode;
            IMLEventDispatcher.DeleteAllTrainingExamplesInGraphCallback += DeleteAllTrainingExamplesInGraph;
            // IMLEventDispatcher.DeleteLastCallback +=

            IMLEventDispatcher.UniversalControlChange += UniversalInterface;
        }
        /// <summary>
        /// 
        /// Unsubscribe to all delegates called on destroy
        /// </summary>
        private void UnsubscribeToDelegates()
        {
            //Debug.Log("unsubscribe from delegates");
            // dispatchers for MLSystem node event
            IMLEventDispatcher.TrainMLSCallback -= Train;
            IMLEventDispatcher.ToggleRunCallback -= ToggleRunning;
            IMLEventDispatcher.ResetModelCallback -= ResetModel;

            // dispatchers for training examples node events
            IMLEventDispatcher.RecordOneCallback -= RecordOne;
            IMLEventDispatcher.ToggleRecordCallback -= ToggleRecording;
            IMLEventDispatcher.StartRecordCallback -= StartRecording;
            IMLEventDispatcher.StopRecordCallback -= StopRecording;
            IMLEventDispatcher.DeleteAllExamplesInNodeCallback -= DeleteAllTrainingExamplesInNode;
            IMLEventDispatcher.DeleteAllTrainingExamplesInGraphCallback -= DeleteAllTrainingExamplesInGraph;

            IMLEventDispatcher.UniversalControlChange -= UniversalInterface;
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
            if (graph != null)
            {
                // If we don't have a memory a previous controller, we remember this one
                if (m_LastKnownGraph == null)
                {
                    m_LastKnownGraph = graph;
                    // We also make sure to assign this IML Component as the one referenced in the IML controller
                    // So that nodes in the graph can know who updates them in the scene
                    graph.SceneComponent = this;
                }
                // If the controller has changed, we make sure to flush wrong information
                else if (m_LastKnownGraph != graph)
                {
                    // We make sure we free the scene component reference in the previous controller to avoid information sent to the wrong place
                    m_LastKnownGraph.SceneComponent = null;
                    // We remember the current controller
                    m_LastKnownGraph = graph;
                }
                // If the controller matches what we remember...
                else if (m_LastKnownGraph == graph)
                {
                    // We make sure the current controller is the right one
                    if (graph.SceneComponent != this)
                    {
                        // Warn in the editor that the controller is being used by several IMLComponents
                        //Debug.LogError("The referenced IML Controller is being used by more than one IML Component!");
                        graph.SceneComponent = this;
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
        public void GetAllNodes()
        {
            // Keep lists of nodes found updated
            if (graph != null)
            {
                foreach (var node in graph.nodes)
                {
                    // Feature nodes
                    CheckNodeIsFeature(node, ref FeatureNodesList);

                    // GameObject nodes
                    CheckTypeAddNodeToList(node, ref m_GameObjectNodeList);

                    // Training Examples nodes
                    CheckNodeIsTraining(node, ref m_TrainingExamplesNodesList);

                    // MLSystem Node
                    CheckNodeIsMLSystem(node, ref m_MLSystemNodeList);

                    // ScriptNodes
                    CheckTypeAddNodeToList(node, ref m_ScriptNodesList);

                    // input set up nodes 
                    CheckNodeIsInput(node, ref m_inputSetUp);

                    // check node is custom controller 
                    CheckNodeIsCustomController(node, ref m_CustomControllerList);
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
        /// <summary>
        /// Check whether node is inout and assign it to the reference in the script 
        /// </summary>
        /// <param name="nodetoAdd"></param>
        /// <param name="setUpNode"></param>
        private void CheckNodeIsInput(XNode.Node nodetoAdd, ref InteractML.ControllerCustomisers.InputSetUp setUpNode)
        {
            //check node not null
            if (nodetoAdd != null) {
                var inputNode = nodetoAdd as InteractML.ControllerCustomisers.InputSetUp;
                // if input node is not null and inputsetup node is null
                if (inputNode != null && m_inputSetUp == null)
                {
                    m_inputSetUp = inputNode;
                    if (inputTypes == null)
                        inputTypes = new List<Type>();
                    inputTypes.Add(typeof(InteractML.ControllerCustomisers.KeyboardInput));
                   
                }
            }

        }

        public void InputTypeAdd()
        {
            m_inputSetUp.devices = new InteractML.ControllerCustomisers.IInputType[inputTypes.Count];
            for(int i =0; i < inputTypes.Count; i++)
            {
                var type = inputTypes[i];

            }
           // m_inputSetUp.devices = inputTypes;
        }
        
        private void CheckNodeIsCustomController(XNode.Node nodeToAdd, ref List<CustomController> listToAddTo)
        {

            // We first check that the node ref is not null
            if (nodeToAdd != null)
            {
                // Then check that the node is a training examples node
                var customNode = nodeToAdd as CustomController;
                if (customNode != null)
                {
                    // Make sure the list is init
                    if (listToAddTo == null)
                        listToAddTo = new List<CustomController>();

                    // If we got a feature, we add it to the list (if it is not there already)
                    if (!listToAddTo.Contains(customNode))
                    {
                        listToAddTo.Add(customNode);
                    }

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
                    trainingNode.listNo = listToAddTo.Count - 1;

                }
            }

        }

        private void CheckNodeIsMLSystem(XNode.Node nodeToAdd, ref List<MLSystem> listToAddTo)
        {
            // We first check that the node ref is not null
            if (nodeToAdd != null)
            {
                // Then check that the node is a MLSystem
                var mlSystemNode = nodeToAdd as MLSystem;

                if(m_MLSystemNodeList.Count > 1)
                {
                    Debug.LogWarning("Only one machine learning system node allowed per graph when using radial you will not be able to control this in the headset");
                }
                if (mlSystemNode != null)
                {
                    // Make sure the list is init
                    if (listToAddTo == null)
                        listToAddTo = new List<MLSystem>();

                    // If we got a feature, we add it to the list (if it is not there already)
                    if (!listToAddTo.Contains(mlSystemNode))
                    {
                        listToAddTo.Add(mlSystemNode);
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

        private void RunMLSystemLogic()
        {
            if (m_MLSystemNodeList == null)
            {
                GetAllNodes();
            }
            for (int i = 0; i < m_MLSystemNodeList.Count; i++)
            {
                // If the node is null...
                if (m_MLSystemNodeList[i] == null)
                {
                    // We call again GetAllNodes to make sure our list is updated
                    GetAllNodes();
                    // Break the for loop
                    break;
                }
                else
                {
                    // Call the update logic per node
                    m_MLSystemNodeList[i].UpdateLogic();
                }
            }

        }

        /// <summary>
        /// Send all the gameobjects in the list to the 
        /// </summary>
        private void SendGameObjectsToIMLController()
        {

            Debug.Log(GameObjectsToUse.Count);
            Debug.Log(m_GOsPerGONodes.Count);
            Debug.Log(m_GameObjectNodeList.Count);
            // Don't do anything if there are no gameObjects from the scene to use



            if (GameObjectsToUse == null || GameObjectsToUse.Count == 0)
            {
                return;
            }

            // Make sure dictionaries and lists are init
            if (m_GOsPerGONodes == null)
                m_GOsPerGONodes = new GOPerGONodeDictionary();
            if (m_GameObjectNodeList == null)
                m_GameObjectNodeList = new List<GameObjectNode>();

            // Go through GONodes looking for empty entries that could contain memory (lost ref due to unity hotlreload)
           /* for (int i = 0; i < m_GameObjectNodeList.Count; i++)
            {
                var goNode = m_GameObjectNodeList[i];
                // If we find a null node, remove it!
                if (goNode == null)
                {
                    // Remove null reference
                    m_GameObjectNodeList.RemoveAt(i);
                    // Adjust index
                    i--;
                    continue;
                }
                if (!goNode.IsTaken)
                {
                    // If there is a scriptHashCode from a previous GO...
                    if (!goNode.GOHashCode.Equals(default))
                    {
                        // Check if the GOsPerGONodes dictionary contains the node and its GO
                        var gameObject = m_GOsPerGONodes.GetKey(goNode);
                        // Set GO if we found it
                        if (gameObject != null)
                        {
                            goNode.SetGameObject(gameObject);
                        }
                        // If we didn't find it...
                        else
                        {
                            // Check if that GO is contained in the GameobjectToUse list
                            // Lambda statement, selecting the GO that matches the hashcode memory from our GONode
                            var resultSearch = GameObjectsToUse.Select(GO => GO).Where(GO => GO.GetHashCode().Equals(goNode.GOHashCode));
                            if (resultSearch.Any())
                            {
                                var goToAdd = resultSearch.First(x => x != null);
                                // If we found a matching GO...
                                if (goToAdd != null)
                                {
                                    // We add that GO to our GONode (if it is not null) 
                                    goNode.SetGameObject(goToAdd);
                                    // Add it to the dictionary as well 
                                    if (!m_GOsPerGONodes.Contains(goToAdd))
                                        m_GOsPerGONodes.Add(goToAdd, goNode);
                                }
                            } else
                            {
                                m_GameObjectNodeList.Remove(goNode);
                            }

                        }

                    }
                }
            }*/

            // Go through gameObjects added by the user
           /* for (int i = 0; i < GameObjectsToUse.Count; i++)
            {
                var go = GameObjectsToUse[i];

                // ADD GAMEOBJECT NODE 
                GameObjectNode goNode = null;

                // If the gameObject is null, we continue to the next one
                if (go == null)
                {
                    continue;
                }

                // Check if any go NODE entry of the dictionary is NOT contain in one of our goNodes, and then remove those wrong entries (cleanup of wrong entries in dictionary)
                if (m_GOsPerGONodes.Count > m_GameObjectNodeList.Count)
                {
                    // Create copy of dictionary to evaluate
                    var goPerGONodesCopy = new GOPerGONodeDictionary();
                    foreach (KeyValuePair<GameObject, GameObjectNode> dicItem in m_GOsPerGONodes)
                    {
                        goPerGONodesCopy.Add(dicItem);
                    }

                    // Iterate through copy, but modify original
                    foreach (KeyValuePair<GameObject, GameObjectNode> dicItem in goPerGONodesCopy)
                    {
                        if (!m_GameObjectNodeList.Contains(dicItem.Value)){
                            Debug.Log("doesn't contain");
                            // Modify original list
                            m_GOsPerGONodes.Remove(dicItem);
                        }
                    }                    
                }

                // Check if the dictionary DOESN'T contain this GameObject value, and then create nodes and dictionary values (it is a new GameObject)
                if (!m_GOsPerGONodes.ContainsKey(go))
                {
                    // First, we try and see if the graph already contains an empty node we can use
                    foreach (var potentialGONode in m_GameObjectNodeList)
                    {
                        // We check if the node is available to use                        
                        // If the node is not taken...
                        if (!potentialGONode.IsTaken)
                        {
                            // This will be our node!
                            goNode = potentialGONode;
                            // Stop searching for nodes
                            break;
                        }
                    }

                    // If we didn't find a suitable existing node...
                    if (goNode == null)
                    {
                        // Create a new gameObject node into the graph
                        goNode = graph.AddNode<GameObjectNode>();
                    }

                    // Configure our node appropiately
                    goNode.SetGameObject(go);

                    // Add that to the dictionary            
                    m_GOsPerGONodes.Add(go, goNode);
                }



            }*/

            /* OLD LOGIC 

            ////Debug.Log("GameObjects being injected to IML Controller");
            //if (!Lists.IsNullOrEmpty(ref m_GameObjectNodeList) && !Lists.IsNullOrEmpty(ref GameObjectsToUse))
            //{
            //    // Compare distance between both lists
            //    int distanceLists = GameObjectsToUse.Count - m_GameObjectNodeList.Count;
            //    // If they are the same length
            //    if (distanceLists == 0)
            //    {
            //        // Assign each node a gameobject
            //        for (int i = 0; i < m_GameObjectNodeList.Count; i++)
            //        {
            //            m_GameObjectNodeList[i].GameObjectDataOut = GameObjectsToUse[i];
            //           // Debug.Log("Injecting GObject " + GameObjectsToUse[i].name);

            //        }

            //    }
            //    // If there are more gObjects than nodes, we spawn a few extract nodes and add the gameobjects to them
            //    else if (distanceLists > 0)
            //    {
            //        // Spawn as many nodes as distance we have
            //        for (int i = 0; i < distanceLists; i++)
            //        {
            //            MLController.AddNode<GameObjectNode>();
            //        }

            //        // Refresh list of nodes
            //        GetAllNodes();

            //        // Assign each node a gameobject
            //        for (int i = 0; i < m_GameObjectNodeList.Count; i++)
            //        {
            //            m_GameObjectNodeList[i].GameObjectDataOut = GameObjectsToUse[i];
            //        }

            //    }
            //    // If there are more nodes than gameObjects
            //    else if (distanceLists < 0)
            //    {
            //        // We warn the user
            //        Debug.LogWarning("There are currently more GameObject Nodes in " + MLController.name + " than GameObjects referenced in the IML Component of " + this.name);

            //        // Add as many GameObjects as we can to the list
            //        for (int i = 0; i < GameObjectsToUse.Count; i++)
            //        {
            //            m_GameObjectNodeList[i].GameObjectDataOut = GameObjectsToUse[i];
            //        }
            //    }
            //}
            
             END OF OLD LOGIC */

        }

        /// <summary>
        /// Gets and sets the data marked with the "SendToIMLController" and "PullFromIMLController" attributes in Monobehaviours subscribed
        /// </summary>
        private void FetchDataFromMonobehavioursSubscribed()
        {
            if (ComponentsWithIMLData == null || ComponentsWithIMLData.Count == 0)
            {
                return;
            }

            if (m_MonoBehavioursPerScriptNode == null)
                m_MonoBehavioursPerScriptNode = new MonobehaviourScriptNodeDictionary();

            // See if we already have any non-assigned scriptNodes that matches any IMLComponent
            if (m_ScriptNodesList != null && m_ScriptNodesList.Count > 0)
            {
                for (int i = 0; i < m_ScriptNodesList.Count; i++)
                {
                    if (m_ScriptNodesList[i] == null)
                    {
                        // Remove null reference
                        m_ScriptNodesList.RemoveAt(i);
                        // Adjust index
                        i--;
                        continue;
                    }
                    if (!m_ScriptNodesList[i].IsTaken)
                    {
                        // If there is a scriptHashCode from a previous script...
                        if (!m_ScriptNodesList[i].ScriptHashCode.Equals(default))
                        {
                            // Check if the monobehavioursPerScriptNode dictionary contains the node and its script
                            var script = m_MonoBehavioursPerScriptNode.GetKey(m_ScriptNodesList[i]);
                            // Set script if we found it
                            if (script != null)
                            {
                                m_ScriptNodesList[i].SetScript(script);
                            }
                            // If we didn't find it...
                            else
                            {
                                // Check if that script is contained in the ComponentsWithIMLData list
                                // Lambda statement, selecting the script that matches the hashcode from our scriptNode
                                var resultSearch = ComponentsWithIMLData.Select(container => container.GameComponent).Where(gameComponent =>
                                    {
                                        MonoBehaviour scriptToReturn = null;
                                        if (gameComponent.GetHashCode().Equals(m_ScriptNodesList[i].ScriptHashCode))
                                            scriptToReturn = gameComponent;
                                        return scriptToReturn;
                                    }
                                );
                                if (resultSearch.Any())
                                {
                                    var scriptToAdd = resultSearch.First(x => x != null);
                                    // If we found a matching script...
                                    if (scriptToAdd != null)
                                    {
                                        bool rogueNodeFound = false;
                                        // Check if the script is already present in the dictionary with a matching scriptnode, if it is, maybe our found scriptNode is a duplicate rogue
                                        while (m_MonoBehavioursPerScriptNode.Contains(scriptToAdd))
                                        {
                                            // Check if that entry has a corresponding not null scriptnode
                                            ScriptNode auxScriptNode = null;
                                            m_MonoBehavioursPerScriptNode.TryGetValue(scriptToAdd, out auxScriptNode);
                                            // If the matching node in dict is null, it might be a corrupted entry
                                            if (auxScriptNode == null)
                                            {
                                                // Remove corrupted entry
                                                m_MonoBehavioursPerScriptNode.Remove(scriptToAdd);
                                            }
                                            // If the matching node is not null, then our scriptToAdd is in a healthy node and we have a rogue node. Exit while loop
                                            else
                                            {
                                                // we are dealing with a rogue scriptNode reference right now, mark it
                                                rogueNodeFound = true;
                                                break; // exit while loop
                                            }
                                        }

                                        // If we are dealing with a rogue Node (a node that shouldn't have been created)...
                                        if (rogueNodeFound)
                                        {
                                            // We need to remove this node from the list
                                            m_ScriptNodesList.RemoveAt(i);
                                            // Adjust index
                                            i--;
                                            continue; // skip to next entry
                                        }
                                        // If it is a healthy but lonely node...
                                        else
                                        {
                                            // We add that script to our scriptNode (if it is not null)                                
                                            m_ScriptNodesList[i].SetScript(scriptToAdd);
                                            // Add it to the dictionary as well, unless it is already present    
                                            m_MonoBehavioursPerScriptNode.Add(scriptToAdd, m_ScriptNodesList[i]);

                                        }
                                    }
                                }
                                // We are dealing with a rogue node if reaching here
                                else
                                {
                                    // Remove null reference
                                    m_ScriptNodesList.RemoveAt(i);
                                    // Adjust index
                                    i--;
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            // Fetch data from scripts added by the user
            for (int i = 0; i < ComponentsWithIMLData.Count; i++)
            {

                var IMLGameComponentContainer = ComponentsWithIMLData[i];
                var gameComponent = ComponentsWithIMLData[i].GameComponent;

                /* ADD SCRIPT NODE */
                ScriptNode scriptNode = null;

                // If the gameComponent is null, we continue to the next one
                if (gameComponent == null)
                {
                    continue;
                }

                // Check if the dictionary DOESN'T contain a fieldInfo for this reflected value, and then create nodes and dictionary values
                if (!m_MonoBehavioursPerScriptNode.ContainsKey(IMLGameComponentContainer.GameComponent))
                {
                    // First, we try and see if the graph already contains an empty node we can use
                    foreach (var node in graph.nodes)
                    {
                        // We see if this node is of the right type
                        if (node.GetType() == typeof(ScriptNode))
                        {
                            // We check if the node is available to use
                            //var isTaken = m_MonoBehavioursPerScriptNode.Values.Any(container => container.Script == gameComponent);
                            var foundScriptNode = (node as ScriptNode);
                            bool isTaken = false;
                            if (foundScriptNode.GetScript() != null)
                            {
                                isTaken = true;
                                bool isSameType = foundScriptNode.GetScript().GetType() == gameComponent.GetType();
                                // If the node is of the same type but we expect to control clones...
                                if (isSameType && IMLGameComponentContainer.ControlClones)
                                {
                                    // We check if the script is attached to a clone
                                    if (foundScriptNode.GetScript().gameObject.name.Contains("(Clone)"))
                                        // If it is a clone we consider it not taken
                                        isTaken = false;
                                }
                            }
                            // If the node is not taken...
                            if (!isTaken)
                            {
                                // This will be our node!
                                scriptNode = foundScriptNode;
                                // Stop searching for nodes
                                break;
                            }
                        }
                    }

                    // If we didn't find a suitable existing node...
                    if (scriptNode == null)
                    {
                        // Create a new script node into the graph
                        scriptNode = graph.AddNode<ScriptNode>();
                    }

                    // Configure our node appropiately
                    scriptNode.SetScript(gameComponent);

                    // Add that to the dictionary            
                    m_MonoBehavioursPerScriptNode.Add(IMLGameComponentContainer.GameComponent, scriptNode);

                }
                // If it contains a fieldInfo for this reflected value, access node
                else
                {
                    m_MonoBehavioursPerScriptNode.TryGetValue(IMLGameComponentContainer.GameComponent, out scriptNode);
                    // If the returned node is null...
                    if (scriptNode == null)
                    {
                        // The corresponding scriptnode was deleted for some reason. Attempt a repair
                        scriptNode = graph.AddNode<ScriptNode>();
                        scriptNode.SetScript(gameComponent);
                        // Reset the entry in dictionary
                        while (m_MonoBehavioursPerScriptNode.Contains(gameComponent))
                        {
                            ScriptNode scriptNodeToDelete = null;
                            m_MonoBehavioursPerScriptNode.TryGetValue(gameComponent, out scriptNodeToDelete);
                            if (scriptNodeToDelete != null)
                                graph.RemoveNode(scriptNodeToDelete);
                            m_MonoBehavioursPerScriptNode.Remove(gameComponent);

                        }
                        m_MonoBehavioursPerScriptNode.Add(gameComponent, scriptNode);
                    }
                }

                // Update ports if required
                if (scriptNode)
                {
                    scriptNode.UpdatePortFields(gameComponent);
                }
            }

            if (m_MonobehaviourClones == null)
                m_MonobehaviourClones = new List<MonoBehaviour>();
            // Fetch data from clones if there are any
            for (int i = 0; i < m_MonobehaviourClones.Count; i++)
            {
                var clone = m_MonobehaviourClones[i];
                // If the clone is null by any chance, remove it from list
                if (clone == null)
                {
                    m_MonobehaviourClones.RemoveAt(i);
                    continue;
                }
                // Get corresponding scriptNode
                if (m_MonoBehavioursPerScriptNode.Contains(clone))
                {
                    ScriptNode scriptNode;
                    m_MonoBehavioursPerScriptNode.TryGetValue(clone, out scriptNode);
                    if (scriptNode)
                        scriptNode.UpdatePortFields(clone);
                }
            }

        }

        /// <summary>
        /// Tries to reuse GameObject Nodes (in case the same graph is used in more than one scene)
        /// </summary>
        private void TryRecycleGameObjectNodes()
        {
            // Don't do anything if there are no gameObjects from the scene to use
            if (GameObjectsToUse == null || GameObjectsToUse.Count == 0)
            {
                return;
            }

            // Make sure dictionary is init
            if (m_GOsPerGONodes == null)
                m_GOsPerGONodes = new GOPerGONodeDictionary();
            // If we don't have any GO nodes found, we can't recycle anything
            if (m_GameObjectNodeList == null)
            {
                m_GameObjectNodeList = new List<GameObjectNode>();
                return; // stop here because we can't recycle nodes if there are none!
            }

            // Go through every gameObject node, seeing if we can recycle it
            for (int i = 0; i < m_GameObjectNodeList.Count; i++)
            {
                var goNode = m_GameObjectNodeList[i];
                // Skip to next entry if current is null!
                if (goNode == null)
                    continue;

                // Check node name to see if it matches one of the gameObjects to use in the scene
                foreach (var go in GameObjectsToUse)
                {
                    string goName = go.name + " (GameObject)";
                    if (goName.Equals(goNode.name))
                    {
                        // We found a match (by name), check if the go instances are not the same (we are double recycling)
                        if (goNode.GetHashCode().Equals(go.GetHashCode()))
                        {
                            // We have already recycled this gameObject! Skip to next gameObject
                            continue;
                        }
                        else
                        {
                            // We can recycle this goNode
                            goNode.SetGameObject(go);
                            // Make sure that the pair GO, GONode is present in dictionary
                            if (m_GOsPerGONodes.ContainsKey(go))
                            {
                                GameObjectNode auxGONode;
                                // If there is already a gameobject key in dictionary, update its goNode key
                                m_GOsPerGONodes.TryGetValue(go, out auxGONode);
                                
                                if (auxGONode == null)
                                {
                                    // Maybe there is an entry corrupted in the dictionary, attempt a repair
                                    while (m_GOsPerGONodes.Contains(go))
                                    {
                                        m_GOsPerGONodes.TryGetValue(go, out auxGONode);
                                        if (auxGONode == null)
                                            m_GOsPerGONodes.Remove(go);
                                        else
                                            break;
                                    }
                                    if (auxGONode == null)
                                        // Add GO, GONode pair to dictionary since there is not one present
                                        m_GOsPerGONodes.Add(go, goNode);
                                }
                                // If the found goNode in dictionary doesn't match our recycled node...
                                if (auxGONode != null && !auxGONode.Equals(goNode))
                                {
                                    // Override it
                                    m_GOsPerGONodes.Remove(go);
                                    m_GOsPerGONodes.Add(go, goNode);
                                }
                            }
                            // Stop iterating
                            break;

                        }
                    }
                }
                
                // The foreach loop should have taken care of the recycling logic :)
            }

        }

        /// <summary>
        /// Tries to reuse Script Nodes (in case the same graph is used in more than one scene)
        /// </summary>
        private void TryRecycleScriptNodes()
        {
            // Don't do anything if there are no scripts from the scene to use
            if (ComponentsWithIMLData == null || ComponentsWithIMLData.Count == 0)
            {
                return;
            }
            // Init dictionary if not init
            if (m_MonoBehavioursPerScriptNode == null)
                m_MonoBehavioursPerScriptNode = new MonobehaviourScriptNodeDictionary();

            // If we don't have any scripts nodes found, we can't recycle anything
            if (m_ScriptNodesList == null)
            {
                m_ScriptNodesList = new List<ScriptNode>();
                return; // stop here because we can't recycle nodes if there are none!
            }

            // Go through every script node, seeing if we can recycle it
            for (int i = 0; i < m_ScriptNodesList.Count; i++)
            {
                var scriptNode = m_ScriptNodesList[i];
                // Continue to next entry if current is null!
                if (scriptNode == null)
                    continue;

                // We are going to check  he existing script name in the scriptNode to see if it matches one of the scripts to use in the scene
                foreach (IMLMonoBehaviourContainer scriptContainer in ComponentsWithIMLData)
                {
                    string scriptName = scriptContainer.GameComponent.GetType().Name + " (Script)";
                    if (scriptName.Equals(scriptNode.name))
                    {
                        // We found a match (by name), check if the script instances are not the same (we are double recycling)
                        if (scriptNode.ScriptHashCode.Equals(scriptContainer.GetHashCode()))
                        {
                            // We have already recycled this gameObject! Skip to next gameObject
                            continue;
                        }
                        else
                        {
                            // We can recycle this goNode
                            scriptNode.SetScript(scriptContainer.GameComponent);
                            // Make sure that the pair Script, ScriptNode is present in dictionary
                            if (m_MonoBehavioursPerScriptNode.ContainsKey(scriptContainer.GameComponent))
                            {
                                ScriptNode auxScriptNode;
                                // If there is already a script key in dictionary, update its scriptNode key
                                m_MonoBehavioursPerScriptNode.TryGetValue(scriptContainer.GameComponent, out auxScriptNode);

                                if (auxScriptNode == null)
                                {
                                    // There might be Corrupted data in dictionary, attempt a repair
                                    while (m_MonoBehavioursPerScriptNode.Contains(scriptContainer.GameComponent))
                                    {
                                        m_MonoBehavioursPerScriptNode.TryGetValue(scriptContainer.GameComponent, out auxScriptNode);
                                        if (auxScriptNode == null)
                                            m_MonoBehavioursPerScriptNode.Remove(scriptContainer.GameComponent);
                                        else
                                            break;
                                    }

                                    // If after the repair, the dictionary didn't contain any healthy reference...
                                    if (auxScriptNode == null) 
                                        // Add script, scriptNode pair to dictionary since there is not one present
                                        m_MonoBehavioursPerScriptNode.Add(scriptContainer.GameComponent, scriptNode);
                                }
                                // If the found scriptNode in dictionary doesn't match our recycled node...
                                if (auxScriptNode != null && !auxScriptNode.Equals(scriptNode))
                                {
                                    // Override it
                                    m_MonoBehavioursPerScriptNode.Remove(scriptContainer.GameComponent);
                                    m_MonoBehavioursPerScriptNode.Add(scriptContainer.GameComponent, scriptNode);
                                }
                            }
                            // Stop iterating
                            break;

                        }
                    }
                }

                // The foreach loop should have taken care of the recycling logic :)
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

            // MLSystem node List
            if (m_MLSystemNodeList != null)
                m_MLSystemNodeList.Clear();
            else
                m_MLSystemNodeList = new List<MLSystem>();

        }

        [ContextMenu("ClearScriptNodes")]
        public void ClearScriptNodes()
        {
            if (m_MonoBehavioursPerScriptNode == null)
                m_MonoBehavioursPerScriptNode = new MonobehaviourScriptNodeDictionary();
            m_MonoBehavioursPerScriptNode.Clear();
        }

        /// <summary>
        /// Update logic loop that will be called inside Unity's Update event
        /// </summary>
        public void UpdateLogic()
        {
            //Debug.Log(universalInputEnabled);
            // Make sure that the icon is init
            if (icon == null && m_inputSetUp != null)
                InitializeIMLIndicator();

            if (icon != null)
            {
                if (icon.graph == null)
                {
                    icon.graph = this;
                }

            }
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
            //GetAllNodes();
            // is reference to the graph is not null
            if (graph != null)
            {
                // if it is a new graph
                if(m_LastKnownGraph != graph)
                {
                    // set up reference
                    IMLControllerOwnershipLogic();
                    // Initialize all nodes
                    Initialize();
                }
                // if graph has lost reference to the IML Compoenent 
                if(graph.IsGraphRunning == false)
                    IMLControllerOwnershipLogic();
                // Fetch data from the Monobehaviours we have subscribed into and out of the IML Controller
                FetchDataFromMonobehavioursSubscribed();

                // Send GameObject data to GONodes
                SendGameObjectsToIMLController();

                // Run logic for all feature nodes
                RunFeaturesLogic();

                // Run logic for all training example nodes
                RunTraininExamplesLogic();

                // Run logic for all MLSystem nodes
                RunMLSystemLogic();


            } else
            {
                // get reference to graph & give graph reference to IML Component
                IMLControllerOwnershipLogic();
            }
            // input logic 
            InputLogic();

        }
        /// <summary>
        /// Update logic for input modules
        /// </summary>
        public void InputLogic(){
            // if user has enables universal input system
            if(universalInputEnabled && universalInputActive)
            {
                //if there is a reference to the node
                if (m_inputSetUp != null)
                {
                    m_inputSetUp.UpdateLogic();
                }
            } else
            {
                foreach(CustomController controller in m_CustomControllerList)
                {
                    if (controller == null)
                        m_CustomControllerList.Remove(controller);
                    else
                     controller.UpdateLogic();
                }
            }

        }

        /// <summary>
        /// Public function that updates the game objects in the graph and make sure no node is empty
        /// </summary>
        public void UpdateGameObjectNodes(bool changingPlayMode = false)
        {
            if (m_GameObjectNodeList == null)
                m_GameObjectNodeList = new List<GameObjectNode>();
            if (GameObjectsToUse == null)
                GameObjectsToUse = new List<GameObject>();

            for (int i = 0; i < m_GameObjectNodeList.Count; i++)
            {
                var goNode = m_GameObjectNodeList[i];
                // Remove node from list if the reference is null
                if (goNode == null)
                {
                    m_GameObjectNodeList.RemoveAt(i);
                    // Decrease counter to not delete the wrong element later
                    i--;
                }
#if UNITY_EDITOR
                else if (changingPlayMode && goNode.CreatedDuringPlaymode)
                {
                    // Destroy node
                    graph.RemoveNode(goNode);
                    // Decrease counter to not delete the wrong element later
                    i--;
                    // Force gameobject reference to null
                    goNode = null;
                }
#endif
                // Check if we need to remove the goNode from the list
                else
                {
                    // If this GO node is not contained in the logic dictionary...
                    if (!m_GOsPerGONodes.ContainsValue(goNode))
                    {
                        // Destroy node
                        graph.RemoveNode(goNode);
                        // Decrease counter to not delete the wrong element later
                        i--;
                        // Force scriptNode reference to null
                        goNode = null;
                    }
                }

                // Now if the node wasn't removed, make sure that there is a gameobject that the node is controlling in the scene list that the user controls
                if (goNode != null)
                {
                    // If we are switching playmodes, it is very likely that we lost the reference to the GO?
                    if (changingPlayMode)
                    {
                        // Check if the gameobject reference is null
                        if (goNode.GameObjectDataOut == null)
                        {
                            // See if we know the hash of the referenced gameobject
                            if (goNode.GOHashCode != default(int))
                            {
                                // Check if the script is contained in the scene list
                                var result = GameObjectsToUse.Select(x => x).Where(go => go.GetHashCode().Equals(goNode.GetHashCode()) );
                                var goFound = result.FirstOrDefault();
                                // If we found the go...
                                if (goFound != null)
                                {
                                    // Add the go to the node
                                    goNode.SetGameObject(goFound);
                                }

                            }
                        }
                    }
                    // If we have a reference to the go the node controls...
                    if (goNode.GameObjectDataOut != null)
                    {
                        // Make sure is managed in the scene list of GOs
                        if (!GameObjectsToUse.Contains(goNode.GameObjectDataOut))
                        {
                            GameObjectsToUse.Add(goNode.GameObjectDataOut);
                        }
                    }

                }

            }
        }

        /// <summary>
        /// Makes sure that there are no more scriptNodes that the ones needed
        /// </summary>
        public void UpdateScriptNodes(bool changingPlayMode = false)
        {
            if (m_ScriptNodesList == null)
                m_ScriptNodesList = new List<ScriptNode>();
            if (ComponentsWithIMLData == null)
                ComponentsWithIMLData = new List<IMLMonoBehaviourContainer>();

            for (int i = 0; i < m_ScriptNodesList.Count; i++)
            {
                var scriptNode = m_ScriptNodesList[i];
                // Remove node from list if the reference is null
                if (scriptNode == null)
                {
                    m_ScriptNodesList.RemoveAt(i);
                    // Decrease counter to not delete the wrong element later
                    i--;
                }
#if UNITY_EDITOR
                else if (changingPlayMode && scriptNode.CreatedDuringPlaymode)
                {
                    // Destroy node
                    graph.RemoveNode(scriptNode);
                    // Decrease counter to not delete the wrong element later
                    i--;
                    // Force scriptNode reference to null
                    scriptNode = null;
                }
#endif
                // Check if we need to remove the scriptNode from the list
                else
                {
                    // If this script node is not contained in the logic dictionary...
                    if (!m_MonoBehavioursPerScriptNode.ContainsValue(scriptNode))
                    {
                        // Destroy node
                        graph.RemoveNode(scriptNode);
                        // Decrease counter to not delete the wrong element later
                        i--;
                        // Force scriptNode reference to null
                        scriptNode = null;
                    }
                }

                // Now if the node wasn't removed, make sure that there is a script that the node is controlling in componentsWithIMLData
                if (scriptNode != null)
                {
                    // If we are switching playmodes, it is very likely that we lost the reference to the script
                    if (changingPlayMode)
                    {
                        // Check if the script reference is null
                        if (scriptNode.GetScript() == null)
                        {
                            // See if we know the hash of the referenced script
                            if (scriptNode.ScriptHashCode != default(int))
                            {
                                // Check if the script is contained in the components with IML Data
                                var result = ComponentsWithIMLData.Select(x => x.GameComponent).Where(gameComponent => gameComponent.GetHashCode() == scriptNode.ScriptHashCode);                                
                                var script = result.FirstOrDefault();
                                // If we found the script...
                                if (script != null)
                                {
                                    // Add the script to the node
                                    scriptNode.SetScript(script);
                                }
                                
                            }
                        }
                    }
                    // If we have a reference to the script the node controls...
                    if (scriptNode.GetScript() != null)
                    {
                        // Make sure is managed in the list of scripts
                        var container = new IMLMonoBehaviourContainer(scriptNode.GetScript());
                        if (!ComponentsWithIMLData.Contains(container))
                        {
                            ComponentsWithIMLData.Add(container);
                        }
                    }
                    
                }

            }
        }

        /// <summary>
        /// Loads and runs (or retrains if needed) models from disk on awake
        /// </summary>
        private void LoadDataForModels()
        {
            if(this != null)
            {
                // There will be waits for things to init. Take into account
                IEnumerator coroutine = LoadDataForModelsCoroutine();
                StartCoroutine(coroutine);
            }
            
        }

        /// <summary>
        /// Coroutine to load and run (or retrain if needed) models from disk on awake
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadDataForModelsCoroutine()
        {
            nodesLoaded = false;
            //Debug.Log("RESET AND RETRAIN CALLED FROM IML COMPONENT");

            //yield return new WaitForSeconds(0.05f);

            //Debug.Log("RESET AND RETRAIN CALLED FROM IML COMPONENT");

            // Reset all models
            //ResetAllModels();
            // if there are training examples nodes
            if (TrainingExamplesNodesList.Count > 0)
            {
                // Reload Training Data Set
                while (!LoadAllTrainingExamples())
                {
                    // wait for a frame until the data is loaded
                    yield return null;
                }
            }
            
            // Wait for another frame
            yield return null;

            // Retrain them
            //ReTrainAllModelsCoroutine();

            // if there are mlsystem nodes in the graph
            if (MLSystemNodeList.Count > 0)
            {
                Debug.Log("load models");
                while (!(bool)IMLEventDispatcher.LoadModelsCallback?.Invoke())
                {
                    // wait for a frame until models are retrained
                    yield return null;
                }
            }
            

            // Wait for another frame
            yield return null;

            nodesLoaded = true;

            yield return null;

            if (Application.isPlaying)
            {
                RunModelsOnPlay();
            }

            yield break;

        }
        /// <summary>
        /// Starts run models on play coroutine
        /// </summary>
        public void RunModelsOnPlay()
        {
            if (this != null)
            {
                // There will be waits for things to init. Take into account
                IEnumerator coroutine = RunModelsOnPlayCoroutine();
                StartCoroutine(coroutine);
            }
            

        }
        /// <summary>
        /// Coroutine for running models checks to see if loaded and then runs models
        /// </summary>
        /// <returns></returns>
        private IEnumerator RunModelsOnPlayCoroutine()
        {
            yield return null;

            // wait for nodes to load
            while (!nodesLoaded)
            {
                yield return null;
            }

            // run models marked run on awake
            RunAllModels();

            yield break;
              
        }
        /// <summary>
        /// Loads training data from disk and forces an init on the training examples node if not init
        /// </summary>
        public bool LoadAllTrainingExamples()
        {
            bool success = false;
            // Only run if we got trainnig examples
            if (m_TrainingExamplesNodesList == null && m_TrainingExamplesNodesList.Count == 0)
                return success;

            // Go through all training examples
            foreach (var trainingExamplesNode in m_TrainingExamplesNodesList)
            {
                // Skip if null
                if (trainingExamplesNode == null)
                    continue;

                // Force node init
                while (!trainingExamplesNode.IsInitialized)
                {
                    // We leave the method (but we will come back to it since we call it from a coroutine)
                    return success;
                }

                // Load Data
                //trainingExamplesNode.LoadDataFromDisk();

                success = true;
            }

            return success;
        }

        /// <summary>
        /// Stops collecting examples on all the training examples nodes (to be called when leaving or entering a scene)
        /// </summary>
        public void StopAllCollectingExamples()
        {
            // Avoid null or empty errors
            if (m_TrainingExamplesNodesList == null || m_TrainingExamplesNodesList.Count == 0)
                return;

            // Iterate all training examples nodes
            foreach (var trainingExamplesNode in m_TrainingExamplesNodesList)
            {
                // Only stop it if it is collecting data
                if (trainingExamplesNode.CollectingData)
                {
                    trainingExamplesNode.StopCollecting();
                }
            }
        }

        [ContextMenu("Delete All Models")]
        public void DeleteAllModels()
        {
            //Debug.Log("Delete All Models Called");

            // I tried deleting the MLSystem nodes, but that gave errors when controlling Rapidlib! Avoiding to do that at all.

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
        public void LoadAllModelsFromDisk(bool reCreateModels = false)
        {
            foreach (var MLSystemNode in m_MLSystemNodeList)
            {
                // Loads the model in the MLSystemNode
                MLSystemNode.LoadModelFromDisk(reCreateModels);
            }
            
        }

        /// <summary>
        /// Saves all models to disk (when possible)
        /// </summary>
        public void SaveAllModels()
        {
            foreach (var MLSystemNode in m_MLSystemNodeList)
            {
                // Save model to disk
                MLSystemNode.SaveModelToDisk();
            }

        }

        /// <summary>
        /// Stops all models if they are running
        /// </summary>
        public void StopAllModels()
        {
            foreach (var MLSystemNode in m_MLSystemNodeList)
            {
                // Stop model if they are running
                if (MLSystemNode.Running)
                    MLSystemNode.StopRunning();
            }
        }

        /// <summary>
        /// Resets all models for the MLSystem nodes (by destroying and re-creating them)
        /// </summary>
        public void ResetAllModels()
        {
            //Debug.Log("Resetting all models...");

            DeleteAllModels();

            //// Go through the list of MLSystem nodes and instantiate new rapidlibs
            //foreach (var MLSystemNode in MLSystemNodesList)
            //{
            //    if (MLSystemNode)
            //    {
            //        MLSystemNode.InstantiateRapidlibModel();                    
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
        public bool RunAllModels()
        {
            Debug.Log("run all models");
            bool success = false;
            foreach (var MLSystemNode in m_MLSystemNodeList)
            {
                if (MLSystemNode)
                {
                    // Only run if the flag is marked to do so
                    bool trainingExamples = false;
                    if (MLSystemNode.RunOnAwake)
                    {
                        // Attempt to load/train if the model is untrained
                        if (MLSystemNode.Untrained)
                        {
                            // First try to load the model (unless is DTW)
                            if (MLSystemNode.Model.TypeOfModel != RapidlibModel.ModelType.DTW)
                            {
                                MLSystemNode.LoadModelFromDisk();
                            }
                            // Only attempt to train if model is still untrained
                            if (MLSystemNode.Untrained)
                            {
                                // Train if there are training examples available
                                for (int i = 0; i < MLSystemNode.IMLTrainingExamplesNodes.Count; i++)
                                {
                                    if (MLSystemNode.IMLTrainingExamplesNodes[i].TrainingExamplesVector.Count > 0)
                                    {
                                        trainingExamples = true;
                                    }
                                }
                                if (trainingExamples)
                                {
                                    success = MLSystemNode.TrainModel();
                                }
                            }
                        }
                        // Toggle Run only if the model is trained (and it is not DTW, the user should do that)
                        if (MLSystemNode.Trained && MLSystemNode.TrainingType != IMLSpecifications.TrainingSetType.SeriesTrainingExamples)
                        {
                            success = MLSystemNode.StartRunning();
                            if (success && icon != null)
                            {
                                icon.SetBody(icon.runningColour);
                            }
                        }
                    }

                }
            }
            return success;
        }

        /// <summary>
        /// Coroutine that will retrain all the models (one per frame, because training is expensive)
        /// </summary>
        /// <returns></returns>
        IEnumerator ReTrainAllModelsCoroutine()
        {
            yield return new WaitForSeconds(0.05f);
            foreach (var MLSystemNode in m_MLSystemNodeList)
            {
                if (MLSystemNode)
                {
                    // Only retrains if the flag is marked to do so
                    if (MLSystemNode.TrainOnPlaymodeChange)
                        MLSystemNode.TrainModel();

                    yield return null;
                }
            }
            
        }

        /// <summary>
        /// Retrain all the models at once
        /// </summary>
        /// <returns></returns>
        private bool RetrainAllModelsPrivate()
        {
            Debug.Log("retraining");
            bool success = false;
            foreach (var MLSystemNode in m_MLSystemNodeList)
            {
                if (MLSystemNode)
                {
                    // Reset Model
                    MLSystemNode.ResetModel();
                    // Attempt to load if not DTW
                    if (MLSystemNode.Model.TypeOfModel != RapidlibModel.ModelType.DTW)
                    {
                        success = MLSystemNode.LoadModelFromDisk();
                    }
                    // If the model didn't succeed in loading
                    if (!success)
                    {
                        // Only retrains if the flag is marked to do so
                        if (MLSystemNode.TrainOnPlaymodeChange)
                            success = MLSystemNode.TrainModel();
                    }
                    if (!success && MLSystemNode.TotalNumTrainingDataConnected == 0)
                    {
                        success = true;
                    }
                    
                }
            }
            return success;
        }

#region SubscriptionOfMonobehaviours

        /// <summary>
        /// Pass a Monobehaviour and mark any field with "SendToIMLController" or "PullFromIMLController" attribute to use it with the IML Component
        /// </summary>
        /// <param name="gameComponent"></param>
        public void SubscribeToIMLController(MonoBehaviour gameComponent, bool controlClones = false)
        {
            if (gameComponent == null)
            {
                Debug.LogError("Monobehaviour passed is null!");
                return;
            }
            else
            {
                if (ComponentsWithIMLData == null)
                    ComponentsWithIMLData = new List<IMLMonoBehaviourContainer>();
                if (m_MonoBehavioursPerScriptNode == null)
                    m_MonoBehavioursPerScriptNode = new MonobehaviourScriptNodeDictionary();
                if (m_MonobehaviourClones == null)
                    m_MonobehaviourClones = new List<MonoBehaviour>();

                var container = new IMLMonoBehaviourContainer(gameComponent, controlClones);
                // If the list of component doesn't contain the monobehaviour to be added...
                if (!ComponentsWithIMLData.Contains(container))
                {
                    ComponentsWithIMLData.Add(container);
                }
                // If the monobehaviour is in the list, it might be a clone
                else if (controlClones)
                {
                    // Check if this clone is already added
                    if (!m_MonoBehavioursPerScriptNode.Contains(gameComponent))
                    {
                        // Retrieve scriptNode managing clones
                        ScriptNode scriptNode = null;
                        // Check if we have a matching script so that we can reuse the scriptNode
                        var KeyValuePairs = m_MonoBehavioursPerScriptNode.GetEnumerator();
                        while (KeyValuePairs.MoveNext())
                        {
                            // If we have a matching type...
                            if (KeyValuePairs.Current.Key.GetType() == gameComponent.GetType())
                            {
                                // Get that scriptNode, it will control our clone
                                scriptNode = KeyValuePairs.Current.Value;
                            }
                        }
                        // If we didn't find a scriptNode for this clone...
                        if (scriptNode == null)
                        {
                            // It might not be created yet, try updating scriptNodes to see if it will be created
                            FetchDataFromMonobehavioursSubscribed();
                            // Try again the search
                            KeyValuePairs = m_MonoBehavioursPerScriptNode.GetEnumerator();
                            while (KeyValuePairs.MoveNext())
                            {
                                // If we have a matching type...
                                if (KeyValuePairs.Current.Key.GetType() == gameComponent.GetType())
                                {
                                    // Get that scriptNode, it will control our clone
                                    scriptNode = KeyValuePairs.Current.Value;
                                }
                            }
                        }
                        // If we succesfully retrieved the scriptNode...
                        if (scriptNode != null)
                        {
                            // Assign this clone to that scriptNode
                            m_MonoBehavioursPerScriptNode.Add(gameComponent, scriptNode);
                            // Save reference in our list of clones
                            m_MonobehaviourClones.Add(gameComponent);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes Monobehaviour from the list to use with the IMLController
        /// </summary>
        /// <param name="gameComponent"></param>
        public void UnsubscribeFromIMLController(MonoBehaviour gameComponent, bool controlClones = false)
        {
            if (gameComponent == null)
            {
                Debug.LogError("Monobehaviour passed is null!");
                return;
            }
            else
            {
                if (ComponentsWithIMLData == null)
                    ComponentsWithIMLData = new List<IMLMonoBehaviourContainer>();

                var container = new IMLMonoBehaviourContainer(gameComponent, controlClones);
                if (ComponentsWithIMLData.Contains(container))
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
                                            graph.RemoveNode(entry2.Value.nodeForField);
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
                    ComponentsWithIMLData.Remove(container);
                }
            }

        }

#endregion

#region Add Nodes

        /// <summary>
        /// Adds a scriptNode internally to the list
        /// </summary>
        /// <param name="node"></param>
        public bool AddScriptNode(ScriptNode node)
        {
            bool nodeAdded = false;
            if (node != null)
            {
                if (m_ScriptNodesList == null) m_ScriptNodesList = new List<ScriptNode>();
                // Add node if it is not contained in list
                if (!m_ScriptNodesList.Contains(node))
                {
                    m_ScriptNodesList.Add(node);
                    nodeAdded = true;

#if UNITY_EDITOR
                    // Mark node as created During Playmode if required
                    if (EditorApplication.isPlaying) node.CreatedDuringPlaymode = true;

                    // Save newnode to graph on disk                              
                    AssetDatabase.AddObjectToAsset(node, graph);
                    // Reload graph into memory since we have modified it on disk
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(graph));
#endif

                }
            }
            return nodeAdded;
        }

        /// <summary>
        /// Adds a gameObjectNode internally to the list
        /// </summary>
        /// <param name="node"></param>
        public bool AddGameObjectNode(GameObjectNode node)
        {
            bool nodeAdded = false;
            if (node != null)
            {
                if (m_GameObjectNodeList == null) m_GameObjectNodeList = new List<GameObjectNode>();
                // Add node if it is not contained in list
                if (!m_GameObjectNodeList.Contains(node))
                {
                    m_GameObjectNodeList.Add(node);
                    nodeAdded = true;
#if UNITY_EDITOR
                    // Mark node as created During Playmode if required
                    if (EditorApplication.isPlaying) node.CreatedDuringPlaymode = true;

                    // Save newnode to graph on disk                              
                    AssetDatabase.AddObjectToAsset(node, graph);
                    // Reload graph into memory since we have modified it on disk
                    //AssetDatabase.SaveAssets();
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(graph));
#endif
                }

            }
            return nodeAdded;
        }

#endregion

#region Deletion of Nodes
        // code needs refactoring 
        /// <summary>
        /// Removes GameObjectNode From GameObjectNodeList 
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteGameObjectNode(GameObjectNode nodeToDelete)
        {
            if (m_GameObjectNodeList.Contains(nodeToDelete))
                m_GameObjectNodeList.Remove(nodeToDelete);

            // GameObjectsToUse user GOs fro scene list
            if (GameObjectsToUse == null)
                GameObjectsToUse = new List<GameObject>();
            else if (nodeToDelete.GameObjectDataOut != null)
                if (GameObjectsToUse.Contains(nodeToDelete.GameObjectDataOut))
                    GameObjectsToUse.Remove(nodeToDelete.GameObjectDataOut);

            // GOs per GONodes dictionary
            if (m_GOsPerGONodes == null)
                m_GOsPerGONodes = new GOPerGONodeDictionary();
            else if (nodeToDelete.GameObjectDataOut != null && m_GOsPerGONodes.ContainsKey(nodeToDelete.GameObjectDataOut))
                m_GOsPerGONodes.Remove(nodeToDelete.GameObjectDataOut);

            // GONodes list
            if (m_GameObjectNodeList == null)
                m_GameObjectNodeList = new List<GameObjectNode>();
            else if (m_GameObjectNodeList.Contains(nodeToDelete))
                m_GameObjectNodeList.Remove(nodeToDelete);


        }
        /// <summary>
        /// Removes MLSystemNode From MLSystemNodeList 
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteMLSystemNode(MLSystem nodeToDelete)
        {
            if (m_MLSystemNodeList.Contains(nodeToDelete))
                m_MLSystemNodeList.Remove(nodeToDelete);
        }
        
        public void DeleteCustomControllerNode(CustomController nodeToDelete)
        {
            if (m_CustomControllerList.Contains(nodeToDelete))
                m_CustomControllerList.Remove(nodeToDelete);
        }
        
        /// <summary>
        /// Removes TrainingExamplesNode From TrainingExamplesNodeList 
        /// </summary>
        /// <param name="nodeToDelete"></param>
        public void DeleteTrainingExamplesNode(TrainingExamplesNode nodeToDelete)
        {
            if (m_TrainingExamplesNodesList.Contains(nodeToDelete))
                m_TrainingExamplesNodesList.Remove(nodeToDelete);
            UpdateTrainingExamplesListNo();
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

        /// <summary>
        /// Removes a script node from the IML Component
        /// </summary>
        /// <param name="node"></param>
        public void DeleteScriptNode(ScriptNode node)
        {
            var container = new IMLMonoBehaviourContainer(node.GetScript());

            // Components with IML Data list
            if (ComponentsWithIMLData == null)
                ComponentsWithIMLData = new List<IMLMonoBehaviourContainer>();
            else if (ComponentsWithIMLData.Contains(container))
                ComponentsWithIMLData.Remove(container);

            // Monobehaviours per scriptNode dictionary
            if (m_MonoBehavioursPerScriptNode == null)
                m_MonoBehavioursPerScriptNode = new MonobehaviourScriptNodeDictionary();
            else if (container.GameComponent != null && m_MonoBehavioursPerScriptNode.ContainsKey(container.GameComponent))
                m_MonoBehavioursPerScriptNode.Remove(container.GameComponent);

            // scriptNodes list
            if (m_ScriptNodesList == null)
                m_ScriptNodesList = new List<ScriptNode>();
            else if (m_ScriptNodesList.Contains(node))
                m_ScriptNodesList.Remove(node);
        }

#endregion

        //Very dirty code need to sort it out in a better way - updates the node pointer when exiting or entering play mode 
        public void updateGameObjectImage()
        {
            
            foreach (GameObjectNode GONode in m_GameObjectNodeList)
            {
                GONode.state = true;
            }
        }

        /// <summary>
        /// Add to internal dictionary of GameObject Nodes and GameObjects
        /// </summary>
        /// <param name="goNode"></param>
        /// <param name="go"></param>
        public void AddToGameObjectNodeDictionary(GameObjectNode goNode, GameObject go)
        {
            m_GOsPerGONodes.Add(go, goNode);
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
#region Delegates
        /// <summary>
        /// Delegate to train model from 
        /// </summary>
        /// <param name="nodeID">ID of the node to be trained</param>
        /// <returns>returns boolean whether sucessfully trained</returns>
        private bool Train(string nodeID)
        {
            bool success = false;
            //iterate through all mls nodes
            foreach (MLSystem MLSNode in MLSystemNodeList)
            {
                //if nodeID matches
                if (nodeID == MLSNode.id)
                {
                    
                    // train model
                    success = MLSNode.TrainModel();
                    // if this is successful save model to the disk
                    if (success)
                        MLSNode.SaveModelToDisk();
                }
            }
            //Debug.Log(icon == null);
            if (success && icon != null)
                icon.SetBody(icon.trainedColor);
            // returns true if nodeID exists and whether training successful
            return success;
        }
        /// <summary>
        ///  Start running delegate
        /// </summary>
        /// <param name="nodeID">nodeID for ls to start running</param>
        /// <returns></returns>
        private bool ToggleRunning(string nodeID)
        {
            bool success = false;
            //iterate through all mls nodes
            foreach (MLSystem MLSNode in MLSystemNodeList)
            {
                // if nodeID matches
                if (nodeID == MLSNode.id)
                {
                    if (MLSNode.Running)
                    {
                        success = MLSNode.StopRunning();
                        if(success && icon != null)
                            icon.SetBody(icon.baseColour);
                    }
                    else
                    {
                        success = MLSNode.StartRunning();
                        if (success && icon != null)
                            icon.SetBody(icon.runningColour);
                        //else
                            //icon.SetBody(icon.current);
                    }
                        

                }
            }
            // return true if nodeID exists and running started
            return success; 
        }
        
       
        

        /// <summary>
        /// Reset model for delegate
        /// </summary>
        /// <param name="nodeID">node id of the MLS nide to reset</param>
        private bool ResetModel(string nodeID)
        {
            //iterate through all mls nodes
            foreach (MLSystem MLSNode in MLSystemNodeList)
            {
                // if nodeID matches
                if (nodeID == MLSNode.id)
                {
                    // Reset model
                    MLSNode.ResetModel();
                }
            }
            icon.SetColourHighlight(icon.selectedHighlight);
            return true;
        }

        /// <summary>
        /// Record one example for delegate
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        private bool RecordOne(string nodeID)
        {
            //iterate through all training nodes
            foreach (TrainingExamplesNode TENode in TrainingExamplesNodesList)
            {
                // if node ID matches AND the node is a single training examples node
                if (nodeID == TENode.id && TENode.GetType().ToString().Contains("InteractML.SingleTrainingExamplesNode"))
                {
                    // add single example
                    
                    if (TENode.AddSingleTrainingExample())
                    {
                        return true;
                    }
                    
                }
            }
            return false; ;
        }

        /// <summary>
        /// Start recording training examples for delegate
        /// </summary>
        /// <param name="nodeID">nodeID of the training examples node</param>
        /// <returns>bool whether training has started</returns>
        private bool ToggleRecording(string nodeID)
        {
            //Debug.Log("toggle");
            bool success = false;
            foreach (TrainingExamplesNode TENode in TrainingExamplesNodesList)
            {
                if (nodeID == TENode.id)
                {
                    if (TENode.CollectingData)
                    {
                       // Debug.Log("stop");
                        success = TENode.StopCollecting();
                        if (success)
                        {
                            if (icon != null)
                                icon.SetBody(icon.baseColour);
                        }
                    } else
                    {
                        //Debug.Log("start");
                        success = TENode.StartCollecting();
                        if (success)
                        {
                            if (icon !=null )
                                icon.SetBody(icon.recordingColour);
                        }
                        else
                        {
                            if (icon != null)
                                icon.SetBody(icon.current);
                        }
                    }
                        
                }
            }
            return success;
        }


        /// <summary>
        /// Start recording training examples for delegate
        /// </summary>
        /// <param name="nodeID">nodeID of the training examples node</param>
        /// <returns>bool whether training has started</returns>
        private bool StartRecording(string nodeID)
        {
            bool success = false;
            foreach (TrainingExamplesNode TENode in TrainingExamplesNodesList)
            {
                if (nodeID == TENode.id)
                {
                    success = TENode.StartCollecting();
                }
            }
            return success;
        }
        /// <summary>
        /// Stop recording training examples for delegate
        /// </summary>
        /// <param name="nodeID">nodeID of the training examples node</param>
        /// <returns>bool whether training has stopped</returns>
        private bool StopRecording(string nodeID)
        {
            foreach (TrainingExamplesNode TENode in TrainingExamplesNodesList)
            {
                if (nodeID == TENode.id)
                {
                    if (TENode.StopCollecting())
                    {
                        return true;
                    }

                }
            }
            return false; ;
        }
        /// <summary>
        /// Delete all training exmples for delefate
        /// </summary>
        /// <param name="nodeID">nodeID of the training examples to delete</param>
        private bool DeleteAllTrainingExamplesInNode(string nodeID)
        {
           // Debug.Log(nodeID);
            foreach (TrainingExamplesNode TENode in TrainingExamplesNodesList)
            {
                if (nodeID == TENode.id)
                {
                    // clear training examples from this node 
                   TENode.ClearTrainingExamples();
                }
            }
            return true;
        }

        private bool DeleteAllTrainingExamplesInGraph(bool deleteFromDisk = true)
        {
            foreach (TrainingExamplesNode TENode in TrainingExamplesNodesList)
            {                
                // clear training examples from this node 
                TENode.ClearTrainingExamples(deleteFromDisk: deleteFromDisk);
            }
            return true;

        }

        private void UniversalInterface(bool value)
        {
            universalInputEnabled = value;
        }

        private void UpdateTrainingExamplesListNo()
        {
            for(int i = 0; i < TrainingExamplesNodesList.Count; i++){
                TrainingExamplesNodesList[i].listNo = i;
            }
        }

#endregion
    }

}
