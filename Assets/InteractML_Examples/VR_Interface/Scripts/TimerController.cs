/* 
Author: Carlos González Díaz
*/
using UnityEngine;
using System.Collections;
using System;
using ReusableMethods;

/// <summary>
/// This component is taking charge of managing the timers of each object
/// </summary>
[AddComponentMenu("CarlosFramework/TimerController")]
public class TimerController : MonoBehaviour
{

    /// <summary>
    /// (Field) The label to identify this instance
    /// </summary>
    [SerializeField]
    private string m_ObjectLabel;
    /// <summary>
    /// (Property) The label to identify this instance
    /// </summary>
    public string ObjectLabel { get { return this.m_ObjectLabel; } set { this.m_ObjectLabel = value; } }

    [SerializeField]
    private float timeToCompare;
    /// <summary>
    /// (Property) The time that we will compare with the actual time, to know how much we need to wait
    /// </summary>
    public float TimeToCompare { get { return this.timeToCompare; } set { this.timeToCompare = value; } }

    /// <summary>
    /// The enum that defines the different states of the timer
    /// </summary>
    public enum TimerStateEnum
    {
        Started,
        Paused,
        Stopped
    }
    [SerializeField]
    private TimerStateEnum timerState;
    /// <summary>
    /// (Property) The actual state of the Timer
    /// </summary>
    public TimerStateEnum TimerState { get { return this.timerState; } set { this.timerState = value; } }

    [SerializeField]
    private float normalizedTimer;
    /// <summary>
    /// (Property) The normalized value of the current timer
    /// </summary>
    public float NormalizedTimer { get { return this.normalizedTimer; } set { this.normalizedTimer = value; } }

    private float minToNormalize;
    /// <summary>
    /// (Property) The minimum in the normalization of the time
    /// </summary>
    private float MinToNormalize { get { return this.minToNormalize; } set { this.minToNormalize = value; } }

    private float maxToNormalize;
    /// <summary>
    /// (Property) The maximum in the normalization of the time
    /// </summary>
    private float MaxToNormalize { get { return this.maxToNormalize; } set { this.maxToNormalize = value; } }

    /// <summary>
    /// Seconds left for the timer to end
    /// </summary>
    [SerializeField]
    private float m_SecondsLeft;
    /// <summary>
    /// Gets the seconds left
    /// </summary>
    /// <value>The seconds left.</value>
    public float SecondsLeft { get { return this.m_SecondsLeft; } }

    /// <summary>
    /// The amount of seconds the timer has been paused, to add that offset to the timer
    /// </summary>
    [SerializeField]
    private float m_OffsetToAddWhenTimerResumes;

    // Use this for initialization
    void Start()
    {
        // When the component loads, we set the first state to stopped
        StopTimer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// The Generic Countdown to be used. True if the countdown finishes. False while not
    /// </summary>
    /// <param name="secondsToWait"> The number of seconds to wait to get true (float) </param>
    /// <returns> True if the countdown finishes. False while not </returns>
    public bool GenericCountDown(float secondsToWait)
    {
        // We check the state of the timer
        switch (TimerState)
        {
            case TimerStateEnum.Started:
                // If it is already running, we check the time

                // We calculate the actual normalized time left
                NormalizedTimer = Normalization.Normalize(Time.time, MinToNormalize, MaxToNormalize);

                // We calculate the seconds left
                m_SecondsLeft = TimeToCompare - Time.time;

                // If the actual time is bigger than the time + secondsToWait, then the countdown is over
                if (Time.time > TimeToCompare)
                {
                    // We stop the timer
                    StopTimer();
                    // Countdown finished!
                    return true;
                }

                // The countdown is still going on, the timer keeps running
                return false;
            case TimerStateEnum.Paused:
                // Gather offset to apply when we resume
                m_OffsetToAddWhenTimerResumes += 1f * Time.deltaTime;

                // The timer is paused, return false
                return false;
            case TimerStateEnum.Stopped:
                // We execute this if the timer is stopped, to make it run for the first time
                ResetTimer(secondsToWait);

                // We start the timer
                StartTimer();
                return false;
            default:
                return false;
        }

    }

    /// <summary>
    /// The function to start the timer
    /// </summary>
    private void StartTimer()
    {
        if (TimerState != TimerStateEnum.Started)
        {
            // We update the state of the timer to start
            TimerState = TimerStateEnum.Started;
        }

        //Debug.Log("Timer started!");
    }

    /// <summary>
    /// The function to completely stop the timer. It will allow to reset the timer next time GenericCountdown is called
    /// </summary>
    public void StopTimer()
    {
        if (TimerState != TimerStateEnum.Stopped)
        {
            // We update the state of the timer to stop
            TimerState = TimerStateEnum.Stopped;
        }

        //Debug.Log("Timer stopped!");
    }

    /// <summary>
    /// The value to pause the countdown (not implemented)
    /// </summary>
    public void PauseTimer()
    {
        // We only pause the time if it is started
        if (TimerState == TimerStateEnum.Started)
        {
            // Update the timer state to paused
            TimerState = TimerStateEnum.Paused;

            // We reset the offset 
            m_OffsetToAddWhenTimerResumes = 0;
        }
    }

    /// <summary>
    /// The method to resume the timer (not implemented)
    /// </summary>
    public void ResumeTimer()
    {
        // We run the logic only if the timer is paused...
        if (TimerState == TimerStateEnum.Paused)
        {

            // Add offset to our timeToCompare
            timeToCompare += m_OffsetToAddWhenTimerResumes;

            // Update MaxToNormalize
            MaxToNormalize = timeToCompare;

            // Upate state
            TimerState = TimerStateEnum.Started;

        }
    }

    /// <summary>
    /// Resets the timer to the seconds passed in
    /// </summary>
    /// <param name="secondsToWait"> The amount of seconds to wait</param>
    public void ResetTimer(float secondsToWait)
    {
        // We execute this if the timer is stopped, to make it run for the first time
        // We update the TimeToCompare so that we can compare it with the actual time
        TimeToCompare = Time.time + secondsToWait;

        // We set the min and max for the normalization
        MinToNormalize = Time.time;
        MaxToNormalize = TimeToCompare;

        // We calculate the actual normalized time left
        NormalizedTimer = Normalization.Normalize(Time.time, MinToNormalize, MaxToNormalize);

        // We reset the offset as well to avoid having any weird values
        m_OffsetToAddWhenTimerResumes = 0f;
    }


}