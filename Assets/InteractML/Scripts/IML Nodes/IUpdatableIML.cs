using System.Collections;
using UnityEngine;

namespace InteractML
{
    public interface IUpdatableIML
    {
        /// <summary>
        /// Flag to check if the class can be externally updatable
        /// </summary>
        bool isExternallyUpdatable { get; }

        /// <summary>
        /// Flag to check if the class is updated already
        /// </summary>
        bool isUpdated { get; set; }

        /// <summary>
        /// Function to call to run code to update class
        /// </summary>
        void Update();

        /// <summary>
        /// Called after update has finished
        /// </summary>
        void LateUpdate();
    }
}