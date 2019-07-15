using UnityEngine;

namespace MalbersAnimations
{
    [CreateAssetMenu(menuName = "Malbers Animations/Actions")]
    public class Action : ScriptableObject
    {
        [Tooltip("Value for the transitions IDAction on the Animator in order to Execute the desirable animation clip")]
        public int ID;

        public static implicit operator int(Action reference)
        {
            return reference.ID;
        }
    }
}