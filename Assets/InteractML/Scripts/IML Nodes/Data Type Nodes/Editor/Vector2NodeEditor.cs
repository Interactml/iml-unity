using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(Vector2Node))]
    public class Vector2NodeEditor : NodeEditor
    {

        private static GUIStyle editorLabelStyle;

        public override void OnBodyGUI()
        {
            if (editorLabelStyle == null) editorLabelStyle = new GUIStyle(EditorStyles.label);
            EditorStyles.label.normal.textColor = Color.white;
            base.OnBodyGUI();
            EditorStyles.label.normal = editorLabelStyle.normal;

        }
    }

}
