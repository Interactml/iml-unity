using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Lip;
using System.Linq;

public class GetLipsValues : MonoBehaviour
{
    [Header("Options")]
    public bool DebugData;

    [Header("Lips Readings")]
    public Dictionary<LipShape, float> LipWeightings;
    private string LipsDataToPrint = "";

    // Update is called once per frame
    void Update()
    {
        SRanipal_Lip.GetLipWeightings(out LipWeightings);

        if (DebugData)
        {
            Debug.Log("Keys: " + LipWeightings.Keys + " Val: " + LipWeightings.Values);

            foreach (KeyValuePair<LipShape, float> kvp in LipWeightings)
            {
                //cartof
                LipsDataToPrint += string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }

            Debug.Log(LipsDataToPrint);
            LipsDataToPrint = "";
        }

    }
}
