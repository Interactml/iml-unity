using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// Enable Disable the Attack Triggers on the Animal
    /// </summary>
    public class AttackBehaviour : StateMachineBehaviour
    {
        public int AttackTrigger = 1;                           //ID of the Attack Trigger to Enable/Disable during the Attack Animation

        [Tooltip("Range on the Animation that the Attack Trigger will be Active")]
        [MinMaxRange(0, 1)]
        public RangedFloat AttackActivation =  new RangedFloat(0.3f,0.6f);

        private bool isOn, isOff;
        private Animal animal;

        private float startAttackTime;  //Time when the Attack Animation Started
        private float attackDelay;      //Duration of the Attack on the Animal (before he can attack again)


        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponent<Animal>();
            animal.IsAttacking = true;                              //Set is Attacking to true so it cannot be interrupted the current attack animation
            animal.Attack1 = false;                                 //Disable the Attack Input Parameter
            isOn = isOff = false;                                   //Reset the ON/OFF parameters (to be used on the Range of the animation
            attackDelay = animal.attackDelay;                       //Get the Attack Delay
            startAttackTime = Time.time;                            //Store the time when the Attack started
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal.IsAttacking = true;                                  //Important Make Sure its stays true!!

            if (!isOn && (stateInfo.normalizedTime % 1) >= AttackActivation.minValue)
            {
                animal.AttackTrigger(AttackTrigger);
                isOn = true;
            }

            if (!isOff && (stateInfo.normalizedTime % 1) >= AttackActivation.maxValue)
            {
                animal.AttackTrigger(0);
                isOff = true;
            }

            if (attackDelay > 0)
            {
                if (Time.time - startAttackTime >= attackDelay)
                {
                    animal.IsAttacking = false;
                }
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal.AttackTrigger(0);                //Disable all Attack Triggers
            isOn = isOff = false;                   //Reset the ON/OFF variables
            animal.IsAttacking = false;             //Make Sure it Stop attacking
        }

    }
}