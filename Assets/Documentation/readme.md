# Folder structure 

All code is in the assets folder under scripts 

## RapidLib 

#### RapidLib.cs 
Needs to be refactor. Rapid Lib bindings, and Rapid Lib component itself. 
Runs the machine learning logic 
Connects to the RapidLib c++ .dll 

There's a bool called AllowExternalData
	If it's using Internal Data, then the script with run all the ML and loops to keep that running. It will as well serialize the data in the JsonHelper. The features you extract are specified in the component. 

​	If it's using External Data, then the script exposes parameters you can inject values into but it will not run the logic explicitly unless told to from outside (aka the node system). This script now does not serials data another script called IMLDataSerialization (in ML Logic folder) The features are set up from outside and passed in via a vector. It also returns a vector that will need to be parsed to work out the number of features. 

With the dll integration, the c# functions act as a bridge to the c++ lib. All parameters you pass in should be primitives. No passing classes. Then the c# pointer language constructs are used here. (IE. IntPtr = Integer Pointer). To learn more search C# pointers. Extern = external function.   

#### JsonHelper.cs 
Used during GDC demo to serial training sets and models 

#### Editor 

In unity, every time you want to change the style of the component, it needs to be in an Editor folder and the code controls the appearance of the component in the inspector. 

This will overwrite Unity's default editor script which all components inherit from. 

To learn more search: Unity Editor Scripting 

---

** TO DO in the refractor*

One class for interfacing with DL (all passing to ML Logic) 
One class for ML logic 
One for the editor layout (would interface with ML Logic) 
One for the node layout (would interface with ML Logic)

---

## XNode 

#### Scripts 

Had functionality to pass data between notes 
Things start as a Node Graph 

Node Graph Script - allows you to create scripts 
We inherit from this to create our custom graph 

In every graph you can but nodes. The Node class is a representation of the node in the graph and you inherit from it. 

Node Editors adjust the layout of the node. You implement the NodeEditor in your own class to create your own node layout 
NodeEditorBase is the top level code and other NodeEditors implement it 

To change the appearance of the nodes go into xNode>Scripts>Editor > Resources > xnode_node.png
[For more see the documentation here](http://https://github.com/Siccity/xNode)

## IML Controllers 

#### IMLController - each graph is an IML controller 
This is a Scriptable Objects. A Scriptable Object is a script that becomes an asset you can include inside a component (aka another script)

## IML Nodes 

This folder holds the nodes that will populate the graphs. 
The Editor folder holds the visual appearance of each script here. The logic is in the script by the same name. These are the for the appearance of the node. They use similar naming conventions to unity's on editor system 

#### Feature Extractors 

These are all current implemented feature extractors 

#### Data Type Nodes

These are all the data types you can use in the node graph 

## IML Logic 

This is the folder that has the logic that controls how the nodes tie together 

#### IMLComponent 

It is the bridge between the node graph and the scene 

*Responsibilities*

Needs to be aware of one IMLGraph  (similar to the animator controller)
You can also set up how many and which game objects you want to reference from the scene 

Is aware of the outputs of the graphs 

**this is how you get the data out**

The graph does not have any loops. The IML Component fetches all the nodes in the graph, updates all of the data, and all the functions which are loops. Without this the graph does not run

#### IMLDataSerialization 

This script knows where to serialize and what to serialize (a process you do to data to transform it into something else)
Serializes: 
custom IML Datatypes into Json (for dll)
TrainingExamples

This converts the custom DataTypes into Json (this data gets used in rapid lib)

In the future, it would automate this process. Parse data from IML to Unity, Unity to IML, and IML to RapidLib 

1. Rapid Lib data is a super long spidery vector 
2. The second level of abstraction is the ML Logic Custom Data Types. These split up the data in smaller vectors that are wrapped into a separate class. An IMLVector3 is an array of 3 ints or floats. 
3. The highest level of abstractions are Unity Structs which are Vector3 

This class handles moving data between these three levels of abstraction 

#### IML Specifications

This script is a list of specifications of the system 
Lists: 
datatypes 
Possible inputs and outputs 
ML systems
States of ML Systems 

#### Attributes

**Attributes** are markers that can be placed above a class, property or function in a script to indicate special behaviour. 

#### SendToIMLController 

It's an attribute you can put on top of any variable in a script to send it into the graph. 
Unity has 20 or 30 you can use by default to show stuff in the inspector. You can also define your own. 

#### Datatypes 

These are the data types you can use in the node graph which get translated into more primitive types for the RapidLib .dll later on 

All the data types inherit from the IMLBase datatype 

IML Input 
Defines what is an input (needs to be an IML Custom Datatype) 

Outputs
Defines what is an output (needs to be an IML Custom Datatype)

IMLTrainingExample 
Holds a list of inputs and outputs for the IMLTraningExamplesNode 
Holds a list of inputs and outputs to be used inside the graph

Editor 
Editor script for the IML Component 

#### Serialization

IMLBaseDataTypeSpecifiedConcreteClassConverter
overwrites some of the default json code to understand the IML Datatypes 

IMLJsonDataType
Turns the data into json 
For example if you want to create a custom data type such as a color, you need to modify this script (IMLJsonTypeConverter) to convert the color to a json datatype. The library is in Assets > json.net



