using UnityEngine;
using System;

namespace InteractML
{
    /// <summary>
    /// Vector4 data type to use in IML models
    /// </summary>
    [Serializable]
    public class IMLVector4 : IMLBaseDataType
    {
        [SerializeField]
        private float[] m_Values = new float[4];
        private Vector4 m_ValuesToReturn;
        private Quaternion m_ValuesToReturnQuaternion;

        public override float[] Values { get { return m_Values; } set { if (value != null) SetValues(value); } }

        private IMLSpecifications.DataTypes m_DataType = IMLSpecifications.DataTypes.Vector4;

        public override IMLSpecifications.DataTypes DataType { get { return m_DataType; } }

        public IMLVector4()
        {
            if (m_Values == null)
                m_Values = new float[4];

            m_DataType = IMLSpecifications.DataTypes.Vector4;
        }

        public IMLVector4(IMLBaseDataType newData)
        {
            if (m_Values == null)
                m_Values = new float[4];

            m_DataType = IMLSpecifications.DataTypes.Vector4;

            if (newData.Values != null && newData.Values.Length > 0)
                SetValues(newData.Values);
        }

        public void SetValues(float[] newValues)
        {
            if (newValues.Length == m_Values.Length)
            {
                m_Values[0] = newValues[0];
                m_Values[1] = newValues[1];
                m_Values[2] = newValues[2];
                m_Values[3] = newValues[3];
            }
            else
            {
                Debug.LogError("Trying to assing a different lenght feature to an IMLVector4");
            }
        }

        public void SetValues(Vector4 newValues)
        {
            m_Values[0] = newValues.x;
            m_Values[1] = newValues.y;
            m_Values[2] = newValues.z;
            m_Values[3] = newValues.w;
        }

        public void SetValues(Quaternion newValues)
        {
            m_Values[0] = newValues.x;
            m_Values[1] = newValues.y;
            m_Values[2] = newValues.z;
            m_Values[3] = newValues.w;
        }

        public Vector4 GetValues()
        {
            m_ValuesToReturn.x = m_Values[0];
            m_ValuesToReturn.y = m_Values[1];
            m_ValuesToReturn.z = m_Values[2];
            m_ValuesToReturn.w = m_Values[3];
            return m_ValuesToReturn;
        }

        public Quaternion GetValuesQuaternion()
        {
            m_ValuesToReturnQuaternion.x = m_Values[0];
            m_ValuesToReturnQuaternion.y = m_Values[1];
            m_ValuesToReturnQuaternion.z = m_Values[2];
            m_ValuesToReturnQuaternion.w = m_Values[3];
            return m_ValuesToReturnQuaternion;
        }


    }

}
