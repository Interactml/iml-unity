using System.Collections;
using UnityEngine;

namespace ReusableMethods
{
    /// <summary>
    /// Extended timer class that allow to record at a specific rate
    /// </summary>
    public class TimerRecorder : Timer
    {
        float m_TimeToStopCapture;
        float m_TimeToNextCapture;

        public void PrepareTimer (float startDelay, float recordTime)
        {
            float time = Time.unscaledTime;
            m_TimeToNextCapture = time + startDelay;
            if (recordTime > 0)
            {
                m_TimeToStopCapture = time + startDelay + recordTime;
            }
            else
            {
                m_TimeToStopCapture = -1;
            }

        }

        public bool RecorderCountdown(float secondsToWait, float captureRate)
        {
            float time = Time.unscaledTime;
            if (TimerState == TimerStateEnum.Stopped)
            {
                m_TimeToNextCapture = time + secondsToWait / captureRate;
                StartTimer();
            }

            // Countdown complete
            if (time >= m_TimeToNextCapture)
            {
                StopTimer();
                return true;
            }
            // not yet
            else
            {
                return false;
            }
        }

    }
}