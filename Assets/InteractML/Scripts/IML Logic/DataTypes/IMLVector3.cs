using UnityEngine;
using System;

namespace InteractML
{
    /// <summary>
    /// Vector3 data type to use in IML models
    /// </summary>
    [Serializable]
    public class IMLVector3 : IMLBaseDataType
    {
        [SerializeField]
        private float[] m_Values = new float[3];
        private Vector3 m_ValuesToReturn;

        public override float[] Values { get { return m_Values; } set { if (value != null) SetValues(value); } }

        private IMLSpecifications.DataTypes m_DataType = IMLSpecifications.DataTypes.Vector3;

        public override IMLSpecifications.DataTypes DataType { get { return m_DataType; } }

        public IMLVector3()
        {
            if (m_Values == null)
                m_Values = new float[3];

            m_DataType = IMLSpecifications.DataTypes.Vector3;
        }

        public IMLVector3(IMLBaseDataType newData)
        {
            if (m_Values == null)
                m_Values = new float[3];

            m_DataType = IMLSpecifications.DataTypes.Vector3;

            if (newData.Values != null && newData.Values.Length > 0)
                SetValues(newData.Values);
        }

        public IMLVector3(Vector3 newData)
        {
            if (m_Values == null)
                m_Values = new float[3];

            m_DataType = IMLSpecifications.DataTypes.Vector3;

            if (newData != null)
                SetValues(newData);
        }

        public override void SetValues(float[] newValues)
        {
            if (newValues.Length == m_Values.Length)
            {
                m_Values[0] = newValues[0];
                m_Values[1] = newValues[1];
                m_Values[2] = newValues[2];
            }
            else
            {
                Debug.LogError("Trying to assing a different lenght feature to an IMLVector3");
            }
        }

        public void SetValues(Vector3 newValues)
        {
            m_Values[0] = newValues.x;
            m_Values[1] = newValues.y;
            m_Values[2] = newValues.z;
        }

        public Vector3 GetValues()
        {
            m_ValuesToReturn.x = m_Values[0];
            m_ValuesToReturn.y = m_Values[1];
            m_ValuesToReturn.z = m_Values[2];
            return m_ValuesToReturn;
        }

        public static implicit operator Vector3(IMLVector3 v)
        {
            throw new NotImplementedException();
        }
    }

}