using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XNode;
using System;

namespace InteractML.DataTypeNodes
{
    /// <summary>
    /// Gets the value from a component
    /// </summary>
    public class GetValueNode : Node
    {
        [Input]
        public GameObject GameObjectWithScript;

        public string ScriptName;

        [SerializeField]
        public Component ScriptToRead;

        public string ValueName;

        [Output]
        public IMLBaseDataType ValueToOutput;

        // The private instances to potentially output
        private IMLFloat m_Float;
        private IMLInteger m_Int;
        private IMLVector2 m_V2;
        private IMLVector3 m_V3;
        private IMLVector4 m_V4;

        // Use this for initialization
        protected override void Init()
        {
            base.Init();

            // Creating instances of the values to use
            m_Float = new IMLFloat();
            m_Int = new IMLInteger();
            m_V2 = new IMLVector2();
            m_V3 = new IMLVector3();
            m_V4 = new IMLVector4();

        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            // Try and pull gameObject
            GameObjectWithScript = GetInputValue<GameObject>("GameObjectWithScript", this.GameObjectWithScript);

            if (GameObjectWithScript)
            {
                ScriptToRead = GameObjectWithScript.GetComponent(ScriptName);

            }

            if (ScriptToRead != null)
            {
                // Try to read the value thrpugh reflection
                var fieldRead = ScriptToRead.GetType().GetField(ValueName);
                // If we read something, we update the corresponding value to return
                if (fieldRead != null)
                {
                    if (fieldRead.FieldType == typeof(float))
                    {
                        m_Float.SetValue((float)fieldRead.GetValue(ScriptToRead));
                        ValueToOutput = m_Float;
                    }
                    else if (fieldRead.FieldType == typeof(int))
                    {
                        m_Int.SetValue((int)fieldRead.GetValue(ScriptToRead));
                        ValueToOutput = m_Float;

                    }
                    else if (fieldRead.FieldType == typeof(Vector2))
                    {
                        m_V2.SetValues((Vector2)fieldRead.GetValue(ScriptToRead));
                        ValueToOutput = m_Float;

                    }
                    else if (fieldRead.FieldType == typeof(Vector3))
                    {
                        m_V3.SetValues((Vector3)fieldRead.GetValue(ScriptToRead));
                        ValueToOutput = m_Float;

                    }
                    else if (fieldRead.FieldType == typeof(Vector4) || fieldRead.FieldType == typeof(Quaternion))
                    {
                        m_V4.SetValues((Vector4)fieldRead.GetValue(ScriptToRead));
                        ValueToOutput = m_Float;
                    }
                }

                Debug.Log("Value read is: " + ValueToOutput.Values[0]);

                return ValueToOutput;

            }

            return true;
        }
    }

}
