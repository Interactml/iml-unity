using UnityEngine;
using InteractML;

public class HeadDirectionTrackerIML : MonoBehaviour
{
    [Header("IML Setup")]
    public IMLComponent IMLSystem;

    [SendToIMLGraph]
    public Vector3 HeadRotation;

    private void Start()
    {
        if (IMLSystem)
        {
            IMLSystem.SubscribeToIMLController(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HeadRotation = this.transform.rotation.eulerAngles;
    }
}
