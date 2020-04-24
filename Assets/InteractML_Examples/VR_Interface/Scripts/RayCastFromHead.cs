using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastFromHead : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // Throw a raycast forward
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        // Check for a box
        if (Physics.Raycast(ray, out hit, 10000))
        {
            BoxForces box = hit.collider.GetComponent<BoxForces>();
            if (box)
            {
                // Make sure box is in sight 
                box.IsBoxInSight = true;
                box.HighlightBox(true);
            }
        }
    }
}
