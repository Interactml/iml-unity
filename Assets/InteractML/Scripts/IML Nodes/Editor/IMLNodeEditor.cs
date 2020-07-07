using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using InteractML;
using XNode;
using XNodeEditor.Internal;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML
{

    /// <summary>
    /// Super class to deal with node drawing
    /// </summary>
    [CustomNodeEditor(typeof(IMLNode))]
    public class IMLNodeEditor : NodeEditor
    {

        #region Variables All Nodes 

        /// <summary>
        /// Texture2D for node color
        /// </summary>
        /// <returns></returns>
        protected Texture2D NodeColor { get; set; }
        protected Texture2D NodeTooltipColor { get; set; }

        /// <summary>
        /// Texture2D  for line color
        /// </summary>
        /// <returns></returns>
        protected Texture2D SeparatorLineColor { get; set; }

        /// <summary>
        /// Texture2D  for line color
        /// </summary>
        /// <returns></returns>
        protected Texture2D SectionTopColor { get; set; }

        /// <summary>
        /// Float value for line weight
        /// </summary>
        /// <returns></returns>
        protected float WeightOfSeparatorLine { get; set; } = 1f;

        /// <summary>
        /// Float value for line weight
        /// </summary>
        /// <returns></returns>
        protected float WeightOfSectionLine { get; set; } = 2f;

        /// <summary>
        /// Float value for node width
        /// </summary>
        /// <returns></returns>
        protected float NodeWidth { get; set; } = 250f;



        /// <summary>
        /// Rects for node layout
        /// </summary>
        protected Rect HeaderRect;
        protected Rect LineBelowHeader;
        protected Rect m_ToolRect;
        protected Rect m_BodyRect;
        protected Rect m_PortRect;
        protected Rect m_InnerBodyRect;
        protected Rect m_HelpRect;
        protected Rect m_WarningRect;
        protected Rect m_InnerWarningRect;
        protected Rect m_InnerInnerWarningRect;
        public Rect ToolTipRect;

        public float bodyheight;
        protected int dataWidth = 45;
        protected int inputWidth = 35;
        protected int bodySpace = 30;
        protected int vectorBodySpace = 50;

        public Vector2 positionPort;
        protected Vector2 scrollPosition;

        //Controls whether you see help tooltip
        public bool showHelp;
        // Controls whether you see a port tooltip
        public bool showPort = false;
        bool showporthelper;
        //Controls whether or not the reskinning of the node is automatically handled in the base IMLNodeEditor class (false to have default xNode skin, true for new IML skin)
        public bool UIReskinAuto = true;
        /// <summary>
        /// Do we need to recalculate background rects?
        /// </summary>
        protected bool m_RecalculateRects;
        //Show output type for ports 
        protected bool showOutput = false;

        /// <summary>
        /// The skin to use on the node
        /// </summary>
        protected GUISkin m_NodeSkin;

        /// <summary>
        /// Reference to this iml node
        /// </summary>
        protected IMLNode m_IMLNode;

        /// <summary>
        /// The serialized representation of this IML Node
        /// </summary>
        protected SerializedObject m_IMLNodeSerialized;

        /// <summary>
        /// The name for this node
        /// </summary>
        public string NodeName;
        public string NodeSubtitle;
        public string TooltipText = "";

        /// <summary>
        /// Number of input ports
        /// </summary>
        protected int m_NumInputs;
        /// <summary>
        /// Number of output ports
        /// </summary>
        protected int m_NumOutputs;

        /// <summary>
        /// List of port pairs to draw
        /// </summary>
        protected List<IMLNodePortPair> m_PortPairs;

        private int m_KnownNumPortPairs;


        // Dictionaries to allow the override of portFields
        protected Dictionary<string, string> InputPortsNamesOverride;
        protected Dictionary<string, string> OutputPortsNamesOverride;
        protected bool OverridePortNames = false;

        protected IMLNodeTooltips nodeTips;

        protected float nodeSpace;

        #endregion
        #region Variables MachineLearningSystemNodes
        protected Rect m_IconCenter;
        protected Rect m_ButtonsRect;

        protected bool buttonTip;
        protected bool buttonTipHelper;
        protected bool bottomButtonTip;
        protected bool bottomButtonTipHelper;

        protected Texture trainingIcon;

        protected int numberOfExamplesTrained;

        #endregion
        #region Variable TrainingExamples 
        protected int m_ConnectedInputs;
        protected int m_ConnectedTargets;
        protected Rect SepLineRect;
        #endregion
        #region XNode Messages

        public override void OnHeaderGUI()
        {
            
            // Load node skin
            if (m_NodeSkin == null)
                m_NodeSkin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

            // If we want to reskin the node
            if (UIReskinAuto)
            {
                // Get references
                m_IMLNode = target as IMLNode;
                m_IMLNodeSerialized = new SerializedObject(m_IMLNode);


                // Initialise header background Rects
                InitHeaderRects();

                NodeColor = GetColorTextureFromHexString("#3A3B5B");

                // Draw line below header
                GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#888EF7"));

                //Display Node name
                if (String.IsNullOrEmpty(NodeName))
                    NodeName = target.GetType().Name;

                GUILayout.BeginArea(HeaderRect);
                GUILayout.Label(NodeName, m_NodeSkin.GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
                if(!String.IsNullOrEmpty(NodeSubtitle))
                    GUILayout.Label(NodeSubtitle, m_NodeSkin.GetStyle("Header Small"), GUILayout.MinWidth(NodeWidth - 10));
                GUILayout.EndArea();

                GUILayout.Label("", GUILayout.MinHeight(60));

            }
            // If we want to keep xNode's default skin
            else
            {
                base.OnHeaderGUI();
            }
        }

        public override Color GetTint()
        {
            ColorUtility.TryParseHtmlString("#3A3B5B", out customNodeColor);
            return customNodeColor;
        }

        public override void OnBodyGUI()
        {
            // If we want to reskin the node
            if (UIReskinAuto)
            {
                // Unity specifically requires this to save/update any serial object.
                // serializedObject.Update(); must go at the start of an inspector gui, and
                // serializedObject.ApplyModifiedProperties(); goes at the end.
                serializedObject.Update();

                // Draw Port Section
                DrawPortLayout();
                ShowNodePorts(InputPortsNamesOverride, OutputPortsNamesOverride, showOutput);
                PortTooltip();

                // Draw Body Section
                InitBodyLayout();
                ShowBodyFields();
                if (nodeSpace == 0)
                    nodeSpace = 100;
                GUILayout.Space(nodeSpace);

                // Draw help button
                float bottomY = HeaderRect.height + m_PortRect.height + m_BodyRect.height;
                DrawHelpButtonLayout(bottomY);
                ShowHelpButton(m_HelpRect);

                serializedObject.ApplyModifiedProperties();

                // if hovering port show port tooltip
                if (showPort)
                {
                    ShowTooltip(m_PortRect, TooltipText);
                }
                //if hovering over help show tooltip 
                if (showHelp && nodeTips != null)
                {
                    ShowTooltip(m_HelpRect, nodeTips.HelpTooltip);
                }

                // Make sure we are not recalculating rects every frame
                m_RecalculateRects = false;
            }
            // If we want to keep xNode's default skin
            else
            {
                base.OnBodyGUI();
            }
        }

        #endregion

        #region Methods

        public void InitHeaderRects()
        {
            // Set header background Rect values
            HeaderRect.x = 5;
            HeaderRect.y = 0;
            HeaderRect.width = NodeWidth - 10;
            if (String.IsNullOrEmpty(NodeSubtitle))
            {
                HeaderRect.height = 60;
            } else
            {
                HeaderRect.height = 70;
            }
            

            // Set Line below header background Rect values
            LineBelowHeader.x = 5;
            LineBelowHeader.y = HeaderRect.height - WeightOfSectionLine;
            LineBelowHeader.width = NodeWidth - 10;
            LineBelowHeader.height = WeightOfSectionLine;
        }

        /// <summary>
        /// Return solid color texture from hex string
        /// </summary>
        public static Texture2D GetColorTextureFromHexString(string colorHex)
        {
            Color color = new Color();
            ColorUtility.TryParseHtmlString(colorHex, out color);
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        /// <summary> Make a simple port field- edited from XNodeEditorGUILayout to edit GUIStyle of port label</summary>
        public static void PortField(GUIContent label, XNode.NodePort port, GUIStyle style, params GUILayoutOption[] options)
        {
            if (port == null) return;
            if (options == null) options = new GUILayoutOption[] { GUILayout.MinWidth(30) };
            Vector2 position = Vector3.zero;
            GUIContent content = label != null ? label : new GUIContent(ObjectNames.NicifyVariableName(port.fieldName));



            // If property is an input, display a regular property field and put a port handle on the left side
            if (port.direction == XNode.NodePort.IO.Input)
            {
                // Display a label
                EditorGUILayout.LabelField(content, style, options);

                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position - new Vector2(15, 0);
            }
            // If property is an output, display a text label and put a port handle on the right side
            else if (port.direction == XNode.NodePort.IO.Output)
            {
                // Display a label
                EditorGUILayout.LabelField(content, new GUIStyle(style) { alignment = TextAnchor.UpperRight }, options);

                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position + new Vector2(rect.width, 0);
            }
            NodeEditorGUILayout.PortField(position, port);
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

        public static Vector2 GetPortPosition(XNode.NodePort port)
        {
            Vector2 position = Vector3.zero;
            if (port == null) return position;

            // If property is an input, display a regular property field and put a port handle on the left side
            if (port.direction == XNode.NodePort.IO.Input)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position - new Vector2(15, -65);
            }
            // If property is an output, display a text label and put a port handle on the right side
            else if (port.direction == XNode.NodePort.IO.Output)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position + new Vector2(rect.width, 0);
            }

            return position;
        }

        // Draws Data Toggle and animates if data coming in 
        public void DataInToggle(Boolean dataIn, Rect m_InnerBodyRect, Rect m_BodyRect)
        {
            // Load node skin
            if (m_NodeSkin == null)
                m_NodeSkin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

            GUILayout.Space(60);
            GUILayout.BeginArea(m_BodyRect);
            GUILayout.Space(30);
            if (dataIn)
            {
                DrawPositionValueTogglesAndLabels(m_NodeSkin.GetStyle("Green Toggle"));
            }
            else
            {
                DrawPositionValueTogglesAndLabels(m_NodeSkin.GetStyle("Red Toggle"));
            }
            GUILayout.EndArea();
        }

        protected virtual void DrawPositionValueTogglesAndLabels(GUIStyle style)
        {
            Debug.Log("need to implement in node editor file");
        }

        protected void DrawValues(List<IMLBaseDataType> values, string name)
        {
            if (values != null)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    if (m_NodeSkin == null)
                        m_NodeSkin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");
                    GUIStyle style = m_NodeSkin.GetStyle("TE Text");
                    var x = 30;
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(x), GUILayout.MaxHeight(x) };
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(bodySpace);
                    GUILayout.TextArea(name + " " + i, style, options);
                    GUILayout.EndHorizontal();
                    switch (values[i].DataType)
                    {
                        case IMLSpecifications.DataTypes.Float:
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(bodySpace);
                            GUILayout.TextArea("float: " + System.Math.Round(values[i].Values[0], 3).ToString(), style, options);
                            GUILayout.EndHorizontal();
                            break;
                        case IMLSpecifications.DataTypes.Integer:
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(bodySpace);
                            GUILayout.TextArea("int: " + System.Math.Round(values[i].Values[0], 3).ToString(), style, options);
                            GUILayout.EndHorizontal();
                            break;
                        case IMLSpecifications.DataTypes.Vector2:
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(bodySpace);
                            GUILayout.TextArea("x: " + System.Math.Round(values[i].Values[0], 3).ToString(), style, options);
                            GUILayout.TextArea("y: " + System.Math.Round(values[i].Values[1], 3).ToString(), style, options);
                            GUILayout.EndHorizontal();
                            break;
                        case IMLSpecifications.DataTypes.Vector3:
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(bodySpace);
                            GUILayout.TextArea("x: " + System.Math.Round(values[i].Values[0], 3).ToString(), style, options);
                            GUILayout.TextArea("y: " + System.Math.Round(values[i].Values[1], 3).ToString(), style, options);
                            GUILayout.TextArea("z: " + System.Math.Round(values[i].Values[2], 3).ToString(), style, options);
                            GUILayout.EndHorizontal();
                            break;
                        case IMLSpecifications.DataTypes.Vector4:
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(bodySpace);
                            GUILayout.TextArea("x: " + System.Math.Round(values[i].Values[0], 3).ToString(), style, options);
                            GUILayout.TextArea("y: " + System.Math.Round(values[i].Values[1], 3).ToString(), style, options);
                            GUILayout.TextArea("z: " + System.Math.Round(values[i].Values[2], 3).ToString(), style, options);
                            GUILayout.TextArea("w: " + System.Math.Round(values[i].Values[2], 3).ToString(), style, options);
                            GUILayout.EndHorizontal();
                            break;
                        default:
                            break;
                    }
                }
            }
            EditorGUILayout.Space();

        }

        protected void HelpButton(string description)
        {

        }

        // <summary>
        /// Draws help button and tells whether mouse is over the tooltip
        /// </summary>
        public virtual void ShowHelpButton(Rect m_HelpRect)
        {
            if (m_NodeSkin == null)
                m_NodeSkin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

            m_HelpRect.x = m_HelpRect.x + 20;
            m_HelpRect.y = m_HelpRect.y + 10;
            m_HelpRect.width = m_HelpRect.width - 40;

            GUILayout.BeginArea(m_HelpRect);
            GUILayout.BeginHorizontal();
            GUILayout.Label("");

            if (GUILayout.Button(new GUIContent("Help"), m_NodeSkin.GetStyle("Help Button")))
            {
                if (showHelp)
                {
                    showHelp = false;
                }
                else
                {
                    showHelp = true;
                }

            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
        // <summary>
        /// Takes in rect and a string. Rect is the rect which the hovered GUI element is in. String is the tip for this element. Draws a tooltip below the element.
        /// </summary>
        public void ShowHelptip(Rect hoveredRect, string tip)
        {
            GUIStyle style = m_NodeSkin.GetStyle("TooltipHelp");
            // calculates the height of the tooltip 
            var x = style.CalcHeight(new GUIContent(tip), hoveredRect.width);
            m_ToolRect.x = hoveredRect.x;
            m_ToolRect.y = hoveredRect.y + hoveredRect.height;
            m_ToolRect.width = hoveredRect.width;
            m_ToolRect.height = 200;

            GUILayout.BeginArea(m_ToolRect);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(GetWidth() - 10), GUILayout.Height(GetWidth() - 50));
            GUILayout.BeginHorizontal();

            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(hoveredRect.width), GUILayout.MaxHeight(x) };
            GUILayout.TextArea(tip, style, options);
            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();


        }

        public void ShowTooltip(Rect hoveredRect, string tip)
        {
            GUIStyle style = m_NodeSkin.GetStyle("Tooltip");
            // calculates the height of the tooltip 
            var x = style.CalcHeight(new GUIContent(tip), hoveredRect.width);

            m_ToolRect.x = hoveredRect.x;
            m_ToolRect.y = hoveredRect.y + hoveredRect.height;
            m_ToolRect.width = hoveredRect.width;
            m_ToolRect.height = x;

            GUILayout.BeginArea(m_ToolRect);
            GUILayout.BeginHorizontal();

            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(hoveredRect.width), GUILayout.MaxHeight(x) };
            GUILayout.TextArea(tip, style, options);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();




        }
        // <summary>
        /// Takes in string goes through ports in that node to check if the mouse is over them. If the mouse is over one then it makes showport true and sets tooltip 
        /// string to the string of json for that port tip 
        /// </summary>
        public void PortTooltip(String[] portTips = null)
        {
            
            if (nodeTips != null)
            {
                portTips = nodeTips.PortTooltip;
                List<NodePort> ports = target.Ports.ToList();


                if (ports.Contains(window.hoveredPort))
                {
                    showporthelper = true;
                    for (int i = 0; i < ports.Count; i++)
                    {
                        if (window.hoveredPort == ports[i])
                        {
                            TooltipText = portTips[i];
                        }

                    }
                }
                else
                {
                    if (Event.current.type == EventType.Layout)
                    {
                        showPort = false;
                    }

                }

                if (showporthelper && Event.current.type == EventType.Layout)
                {
                    showPort = true;
                    showporthelper = false;
                }
            }
            
        }
        // <summary>
        /// Takes in rect and returns true if mouse is currently in that rect
        /// </summary>
        /// <returns>boolean </returns>
        public bool IsThisRectHovered(Rect rect)
        {
            bool test = false;

            if (rect.Contains(Event.current.mousePosition))
            {
                test = true;
            }
            else if (Event.current.type == EventType.MouseMove && !rect.Contains(Event.current.mousePosition))
            {
                test = false;

            }

            return test;
        }
        public void DrawWarningLayout(Rect help)
        {
            m_WarningRect.x = 5;
            m_WarningRect.y = help.y + help.height;
            m_WarningRect.width = NodeWidth - 10;
            m_WarningRect.height = 130;

            // Draw body background purple rect below ports
            GUI.DrawTexture(m_WarningRect, NodeColor);
        }

        public void ShowWarning(string tip)
        {
            m_InnerWarningRect.x = m_WarningRect.x + 20;
            m_InnerWarningRect.y = m_WarningRect.y + 20;
            m_InnerWarningRect.width = m_WarningRect.width - 40;
            m_InnerWarningRect.height = m_WarningRect.height - 40;

            // Draw darker purple rect
            GUI.DrawTexture(m_InnerWarningRect, GetColorTextureFromHexString("#1C1D2E"));

            m_InnerInnerWarningRect.x = m_InnerWarningRect.x + 10;
            m_InnerInnerWarningRect.y = m_InnerWarningRect.y + 10;
            m_InnerInnerWarningRect.width = m_InnerWarningRect.width - 20;
            m_InnerInnerWarningRect.height = m_InnerWarningRect.height - 20;

            GUILayout.BeginArea(m_InnerInnerWarningRect);
            GUILayout.BeginHorizontal();
            GUILayout.Button("", m_NodeSkin.GetStyle("Warning"));
            GUILayout.Space(5);
            GUILayout.Label("Warning", m_NodeSkin.GetStyle("Warning Header"));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.Label(tip, m_NodeSkin.GetStyle("Warning Label"));

            GUILayout.EndArea();

        }
        /// <summary>
        /// Define rect values for port section and paint textures based on rects 
        /// </summary>
        protected virtual void DrawPortLayout()
        {
            // Add x units to height per extra port
            if (m_PortPairs == null)
                m_PortPairs = new List<IMLNodePortPair>();

            // Check whether we need to recalculate rects because there are more portPairs
            if (m_KnownNumPortPairs != m_PortPairs.Count)
            {
                m_KnownNumPortPairs = m_PortPairs.Count;
                m_RecalculateRects = true;
            }

            if (m_RecalculateRects)
            {
                int extraHeight = (m_PortPairs.Count * 10);
                // Draw body background purple rect below header
                m_PortRect.x = 5;
                m_PortRect.y = HeaderRect.height;
                m_PortRect.width = NodeWidth - 10;
                m_PortRect.height = 50 + extraHeight;
            }
            
            //GUI.DrawTexture(m_PortRect, NodeColor);

            // Calculate rect for line below ports
            Rect lineRect = new Rect(m_PortRect.x, HeaderRect.height + m_PortRect.height - WeightOfSectionLine, m_PortRect.width, WeightOfSectionLine);
            Texture2D lineTex = GetColorTextureFromHexString("#888EF7");

            // Draw line below ports
            GUI.DrawTexture(lineRect, lineTex);

        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        protected virtual void InitBodyLayout()
        {
            if(m_RecalculateRects)
            {
                m_BodyRect.x = 5;
                m_BodyRect.y = HeaderRect.height + m_PortRect.height;
                m_BodyRect.width = NodeWidth - 10;
                if (m_BodyRect.height == 0)
                    m_BodyRect.height = 90;
            }
            // Draw body background purple rect below header
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        protected virtual void DrawHelpButtonLayout(float y)
        {
            m_HelpRect.x = 5;
            m_HelpRect.y = y;
            m_HelpRect.width = NodeWidth - 10;
            m_HelpRect.height = 40;
            //Draw separator line
            GUI.DrawTexture(new Rect(m_HelpRect.x, HeaderRect.height + m_PortRect.height + m_BodyRect.height - WeightOfSeparatorLine, m_HelpRect.width, WeightOfSeparatorLine*2), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        protected void ShowNodePorts(Dictionary<string, string> inputsNameOverride = null, Dictionary<string, string> outputsNameOverride = null, bool showOutputType = false)
        {
            if(NodeSubtitle != null)
            {
                GUILayout.Space(15);
            } else
            {
                GUILayout.Space(5);
            }
            

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            // Init variables
            GUIContent inputPortLabel;
            GUIContent outputPortLabel;
            // Make sure port pair list is init
            if (m_PortPairs == null)
                m_PortPairs = new List<IMLNodePortPair>();

            bool updatePortPairs = false;
            // DIRTY CODE
            // If the node is an mls node, check if the output ports have been updated
            if (target is IMLConfiguration)
            {
                var mlsNode = (target as IMLConfiguration);
                updatePortPairs = mlsNode.OutputPortsChanged;
                // Set flag to false in mls node to not redraw every frame
                mlsNode.OutputPortsChanged = false;
            }
            // Generic check if the number ports changes to reduce the times we reserve memory
            if (m_NumInputs != target.Inputs.Count() || m_NumOutputs != target.Outputs.Count()) updatePortPairs = true;

            // Get number of ports to avoid reserving memory twice
            if (updatePortPairs)
            {
                // Update known number of ports
                m_NumInputs = target.Inputs.Count();
                m_NumOutputs = target.Outputs.Count();
                // Get inputs and outputs ports
                IEnumerator<NodePort> inputs = target.Inputs.GetEnumerator();
                IEnumerator<NodePort> outputs = target.Outputs.GetEnumerator();
                // Add them to the list
                AddPairsToList(inputs, outputs, ref m_PortPairs);
            }



            // Go through port pairs to draw them together
            foreach (var pair in m_PortPairs)
            {

                // Will draw them in a horizontal pair
                GUILayout.BeginHorizontal();

                // Draw input (if any)
                if (pair.input != null)
                {
                    if (NodeEditorGUILayout.IsDynamicPortListPort(pair.input)) continue;
                    inputPortLabel = new GUIContent(pair.input.fieldName);
                    // Check if an override of the label is needed
                    if (inputsNameOverride != null)
                    {
                        if (inputsNameOverride.ContainsKey(pair.input.fieldName))
                        {
                            string newLabel = inputPortLabel.text;
                            inputsNameOverride.TryGetValue(pair.input.fieldName, out newLabel);
                            inputPortLabel.text = newLabel;
                        }
                    }
                    // Draw port
                    IMLNodeEditor.PortField(inputPortLabel, m_IMLNode.GetInputPort(pair.input.fieldName), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));
                }
                // Draw output (if any)
                if (pair.output != null)
                {
                    if (NodeEditorGUILayout.IsDynamicPortListPort(pair.output)) continue;
                    outputPortLabel = new GUIContent(pair.output.fieldName);
                    // Check if an override of the label is needed
                    if (outputsNameOverride != null)
                    {
                        if (outputsNameOverride.ContainsKey(pair.output.fieldName))
                        {
                            string newLabel = outputPortLabel.text;
                            outputsNameOverride.TryGetValue(pair.output.fieldName, out newLabel);
                            outputPortLabel.text = newLabel;
                        }
                    }
                    // Check if we require to show the data type of the output
                    if (showOutputType == true)
                    {
                        string type = pair.output.ValueType.ToString();
                        // Remove namespace from string (if any)
                        if (type.Contains("."))
                        {
                            // Remove everything until "."                           
                            type = type.Remove(0, type.IndexOf(".") + 1);
                        }
                        // Add type to label text
                        outputPortLabel.text = string.Concat(outputPortLabel.text, " (", type, ")");
                    }
                    // Draw port
                    IMLNodeEditor.PortField(outputPortLabel, m_IMLNode.GetOutputPort(pair.output.fieldName), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

                }

                GUILayout.EndHorizontal();

            }

            serializedObject.ApplyModifiedProperties();


        }

        /// <summary>
        /// Shows all the property fields from the node
        /// </summary>
        protected virtual void ShowBodyFields()
        {
            string[] excludes = { "m_Script", "graph", "position", "ports" };

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            EditorGUIUtility.labelWidth = LabelWidth;
            GUILayout.Space(m_PortRect.height * 0.5f);
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (excludes.Contains(iterator.name)) continue;

                XNode.Node node = iterator.serializedObject.targetObject as XNode.Node;
                XNode.NodePort port = node.GetPort(iterator.name);

                // Don't allow to draw ports (// TO DO, add ports to the list now?)
                if (port != null) continue;

                // Save original editorStyle 
                Color originalContentColor = GUI.contentColor;
                Color originalColor = EditorStyles.label.normal.textColor;
                Font originalFont = EditorStyles.label.font;
                int origianlFontSize = EditorStyles.label.fontSize;

                // Replace skin for entire editorStyle with custom
                EditorStyles.label.normal.textColor = Color.white;
                EditorStyles.label.font = m_NodeSkin.label.font;
                EditorStyles.label.fontSize = 13;

                // Draw property
                NodeEditorGUILayout.PropertyField(iterator, (NodePort)null, true);

                // Place original skin back to not mess other nodes
                EditorStyles.label.normal.textColor = originalColor;
                EditorStyles.label.font = originalFont;
                EditorStyles.label.fontSize = origianlFontSize;
            }

        }

        /// <summary>
        /// Add portPairs to a list passed in
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="outputs"></param>
        /// <param name="pairs"></param>
        protected void AddPairsToList(IEnumerator<NodePort> inputs, IEnumerator<NodePort> outputs, ref List<IMLNodePortPair> pairs)
        {
            IMLNodePortPair portPair = new IMLNodePortPair();
            portPair.Reset();

            // while there are inputs...
            while (inputs.MoveNext())
            {
                // Add input to pair
                portPair.input = inputs.Current;
                // If the input is null, skip to next iteration
                if (portPair.input == null) continue;
                // If the list doesn't have a pair with our input...
                if (!pairs.Any(x => (x != null && x.input != null) && x.input.fieldName.Equals(portPair.input.fieldName)))
                {
                    // If there is an output...
                    if (outputs.MoveNext())
                    {
                        // Add output to pair
                        portPair.output = outputs.Current;
                    }
                    // Add pair to list
                    pairs.Add(new IMLNodePortPair(portPair.input, portPair.output));
                }
                // Reset pair for next use
                portPair.Reset();
            }

            // Check if there any outputs left (in case we have more outputs than inputs)
            while (outputs.MoveNext())
            {
                // Add output to pair
                portPair.output = outputs.Current;
                // If the output is null, skip to next iteration
                if (portPair.output == null) continue;
                // If the list doesn't contain a pair with our output...
                if (!pairs.Any(x => (x != null && x.output != null) && x.output.fieldName.Equals(portPair.output.fieldName)))
                {
                    // Add pair to list (input will be null)
                    pairs.Add(new IMLNodePortPair(portPair.input, portPair.output));
                }
                // Reset pair for next use
                portPair.Reset();
            }


        }

        /// <summary>
        /// Returns the loaded IMLGUISkin
        /// </summary>
        /// <returns></returns>
        protected GUISkin LoadIMLGUISkin()
        {
            // Load node skin
            return Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

        }
        protected void ShowRunOnAwakeToggle(IMLConfiguration configNode)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(bodySpace);
            configNode.RunOnAwake = EditorGUILayout.Toggle(configNode.RunOnAwake, m_NodeSkin.GetStyle("Local Space Toggle"));
            EditorGUILayout.LabelField("Run Model On Play", m_NodeSkin.GetStyle("Node Local Space Label"));
            GUILayout.EndHorizontal();
        }

        protected void ShowTrainingIcon(string MLS)
        {
            m_IconCenter.x = m_BodyRect.x;
            m_IconCenter.y = m_BodyRect.y;
            m_IconCenter.width = m_BodyRect.width;
            m_IconCenter.height = 150;

            GUILayout.BeginArea(m_IconCenter);
            GUILayout.Space(bodySpace);
            GUILayout.BeginHorizontal();
            if(trainingIcon == null)
               trainingIcon = EditorGUIUtility.Load("Icons/"+MLS+ ".png") as Texture;
            GUILayout.Box(trainingIcon, m_NodeSkin.GetStyle(MLS + " MLS Image"));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(MLS, m_NodeSkin.GetStyle(MLS +" Label"));
            GUILayout.EndHorizontal();
            
            GUILayout.EndArea();

        }

        /// <summary>
        /// Show the load, delete and record buttons
        /// </summary>
        protected void ShowButtons(IMLConfiguration node)
        {
            m_ButtonsRect.x = m_IconCenter.x + 20;
            m_ButtonsRect.y = m_IconCenter.y + m_IconCenter.height;
            m_ButtonsRect.width = m_BodyRect.width-40;
            m_ButtonsRect.height = 150;

            
            GUILayout.BeginArea(m_ButtonsRect);
            
            // if button contains mouse position
            TrainModelButton();
            GUILayout.Space(15);
            RunModelButton();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Space(NodeWidth/2 - 40);
            if (GUILayout.Button("", m_NodeSkin.GetStyle("Reset")))
            {
                node.ResetModel();
                numberOfExamplesTrained = 0;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("reset model", m_NodeSkin.GetStyle("Reset Pink Label"));
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            ShowRunOnAwakeToggle(node);
            GUILayout.EndArea();
        }

        protected virtual void TrainModelButton()
        {

        }

        protected virtual void RunModelButton()
        {

        }

        #endregion

    }

}

