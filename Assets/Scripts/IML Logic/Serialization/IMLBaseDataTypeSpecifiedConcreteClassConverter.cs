using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using InteractML;

/// <summary>
/// Override for JSON default contract resolver to understand the IMLBaseDataType
/// </summary>
public class IMLBaseDataTypeSpecifiedConcreteClassConverter :  DefaultContractResolver
{
    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        if (typeof(IMLBaseDataType).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
        return base.ResolveContractConverter(objectType);
    }
}

