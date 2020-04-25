using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [Header("Setup")]
    public GameObject BoxToSpawn;
    public float TimeToSpawn;
    public float ForceAtSpawn;

    private TimerController m_Timer;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Timer = this.gameObject.AddComponent<TimerController>();
        m_Timer.ObjectLabel = "Box Spawner Timer";
    }

    // Update is called once per frame
    void Update()
    {
        // Every x seconds...
        if (m_Timer.GenericCountDown(TimeToSpawn))
        {
            // Spawn a box
            GameObject boxSpawned = Instantiate(BoxToSpawn);
            // Move the box to center and launch it up!
            boxSpawned.transform.position = this.transform.position;
            boxSpawned.GetComponent<Rigidbody>().AddForce(Vector3.up * ForceAtSpawn, ForceMode.Impulse);
        }
    }


}
