using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.Interaction.Toolkit
{
    internal static class SortingHelpers
    {
        public static void Sort<T>(IList<T> hits, IComparer<T> comparer) where T : struct
        {
            T temp;

            bool fullPass = true;
            do
            {
                fullPass = true;
                for (int i = 1; i < hits.Count; i++)
                {
                    int result = comparer.Compare(hits[i - 1], hits[i]);
                    if (result > 0)
                    {
                        temp = hits[i - 1];
                        hits[i - 1] = hits[i];
                        hits[i] = temp;
                        fullPass = false;
                    }
                }
            } while (fullPass == false);
        }
    }
}
