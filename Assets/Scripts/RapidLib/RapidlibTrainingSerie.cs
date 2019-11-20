using System.Collections.Generic;
using System;

namespace InteractML
{
    /// <summary>
    /// Holds a serie of training examples following the format for DTW
    /// (Useful when working with time series or several frames as one example)
    /// </summary>
    [Serializable]
    public struct RapidlibTrainingSerie
    {
        private List<double[]> m_ExampleSerie;
        /// <summary>
        /// The example serie of features to input
        /// </summary>
        public List<double[]> ExampleSerie { get { return m_ExampleSerie; } }

        private string m_LabelSerie;
        /// <summary>
        /// The label (output) for this serie
        /// </summary>
        public string LabelSerie { get => m_LabelSerie; set => m_LabelSerie = value; }

        #region Constructors

        public RapidlibTrainingSerie(List<double[]> serie)
        {
            if (serie != null)
            {
                m_ExampleSerie = new List<double[]>(serie);
            }
            else
            {
                m_ExampleSerie = new List<double[]>();
            }

            m_LabelSerie = "";
        }

        public RapidlibTrainingSerie(List<double[]> serie, string labelSerie)
        {
            if (serie != null)
            {
                m_ExampleSerie = new List<double[]>(serie);
            }
            else
            {
                m_ExampleSerie = new List<double[]>();
            }

            if (!String.IsNullOrEmpty(labelSerie))
            {
                m_LabelSerie = labelSerie;
            }
            else
            {
                m_LabelSerie = "";

            }

        }

        #endregion

        #region Public Methods

        public void AddTrainingExample(double[] input, string output)
        {
            m_ExampleSerie.Add(input);
        }

        public void ClearSerie()
        {
            m_ExampleSerie.Clear();
            m_LabelSerie = "";
        }

        #endregion
    }

}
