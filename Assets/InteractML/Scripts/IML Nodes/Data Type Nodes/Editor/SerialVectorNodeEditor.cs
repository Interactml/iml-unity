﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(SerialVectorNode))]
    public class SerialVectorNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private SerialVectorNode m_SerialVectorNode;

        public override void OnHeaderGUI()
        {
            m_SerialVectorNode = (target as SerialVectorNode);
            NodeName = "LIVE SERIAL VECTOR DATA";
            nodeSpace = 120 + (m_ConnectedInputs * 20);
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            InputPortsNamesOverride = new Dictionary<string, string>();
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Serial Vector\nData In");
            base.OutputPortsNamesOverride.Add("m_Out", "Serial Vector \nData Out");
            base.nodeTips = m_SerialVectorNode.tooltips;
            m_ConnectedInputs = m_SerialVectorNode.FeatureValues.Values.Length;
            m_BodyRect.height = 60 + (m_ConnectedInputs * 20);
            base.OnBodyGUI();
        }


        /// <summary>
        /// Show the serial value fields with attribute name
        /// </summary>
        protected override void ShowBodyFields()
        {
            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);

            if (m_SerialVectorNode.FeatureValues.Values.Length == 0 || m_SerialVectorNode.FeatureValues.Values == null)
            {
                EditorGUILayout.LabelField("Connect an array", Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
            }
            else
            { 
                for (int i = 0; i < m_SerialVectorNode.FeatureValues.Values.Length; i++)
                {
                    EditorGUILayout.LabelField("Element " + i + ":  " + System.Math.Round(m_SerialVectorNode.FeatureValues.Values[i], 3).ToString(), Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin").GetStyle("Node Body Label"));
                }
            }
            GUILayout.EndArea();

        }
    }
}

