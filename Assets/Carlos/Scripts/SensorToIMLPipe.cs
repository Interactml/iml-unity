using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using System.Linq;

/// <summary>
/// Pipes data from Vive Sensors to an iml controller
/// </summary>
public class SensorToIMLPipe : MonoBehaviour
{
    // Scripts to set up
    [Header("Sensor Scripts")]
    public GetLipsValues LipSensor;
    public GetEyeValues EyesSensor;

    [Header("ML Component")]
    public IMLComponent MLComponent;

    // Values to send to IMLController
    // Lips
    [SendToIMLController]
    public float[] LipReadings;
    // Eyes
    [SendToIMLController]
    public float PupilDiameterLeft, PupilDiameterRight;
    [SendToIMLController]
    public Vector2 PupilPositionLeft, PupilPositionRight;
    [SendToIMLController]
    public float EyeOpenLeft, EyeOpenRight;




    // Start is called before the first frame update
    void Start()
    {
        // subscribe to ml component to send values
        if (MLComponent)
        {
            MLComponent.SubscribeToIMLController(this);
        }
        // Format array to send correctly
        if (LipSensor)
        {
            CheckLipReadingsSize(ref LipReadings, LipSensor);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (LipSensor)
        {
            CheckLipReadingsSize(ref LipReadings, LipSensor);
            // Read values from lipsensor
            for (int i = 0; i < LipReadings.Length; i++)
            {
                var reading = LipSensor.LipWeightings.ElementAt(i).Value;
                LipReadings[i] = reading;
            }

        }

        if (EyesSensor)
        {
            // Read values from eye sensor
            PupilDiameterLeft = EyesSensor.pupilDiameterLeft;
            PupilDiameterRight = EyesSensor.pupilDiameterRight;

            PupilPositionLeft = EyesSensor.pupilPositionLeft;
            PupilPositionRight = EyesSensor.pupilPositionRight;

            EyeOpenLeft = EyesSensor.eyeOpenLeft;
            EyeOpenRight = EyesSensor.eyeOpenRight;            
        }
    }

    // Check that array of readings is in the correct size
    private void CheckLipReadingsSize(ref float[] lipReadingsArray, GetLipsValues lipSensorScript)
    {
        if (lipSensorScript.LipWeightings != null)
        {
            // If lipreadings array null or incorrect size
            if (lipReadingsArray == null)
            {
                lipReadingsArray = new float[lipSensorScript.LipWeightings.Count];
            }
            else if (LipReadings.Length != lipSensorScript.LipWeightings.Count)
            {
                lipReadingsArray = new float[lipSensorScript.LipWeightings.Count];
            }

        }

    }

}
