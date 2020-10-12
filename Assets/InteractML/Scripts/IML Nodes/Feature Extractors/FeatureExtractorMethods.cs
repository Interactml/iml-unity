using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor;
using XNodeEditor;
#endif

namespace InteractML.FeatureExtractors
{
    /// </summary>
    /// Contains extension methods for data type node classes
    /// </summary>
    public static class FeatureExtractorMethods
    {
        
        /// </summary>
     /// Checks if receiving data within an amount of frames (Count)
     /// </summary>
     /// <param name="data type node"></param>
     /// <return></return>
        public static bool IsReceivingData(this BaseExtractorNode node)
        {

            //Check if count has counted to Counter
            if (node.Counter == node.Count)
            {
                //reset counter
                node.Counter = 0;

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
                        }
                        else
                        {
                            node.ReceivingData = true;
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
    }
}
