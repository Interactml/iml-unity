﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(IntegerNode))]
    public class IntegerNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private IntegerNode m_IntegerNode;

        public override void OnHeaderGUI()
        {
            // Get reference to the current node
            m_IntegerNode = (target as IntegerNode);
            NodeName = "LIVE INT DATA";
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            InputPortsNamesOverride = new Dictionary<string, string>();
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Int\nData In");
            base.OutputPortsNamesOverride.Add("m_Out", "Int\nData Out");
            base.nodeTips = m_IntegerNode.tooltips;
            m_BodyRect.height = 80;
            base.OnBodyGUI();

        }

        protected override void ShowBodyFields()
        {
            DataInToggle(m_IntegerNode.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }
        protected override void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            // If something is connected to the input port show incoming data
            if (m_IntegerNode.InputConnected)
            {
                m_IntegerNode.int_switch = EditorGUILayout.Toggle(m_IntegerNode.int_switch, style);
                EditorGUILayout.LabelField("Int: " + System.Math.Round(m_IntegerNode.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
                
            }
            // If there is nothing connected to the input port show editable fields
            else
            {
                m_IntegerNode.int_switch = EditorGUILayout.Toggle(m_IntegerNode.int_switch, style, GUILayout.MaxWidth(dataWidth));
                EditorGUILayout.LabelField("Int: ", m_NodeSkin.GetStyle("Node Body Label"), GUILayout.MaxWidth(dataWidth));
                m_IntegerNode.m_UserInput = EditorGUILayout.IntField(m_IntegerNode.m_UserInput, GUILayout.MaxWidth(inputWidth));
                
            }
            GUILayout.EndHorizontal();
        }

    }
}

