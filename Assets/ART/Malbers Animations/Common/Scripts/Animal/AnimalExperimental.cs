using UnityEngine;
using System.Collections;
using MalbersAnimations.Events;
using System.Collections.Generic;

namespace MalbersAnimations
{
    //Experimental Methods
    public partial class Animal 
    {
        public Dictionary<string, BoolEvent> Inputs;

        /// <summary>
        /// Initialize the Inputs from the Input Scripts
        /// </summary>
        /// <param name="key"></param>
        public void InitializeInputs(Dictionary<string, BoolEvent> keys)
        {
            if (Inputs == null) Inputs = new Dictionary<string, BoolEvent>();

            Inputs = keys;
            CharacterConnect();
        }

        /// <summary>
        /// If an Input is received, Invoke the Bool Value so the methods asociated to it can use it
        /// </summary>
        /// <param name="key">name of the input</param>
        /// <param name="value">Input Value</param>
        public void SetInput(string key, bool value)
        {
            BoolEvent inputE;

            if (Inputs.TryGetValue(key, out inputE))
            {
                inputE.Invoke(value);
            }
        }

        /// <summary>
        /// Add a Single Input to the Animal
        /// </summary>
        public  void AddInput(string key, BoolEvent NewBool)
        {
            if (!Inputs.ContainsKey(key))
            {
                Inputs.Add(key, NewBool);
            }
        }

        /// <summary>
        /// Connect all the Inputs to the Funtions
        /// </summary>
        private void CharacterConnect()
        {
            BoolEvent BAttack1, BAttack2, BAction, BJump, BShift, BFly, 
                BDown, BUp, BDodge, BDeath, BStun, BDamaged, BSpeed1, BSpeed2, BSpeed3, BSpeedUp, BSpeedDown;

            if (Inputs.TryGetValue("Attack1", out BAttack1)) BAttack1.AddListener(value => Attack1 = value);
            if (Inputs.TryGetValue("Attack2", out BAttack2)) BAttack2.AddListener(value => Attack2 = value);
            if (Inputs.TryGetValue("Action", out BAction)) BAction.AddListener(value => Action = value);

            if (Inputs.TryGetValue("Jump", out BJump)) BJump.AddListener(value => Jump = value);
            if (Inputs.TryGetValue("Shift", out BShift)) BShift.AddListener(value => Shift = value);
            if (Inputs.TryGetValue("Fly", out BFly)) BFly.AddListener(value => Fly = value);

            if (Inputs.TryGetValue("Down", out BDown)) BDown.AddListener(value => Down = value);
            if (Inputs.TryGetValue("Up", out BUp)) BUp.AddListener(value => Up = value);

            if (Inputs.TryGetValue("Dodge", out BDodge)) BDodge.AddListener(value => Dodge = value);
            if (Inputs.TryGetValue("Death", out BDeath)) BDeath.AddListener(value => Death = value);
            if (Inputs.TryGetValue("Stun", out BStun)) BStun.AddListener(value => Stun = value);
            if (Inputs.TryGetValue("Damaged", out BDamaged)) BDamaged.AddListener(value => Damaged = value);

            if (Inputs.TryGetValue("Speed1", out BSpeed1)) BSpeed1.AddListener(value => Speed1 = value);
            if (Inputs.TryGetValue("Speed2", out BSpeed2)) BSpeed2.AddListener(value => Speed2 = value);
            if (Inputs.TryGetValue("Speed3", out BSpeed3)) BSpeed3.AddListener(value => Speed3 = value);

            if (Inputs.TryGetValue("SpeedUp", out BSpeedUp)) BSpeedUp.AddListener(value => SpeedUp = value);
            if (Inputs.TryGetValue("SpeedDown", out BSpeedDown)) BSpeedDown.AddListener(value => SpeedDown = value);
        }
    }
}
