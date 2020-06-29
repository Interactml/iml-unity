using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
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

    static IMLEditorManager()
    {
#if UNITY_EDITOR
        // Subscribe this manager to the editor update loop
        EditorApplication.update += UpdateLogic;

        // Make sure the list is init
        if (m_IMLComponents == null)
            m_IMLComponents = new List<IMLComponent>();

        // When the project starts for the first time, we find the iml components present in that scene
        FindIMLComponents();

        //Debug.Log("New IMLEditorManager created in scene " + EditorSceneManager.GetActiveScene().name);
        
        // Subscribe manager event to the sceneOpened event
        EditorSceneManager.sceneOpened += SceneOpenedLogic;

        // Subscribe manager event to the playModeStateChanged event
        EditorApplication.playModeStateChanged += PlayModeStateChangedLogic;
#endif
    }

    private static void UpdateLogic()
    {
#if UNITY_EDITOR

        // Only run update logic when the app is not running (outside playmode or paused)
        if (!EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            if (m_IMLComponents != null && m_IMLComponents.Count > 0)
            {
                //Debug.Log("IML Components number: " + m_IMLComponents.Count);

                // Run each of the updates in the iml components
                foreach (var MLcomponent in m_IMLComponents)
                {
                    //Debug.Log("**EDITOR**");
                    MLcomponent.UpdateLogic();
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
        Debug.Log("SceneOpened");
        ClearIMLComponents();
        FindIMLComponents();
        // Reload all models (if we can) when we enter playmode or when we come back to the editor
        foreach (var MLComponent in m_IMLComponents)
        {
            // Reload models
            MLComponent.LoadAllModelsFromDisk(reCreateModels: true);
            // Run them (if marked with RunOnAwake)
        }

    }

    /// <summary>
    /// When we change playmode, we make sure to reset all iml models
    /// </summary>
    /// <param name="playModeStatus"></param>
    private static void PlayModeStateChangedLogic(PlayModeStateChange playModeStatus)
    {
        #region Enter Events

        // We load models if we are entering a playmode (not required when leaving playmode)
        if (playModeStatus == PlayModeStateChange.EnteredPlayMode)
        {
            // Reload all models (if we can) when we enter playmode or when we come back to the editor
            foreach (var MLComponent in m_IMLComponents)
            {
                // Reload models
                MLComponent.LoadAllModelsFromDisk();
                // Run them (if marked with RunOnAwake)
                MLComponent.RunAllModels();
            }
            //Debug.Log("**Models reconfigured in editor status: " + playModeStatus + "**");
        }

        if (playModeStatus == PlayModeStateChange.EnteredEditMode)
        {
            foreach (var MLComponent in m_IMLComponents)
            {
                MLComponent.updateGameObjectImage();
                MLComponent.GetAllNodes();
                MLComponent.UpdateScriptNodes(changingPlayMode: true);
            }
        }

        #endregion

        #region Exit Events

        // Remove any scriptNodes added during playtime when leaving playMode
        if (playModeStatus == PlayModeStateChange.ExitingPlayMode)
        {
            foreach (var MLComponent in m_IMLComponents)
            {
                MLComponent.UpdateScriptNodes(changingPlayMode: true);
            }
        }

        // We save models if we are leaving a playmode or editormode
        if (playModeStatus == PlayModeStateChange.ExitingEditMode || playModeStatus == PlayModeStateChange.ExitingPlayMode)
        {
            foreach (var MLComponent in m_IMLComponents)
            {
                MLComponent.StopAllModels();
                MLComponent.SaveAllModels();
            }
        }

        #endregion

    }

#endif

    /// <summary>
    /// Clears the entire list of iml components (to be called when a new scene loads for example)
    /// </summary>
    private static void ClearIMLComponents()
    {
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
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(imlSystem, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(imlSystem, "Create " + imlSystem.name);
        Selection.activeObject = imlSystem;
    }
}
