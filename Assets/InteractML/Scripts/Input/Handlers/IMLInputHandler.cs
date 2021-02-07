using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

namespace InteractML
{
    public class IMLInputHandler : InputHandler
    {
        public InputDevice device;
        public override event StateChange ButtonDown;
        public override event StateChange ButtonUp;
        public override event StateChange ButtonHold;
     
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

        public override void SetButtonNo(int no)
        {
            //device.
        }

        public override void SetTriggerType(IMLTriggerTypes triggerT)
        {
            
        }
    }
}
