using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractML
{
    [System.Serializable]
    public class IMLBoolean : IMLBaseDataType
    {
        [SerializeField]
        private float[] m_Values = new float[1];

        public override float[] Values { get { return m_Values; } set { if (value != null) SetValues(value[0]); } }

        private IMLSpecifications.DataTypes m_DataType = IMLSpecifications.DataTypes.Boolean;

        public override IMLSpecifications.DataTypes DataType { get { return m_DataType; } }

        public IMLBoolean()
        {
            if (m_Values == null)
                m_Values = new float[1];

            m_DataType = IMLSpecifications.DataTypes.Boolean;

        }


        public IMLBoolean(IMLBaseDataType newData)
        {
            if (m_Values == null)
                m_Values = new float[1];

            m_DataType = IMLSpecifications.DataTypes.Float;

            if (newData.Values != null && newData.Values.Length > 0)
                SetValues(newData.Values[0]);

        }

        public override void SetValues(float[] newValues)
        {
            if (m_Values == null) m_Values = new float[1];
            if (newValues != null) SetValues(newValues[0]);
        }

        public void SetValues(float newValue)
        {
            if (ReusableMethods.Floats.NearlyEqual(newValue, 1f, 0.005f))
                m_Values[0] = 1f;
            else if (ReusableMethods.Floats.NearlyEqual(newValue, 0f, 0.005f))
                m_Values[0] = 0f;
        }

        public void SetValues(bool newValue)
        {
            if (newValue == true)
                m_Values[0] = 1f;
            else
                m_Values[0] = 0f;
        }

        public bool GetValue()
        {
            if (ReusableMethods.Floats.NearlyEqual(m_Values[0], 1f, 0.005f))
                return true;
            else 
                return false;
        }
    }
}

