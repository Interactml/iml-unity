using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class CreateFarm : MonoBehaviour
{
    public int creation;

    private int lastCreation;

    public GameObject person;

    public GameObject[] creations;

    // Start is called before the first frame update
    void Start()
    {
        creation = 0;
        lastCreation = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(creation != lastCreation)
        {
            if(creation != 0)
            {
                Vector3 position = person.transform.position + new Vector3(0, 0, 10);
                Instantiate(creations[creation - 1], position, Quaternion.identity);
            }
            lastCreation = creation;
        }
    }
}
