using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace InteractML.Debugging
{
    /// <summary>
    /// Component containing methods to ease debugging aspects of InteractML
    /// </summary>
    public class EditorDebugger : MonoBehaviour
    {
        public static void PrintIMLEventToggleRecordMethods()
        {
            if (IMLEventDispatcher.ToggleRecordCallback != null)
            {
                var methodList = IMLEventDispatcher.ToggleRecordCallback.GetInvocationList();
                if (methodList != null)
                {
                    if (methodList.Length == 0) Debug.Log("No methods in ToggleRecordCallback!");
                    foreach (var method in methodList)
                    {
                        Debug.Log($"{method.GetMethodInfo().Name}");
                    }

                }

            }
            else
            {
                Debug.Log("No methods in ToggleRecordCallback!");
            }
        }

        public static void PrintIMLEventStartRecordMethods()
        {
            if (IMLEventDispatcher.StartRecordCallback != null)
            {
                var methodList = IMLEventDispatcher.StartRecordCallback.GetInvocationList();
                if (methodList != null)
                {
                    if (methodList.Length == 0) Debug.Log("No methods in StartRecordCallback!");
                    foreach (var method in methodList)
                    {
                        Debug.Log($"{method.GetMethodInfo().Name}");
                    }

                }

            }
            else
            {
                Debug.Log("No methods in StartRecordCallback!");
            }
        }

        public static void PrintIMLEventStopRecordMethods()
        {
            if (IMLEventDispatcher.StopRecordCallback != null)
            {
                var methodList = IMLEventDispatcher.StopRecordCallback.GetInvocationList();
                if (methodList != null)
                {
                    if (methodList.Length == 0) Debug.Log("No methods in StopRecordCallback!");
                    foreach (var method in methodList)
                    {
                        Debug.Log($"{method.GetMethodInfo().Name}");
                    }

                }

            }
            else
            {
                Debug.Log("No methods in StopRecordCallback!");
            }
        }

    }

}
