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
    /// <summary>
    /// Public external editor callbacks for scene opened
    /// </summary>
    public static EditorSceneManager.SceneOpenedCallback SceneOpenedCallbacks;
    /// <summary>
    /// Public external editor callbacks for update
    /// </summary>
    public static EditorApplication.CallbackFunction UpdateCallbacks;
    /// <summary>
    /// Public external editor callbacks for playmodeStateChanged
    /// </summary>
    public static System.Action<PlayModeStateChange> PlayModeStateChangedCallbacks;
#endif

    static IMLEditorManager()
    {
#if UNITY_EDITOR
        // Make sure the list is init
        if (m_IMLComponents == null)
            m_IMLComponents = new List<IMLComponent>();

        // When the project starts for the first time, we find the iml components present in that scene
        FindIMLComponents();
        // Also find addons
        FindIMLAddons();

        // Subscribe this manager to the editor update loop
        EditorApplication.update += UpdateLogic;
        EditorApplication.update += UpdateCallbacks; // External


        //Debug.Log("New IMLEditorManager created in scene " + EditorSceneManager.GetActiveScene().name);
        
        // Subscribe manager event to the sceneOpened event
        EditorSceneManager.sceneOpened += SceneOpenedLogic;
        EditorSceneManager.sceneOpened += SceneOpenedCallbacks; // External

        // Subscribe manager event to the playModeStateChanged event
        EditorApplication.playModeStateChanged += PlayModeStateChangedLogic;
        EditorApplication.playModeStateChanged += PlayModeStateChangedCallbacks; // External


        
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
        ClearIMLComponents();
        FindIMLComponents();
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
        ClearIMLAddons();
        FindIMLAddons();
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
        // Repair list of known iml components if any of them is null
        if (NullIMLComponents()) RepairIMLComponents();

        foreach (IMLComponent MLComp in m_IMLComponents) 
        {
            foreach (MLSystem MLS in MLComp.MLSystemNodeList) {
                if (MLS != null)
                    MLS.UIErrors();
                else
                    Debug.LogWarning("Null reference to a MLComponent in IMLEditorManager.PlayModeStateChangedLogic()");
            }
        }

        // Repair list of addons
        if (NullIMLAddons()) RepairIMLAddons();

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
                if (addon != null)
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
        // Clear private list
        m_IMLComponents.Clear();
    }

    /// <summary>
    /// Finds all the iml components already present in the scene (to be called after 
    /// </summary>
    private static void FindIMLComponents()
    {
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

    #endregion

    #region IMLAddons Search

    /// <summary>
    /// Clears the entire list of iml components (to be called when a new scene loads for example)
    /// </summary>
    private static void ClearIMLAddons()
    {
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


    #region IMLComponents Subscriptions

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

    #endregion


    #region IMLAddons Subscriptions

    /// <summary>
    /// Subscribes an imlcomponent to the list (avoiding duplicates)
    /// </summary>
    /// <param name="newComponentToAdd"></param>
    public static void SubscribeIMLAddon(IAddonIML newComponentToAdd)
    {
        // Make sure the list is initialised
        if (m_IMLAddons == null)
        {
            m_IMLAddons = new List<IAddonIML>();
        }

        // Make sure the list doesn't contain already the component we want to add
        if (!m_IMLAddons.Contains(newComponentToAdd))
        {
            // We add the component if it it is not in the list already
            m_IMLAddons.Add(newComponentToAdd);
            // We init it if not yet done
            if (!newComponentToAdd.IsInit()) newComponentToAdd.Initialize();
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
        imlSystem.AddComponent<IMLComponent>();
        // Add any addons that should go together with the IMLComponent
        if (m_IMLAddons != null && m_IMLAddons.Count > 0)
        {
            foreach (var addon in m_IMLAddons)
            {
                if (addon != null) addon.AddAddonToGameObject(imlSystem);
            }
        }
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(imlSystem, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(imlSystem, "Create " + imlSystem.name);
        Selection.activeObject = imlSystem;
    }

#endif

}
