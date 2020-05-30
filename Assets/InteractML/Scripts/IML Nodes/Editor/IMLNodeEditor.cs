using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XNodeEditor.Internal;
using InteractML;
using XNode;
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

        #region Variables

        /// <summary>
        /// Texture2D for node color
        /// </summary>
        /// <returns></returns>
        protected Texture2D NodeColor { get; set; }

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
        /// Rect value for node header
        /// </summary>
        /// <returns></returns>
        protected Rect HeaderRect;

        /// <summary>
        /// Rect value for node header
        /// </summary>
        /// <returns></returns>
        protected Rect LineBelowHeader;

        public Vector2 positionPort;

        string description;

        public bool toolTipOn;

        /// <summary>
        /// Controls whether or not the reskinning of the node is automatically handled in the base IMLNodeEditor class (false to have default xNode skin, true for new IML skin)
        /// </summary>
        public bool UIReskinAuto = true;

        /// <summary>
        /// The skin to use on the node
        /// </summary>
        private GUISkin m_NodeSkin;

        /// <summary>
        /// Reference to this iml node
        /// </summary>
        private IMLNode m_IMLNode;

        /// <summary>
        /// The serialized representation of this IML Node
        /// </summary>
        private SerializedObject m_IMLNodeSerialized;

        /// <summary>
        /// The name for this node
        /// </summary>
        public string NodeName;

        /// <summary>
        /// Rects for node layout
        /// </summary>
        private Rect m_BodyRect;
        private Rect m_PortRect;
        private Rect m_InnerBodyRect;
        private Rect m_HelpRect;

        /// <summary>
        /// Number of input ports
        /// </summary>
        private int m_NumInputs;
        /// <summary>
        /// Number of output ports
        /// </summary>
        private int m_NumOutputs;

        /// <summary>
        /// List of port pairs to draw
        /// </summary>
        private List<IMLNodePortPair> m_PortPairs;

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

                // Draw header background Rect
                GUI.DrawTexture(HeaderRect, NodeColor);

                // Draw line below header
                GUI.DrawTexture(LineBelowHeader, GetColorTextureFromHexString("#888EF7"));

                //Display Node name
                if (String.IsNullOrEmpty(NodeName))
                    NodeName = typeof(IMLNode).Name + "(Script)";
                GUILayout.BeginArea(HeaderRect);
                GUILayout.Space(10);
                GUILayout.Label(NodeName, m_NodeSkin.GetStyle("Header"), GUILayout.MinWidth(NodeWidth - 10));
                GUILayout.EndArea();

                GUILayout.Label("", GUILayout.MinHeight(60));

            }
            // If we want to keep xNode's default skin
            else
            {
                base.OnHeaderGUI();
            }
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
                ShowNodePorts();

                // Draw Body Section
                DrawBodyLayout();
                ShowBodyFields();

                // Draw help button
                DrawHelpButtonLayout();
                ShowHelpButton(m_HelpRect);

                serializedObject.ApplyModifiedProperties();
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
            HeaderRect.height = 60;

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

        public static void IMLNoodleDraw(Vector2 Out, Vector2 In)
        {
            Gradient grad = new Gradient();
            Color a = hexToColor("#ffffff");
            Color b = hexToColor("#000000");
            grad.SetKeys(
                new GradientColorKey[] { new GradientColorKey(a, 0f), new GradientColorKey(b, 1f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
                );
            NoodlePath noodlePath = NodeEditorPreferences.GetSettings().noodlePath;
            float noodleThickness = 10f;
            NoodleStroke noodleStroke = NodeEditorPreferences.GetSettings().noodleStroke;
            
            List<Vector2> gridPoints = new List<Vector2>();
            gridPoints.Add(Out);
            gridPoints.Add(In);
            //NodeEditorGUI.DrawNoodle(grad, noodlePath, noodleStroke, noodleThickness, gridPoints);
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
                position = rect.position - new Vector2(15, - 65);
            }
            // If property is an output, display a text label and put a port handle on the right side
            else if (port.direction == XNode.NodePort.IO.Output)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                position = rect.position + new Vector2(rect.width, 0);
            }

            return position;
        }

        public void DataInToggle(Boolean dataIn, Rect m_InnerBodyRect, Rect m_BodyRect)
        {
            // Load node skin
            if (m_NodeSkin == null)
                m_NodeSkin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

            m_InnerBodyRect.x = m_BodyRect.x + 20;
            m_InnerBodyRect.y = m_BodyRect.y + 20;
            m_InnerBodyRect.width = m_BodyRect.width - 20;
            m_InnerBodyRect.height = m_BodyRect.height - 20;

            GUILayout.BeginArea(m_InnerBodyRect);

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

        protected void HelpButton(string description)
        {
            
        }

        public void DrawHelpButtonLayout(Rect m_ButtonsRect, Rect m_PortRect, Rect m_IconsRect)
         {
             m_HelpRect.x = 5;
             m_ButtonsRect.height = m_ButtonsRect.height + 15;
             m_HelpRect.y = HeaderRect.height + m_PortRect.height + m_IconsRect.height + m_ButtonsRect.height;
             m_HelpRect.width = NodeWidth - 10;
             m_HelpRect.height = 40;

             // Draw body background purple rect below ports
             GUI.DrawTexture(m_HelpRect, NodeColor);
         }

        public void ShowHelpButton(Rect m_HelpRect)
        {
            // Load node skin
            if (m_NodeSkin == null)
                m_NodeSkin = Resources.Load<GUISkin>("GUIStyles/InteractMLGUISkin");

            m_HelpRect.x = m_HelpRect.x + 20;
            m_HelpRect.y = m_HelpRect.y + 10;
            m_HelpRect.width = m_HelpRect.width - 40;

            Vector3 mouse = Input.mousePosition;


            GUILayout.BeginArea(m_HelpRect);
            GUILayout.BeginHorizontal();
            GUILayout.Label("");
            GUILayout.Button(new GUIContent("Help"), m_NodeSkin.GetStyle("Help Button"));
            if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition) && Event.current.type == EventType.Repaint)
            {
                toolTipOn = true;
            }
            else if (Event.current.type == EventType.Repaint && !GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                toolTipOn = false;

            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        public void ShowTooltip(Rect m_ToolRect, Rect m_HelpRect, string tip)
        {
            Vector3 mouse = Input.mousePosition;
            m_ToolRect.x = mouse.x + 10;
            m_ToolRect.y = mouse.y + 10;
            m_ToolRect.width = m_HelpRect.width - 40;
            m_ToolRect.height = m_HelpRect.height;
            


            GUILayout.BeginArea(m_ToolRect);
            GUILayout.BeginHorizontal();
            GUILayout.Label(tip);

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }

        /// <summary>
        /// Define rect values for port section and paint textures based on rects 
        /// </summary>
        private void DrawPortLayout()
        {
            // Add x units to height per extra port
            int numPorts = m_IMLNode.Ports.Count();
            int extraHeight = numPorts * 3; 
            
            // Draw body background purple rect below header
            m_PortRect.x = 5;
            m_PortRect.y = HeaderRect.height;
            m_PortRect.width = NodeWidth - 10;
            m_PortRect.height = 60 + extraHeight;
            GUI.DrawTexture(m_PortRect, NodeColor);

            // Draw line below ports
            GUI.DrawTexture(new Rect(m_PortRect.x, HeaderRect.height + m_PortRect.height - WeightOfSectionLine, m_PortRect.width, WeightOfSectionLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawBodyLayout()
        {
            m_BodyRect.x = 5;
            m_BodyRect.y = HeaderRect.height + m_PortRect.height;
            m_BodyRect.width = NodeWidth - 10;
            m_BodyRect.height = 150;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_BodyRect, NodeColor);
        }

        /// <summary>
        /// Define rect values for node body and paint textures based on rects 
        /// </summary>
        private void DrawHelpButtonLayout()
        {
            m_HelpRect.x = 5;
            m_HelpRect.y = HeaderRect.height + m_PortRect.height + m_BodyRect.height;
            m_HelpRect.width = NodeWidth - 10;
            m_HelpRect.height = 40;

            // Draw body background purple rect below header
            GUI.DrawTexture(m_HelpRect, NodeColor);

            //Draw separator line
            GUI.DrawTexture(new Rect(m_HelpRect.x, HeaderRect.height + m_PortRect.height + m_BodyRect.height - WeightOfSeparatorLine, m_HelpRect.width, WeightOfSeparatorLine), GetColorTextureFromHexString("#888EF7"));
        }

        /// <summary>
        /// Show the input/output port fields 
        /// </summary>
        private void ShowNodePorts()
        {
            GUILayout.Space(5);

            // Iterate through dynamic ports and draw them in the order in which they are serialized
            // Init variables
            GUIContent inputPortLabel;
            GUIContent outputPortLabel;
            IMLNodePortPair portPair = new IMLNodePortPair();
            // Make sure port pair list is init
            if (m_PortPairs == null)
                m_PortPairs = new List<IMLNodePortPair>();
            // Get number of ports to avoid reserving memory twice
            if (m_NumInputs != target.Inputs.Count() || m_NumOutputs != target.Outputs.Count())
            {
                // Update known number of ports
                m_NumInputs = target.Inputs.Count();
                m_NumOutputs = target.Outputs.Count();
                // Get inputs and outputs ports
                IEnumerator<NodePort> inputs = target.Inputs.GetEnumerator();
                IEnumerator<NodePort> outputs = target.Outputs.GetEnumerator();

                // while there are inputs...
                while (inputs.MoveNext())
                {
                    // Add input to pair
                    portPair.input = inputs.Current;
                    // If there is an output...
                    if (outputs.MoveNext())
                    {
                        // Add output to pair
                        portPair.output = outputs.Current;
                    }
                    // Add pair to list
                    m_PortPairs.Add(new IMLNodePortPair(portPair.input, portPair.output));
                    // Reset pair for next use
                    portPair.Reset();
                }

                // Check if there any outputs left (in case we have more outputs than inputs)
                while (outputs.MoveNext())
                {
                    // Add output to pair
                    portPair.output = outputs.Current;
                    // Add pair to list (input will be null)
                    m_PortPairs.Add(new IMLNodePortPair(portPair.input, portPair.output));
                    // Reset pair for next use
                    portPair.Reset();
                }
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
                    IMLNodeEditor.PortField(inputPortLabel, m_IMLNode.GetInputPort(pair.input.fieldName), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

                }
                // Draw output (if any)
                if (pair.output != null)
                {
                    if (NodeEditorGUILayout.IsDynamicPortListPort(pair.output)) continue;
                    outputPortLabel = new GUIContent(pair.output.fieldName);
                    IMLNodeEditor.PortField(outputPortLabel, m_IMLNode.GetOutputPort(pair.output.fieldName), m_NodeSkin.GetStyle("Port Label"), GUILayout.MinWidth(0));

                }

                GUILayout.EndHorizontal();

            }

            serializedObject.ApplyModifiedProperties();

        }

        /// <summary>
        /// Shows all the property fields from the node
        /// </summary>
        private void ShowBodyFields()
        {
            string[] excludes = { "m_Script", "graph", "position", "ports" };

            // Iterate through serialized properties and draw them like the Inspector (But with ports)
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            EditorGUIUtility.labelWidth = LabelWidth;
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
                NodeEditorGUILayout.PropertyField(iterator, (NodePort) null, true );

                // Place original skin back to not mess other nodes
                EditorStyles.label.normal.textColor = originalColor;
                EditorStyles.label.font = originalFont;
                EditorStyles.label.fontSize = origianlFontSize;
            }

        }


        #endregion

    }

}

