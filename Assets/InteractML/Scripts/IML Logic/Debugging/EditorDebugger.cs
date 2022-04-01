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
        public static void PrintIMLEventToggleTrainMethods()
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
        }

    }

}
