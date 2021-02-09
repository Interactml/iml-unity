using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

namespace InteractML.ControllerCustomisers
{
    public static class InputHelperMethods 
    {
        public static string[] deviceEnumSetUp(IMLInputDevices device)
        {
            string[] buttonOptions = new string[0];
            switch (device)
            {
                case IMLInputDevices.Keyboard:
                    buttonOptions = System.Enum.GetNames(typeof(KeyCode));
                    break;
                /*case IMLInputDevices.Mouse:
                    buttonOptions = System.Enum.GetNames(typeof(UnityEngine.InputSystem.LowLevel.MouseButton));
                    break;*/
                case IMLInputDevices.VRControllers:
                    buttonOptions = System.Enum.GetNames(typeof(IMLControllerInputs));
                    break;
                default:
                    buttonOptions = new string[] { "none" };
                    break;
            }
            return buttonOptions;
        }
    }
}
