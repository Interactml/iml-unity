using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractVector4))]
    public class ExtractVector4NodeEditor : NodeEditor
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
