using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MalbersAnimations.Utilities
{
    public class EffectManager : MonoBehaviour, IAnimatorListener
    {
        public List<Effect> Effects;


        void Awake()
        {
            foreach (var e in Effects)
            {
                e.Owner = transform;  //Save in all effects that the owner of the effects is this transform
                if (!e.instantiate && e.effect)
                {
                    e.Instance = e.effect;
                }
            }

        }

        public virtual void PlayEffect(int ID)
        {
            List<Effect> effects = Effects.FindAll(effect => effect.ID == ID && effect.active == true);

            if (effects != null)
                foreach (var effect in effects) Play(effect);
        }


        IEnumerator IPlayEffect(Effect e)
        {
           
            if (e.delay > 0)
                yield return new WaitForSeconds(e.delay);

            yield return new WaitForEndOfFrame();                   //Wait until the Animation Cyle has pass (LateUpdate)

            if (e.instantiate)                                      //If instantiate is active (meaning is a prefab)
            {
                e.Instance = Instantiate(e.effect);                 //Instantiate!
                e.effect.gameObject.SetActive(false);
            }
            else
            {
                e.Instance = e.effect;                              //Use the effect as the gameobject
            }

            if (e.Instance && e.root)
            {
                e.Instance.transform.position = e.root.position;
              
             
                e.Instance.gameObject.SetActive(true);
            }

            var trail = e.Instance.GetComponentInChildren<TrailRenderer>(); //UNITY BUG!!! WITH TRAIL RENDERERS
            if (trail) trail.Clear();

            e.Instance.transform.localScale = Vector3.Scale(e.Instance.transform.localScale, e.ScaleMultiplier); //Scale the Effect

            e.OnPlay.Invoke();                                      //Invoke the Play Event

            if (e.root)
            {
                if (e.isChild)
                {
                    e.Instance.transform.parent = e.root;
                }

                if (e.useRootRotation) e.Instance.transform.rotation = e.root.rotation;     //Orient to the root rotation
                
            }

            //Apply Offsets
            e.Instance.transform.localPosition += e.PositionOffset;
            e.Instance.transform.localRotation *= Quaternion.Euler(e.RotationOffset);

            if (e.Modifier) e.Modifier.StartEffect(e);              //Apply  Modifier when the effect play

            StartCoroutine(Life(e));

            yield return null;
        }

        public virtual void StopEffect(int ID)
        {
            List<Effect> effects = Effects.FindAll(effect => effect.ID == ID && effect.active == true);

            if (effects != null)

            {
                foreach (var e in effects)
                {
                    if (e.Modifier) e.Modifier.StopEffect(e);              //Play Modifier when the effect play
                    e.OnStop.Invoke();
                    e.On = false;
                }
            }
        }

        IEnumerator Life(Effect e)
        {
            if (e.life > 0)
            {
                yield return new WaitForSeconds(e.life);

                if (e.Modifier) e.Modifier.StopEffect(e);              //Play Modifier when the effect play

                e.OnStop.Invoke();


                if (e.instantiate)
                {
                    Destroy(e.Instance);
                }
            }

            yield return null;
        }

       protected virtual void Play(Effect effect)
        {
            if (effect.effect == null) return;  //There's no effect available
          
            if (effect.Modifier) effect.Modifier.AwakeEffect(effect);        //Execute the Method PreStart Effect if it has a modifier

            if (effect.toggleable)
            {
                effect.On = !effect.On;

                if (effect.On)
                {
                    StartCoroutine(IPlayEffect(effect));
                }
                else
                {
                    effect.OnStop.Invoke();
                }
            }
            else
            {
                StartCoroutine(IPlayEffect(effect));
            }
        }


        /// <summary>
        /// IAnimatorListener function
        /// </summary>
        public virtual void OnAnimatorBehaviourMessage(string message, object value)
        {
            this.InvokeWithParams(message, value);
        }

        //─────────────────────────────────CALLBACKS METHODS───────────────────────────────────────────────────────────────────

        /// <summary>
        /// Disables all effects using their name
        /// </summary>
        public virtual void _DisableEffect(string name)
        {
            List<Effect> effects = Effects.FindAll(effect => effect.Name.ToUpper() == name.ToUpper());

            if (effects != null)
            {
                foreach (var e in effects) e.active = false;
            }
            else
            {
                Debug.LogWarning("No effect with the name: " + name + " was found");
            }
        }

        /// <summary>
        /// Disables all effects using their ID
        /// </summary>
        public virtual void _DisableEffect(int ID)
        {
            List<Effect> effects = Effects.FindAll(effect => effect.ID == ID);

            if (effects != null)
            {
                foreach (var e in effects) e.active = false;
            }
            else
            {
                Debug.LogWarning("No effect with the ID: " + ID + " was found");
            }
        }

        public virtual void _EnableEffect(string name)
        {
            List<Effect> effects = Effects.FindAll(effect => effect.Name.ToUpper() == name.ToUpper());

            if (effects != null)
            {
                foreach (var e in effects) e.active = true;
            }
            else
            {
                Debug.LogWarning("No effect with the name: " + name + " was found");
            }
        }

        public virtual void _EnableEffect(int ID)
        {
            List<Effect> effects = Effects.FindAll(effect => effect.ID == ID);

            if (effects != null)
            {
                foreach (var e in effects) e.active = true;
            }
            else
            {
                Debug.LogWarning("No effect with the ID: " + ID + " was found");
            }
        }

        public virtual void _EnableEffectPrefab(int ID)
        {
            Effect e = Effects.Find(item => item.ID == ID);

            if (e != null)
            {
                e.Instance.SetActive(true);
            }
        }

        public virtual void _DisableEffectPrefab(int ID)
        {
            Effect e = Effects.Find(item => item.ID == ID);

            if (e != null)
            {
                e.Instance.SetActive(false);
            }
        }
    }

    [System.Serializable]
    public class Effect
    {
        public string Name  = "EffectName";
        public int ID;
        public bool active = true;
        public Transform root;
       
        public bool isChild;
        public bool useRootRotation = true;
        public GameObject effect;
        public Vector3 RotationOffset;
        public Vector3 PositionOffset;
        public Vector3 ScaleMultiplier = Vector3.one;


        /// <summary>
        /// Life of the Effect  
        /// </summary>
        public float life = 10f;

        /// <summary>
        /// Delay Time to execute the effect after is called.
        /// </summary>
        public float delay;
        /// <summary>
        /// Is the Effect an Instance?
        /// </summary>
        public bool instantiate = true;
        /// <summary>
        /// When Toggleable is on the Effect will not be destroy or instantiated.. instead you can use the events for enabling/disabling options on the effect
        /// </summary>
        public bool toggleable = false;
       
        /// <summary>
        /// Is the Effect Active?
        /// </summary>
        public bool On;

        /// <summary>
        /// Scriptable Object to Modify anything you want before, during or after the effect is invoked
        /// </summary>
        public EffectModifier Modifier;


        public UnityEvent OnPlay;
        public UnityEvent OnStop;

        protected Transform owner;

        public Transform Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public GameObject Instance
        {
            get { return instance; }
            set { instance = value; }
        }

        protected GameObject instance;
    }
}