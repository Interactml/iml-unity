using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using XNode;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.DataTypeNodes
{
    /// </summary>
    /// Contains extension methods for data type node classes
    /// </summary>
    public static class DataTypeNodeMethods
    {
        /// </summary>
        /// Checks if receiving data within an amount of frames (Count)
        /// </summary>
        /// <param name="data type node"></param>
        /// <return></return>
        public static bool IsReceivingData<T>(this BaseDataTypeNode<T> node)
        {
            //Check if count has counted to Counter
            if (node.Counter == node.Count)
            {
                //reset counter
                node.Counter = 0;

                // Check for null
                if (node.PreviousFeatureValues == null || node.PreviousFeatureValues.Values == null)
                {
                    Debug.LogError("There are null references in node, aborting data pulling!");
                    return false;
                }

                // check if storage types for new feature values and previous values hold the same number of values to compare
                if (node.PreviousFeatureValues.Values.Length == node.FeatureValues.Values.Length)
                {
                    // for each of the feature values
                    for (int i = 0; i < node.FeatureValues.Values.Length; i++)
                    {
                        //check if previously stored features values match current feature values and set ReceivingData boolean fields accordingly
                        if (node.FeatureValues.Values[i] == node.PreviousFeatureValues.Values[i])
                        {
                            node.ReceivingData = false;
                            node.FeatureValueReceivingData[i] = false;
                        }
                        else
                        {
                            node.ReceivingData = true;
                            node.FeatureValueReceivingData[i] = true;
                            break;
                        }
                    }
                }

                //reset previous values stored for feature values
                node.PreviousFeatureValues.Values = node.FeatureValues.Values;
            }
            //Increment Counter
            node.Counter++;
            return node.ReceivingData;
        }

        /// </summary>
        /// Checks if input is connected to node and returns result
        /// </summary>
        /// <param name="data type node"></param>
        /// <return></return>
        public static bool IsInputConnected<T>(this BaseDataTypeNode<T> node)
        {
            //If there is no input connected take input from the user
            if (node.GetInputNodesConnected("m_In") == null)
            {
                return false;
            }
            return true;
        }

        /// </summary>
        /// Checks if toggle flag if on/off for each feature and returns updated float array 
        /// </summary>
        /// <param name="data type node"></param>
        /// <param name="float array"></param>
        /// <return></return>
        public static float[] CheckTogglesAndUpdateFeatures<T>(this BaseDataTypeNode<T> node, float[] value)
        {
            if (node.ToggleSwitches != null && node.FeatureValues.Values != null)
            {
                // checks the amount of feature values matches the size of the amount of toggles and items in the float array, throws an error otherwise
                if (node.ToggleSwitches.Length == node.FeatureValues.Values.Length)
                {
                    if (value.Length == node.FeatureValues.Values.Length)
                    {
                        // for each of the feature values 
                        for (int i = 0; i < node.FeatureValues.Values.Length; i++)
                        {
                            // set any values where the toggle is off to 0
                            if (!node.ToggleSwitches[i]) { value[i] = 0; }
                        }
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
        /// Checks if input to input port is an array, if so check that the size of the array is the same as the number of features 
        /// </summary>
        /// <param name="data type node"></param>
        /// <param name="to port"></param>
        /// <param name="from port"></param>
        /// <return></return>
        public static void CheckArraySizeAgainstFeatureValues<T>(this BaseDataTypeNode<T> node, NodePort from, NodePort to)
        {
            Debug.Log(from.node);
            Debug.Log(to.GetInputValue().GetType());
            // if connected a float array to input port
            if (to.GetInputValue().GetType() == typeof(float[]))
            {
                // check that it size matches features in data type otherwise disconnect float array
                if (to.node.GetInputValue<float[]>("m_In").Length != node.FeatureValues.Values.Length) { from.Disconnect(to); }
            }
        }

        /// </summary>
        /// Initialise size of boolean array storing toggle switch value to match amount of feature values 
        /// </summary>
        /// <param name="movement feature node"></param>
        /// <param name="new array size"></param>
        /// <return></return>
        public static void UpdateToggleSwitchArray<T>(this BaseDataTypeNode<T> node, int size)
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
        public static void UpdateReceivingDataArray<T>(this BaseDataTypeNode<T> node, int size)
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
