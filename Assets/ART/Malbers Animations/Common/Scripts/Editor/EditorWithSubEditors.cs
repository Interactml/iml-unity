using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MalbersAnimations
{
    // This class acts as a base class for Editors that have Editors
    // nested within them.  For example, the InteractableEditor has
    // an array of ConditionCollectionEditors.
    // It's generic types represent the type of Editor array that are
    // nested within this Editor and the target type of those Editors.
    public abstract class EditorWithSubEditors<TEditor, TTarget> : Editor
        where TEditor : Editor
        where TTarget : Object
    {
        protected TEditor subEditor;
        protected TTarget oldTarget;


        protected List<TEditor> subEditors;         // Array of Editors nested within this Editor.


        // This should be called in OnEnable and at the start of OnInspectorGUI.
        protected void CheckAndCreateSubEditors(List<TTarget> subEditorTargets)
        {
            // If there are the correct number of subEditors then do nothing.
            if (subEditors != null && subEditors.Count == subEditorTargets.Count)
                return;

           

            // Otherwise get rid of the editors.
            CleanupEditors();

            // Create an array of the subEditor type that is the right length for the targets.
            // subEditors = new List<TEditor[subEditorTargets.Count];
            subEditors = new List<TEditor>();
            // Populate the array and setup each Editor.

            for (int i = 0; i < subEditorTargets.Count; i++)
            {
                //subEditors[i] = CreateEditor(subEditorTargets[i]) as TEditor;
                subEditors.Add(CreateEditor(subEditorTargets[i]) as TEditor);
                SubEditorSetup(subEditors[i]);
            }
        }


        // This should be called in OnDisable.
        protected void CleanupEditors()
        {
            // If there are no subEditors do nothing.
            if (subEditors == null)
                return;

            foreach (var item in subEditors)
            {
                DestroyImmediate(item);
            }

            //// Otherwise destroy all the subEditors.
            //for (int i = 0; i < subEditors.Count; i++)
            //{
            //    DestroyImmediate(subEditors[i]);
            //}

            // Null the array so it's GCed.
            subEditors = null;
        }

        protected void CreateSubEditor(TTarget targetEditor)
        {
            if (oldTarget == targetEditor) return;

            CleanupEditor();

            subEditor = CreateEditor(targetEditor) as TEditor;
            oldTarget = targetEditor;
            SubEditorSetup(subEditor);
        }

        // This should be called in OnDisable.
        protected void CleanupEditor()
        {
            // If there are no subEditors do nothing.
            if (subEditor == null)
                return;

             //DestroyImmediate(subEditor);

            // Null the array so it's GCed.
            subEditor = null;
        }


        // This must be overridden to provide any setup the subEditor needs when it is first created.
        protected abstract void SubEditorSetup(TEditor editor);
    }
}