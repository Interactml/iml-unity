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

    // Agent IK controller 
    public AgentGazeIK ikController;

    //int for the random direction 
    private int randomDirection;
    //int to control animation for whether the agent understands or not
    public int animationSuccess = 0;

    // text for directions and timer 
    public TextMeshProUGUI directionsText;
    public TextMeshProUGUI timer;

    // boolean of whether the experience has finished
    bool finished = true; 

    // score of user 
    private int score = 0;
    // time allowed for the game
    public float timeGiven;
    // float to track time remaining 
    float timeRemaining = 60;

    // Start is called before the first frame update
    void Start()
    {
        agentAudio.clip = feedbackSound;
        ikController.updateObjLookAt(ikController.head.transform);
    }

    // Update is called once per frame
    void Update()
    {
        // if we haven't finished and there is time remaining
        if (timeRemaining > 0 && !finished)
        {
            // reduce time remaining
            timeRemaining -= Time.deltaTime;

            // if the randomly generated direction matches the IMLOutput 
            if (randomDirection == direction)
            {
                // if this direction contained right mke agent look right
                if (directions[randomDirection].Contains("right"))
                {
                    ikController.updateObjLookAt(ikController.right.transform);
                }
                // if this directions contained left look left
                else if (directions[randomDirection].Contains("left"))
                {
                    ikController.updateObjLookAt(ikController.left.transform);
                }
                // otherwise agent to look at user
                else
                {
                    ikController.updateObjLookAt(ikController.head.transform);
                }
                // play understanding sound
                agentAudio.Play();
                //set animation success to 0
                animationSuccess = 0;
                //increase score
                score++;
                // generate new random direction
                randomDirection = Random.Range(0, directions.Length - 1);
                // set this direction as the text for instructions
                directionsText.text = directions[randomDirection];
                
            }
            else
            {
                // if the direction has not been recognised increase animation success to 
                animationSuccess++;

            }
            // set animation with animation success
            agentAnimator.SetInteger("Understands", animationSuccess);
            // show time remaining
            timer.text = timeRemaining.ToString();
          
        }
        // if not finished and no time remaining 
        else if (timeRemaining < 0 && !finished)
        {
            // set finished to true
            finished = true;
            // set finished to true on animation
            agentAnimator.SetBool("Finished", true);
            // set understand to - 1 in animation
            agentAnimator.SetInteger("Understands", -1);
            // show score
            directionsText.text = "Score: " + score;
            // if losing score
            if (score <= 7)
            {
                // play fail clip and show they didnt understand
                agentAudio.clip = failClip;
                timer.text = "Try again!";
            } else
            {
                // play success  clip and show they understood
                timer.text = "Great explanation!";
                agentAudio.clip = successClip;
            }
            // play audio
            agentAudio.Play();

        }

    }
    /// <summary>
    /// Called from event system
    /// </summary>
    public void Restart()
    {
        // if the play has finished start new game on trigger
        if (finished)
        {
            // set finished to false in animator
            agentAnimator.SetBool("Finished", false);
            // new random direction
            randomDirection = Random.Range(1, directions.Length - 1);
            // set text
            directionsText.text = directions[randomDirection];
            // set time given 
            timeRemaining = timeGiven;
            // finished is false
            finished = false;
        }
         
        
    }
}
