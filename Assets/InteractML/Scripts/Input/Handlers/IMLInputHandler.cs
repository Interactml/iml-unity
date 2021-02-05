using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

namespace InteractML
{
    public class IMLInputHandler : InputHandler
    {
        public InputDevice device;
        public delegate bool StateChange();
        public event StateChange ButtonDown;
        public event StateChange ButtonUp;
        public event StateChange ButtonHold;
     
        public override void HandleState()
        {

            //Debug.Log(controller.axisToPressThreshold);
            /*if (controller.inputDevice.IsPressed(button, out bool pressed, controller.axisToPressThreshold))
            {
                if (previousPress != pressed)
                {
                    previousPress = pressed;
                    if (pressed)
                    {
                        ButtonDown?.Invoke();
                        ButtonHold?.Invoke();
                    }
                    else
                    {
                        ButtonUp?.Invoke();
                        ButtonDown?.Invoke();
                    }
                }
            }*/
        }

        public override void SetButton()
        {
            //device.
        }

    }
}
