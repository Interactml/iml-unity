using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class HeadsetPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         Transform headsettransform = VRTK_DeviceFinder.HeadsetCamera();
        if(headsettransform != null)
        {
            this.transform.position = headsettransform.position;
            this.transform.rotation = headsettransform.rotation;
        }
         
    }
}
