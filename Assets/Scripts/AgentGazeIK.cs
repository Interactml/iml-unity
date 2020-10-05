using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGazeIK : MonoBehaviour
{
    protected Animator m_animator;
    [SerializeField] private bool m_ikActive = true;

    public Transform m_lookObj = null;

    public float m_ikWeight = 1.0f;
    private float m_ikWeightCurDest = 1.0f;
    [SerializeField] private float m_ikBlendDuration = 4.5f;

    private Vector3 transitionLookAtPosition;

    //objects to look at
    //public GameObject DistanceLookAt1, DistanceLookAt2, DistanceLookAt3;
    public Transform UserHeadCamTransform;//the user
    public int lookAtProgress;
    public bool TransitionLookAt;
    public float TransitionTime;
    public float ChangedLookAtPositionTime;

    public bool IkActive
    {
        get { return m_ikActive; }
        set { m_ikActive = value; }
    }

    public void SetIK(int val)
    {
        if (m_ikWeightCurDest != val)
        {
            m_ikWeightCurDest = val;
            StartCoroutine("UpdateIkWeight", val);
        }
    }

    void Awake()
    {
        TransitionLookAt = false;
        TransitionTime = 3.0f;//time to transition the look at from one game object to another

        lookAtProgress = 1;
        m_ikWeight = 0.9f;
        m_animator = GetComponent<Animator>();

        UserHeadCamTransform = Camera.main.transform;
        // this should be the user
        m_lookObj = UserHeadCamTransform;
    }

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (m_animator)
        {
           
            //if the IK is active, set the position and rotation directly to the goal. 
            if (m_ikActive)
            {
                // Set the look target position, if one has been assigned
                if (m_lookObj != null)// && !TransitionLookAt)
                {
                    //SetIK(1);
                    m_animator.SetLookAtWeight(m_ikWeight, 0.2f, 0.7f, 1.0f);


                    //Vector3 rndOffset = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                    //m_animator.SetLookAtPosition(m_lookObj.position + rndOffset);

                    //Debug.Log("look at in AnimatorIK");

                    if (!TransitionLookAt)
                    {
                        m_animator.SetLookAtPosition(m_lookObj.position);
                        //updateObjLookAt();
                    }
                    else m_animator.SetLookAtPosition(transitionLookAtPosition);
                }
            }
        }
    }

    private void TransitionGaze(Transform fromLookTransform, Transform toLookTransform, float transitionTime = 3.0f)
    {
        TransitionLookAt = true;
        StartCoroutine(TransitionToNewPosition(fromLookTransform, toLookTransform, Time.time, transitionTime));
        m_lookObj = toLookTransform;
    }

    private IEnumerator TransitionToNewPosition(Transform previousLookAtTransform, Transform newLookAtTransform, float changedLookAtTime, float transitionTime)
    {
        //Debug.Log("inCorotine");
        float t = (Time.time - changedLookAtTime) / TransitionTime;
        while (TransitionLookAt)
        {
            transitionLookAtPosition = Vector3.Lerp(previousLookAtTransform.position, newLookAtTransform.position, t);

            yield return new WaitForSeconds(.01f);
            t = (Time.time - changedLookAtTime) / TransitionTime;
            if (t >= 1.0f) TransitionLookAt = false;
        }
        yield return null;
    }


    // Smooth transition between IK active and inactive
    private IEnumerator UpdateIkWeight(float dest)
    {
        m_ikWeight = Mathf.Abs(1 - dest);   // set IK active/inactive end state
        m_ikActive = true;                  // enable IK

        // Lerp between current and end IK states
        for (float i = 0; i < 1f; i = (i + Time.deltaTime) / m_ikBlendDuration)
        {
            m_ikWeight = Mathf.Lerp(m_ikWeight, dest, i);
            yield return null;
        }

        // If the end IK state is inactive, disable IK updating weight and position for animator
        if (dest == 0)
            m_ikActive = false;
    }

    //have the agent looking at the user
    private void updateObjLookAt()
    {
        if (m_lookObj != UserHeadCamTransform)
            TransitionGaze(m_lookObj.transform, UserHeadCamTransform);
    }

    public void updateObjLookAt(Transform newLookAtObject)
    {
        if (m_lookObj != newLookAtObject.transform)
            TransitionGaze(m_lookObj.transform, newLookAtObject.transform);

    }

}
