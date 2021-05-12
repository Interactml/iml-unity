using System.Collections;
using System.Collections.Generic;


namespace InteractML
{
    /// <summary>
    /// Simple class specifying Input/Outputs and Features supported by IML
    /// </summary>
    public class IMLSpecifications
    {
        /// <summary>
        /// Defines the list of data types available. Modify this enum to add your custom data types!
        /// </summary>
        public enum DataTypes { Float, Integer, Vector2, Vector3, Vector4, Array, Boolean }

        /// <summary>
        /// Defines what kind of INPUTS a model supports
        /// </summary>
        public enum InputsEnum
        {
            Float = DataTypes.Float,
            Integer = DataTypes.Integer,
            Vector2 = DataTypes.Vector2,
            Vector3 = DataTypes.Vector3,
            Vector4 = DataTypes.Vector4,
            Array = DataTypes.Array,
            Boolean = DataTypes.Boolean
        }

        /// <summary>
        /// Defines what kind of OUTPUTS a model supports
        /// </summary>
        public enum OutputsEnum
        {
            Float = DataTypes.Float,
            Integer = DataTypes.Integer,
            Vector2 = DataTypes.Vector2,
            Vector3 = DataTypes.Vector3,
            Vector4 = DataTypes.Vector4,
            Array = DataTypes.Array,
            Boolean = DataTypes.Boolean
        }

        /// <summary>
        /// Defines what are the learning types of the IML System
        /// </summary>
        public enum LearningType { Classification, Regression, DTW }

        /// <summary>
        /// Defines the possible status of the training process
        /// </summary>
        public enum ModelStatus { Untrained, Training, Trained, Running }

        public enum TrainingSetType { SingleTrainingExamples, SeriesTrainingExamples }


    }
}

