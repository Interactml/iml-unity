using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// Counts the cylces to change to the next animation
    /// </summary>
    public class SleepBehavior : StateMachineBehaviour
    {
        public bool CyclesFromController;
        public int Cycles, transitionID;

        int currentCycle;
        Animal animal;

   
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animal == null)
            {
                animal = animator.GetComponent<Animal>();
            }

            if (!animal) return;
            if (animal.GotoSleep == 0) return; //Do nothing if the goto Sleep is set to 0

            if (animator.GetCurrentAnimatorStateInfo(layerIndex).tagHash == AnimTag.Idle)      //If is in idle, start to count , to get to sleep
            {
                animal.Tired++;
                if (animal.Tired >= animal.GotoSleep)
                {
                    animal.SetIntID(transitionID);  //Get to the Sleep Mode
                    animal.Tired = 0;
                }
            }
            else
            {
                CyclesToSleep();
            }
        }

        void CyclesToSleep()
        {
            if (CyclesFromController)
            {
                Cycles = animal.GotoSleep;
                if (Cycles == 0) return;
            }
            currentCycle++;

            if (currentCycle >= Cycles)
            {
                animal.SetIntID(transitionID);
                currentCycle = 0;
            }
        }
    }
}

