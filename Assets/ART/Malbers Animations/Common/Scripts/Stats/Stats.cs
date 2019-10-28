using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MalbersAnimations.Scriptables;
using UnityEngine.Events;
using MalbersAnimations.Events;
using System;

namespace MalbersAnimations
{
    public class Stats : MonoBehaviour
    {
        public List<Stat> stats = new List<Stat>();

        [SerializeField] private Stat PinnedStat;

        private void Start()
        {
            StopAllCoroutines();

            foreach (var stat in stats)
            {
                stat.InitializeStat(this);
            }
        }


        private void OnDisable()
        {
            StopAllCoroutines();

            foreach (var stat in stats) stat.Clean();
        }


        public virtual void _PinStat(string name) { PinnedStat = GetStat(name); }

        public virtual void _PinStat(int ID) { PinnedStat = GetStat(ID); }

        public virtual void _PinStat(IntVar ID) { PinnedStat = GetStat(ID); }

        public virtual Stat GetStat(string name)
        {
            PinnedStat = stats.Find(item => item.name == name);
            return PinnedStat;
        }

        public virtual Stat GetStat(int ID)
        {
            PinnedStat = stats.Find(item => item.ID == ID);
            return PinnedStat;
        }

        public virtual Stat GetStat(IntVar ID)
        {
            PinnedStat = stats.Find(item => item.ID == ID);
            return PinnedStat;
        }

        public virtual void _PinStatModifyValue(float value)
        {
            if (PinnedStat != null) PinnedStat.Modify(value);
            else Debug.Log("There's no Pinned Stat");
        }

        public virtual void _PinStatModifyValue(float value, float time)
        {
            if (PinnedStat != null) PinnedStat.Modify(value,time);
            else Debug.Log("There's no Pinned Stat");
        }

        public virtual void _PinStatModifyValue1Sec(float value)
        {
            if (PinnedStat != null) PinnedStat.Modify(value, 1);
            else Debug.Log("There's no Pinned Stat");
        }

        public virtual void _PinStatSetValue(float value)
        {
            if (PinnedStat != null) PinnedStat.Value = value;
            else Debug.Log("There's no Pinned Stat");
        }


        /// <summary>
        /// Set the Pinned Stat MAX Value
        /// </summary>
        public virtual void _PinStatModifyMaxValue(float value)
        {
            if (PinnedStat != null) PinnedStat.ModifyMAX(value);
            else Debug.Log("There's no Pinned Stat");
        }

        /// <summary>
        /// Set the Pinned Stat MAX Value
        /// </summary>
        public virtual void _PinStatSetMaxValue(float value)
        {
            if (PinnedStat != null) PinnedStat.MaxValue = value;
            else Debug.Log("There's no Pinned Stat");
        }

        /// <summary>
        /// Enable/Disable the Pinned Stat Regeneration Rate
        /// </summary>
        public virtual void _PinStatModifyRegenerationRate(float value)
        {
            if (PinnedStat != null)
            {
                PinnedStat.ModifyRegenerationRate(value);
            }
            else
            {
                Debug.Log("There's no Active Stat or the Stat you are trying to modify does not exist");
            }
        }


        /// <summary>
        /// Enable/Disable the Pinned Stat Degeneration
        /// </summary>
        public virtual void _PinStatDegenerate(bool value)
        {
            if (PinnedStat != null)
            {
                PinnedStat.Degenerate = value;
            }
            else
            {
                Debug.Log("There's no Active Stat or the Stat you are trying to modify does not exist");
            }
        }


        /// <summary>
        /// Enable/Disable the Pinned Stat Regeneration
        /// </summary>
        public virtual void _PinStatRegenerate(bool value)
        {
            if (PinnedStat != null)
            {
                PinnedStat.Regenerate = value;
            }
            else
                Debug.Log("There's no Active Stat or the Stat you are trying to modify does not exist");
        }

        /// <summary>
        /// Enable/Disable the Pinned Stat
        /// </summary>
        public virtual void _PinStatEnable(bool value)
        {
            if (PinnedStat != null)
            {
                PinnedStat.Active = value;
            }
            else
                Debug.Log("There's no Active Stat or the Stat you are trying to modify does not exist");
        }

