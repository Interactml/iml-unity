using UnityEngine;
using System.Collections;

public class SimpleBlinks : MonoBehaviour
{

    SkinnedMeshRenderer eyeLidMeshRenderer;
    SkinnedMeshRenderer eyeLashMeshRenderer;
    float blinkLevel = 0.0f;
    float blendSpeed = 20.0f;
    bool printDebug = false;

    public string CurrentBlinkState;

    void Awake()
    {
        eyeLidMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        eyeLashMeshRenderer = GameObject.Find("Eyelashes").GetComponent<SkinnedMeshRenderer>();

        if (blinkLevel < 100f) CurrentBlinkState = "closing"; //the eyes are open, so the state is to close them;
        else CurrentBlinkState = "opening"; //this is only when the blinklevel is 100, but it starts as 0, when the eyes are fully open
    }

    void Start()
    {
        if (printDebug) Debug.Log("blink level: " + blinkLevel);

        StartCoroutine(Blink());
    }


    private IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (printDebug) Debug.Log("Coroutine took: " + waitTime + " seconds");

        CurrentBlinkState = "closing";
        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        if (printDebug) Debug.Log("starting Blink");

        bool action = false;
        while (CurrentBlinkState != "waiting")
        {
            //frist 'closing', then 'opening', than 'waiting'; the while ends at 'waiting' state
            //yield return new WaitForSeconds(5); //...as soon as 1-8 seconds have passed
            //Debug.Log("blink: " + blinkLevel);

            if (CurrentBlinkState == "closing")
            {
                if (blinkLevel < 100.0f)
                {
                    //that mneans that the blink level is less than 100; close the eyes (incrase the blink level) until the blink level reaches 100, when the eyes are fully closed.
                    //Debug.Log("eyes are closing with: " + blendSpeed + " speed, the curent blink level is: " + blinkLevel + ", and the current state is: " + CurrentBlinkState);
                    if (printDebug) Debug.Log("the current state is: " + CurrentBlinkState);

                    //blendSpeed = 10.0f;
                    updateEyeBlink(blinkLevel);
                    blinkLevel += blendSpeed; //...increase weight
                    //new WaitForSeconds(1f);
                    yield return new WaitForSeconds(.05f);

                }
                else
                    CurrentBlinkState = "opening";

                action = true;
            }

            if (CurrentBlinkState == "opening")
            {
                if (blinkLevel > 0.0f)
                {
                    //Debug.Log("eyes are opening with: " + blendSpeed + " speed, the curent blink level is: " + blinkLevel + ", and the current state is: " + CurrentBlinkState);
                    if (printDebug) Debug.Log("the current state is: " + CurrentBlinkState);

                    //blendSpeed = 10.0f;
                    blinkLevel -= blendSpeed; //...decrease weight
                    updateEyeBlink(blinkLevel);
                    //new WaitForSeconds(1f);
                    yield return new WaitForSeconds(.05f);
                }
                else
                    CurrentBlinkState = "waiting"; //the eyes are fully open now so the blink cycle has ended; there is a waiting time now until the enxt blink cycle

                action = true;
            }


            if (CurrentBlinkState == "waiting")
            {
                action = true;
                //Debug.Log("eyes are open and we wait the curent blink level is: " + blinkLevel + ", and the current state is: " + CurrentBlinkState);
                if (printDebug) Debug.Log("the current state is: " + CurrentBlinkState);

                //new WaitForSeconds(5f);
                yield return new WaitForSeconds(.05f);
                //CurrentBlinkState = "closing";
            }

            if (!action)
            {
                if (printDebug) Debug.Log("no action performed");
                yield return null;
            }
            action = false;
        }
        if (printDebug) Debug.Log("returning");
        StartCoroutine(Wait(Random.Range(2.0f, 6.0f)));
        yield return new WaitForSeconds(.01f); //...as soon as 1-8 seconds have passed
        //CurrentBlinkState = "closing";
    }

    void updateEyeBlink(float blinkValue)
    {   //update the eyelids
        eyeLidMeshRenderer.SetBlendShapeWeight(0, blinkValue);
        eyeLidMeshRenderer.SetBlendShapeWeight(1, blinkValue);
        //update the eyelashes
        eyeLashMeshRenderer.SetBlendShapeWeight(0, blinkValue);
        eyeLashMeshRenderer.SetBlendShapeWeight(1, blinkValue);
    }
}