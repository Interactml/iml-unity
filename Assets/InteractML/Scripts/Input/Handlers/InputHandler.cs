using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML
{
    public abstract class InputHandler 
    {
        public int buttonNo;
        public IMLTriggerTypes triggerType;
        private bool previousPress;

        public abstract void HandleState();
        public abstract void SetButton();
    }
}