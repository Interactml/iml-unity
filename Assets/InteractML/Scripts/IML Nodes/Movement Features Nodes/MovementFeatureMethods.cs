using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.MovementFeatures
{
    /// </summary>
    /// Contains extension methods for data type node classes
    /// </summary>
    public static class MovementFeatureMethods
    {
        
        /// </summary>
     /// Checks if receiving data within an amount of frames (Count)
     /// </summary>
     /// <param name="data type node"></param>
     /// <return></return>
        public static bool IsReceivingData(this BaseMovementFeatureNode node)
        {
            if (node.FeatureValueReceivingData != null)
            {
                //Check if count has counted to Counter
                if (node.Counter == node.Count)
                {
                    //reset counter
                    node.Counter = 0;

                    // check if storage types for new feature values and previous values hold the same number of values to compare
                    if (node.FeatureValues != null && node.PreviousFeatureValues != null)
                    {

                        if (node.PreviousFeatureValues.Values.Length == node.FeatureValues.Values.Length)
                        {
                            // for each of the feature values
                            for (int i = 0; i < node.FeatureValues.Values.Length; i++)
                            {
                                //check if previously stored features values match current feature values and set ReceivingData boolean fields accordingly
                                if (node.FeatureValues.Values[i] == node.PreviousFeatureValues.Values[i])
                                {
                                    node.FeatureValueReceivingData[i] = false;
                                    node.ReceivingData = false;
                                }
                                else
                                {
                                    node.FeatureValueReceivingData[i] = true;
                                    node.ReceivingData = true;
                                }
                            }
                        }


                        //reset previous values stored for feature values
                        node.PreviousFeatureValues.Values = node.FeatureValues.Values;
                    }
                }
            }
            //Increment Counter
            node.Counter++;
            return node.ReceivingData;
        }

        /// </summary>
        /// Checks if toggle flag if on/off for each feature and returns updated float array 
        /// </summary>
        /// <param name="data type node"></param>
        /// <param name="float array"></param>
        /// <return></return>
        public static float[] CheckTogglesAndUpdateFeatures(this BaseMovementFeatureNode node, float[] value)
        {
            // checks the amount of feature values matches the size of the amount of toggles and items in the float array, throws an error otherwise
            // check if storage types for new feature values and previous values hold the same number of values to compare
            if (node.FeatureValues != null && node.ToggleSwitches != null)
            {
                if (node.ToggleSwitches.Length == node.FeatureValues.Values.Length && value.Length == node.FeatureValues.Values.Length)
                {
                    // for each of the feature values 
                    for (int i = 0; i < node.FeatureValues.Values.Length; i++)
                    {
                        // set any values where the toggle is off to 0
                        if (!node.ToggleSwitches[i]) { value[i] = 0; }
                    }
                }
                else
                {
                    Debug.Log("The number of feature values in the node does not match the number of items in the boolean array for toggle switches: Cannot update values");
                }
            }
            return value;
        }

        /// </summary>
        /// Initialise size of boolean array storing toggle switch value to match amount of feature values 
        /// </summary>
        /// <param name="movement feature node"></param>
        /// <param name="new array size"></param>
        /// <return></return>
        public static void UpdateToggleSwitchArray(this BaseMovementFeatureNode node, int size)
        {
            
            if (node.ToggleSwitches != null && node.ToggleSwitches.Length != size)
            {
                // create new array of boolean for each of the features in the data type and set all to true
                node.ToggleSwitches = new bool[size];
                for (int i = 0; i < size; i++) { node.ToggleSwitches[i] = true; }
            }
        }

        /// </summary>
        /// Initialise size of array storing boolean value if feature value is receiving data to match amount of feature values 
        /// </summary>
        /// <param name="movement feature node"></param>
        /// <param name="new array size"></param>
        /// <return></return>
        public static void UpdateReceivingDataArray(this BaseMovementFeatureNode node, int size)
        {
            if (node.FeatureValueReceivingData != null && node.FeatureValueReceivingData.Length != size)
            {
                // create new array of boolean for each of the features in the data type and set all to false
                node.FeatureValueReceivingData = new bool[size];
                for (int i = 0; i < size; i++) { node.FeatureValueReceivingData[i] = false; }
            }
        }
    }
}
