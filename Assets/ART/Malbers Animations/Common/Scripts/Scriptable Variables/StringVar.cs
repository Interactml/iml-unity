using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    ///<summary>
    /// String Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple
    /// </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptable Variables/String Var")]
    public class StringVar : ScriptableObject
    {
        [SerializeField]
        /// <summary>
        /// The current value
        /// </summary>
        private string value = "";
        /// <summary>
        /// The default float value to return to
        /// </summary>
        [SerializeField]
        private string defaultValue = "";

#if UNITY_EDITOR
        [TextArea(3, 20)]
        public string Description = "";
#endif
        /// <summary>
        /// When active OnValue changed will ve used every time the value changes (you can subscribe only at runtime .?)
        /// </summary>
        public bool UseEvent = true;
        /// <summary>
        /// Invoked when the value changes
        /// </summary>
        public Events.StringEvent OnValueChanged = new Events.StringEvent();

        /// <summary>
        /// Value of the String Scriptable variable
        /// </summary>
        public virtual string Value
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

        /// <summary>
        /// The default float value to return to
        /// </summary>
        public virtual string DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        /// <summary>
        /// Reset the Float Value to the Default value
        /// </summary>
        public virtual void ResetValue() { Value = DefaultValue; }

        public virtual void SetValue(StringVar var)
        {
            Value = var.Value;
            DefaultValue = var.DefaultValue;
        }

        public static implicit operator string(StringVar reference)
        {
            return reference.Value;
        }
    }
}
