using FIMSpace.Basics;
using UnityEngine;
using UnityEngine.AI;

namespace FIMSpace.GroundFitter
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class FGroundFitter_Demo_NavMesh : MonoBehaviour
    {
        public FGroundFitter_Base TargetGroundFitter;
        [Range(0.5f, 50f)]
        public float RotationSpeed = 3f;
        public float AnimationSpeedScale = 1f;

        private NavMeshAgent agent;
        private FAnimationClips animationClips;

        private bool moving;
        private bool reachedDestination;
        private Vector3 lastAgentPosition;
        private string movementClip;


        private void Reset()
        {
            TargetGroundFitter = GetComponent<FGroundFitter_Base>();

            if (TargetGroundFitter) TargetGroundFitter.GlueToGround = false;

            agent = GetComponent<NavMeshAgent>();

            if (agent)
            {
                agent.acceleration = 1000;
                agent.angularSpeed = 100;
            }
        }


        protected virtual void Start()
        {
            if (TargetGroundFitter == null) TargetGroundFitter = GetComponent<FGroundFitter_Base>();

            if (TargetGroundFitter) TargetGroundFitter.GlueToGround = false;

            agent = GetComponent<NavMeshAgent>();

            agent.Warp(transform.position);
            agent.SetDestination(transform.position);
            moving = false;
            lastAgentPosition = transform.position;
            reachedDestination = true;

            animationClips = new FAnimationClips(GetComponentInChildren<Animator>());
            animationClips.AddClip("Idle");

            if (FAnimatorMethods.StateExists(animationClips.Animator, "Move") || FAnimatorMethods.StateExists(animationClips.Animator, "move"))
                movementClip = "Move";
            else
                movementClip = "Walk";

            animationClips.AddClip(movementClip);
        }


        protected virtual void Update()
        {
            // Animation Stuff
            animationClips.SetFloat("AnimationSpeed", agent.desiredVelocity.magnitude * AnimationSpeedScale, 8f);
            IsMovingCheck();

            // Direction stuff
            Vector3 dir = (agent.nextPosition - lastAgentPosition).normalized;
            Debug.DrawRay(transform.position + Vector3.up * 0.2f, dir, Color.white);

            transform.position = agent.nextPosition;

            if (moving)
            {
                // Calculating look rotation to target point
                Vector3 targetPoint = agent.nextPosition + agent.desiredVelocity;
                float yRotation = Quaternion.LookRotation(new Vector3(targetPoint.x, 0f, targetPoint.z) - transform.position).eulerAngles.y;

                // Setting top down rotation in ground fitter component with smoothing (lerp)
                //TargetGroundFitter.RotationYAxis = yRotation;
                TargetGroundFitter.UpAxisRotation = Mathf.LerpAngle(TargetGroundFitter.UpAxisRotation, yRotation, Time.deltaTime * RotationSpeed);
            }

            lastAgentPosition = agent.nextPosition;
        }


        private bool IsMovingCheck()
        {
            bool preMov = moving;

            moving = true;

            if (!agent.pathPending)
                if (agent.remainingDistance <= agent.stoppingDistance)
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        if (!reachedDestination) OnReachDestination();
                        moving = false;
                    }

            if (preMov != moving) OnStartMoving();

            return moving;
        }


        protected virtual void OnReachDestination()
        {
            reachedDestination = true;
            animationClips.CrossFadeInFixedTime("Idle", 0.25f);
        }


        protected virtual void OnStartMoving()
        {
            reachedDestination = false;
            animationClips.CrossFadeInFixedTime(movementClip, 0.25f);
        }
    }
}