using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using InteractML;
using System;
using Newtonsoft.Json.Linq;

/// <summary>
/// Custom Class that extends the json serializer Converter to help deserialize our custom IML classes
/// </summary>
public class IMLJsonTypeConverter : JsonConverter
{
    #region Variables

    // We make this static serializer settings to avoid a stack overflow (that is what the stackoverflow answer said)
    // I assume the JSON serializer will try to access it somehow, I don't specifically pass it in
    static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new IMLBaseDataTypeSpecifiedConcreteClassConverter() };

    #endregion

    #region JsonConverter Overrides

    public override bool CanConvert(Type objectType)
    {
        return (objectType == typeof(IMLBaseDataType));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);
        //Debug.Log("Calling readjson IMLBaseDataType. Object type is: " + objectType.ToString());


        // If we are deserializing an IMLBaseDataType...
        if (objectType == typeof(IMLBaseDataType))
        {
            return DeserializeIMLDataType(jo);
        }
        // If we are deserializing a Training Example....
        else if (objectType == typeof(IMLTrainingExample))
        {
            return DeserializeIMLTrainingExample(jo);
        }
        // If we are deserializing a Training Series...
        else if (objectType == typeof(IMLTrainingSeries))
        {
            return DeserializeIMLTrainingSeries(jo);
        }
        // If it is not one of our custom types that we control...
        else
        {
            return null;
        }

    }

    public override bool CanWrite
    {
        get { return false; }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException(); // won't be called because CanWrite returns false
    }

    #endregion

    #region Private Methods

    private IMLBaseDataType DeserializeIMLDataType (JObject jo)
    {
        // We know there we are expecting an IMLDataType and know its strcuture
        var valueToRead = jo["DataType"];
        // Handle in case it is not an IMLDataType
        if (valueToRead == null)
        {
            Debug.LogError("The value to deserialize is null when transforming it. It is not an IMLDataType!");
            return null;
        }
        // If we loaded it correctly, we convert it to the right format
        switch (jo["DataType"].Value<int>())
        {
            case (int)IMLSpecifications.DataTypes.Float:
                return JsonConvert.DeserializeObject<IMLFloat>(jo.ToString(), SpecifiedSubclassConversion);
            case (int)IMLSpecifications.DataTypes.Integer:
                return JsonConvert.DeserializeObject<IMLInteger>(jo.ToString(), SpecifiedSubclassConversion);
            case (int)IMLSpecifications.DataTypes.Vector2:
                return JsonConvert.DeserializeObject<IMLVector2>(jo.ToString(), SpecifiedSubclassConversion);
            case (int)IMLSpecifications.DataTypes.Vector3:
                return JsonConvert.DeserializeObject<IMLVector3>(jo.ToString(), SpecifiedSubclassConversion);
            case (int)IMLSpecifications.DataTypes.Vector4:
                return JsonConvert.DeserializeObject<IMLVector4>(jo.ToString(), SpecifiedSubclassConversion);
            case (int)IMLSpecifications.DataTypes.Array:
                return JsonConvert.DeserializeObject<IMLArray>(jo.ToString(), SpecifiedSubclassConversion);
            default:
                throw new Exception();
        }
        // If we reach here something went wrong
        throw new NotImplementedException();

    }

    private IMLTrainingExample DeserializeIMLTrainingExample(JObject jo)
    {
        // We know there we are expecting an IMLDataType and know its structure
        var inputsRead = jo["Inputs"];
        var outputsRead = jo["Outputs"];
        // Handle in case it is not an IMLDataType
        if (inputsRead == null)
        {
            Debug.LogError("The value to deserialize is null when transforming it. It is not an IMLDataType!");
            return null;
        }


        return null;
    }

    private IMLInput DeserializeIMLInput(JObject jo)
    {        
        return null;
    }

    private IMLOutput DeserializeIMLOutput(JObject jo)
    {
        return null;
    }

    private IMLTrainingExample DeserializeIMLTrainingSeries(JObject jo)
    {
        // We know there we are expecting an Series and know its structure
        var inputsRead = jo["Series"];
        var outputsRead = jo["LabelSeries"];
        // Handle in case it is not an IMLDataType
        if (inputsRead == null)
        {
            Debug.LogError("The value to deserialize is null when transforming it. It is not an IMLDataType!");
            return null;
        }


        return null;
    }

    #endregion
}
