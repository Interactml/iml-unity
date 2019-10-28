using UnityEngine;

namespace MalbersAnimations
{
    public class RigidConstraintsB : StateMachineBehaviour
    {
        public bool PosX, PosY = true, PosZ, RotX = true, RotY = true, RotZ = true;
        public bool OnEnter = true, OnExit;
        protected int Amount = 0;
        Rigidbody rb;

        bool ExitTime;

        public float OnEnterDrag = 0;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Amount = 0;
            rb = animator.GetComponent<Rigidbody>();

            if (PosX) Amount += 2;
            if (PosY) Amount += 4;
            if (PosZ) Amount += 8;
            if (RotX) Amount += 16;
            if (RotY) Amount += 32;
            if (RotZ) Amount += 64;

            if (OnEnter && rb) { rb.constraints = (RigidbodyConstraints)Amount; }

            ExitTime = false;

            rb.drag = OnEnterDrag;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!ExitTime && OnExit && stateInfo.normalizedTime > 1)
            {
                rb.constraints = (RigidbodyConstraints)Amount;
                ExitTime = true;
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (OnExit && rb) rb.constraints = (RigidbodyConstraints)Amount;
        }
    }
}