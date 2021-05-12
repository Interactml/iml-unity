using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// This is the Interface to comunicate between Animators Behaviours and Component scripts
    /// </summary>
    public interface IAnimatorBehaviour
    {
        /// <summary>
        /// This is called on State Enter
        /// </summary>
        /// <param name="ID">ID is the Animator StringHash ID of the Animation Tag</param>
        /// <param name="stateInfo">Animator State info</param>
        /// <param name="layerIndex">Current Layer of where the Animator Behavior is</param>
        void OnStateEnter(int ID, AnimatorStateInfo stateInfo, int layerIndex);
        void OnStateExit(int ID, AnimatorStateInfo stateInfo, int layerIndex);
        void OnStateMove(int ID, AnimatorStateInfo stateInfo, int layerIndex);
        void OnStateUpdate(int ID, AnimatorStateInfo stateInfo, int layerIndex);
    }
}