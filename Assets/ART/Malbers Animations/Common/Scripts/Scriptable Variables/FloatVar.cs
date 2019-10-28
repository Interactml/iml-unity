using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    ///<summary>
    /// Float Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple
    /// </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptable Variables/Float Var")]
    public class FloatVar : ScriptableObject
    {
        /// <summary>
        /// The current value
        /// </summary>
        [SerializeField] private float value = 0;

#if UNITY_EDITOR
        [TextArea(3, 20)]
        public string Description = "";
#endif
        /// <summary>
        /// When active OnValue changed will ve used every time the value changes (you can subscribe only at runtime .?)
        /// </summary>
        public bool UseEvent = false;

        /// <summary>
        /// Invoked when the value changes
        /// </summary>
        public Events.FloatEvent OnValueChanged = new Events.FloatEvent();

        /// <summary>
        /// Value of the Float Scriptable variable
        /// </summary>
        public virtual float Value
        {
            get { return value; }
            set
            {
                if (this.value != value)                                //If the value is diferent change it
                {
                    this.value = value;
                    if (UseEvent) OnValueChanged.Invoke(value);         //If we are using OnChange event Invoked
                }
            }
        }

        public static implicit operator float(FloatVar reference)
        {
            return reference.Value;
        }
    }
}