using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class TestInOutDataIML : MonoBehaviour
{
    [Header("IML Component Setup")]
    public IMLComponent MLComponent;

    [Header("Values")]
    [SendToIMLController]
    public float ValueToSend;
    [PullFromIMLController]
    public float ValueToPull;

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
