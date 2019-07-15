using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quick Class to Debug local and world position/rotation of a GObj
/// </summary>
public class ShowDeepTransform : MonoBehaviour
{
    public bool showLocals;
    public Vector3 position;
    public Quaternion rotation;
    
    // Update is called once per frame
    void Update()
    {
        if (showLocals)
        {
            position = transform.localPosition;
            rotation = transform.localRotation;
        }
        else
        {
            position = transform.position;
            rotation = transform.rotation;
        }
    }
}
