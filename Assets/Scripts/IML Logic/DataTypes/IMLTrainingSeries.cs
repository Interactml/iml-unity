using System.Collections;
using System.Collections.Generic;
using System;

namespace InteractML
{
    /// <summary>
    /// Holds a series of IML inputs following the format for DTW
    /// (Useful when working with time series or several frames as one example)
    /// </summary>
    [Serializable]
    public struct IMLTrainingSeries
    {
        private List<IMLInput> m_Series;
        /// <summary>
        /// The series of input features
        /// </summary>
        public List<IMLInput> Series { get => m_Series; }

        private string m_LabelSeries;
        /// <summary>
        /// the label (output) for this serie
        /// </summary>
        public string LabelSeries { get => m_LabelSeries; set => m_LabelSeries = value; }


        #region Constructors

        public IMLTrainingSeries(List<IMLInput> serie)
        {
            if (serie != null)
            {
                m_Series = new List<IMLInput>(serie);
            }
            else
            {
                m_Series = new List<IMLInput>();
            }

            m_LabelSeries = "";
        }

        public IMLTrainingSeries(List<IMLInput> serie, string LabelSeries)
        {
            if (serie != null)
            {
                m_Series = new List<IMLInput>(serie);
            }
            else
            {
                m_Series = new List<IMLInput>();
            }

            if (!String.IsNullOrEmpty(LabelSeries))
            {
                m_LabelSeries = LabelSeries;
            }
            else
            {
                m_LabelSeries = "";

            }

        }

        public IMLTrainingSeries(IMLTrainingSeries serieToCopy)
        {
            if (serieToCopy.Series != null)
                m_Series = new List<IMLInput>(serieToCopy.Series);
            else
                m_Series = new List<IMLInput>();

            if (!String.IsNullOrEmpty(serieToCopy.LabelSeries))
                m_LabelSeries = serieToCopy.LabelSeries;
            else
                m_LabelSeries = "";

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a training example input into the serie
        /// </summary>
        /// <param name="input"></param>
        public void AddTrainingExample(IMLInput input)
        {
            if (m_Series == null)
                m_Series = new List<IMLInput>();
            // Create a new instance to avoid passing a reference from outside to our list
            IMLInput inputSerie = new IMLInput(input);

            m_Series.Add(inputSerie);
        }

        /// <summary>
        /// Adds a training example into the serie (all outputs must be the same per serie)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddTrainingExample(IMLInput input, string output)
        {
            if (m_Series == null)
                m_Series = new List<IMLInput>();
            // Create a new instance to avoid passing a reference from outside to our list
            IMLInput inputSerie = new IMLInput(input);

            m_Series.Add(inputSerie);
            m_LabelSeries = output;
        }

        /// <summary>
        /// Clears serie inputs and label
        /// </summary>
        public void ClearSerie()
        {
            if (m_Series == null)
                m_Series = new List<IMLInput>();
            else
                m_Series.Clear();

            m_LabelSeries = "";
        }

        #endregion

    }

}
