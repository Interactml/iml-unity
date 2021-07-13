﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.ControllerCustomisers
{
    /// <summary>
    /// 
    /// </summary>
    [CustomNodeEditor(typeof(VRTrigger))]
    public class VRTriggerEditor : CustomControllerEditor
    {

        VRTrigger press;
        int count = 0;
        int counter = 4;

        public override void OnBodyGUI()
        {
            press = target as VRTrigger;
            base.OnBodyGUI();
        }

        protected override void ShowBodyFields()
        {
            base.ShowBodyFields();
            nodeSpace = 50;
            GUILayout.Space(-80);
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label("Controller", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Port Label"), GUILayout.MinWidth(100));
            press.hand = (IMLSides)EditorGUILayout.EnumPopup(press.hand);
            if (GUI.changed)
            {
                press.OnSideChange();
                //mark as changed
                EditorUtility.SetDirty(press);
            }
            GUILayout.EndHorizontal();
        }
    }
}



