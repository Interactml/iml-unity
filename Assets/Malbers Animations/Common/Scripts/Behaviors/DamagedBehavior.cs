using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// Calculate the Direction from where is coming the hit and plays hits respective animation.
    /// </summary>
    public class DamagedBehavior : StateMachineBehaviour
    {
        int Side = 0;
        public bool DirectionalDamage = true;

        override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            Animal animal = animator.GetComponent<Animal>();

            animal.Damaged = false;                                     // Set Damage to false so it wont get "pressed"
            animator.SetBool(Hash.Damaged, false);
            animal.StartCoroutine(animal.CDamageInterrupt());           //Damage Interrupt

            if (!DirectionalDamage) return;                             //if is not Directional Damage just skip the next code

            Vector3 hitdirection = animal.HitDirection;                 //Store the direction from where the animal was hitted
            Vector3 forward = animator.transform.forward;
                                                

            hitdirection.y = 0;
            forward.y = 0;

            float angle = Vector3.Angle(forward, hitdirection);                           //Get The angle

            bool left = Vector3.Dot(animal.T_Right, animal.HitDirection) < 0;             //Calculate which directions comes the hit Left or right


            if (left)
            {
                if (angle > 0 && angle <= 60) Side = 3;
                else if (angle > 60 && angle <= 120) Side = 2;
                else if (angle > 120 && angle <= 180) Side = 1;
            }
            else
            {
                if (angle > 0 && angle <= 60) Side = -3;
                else if (angle > 60 && angle <= 120) Side = -2;
                else if (angle > 120 && angle <= 180) Side = -1;
            }


            animator.SetInteger(Hash.IDInt, Side);      //Set the ID to the Parameter on the animator
        }
    }
}