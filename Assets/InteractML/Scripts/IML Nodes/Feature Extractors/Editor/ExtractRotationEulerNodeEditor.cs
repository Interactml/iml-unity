﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    [CustomNodeEditor(typeof(ExtractRotationEuler))]
    public class ExtractRotationEulerNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private ExtractRotationEuler m_ExtractRotationEuler;

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_ExtractRotationEuler = (target as ExtractRotationEuler);

            // Initialise node name
            NodeName = "LIVE ROTATION DATA";
            NodeSubtitle = "Euler";

            // Initialise node height
            m_BodyRect.height = 150;
            nodeSpace = 150;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("GameObjectDataIn", "Game Object\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("LiveDataOut", "Rotation\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_ExtractRotationEuler.tooltips;

            // Initialise axis labels
            feature_labels = new string[3] { "x: ", "y: ", "z: " };

        }


        protected override void ShowBodyFields()
        {
            GUILayout.Space(60);
            // set body space based on node editors rects 
            GUILayout.BeginArea(m_BodyRect);
            GUILayout.Space(20);

            //draws node data fields
            FeatureExtractorEditorMethods.DrawFeatureValueToggleAndLabel(this, m_ExtractRotationEuler, m_ExtractRotationEuler.FeatureValues.Values.Length, IMLNodeEditorMethods.DataInToggle(this, m_ExtractRotationEuler.ReceivingData));

            GUILayout.Space(10);
            //draw toggle to select whether to use localspace
            m_ExtractRotationEuler.LocalSpace = FeatureExtractorEditorMethods.DrawLocalSpaceToggle(this, m_ExtractRotationEuler.LocalSpace);

            GUILayout.EndArea();
        }

        

    }

}
