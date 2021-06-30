    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
    [System.Serializable]
    public class MaterialSound
    {
        public AudioClip[] Sounds;
        public PhysicMaterial material;
    }

    /// <summary>
    /// This class is for using Physic materials for sound effects
    /// </summary>
    public class SoundByMaterial : MonoBehaviour
    {
        public AudioClip DefaultSound;
        public List<MaterialSound> materialSounds;

        private AudioSource audioSource;

        protected AudioSource Audio_Source
        {
            get
            {
                if (!audioSource)
                {
                    audioSource = GetComponent<AudioSource>();
                }
                return audioSource;
            }
            set { audioSource = value; }
        }

        public virtual void PlayMaterialSound(RaycastHit hitSurface)
        {
            var hits = hitSurface.collider;
            if (hits)
            {
                PlayMaterialSound(hits.sharedMaterial);
            }
        }

        public virtual void PlayMaterialSound(GameObject hitSurface)
        {
            var hits = hitSurface.GetComponent<Collider>();
            if (hits)
            {
                PlayMaterialSound(hits.sharedMaterial);
            }
        }

        public virtual void PlayMaterialSound(Collider hitSurface)
        {
            PlayMaterialSound(hitSurface.sharedMaterial);
        }

        public virtual void PlayMaterialSound(PhysicMaterial hitSurface)
        {
            if (!Audio_Source)
            {
                Audio_Source = gameObject.AddComponent<AudioSource>();
                Audio_Source.spatialBlend = 1;
            }

            MaterialSound mat = materialSounds.Find(item => item.material == hitSurface);


            if (mat != null)
            {
                var sound = mat.Sounds[Random.Range(0, mat.Sounds.Length)];
                Audio_Source.clip = sound;
                audioSource.Play();
            }
            else
            {
                if (DefaultSound)
                {
                    Audio_Source.clip = DefaultSound;
                    audioSource.Play();
                }
            }
        }
    }
}