using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// This is for using the left and right key for acivating the Dodge animations
    /// </summary>
    public class DodgeKeys : MonoBehaviour
    {
        Animal animal;
        public float DoubleKeyTime = 0.3f;
        bool DodgePressOne;

        void Start()
        {
            animal = GetComponent<Animal>();
            animal.OnMovementReleased.AddListener(OnMovementReleased);
        }

        void OnMovementReleased(bool released)
        {
            if (!released)
            {
                if (animal.Direction != 0 && !DodgePressOne) //is the Right Key
                {
                    DodgePressOne = true;
                    Invoke("ResetDodgeKeys", DoubleKeyTime);
                }
                else if (animal.Direction != 0 && DodgePressOne)
                {
                    animal.Dodge = true;
                    Invoke("ResetDodgeKeys", 0.1f);
                }
            }
        }

        void ResetDodgeKeys()
        {
            DodgePressOne = false;
            animal.Dodge = false;
        }

        private void OnDisable() { animal.OnMovementReleased.RemoveListener(OnMovementReleased); }
    }
}