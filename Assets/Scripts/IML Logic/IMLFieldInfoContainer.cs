using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XNode;

namespace InteractML
{
    public class IMLFieldInfoContainer
    {
        public FieldInfo fieldInfo;
        public Node nodeForField;
        public MonoBehaviour gameComponentWhereFieldIs;
        public IMLSpecifications.DataTypes DataType;

        public IMLFieldInfoContainer(FieldInfo fieldInfo, Node newNode, IMLSpecifications.DataTypes dataType, MonoBehaviour gameComponent)
        {
            this.fieldInfo = fieldInfo;
            this.nodeForField = newNode;
            this.gameComponentWhereFieldIs = gameComponent;
            this.DataType = dataType;
        }
    }

}
