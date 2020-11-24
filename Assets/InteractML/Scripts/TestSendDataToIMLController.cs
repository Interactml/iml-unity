using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class TestSendDataToIMLController : MonoBehaviour
{
    public IMLComponent IMLcomponentToSendData;

    [SendToIMLGraph]
    public float floatValueToSend;

    public double[][] valuesPulled;

    public double oneValuePulled;


    // Start is called before the first frame update
    void Start()
    {
        if (IMLcomponentToSendData)
        {
            IMLcomponentToSendData.SubscribeToIMLController(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Copy outputs array
        IMLcomponentToSendData.IMLControllerOutputs.CopyTo(valuesPulled);

        // Get first of the outputs
        oneValuePulled = valuesPulled[0][0];
    }
}
