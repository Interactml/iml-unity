using UnityEngine;

/// <summary>
/// FMoeglich: Class with methods which can be helpful when using unity Animator class
/// </summary>
public static class FAnimatorMethods
{
    /// <summary>
    /// Sets animator's float value with lerp
    /// </summary>
    public static void LerpFloatValue(Animator animator, string name = "RunWalk", float value = 0f, float deltaSpeed = 8f)
    {
        float newValue = animator.GetFloat(name);
        newValue = FLogicMethods.FLerp(newValue, value, Time.deltaTime * deltaSpeed);
        animator.SetFloat(name, newValue);
    }

    /// <summary>
    /// Function called to detect if no-looped animation finish
    /// </summary>
    public static bool CheckAnimationEnd(Animator animator, int layer = 0, bool reverse = false, bool checkAnimLoop = true)
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layer);

        if (!animator.IsInTransition(layer))
        {
            if (checkAnimLoop)
            {
                if (info.loop == false)
                {
                    if (!reverse)
                        if (info.normalizedTime > 0.98f) return true;
                        else
                        if (info.normalizedTime < 0.02f) return true;
                }
            }
            else /* Same operation as above but without checking if animation is looped in the source */
            {
                if (!reverse)
                    if (info.normalizedTime > 0.98f) return true;
                    else
                    if (info.normalizedTime < 0.02f) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Resetting all additional layers' weights to zero (lerp but reaching value)
    /// </summary> 
    public static void ResetLayersWeights(Animator animator, float speed = 10f)
    {
        for (int i = 1; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i, FLogicMethods.FLerp(animator.GetLayerWeight(i), 0f, Time.deltaTime * speed));
        }
    }

    /// <summary>
    /// Transitioning value of animator layer's weight to target with smooth effect
    /// </summary>
    public static void LerpLayerWeight(Animator animator, int layer = 0, float newValue = 1f, float speed = 8f)
    {
        float newWeight = animator.GetLayerWeight(layer);
        newWeight = FLogicMethods.FLerp(newWeight, newValue, Time.deltaTime * speed);
        animator.SetLayerWeight(layer, newWeight);
    }

    /// <summary>
    /// Returning true if state exist
    /// </summary>
    public static bool StateExists(Animator animator, string clipName, int layer = 0)
    {
        int animHash = Animator.StringToHash(clipName);
        return animator.HasState(layer, animHash);
    }
}
