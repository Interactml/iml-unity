using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class test : MonoBehaviour
{
    [PullFromIMLController]
    public int triggerEvent;

    [SendToIMLController]
    public Vector3 inputData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
