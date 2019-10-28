using UnityEngine;

namespace MalbersAnimations.Utilities
{
    public class BlinkEyes : MonoBehaviour, IAnimatorListener
    {
        public Animator animator;
        public string parameter;

        /// <summary>
        /// This method is called by animation clips events, this will open an close the animal eyes
        /// </summary>
        public virtual void Eyes(int ID)
        {
          if (animator)  animator.SetInteger(parameter, ID);
        }


        public virtual void OnAnimatorBehaviourMessage(string message, object value)
        {
            this.InvokeWithParams(message, value);
        }
    }
}
