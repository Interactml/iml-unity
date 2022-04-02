using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using InteractML.Addons;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class IMLEditorManager
{
    private static List<IMLComponent> m_IMLComponents;
    public bool modeHasChanged {get; private set;}

    /// <summary>
    /// Addons present in the scene
    /// </summary>
    private static List<IAddonIML> m_IMLAddons;

#if UNITY_EDITOR
    // CALLBACKS FOR OTHER EDITOR SCRIPTS

    #region Early Callbacks
    /// <summary>
    /// Public external EARLY editor callbacks for scene opened (called before regular IMLEditorManager SceneOpened logic)
    /// </summary>
    public static EditorSceneManager.SceneOpenedCallback EarlySceneOpenedCallbacks;
    /// <summary>
    /// Public external EARLY editor callbacks for update (called before regular IMLEditorManager Update logic)
    /// </summary>
    public static EditorApplication.CallbackFunction EarlyUpdateCallbacks;
    /// <summary>
    /// Public external editor EARLY callbacks for playmodeStateChanged (called before regular PlayModeStateChanged logic)
    /// </summary>
    public static System.Action<PlayModeStateChange> EarlyPlayModeStateChangedCallbacks;
    /// <summary>
    /// Public external editor EARLY callbacks for scripts hot relaod (called before regular script hot reload logic)
    /// </summary>
    public static System.Action EarlyScriptReloadCallbacks;
    #endregion

    #region IMLSystem Instantiation
    /// <summary>
    /// Call after an IML System has been created in the editor
    /// </summary>
    public static System.Action<IMLComponent> IMLSystemCreatedCallback;
    #endregion

#endif

    static IMLEditorManager()
    {
        Initiliaze();
    }


    private static void Initiliaze()
    {
#if UNITY_EDITOR
        // Make sure the list is init
        if (m_IMLComponents == null) m_IMLComponents = new List<IMLComponent>();
        if (m_IMLAddons == null) m_IMLAddons = new List<IAddonIML>();

        // Make sure editor callbacks are up to date before doing anything else
        UnsubscribeEditorCallbacks();
        SubscribeEditorCallbacks();

        // When the project starts for the first time, we find the iml components present in that scene
        if (m_IMLComponents.Count == 0) FindIMLComponents();
        // Also find addons
        if (m_IMLAddons.Count == 0) FindIMLAddons();
#endif

    }

#if UNITY_EDITOR
    /// <summary>
    /// Refind and re-subscribe components when scripts reload (we loose all refs on hot reload)
    /// </summary>
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void ScriptsReloadedLogic()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            // Invoke any early callbacks for when script reloads (also include external ones)
            if (EarlyScriptReloadCallbacks != null) EarlyScriptReloadCallbacks.Invoke();
            // Init class
            Initiliaze();
        }
#endif
    }

    private static void UpdateLogic()
    {
#if UNITY_EDITOR

        // Only run update logic when the app is not running (outside playmode or paused)
        if (!EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            // IML Components
            if (m_IMLComponents != null && m_IMLComponents.Count > 0)
            {
                //Debug.Log("IML Components number: " + m_IMLComponents.Count);

                // Repair list of known iml components if any of them is null
                if (NullIMLComponents()) RepairIMLComponents();

                // Run each of the updates in the iml components
                foreach (var MLcomponent in m_IMLComponents)
                {
                    //Debug.Log("**EDITOR**");
                    if (MLcomponent != null)
                        MLcomponent.UpdateLogic();
                    else
                        Debug.LogWarning("There is a null reference to a MLComponent in IMLEditorManager.UpdateLogic()");
                }
            }

            // Addons
            if (m_IMLAddons != null && m_IMLAddons.Count > 0)
            {
                // Repair list of known addons if any of them is null
                if (NullIMLAddons()) RepairIMLAddons();

                // Run each of the updates in the addons
                foreach (var addon in m_IMLAddons)
                {
                    //Debug.Log("**EDITOR**");
                    if (addon != null)
                        addon.EditorUpdateLogic();
                    else
                        Debug.LogWarning("There is a null reference to an IML Addon in IMLEditorManager.UpdateLogic()");
                }
            }

        }

#endif

    }

