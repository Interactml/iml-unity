using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML; 

public class Arduino_Data_To_IML : MonoBehaviour
{
    // Start is called before the first frame update

    [SendToIMLController]
    public float dataFromArduino = 0.0f;

    [SendToIMLController]
    public float[] dataArray; 

    public IMLComponent iml;

 
    void Start()
    {
        iml.SubscribeToIMLController(this);
        
    }

    // Update is called once per frame
    void Update()
    {
        dataFromArduino = this.GetComponent<serialEx>().incomingData;

        //gets data out of IML System 
        foreach (var item in iml.IMLControllerOutputs)
        {
            for (int i = 0; i < item.Length; i++)
            {
                Debug.Log(item[i]);
            }
        }
    }
}
