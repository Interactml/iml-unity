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

        /// <summary>
        /// Go through list of input handlers and change the trigger type on that handler 
        /// </summary>
        /// <param name="buttonName">name of the button</param>
        /// <param name="triggerT"> type of trigger</param>
        /// <param name="handlers">list of handlers</param>
        public static void TriggerChange(string buttonName, IMLTriggerTypes triggerT, List<InputHandler> handlers)
        {
            foreach (InputHandler handler in handlers)
            {
                if (buttonName == handler.buttonName)
                {
                    handler.SetTriggerType(triggerT);
                }
            }
        }
        /// <summary>
        /// Go through list of handlers and when it is the correct handler change the button
        /// </summary>
        /// <param name="actionName">name of action to trigger</param>
        /// <param name="button">button number </param>
        public static void OnButtonChange(string actionName, int button, List<InputHandler> handlers)
        {
            foreach (InputHandler handler in handlers)
            {
                if (actionName == handler.buttonName)
                {
                    handler.SetButtonNo(button);
                }
            }
        }
    }

    
}
