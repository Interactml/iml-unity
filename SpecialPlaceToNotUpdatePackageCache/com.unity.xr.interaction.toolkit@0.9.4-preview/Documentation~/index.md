# XR Interaction Toolkit

The XR Interaction Toolkit package is a high-level, component-based, interaction system. It provides a framework that makes 3D and UI interactions available from Unity input events. The core of this system is a set of base Interactor and Interactable components, and an Interaction Manager that ties these two types of components together. It also contains helper components that you can use to extend functionality for drawing visuals and hooking in your own interaction events.

XR Interaction Toolkit contains a set of components that support the following Interaction tasks:
- Cross-platform XR controller input
- Basic object hover, select and grab
- Haptic feedback through XR controllers
- Visual feedback (tint/line rendering) to indicate possible and active interactions
- Basic canvas UI interaction with XR controllers
- A VR camera rig for handling stationary and room-scale VR experiences

To use these AR interaction components, you must have the [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@2.0/manual/index.html) package in your Project, The AR functionality provided by the XR Interaction Toolkit includes:
- AR gesture system to map screen touches to gesture events
- AR interactable can place virtual objects in the real world
- AR gesture interactor and interactables to translate gestures such as place, select, translate, rotate, and scale into object manipulation
- AR annotations to inform users about AR objects placed in the real world

## Installing The XR Interaction Toolkit

To install this package, follow the instructions in the [Package Manager documentation](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html).

## Glossary

| Term | Meaning |
|-|-|
| Controller | A component that turns XR controller input such as a button press into interaction events like hover, or select. Also provides a way to show controller models and send haptic feedback to the controller. |
| Object | Anything that the user sees or interacts with in the virtual world. |
| Interactor | An object in a Scene that can select or move another object in that Scene. |
| Interactable | An object in a Scene that the user can interact with (for example, grab it, press it, or throw it). |
| Hover | The state where an Interactor is in a valid state to interact with an object. This differs between Ray and Direct interaction.|
| Select | The state where an Interactor is currently interacting with an object. |
| Interaction Manager | A manager component that handles interaction between a set of Interactors and Interactables in a scene. |
| Gesture | Sequences of movements that translate into an action that manipulates an interactable. |
| Annotation | A piece of content placed above (or next to) an AR object to give users information and context. |
| Haptic | Sensory or visual stimuli that is sent to the user to give feedback for interaction. |

## Setup

### Setup through GameObject menu

To set up a Scene for use with the XR Interaction Toolkit, use the commands in the **XR** menu **(Game Object > XR)** to create Interaction GameObjects. You can drop Interactors, Interactables, and the VR Camera Rig into the Scene from this menu.

![ui_helpers](images/ui_helpers.png)

### Detailed setup

#### Interaction Manager

Every Scene that uses the XR Interaction Toolkit needs at least one Interaction Manager to facilitate interaction between Interactors and Interactables. By default, Interactors and Interactables reference the first Interaction Manager they find in the Scene if you don’t specify one. You can also break up larger Scenes with multiple Managers or turn specific ones on and off to enable sets of interaction.

#### Controller/Interactor

![interactor_setup](images/interactor_setup.png)

An Interactor component controls how a GameObject interacts with other objects in the Scene. There are multiple types of Interactors. The example in the screenshot above uses a Ray Interactor, a component that uses [Unity ray casting](https://docs.unity3d.com/ScriptReference/Physics.Raycast.html) in order to find valid Interactable objects in the Scene.  

To receive input from a XR input device, the Interactor GameObject needs a Controller object. Add one of these to your Interactor Game Object and bind some or all of the following actions to an XR input device:

| Action | Required for |
| - | - |
| Position, Rotation, and Select | Basic interaction |
| Activation | Activating a selected object |
| Press | Interacting with UI objects |

The Controller and Interactor have limited support for haptic feedback. To enable haptic feedback, use the **Haptic Device Name** property **(Window > Input Debugger)** to make your Controller (or Controller Preset) specify the name of a device that supports haptic feedback.). The Interactor can then specify intensities and durations of haptic feedback to play back on select and hover.

The **Enable Interaction with UI GameObjects** checkbox controls whether this **XR Ray Interactor** can interact with Unity UI elements in the scene. 

#### Interactable

![interactable_setup](images/interactable_setup.png)

