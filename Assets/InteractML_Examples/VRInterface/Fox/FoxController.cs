using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class FoxController : MonoBehaviour
{
    [PullFromIMLGraph]
    public int trick;

    // Start is called before the first frame update
    void Start()
    {
        trick = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Animator>().SetInteger("trick", trick);
    }
}
