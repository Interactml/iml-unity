using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML
{
    public class VRButtonHandler : InputHandler
    {

        public IMLControllerInputs triggerButton;
        public IMLSides controllerSide;
        public XRController leftController;
        public XRController rightController;

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
            triggerButton = (IMLControllerInputs)buttonNo;
        }

    }
}
