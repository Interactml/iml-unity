using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ReflectionPublicVars : MonoBehaviour
{
    public Component[] myComponents;

    //public ClassPublicVars classToGetVars;

    // Start is called before the first frame update
    void Start()
    {
        //var vars = classToGetVars.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        //Debug.Log("============");

        //foreach (var field in vars)
        //{
        //    Debug.Log("Component:  " + classToGetVars.GetType().Name + "        Var Name:  " + field.Name + "         Type:  " + field.FieldType + "       Value:  " + field.GetValue(field) + "\n");

        //    if (field.Name == "varFloat1")
        //    {
        //        Debug.Log("Modifying float...");
        //        field.SetValue(classToGetVars, 12533656f);
        //    }
        //}

        //Debug.Log("============");

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Component myComp in myComponents)
        {
            Type myObjectType = myComp.GetType();
            foreach (var thisVar in myComp.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                try
                {
                    Debug.Log("Component:  " + myComp.name + "        Var Name:  " + thisVar.Name + "         Type:  " + thisVar.FieldType  + "       Value:  " + thisVar.GetValue(myComp) + "\n");

                    //Debug.Log("Component:  " + myComp.name + "        Var Name:  " + thisVar.Name + "         Type:  " + thisVar.PropertyType + "       Value:  " + thisVar.GetValue(myComp, null) + "\n");

                    if (thisVar.Name == "varFloat1")
                    {
                        Debug.Log("Modifying float...");
                        thisVar.SetValue(myComp, 12533656f);
                    }

                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }
}
