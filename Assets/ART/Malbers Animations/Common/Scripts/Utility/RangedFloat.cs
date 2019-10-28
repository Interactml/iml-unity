namespace MalbersAnimations
{
    /// <summary>
    /// Create a Range with a max and a min float value 
    /// </summary>
    [System.Serializable]
    public struct RangedFloat
    {
        public float minValue;
        public float maxValue;

        public RangedFloat(float minValue, float maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /// <summary>
        /// Returns a RandomValue between Min and Max
        /// </summary>
        public float RandomValue
        {
            get { return UnityEngine.Random.Range(minValue, maxValue); }
        }

        /// <summary>
        /// Is the value in between the min and max of the FloatRange
        /// </summary>
        public bool IsInRange(float value)
        {
            return value >= minValue && value <= maxValue;
        }
    }
}