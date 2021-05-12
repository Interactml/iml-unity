using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    ///<summary>
    /// Float Scriptable Variable. Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple
    /// </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptable Variables/Vector3 Var")]
    public class Vector3Var : ScriptableObject
    {
        /// <summary>
        /// The current value
        /// </summary>
        [SerializeField] private Vector3 value = Vector3.zero;
       
 
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
        public Events.Vector3Event OnValueChanged = new Events.Vector3Event();

        /// <summary>
        /// Value of the Float Scriptable variable
        /// </summary>
        public virtual Vector3 Value
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

        public virtual void SetValue(Vector3Var var)
        {
            Value = var.Value;
        }

        public static implicit operator Vector3(Vector3Var reference)
        {
            return reference.Value;
        }

        public static implicit operator Vector2(Vector3Var reference)
        {
            return reference.Value;
        }

    }
}