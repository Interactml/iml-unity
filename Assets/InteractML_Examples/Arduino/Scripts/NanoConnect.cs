using UnityEngine;
using System.IO.Ports;


public class NanoConnect : MonoBehaviour
{
    public SerialPort serial = new SerialPort("COM8");
    string[] selectedDevice;
    string read_characteristic = "00001143-0000-1000-8000-00805f9b34fb";
    string write_characteristic = "00001142-0000-1000-8000-00805f9b34fb";

    public bool isScanningDevices = true;
    // Start is called before the first frame update
    void Start()
    {

            serial.Open(); 
            serial.DtrEnable = true; //Configuramos control de datos por DTR. // We configure data control by DTR. 
            serial.ReadTimeout = 500; 
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(serial.ReadLine());
    }
}

    
