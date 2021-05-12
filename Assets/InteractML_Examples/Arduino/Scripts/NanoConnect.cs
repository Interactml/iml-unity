using UnityEngine;
using System.IO.Ports;
using InteractML;


public class NanoConnect : MonoBehaviour
{
    public string port;
    public SerialPort serial;
    public bool isScanningDevices = true;

    [SendToIMLGraph]
    public float[] outArray;
    // Start is called before the first frame update
    void Start()
    {
        serial = new SerialPort(port);
        if(serial != null) {
            serial.Open();
            serial.DtrEnable = true; //Configuramos control de datos por DTR. // We configure data control by DTR. 
            serial.ReadTimeout = 500;
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(serial.ReadLine());
        if (serial != null && serial.IsOpen)
        {
            string data = serial.ReadLine();
            string[] sStrings = data.Split(","[0]);
            outArray = new float[sStrings.Length];
            for (int i = 0; i < 3; i++)
            {
                outArray[i] = float.Parse(sStrings[i]);
            }
            Debug.Log(outArray.Length);
        }
    }
}

    