        /// <summary>
        /// Modify the Pinned Stat value with a 'new Value',  'ticks' times , every 'timeBetweenTicks' seconds
        /// </summary>
        public virtual void _PinStatModifyValue(float newValue, int ticks, float timeBetweenTicks)
        {
            if (PinnedStat != null)
            {
                PinnedStat.Modify(newValue, ticks, timeBetweenTicks);
            }
            else
                Debug.Log("There's no Active Stat or the Stat you are trying to modify does not exist");
        }

        /// <summary>
        /// Clean the Pinned Stat from All Regeneration/Degeneration and Modify Tick Values
        /// </summary>
        public virtual void _PinStatCLEAN()
        {
            if (PinnedStat != null)
            {
                PinnedStat.Clean();
            }
            else
                Debug.Log("There's no Active Stat or the Stat you are trying to modify does not exist");
        }

    }


    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    ///STAT CLASS
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    [Serializable]
    public class Stat
    {
        #region Variables
        /// <summary>
        /// Name of the Stat
        /// </summary>
        public string name;
        [SerializeField] private bool active = true;


        /// <summary>
        /// ID of the Stat
        /// </summary>
        public IntReference ID;
        /// <summary>
        /// Value and Default/RestoreValue of the Stat
        /// </summary>
        [SerializeField] private FloatReference value;
        /// <summary>
        ///  Restore Value of the Stat
        /// </summary>
        [SerializeField] private FloatReference maxValue;
        /// <summary>
        ///  Min Value of the Stat
        /// </summary>
        [SerializeField] private FloatReference minValue;

        [SerializeField] private bool regenerate = false;
        /// <summary>
        /// Regeneration Rate. When the value is modified this will increase or decrease it over time.
        /// </summary>
        public FloatReference RegenRate;
        /// <summary>
        /// Regeneration Rate. When the value is modified this will increase or decrease it over time.
        /// </summary>
        public FloatReference RegenWaitTime;

        [SerializeField] private bool degenerate = false;
        /// <summary>
        /// Degeneration Rate. When the value is modified this will increase or decrease it over time.
        /// </summary>
        public FloatReference DegenRate;

      

        private bool isBelow = false;
        private bool isAbove = false;
        #endregion

        #region Events
        public bool ShowEvents = false;
        public UnityEvent OnStatFull = new UnityEvent();
        public UnityEvent OnStatEmpty = new UnityEvent();
        [SerializeField] public float Below;
        [SerializeField] public float Above;
        public UnityEvent OnStatBelow = new UnityEvent();
        public UnityEvent OnStatAbove = new UnityEvent();
        public FloatEvent OnValueChangeNormalized = new FloatEvent();
        public FloatEvent OnValueChange = new FloatEvent();
        public BoolEvent OnDegenereate = new BoolEvent();
        #endregion

