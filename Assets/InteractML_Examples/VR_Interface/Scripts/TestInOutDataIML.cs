using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class TestInOutDataIML : MonoBehaviour
{
    [Header("IML Component Setup")]
    public IMLComponent MLComponent;

    [Header("Values")]
    [Header("Floats")]
    [SendToIMLController]
    public float ValueToSend;
    [PullFromIMLController]
    public float ValueToPull;

    [Header("Integers")]
    [SendToIMLController]
    public int ValueToSendInt;
    [PullFromIMLController]
    public int ValueToPullInt;

    [Header("Vector 2")]
    [SendToIMLController]
    public Vector2 ValueToSendV2;
    [PullFromIMLController]
    public Vector2 ValueToPullV2;

    [Header("Vector 3")]
    [SendToIMLController]
    public Vector3 ValueToSendV3;
    [PullFromIMLController]
    public Vector3 ValueToPullV3;

    [Header("Vector 4")]
    [SendToIMLController]
    public Vector4 ValueToSendV4;
    [PullFromIMLController]
    public Vector4 ValueToPullV4;

    [Header("Arrays")]
    [SendToIMLController]
    public float[] ValueToSendArray;
    [PullFromIMLController]
    public float[] ValueToPullArray;


    // Start is called before the first frame update
    void Start()
    {
        if (MLComponent)
        {
            MLComponent.SubscribeToIMLController(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
