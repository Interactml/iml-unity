using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    ///<summary>
    /// Int Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple
    /// </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptable Variables/Int Var")]
    public class IntVar : ScriptableObject
    {
        //  public bool Clone = false;

        /// <summary>
        /// The current value
        /// </summary>
        [SerializeField]   private int value = 0;
     
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
        public Events.IntEvent OnValueChanged = new Events.IntEvent();

        /// <summary>
        /// Value of the Float Scriptable variable
        /// </summary>
        public virtual int Value
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

        public virtual void SetValue(IntVar var)
        {
            Value = var.Value;
        }

        public static implicit operator int(IntVar reference)
        {
            return reference.Value;
        }
    }
}