Interactable components define how the user can interact with objects in a Scene. In the screenshot above, the interactable GameObject supports grabbing, moving, dropping and throwing.

Interactables support a default Sphere collider to detect interaction, but other types of colliders can provide better hit detection.

**Example:** To set up an Interactable object that the user can grab, select it in your Scene and add these components:
- **Grab Interactable**
- A convex **Mesh Collider**

#### XR Interaction Toolkit Debugger window

![interaction_debugger](images/interaction_debugger.png)

The XR Interaction Toolkit Debugger window displays a top-down view of all the Input Devices, Interactables, and Interactors in a Scene. It also displays their relationship to each other and their parent Interaction Managers. To access this window, go to Window > Analysis.

## InteractionLayerMask

The InteractionLayerMask is a mechanism for filtering which interactors can act upon which interactables.  Interactors and interactables that share at least one layer in their InteractionLayerMask are allowed to interact.  If an interactor and interactable do not share at least one layer in their InteractionLayerMask then they are not allowed to interact.  Mask comparisons do not consider the layer on an interactor's or interactable's gameobject.  Comparisons only consider the InteractionLayerMasks on the interactor and interactable.

InteractionLayerMasks use physics layers, but this mask is distinct from a raycast mask.  A raycast mask specifies what a raycast can hit. The InteractionLayerMask specifies whether an interactor and interactable can interact through state changes such as select and hover.

InteractionLayerMasks can be set on both interactors and interactables, and default to interact with all layers.

## UI Interaction Setup

To interact with Unity's built-in UI elements, you need to perform extra steps, particularly if you’re dealing with 3D-tracked devices. The XR Interaction Toolkit package provides a number of new components that you can use to convert an XR controller to work seamlessly with the UI, as well as helper menu options that handle basic configuration settings.

### Using the Helpers

The XR Interaction package comes with a helper that perform basic setup. Use these helpers to create a new UI canvas. Access them from the **GameObject > XR** menu.

| Helper | Function |
| - | - |
| **UI Canvas** | Creates a new world-space canvas that you can then add standard UI elements to. If you haven’t configured an EventSystem yet, it also creates and configures a new EventSystem for XR. |

![ui_helpers](images/ui_helpers.png)

### Custom Setup

#### EventSystem

The EventSystem component acts as a central dispatch for UI events to process input, and update individual active canvases. Additionally, each EventSystem needs an InputModule to process input. Use the default configuration, pictured below, as a starting point. Only one InputModule can be active at one time.

__Note:__ If you already created a canvas or you created the EventSystem first, you might have a StandaloneInputModule component in your Project. Remove it, or it might block input processing.

![event_system_setup](images/ui_event_system_setup.png)

#### Canvas
All UI elements exist in the canvas. In the XR Interaction Toolkit, a user can only interact with canvases that have their **Render Mode** set to **World Space**. The XR Interaction Toolkit package contains a new component (pictured below), called the **Tracked Device Graphic Raycaster**. This component lets you use 3D tracked devices to highlight and select UI elements in that canvas.

![ui_canvas_setup](images/ui_canvas_setup.png)

## Architecture

### States

The Interaction system has three common states: **Hover**, **Select**, and **Activate**. These states can mean different things to different Interactables. **Hover** and **Select** are loosely related to the traditional GUI concepts of mouse-over and mouse-down. **Activate** is specific to XR.

| State | Function |
| - | - |
| **Hover** | If an Interactable is a valid target for the Interactor its state changes to **Hover**. Hovering on an object signifies an intention to interact with it, but doesn’t typically change the behavior of that object, though it might create a visual indicator for this change of state, like how a hovered button changes tint. |
| **Select** | Selection requires an action such as a button or trigger press from the user to enable the **Selection** state. When an Interactable is in the **Selection** state, Unity considers the selected Interactor to be interacting with it. For example, **Selection** can simulate picking up a grabbable object, holding a lever or preparing to push a door that has focus via hovering. |
| **Activate** | Activation is an extra action, typically mapped to a button or trigger that affects the currently selected object. This lets the user further interact with an object they’ve selected. The **Activate** action depends on the Interactable. For example, you can use **Activate** to fire a gun that an Interactor has currently selected. You can hook the Interactable Event Callbacks component to process **Activate** into an action without any additional code (by hooking an existing callback into the OnActivate via UnityEvents). |**

### Components

