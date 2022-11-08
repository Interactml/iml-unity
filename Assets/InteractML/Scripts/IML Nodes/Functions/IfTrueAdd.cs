using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.Functions
{
	/// <summary>
	/// Node that adds a value to a variable
	/// </summary>
	public class IfTrueAdd : IMLNode, IUpdatableIML
	{
		[Input]
		public bool True;
		[Input]
		public IMLBaseDataType Amount;
		[Input]
		public IMLBaseDataType In;
		[Output]
		public IMLBaseDataType Out;

        public bool isExternallyUpdatable => true;

        public bool isUpdated { get => m_IsUpdated; set => m_IsUpdated = value; }
        public bool isLateUpdated { get => m_IsLateUpdated; set => m_IsLateUpdated = value; }

        private bool m_IsUpdated;

		private bool m_IsLateUpdated;

        // Use this for initialization
        protected override void Init()
		{
			base.Init();

		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port)
		{
			return Out;
		}

        public void Update()
        {
            if (GetInputValue<bool>("True"))
            {
				//Out = In + Amount;
				m_IsUpdated = true;
				Debug.Log("ADD!");
            }
        }

        public void LateUpdate()
        {
			// do nothing
        }
    }
}
