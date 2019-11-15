using System.Collections.Generic;
using System;

namespace InteractML
{
    /// <summary>
    /// Holds a serie of training examples
    /// (Useful when working with time series or several frames as one example)
    /// </summary>
    [Serializable]
    public struct RapidlibTrainingSerie
    {
        private List<RapidlibTrainingExample> exampleSerie;

        public RapidlibTrainingSerie(List<RapidlibTrainingExample> serie)
        {
            if (serie != null)
            {
                exampleSerie = new List<RapidlibTrainingExample>(serie);
            }
            else
            {
                exampleSerie = new List<RapidlibTrainingExample>();
            }
        }

        public void AddTrainingExample(RapidlibTrainingExample trainingExample)
        {
            exampleSerie.Add(trainingExample);
        }
    }

}
