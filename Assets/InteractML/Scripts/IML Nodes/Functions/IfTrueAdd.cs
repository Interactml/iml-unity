﻿using System.Collections;
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
		[Input(typeConstraint = TypeConstraint.Strict, connectionType = ConnectionType.Override)]
		public bool True;
		[Input(connectionType = ConnectionType.Override)]
		public IMLBaseDataType Amount;
		[Input(connectionType = ConnectionType.Override)]
		public IMLBaseDataType In;
		[Output]
		public float[] Out;
		private IMLBaseDataType m_OutDataType;

        public bool isExternallyUpdatable => true;

        public bool isUpdated { get => m_IsUpdated; set => m_IsUpdated = value; }
        public bool isLateUpdated { get => m_IsLateUpdated; set => m_IsLateUpdated = value; }

        private bool m_IsUpdated;

		private bool m_IsLateUpdated;

        #region IUpdatable Messages

        // Use this for initialization
        protected override void Init()
		{
			base.Init();
		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port)
		{
			In = PullData("In");
			if (In != null)
			{
				if (m_OutDataType == null)
					m_OutDataType = In;

				Out = m_OutDataType.Values;
			}
			else
				Out = default(float[]);
            return Out;
		}

        public void Update()
        {
			In = PullData("In");
			Amount = PullData("Amount");
			if (In != null && Amount != null && GetInputValue<bool>("True"))
            {

				m_OutDataType = In.Add(Amount);
				m_IsUpdated = true;
				Debug.Log("ADD!");
			}
        }

        public void LateUpdate()
        {
			// do nothing
        }

        #endregion

        #region Private Methods

		private IMLBaseDataType PullData(string port)
        {
			var portIn = GetInputPort(port);
			if (portIn.IsConnected)
			{
				var originType = portIn.Connection.ValueType;
				var inData = portIn.GetInputValue();
				if (originType != null)
				{
					return IMLBaseDataType.GetDataTypeInstance(originType, inData);
				}
			}
			return null;
		}

		#endregion
	}
}
