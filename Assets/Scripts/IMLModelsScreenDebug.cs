using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IMLModelsScreenDebug : MonoBehaviour
{
    private RapidLib[] m_IMLModels;
    private string m_TextToPrint;
    
    // Start is called before the first frame update
    void Start()
    {
        m_IMLModels = FindObjectsOfType<RapidLib>();
    }

    private void OnGUI()
    {
    //    m_TextToPrint = "";
    //    for (int i = 0; i < m_IMLModels.Length; i++)
    //    {
    //        string statusString = "INACTIVE";

    //        if (m_IMLModels[i].CollectingData)
    //        {
    //            statusString = "COLLECTING DATA";
    //        }
    //        else if (m_IMLModels[i].Running)
    //        {
    //            statusString = "RUNNING";
    //        }

    //        m_TextToPrint += "\n " + m_IMLModels[i].name + " is: " + statusString;
    //    }

    //    GUILayout.Label(m_TextToPrint);
    }
}
