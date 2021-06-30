using UnityEngine;

public class RootMotionBehaviour : StateMachineBehaviour {

    public bool OnEnter;
    public bool RootMotionOnEnter;
    [Space]
    public bool OnExit;
    public bool RootMotionOnExit;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (OnEnter)
        {
            animator.applyRootMotion = RootMotionOnEnter;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (OnExit)
        {
            animator.applyRootMotion = RootMotionOnExit;
        }
    }
}
