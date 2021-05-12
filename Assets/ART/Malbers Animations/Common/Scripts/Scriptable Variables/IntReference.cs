using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Scriptables
{
    [System.Serializable]
    public class IntReference
    {
        public bool UseConstant = true;
        public int ConstantValue;
        public int ResetValue;
        public IntVar Variable;

        public IntReference()
        {
            UseConstant = true;
            ConstantValue = 0;
        }

        public IntReference(bool variable = false)
        {
            UseConstant = !variable;

            if (!variable)
            {
                ConstantValue = 0;
            }
            else
            {
                Variable = ScriptableObject.CreateInstance<IntVar>();
                Variable.Value = 0;
            }
        }

        public IntReference(int value)
        {
            Value = value;
        }

        public int Value
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
        public static implicit operator int(IntReference reference)
        {
            return reference.Value;
        }
        #endregion
    }
}