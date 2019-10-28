using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// This will manage the steps sounds and tracks for each animal, on each feet there's a Script StepTriger (Basic)
    /// </summary>
    public class StepsManager : MonoBehaviour, IAnimatorListener
    {
        [Tooltip("Enable Disable the Steps Manager")]
        public bool Active = true;
        public LayerMask GroundLayer = 1;
        public ParticleSystem Tracks;
        public ParticleSystem Dust;
        public float StepsVolume = 0.2f;
        public int DustParticles = 30;

        [Tooltip("Scale of the dust and track particles")]
        public Vector3 Scale = Vector3.one;

        public AudioClip[] clips;
        [Tooltip("Distance to Instantiate the tracks on a terrain")]
        public float trackOffset = 0.0085f;


        //Is Called by any of the "StepTrigger" Script on a feet when they collide with the ground.
        public void EnterStep(StepTrigger foot)
        {
            if (Tracks && !Tracks.gameObject.activeInHierarchy)         //If is a prefab clone it!
            {
                Tracks = Instantiate(Tracks,transform, false);
                Tracks.transform.localScale = Scale;
            }

            if (Dust && !Dust.gameObject.activeInHierarchy)
            {
                Dust = Instantiate(Dust, transform, false);             //If is a prefab clone it!
                Dust.transform.localScale = Scale;
            }

            if (!Active) return;

            RaycastHit footRay;

            if (foot.StepAudio && clips.Length > 0) //If the track has an AudioSource Component and whe have some audio to play
            {
                foot.StepAudio.clip = clips[Random.Range(0, clips.Length)];  //Set the any of the Audio Clips from the list to the Feet's AudioSource Component
                foot.StepAudio.Play();  //Play the Audio
            }

            //Track and particles
            if (!foot.HasTrack)  // If we are ready to set a new track
            {
                if (Physics.Raycast(foot.transform.position, -transform.up, out footRay, 1, GroundLayer))
                {
                    if (Tracks)
                    {

                        ParticleSystem.EmitParams ptrack = new ParticleSystem.EmitParams();
                        ptrack.rotation3D = (Quaternion.FromToRotation(-foot.transform.forward, footRay.normal) * foot.transform.rotation).eulerAngles; //Get The Rotation
                        ptrack.position = new Vector3(foot.transform.position.x, footRay.point.y + trackOffset, foot.transform.position.z); //Get The Position
                        Tracks.Emit(ptrack, 1);
                    }

                    if (Dust)
                    {
                        Dust.transform.position = new Vector3(foot.transform.position.x, footRay.point.y + trackOffset, foot.transform.position.z); //Get The Position
                        Dust.transform.rotation = (Quaternion.FromToRotation(-foot.transform.forward, footRay.normal) * foot.transform.rotation);
                        Dust.transform.Rotate(-90, 0, 0);
                        Dust.Emit(DustParticles);
                    }
                }
            }
        }

        /// <summary>Disable this script, ex.. deactivate when is sleeping or death </summary>
        public virtual void EnableSteps(bool value)
        {
            Active = value;
        }

        public virtual void OnAnimatorBehaviourMessage(string message, object value)
        {
            this.InvokeWithParams(message, value);
        }
    }
}
