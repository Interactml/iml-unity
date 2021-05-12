﻿using System.Collections;
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
        /// <summary>
        /// Dictionary to hold references of GameObjects and which GONode manages them
        /// </summary>
        [SerializeField, HideInInspector]
        private GOPerGONodeDictionary m_GOsPerGONodes;


        #region Private Lists of Nodes (Fields)
        /* Private Lists of nodes that we can have in the graph */
        private List<TextNote> m_TextNoteNodesList;
        private List<TrainingExamplesNode> m_TrainingExamplesNodesList;
        private List<IMLConfiguration> m_IMLConfigurationNodesList;
        [SerializeField, HideInInspector]
        private List<GameObjectNode> m_GameObjectNodeList;
        private List<RealtimeIMLOutputNode> m_RealtimeIMLOutputNodesList;
        public List<IFeatureIML> FeatureNodesList;
        [SerializeField, HideInInspector]
        private List<ScriptNode> m_ScriptNodesList;
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
        [Header("Scripts to Track")]
        [Tooltip("Add number of Scripts to use in the IML Controller and what they are here")]
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
                LoadDataAndRunOnAwakeModels();
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

                    // ScriptNodes
                    CheckTypeAddNodeToList(node, ref m_ScriptNodesList);

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
            // Don't do anything if there are no gameObjects from the scene to use
            if (GameObjectsToUse == null || GameObjectsToUse.Count == 0)
            {
                return;
            }

            // Make sure dictionaries and lists are init
            if (m_MonoBehavioursPerScriptNode == null)
                m_MonoBehavioursPerScriptNode = new MonobehaviourScriptNodeDictionary();
            if (m_GameObjectNodeList == null)
                m_GameObjectNodeList = new List<GameObjectNode>();

            // Go through GONodes looking for empty entries that could contain memory (lost ref due to unity hotlreload)
            for (int i = 0; i < m_GameObjectNodeList.Count; i++)
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
                            }

                        }
                        
                    }
                }
            }

            // Go through gameObjects added by the user
            for (int i = 0; i < GameObjectsToUse.Count; i++)
            {
                var go = GameObjectsToUse[i];

                /* ADD GAMEOBJECT NODE */
                GameObjectNode goNode = null;

                // If the gameObject is null, we continue to the next one
                if (go == null)
                {
                    continue;
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
                        // Create a new script node into the graph
                        goNode = MLController.AddNode<GameObjectNode>();
                    }

                    // Configure our node appropiately
                    goNode.SetGameObject(go);

                    // Add that to the dictionary            
                    m_GOsPerGONodes.Add(go, goNode);
                }



            }

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
                                        // We add that script to our scriptNode (if it is not null)                                
                                        m_ScriptNodesList[i].SetScript(scriptToAdd);
                                        // Add it to the dictionary as well    
                                        m_MonoBehavioursPerScriptNode.Add(scriptToAdd, m_ScriptNodesList[i]);
                                    }
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
                    foreach (var node in MLController.nodes)
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
                                    if (foundScriptNode.GetScript().gameObject.name.Contains("(Clone)") )
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
                        scriptNode = MLController.AddNode<ScriptNode>();
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
                if (m_MonoBehavioursPerScriptNode.Contains(clone) )
                {
                    ScriptNode scriptNode;
                    m_MonoBehavioursPerScriptNode.TryGetValue(clone, out scriptNode);
                    if (scriptNode)
                        scriptNode.UpdatePortFields(clone);
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

                // Send GameObject data to GONodes
                SendGameObjectsToIMLController();

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
                    MLController.RemoveNode(goNode);
                    // Decrease counter to not delete the wrong element later
                    i--;
                    // Force scriptNode reference to null
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
                        MLController.RemoveNode(goNode);
                        // Decrease counter to not delete the wrong element later
                        i--;
                        // Force scriptNode reference to null
                        goNode = null;
                    }
                }

                // Now if the node wasn't removed, make sure that there is a script that the node is controlling in the scene list that the user controls
                if (goNode != null)
                {
                    // If we are switching playmodes, it is very likely that we lost the reference to the GO?
                    if (changingPlayMode)
                    {
                        // Check if the script reference is null
                        if (goNode.GameObjectDataOut == null)
                        {
                            // See if we know the hash of the referenced script
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
                    MLController.RemoveNode(scriptNode);
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
                        MLController.RemoveNode(scriptNode);
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
        public void LoadDataAndRunOnAwakeModels()
        {
            // There will be waits for things to init. Take into account
            IEnumerator coroutine = LoadDataAndRunOnAwakeModelsCoroutine();
            StartCoroutine(coroutine);

        }

        /// <summary>
        /// Coroutine to load and run (or retrain if needed) models from disk on awake
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadDataAndRunOnAwakeModelsCoroutine()
        {
            yield return new WaitForSeconds(0.05f);

            Debug.Log("RESET AND RETRAIN CALLED FROM IML COMPONENT");

            // Reset all models
            //ResetAllModels();

            // Reload Training Data Set
            while (!LoadAllTrainingExamples())
            {
                // wait for a frame until the data is loaded
                yield return null;
            }

            // Wait for another frame
            yield return null;

            // Retrain them
            //ReTrainAllModelsCoroutine();

            while(!RetrainAllModelsPrivate())
            {
                // wait for a frame until models are retrained
                yield return null;
            }

            // Wait for another frame
            yield return null;

            // Run the models
            while(!RunAllModels() )
            {
                yield return null;
            }

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
                trainingExamplesNode.LoadDataFromDisk();

                success = true;
            }

            return success;
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
        public void LoadAllModelsFromDisk(bool reCreateModels = false)
        {
            foreach (var IMLConfigNode in m_IMLConfigurationNodesList)
            {
                // Loads the model in the IMLConfigNode
                IMLConfigNode.LoadModelFromDisk(reCreateModels);
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
        public bool RunAllModels()
        {
            bool success = false;
            foreach (var imlConfigNode in m_IMLConfigurationNodesList)
            {
                if (imlConfigNode)
                {
                    // Only run if the flag is marked to do so
                    bool trainingExamples = false;
                    if (imlConfigNode.RunOnAwake)
                    {
                        // Attempt to load/train if the model is untrained
                        if (imlConfigNode.Untrained)
                        {
                            // First try to load the model (unless is DTW)
                            if (imlConfigNode.LearningType != IMLSpecifications.LearningType.DTW)
                            {
                                imlConfigNode.LoadModelFromDisk();
                            }
                            // Only attempt to train if model is still untrained
                            if (imlConfigNode.Untrained)
                            {
                                // Train if there are training examples available
                                for (int i = 0; i < imlConfigNode.IMLTrainingExamplesNodes.Count; i++)
                                {
                                    if (imlConfigNode.IMLTrainingExamplesNodes[i].TrainingExamplesVector.Count > 0)
                                    {
                                        trainingExamples = true;
                                    }
                                }
                                if (trainingExamples)
                                {
                                    success = imlConfigNode.TrainModel();
                                }
                            }
                        }
                        // Toggle Run only if the model is trained (and it is not DTW, the user should do that)
                        if (imlConfigNode.Trained && imlConfigNode.LearningType != IMLSpecifications.LearningType.DTW)
                        {
                            imlConfigNode.ToggleRunning();
                            success = true;
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
        /// Retrain all the models at once
        /// </summary>
        /// <returns></returns>
        private bool RetrainAllModelsPrivate()
        {
            bool success = false;
            foreach (var imlConfigNode in m_IMLConfigurationNodesList)
            {
                if (imlConfigNode)
                {
                    // Reset Model
                    imlConfigNode.ResetModel();
                    // Attempt to load if not DTW
                    if (imlConfigNode.LearningType != IMLSpecifications.LearningType.DTW)
                    {
                        success = imlConfigNode.LoadModelFromDisk();
                    }
                    // If the model didn't succeed in loading
                    if (!success)
                    {
                        // Only retrains if the flag is marked to do so
                        if (imlConfigNode.TrainOnPlaymodeChange)
                            success = imlConfigNode.TrainModel();
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
                    AssetDatabase.AddObjectToAsset(node, MLController);
                    // Reload graph into memory since we have modified it on disk
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(MLController));
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
                    AssetDatabase.AddObjectToAsset(node, MLController);
                    // Reload graph into memory since we have modified it on disk
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(MLController));
#endif
                }

            }
            return nodeAdded;
        }

        #endregion

        #region Deletion of Nodes

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
