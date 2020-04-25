using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDestroyer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // If we collide with a box
        if (collision.gameObject.GetComponent<BoxForces>())
        {
            // We destroy it
            Destroy(collision.gameObject);
        }
    }
}
