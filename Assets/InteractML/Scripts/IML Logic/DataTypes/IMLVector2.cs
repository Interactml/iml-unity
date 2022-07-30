using UnityEngine;

namespace InteractML
{
    /// <summary>
    /// Vector2 data type to use in IML models
    /// </summary>
    [System.Serializable]
    public class IMLVector2 : IMLBaseDataType
    {
        private float[] m_Values = new float[2];
        private Vector2 m_ValuesToReturn;

        public override float[] Values { get { return m_Values; } set { if (value != null) SetValues(value); } }

        private IMLSpecifications.DataTypes m_DataType = IMLSpecifications.DataTypes.Vector2;

        public override IMLSpecifications.DataTypes DataType { get { return m_DataType; } }

        public IMLVector2()
        {
            if (m_Values == null)
                m_Values = new float[2];

            m_DataType = IMLSpecifications.DataTypes.Vector2;
        }

        public IMLVector2(IMLBaseDataType newData)
        {
            if (m_Values == null)
                m_Values = new float[2];

            m_DataType = IMLSpecifications.DataTypes.Vector2;

            if (newData.Values != null && newData.Values.Length > 0)
                SetValues(newData.Values);
        }

        public IMLVector2(Vector2 newData)
        {
            if (m_Values == null)
                m_Values = new float[2];

            m_DataType = IMLSpecifications.DataTypes.Vector2;

            if (newData != null)
                SetValues(newData);
        }

        public void SetValues(Vector2 newValues)
        {
            m_Values[0] = newValues.x;
            m_Values[1] = newValues.y;
        }

        public override void SetValues(float[] newValues)
        {
            if (newValues.Length == m_Values.Length)
            {
                m_Values[0] = newValues[0];
                m_Values[1] = newValues[1];
            }
            else
            {
                Debug.LogError("Trying to assing a different lenght feature to an IMLVector2");
            }
        }

        public Vector2 GetValues()
        {
            m_ValuesToReturn.x = m_Values[0];
            m_ValuesToReturn.y = m_Values[1];
            return m_ValuesToReturn;
        }


    }

}
