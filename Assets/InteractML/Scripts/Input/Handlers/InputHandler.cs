using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InteractML
{
    public abstract class InputHandler : ScriptableObject
    {
        public int buttonNo { get { return m_buttonNo; } set { m_buttonNo = value; } }
        [SerializeField]
        private int m_buttonNo;
        public string buttonName;
        
        public IMLTriggerTypes triggerType;
        protected bool previousPress;

        public delegate bool StateChange();
        public abstract event StateChange ButtonDown;
        public abstract event StateChange ButtonUp;
        public abstract event StateChange ButtonHold;

        public abstract void HandleState();
        public abstract void SetButtonNo(int button);
        public abstract void SetTriggerType(IMLTriggerTypes triggerT);
    }
}