#### Interactors
Interactor components handle the actions of hovering and selecting objects in the world. Any object with an Interactable component is registered with an Interaction Manager and checked each frame for the list of possible objects it can hover or select.

#### Interactables
Interactables are objects in a Scene that an Interactor can hover, select, and activate. By default, all Interactables can be affected by all Interactors. You can change this behavior by grouping Interactors and Interactables into complementary Interaction Managers. The Interactable defines the behavior of **Hover, Select** and **Activate**. The same Interactor might be able to pick up and throw a ball, shoot a gun, or press a 3D button on a keypad.

#### Interaction Manager
The Interaction Manager acts as an intermediary between Interactors and Interactables in a Scene. You can have multiple Interaction Managers, each with their own valid set of Interactors and Interactables. On Awake, both Interactors and Interactables register with the first found Interaction Manager in the Scene, unless you already assigned them a specific Interaction Manager in the Inspector. Every Scene needs to have at least one Interaction Manager for the system to work.

#### Controllers
The Controller component provides a way to abstract input data and translate it into Interaction states, notably for selection and activation. Position, Rotation, Selection, and Activation events are bound via input actions to a specific device's controls, which Interactors then query.

You can also use the Controller Recorder component to record and play back Input data. You can use this to to test out interactions or run test Scenes without an XR device attached.

__Note:__ Controller Recorder currently only works in the Editor.
### Update loop
The update loop of the Interaction Manager queries Interactors and Interactables, and handles the hover and selection states. First, it asks Interactors for a valid list of targets (used for both hover and selection). It then checks both Interactors and Interactables to see if their existing hover and selection objects are still valid. After invalid previous states have been cleared (and exited via OnSelectExit/OnHoverExit), it queries both objects for valid selection and hover states, and the objects enter a new state via OnSelectEnter/OnHoverEnter.

All registered Interactables and Interactors are updated by the Interaction Manager explicitly to preserve update order. Interactables and Interactors do not use the normal monobehaviour Upadate call. 

## AR Interaction

**Note**: AR interaction components are only available while using Unity 2019.2 or later and in a Project that also includes the [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@2.0/manual/index.html) package. You can install the AR Foundation package via the [Unity Package Manager](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html).

### AR gestures

AR interaction is mostly driven by an AR Gesture Interactor component that translates touch events into gestures such as tap, drag, and pinch. These gestures get fed down to gesture Interactables that turn these into interactions.

The XR Interaction Tookit package comes with a number of pre-defined gestures and gesture interactables, but you can always extend this package by defining your own gestures.

| Gesture | Triggered by input | Maps to interactable |
|-|-|-|
| Tap | User touches the screen | AR Placement Interactable, AR Selection Interactable |
| Drag | User drags finger across screen | AR Translation Interactable |
| Pinch | User moves two fingers together or apart in a straight line | AR Scale Interactable |
| Twist | User rotates two fingers around a center point | AR Rotation Interactable |
| Two Finger Drag | User drags with two fingers | Nothing currently |

The AR Gesture Interactor component translates screen touches to gestures. Typically, you should place this component on the Camera or another object that has its position driven by the Tracked Pose Driver component. Unity feeds gestures down to Interactables, which then to respond to the gesture event.

### Placement of objects with the AR Placement Interactable

The AR Placement Interactable component facilitates placing objects in the Scene. Users specify a placement prefab that Unity later places on an AR plane when a tap occurs. Unity also generates a ray cast against the plane at the same time. The Prefab can contain additional AR interactables to facilitate further gesture interaction.

### AR annotations

Use the AR Annotation Interactable to place annotations alongside virtual objects in an AR Scene. These annotations are Prefabs that the application can show or hide when they satisfy a set of constraints. Each annotation has a minimum and maximum range from the Camera at which it displays, as well as a maximum Field of View (FOV) center offset from the Camera to hide or minimize annotations that are not centered in the user's view.

## Extending The XR Interaction Toolkit

![xrft_interaction_update](images/xrft_interaction_update.png)

