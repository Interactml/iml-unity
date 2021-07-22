using System.Collections.Generic;

namespace InteractML
{
    /// <summary>
    /// Implement this interface when a class will act as training dataset for a ML model
    /// </summary>
    public interface IDataSetIML
    {
        /// <summary>
        /// Training data for kNN (classification), MLP (regression)
        /// </summary>
        List<IMLTrainingExample> TrainingExamplesVector { get; }
        /// <summary>
        /// Training data for DTW (time series classification)
        /// </summary>
        List<IMLTrainingSeries> TrainingSeriesCollection { get; }

    }
}