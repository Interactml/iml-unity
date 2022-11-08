namespace InteractML
{
    /// <summary>
    /// Integer data type to use in IML models
    /// </summary>
    [System.Serializable]
    public class IMLInteger : IMLBaseDataType
    {
        private float[] m_Values = new float[1];

        public override float[] Values { get { return m_Values; } set { if (value != null) SetValues((int)value[0]); } }

        private IMLSpecifications.DataTypes m_DataType = IMLSpecifications.DataTypes.Integer;

        public override IMLSpecifications.DataTypes DataType { get { return m_DataType; } }

        public IMLInteger()
        {
            if (m_Values == null)
                m_Values = new float[1];

            m_DataType = IMLSpecifications.DataTypes.Integer;

        }


        public IMLInteger(IMLBaseDataType newData)
        {
            if (m_Values == null)
                m_Values = new float[1];

            m_DataType = IMLSpecifications.DataTypes.Integer;

            if (newData.Values != null && newData.Values.Length > 0)
                SetValues( (int) newData.Values[0]);

        }

        public IMLInteger(int newData)
        {
            if (m_Values == null)
                m_Values = new float[1];

            m_DataType = IMLSpecifications.DataTypes.Integer;

            SetValues(newData);

        }

        public override void SetValues(float[] newValues)
        {
            if (m_Values == null) m_Values = new float[1];
            if (newValues != null) m_Values[0] = newValues[0];
        }

        public void SetValues(int newValue)
        {
            m_Values[0] = newValue;
        }

        public int GetValue()
        {
            return (int)m_Values[0];
        }

        public override IMLBaseDataType Add(IMLBaseDataType amount)
        {
            if (amount != null && amount.Values != null && amount.Values.Length > 0)
            {
                if (amount.DataType == DataType || amount.DataType == IMLSpecifications.DataTypes.Float)
                {
                    SetValues((int)m_Values[0] + (int)amount.Values[0]);
                }
                else
                {
                    UnityEngine.Debug.LogError("Wrong type passed in data type operation");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("Null reference in data type operation");
            }
            return this;
        }

        public override IMLBaseDataType Substract(IMLBaseDataType amount)
        {
            if (amount != null && amount.Values != null && amount.Values.Length > 0)
            {
                if (amount.DataType == DataType || amount.DataType == IMLSpecifications.DataTypes.Float)
                {
                    SetValues((int)m_Values[0] - (int)amount.Values[0]);
                }
                else
                {
                    UnityEngine.Debug.LogError("Wrong type passed in data type operation");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("Null reference in data type operation");
            }
            return this;
        }

    }

}