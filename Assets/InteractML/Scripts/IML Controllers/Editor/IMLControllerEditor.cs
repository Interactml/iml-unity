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
    }

}
