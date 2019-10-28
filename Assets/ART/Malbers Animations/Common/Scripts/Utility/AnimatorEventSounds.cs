using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Utilities
{

    public class AnimatorEventSounds : MonoBehaviour
    {

        public List<EventSound> m_EventSound;
        public AudioSource _audioSource;
        protected Animator anim;

        void Start()
        {
            anim = GetComponent<Animator>();                    //Get the reference for the animator

            if (_audioSource == null)                           //if there's no audio source add one..
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            _audioSource.volume = 0;                            
        }


        public virtual void PlaySound(AnimationEvent e)
        {
            if (e.animatorClipInfo.weight < 0.1) return; // if is too small the weight of the animation clip do nothing

            EventSound SoundEvent = m_EventSound.Find(item => item.name == e.stringParameter);

            if (SoundEvent != null)
            {
                SoundEvent.VolumeWeight = e.animatorClipInfo.weight;

                if (anim) _audioSource.pitch = anim.speed;                     //Match the AnimatorSpeed with the Sound Pitch

                if (_audioSource.isPlaying)                                         //If the Audio is already Playing play the one that has more weight
                {
                    if (SoundEvent.VolumeWeight * SoundEvent.volume > _audioSource.volume)
                    {
                        SoundEvent.PlayAudio(_audioSource);
                    }
                }
                else
                {
                    SoundEvent.PlayAudio(_audioSource);
                }
            }
        }
    }

    [System.Serializable]
    public class EventSound
    {
        public string name = "Name Here";
        public AudioClip[] Clips;
        public float volume = 1;
        public float pitch = 1;

        protected float volumeWeight = 1;

        public float VolumeWeight
        {
            set { volumeWeight = value; }
            get { return volumeWeight; }
        }

        public void PlayAudio(AudioSource audio)
        {
            if (audio == null) return;                              //Do nothing if the audio is empty
            if (Clips == null || Clips.Length == 0) return;         //Do nothing if there's no clips 

            audio.spatialBlend = 1;                                 //Set the sound to 3D

            audio.clip = Clips[Random.Range(0, Clips.Length)];      //Set a random clip to the audio Source
            audio.pitch *= pitch;                                   //Depending the animator speed modify the pitch
            audio.volume = Mathf.Clamp01(volume * VolumeWeight);    //Depending the weight of the animation clip modify the volume
            audio.Play();                                           //Play the Audio
        }
    }
}