using UnityEngine;

namespace InteractML
{
    public abstract class InputHandler
    {
        public bool buttonSet = false;
        public int buttonNo { get { return m_buttonNo; } set { m_buttonNo = value; } }
        [SerializeField]
        private int m_buttonNo;
        public string buttonName;
        
        public IMLTriggerTypes triggerType;
        protected bool previousPress = false;

        public abstract event IMLEventDispatcher.IMLEvent ButtonFire;

        public abstract void HandleState();
        public abstract void SetButtonNo(int button);
        public abstract void SetTriggerType(IMLTriggerTypes triggerT);

        public string nodeID;

    }
}