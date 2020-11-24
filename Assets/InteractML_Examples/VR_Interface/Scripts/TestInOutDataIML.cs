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
    [SendToIMLGraph]
    public float ValueToSend;
    [PullFromIMLGraph]
    public float ValueToPull;

    [Header("Integers")]
    [SendToIMLGraph]
    public int ValueToSendInt;
    [PullFromIMLGraph]
    public int ValueToPullInt;

    [Header("Vector 2")]
    [SendToIMLGraph]
    public Vector2 ValueToSendV2;
    [PullFromIMLGraph]
    public Vector2 ValueToPullV2;

    [Header("Vector 3")]
    [SendToIMLGraph]
    public Vector3 ValueToSendV3;
    [PullFromIMLGraph]
    public Vector3 ValueToPullV3;

    [Header("Vector 4")]
    [SendToIMLGraph]
    public Vector4 ValueToSendV4;
    [PullFromIMLGraph]
    public Vector4 ValueToPullV4;

    [Header("Arrays")]
    [SendToIMLGraph]
    public float[] ValueToSendArray;
    [PullFromIMLGraph]
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
