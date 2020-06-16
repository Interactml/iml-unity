using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class TestNicolaClariceSendScript : MonoBehaviour
{
    [SendToIMLController]
    public Vector3 SendToGraph;

    [PullFromIMLController]
    public float ReceiveFromGraph;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
