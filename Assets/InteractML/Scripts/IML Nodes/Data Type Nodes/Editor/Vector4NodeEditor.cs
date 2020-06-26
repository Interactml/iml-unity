using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    [CustomNodeEditor(typeof(Vector4Node))]
    public class Vector4NodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private Vector4Node m_Vector4Node;

        public override void OnHeaderGUI()
        {
            m_Vector4Node = (target as Vector4Node);
            NodeName = "LIVE VECTOR 4 DATA";
            nodeSpace = 60;
            base.OnHeaderGUI();
        }

        public override void OnBodyGUI()
        {
            InputPortsNamesOverride = new Dictionary<string, string>();
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("m_In", "Vector3\nData In");
            base.OutputPortsNamesOverride.Add("m_Out", "Vector3\nData Out");
            base.nodeTips = m_Vector4Node.tooltips;
            bodyheight = 160;
            base.OnBodyGUI();
        }

        protected override void ShowBodyFields()
        {
            DataInToggle(m_Vector4Node.ReceivingData, m_InnerBodyRect, m_BodyRect);
        }

        protected override void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            // If something is connected to the input port show incoming data
            if (m_Vector4Node.InputConnected)
            {
                GUILayout.BeginHorizontal();
                m_Vector4Node.x_switch = EditorGUILayout.Toggle(m_Vector4Node.x_switch, style);
                EditorGUILayout.LabelField("x: " + System.Math.Round(m_Vector4Node.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                m_Vector4Node.y_switch = EditorGUILayout.Toggle(m_Vector4Node.y_switch, style);
                EditorGUILayout.LabelField("y: " + System.Math.Round(m_Vector4Node.FeatureValues.Values[1], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                m_Vector4Node.z_switch = EditorGUILayout.Toggle(m_Vector4Node.z_switch, style);
                EditorGUILayout.LabelField("z: " + System.Math.Round(m_Vector4Node.FeatureValues.Values[2], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                m_Vector4Node.w_switch = EditorGUILayout.Toggle(m_Vector4Node.w_switch, style);
                EditorGUILayout.LabelField("w: " + System.Math.Round(m_Vector4Node.FeatureValues.Values[3], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
                GUILayout.EndHorizontal();

            }
            // If there is nothing connected to the input port show editable fields
            else
            {
                GUILayout.BeginHorizontal();
                m_Vector4Node.x_switch = EditorGUILayout.Toggle(m_Vector4Node.x_switch, style, GUILayout.MaxWidth(40));
                EditorGUILayout.LabelField("x: ", m_NodeSkin.GetStyle("Node Body Label"), GUILayout.MaxWidth(20));
                m_Vector4Node.m_UserInput.x = EditorGUILayout.FloatField(m_Vector4Node.m_UserInput.x, GUILayout.MaxWidth(60));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                m_Vector4Node.y_switch = EditorGUILayout.Toggle(m_Vector4Node.y_switch, style, GUILayout.MaxWidth(40));
                EditorGUILayout.LabelField("y: ", m_NodeSkin.GetStyle("Node Body Label"), GUILayout.MaxWidth(20));
                m_Vector4Node.m_UserInput.y = EditorGUILayout.FloatField(m_Vector4Node.m_UserInput.y, GUILayout.MaxWidth(60));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                m_Vector4Node.z_switch = EditorGUILayout.Toggle(m_Vector4Node.z_switch, style, GUILayout.MaxWidth(40));
                EditorGUILayout.LabelField("z: ", m_NodeSkin.GetStyle("Node Body Label"), GUILayout.MaxWidth(20));
                m_Vector4Node.m_UserInput.z = EditorGUILayout.FloatField(m_Vector4Node.m_UserInput.z, GUILayout.MaxWidth(60));
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                m_Vector4Node.w_switch = EditorGUILayout.Toggle(m_Vector4Node.w_switch, style, GUILayout.MaxWidth(40));
                EditorGUILayout.LabelField("w: ", m_NodeSkin.GetStyle("Node Body Label"), GUILayout.MaxWidth(20));
                m_Vector4Node.m_UserInput.w = EditorGUILayout.FloatField(m_Vector4Node.m_UserInput.w, GUILayout.MaxWidth(60));
                GUILayout.EndHorizontal();

            }

            

        }

        /// <summary>
        /// Show the serial value fields with attribute name
        /// </summary>
        //private void ShowVector4Values()
        //{
        //    m_InnerBodyRect.x = m_BodyRect.x + 20;
        //    m_InnerBodyRect.y = m_BodyRect.y + 20;
        //    m_InnerBodyRect.width = m_BodyRect.width - 20;
        //    m_InnerBodyRect.height = m_BodyRect.height - 20;

        //    GUILayout.BeginArea(m_InnerBodyRect);
        //    EditorGUILayout.Space();
        //    EditorGUILayout.LabelField("Name: " + m_Vector4Node.ValueName, m_NodeSkin.GetStyle("Node Body Label"));
        //    EditorGUILayout.Space();
        //    EditorGUILayout.LabelField("x: " + System.Math.Round(m_Vector4Node.FeatureValues.Values[0], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
        //    EditorGUILayout.Space();
        //    EditorGUILayout.LabelField("y: " + System.Math.Round(m_Vector4Node.FeatureValues.Values[1], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
        //    EditorGUILayout.Space();
        //    EditorGUILayout.LabelField("z: " + System.Math.Round(m_Vector4Node.FeatureValues.Values[2], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));
        //    EditorGUILayout.Space();
        //    EditorGUILayout.LabelField("w: " + System.Math.Round(m_Vector4Node.FeatureValues.Values[3], 3).ToString(), m_NodeSkin.GetStyle("Node Body Label"));

        //    GUILayout.EndArea();

        //}

        /// <summary>
        /// Display help button
        /// </summary>
        private void ShowHelpButton()
        {
            m_HelpRect.x = m_HelpRect.x + 20;
            m_HelpRect.y = m_HelpRect.y + 10;
            m_HelpRect.width = m_HelpRect.width - 30;

            GUILayout.BeginArea(m_HelpRect);
            GUILayout.BeginHorizontal();
            GUILayout.Label("");
            GUILayout.Button("Help", m_NodeSkin.GetStyle("Help Button"));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}

