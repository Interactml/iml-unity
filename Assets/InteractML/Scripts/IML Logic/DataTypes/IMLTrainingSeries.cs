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
        private List<List<IMLInput>> m_Series;
        /// <summary>
        /// The series of input features
        /// </summary>
        public List<List<IMLInput>> Series { get => m_Series; set => m_Series = value; }

        private string m_LabelSeries;
        /// <summary>
        /// the label (output) for this serie
        /// </summary>
        public string LabelSeries { get => m_LabelSeries; set => m_LabelSeries = value; }


        #region Constructors

        public IMLTrainingSeries(List<List<IMLInput>> serie)
        {
            if (serie != null)
            {
                m_Series = new List<List<IMLInput>>(serie);
            }
            else
            {
                m_Series = new List<List<IMLInput>>();
            }

            m_LabelSeries = "";
        }

        public IMLTrainingSeries(List<List<IMLInput>> serie, string LabelSeries)
        {
            if (serie != null)
            {
                m_Series = new List<List<IMLInput>>(serie);
            }
            else
            {
                m_Series = new List<List<IMLInput>>();
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
                m_Series = new List<List<IMLInput>>(serieToCopy.Series);
            else
                m_Series = new List<List<IMLInput>>();

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
        public void AddFeatures(List<IMLInput> input)
        {
            if (m_Series == null)
                m_Series = new List<List<IMLInput>>();

            // Create a new instance to avoid passing a reference from outside to our list
            m_Series.Add(new List<IMLInput>(input));
           
        }

        /// <summary>
        /// Adds a training example into the series (all outputs must be the same per serie)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public void AddFeatures(List<IMLInput> input, string output)
        {
            if (m_Series == null)
                m_Series = new List<List<IMLInput>>();

            // Create a new instance to avoid passing a reference from outside to our list
            m_Series.Add(new List<IMLInput>(input));

            m_LabelSeries = output;
        }

        /// <summary>
        /// Clears series inputs and label
        /// </summary>
        public void ClearSerie()
        {
            if (m_Series == null)
                m_Series = new List<List<IMLInput>>();
            else
                m_Series.Clear();

            m_LabelSeries = "";
        }

        public List<double[]> GetSeriesFeatures()
        {
            List<double[]> valuesToReturn = new List<double[]>();

            if (m_Series == null)
                m_Series = new List<List<IMLInput>>();

            // Go through features
            foreach (var features in m_Series)
            {
                // Add each feature to series to return
                foreach (var feature in features)
                {
                    double[] newFeature = new double[feature.InputData.Values.Length];
                    feature.InputData.Values.CopyTo(newFeature, 0);
                    valuesToReturn.Add(newFeature);
                }
            }

            return valuesToReturn;
        }

        #endregion

    }

}
