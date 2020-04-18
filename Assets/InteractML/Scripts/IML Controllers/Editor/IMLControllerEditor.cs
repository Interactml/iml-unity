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

            // Set the background color and highlight color
            NodeEditorPreferences.GetSettings().highlightColor = hexToColor("21203B");
            NodeEditorPreferences.GetSettings().gridLineColor = hexToColor("21203B");
            NodeEditorPreferences.GetSettings().gridBgColor = hexToColor("21203B");
            
            //Clone keys so we can enumerate the dictionary and make changes.
            var typeColorKeys = new List<Type>(typeColors.Keys);

            //Display type colors. Save them if they are edited by the user
            foreach (var type in typeColorKeys) {
                string typeColorKey = NodeEditorUtilities.PrettyName(type);
                Color col = typeColors[type];
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                col = EditorGUILayout.ColorField(typeColorKey, col);
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck()) {
                    typeColors[type] = col;
                    if (settings.typeColors.ContainsKey(typeColorKey)) settings.typeColors[typeColorKey] = col;
                    else settings.typeColors.Add(typeColorKey, col);
                    SavePrefs(key, settings);
                    NodeEditorWindow.RepaintAll();
                }
            
        }


        public override NodeEditorPreferences.Settings GetDefaultPreferences()
        {
            return new NodeEditorPreferences.Settings()
            {
                gridBgColor = hexToColor("21203B"),
                gridLineColor = hexToColor("21203B"),
                highlightColor = hexToColor("21203B"),
                typeColors = 
            };
        }

        public static Color hexToColor(string hex)
        {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return new Color32(r, g, b, a);
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
