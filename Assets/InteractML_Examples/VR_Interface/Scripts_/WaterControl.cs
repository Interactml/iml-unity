using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class WaterControl : MonoBehaviour
{
    //Declare variables to control
    private GameObject water;
    private Transform waterScale;

    [PullFromIMLController]     //Allow InteractML model to control the scale of water
    public float x, y, z;

    void Start()
    {
        //Get the transform component
        waterScale = GetComponent<Transform>();
        Debug.Log("Ready");

        if (waterScale == null)
        {
            x = 1f;
            y = 1f;
            z = 1f;
        }
    }

    void Update()
    {
        Debug.Log($"Received x: {x}, y: {y}, z: {z}");

        if (transform.localScale.x > 1)
        {
            transform.localScale += new Vector3(1f, 0, 0);
            Debug.Log("Recieved 2");
        }
    }
}