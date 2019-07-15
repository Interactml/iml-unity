using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// Allows the Animal to AutoGlide using the Fly Blendtree when Vertical:1 is Fly and Vertical:2 is Glide
    /// </summary>
    public class AutoGlideBehaviour : StateMachineBehaviour
    {
        [MinMaxRange(0, 10)]
        public RangedFloat GlideChance = new RangedFloat(0.8f,4);
        [MinMaxRange(0, 10)]
        public RangedFloat FlapChange = new RangedFloat(0.5f,4);

        protected bool isGliding = false;
        protected float FlyStyleTime = 1;
        protected float currentTime = 1;
        protected bool Default_UseShift;
        protected Animal animal;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponent<Animal>();

            Default_UseShift = animal.UseShift;
            animal.UseShift = false;

            FlyStyleTime = GlideChance.RandomValue;
            currentTime = Time.time;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!animal.Fly) return;
            if (Time.time - FlyStyleTime >= currentTime)
            {
                currentTime = Time.time;
                isGliding = !isGliding;

                FlyStyleTime = isGliding ? GlideChance.RandomValue : FlapChange.RandomValue;

                animal.GroundSpeed = isGliding ? 2 : Random.Range(1f, 1.5f);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal.UseShift = Default_UseShift;
        }
    }



    public class FlySprintBehaviour1 : StateMachineBehaviour
    {
        public bool UseSprint = true;
        [Tooltip("Float Parameter on the Animator to Modify When Sprint is Enabled, if this value is null it will not change the multiplier")]
        public string Speed_Param = "SpeedMultiplier";

        public float ShiftMultiplier = 2f;

        public float AnimSpeedDefault = 1f;
        [Tooltip("Amount of Speed Multiplier  to use on the Speed Multiplier Animator parameter when 'UseSprint' is Enabled\n if this value is null it will not change the multiplier")]
        public float AnimSprintSpeed = 2f;
        [Tooltip("Smoothness to use when the SpeedMultiplier changes")]
        public float AnimSprintLerp = 2f;

        protected int SpeedHash = Animator.StringToHash("SpeedMultiplier");
        protected float CurrentSpeedMultiplier;

        protected float Shift;
        protected Animal animal;
        protected Speeds BehaviourSpeed;


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponent<Animal>();
            BehaviourSpeed = animal.flySpeed;

            if (Speed_Param != string.Empty)
            {
                SpeedHash = Animator.StringToHash(Speed_Param);
                animator.SetFloat(SpeedHash, AnimSpeedDefault);
            }

            CurrentSpeedMultiplier = AnimSpeedDefault;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var deltaTime = Time.deltaTime;

            Shift = Mathf.Lerp(Shift, animal.Shift ? ShiftMultiplier : 1, BehaviourSpeed.lerpPosition * deltaTime);   //Calculate the Shift
            CurrentSpeedMultiplier = Mathf.Lerp(CurrentSpeedMultiplier, animal.Shift ? AnimSprintSpeed : AnimSpeedDefault, deltaTime * AnimSprintLerp);
            if (Speed_Param != string.Empty) animal.Anim.SetFloat(SpeedHash, CurrentSpeedMultiplier);

            animal.DeltaPosition += animal.T_Forward * Shift * deltaTime;
        }
    }
}