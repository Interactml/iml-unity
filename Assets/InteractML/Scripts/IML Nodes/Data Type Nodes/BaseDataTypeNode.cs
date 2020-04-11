using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace InteractML.DataTypeNodes
{
    public abstract class BaseDataTypeNode<T> : Node, IFeatureIML
    {
        // Data variables
        public abstract T In { get; set; }

        public abstract T Value { get; set; }

        public abstract T Out { get; set; }

        // IMLFeature variables
        public abstract IMLBaseDataType FeatureValues { get; }

        /// <summary>
        /// Lets external classes know if they should call UpdateFeature (always true for a data type)
        /// </summary>
        bool IFeatureIML.isExternallyUpdatable { get { return true; } }

        /// <summary>
        /// Was tge featyre already updated?
        /// </summary>
        bool IFeatureIML.isUpdated { get ; set; }

        public string ValueName;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            Out = Value;
            return Out;
        }

        /// <summary>
        /// This update fetches the input value connected to this data type node
        /// </summary>
        /// <returns></returns>
        object IFeatureIML.UpdateFeature()
        {
            Value = GetInputValue<T>("In");
            return this;
        }
    }
}
