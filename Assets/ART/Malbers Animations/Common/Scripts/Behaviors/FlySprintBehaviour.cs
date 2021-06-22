using UnityEngine;


namespace MalbersAnimations
{
    public class FlySprintBehaviour : StateMachineBehaviour
    {

        public bool IsRootMotion = false;
        [Tooltip("Float Parameter on the Animator to Modify When Sprint is Enabled, if this value is null it will not change the multiplier")]
        public string Speed_Param = "SpeedMultiplier";

        public float ShiftMultiplier = 2f;

        public float AnimSpeedDefault = 1f;
        [Tooltip("Amount of Speed Multiplier  to use on the Speed Multiplier Animator parameter when 'UseSprint' is Enabled\n if this value is null it will not change the multiplier")]
        public float AnimSprintSpeed = 2f;
        [Tooltip("Smoothness to use when the SpeedMultiplier changes")]
        public float AnimSprintLerp = 2f;

        [Tooltip("Do not Glide while pressing shift")]
        public bool NoGliding = true;

        protected int SpeedHash = Animator.StringToHash("SpeedMultiplier");
        protected float CurrentSpeedMultiplier;

        protected float Shift;
        protected Animal animal;
        protected Speeds BehaviourSpeed;


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponent<Animal>();
            BehaviourSpeed = animal.flySpeed;

            Shift = 0;

            if (Speed_Param != string.Empty)
            {
                SpeedHash = Animator.StringToHash(Speed_Param);
                animator.SetFloat(SpeedHash, AnimSpeedDefault);
            }

            CurrentSpeedMultiplier = AnimSpeedDefault;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animal.MovementReleased ) return;  //if the Dragon is not moving forward do nothing

            var deltaTime = Time.deltaTime;

            Shift = Mathf.Lerp(Shift, animal.Shift ? ShiftMultiplier : 1, BehaviourSpeed.lerpPosition * deltaTime);   //Calculate the Shift
            CurrentSpeedMultiplier = Mathf.Lerp(CurrentSpeedMultiplier,
                (animal.Shift && animal.MovementForward > 0)
                ? AnimSprintSpeed : AnimSpeedDefault, deltaTime * AnimSprintLerp);

            if (Speed_Param != string.Empty) animal.Anim.SetFloat(SpeedHash, CurrentSpeedMultiplier);

            if (IsRootMotion)
            {
                animal.DeltaPosition += animator.velocity * Shift * deltaTime;
              
            }
            else
            {
                animal.DeltaPosition += animal.T_Forward * Shift * Mathf.Clamp(animal.Speed, 0, 1) * deltaTime;
            }
            if (animal.Shift && NoGliding) animal.Speed = Mathf.Lerp(animal.Speed, 1, deltaTime * 6f);
        }
    }
}
