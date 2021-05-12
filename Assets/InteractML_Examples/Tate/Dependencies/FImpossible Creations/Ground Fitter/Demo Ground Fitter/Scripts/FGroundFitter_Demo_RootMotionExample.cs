using UnityEngine;

namespace FIMSpace.GroundFitter
{
    public class FGroundFitter_Demo_RootMotionExample : FGroundFitter_Movement
    {
        protected override void Start()
        {
            base.Start();

            clips.AddClip("RotateL");
            clips.AddClip("RotateR");
        }

        protected override void HandleAnimations()
        {
            if (Input.GetKey(KeyCode.A))
            {
                CrossfadeTo("RotateL", 0.25f);
                MoveVector = Vector3.zero;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                CrossfadeTo("RotateR", 0.25f);
                MoveVector = Vector3.zero;
            }
            else
            {
                if (ActiveSpeed > 0.15f)
                {
                    if (Sprint)
                        CrossfadeTo("Run", 0.25f);
                    else
                        CrossfadeTo("Walk", 0.25f);
                }
                else
                {
                    CrossfadeTo("Idle", 0.25f);
                }
            }

            // If object is in air we just slowing animation speed to zero
            if (animatorHaveAnimationSpeedProp)
                if (inAir) FAnimatorMethods.LerpFloatValue(animator, "AnimationSpeed", 0f);
                else
                    FAnimatorMethods.LerpFloatValue(animator, "AnimationSpeed", MultiplySprintAnimation ? (ActiveSpeed / BaseSpeed) : Mathf.Min(1f, (ActiveSpeed / BaseSpeed)));
        }
    }
}