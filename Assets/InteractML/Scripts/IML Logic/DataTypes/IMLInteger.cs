namespace InteractML
{
    /// <summary>
    /// Integer data type to use in IML models
    /// </summary>
    [System.Serializable]
    public class IMLInteger : IMLBaseDataType
    {
        private float[] m_Values = new float[1];

        public override float[] Values { get { return m_Values; } set { if (value != null) SetValue((int)value[0]); } }

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
                SetValue( (int) newData.Values[0]);

        }

        public IMLInteger(int newData)
        {
            if (m_Values == null)
                m_Values = new float[1];

            m_DataType = IMLSpecifications.DataTypes.Integer;

            SetValue(newData);

        }


        public void SetValue(int newValue)
        {
            m_Values[0] = newValue;
        }

        public int GetValue()
        {
            return (int)m_Values[0];
        }

    }

}