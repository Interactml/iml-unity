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

            // Set port colours based on type
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(GameObject))] = hexToColor("#E24680");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(float))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(int))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(float[]))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(Vector2))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(Vector3))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(Vector4))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(XNode.Node))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(List<XNode.Node>))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(TrainingExamplesNode))] = hexToColor("#74DF84");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(List<TrainingExamplesNode>))] = hexToColor("#74DF84");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(IMLConfiguration))] = hexToColor("#5EB3F9");
            NodeEditorPreferences.GetSettings().portTooltips = false;
        }


        public override NodeEditorPreferences.Settings GetDefaultPreferences()
        {
            return new NodeEditorPreferences.Settings()
            {
                gridBgColor = hexToColor("21203B"),
                gridLineColor = hexToColor("21203B"),
                highlightColor = hexToColor("21203B"),

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

            //if (type.Namespace == "InteractML")
            //{
            //    return base.GetNodeMenuName(type).Replace("InteractML", "");
            //}
            if (type.PrettyName() == "InteractML.CIMLConfiguration")
            {
                return base.GetNodeMenuName(type).Replace("InteractML.CIMLConfiguration", "Machine Learning System - Classification");
            }
            if (type.PrettyName() == "InteractML.RIMLConfiguration")
            {
                return base.GetNodeMenuName(type).Replace("InteractML.RIMLConfiguration", "Machine Learning System - Regression");
            }
            if (type.PrettyName() == "InteractML.DTWIMLConfiguration")
            {
                return base.GetNodeMenuName(type);
            }
            if (type.PrettyName() == "InteractML.GameObjectNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.RealtimeIMLOutputNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.SeriesTrainingExamplesNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.SingleTrainingExamplesNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.TextNote")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.FeatureExtractors.ExtractDistanceToFirstInput")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.FeatureExtractors.ExtractPosition")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.FeatureExtractors.ExtractRotation")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.FeatureExtractors.ExtractRotationEuler")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.FeatureExtractors.ExtractVelocity")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.DataTypeNodes.FloatNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.DataTypeNodes.IntegerNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.DataTypeNodes.SerialVectorNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.DataTypeNodes.Vector2Node")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.DataTypeNodes.Vector3Node")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.DataTypeNodes.Vector4Node")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            else return null;
        }

        /// <summary> Safely remove a node and all its connections. </summary>
        public override void RemoveNode(XNode.Node node)
        {
            string name = node.GetType().ToString();
            //find out if the node is a feature extractor or data type node connected to training examples with trained data
            if (name.Contains("FeatureExtractor") || name.Contains("DataType"))
            {
                foreach (XNode.NodePort output in node.Outputs)
                {
                    foreach (XNode.NodePort connected in output.GetConnections())
                    {
                        if (connected.node.name.Contains("Training"))
                        {
                            TrainingExamplesNode teNode = connected.node as TrainingExamplesNode;
                            if (teNode.TrainingExamplesVector.Count > 0)
                            {
                                Debug.Log("could not delete node");
                                return;
                            }
                        }
                    }

                }


            }
            base.RemoveNode(node);
        }

    }

}
