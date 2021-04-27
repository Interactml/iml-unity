using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractML
{
    public enum IMLInputDevices
    {
        None,
        Keyboard,
        /*Mouse, */
        VRControllers,
        ViveControllers
        /*VRHands*/
    }

    public enum IMLSides
    {
        Left,
        Right,
        Both
    }
    public enum IMLControllerInputs
    {
        Trigger,
        Grip,
        Primary,
        Secondary
    }
    public enum IMLHandInputs
    {
        
    }
    public enum IMLTriggerTypes
    {
        Up,
        Down,
        Hold
        
    }
}

