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

        public RapidlibTrainingSerie(RapidlibTrainingSerie serieToCopy)
        {
            if (serieToCopy.ExampleSerie != null)
                m_ExampleSerie = new List<double[]>(serieToCopy.ExampleSerie);
            else
                m_ExampleSerie = new List<double[]>();

            if (!String.IsNullOrEmpty(serieToCopy.LabelSerie))
                m_LabelSerie = serieToCopy.LabelSerie;
            else
                m_LabelSerie = "";

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a training example input into the serie
        /// </summary>
        /// <param name="input"></param>
        public void AddTrainingExample(double[] input)
        {
            if (m_ExampleSerie == null)
                m_ExampleSerie = new List<double[]>();
            // Create a new instance to avoid passing a reference from outside to our list
            double[] inputSerie = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                inputSerie[i] = input[i];
            }
            m_ExampleSerie.Add(inputSerie);
        }

        /// <summary>
        /// Adds a training example into the serie (all outputs must be the same per serie)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTrainingExample(double[] input, string output)
        {
            if (m_ExampleSerie == null)
                m_ExampleSerie = new List<double[]>();
            // Create a new instance to avoid passing a reference from outside to our list
            double[] inputSerie = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                inputSerie[i] = input[i];
            }
            m_ExampleSerie.Add(inputSerie);
            m_LabelSerie = output;
        }

        /// <summary>
        /// Clears serie inputs and label
        /// </summary>
        public void ClearSerie()
        {
            if (m_ExampleSerie == null)
                m_ExampleSerie = new List<double[]>();
            else
                m_ExampleSerie.Clear();

            m_LabelSerie = "";
        }

        #endregion
    }

}
