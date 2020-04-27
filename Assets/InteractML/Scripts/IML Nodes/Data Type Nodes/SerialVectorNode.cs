using UnityEngine;

namespace InteractML.DataTypeNodes
{
    [NodeWidth(250)]
    public class SerialVectorNode : BaseDataTypeNode<float[]>
    {
        // Override set behaviour to avoid passing data by reference because of arrays
        // Input
        public override float[] In { get => base.In; set => base.In = CopyTo(value, base.In); }

        // Value itself contained in the node
        public override float[] Value { get => base.Value; set => base.Value = CopyTo(value, base.Value); }

        // Output
        public override float[] Out { get => base.Out; set => base.Out = CopyTo(value, base.Out); }

        // IML Feature
        public override IMLBaseDataType FeatureValues
        {
            get
            {
                // Make sure feature value is never null
                if (m_FeatureValues == null)
                    m_FeatureValues = new IMLSerialVector();

                // Update local IML Data copy
                m_FeatureValues.SetValues(Value);
                return m_FeatureValues;
            }
        }
        /// <summary>
        /// Local specific IML data type
        /// </summary>
        private IMLSerialVector m_FeatureValues;

        /// <summary>
        /// Copies data from incoming array to our array
        /// </summary>
        /// <param name="arrayIn"></param>
        /// <param name="arrayOut"></param>
        private float[] CopyTo(float[] arrayIn, float[] arrayOut)
        {
            // Null checks
            if (arrayIn == null)
                throw new System.Exception("Array In is null when attempting to copy in SerialVector node.");
            if (arrayOut == null)
                throw new System.Exception("Array out is null when attempting to copy in SerialVector node.");
            // Length check
            if (arrayIn.Length != arrayOut.Length)
                arrayOut = new float[arrayIn.Length];
            // Copy contents to array out
            arrayIn.CopyTo(arrayOut, 0);
            // return arrayout
            return arrayOut;
        }

    }
}