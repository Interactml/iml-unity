
namespace MalbersAnimations.Scriptables
{
    [System.Serializable]
    public class BoolReference
    {
        public bool UseConstant = true;
        public bool ConstantValue;

        /// <summary>
        /// The Value to return to when Reset is called
        /// </summary>
        public bool DefaultValue;

        /// <summary>
        /// The Value to return to when Reset is called
        /// </summary>
        public BoolVar Variable;

        public BoolReference()
        {
            UseConstant = true;
            ConstantValue = false;
            DefaultValue = false;
        }

        public BoolReference(bool value)
        {
            Value = value;
        }

        public bool Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
            set
            {
                if (UseConstant)
                    ConstantValue = value;
                else
                    Variable.Value = value;
            }
        }
        #region Operators
        public static implicit operator bool(BoolReference reference)
        {
            return reference.Value;
        }
        #endregion
    }
}