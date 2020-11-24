using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class DanceControl : MonoBehaviour
{
    [PullFromIMLGraph]
    public int danceMove;
    public List<Animator> dancers; 
    // Start is called before the first frame update
    void Start()
    {
        danceMove = 0;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Animator dancer in dancers)
        {
            dancer.SetInteger("Dance", danceMove);
        }
    }
}
