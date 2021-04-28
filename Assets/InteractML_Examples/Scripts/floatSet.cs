using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class floatSet : MonoBehaviour
{
    [SendToIMLGraph]
    public float floatSetter;
    
        // Start is called before the first frame update
    void Start()
    {
        floatSetter = 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
