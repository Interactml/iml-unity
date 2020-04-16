using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeGraphEditor(typeof(IMLController))]
    public class IMLControllerEditor : NodeGraphEditor
    {

        public override void OnOpen()
        {
            base.OnOpen();
           
            window.titleContent.text = "InteractML";
            
        }


        public override NodeEditorPreferences.Settings GetDefaultPreferences()
        {
            return new NodeEditorPreferences.Settings()
            {
                gridBgColor = Color.white,
                gridLineColor = Color.white
            };
        }

        /// <summary> 
        /// Overriding GetNodeMenuName lets you control if and how nodes are categorized.
        /// In this example we are sorting out all node types that are not in the XNode.Examples namespace.
        /// </summary>
        public override string GetNodeMenuName(System.Type type)
        {
            if (type.Namespace == "InteractML")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.Namespace == "InteractML.FeatureExtractors")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            else return null;
        }

    }

}
