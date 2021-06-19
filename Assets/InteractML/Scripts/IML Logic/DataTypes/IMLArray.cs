using UnityEngine;
using System;

namespace InteractML
{
    /// <summary>
    /// Configurable data vector for configurable serial connections 
    /// </summary>
    [Serializable]
    public class IMLArray : IMLBaseDataType
    {

        [SerializeField]
        private float[] m_Values;

        public override float[] Values { get { return m_Values; } set { if (value != null) SetValues(value); } }

        private IMLSpecifications.DataTypes m_DataType = IMLSpecifications.DataTypes.Array;

        public override IMLSpecifications.DataTypes DataType { get { return m_DataType; } }

        public IMLArray()
        {
            if (m_Values == null)
                m_Values = new float[0];

            m_DataType = IMLSpecifications.DataTypes.Array;
        }

        /// <summary>
        /// Creates IMLArray with size
        /// </summary>
        /// <param name="serialVectorSize"></param>
        public IMLArray(int serialVectorSize)
        {
            m_DataType = IMLSpecifications.DataTypes.Array;
            m_Values = new float[serialVectorSize];
        }

        /// <summary>
        /// Creates an IMLArray from another existing IML Data
        /// </summary>
        /// <param name="newData"></param>
        public IMLArray(IMLBaseDataType newData)
        {
            m_DataType = IMLSpecifications.DataTypes.Array;

            if (newData.Values != null && newData.Values.Length > 0)
                SetValues(newData.Values);
        }

        /// <summary>
        /// Creates an IMLArray from an existing array
        /// </summary>
        /// <param name="newData"></param>
        public IMLArray(float[] newData)
        {
            m_DataType = IMLSpecifications.DataTypes.Array;

            if (newData != null && newData.Length > 0)
                SetValues(newData);

        }

        /// <summary>
        /// Sets a new array of values
        /// </summary>
        /// <param name="newValues"></param>
        public override void SetValues(float[] newValues)
        {
            // don't do anything if the values passed are empty or null
            if (newValues == null || newValues.Length == 0)
                return;

            if (m_Values == null||m_Values.Length == 0||m_Values.Length != newValues.Length)
            {
                //debugging code needs revisting
                if(newValues == null)
                {
                    newValues = new float[0];
                }
                m_Values = new float[newValues.Length];
            }

            for (int i = 0; i < newValues.Length; i++)
            {
                m_Values[i] = newValues[i];
            }
      
        }

        /// <summary>
        /// Set Values in index position
        /// </summary>
        public void SetValues(int index, float newValues)
        {
            if (m_Values != null||m_Values.Length != 0)
            {
                m_Values[index] = newValues;
            }
            else
            {
                Debug.LogError("Array is null or empty, cannot set values!");
            }
      
        }

        public float[] GetValues()
        {
            return m_Values;
        }

    }

}
