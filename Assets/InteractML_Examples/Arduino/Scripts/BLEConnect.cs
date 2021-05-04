using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class BLEConnect : MonoBehaviour
{
    // Change this to match your device.
    public string targetDeviceName = "Arduino";
    public string serviceUuid = "{00001101-0000-1000-8000-00805f9b34fb}";
    public string[] characteristicUuids = {
         "{00001143-0000-1000-8000-00805f9b34fb}",      // CUUID 1
         "{00001142-0000-1000-8000-00805f9b34fb}"       // CUUID 2
    };

    BLEApi ble;
    BLEApi.BLEScan scan;
    bool isScanning = false, isConnected = false;
    string deviceId = null;
    IDictionary<string, string> discoveredDevices = new Dictionary<string, string>();
    int devicesCount = 0;

    // BLE Threads 
    Thread scanningThread, connectionThread, readingThread;

    // GUI elements
    public TMPro.TextMeshPro TextDiscoveredDevices, TextIsScanning, TextTargetDeviceConnection, TextTargetDeviceData;
    public bool ButtonEstablishConnection, ButtonStartScan;
    int remoteAngle, lastRemoteAngle;

    // Start is called before the first frame update
    void Start()
    {
        ble = new BLEApi();
        ButtonEstablishConnection = false;
        TextTargetDeviceConnection.text = targetDeviceName + " not found.";
        
        StartScanHandler();
        
        readingThread = new Thread(ReadBleData);

        
    }

    // Update is called once per frame
    void Update()
    {
        if (isScanning)
        {
            if (ButtonStartScan)
                ButtonStartScan = false;

            if (discoveredDevices.Count > devicesCount)
            {
                UpdateGuiText("scan");
                devicesCount = discoveredDevices.Count;
            }
        }
        else
        {
            /* Restart scan in same play session not supported yet.
            if (!ButtonStartScan.enabled)
                ButtonStartScan.enabled = true;
            */
            if (TextIsScanning.text != "Not scanning.")
            {
                TextIsScanning.color = Color.white;
                TextIsScanning.text = "Not scanning.";
            }
        }
        Debug.Log(deviceId);
        // The target device was found.
        if (deviceId != null && deviceId != "-1")
        {
            Debug.Log("here");
            if (!ble.isConnected)
                //ConnectBleDevice();
            // Target device is connected and GUI knows.
            if (ble.isConnected && isConnected)
            {
                Debug.Log("write data");
                UpdateGuiText("writeData");
            }
            // Target device is connected, but GUI hasn't updated yet.
            else if (ble.isConnected && !isConnected)
            {
                UpdateGuiText("connected");
                isConnected = true;
                Debug.Log("Device was found, but not connected yet");
                // Device was found, but not connected yet. 
            }
            else if (!ButtonEstablishConnection && !isConnected)
            {
                Debug.Log("Found target device:\n" + targetDeviceName);
                ButtonEstablishConnection = true;
                TextTargetDeviceConnection.text = "Found target device:\n" + targetDeviceName;
            }
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }

    private void OnApplicationQuit()
    {
        CleanUp();
    }

    // Prevent threading issues and free BLE stack.
    // Can cause Unity to freeze and lead
    // to errors when omitted.
    private void CleanUp()
    {
        try
        {
            scan.Cancel();
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Thread or object never initialized.\n" + e);
        }
        try
        {
            ble.Close();
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Thread or object never initialized.\n" + e);
        }
        try
        {
            scanningThread.Abort();
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Thread or object never initialized.\n" + e);
        }
        try
        {
            connectionThread.Abort();
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Thread or object never initialized.\n" + e);
        }
    }

    public void StartScanHandler()
    {
        Debug.Log("start scan");
        devicesCount = 0;
        isScanning = true;
        discoveredDevices.Clear();
        scanningThread = new Thread(ScanBleDevices);
        scanningThread.Start();
        TextIsScanning.color = new Color(244, 180, 26);
        TextIsScanning.text = "Scanning...";
        TextDiscoveredDevices.text = "";
    }

    public void ResetHandler()
    {
        TextTargetDeviceData.text = "";
        TextTargetDeviceConnection.text = targetDeviceName + " not found.";
        // Reset previous discovered devices
        discoveredDevices.Clear();
        TextDiscoveredDevices.text = "No devices.";
        deviceId = null;
        CleanUp();
    }

    private void ReadBleData(object obj)
    {
        byte[] packageReceived = BLEApi.ReadBytes();
        // Convert little Endian.
        // In this example we're interested about an angle
        // value on the first field of our package.
        remoteAngle = packageReceived[0];
        Debug.Log("Angle: " + remoteAngle);
        //Thread.Sleep(100);
    }

    void UpdateGuiText(string action)
    {
        switch (action)
        {
            case "scan":
                TextDiscoveredDevices.text = "";
                foreach (KeyValuePair<string, string> entry in discoveredDevices)
                {
                    TextDiscoveredDevices.text += "DeviceID: " + entry.Key + "\nDeviceName: " + entry.Value + "\n\n";
                    Debug.Log("Added device: " + entry.Key);
                }
                break;
            case "connected":
                ButtonEstablishConnection = false;
                TextTargetDeviceConnection.text = "Connected to target device:\n" + targetDeviceName;
                break;
            case "writeData":
                if (!readingThread.IsAlive)
                {
                    readingThread = new Thread(ReadBleData);
                    readingThread.Start();
                }
                if (remoteAngle != lastRemoteAngle)
                {
                    TextTargetDeviceData.text = "Remote angle: " + remoteAngle;
                    lastRemoteAngle = remoteAngle;
                }
                break;
        }
    }

    void ScanBleDevices()
    {
        scan = BLEApi.ScanDevices();
        Debug.Log("BLE.ScanDevices() started.");
        scan.Found = (_deviceId, deviceName) =>
        {
            Debug.Log("found device with name: " + deviceName + " " +_deviceId);
            discoveredDevices.Add(_deviceId, deviceName);
            Debug.Log(deviceId);
            if (deviceId == null && deviceName == targetDeviceName)
                deviceId = _deviceId;
        };

        scan.Finished = () =>
        {
            isScanning = false;
            Debug.Log("scan finished");
            if (deviceId == null)
            {
                deviceId = "-1";
                Debug.Log(deviceId);
            }
                
        };
        while (deviceId == null)
            Thread.Sleep(500);
        scan.Cancel();
        scanningThread = null;
        isScanning = false;

        if (deviceId == "-1")
        {
            Debug.Log("no device found!");
            return;
        }
        if(deviceId != null || deviceId != "-1")
            StartConHandler();
    }

    // Start establish BLE connection with
    // target device in dedicated thread.
    public void StartConHandler()
    {
        connectionThread = new Thread(ConnectBleDevice);
        connectionThread.Start();
    }

    void ConnectBleDevice()
    {
        if (deviceId != null)
        {
            try
            {
                ble.Connect(deviceId,
                serviceUuid,
                characteristicUuids);
            }
            catch (Exception e)
            {
                Debug.Log("Could not establish connection to device with ID " + deviceId + "\n" + e);
            }
        }
        if (ble.isConnected)
            Debug.Log("Connected to: " + targetDeviceName + deviceId);
    }

    ulong ConvertLittleEndian(byte[] array)
    {
        int pos = 0;
        ulong result = 0;
        foreach (byte by in array)
        {
            result |= ((ulong)by) << pos;
            pos += 8;
        }
        return result;
    }
}