#if UNITY_EDITOR
    /// <summary>
    /// When the scene opens we will clear and find all the imlComponents
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private static void SceneOpenedLogic(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
    {
        // IML Components
        if (NullIMLComponents()) RepairIMLComponents();
        // Reload all models (if we can) when we enter playmode or when we come back to the editor
        foreach (var MLComponent in m_IMLComponents)
        {
            if (MLComponent != null)
            {
                // Get all nodes
                MLComponent.GetAllNodes();
                //// Reload models
                //MLComponent.LoadAllModelsFromDisk(reCreateModels: true);
                //// Run them (if marked with RunOnAwake)
                //MLComponent.RunAllModels();
            }
            else
            {
                Debug.LogWarning("There is a null reference to a MLComponent in IMLEditorManager.SceneOpenedLogic()");
            }
        }

        // Addons
        if (NullIMLAddons()) RepairIMLAddons();
        foreach (var addon in m_IMLAddons)
        {
            if (addon != null)
            {
                addon.EditorSceneOpened();
            }
        }
    }


    /// <summary>
    /// When we change playmode, we make sure to reset all iml models
    /// </summary>
    /// <param name="playModeStatus"></param>
    private static void PlayModeStateChangedLogic(PlayModeStateChange playModeStatus)
    {
        bool nullIMLComponents = false;
        bool nullIMLAddons = false;
        // Repair list of known iml components if any of them is null
        if (NullIMLComponents()) nullIMLComponents = true;
        if (NullIMLAddons()) nullIMLAddons = true;

        if (nullIMLComponents || nullIMLAddons)
        {
            // If we have null references, we need to clear all callbacks in event dispatcher
            // as we will lose the ref to all IML components! We avoid ghost refs to null IML Components
            IMLEventDispatcher.ClearAllCallbacks();
            if (nullIMLComponents) RepairIMLComponents();
            // Repair list of addons too
            if (nullIMLAddons) RepairIMLAddons();
        }

        foreach (IMLComponent MLComp in m_IMLComponents) 
        {
            foreach (MLSystem MLS in MLComp.MLSystemNodeList) {
                if (MLS != null)
                    MLS.UIErrors();
                else
                    Debug.LogWarning("Null reference to a MLComponent in IMLEditorManager.PlayModeStateChangedLogic()");
            }
        }

        #region Enter Events

        // We load models if we are entering a playmode (not required when leaving playmode)
        if (playModeStatus == PlayModeStateChange.EnteredPlayMode)
        {
            // IML Components
            // Reload all models (if we can) when we enter playmode or when we come back to the editor
            foreach (var MLComponent in m_IMLComponents)
            {
                if (MLComponent != null)
                {
                    //Debug.Log("Play mode state changed");
                    //MLComponent.LoadDataAndRunOnAwakeModels();
                    MLComponent.RunModelsOnPlay();
                    //// Reload models
                    //MLComponent.LoadAllModelsFromDisk(reCreateModels: true);
                    //// Run them (if marked with RunOnAwake)
                    //MLComponent.RunAllModels();
                }
                else
                {
                    Debug.LogWarning("Null reference to a MLComponent in IMLEditorManager.PlayModeStateChangedLogic() when EnteredPlayMode");
                }
            }
            //Debug.Log("**Models reconfigured in editor status: " + playModeStatus + "**");

            // IML Addons
            foreach (var addon in m_IMLAddons)
            {
                if (addon != null)
                {
                    addon.EditorEnteredPlayMode();
                }
            }
        }

        if (playModeStatus == PlayModeStateChange.EnteredEditMode)
        {
            // IML Components
            foreach (var MLComponent in m_IMLComponents)
            {
                if (MLComponent != null)
                {
                    MLComponent.updateGameObjectImage();
                    MLComponent.GetAllNodes();
                    MLComponent.UpdateGameObjectNodes(changingPlayMode: true);
                    MLComponent.UpdateScriptNodes(changingPlayMode: true);
                }
                else
                {
                    Debug.LogWarning("Null reference to a MLComponent in IMLEditorManager.PlayModeStateChangedLogic() when EnteredEditMode");
                }
            }

            // IML Addons
            foreach (var addon in m_IMLAddons)
            {
                if (addon != null && (addon as MonoBehaviour) != null)
                {
                    addon.EditorEnteredEditMode();
                }
            }

        }

        #endregion

        #region Exit Events

        // Remove any scriptNodes added during playtime when leaving playMode
        if (playModeStatus == PlayModeStateChange.ExitingPlayMode)
        {
            // IML Components
            foreach (var MLComponent in m_IMLComponents)
            {
                if (MLComponent != null)
                {
                    MLComponent.UpdateGameObjectNodes(changingPlayMode: true);
                    MLComponent.UpdateScriptNodes(changingPlayMode: true);
                }
                else
                {
                    Debug.LogWarning("Null reference to a MLComponent in IMLEditorManager.PlayModeStateChangedLogic() when ExitingPlayMode");
                }
            }

            // IML Addons
            foreach (var addon in m_IMLAddons)
            {
                if (addon != null)
                {
                    addon.EditorExitingPlayMode();
                }
            }

        }

        // We stop models if we are leaving a playmode or editormode
        if (playModeStatus == PlayModeStateChange.ExitingEditMode || playModeStatus == PlayModeStateChange.ExitingPlayMode)
        {
            // IML Components
            foreach (var MLComponent in m_IMLComponents)
            {
                if (MLComponent != null)
                    MLComponent.StopAllModels();
                else
                    Debug.LogWarning("Null reference to a MLComponent in IMLEditorManager.PlayModeStateChangedLogic() when leaving a playmode or editormode");
            }

            // IML Addons
            foreach (var addon in m_IMLAddons)
            {
                if (addon != null)
                {
                    addon.EditorExitingEditMode();
                }
            }

        }

        #endregion

    }

#endif

    #region IMLComponents Search

    /// <summary>
    /// Clears the entire list of iml components (to be called when a new scene loads for example)
    /// </summary>
    private static void ClearIMLComponents()
    {
        if (m_IMLComponents == null) m_IMLComponents = new List<IMLComponent>();
        // Clear private list
        m_IMLComponents.Clear();
    }

    /// <summary>
    /// Finds all the iml components already present in the scene 
    /// </summary>
    private static void FindIMLComponents()
    {
        //Debug.Log("FindIMLComponents called  ");
        // Get all iml components in scene
        var componentsFound = Object.FindObjectsOfType<IMLComponent>();

        // If we found any components, try to subscribe them to the list
        if (componentsFound != null)
        {
            foreach (var component in componentsFound)
            {
                SubscribeIMLComponent(component);
            }

        }
    }

    /// <summary>
    /// Repairs the list of known IML Components
    /// </summary>
    private static void RepairIMLComponents()
    {
        ClearIMLComponents();
        FindIMLComponents();        
    }

    /// <summary>
    /// Is any of the IMLComponents null?
    /// </summary>
    /// <returns></returns>
    private static bool NullIMLComponents()
    {
        return m_IMLComponents.Any(x => x == null) ? true : false;
    }

    private static void InitIMLComponent(ref IMLComponent component)
    {
        if (component != null)
        {
            // Using this as the equivalent of "OnEnable" in Edit mode
            component.Initialize();
            //component.SubscribeToDelegates();
        }
    }

    private static void SubscribeIMLComponentsToEvenDispatcher(ref IMLComponent component)
    {
        if (component != null)
        {
            // Using this as the equivalent of "OnEnable" in Edit mode
            component.SubscribeToDelegates();
        }
    }

    #endregion

    #region IMLAddons Search

    /// <summary>
    /// Clears the entire list of iml components (to be called when a new scene loads for example)
    /// </summary>
    private static void ClearIMLAddons()
    {
        if (m_IMLAddons == null) m_IMLAddons = new List<IAddonIML>();
        // Clear private list
        m_IMLAddons.Clear();
    }

    /// <summary>
    /// Finds all the iml addons already present in the scene (to be called after 
    /// </summary>
    private static void FindIMLAddons()
    {
        // Get all iml components in scene
        var componentsFound = Object.FindObjectsOfType<MonoBehaviour>().OfType<IAddonIML>();

        // If we found any components, try to subscribe them to the list
        if (componentsFound != null)
        {
            Debug.Log($"{componentsFound.Count()} addons found in scene!");
            foreach (var component in componentsFound)
            {
                SubscribeIMLAddon(component);
            }

        }
    }

    /// <summary>
    /// Repairs the list of known IML Addons
    /// </summary>
    private static void RepairIMLAddons()
    {
        ClearIMLAddons();
        FindIMLAddons();
    }

    /// <summary>
    /// Is any of the IMLAddons null?
    /// </summary>
    /// <returns></returns>
    private static bool NullIMLAddons()
    {
        return m_IMLAddons.Any(x => x == null) ? true : false;
    }

    #endregion


    #region Subscriptions

    private static void SubscribeEditorCallbacks()
    {
#if UNITY_EDITOR
        // Subscribe this manager to the editor update loop
        EditorApplication.update += EarlyUpdateCallbacks; // External also
        EditorApplication.update += UpdateLogic;

        //Debug.Log("New IMLEditorManager created in scene " + EditorSceneManager.GetActiveScene().name);

        // Subscribe manager event to the sceneOpened event
        EditorSceneManager.sceneOpened += EarlySceneOpenedCallbacks; // External also
        EditorSceneManager.sceneOpened += SceneOpenedLogic;

        // Subscribe manager event to the playModeStateChanged event
        EditorApplication.playModeStateChanged += EarlyPlayModeStateChangedCallbacks; // External also
        EditorApplication.playModeStateChanged += PlayModeStateChangedLogic;


#endif
    }

    private static void UnsubscribeEditorCallbacks()
    {
#if UNITY_EDITOR
        // Subscribe this manager to the editor update loop
        EditorApplication.update -= UpdateLogic;
        EditorApplication.update -= EarlyUpdateCallbacks; // External


        //Debug.Log("New IMLEditorManager created in scene " - EditorSceneManager.GetActiveScene().name);

        // Subscribe manager event to the sceneOpened event
        EditorSceneManager.sceneOpened -= SceneOpenedLogic;
        EditorSceneManager.sceneOpened -= EarlySceneOpenedCallbacks; // External

        // Subscribe manager event to the playModeStateChanged event
        EditorApplication.playModeStateChanged -= PlayModeStateChangedLogic;
        EditorApplication.playModeStateChanged -= EarlyPlayModeStateChangedCallbacks; // External

#endif
    }

    /// <summary>
    /// Subscribes an imlcomponent to the list (avoiding duplicates)
    /// </summary>
    /// <param name="newComponentToAdd"></param>
    public static void SubscribeIMLComponent(IMLComponent newComponentToAdd)
    {
        // Make sure the list is initialised
        if (m_IMLComponents == null)
        {
            m_IMLComponents = new List<IMLComponent>();
        }

        // Make sure the list doesn't contain already the component we want to add
        if (!m_IMLComponents.Contains(newComponentToAdd))
        {
            // We add the component if it it is not in the list already
            m_IMLComponents.Add(newComponentToAdd);
            // Init Component (and subscribe to Event Distpatcher delegates). In runtime, the init is handled inside IMLComponent.Awake()
            InitIMLComponent(ref newComponentToAdd);
        }
    }

    /// <summary>
    /// Unsubscribes an iml component from the list
    /// </summary>
    /// <param name="componentToRemove"></param>
    public static void UnsubscribeIMLComponent(IMLComponent componentToRemove)
    {
        // Make sure the list is initialised
        if (m_IMLComponents == null)
        {
            m_IMLComponents = new List<IMLComponent>();
        }

        // Make sure the list contains already the component we want to remove
        if (m_IMLComponents.Contains(componentToRemove))
        {
            // We remove the component from the list
            m_IMLComponents.Remove(componentToRemove);
        }


    }

    /// <summary>
    /// Subscribes an imlcomponent to the list (avoiding duplicates)
    /// </summary>
    /// <param name="newAddon"></param>
    public static void SubscribeIMLAddon(IAddonIML newAddon)
    {
        // Make sure the list is initialised
        if (m_IMLAddons == null)
        {
            m_IMLAddons = new List<IAddonIML>();
        }

        // Make sure the list doesn't contain already the component we want to add
        if (!m_IMLAddons.Contains(newAddon))
        {
            // We add the component if it it is not in the list already
            m_IMLAddons.Add(newAddon);
            // Init IML Addon
            newAddon.Initialize();


        }
    }

    /// <summary>
    /// Unsubscribes an iml component from the list
    /// </summary>
    /// <param name="componentToRemove"></param>
    public static void UnsubscribeIMLAddon(IAddonIML componentToRemove)
    {
        // Make sure the list is initialised
        if (m_IMLAddons == null)
        {
            m_IMLAddons = new List<IAddonIML>();
        }

        // Make sure the list contains already the component we want to remove
        if (m_IMLAddons.Contains(componentToRemove))
        {
            // We remove the component from the list
            m_IMLAddons.Remove(componentToRemove);
        }
    }

    #endregion


#if UNITY_EDITOR

    /// <summary>
    /// Creates an IML System GameObject in the scene
    /// </summary>
    [MenuItem("GameObject/InteractML/IML System", false, 10)]
    public static void CreateIMLSystem(MenuCommand menuCommand)
    {
        // Create a custom IML game object
        GameObject imlSystem = new GameObject("IML System");
        // Add IML Component
        var imlComponent = imlSystem.AddComponent<IMLComponent>();
        // Any external logic to needs to access the creation of IMLComponent (i.e. addons)
        IMLSystemCreatedCallback.Invoke(imlComponent);
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(imlSystem, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(imlSystem, "Create " + imlSystem.name);
        Selection.activeObject = imlSystem;
    }

#endif

}
