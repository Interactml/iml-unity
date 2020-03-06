using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class serialEx : MonoBehaviour
{
    //note to devs. The ardunity plugin is a bit less obvious if you're on a mac. Write the code to arduino, then start Unity's play mode. Now open the Serial Monitor and all is well. Don't know why you need to do it that way, but you do. On Windows, it'll just send. 
    public SerialController serialController;
    int timeBetweenUpdates = 250;
    System.DateTime lastUpdate = System.DateTime.Now;
    System.DateTime currentTime = System.DateTime.Now;

   
    public float incomingData = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

        serialController = GameObject.Find("SerialController").GetComponent<SerialController>();

    }

    // Update is called once per frame
    void Update()
    {
        currentTime = System.DateTime.Now;
        if ((currentTime - lastUpdate).Milliseconds > timeBetweenUpdates)
        {
            //Debug.Log("bye");
            serialController.SendSerialMessage("bye");
            lastUpdate = currentTime;
        }
        
    }

    // Invoked when a line of data is received from the serial device.
    // This is from the example code for the package Ardity
    void OnMessageArrived(string msg)
    {
        Debug.Log("Arrived: " + msg);
        incomingData = float.Parse(msg); 
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success' will be 
    // 'true' upon connection, and false upon disconnection or failure to connect
    // This is from the example code for the package Ardity
    void OnConnectionEvent(bool success)
    {
        Debug.Log(success ? "Device connected" : "Device disconnected");
    }
}
