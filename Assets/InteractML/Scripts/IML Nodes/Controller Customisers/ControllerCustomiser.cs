using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace InteractML
{
    public abstract class CustomController : IMLNode
    {

        public bool inputEvent { get { return m_inputEvent; } }
        [Output, SerializeField]
        private bool m_inputEvent;
        public abstract void UpdateLogic();

    }
}

