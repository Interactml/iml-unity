using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif
using InteractML.DataTypeNodes;
using InteractML.FeatureExtractors;

namespace InteractML
{
    public static class IMLNodeEditorMethods
    {
        
        /// <summary>
        /// Returns data toggle style (red or green) based on data in boolean 
        /// </summary>
        public static GUIStyle DataInToggle(this IMLNodeEditor nodeEditor, bool dataIn)
        {
            //if data coming in return green if not red toggle
            if (dataIn)
                return nodeEditor.m_NodeSkin.GetStyle("Green Toggle");
            else
                return nodeEditor.m_NodeSkin.GetStyle("Red Toggle");
        }
    }
}
