using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class IMLOutputConsoleDebug : MonoBehaviour
{
    /// <summary>
    /// IML Component to read data from
    /// </summary>
    public IMLComponent MLComponent;
    
    // Update is called once per frame
    void Update()
    {
        // Only read data if ML component is assigned
        if (MLComponent != null)
        {
            // Only read data if there any outputs to read
            if (MLComponent.IMLControllerOutputs.Count > 0)
            {
                // Show each output on console
                foreach (var output in MLComponent.IMLControllerOutputs)
                {
                    string resultToDebug = "Outputs are: \n";
                    for (int i = 0; i < output.Length; i++)
                    {
                        // Add a line break at the end in case we have more than one output
                        resultToDebug += output[i].ToString() + "\n";
                    }
                    // Print on console
                    Debug.Log(resultToDebug);
                }
            }
        }
    }
}
