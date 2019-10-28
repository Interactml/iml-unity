using UnityEngine;

namespace MalbersAnimations.Utilities
{
    /// <summary>
    /// Adding Coments on the the Animator.
    /// </summary>
    public class CommentB : StateMachineBehaviour
    {
        [Multiline] public string text;
    }
}