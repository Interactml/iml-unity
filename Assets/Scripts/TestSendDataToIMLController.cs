using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class TestSendDataToIMLController : MonoBehaviour
{
    public IMLComponent IMLcomponentToSendData;

    [SendToIMLController]
    public float floatValue;

    [SendToIMLController]
    public int intValue;

    [SendToIMLController]
    public Vector2 v2Value;

    [SendToIMLController]
    public Vector3 v3Value;

    [SendToIMLController]
    public Vector4 v4Value;
    
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
        
    }
}
