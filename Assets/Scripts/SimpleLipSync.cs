using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLipSync : MonoBehaviour
{
    private CheckAudioOutput audioOutputDataScript;
    float currentMouthLevel;
    float blendSpeed = 5.0f;
    int mouthOpenBlendshapesIndex = 35; //this is the index from the blendshapes;
    int midMouthRightBlendshapesIndex = 31;
    int midMouthLeftBlendshapesIndex = 30;
    int jawDownBlendshapesIndex = 22; //this is the index from the blendshapes;
    SkinnedMeshRenderer skinnedMeshRenderer;

    bool printDebug = false;
    bool runningCoroutine = false;
    bool isMouthClosed = true;

    public float SpectrumAvgThreshold;
    // Start is called before the first frame update
    void Awake()
    {
        //get the element that has the currentr vd spectrum data
        audioOutputDataScript = GameObject.FindObjectOfType<CheckAudioOutput>();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        currentMouthLevel = 0;
        SpectrumAvgThreshold = 0.0002f;
    }

    // Update is called once per frame
    void Update()
    {

        //print the current avg
        //Debug.Log("current avg is: " + audioOutputDataScript.CurrentSpectrumAvg);

        if (audioOutputDataScript.CurrentSpectrumAvg >= SpectrumAvgThreshold)
        {
            float nextMouthLevel = audioOutputDataScript.CurrentSpectrumAvg * 100000;
            if (nextMouthLevel > 40) nextMouthLevel = 40f;
            nextMouthLevel = Mathf.Lerp(10f, 40f, nextMouthLevel);
            nextMouthLevel = Random.Range(30.0f, 50.0f);
            if (!runningCoroutine)
                if (isMouthClosed)
                {
                    bool extraLipMovement = audioOutputDataScript.CurrentSpectrumAvg >= SpectrumAvgThreshold * 2;
                    StartCoroutine(MouthMovement(nextMouthLevel, "opening", extraLipMovement));
                    isMouthClosed = false;
                }
                else
                {
                    StartCoroutine(MouthMovement(nextMouthLevel / 2, "closing"));
                    isMouthClosed = true;
                }

        }
        else
        {
            if (!runningCoroutine)
            {
                StartCoroutine(MouthMovement(0f, "closing"));
                isMouthClosed = true;

            }
        }


    }


    private IEnumerator MouthMovement(float desiredMouthLevel, string mouthAction, bool extraMovement = false)
    {
        runningCoroutine = true;
        bool action = true;
        while (action)
        {
            if (mouthAction == "opening")
            {
                if (currentMouthLevel < desiredMouthLevel)
                {
                    //if the current mouthlevel is leess than the one we want, increase it, hence open the mouth until it reaches the level desired
                    if (printDebug) Debug.Log("the current action is: " + mouthAction);

                    skinnedMeshRenderer.SetBlendShapeWeight(mouthOpenBlendshapesIndex, currentMouthLevel);

                    float increaseRate = 1.4f;
                    //bool extraMovement = Random.Range(0f, 1f) > 0.5f;
                    if (extraMovement)
                    {
                        skinnedMeshRenderer.SetBlendShapeWeight(jawDownBlendshapesIndex, currentMouthLevel / 2);
                        skinnedMeshRenderer.SetBlendShapeWeight(midMouthLeftBlendshapesIndex, currentMouthLevel * increaseRate);
                        skinnedMeshRenderer.SetBlendShapeWeight(midMouthRightBlendshapesIndex, currentMouthLevel * increaseRate);
                    }


                    currentMouthLevel += blendSpeed; //increase the weight                     
                    yield return new WaitForSeconds(.01f);

                }
                else
                {
                    action = false; // the currentMouthLevel is at the desired level

                }
            }
            else if (mouthAction == "closing")
            {

                if (currentMouthLevel > desiredMouthLevel)
                {
                    //if the current mouthlevel is leess than the one we want, increase it, hence open the mouth until it reaches the level desired
                    if (printDebug) Debug.Log("the current action is: " + mouthAction);


                    skinnedMeshRenderer.SetBlendShapeWeight(mouthOpenBlendshapesIndex, currentMouthLevel);
                    skinnedMeshRenderer.SetBlendShapeWeight(jawDownBlendshapesIndex, 0);
                    skinnedMeshRenderer.SetBlendShapeWeight(midMouthLeftBlendshapesIndex, 0);
                    skinnedMeshRenderer.SetBlendShapeWeight(midMouthRightBlendshapesIndex, 0);



                    currentMouthLevel -= blendSpeed; //decrese the weight                     
                    yield return new WaitForSeconds(.01f);

                }
                else
                {
                    action = false; // the currentMouthLevel is at the desired level
                    yield return new WaitForSeconds(.05f);

                }

            }
            else
                //yield return null;
                action = false;

            if (printDebug) Debug.Log("the end of while, currentMouthLevel is: " + currentMouthLevel + "desired level: " + desiredMouthLevel);

        }
        if (printDebug) Debug.Log("ending now ");
        runningCoroutine = false;

        yield return new WaitForSeconds(.05f);

    }
}