You can extend the XR Interaction Toolkit system through either [inheritance](https://unity3d.com/learn/tutorials/topics/scripting/inheritance) or composition. Both Interactors and Interactables derive from an abstract base class that you can derive from and use to hook into the Interaction Manager to provide your own functionality. Additionally, you can use helpers, in particular event callbacks, to add functionality to the existing components.
![xrff_class_hierarchy](images/xrff_class_hierarchy.png)

### Interactor and Interactable Event Callbacks

Interactors and Interactables both have various event callbacks that can be used to drive reactions to interaction state changes.  You can use events these to define your own behavior to hover, selection and activation state changes with no additional coding.

![callback_examples](images/interactable_events_callback_example.png)


### Extending Interactors

You can derive from the Base Interactor abstract base class to define your own way of interacting with the world. Unity currently provides Direct (collision) and Ray Interactors, but you can decide how you want the user to choose objects in the world and what they can do with them (though typically you do the latter by deriving from the Base Interactable class).  

In addition to standard Unity callbacks, you can override the following methods:

Method/Property | Description
--- | ---
GetValidTargets | Retrieves the list of interactables that this interactor can interact with this frame.
isHoverActive | Returns true if this Interactor is in a state where it could hover, false otherwise.
isSelectActive | Gets whether this interactor is in a state where it could select.
CanHover | Returns true if this Interactor is in a state where it could select, false otherwise.
CanSelect | Returns true if the Interactable is valid for selection this frame, false otherwise.
isSelectExclusive | Returns true if this Interactor requires exclusive selection of an Interactable, false otherwise.
overrideSelectedInteractableMovement | Gets whether this Interactor can override the movement type of the currently selected Interactable.
selectedInteractableMovementTypeOverride | Gets the movement type to use when overriding the selected Interactable's movement.
OnHoverEnter | The Interaction Manager calls this method when the Interactor first initiates hovering over an Interactable.
OnHoverExit | The Interaction Manager calls this method when the Interactor ends hovering over an interactable.
OnSelectEnter | The Interaction Manager calls this method when the Interactor first initiates selection of an Interactable.
OnSelectExit | The Interaction Manager calls this method when the Interactor ends selection of an Interactable.

### Extending Interactables

You can derive from the BaseInteractable abstract base class to define your own way of interacting with the world. Currently, Unity only provides the Grab Interactable, but you can either create your own Interactables from Base Interactable or derive from Grab Interactable if you want to adopt grabbing behavior.

In addition to standard Unity callbacks, you can override the following methods:

Method | Description
--- | ---
IsHoverableBy | Determines if this Interactable can be hovered by a given Interactor.
IsSelectableBy | Determines if this Interactable can be selected by a given Interactor.
OnHoverEnter | The Interaction Manager calls this method when the Interactor first initiates hovering over an Interactable.
OnHoverExit | The Interaction Manager calls this method when the Interactor ends hovering over an Interactable.
OnSelectEnter | The Interaction Manager calls this method when the Interactor first initiates selection of an Interactable.
OnSelectExit | The Interaction Manager calls this method when the Interactor ends selection of an Interactable.
OnActivate | The Interaction Manager calls this method when the Interactor sends an activation event down to an Interactable.
OnFirstHoverEnter | The Interaction Manager calls this method when the first Interactor initiates hovering over an Interactable.
OnLastHoverExit | The Interaction Manager calls this method when the last Interactor ends hovering over an interactable.

The **OnLastHoverExit** and **OnFirstHoverEnter** events can be used to control highlight states of objects as they will correctly fire when the first highlight is detected, and when no other interactor is highlighting an object.


------

## Locomotion

The XR Interaction Tookit package provides a set of locomotion primitives to act as an example of how to use the locomotion system primitives. These components are:
- An XR Rig that represents the player
- A Locomotion System
- A Teleportation System, with Teleportation destinations
- A Snap Turn System

The [locomotion documentation](locomotion.md) explains these sections in more detail.

# Technical details

### Requirements

This version of the XR Interaction Toolkit is compatible with the following versions of the Unity Editor:

* 2019.3 and later.

### Document revision history

|Date|Reason|
|---|---|
|May 1, 2018|Document created.|
|May 30, 2018|Document revised with commands and updated component names.|
|June 6, 2019|Document revised with updated UI module documentation.|
|July 10, 2019|Document revised with AR interaction documentation.|
|July 18, 2019|Document revised with Locomotion documentation.|
|Jun 31, 2019|Finalized Locomotion documentation.|
|August 13, 2019|Document edits.|
|September 4, 2019|Document revised with updated images and component names.|
|October 3, 2019|Document update to reflect package naming for release|
|December 4, 2019|Document revised with documentation team feedback|
|December 12, 2019|Fixed image linking|
|January 10, 2020|removed private github link|
