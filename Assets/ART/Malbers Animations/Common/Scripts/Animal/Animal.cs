using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody))]
    /// <summary>
    /// This will controll all Animals Motion  
    /// Version 2.0
    /// </summary>
    public partial class Animal : MonoBehaviour, IAnimatorListener , IMDamagable ,IMCharacter , ICharacterMove
    {
        //This was left in blank Intentionally

        //Animal Variables: All variables
        //Animal Movement: All Locomotion Logic
        //Animal CallBacks: All public methods and behaviors thatic can be called outside the script
    }

    /// <summary>
    /// Saves the direction and the Amount of damage
    /// </summary>
    public class DamageValues
    {
        public Vector3 Direction;
        public float Amount = 0;

        public DamageValues(Vector3 dir, float amount = 0)
        {
            Direction = dir;
            Amount = amount;
        }
    }

    [System.Serializable]
    public struct Speeds
    {
        public string name;
        /// <summary>
        /// Add additional speed to the transfrom
        /// </summary>
        public float position;

        /// <summary>
        /// Changes the Animator Speed
        /// </summary>
        public float animator;

        /// <summary>
        /// Smoothness to change to the Transform speed, higher value more Responsiveness
        /// </summary>
        public float lerpPosition;

        /// <summary>
        /// Smoothness to change to the Animator speed, higher value more Responsiveness
        /// </summary>
        public float lerpAnimator;

        /// <summary>
        /// Add Aditional Rotation to the Speed
        /// </summary>
        public float rotation;

        public float lerpRotation;

        public Speeds(int defaultt)
        {
            position = 0;
            animator = 1;
            lerpPosition = 2f;
            lerpAnimator = 2f;
            rotation = 0;
            lerpRotation = 2f;
            name = string.Empty;
        }

        public Speeds(float lerpPos, float lerpanim, float lerpTurn)
        {
            position = 0;
            animator = 1;
            rotation = 0;
            lerpPosition = lerpPos;
            lerpAnimator = lerpanim;
            lerpRotation = lerpTurn;
            name = string.Empty;
        }
    }

    [System.Serializable]
    public class AnimalEvent : UnityEvent<Animal> { }
}

