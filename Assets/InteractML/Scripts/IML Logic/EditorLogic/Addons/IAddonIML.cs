using UnityEditor;
using UnityEngine;

namespace InteractML.Addons
{
    /// <summary>
    /// Interface that specifies what public methods addons should expose for external consumption
    /// </summary>
    public interface IAddonIML 
    {
        /// <summary>
        /// Is addon instance initialized?
        /// </summary>
        /// <returns></returns>
        bool IsInit();

        /// <summary>
        /// Initializes addon
        /// </summary>
        void Initialize();

        /// <summary>
        /// Adds the addon component to a gameObject (i.e. the IMLComponent)
        /// </summary>
        /// <param name="GO"></param>
        void AddAddonToGameObject(GameObject GO);

        #region Consumed by IMLEditorManager

        // The following are methods consumed by the IMLEditorManager
        void EditorUpdateLogic();
        void EditorSceneOpened();
        void EditorEnteredPlayMode();
        void EditorEnteredEditMode();
        void EditorExitingPlayMode();
        void EditorExitingEditMode();

        #endregion

    }
}