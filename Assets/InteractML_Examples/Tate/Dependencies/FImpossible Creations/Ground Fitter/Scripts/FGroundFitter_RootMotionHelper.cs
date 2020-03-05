using UnityEngine;

namespace FIMSpace.GroundFitter
{
    /// <summary>
    /// FM: Class which helps ground fitter handle root motion when animator is in another transform in hierarchy
    /// </summary>
    public class FGroundFitter_RootMotionHelper : MonoBehaviour
    {
        public FGroundFitter_Movement MovementController;
        public FGroundFitter_Base_RootMotion OptionalFitter;

        private void OnAnimatorMove()
        {
            if (MovementController)
                MovementController.OnAnimatorMove();
            else
                if (OptionalFitter)
                OptionalFitter.OnAnimatorMove();
            else
                Destroy(this);
        }
    }
}