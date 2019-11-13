using System;

namespace InteractML
{
    /// <summary>
    /// Holds a training example ready to be used with Rapidlib
    /// </summary>
    [Serializable]
    public class RapidlibTrainingExample
    {
        public double[] input;
        public double[] output;
    }

}

