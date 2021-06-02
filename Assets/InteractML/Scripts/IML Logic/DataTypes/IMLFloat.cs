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

        public override float[] Values { get { return m_Values; } set { if(value != null) SetValue(value[0]);  } }

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
                SetValue(newData.Values[0]);

        }

        public IMLFloat(float newData)
        {
            if (m_Values == null)
                m_Values = new float[1];

            m_DataType = IMLSpecifications.DataTypes.Float;

            SetValue(newData);

        }

        public void SetValue(float newValue)
        {
            m_Values[0] = newValue;
        }

        public float GetValue()
        {
            return m_Values[0];
        }

    }

}