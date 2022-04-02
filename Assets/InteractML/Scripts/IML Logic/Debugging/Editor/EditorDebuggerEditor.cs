using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InteractML.Debugging
{
    [CustomEditor(typeof(EditorDebugger))]
    public class EditorDebuggerEditor : Editor
    {
        EditorDebugger m_Debugger;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            m_Debugger = target as EditorDebugger;

            // BUTTONS
            if (GUILayout.Button("PrintIMLEventToggleRecordMethods"))
            {
                EditorDebugger.PrintIMLEventToggleRecordMethods();
            }

            if (GUILayout.Button("PrintIMLEventStartRecordMethods"))
            {
                EditorDebugger.PrintIMLEventStartRecordMethods();
            }

            if (GUILayout.Button("PrintIMLEventStopRecordMethods"))
            {
                EditorDebugger.PrintIMLEventStopRecordMethods();
            }

        }
    }

}
