using UnityEngine;
using System.Collections;


namespace MalbersAnimations
{
    
    public class StepTrigger : MonoBehaviour
    {
        StepsManager _StepsManager;
        //Collider col;
        public float WaitNextStep = 0.2f;

        [HideInInspector]
        public AudioSource StepAudio;

        //[HideInInspector]
        //public Vector3 contactPoint;

        WaitForSeconds wait;

        bool hastrack;                      // Check if already  has a track... don't put another
        bool waitrack;                      // Check if is time to put a track; 
        public bool HasTrack
        {
            get { return hastrack; }
            set { hastrack = value; }
        }

        void Awake()
        {
            _StepsManager = GetComponentInParent<StepsManager>();
            //col = GetComponent<Collider>();

            if (_StepsManager == null) //If there's no  StepManager Remove the Stepss
            {
                Destroy(gameObject);
                return;
            }
            if (_StepsManager.Active == false) //If there's no  StepManager Remove the Stepss
            {
                gameObject.SetActive(false);
                return;
            }

            StepAudio = GetComponent<AudioSource>();

            if (StepAudio == null)
            {
                StepAudio = gameObject.AddComponent<AudioSource>();
            }

            StepAudio.spatialBlend = 1;  //Make the Sound 3D
            if (_StepsManager) StepAudio.volume = _StepsManager.StepsVolume;

            wait = new WaitForSeconds(WaitNextStep);

           
        }


        void OnTriggerEnter(Collider other)
        {
            if (!waitrack && _StepsManager)             //
            {
                 StartCoroutine(WaitForStep());     //Wait Half a Second before making another Step

                //contactPoint = other.ClosestPointOnBounds(transform.position);

                _StepsManager.EnterStep(this);
                hastrack = true;
                
            }
        }

        void OnTriggerExit(Collider other)
        {
            hastrack = false; // if the feet is on the air then can put a track
        }

        IEnumerator WaitForStep()
        {
            waitrack =  true;
            yield return wait;
            waitrack = false;
        }
    }
}