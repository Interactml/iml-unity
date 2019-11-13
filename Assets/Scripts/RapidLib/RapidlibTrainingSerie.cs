using System.Collections.Generic;
using System;

namespace InteractML
{
    /// <summary>
    /// Holds a serie of training examples
    /// (Useful when working with time series or several frames as one example)
    /// </summary>
    [Serializable]
    public class RapidlibTrainingSerie
    {
        public List<RapidlibTrainingExample> exampleSerie;
    }

}
