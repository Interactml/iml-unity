using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.MovementFeatures
{
    [CustomNodeEditor(typeof(RotationQuaternionNode))]
    public class ExtractRotationQuaternionNodeEditor : IMLNodeEditor
    {
        /// <summary>
        /// Reference to the node itself
        /// </summary>
        private RotationQuaternionNode m_ExtractRotationQuaternion;

        /// <summary>
        /// Initialise node specific interface labels and parameters
        /// </summary>
        public override void OnCreate()
        {
            // Get reference to the current node
            m_ExtractRotationQuaternion = (target as RotationQuaternionNode);

            // Initialise node name
            NodeName = "LIVE ROTATION DATA";
            NodeSubtitle = "Quaternion";

            // Initialise node height
            m_BodyRect.height = 180;
            nodeSpace = 180;

            // Initialise input port labels
            InputPortsNamesOverride = new Dictionary<string, string>();
            base.InputPortsNamesOverride.Add("GameObjectDataIn", "Game Object\nData In");

            // Initialise output port labels
            OutputPortsNamesOverride = new Dictionary<string, string>();
            base.OutputPortsNamesOverride.Add("LiveDataOut", "Rotation\nData Out");

            // Initialise node tooltips
            base.nodeTips = m_ExtractRotationQuaternion.tooltips;
            
            // Initialise axis labels
            feature_labels = new string[4] { "x: ", "y: ", "z: ", "w: " };
        }


        protected override void ShowBodyFields()
        {
            GUILayout.Space(60);
            // set body space based on node editors rects 
            GUILayout.BeginArea(m_BodyRect);
            GUILayout.Space(20);

            //draws node data fields
            MovementFeatureEditorMethods.DrawFeatureValueToggleAndLabel(this, m_ExtractRotationQuaternion);
            
            // commented out as not using local space toggle
            //GUILayout.Space(10);
            //draw toggle to select whether to use localspace
            //m_ExtractRotationQuaternion.LocalSpace = MovementFeatureEditorMethods.DrawLocalSpaceToggle(this, m_ExtractRotationQuaternion.LocalSpace);

            GUILayout.EndArea();
        }

    }

}
