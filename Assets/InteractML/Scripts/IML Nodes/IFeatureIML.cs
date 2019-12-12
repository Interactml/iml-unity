namespace InteractML
{
    /// <summary>
    /// Implement this interface when a class will act as an input/output feature for the IML system
    /// </summary>
    public interface IFeatureIML
    {
        /// <summary>
        /// Values returned by this feauture
        /// </summary>
        IMLBaseDataType FeatureValues { get; }

        /// <summary>
        /// Flag to check if the feature can be externally updatable
        /// </summary>
        bool isExternallyUpdatable { get; }

        /// <summary>
        /// Flag to check if the feature is updated already
        /// </summary>
        bool isUpdated { get; set; }

        /// <summary>
        /// Function to call to run code to update feature values
        /// </summary>
        object UpdateFeature();

    }

}
