using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.ControllerCustomisers
{
    /// <summary>
    /// 
    /// </summary>
    [CustomNodeEditor(typeof(KeyboardPress))]
    public class KeyboardPressEditor : CustomControllerEditor
    {

        KeyboardPress press;
        int count = 0;
        int counter = 4;

        public override void OnBodyGUI()
        {
            Event e = Event.current;
            press = target as KeyboardPress;

            if (press.handler.button == e.keyCode && e.keyCode != KeyCode.None) {
                if(count == 0)
                    press.handler.HandleStateEditor();
                count++;
            }
            if (count == 4)
            {
                count = 0;
            }
           
            base.OnBodyGUI();
        }


    }
}



