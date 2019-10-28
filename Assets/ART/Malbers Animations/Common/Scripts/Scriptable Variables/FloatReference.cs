using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    [System.Serializable]
    public class FloatReference
    {
        public bool UseConstant = true;
        public float ConstantValue;

        /// <summary>
        /// The Value to return to when Reset is called
        /// </summary>
        public FloatVar Variable;

        public FloatReference()
        {
            UseConstant = true;
            ConstantValue = 0;
        }

        public FloatReference(bool variable = false)
        {
            UseConstant = !variable;

            if (!variable)
            {
                ConstantValue = 0;
            }
            else
            {
                Variable = ScriptableObject.CreateInstance<FloatVar>();
                Variable.Value = 0;
            }
        }

        public FloatReference(float value)
        {
            Value = value;
        }

        public float Value
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

        public static implicit operator float(FloatReference reference)
        {
            return reference.Value;
        }
    }
}