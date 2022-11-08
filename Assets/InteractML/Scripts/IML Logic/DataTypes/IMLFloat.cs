using UnityEngine;

namespace InteractML
{
    /// <summary>
    /// Float data type to use in IML models
    /// </summary>
    [System.Serializable]
    public class IMLFloat : IMLBaseDataType
    {
        [SerializeField]
        private float[] m_Values = new float[1];

        public override float[] Values { get { return m_Values; } set { if(value != null) SetValues(value[0]);  } }

        private IMLSpecifications.DataTypes m_DataType = IMLSpecifications.DataTypes.Float;

        public override IMLSpecifications.DataTypes DataType { get { return m_DataType; } }

        public IMLFloat()
        {
            if (m_Values == null)
                m_Values = new float[1];

            m_DataType = IMLSpecifications.DataTypes.Float;

        }


        public IMLFloat(IMLBaseDataType newData)
        {
            if (m_Values == null)
                m_Values = new float[1];

            m_DataType = IMLSpecifications.DataTypes.Float;

            if (newData.Values != null && newData.Values.Length > 0)
                SetValues(newData.Values[0]);

        }

        public IMLFloat(float newData)
        {
            if (m_Values == null)
                m_Values = new float[1];

            m_DataType = IMLSpecifications.DataTypes.Float;

            SetValues(newData);

        }

        public override void SetValues(float[] newValues)
        {
            if (m_Values == null) m_Values = new float[1];
            if (newValues != null) m_Values[0] = newValues[0];
        }

        public void SetValues(float newValue)
        {
            m_Values[0] = newValue;
        }

        public float GetValue()
        {
            return m_Values[0];
        }

        public override IMLBaseDataType Add(IMLBaseDataType amount)
        {
            if (amount != null && amount.Values != null && amount.Values.Length > 0)
            {
                if (amount.DataType == DataType || amount.DataType == IMLSpecifications.DataTypes.Integer)
                {
                    SetValues(m_Values[0] + amount.Values[0]);
                }
                else
                {
                    Debug.LogError("Wrong type passed in data type operation");
                }
            }
            else
            {
                Debug.LogError("Null reference in data type operation");
            }
            return this;
        }

        public override IMLBaseDataType Substract(IMLBaseDataType amount)
        {
            if (amount != null && amount.Values != null && amount.Values.Length > 0)
            {
                if (amount.DataType == DataType || amount.DataType == IMLSpecifications.DataTypes.Integer)
                {
                    SetValues(m_Values[0] - amount.Values[0]);
                }
                else
                {
                    Debug.LogError("Wrong type passed in data type operation");
                }
            }
            else
            {
                Debug.LogError("Null reference in data type operation");
            }
            return this;
        }
    }

}