using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractML;
using TMPro;

public class IMLControls : MonoBehaviour
{
    [PullFromIMLController]
    public int direction;

    //strings for description of movement
    public string[] directions;
    // agent animator
    public Animator agentAnimator;
    // avatar audio and success sounds 
    public AudioSource agentAudio;
    public AudioClip successClip;
    public AudioClip failClip;
    public AudioClip feedbackSound;
    public AgentGazeIK ikController;

    private int randomDirection;
    public int animationSuccess = 0;

    public TextMeshProUGUI directionsText;
    public TextMeshProUGUI timer;

    public bool success = false;

    private int score = 0;

    float timeRemaining = 30;

    // Start is called before the first frame update
    void Start()
    {
        agentAudio.clip = feedbackSound;
    }

    // Update is called once per frame
    void Update()
    {

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            if (randomDirection == direction)
            {
                agentAudio.Play();
                animationSuccess = 0;
                //success = true;
                score++;
                randomDirection = Random.Range(0, directions.Length - 1);
                directionsText.text = directions[randomDirection];
                if (directions[randomDirection].Contains("right"))
                {
                    ikController.updateObjLookAt(ikController.right.transform);
                } else if (directions[randomDirection].Contains("left"))
                {
                    ikController.updateObjLookAt(ikController.left.transform);
                }
                else
                {
                    ikController.updateObjLookAt(ikController.head.transform);
                }
            }
            else
            {
                animationSuccess++;

            }
            //agentAnimator.SetInteger("Understands", animationSuccess);
            timer.text = timeRemaining.ToString();
        } else
        {
            //agentAnimator.SetInteger("Understands", -1);
            directionsText.text = "Score: " + score;
            if (score <= 7)
            {
                agentAudio.clip = failClip;
            } else
            {
                agentAudio.clip = successClip;
            }

            agentAudio.Play();

        }

    }
}