        #region Properties
        /// <summary>
        /// Is the Stat Enabled? when Disable no modification can be done. All current modification can't be stopped
        /// </summary>
        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                if (value) StartRegeneration(); //If the Stat was activated start the regeneration
                else
                {
                    StopRegeneration();
                }
            }
        }
        /// <summary>
        /// Current value of the Stat
        /// </summary>
        public float Value
        {
            get { return value.Value; }
            set
            {
                if (!Active) return; //If the  Stat is not Active do nothing

                if (value < 0) value = 0;           //if value gets below zero interrupt it.

                if (this.value.Value != value)      //Check the code below only if the value has changed
                {
                    this.value.Value = value;

                    if (value == 0) OnStatEmpty.Invoke();   //if the Value is 0 invoke Empty Stat

                    OnValueChangeNormalized.Invoke(value / MaxValue);
                    OnValueChange.Invoke(value);

                    if (value > Above && !isAbove)
                    {
                        OnStatAbove.Invoke();
                        isAbove = true;
                        isBelow = false;
                    }
                    else if (value < Below && !isBelow)
                    {
                        OnStatBelow.Invoke();
                        isBelow = true;
                        isAbove = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets Sets the Default Value for the Stat... the value to return to
        /// </summary>
        public float MaxValue
        {
            get { return maxValue; }
            set { maxValue.Value = value; }
        }

        /// <summary>
        /// Gets Sets the Default Value for the Stat... the value to return to
        /// </summary>
        public float MinValue
        {
            get { return minValue; }
            set { minValue.Value = value; }
        }

        /// <summary>
        /// Can the Stat Regenerate over time
        /// </summary>
        public bool Regenerate
        {
            get { return regenerate; }
            set
            {
                regenerate = value;
                Regenerate_OldValue = regenerate;           //In case Regenerate is changed 
                StartRegeneration();
            }
        }

        /// <summary>
        /// Can the Stat Degenerate over time
        /// </summary>
        public bool Degenerate
        {
            get { return degenerate; }
            set
            {
                if (degenerate != value)        //If the Values are diferent then...
                {
                    degenerate = value;
                    OnDegenereate.Invoke(value);

                    if (degenerate)
                    {
                        regenerate = false;     //Do not Regenerate if we are Degenerating
                        StartDegeneration();
                        StopRegeneration();
                    }
                    else
                    {
                        regenerate = Regenerate_OldValue;   //If we are no longer Degenerating Start Regenerating again
                        StopDegeneration();
                        StartRegeneration();
                    }
                }
            }
        }

        #endregion


        bool Regenerate_OldValue;
        internal void InitializeStat(MonoBehaviour holder)
        {
            isAbove = isBelow = false;
            Coroutine = holder;

            if (value.Value > Above) isAbove = true;        //This means that The Stat Value is over the Above value
            else if (value.Value < Below) isBelow = true;   //This means that The Stat Value is under the Below value

            Regenerate_OldValue = Regenerate;

            if (MaxValue < Value)
            {
                MaxValue = Value;
            }

            Regeneration = null;
            Degeneration = null;
            ModifyPerTicks = null;

            StartRegeneration();
        }


        /// <summary>
        /// Adds or remove to the Stat Value
        /// </summary>
        /// <param name="newValue"></param>
        public virtual void Modify(float newValue)
        {
            if (!Active) return;            //Ignore if the Stat is Disable
            Value += newValue;
            StartRegeneration();
        }

        /// <summary>
        /// Adds or remove to the Stat Value
        /// </summary>
        /// <param name="newValue"></param>
        public virtual void Modify(float newValue, float time)
        {
            if (!Active) return;            //Ignore if the Stat is Disable

            StopSlowModification();


            ModifySlow = C_SmoothChangeValue(newValue, time);
            Coroutine.StartCoroutine(ModifySlow);
        }

        /// <summary>
        /// Modify the Stat value with a 'new Value',  'ticks' times , every 'timeBetweenTicks' seconds
        /// </summary>
        public virtual void Modify(float newValue, int ticks, float timeBetweenTicks)
        {
            if (!Active) return;            //Ignore if the Stat is Disable

            if (ModifyPerTicks != null) Coroutine.StopCoroutine(ModifyPerTicks);
            {
                ModifyPerTicks = C_ModifyTicksValue(newValue, ticks, timeBetweenTicks);
                Coroutine.StartCoroutine(ModifyPerTicks);
            }
        }

        /// <summary>
        /// Add or Remove Value the 'MaxValue' of the Stat
        /// </summary>
        public virtual void ModifyMAX(float newValue)
        {
            if (!Active) return;            //Ignore if the Stat is Disable

            MaxValue += newValue;
            StartRegeneration();
        }


        /// <summary>
        /// Add or Remove Rate to the Regeneration Rate
        /// </summary>
        /// <param name="newValue">Value to add or remove to the Regenation Rate</param>
        public virtual void ModifyRegenerationRate(float newValue)
        {
            if (!Active) return;            //Ignore if the Stat is Disable

            RegenRate.Value += newValue;
            StartRegeneration();
        }

        public virtual void ModifyRegenerationWait(float newValue)
        {
            if (!Active) return;            //Ignore if the Stat is Disable

            RegenWaitTime.Value += newValue;

            if (RegenWaitTime < 0) RegenWaitTime.Value = 0;
        }

        public virtual void SetRegenerationRate(float newValue)
        {
            if (!Active) return;            //Ignore if the Stat is Disable
            RegenRate.Value = newValue;
        }

        /// <summary>
        /// Reset the Stat to the Default Value
        /// </summary>
        public virtual void Reset()
        {
            Value = MaxValue;
        }

        public virtual void Clean()
        {
            StopDegeneration();
            StopRegeneration();
            StopTickDamage();
            StopSlowModification();
        }


        protected virtual void StartRegeneration()
        {
            StopRegeneration();
            if (RegenRate == 0 || !Regenerate) return;            //Means if there's no Regeneration

            Regeneration = C_Regenerate();
            Coroutine.StartCoroutine(Regeneration);
        }

        protected virtual void StartDegeneration()
        {
            if (DegenRate == 0) return;                                             //Means there's no Degeneration
            StopDegeneration();
            Degeneration = C_Degenerate();
            Coroutine.StartCoroutine(Degeneration);
        }

        protected virtual void StopRegeneration()
        {
            if (Regeneration != null) Coroutine.StopCoroutine(Regeneration);       //If there was a regenation active .... interrupt it
            Regeneration = null;
        }

        protected virtual void StopDegeneration()
        {
            if (Degeneration != null) Coroutine.StopCoroutine(Degeneration);   //if it was ALREADY Degenerating.. stop
            Degeneration = null;
        }

        protected virtual void StopTickDamage()
        {
            if (ModifyPerTicks != null) Coroutine.StopCoroutine(ModifyPerTicks);   //if it was ALREADY Degenerating.. stop
            ModifyPerTicks = null;
        }

        protected virtual void StopSlowModification()
        {
            if (ModifySlow != null) Coroutine.StopCoroutine(ModifySlow);       //If there was a regenation active .... interrupt it
            ModifySlow = null;
        }

        #region Coroutines
        internal MonoBehaviour Coroutine; //I need this to use coroutines in this class because it does not inherit from Monobehaviour
        public IEnumerator Regeneration;
        public IEnumerator Degeneration;
        public IEnumerator ModifyPerTicks;
        public IEnumerator ModifySlow;

        protected virtual IEnumerator C_Regenerate()
        {
            if (RegenWaitTime > 0) yield return new WaitForSeconds(RegenWaitTime);   //Wait a time to regenerate

            float ReachValue = RegenRate > 0 ? MaxValue : 0; //Set to the default or 0
            bool Positive = RegenRate > 0; //is the Regeneration Positive?

            while (Value != ReachValue)
            {
                Value += (RegenRate * Time.deltaTime);

                if (Positive && Value > MaxValue)
                {
                    Reset();
                    OnStatFull.Invoke();
                }
                else if (!Positive && Value < 0)
                {
                    Value = MinValue;
                    OnStatEmpty.Invoke();
                }

                yield return null;
            }
            yield return null;
        }

        protected virtual IEnumerator C_Degenerate()
        {
            while (Degenerate || Value <= MinValue)
            {
                Value -= (DegenRate * Time.deltaTime);
                yield return null;
            }
            yield return null;
        }

        protected virtual IEnumerator C_ModifyTicksValue(float value, int Ticks, float time)
        {
            var WaitForTicks = new WaitForSeconds(time);

            for (int i = 0; i < Ticks; i++)
            {
                Value += value;
                if (Value <= MinValue)
                {
                    Value = MinValue;
                    break;
                }
                yield return WaitForTicks;
            }

            yield return null;

            StartRegeneration();
        }

        protected virtual IEnumerator C_SmoothChangeValue(float newvalue, float smoothChangeValueTime)
        {
            StopRegeneration();

            Debug.Log(newvalue);

            float currentTime = 0;
            float currentValue = Value;
            newvalue = Value + newvalue;


            while (currentTime <= smoothChangeValueTime)
            {

                Value = Mathf.Lerp(currentValue, newvalue, currentTime / smoothChangeValueTime);
                currentTime += Time.deltaTime;


                yield return null;
            }
            Value = newvalue;

            yield return null;
            StartRegeneration();
        }
        #endregion

    }
}