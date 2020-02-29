using UnityEngine;

/// <summary>
/// FM: Class containing few handy easing functions
/// </summary>
public static class FEasing
{
    #region Example of how to use easings

    /// <summary>
    /// Example how to use easing function
    /// </summary>
    //static System.Collections.IEnumerator Example()
    //{
    //    float timeInSecs = 3f;
    //    float elapsed = 0f;

    //    Vector3 from = new Vector3(-1f, 2f, 1f);
    //    Vector3 to = new Vector3(1f, 0f, 3f);

    //    // Simple example with hard defined easing function
    //    while (elapsed < timeInSecs)
    //    {
    //        float easeValue = EaseInOutCubic(0f, 1f, elapsed / timeInSecs);

    //        Vector3 easedTargetVector = Vector3.Lerp(from, to, easeValue);

    //        yield return null;
    //    }

    //    // Example with easing function to choose

    //    EFease easeType = EFease.EaseInOutElastic;

    //    float extraValue = 1.1f;

    //    while (elapsed < timeInSecs)
    //    {
    //        float easeValue = GetEasingFunction(easeType)(0f, 1f, elapsed / timeInSecs, extraValue);

    //        Vector3 easedTargetVector = Vector3.Lerp(from, to, easeValue);

    //        yield return null;
    //    }

    //}

    #endregion

    public enum EFease
    {
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,

        EaseInOutElastic,
        EaseInElastic,
        EaseOutElastic,

        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,

        Linear,
    }

    #region Easing Methods

    /// <summary>
    /// Cubic smooth ease, ignore argument is for delegate to work with extra arguments
    /// </summary>
    public static float EaseInCubic(float start, float end, float value, float ignore = 1f)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static float EaseOutCubic(float start, float end, float value, float ignore = 1f)
    {
        value -= 1;
        end -= start;
        return end * (value * value * value + 1) + start;
    }


    public static float EaseInOutCubic(float start, float end, float value, float ignore = 1f)
    {
        value /= .5f;
        end -= start;

        if (value < 1) return end * 0.5f * value * value * value + start;

        value -= 2;

        return end * 0.5f * (value * value * value + 2) + start;
    }


    public static float EaseOutElastic(float start, float end, float value, float rangeMul = 1f)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f * rangeMul;
        float s;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p * 0.25f * rangeMul;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return (a * Mathf.Pow(2, -10 * value * rangeMul) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
    }


    public static float EaseInElastic(float start, float end, float value, float rangeMul = 1f)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f * rangeMul;
        float s;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = (p / 4) * rangeMul;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return -(a * Mathf.Pow(2, 10 * rangeMul * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
    }


    public static float EaseInOutElastic(float start, float end, float value, float rangeMul = 1f)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f * rangeMul;
        float s;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d * 0.5f) == 2) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4 * rangeMul;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        return a * Mathf.Pow(2, -10 * rangeMul * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
    }


    public static float EaseInExpo(float start, float end, float value, float ignore = 1f)
    {
        end -= start;
        return end * Mathf.Pow(2, 10 * (value - 1)) + start;
    }

    public static float EaseOutExpo(float start, float end, float value, float ignore = 1f)
    {
        end -= start;
        return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
    }

    public static float EaseInOutExpo(float start, float end, float value, float ignore = 1f)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
        value--;
        return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
    }

    public static float Linear(float start, float end, float value, float ignore = 1f)
    {
        return Mathf.Lerp(start, end, value);
    }

    #endregion

    public delegate float Function(float s, float e, float v, float extraParameter = 1f);

    public static Function GetEasingFunction(EFease easingFunction)
    {
        if (easingFunction == EFease.EaseInCubic) return EaseInCubic;
        if (easingFunction == EFease.EaseOutCubic) return EaseOutCubic;
        if (easingFunction == EFease.EaseInOutCubic) return EaseInOutCubic;

        if (easingFunction == EFease.EaseInElastic) return EaseInElastic;
        if (easingFunction == EFease.EaseOutElastic) return EaseOutElastic;
        if (easingFunction == EFease.EaseInOutElastic) return EaseInOutElastic;

        if (easingFunction == EFease.EaseInExpo) return EaseInExpo;
        if (easingFunction == EFease.EaseOutExpo) return EaseOutExpo;
        if (easingFunction == EFease.EaseInOutExpo) return EaseInOutExpo;

        if (easingFunction == EFease.Linear) return Linear;

        return null;
    }
}

