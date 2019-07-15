using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;

public class SendDataToGraphTest : MonoBehaviour
{

    private InteractML.IMLController ControllerToSendData;

    public IMLComponent IMLSystem;

    public GameObject ObjectToExtractDataFrom;

    public string newText;

    // Start is called before the first frame update
    void Start()
    {
        if (IMLSystem != null && IMLSystem.MLController != null)
            ControllerToSendData = IMLSystem.MLController;
    }

    // Update is called once per frame
    void Update()
    {
        //if (ObjectToExtractDataFrom != null)
        //    ObjectToExtractDataFrom.transform.position += Vector3.right * Time.deltaTime * 1.1f;

        if (IMLSystem != null)
        {
            // Send text
            IMLSystem.SendText(newText);
            // Send position
            IMLSystem.SendVector3(ObjectToExtractDataFrom.transform.position);
        }
    }
}
