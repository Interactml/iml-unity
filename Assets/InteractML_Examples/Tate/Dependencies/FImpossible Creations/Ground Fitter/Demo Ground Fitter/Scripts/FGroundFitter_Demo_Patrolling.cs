using FIMSpace.Basics;
using UnityEngine;

namespace FIMSpace.GroundFitter
{
    /// <summary>
    /// FM: Example demo component to controll spiders' movement
    /// </summary>
    public class FGroundFitter_Demo_Patrolling : MonoBehaviour
    {
        public Vector4 MovementRandomPointRange = new Vector4(25, -25, 25, -25);
        public float speed = 1f;

        private Transform bodyTransform;
        private float bodyRotateSpeed = 5f;

        private Animator animator;
        private FGroundFitter fitter;
        private float timer;
        private Vector3 targetPoint;
        private bool onDestination;

        private FAnimationClips clips;

        void Start()
        {
            fitter = GetComponent<FGroundFitter>();
            animator = GetComponentInChildren<Animator>();
            timer = Random.Range(1f, 5f);

            if (name.Contains("Fpider")) bodyTransform = transform.GetChild(0).Find("BSkeleton").GetChild(0).Find("Body_Shield");

            transform.rotation = Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f);
            fitter.UpAxisRotation = transform.rotation.eulerAngles.y;

            onDestination = true;

            transform.localScale = Vector3.one * Random.Range(0.5f, 1f);

            clips = new FAnimationClips(animator);
            clips.AddClip("Idle");
            clips.AddClip("Move");
        }

        void Update()
        {
            if (onDestination)
            {
                timer -= Time.deltaTime;
                if (timer < 0f) ChooseNewDestination();

                bodyRotateSpeed = Mathf.Lerp(bodyRotateSpeed, 50f, Time.deltaTime * 2f);
            }
            else
            {
                if (fitter.LastRaycast.transform) transform.position = fitter.LastRaycast.point;

                transform.position += transform.forward * speed * Time.deltaTime;

                if ( Vector3.Distance(transform.position, targetPoint) < 2f )
                {
                    ReachDestination();
                }

                Quaternion targetRot = Quaternion.LookRotation(targetPoint - transform.position);
                fitter.UpAxisRotation = Mathf.LerpAngle(fitter.UpAxisRotation, targetRot.eulerAngles.y, Time.deltaTime * 7f);

                bodyRotateSpeed = Mathf.Lerp(bodyRotateSpeed, -250f, Time.deltaTime * 3f);
            }

            if (bodyTransform) bodyTransform.Rotate(0f, 0f, Time.deltaTime * bodyRotateSpeed);
        }

        private void ChooseNewDestination()
        {
            targetPoint = new Vector3(Random.Range(MovementRandomPointRange.x, MovementRandomPointRange.y), 0f, Random.Range(MovementRandomPointRange.z, MovementRandomPointRange.w));

            RaycastHit hit;
            Physics.Raycast(targetPoint + Vector3.up * 1000f, Vector3.down, out hit, Mathf.Infinity, fitter.GroundLayerMask, QueryTriggerInteraction.Ignore);

            if (hit.transform)
            {
                targetPoint = hit.point;
            }

            animator.CrossFadeInFixedTime(clips["Move"], 0.25f);

            onDestination = false;
        }

        private void ReachDestination()
        {
            timer = Random.Range(1f, 5f);
            onDestination = true;
            animator.CrossFadeInFixedTime(clips["Idle"], 0.15f);
        }
    }
}