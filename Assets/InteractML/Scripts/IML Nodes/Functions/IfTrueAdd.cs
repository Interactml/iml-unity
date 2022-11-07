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

        public bool isExternallyUpdatable => !m_IsUpdated;

        public bool isUpdated { get => m_IsUpdated; set => m_IsUpdated = value; }
		private bool m_IsUpdated;

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
			Debug.Log("Update called!");
        }

        public void LateUpdate()
        {
            if (m_IsUpdated)
            {
				m_IsUpdated = false;
				Debug.Log("Late update called!");
            }
        }
    }
}
