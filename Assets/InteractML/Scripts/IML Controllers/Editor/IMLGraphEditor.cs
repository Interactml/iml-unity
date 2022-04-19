using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using XNodeEditor;
#endif

namespace InteractML
{
    [CustomNodeGraphEditor(typeof(IMLGraph))]
    public class IMLGraphEditor : NodeGraphEditor
    {
        // reference to the graph
        private IMLGraph graph;

        /// <summary>
        /// initialise graph reference when created
        /// </summary>
        public override void OnCreate()
        {
            // get reference to current graph
            graph = (target as IMLGraph);
            base.OnCreate();
        }


        public override void OnOpen()
        {
            base.OnOpen();

            window.titleContent.text = "InteractML";

            // Set the background color and highlight color
            NodeEditorPreferences.GetSettings().highlightColor = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().gridLineColor = hexToColor("21203B");
            NodeEditorPreferences.GetSettings().gridBgColor = hexToColor("21203B");

            // Set port colours based on type
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(GameObject))] = hexToColor("#E24680");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(float))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(int))] = hexToColor("#888EF7");
            //NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(bool))] = hexToColor("#F6C46F");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(float[]))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(Vector2))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(Vector3))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(Vector4))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(XNode.Node))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(List<XNode.Node>))] = hexToColor("#888EF7");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(TrainingExamplesNode))] = hexToColor("#74DF84");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(List<TrainingExamplesNode>))] = hexToColor("#74DF84");
            NodeEditorPreferences.GetSettings().typeColors[NodeEditorUtilities.PrettyName(typeof(MLSystem))] = hexToColor("#5EB3F9");
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

            /* ML SYSTEMS */
            if (type.PrettyName() == "InteractML.ClassificationMLSystem")
            {
                return base.GetNodeMenuName(type).Replace("InteractML.ClassificationMLSystem", "Machine Learning System - Classification");
            }
            if (type.PrettyName() == "InteractML.RegressionMLSystem")
            {
                return base.GetNodeMenuName(type).Replace("InteractML.RegressionMLSystem", "Machine Learning System - Regression");
            }
            if (type.PrettyName() == "InteractML.DTWMLSystem")
            {
                return base.GetNodeMenuName(type);
            }
            //commented outas gameobject should be controlled in the inspector
            //if (type.PrettyName() == "InteractML.GameObjectNode")
            //{
            //    return base.GetNodeMenuName(type).Replace("InteractML", "");
            //}

            /* TRAINING EXAMPLES & DATASETS */
            if (type.PrettyName() == "InteractML.SeriesTrainingExamplesNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.SingleTrainingExamplesNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            // Node to hold several single training Examples
            if (type.PrettyName() == "InteractML.TrainingDataSetsNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }


            /* TEXT NOTES */
            if (type.PrettyName() == "InteractML.TextNote")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }

            /* MOVEMENT FEATURES */
            if (type.PrettyName() == "InteractML.GameObjectMovementFeatures.DistanceToFirstInputNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.GameObjectMovementFeatures.PositionNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.GameObjectMovementFeatures.RotationQuaternionNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.GameObjectMovementFeatures.RotationEulerNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.GameObjectMovementFeatures.VelocityNode")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.DataTypeNodes.BooleanNode")
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
            if (type.PrettyName() == "InteractML.DataTypeNodes.ArrayNode")
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
            /* if (type.PrettyName() == "InteractML.ControllerCustomisers.MouseClick")
             {
                 return base.GetNodeMenuName(type).Replace("InteractML", "");
             }*/
             if (type.PrettyName() == "InteractML.ControllerCustomisers.VRTrigger")
             {
                 return base.GetNodeMenuName(type).Replace("InteractML", "");
             }
             if (type.PrettyName() == "InteractML.ControllerCustomisers.KeyboardPress")
             {
                 return base.GetNodeMenuName(type).Replace("InteractML", "");
             }

            /* CONTROLLER CUSTOMISERS */
            if (type.PrettyName() == "InteractML.ControllerCustomisers.InputSetUp")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
            if (type.PrettyName() == "InteractML.CustomControllers.InputSetUp")
              {
                  return base.GetNodeMenuName(type).Replace("InteractML", "");
              }
              if (type.PrettyName() == "InteractML.CustomControllers.VRTrigger")
              {
                  return base.GetNodeMenuName(type).Replace("InteractML", "");
              }
            if (type.PrettyName() == "InteractML.ControllerCustomisers.KeyboardPress")
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }

#if MECM
            // This nodes are exclusive to the MECM that Carlos is working on part-time. It won't compile on any other InteractML version
            if (type.PrettyName().Contains("MECM."))
            {
                return base.GetNodeMenuName(type).Replace("InteractML", "");
            }
#endif

            else return null;
        }

        /// <summary> Safely remove a node and all its connections. </summary>
        public override void RemoveNode(XNode.Node node)
        {
            string name = node.GetType().ToString();
            //find out if the node is a feature extractor or data type node connected to training examples with trained data
            if (name.Contains("MovementFeatures") || name.Contains("DataType"))
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

        /// <summary>
        /// Add gameObject nodes when dragged into a graph from scene hierarchy
        /// </summary>
        /// <param name="objects"></param>
        public override void OnDropObjects(UnityEngine.Object[] objects)
        {
            // for every object dragged into graph
            foreach (UnityEngine.Object ob in objects)
            {
                var type = ob.GetType();
                // check if the object dragged into the graph is a gameobject type
                if (ob.GetType() == typeof(GameObject))
                {
                    // check if the gameobject is in the scene hierarchy
                    if ((ob as GameObject).scene.IsValid())
                    {
                        // create a new gameObject node and add it to the internal gameobject node list
                        var goNode = graph.AddNode<GameObjectNode>();
                        // set position of node to dropped position
                        goNode.position = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
                        // configure node with game object dropped in
                        goNode.SetGameObject((GameObject)ob);
                        // add gameobject to internal list of game objects
                        graph.SceneComponent.GameObjectsToUse.Add((GameObject)ob);
                        // add to internal dictionary of goNodes and gameobjects
                        graph.SceneComponent.AddToGameObjectNodeDictionary(goNode, (GameObject)ob);
                    }
                }
                else if (ob is MonoBehaviour)
                {
                    var script = ob as MonoBehaviour;
                    // check if the script is in a gameobject and the GO is in the scene hierarchy
                    if (script.gameObject != null && script.gameObject.scene.IsValid())
                    {
                        // Create a new scriptNode and add it to the internal scriptNode list
                        var scriptNode = graph.AddNode<ScriptNode>();
                        // set position of node to dropped position
                        scriptNode.position = NodeEditorWindow.current.WindowToGridPosition(Event.current.mousePosition);
                        // configure node with script dropped in
                        scriptNode.SetScript(script);
                        // add to internal list of scripts
                        graph.SceneComponent.AddScriptNode(scriptNode);
                        // add to internal dictionary of scriptNodes and scripts
                        graph.SceneComponent.AddToScriptNodeDictionaries(scriptNode, script);
                        
                    }
                }
                else
                {
                    base.OnDropObjects(objects);
                }
            }
        }
        public void OnInspectorUpdate()
        {
            Debug.Log("inspector update");
            window.Repaint();
        }
    }
}
