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
            base.GetDefaultPreferences();
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
    }

}
