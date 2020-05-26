//---------------------------------------------------------------------------
// Carlos Gonzalez Diaz - TFG - Simulador Virtual Carabina M4 - 2016
// Universidad Rey Juan Carlos - ETSII
//---------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Contains different classes with ReusableMethods in programming
/// </summary>
namespace ReusableMethods
{
    /// <summary>
    /// Class containing methods using probabilities
    /// </summary>
    public static class Probabilities
    {
        /// <summary>
        /// The method for trigerring something based on a probability value
        /// </summary>
        /// <param name="probVal"> The probability float value in the range [0, 1]</param>
        /// <returns> Returns more often True the higher probVal is </returns>
        public static bool GetResultProbability(float probVal)
        {
            // We check that probVal is between [0, 1]
            if (probVal > 1f || probVal < 0f)
            {
                Debug.LogError("The probability in GetResultProbability() is out of the range [0, 1]");
                return false;

            }

            // We generate a value between the range [0, 1]
            float valueToCompare = UnityEngine.Random.Range(0f, 1f);

            // If valueToCompare is below probVal, we return true. 
            // The higher probVal, the most likely to return true we will have
            if (valueToCompare < probVal)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }

    /// <summary>
    /// The class that will contain the methods for creating countdowns
    /// </summary>
    public class Countdown
    {
        /// <summary>
        /// This is just a test empty method
        /// </summary>
        /// <returns> No usable value, is an example</returns>
        public static int CreateCountdown()
        {
            return 4;
        }
    }

    /// <summary>
    /// Class containing methods and tools for enums
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Sets a random value for the Enum Type passed in
        /// </summary>
        /// <typeparam name="T"> T must be an Enum Type, from where to select the task</typeparam>
        /// <returns> The new task to perform</returns>
        public static T GetRandomEnumValue<T>() where T : struct, IComparable, IFormattable, IConvertible
        {
            // Since Enum Type implements IConvertible interface, we check if T is struct & IConvertible
            // This will still permit passing of value types implementing IConvertible. The chances are rare though.

            // If the type is not an enum, we throw an exception
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type (Enum)");
            }

            // We define a local integer
            int taskToChoose;
            // We calculate a random value, based on the Enum length
            taskToChoose = UnityEngine.Random.Range(0, Enum.GetValues(typeof(T)).Length);
            // We cast and select then the value and return the enum
            //return (T) Convert.ChangeType(taskToChoose, typeof(T));
            return (T)(object)taskToChoose;

        }
    }

    /// <summary>
    /// Class containing tools and methods to calculate points
    /// </summary>
    public static class Points
    {
        /// <summary>
        /// The function returns the opposite point to an object based on a direction, determined by a radius
        /// </summary>
        /// <param name="objectPosition"> The actual position of the object to work with</param>
        /// <param name="directionToDangerPoint"> The normalized direction from where the danger is coming from</param>
        /// <param name="radiusForNewPoint"> The radius to calculate the new point</param>
        /// <returns></returns>
        public static Vector3 CalculateOppositePoint(Vector3 objectPosition, Vector3 directionToDangerPoint, float radiusForNewPoint)
        {
            // We calculate the new point, adding the normalized vector times the radius to the current object position
            Vector3 auxOppositePoint = objectPosition + (-directionToDangerPoint * radiusForNewPoint);

            return auxOppositePoint;
        }

        /// <summary>
        /// Returns the furthest point from a position, given the position and the points array
        /// </summary>
        /// <param name="objectPosition"> The position of the object we want to check</param>
        /// <param name="points"> The array of points to get the furthest one</param>
        /// <returns> The furthest away point</returns>
        public static Vector3 CalculateFurthestPoint(Vector3 objectPosition, Vector3[] points)
        {
            // If there are any points...
            if (points.Length > 0)
            {
                // We declare the point to return
                Vector3 pointToReturn = Vector3.one;
                // We declare the furthest distance that the point has (closest is 0)
                float furthestDistance = 0f;
                // We iterate over our points array
                for (int i = 0; i < points.Length; i++)
                {
                    // If the point is not inside our furthestDistance radius...
                    if (!Vectors.CheckDistance(Vectors.CalculateDirection(objectPosition, points[i]), furthestDistance))
                    {
                        //... It is further away! We select it as our furthest point
                        pointToReturn = points[i];
                        // We update our furthestDistance with the distance to this point
                        furthestDistance = Vector3.Distance(objectPosition, pointToReturn);
                    }
                }
                //// We debug the distance in the inspector
                //Debug.Log("Furthest point calculates is " + furthestDistance.ToString() + " units far away.");
                // We return the furthest point
                return pointToReturn;
            }
            // If there are no points passed in...
            else
            {
                // We show an error in the inspector
                Debug.LogError("There are no points to calculate the furthest from!");
                // We return zero
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Returns the furthest point from a position, given the position and the transforms array
        /// </summary>
        /// <param name="objectPosition"> The position of the object we want to check</param>
        /// <param name="points"> The array of transforms to get the furthest point from</param>
        /// <returns> The furthest away point</returns>
        public static Vector3 CalculateFurthestPoint(Vector3 objectPosition, Transform[] transforms)
        {
            // We declare an array of points with as many points as transforms are passed in
            Vector3[] pointsFromTransform = new Vector3[transforms.Length];
            // We iterate over the array of points and assign each point the correspondant transform.position
            for (int i = 0; i < pointsFromTransform.Length; i++)
            {
                // We assign each point the correspondant transform.position
                pointsFromTransform[i] = transforms[i].position;
            }

            // We call CalculateFurthestPoint (vector3, vector3[])
            return CalculateFurthestPoint(objectPosition, pointsFromTransform);
        }

    }

    /// <summary>
    /// Class containing tools and methods for performing mathematical statistical conversions
    /// </summary>
    public static class Normalization
    {
        /// <summary>
        /// Returns a normalized value depending on a min and max
        /// </summary>
        /// <param name="value"> The current value to normalize </param>
        /// <param name="min"> The min of that value </param>
        /// <param name="max"> The max of that value </param>
        /// <returns> The normalized value calculated </returns>
        public static float Normalize(float value, float min, float max)
        {
            // We calculate the normalize value of the value passed in
            return (value - min) / (max - min);
        }

        /// <summary>
        /// Returns a denormalized value depending on a min and max
        /// </summary>
        /// <param name="valueNormalized"> The current value to denormalize </param>
        /// <param name="min"> The min of that value </param>
        /// <param name="max"> The max of that value </param>
        /// <returns> The denormalized value calculated </returns>
        public static float Denormalize(float valueNormalized, float min, float max)
        {
            // We calculate the denormalized value of the value passed in
            return (valueNormalized * (max - min) + min);
        }

        /// <summary>
        /// Returns a scaled value between a mix and a max from a normalized value passed in
        /// </summary>
        /// <param name="normalizedValue"> The normalize value to scale</param>
        /// <param name="minToScale"> The minimum the scalation can reach</param>
        /// <param name="maxToScale"> The maximum the scalation can reach</param>
        /// <returns></returns>
        public static float ScaleNormalize(float normalizedValue, float minToScale, float maxToScale)
        {
            return (normalizedValue * (maxToScale - minToScale) + minToScale);
        }
    }

    /// <summary>
    /// Class containing tools and methods for performing mathematical vector calculations
    /// </summary>
    public static class Vectors
    {
        /// <summary>
        /// Calculates the direction from a position to another
        /// </summary>
        /// <param name="fromPos"> The origin position</param>
        /// <param name="toPos"> The destination position</param>
        /// <returns> The non-normalized vector direction</returns>
        public static Vector3 CalculateDirection(Vector3 fromPos, Vector3 toPos)
        {
            // We calculate the direction to the vector toPos
            return (toPos - fromPos);
        }

        /// <summary>
        /// Checks the distance to an object, to see if it is within radius (this is a fast way of doing it)
        /// </summary>
        /// <param name="direction"> The non-normalized direction vector</param>
        /// <param name="maxRange"> The maximumDistance (radius) to check</param>
        /// <returns> True if the direction is below the maxRange</returns>
        public static bool CheckDistance(Vector3 direction, float maxRange)
        {
            // If the sqrMagnitude of the direction is lower than the squares distance ...
            // The sqrMagnitude property gives the square of the magnitude value, and is calculated like the magnitude but without the time-consuming square root operation
            if (direction.sqrMagnitude < maxRange * maxRange)
            {
                return true;
            }
            // If it is above...
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Search for the closest object with the provided tag within a radius
        /// </summary>
        /// <param name="origin"> The origin from where to search</param>
        /// <param name="radius"> The radius of the search</param>
        /// <param name="tagToSearch"> The tag of the object to find</param>
        /// <returns></returns>
        public static GameObject SearchClosestObjectWihTag(Vector3 origin, float radius, string tagToSearch)
        {
            // We create the helper array that will contain all the close colliders
            Collider[] allCollidersArray = null;

            // The GameObject to return
            GameObject result = null;

            // To see how many objects are counted
            int aux;

            // We create a sphere with the origin and radius provided and get all colliders hitted by it
            //int aux = Physics.OverlapSphereNonAlloc(origin, radius, allCollidersArray);
            allCollidersArray = Physics.OverlapSphere(origin, radius);
            aux = allCollidersArray.Length;

            // We only run this part if there is any object found
            if (aux > 0 && allCollidersArray != null)
            {
                // We define the distance variable to check the object
                float distanceToObject = 0f;
                // We define the actual distance of the closest found object
                float closestObjectDistance = 0f;

                // We go through all the colliders found and save the ones we want to the return colliders array
                for (int i = 0; i < allCollidersArray.Length; i++)
                {
                    //Debug.Log("Searching for " + tagToSearch + " ...");
                    if (allCollidersArray[i].CompareTag(tagToSearch))
                    {
                        // We calculate the distance to the found object
                        distanceToObject = Vector3.Distance(origin, allCollidersArray[i].transform.position);
                        // If there are any other closest object, we set the actual distance as the closest
                        if (closestObjectDistance == 0f)
                        {
                            closestObjectDistance = distanceToObject;
                        }
                        // We check if the distance is below or equal the closestDistance found
                        if (distanceToObject <= closestObjectDistance)
                        {
                            // If it is, we set this object as the result to return and update the closestDistance found
                            result = allCollidersArray[i].gameObject;
                            closestObjectDistance = distanceToObject;
                            //Debug.Log(tagToSearch + " found!");
                        }
                    }
                }
            }

            // Debug the amount of colliders found
            //Debug.Log(aux.ToString() + "Colliders found");



            // We return the objects
            return result;
        }

        /// <summary>
        /// Returns the absolute vector of vectorToAbs
        /// </summary>
        /// <param name="vectorToAbs"> The vector to get the absolute from</param>
        /// <returns> The absolute vector</returns>
        public static Vector3 Abs(Vector3 vectorToAbs)
        {
            vectorToAbs.x = Mathf.Abs(vectorToAbs.x);
            vectorToAbs.y = Mathf.Abs(vectorToAbs.y);
            vectorToAbs.z = Mathf.Abs(vectorToAbs.z);
            return vectorToAbs;
        }
    }

    /// <summary>
    /// Class containing tools and methods for dealing with Arrays
    /// </summary>
    public static class Arrays
    {
        /// <summary>
        /// Set all the values in the array to the valueToSet
        /// </summary>
        /// <typeparam name="T"> The type of the array </typeparam>
        /// <param name="arrayToSet"> The array we want to change </param>
        /// <param name="valueToSet"> The value we want to set in every position of the array </param>
        public static void SetAllArray<T>(ref T[] arrayToSet, T valueToSet)
        {
            // We go through all the array
            for (int i = 0; i < arrayToSet.Length; i++)
            {
                // We set each position of the array to the value we want
                arrayToSet[i] = valueToSet;
            }
        }

        /// <summary>
        /// SetActive all the values in the array to the specified value
        /// </summary>
        /// <typeparam name="T"> The type of the array </typeparam>
        /// <param name="arrayToSet"> The array we want to change </param>
        /// <param name="valueToSet"> The value we want to set in every position of the array </param>
        public static void SetActiveAllArray<T>(ref T[] arrayToSet, bool valueToSet)
        {

            // We don't execute if the array is 0
            if (arrayToSet.Length == 0)
            {
                return;
            }

            //Debug.Log("Setting array " + arrayToSet.ToString() + " to " + valueToSet.ToString());
            // We evaluate the first component to see what type the array is
            // If it is a Behaviour (almost every component)...
            if (arrayToSet[0] is Behaviour)
            {
                // ... we go through all the array
                for (int i = 0; i < arrayToSet.Length; i++)
                {
                    // We set each position of the array to the value we want                
                    (arrayToSet[i] as Behaviour).enabled = valueToSet;
                }

            }
            // If it is a GameObject ...
            else if (arrayToSet[0] is GameObject)
            {
                // ... we go through all the array
                for (int i = 0; i < arrayToSet.Length; i++)
                {
                    // We set each position of the array to the value we want                
                    (arrayToSet[i] as GameObject).SetActive(valueToSet);
                }
            }
            // If it is a Collider ...
            else if (arrayToSet[0] is Collider)
            {
                // ... we go through all the array
                for (int i = 0; i < arrayToSet.Length; i++)
                {
                    // We set each position of the array to the value we want                
                    (arrayToSet[i] as Collider).enabled = valueToSet;
                }
            }
            // If it is a Renderer...
            else if (arrayToSet[0] is Renderer)
            {
                // ... we go through all the array
                for (int i = 0; i < arrayToSet.Length; i++)
                {
                    // We set each position of the array to the value we want                
                    (arrayToSet[i] as Renderer).enabled = valueToSet;
                }
            }

        }

        /// <summary>
        /// Checks if all the values of the array are matching valueToGet
        /// </summary>
        /// <typeparam name="T"> The type of the array</typeparam>
        /// <param name="arrayToGet"> The array we want to check</param>
        /// <param name="valueToGet"> The value we want to ger in every position of the array</param>
        public static bool GetActiveAllArray<T>(ref T[] arrayToGet, bool valueToGet)
        {
            // We declare the result to return and intialize it false
            bool result = false;
            // If it is a Behaviour (almost every component)...
            if (arrayToGet[0] is Behaviour)
            {
                // ... we go through all the array
                for (int i = 0; i < arrayToGet.Length; i++)
                {
                    // We get each position of the array and compare it to the value we want   
                    // If the value matches...                                 
                    if ((arrayToGet[i] as Behaviour).enabled == valueToGet)
                    {
                        // ...We set result true and continue
                        result = true;
                    }
                    // If the value does not match...
                    else
                    {
                        // ... We set result false and return it, stopping the method call
                        result = false;
                        return result;
                    }
                }

            }
            // If it is a GameObject ...
            else if (arrayToGet[0] is GameObject)
            {
                // ... we go through all the array
                for (int i = 0; i < arrayToGet.Length; i++)
                {
                    // We get each position of the array and compare it to the value we want   
                    // If the value matches...                                 
                    if ((arrayToGet[i] as GameObject).activeSelf == valueToGet)
                    {
                        // ...We set result true and continue
                        result = true;
                    }
                    // If the value does not match...
                    else
                    {
                        // ... We set result false and return it, stopping the method call
                        result = false;
                        return result;
                    }
                }
            }
            // If it is a Collider ...
            else if (arrayToGet[0] is Collider)
            {
                // ... we go through all the array
                for (int i = 0; i < arrayToGet.Length; i++)
                {
                    // We get each position of the array and compare it to the value we want   
                    // If the value matches...                                 
                    if ((arrayToGet[i] as Collider).enabled == valueToGet)
                    {
                        // ...We set result true and continue
                        result = true;
                    }
                    // If the value does not match...
                    else
                    {
                        // ... We set result false and return it, stopping the method call
                        result = false;
                        return result;
                    }
                }
            }
            // If it is a Renderer...
            else if (arrayToGet[0] is Renderer)
            {
                // ... we go through all the array
                for (int i = 0; i < arrayToGet.Length; i++)
                {
                    // We get each position of the array and compare it to the value we want   
                    // If the value matches...                                 
                    if ((arrayToGet[i] as Renderer).enabled == valueToGet)
                    {
                        // ...We set result true and continue
                        result = true;
                    }
                    // If the value does not match...
                    else
                    {
                        // ... We set result false and return it, stopping the method call
                        result = false;
                        return result;
                    }
                }
            }

            // We return the result
            return result;
        }

        /// <summary>
        /// Finds GameObjects with components as children to the parent passed in, adding them to an array passed in
        /// </summary>
        /// <typeparam name="T"> The type of the component to find</typeparam>
        /// <param name="arrayToPopulate"> The array we want to populate with GameObjects containing T</param>
        /// <param name="parent">The parent potentially having the children</param>
        public static void FindComponentAsChildren<T>(ref GameObject[] arrayToPopulate, GameObject parent)
        {
            // We initialize a new temporary list of components
            List<T> componentList = new List<T>();
            // We fill the list from the children of the current object
            parent.GetComponentsInChildren<T>(true, componentList);
            // If we found any...
            if (componentList.Count > 0)
            {
                // We then copy this list to our component field 
                // We resize the array 
                Array.Resize<GameObject>(ref arrayToPopulate, componentList.Count);
                // We copy each result as a gameObject to the component array
                for (int i = 0; i < componentList.Count; i++)
                {
                    arrayToPopulate[i] = (componentList[i] as Behaviour).gameObject;
                }
            }
        }

        /// <summary>
        /// Finds Components as children to the parent passed in, adding them to an array passed in
        /// </summary>
        /// <typeparam name="T"> The type of the component to find</typeparam>
        /// <param name="arrayToPopulate"> The array we want to populate with Components of type T</param>
        /// <param name="parent">The parent potentially having the children</param>
        public static void FindComponentAsChildren<T>(ref T[] arrayToPopulate, GameObject parent)
        {
            // We initialize a new temporary list of components
            List<T> componentList = new List<T>();
            // We fill the list from the children of the current object
            parent.GetComponentsInChildren<T>(true, componentList);
            // If we found any...
            if (componentList.Count > 0)
            {
                // We then copy this list to our component field 
                // We resize the array 
                Array.Resize<T>(ref arrayToPopulate, componentList.Count);
                // We copy each result as a gameObject to the component array
                for (int i = 0; i < componentList.Count; i++)
                {
                    arrayToPopulate[i] = componentList[i];
                }
            }
        }


        public static void FindComponentAsChildren<T>(ref T[] arrayToPopulate, GameObject parent, string tagToCompare)
        {
            // We initialize a new temporary list of components
            List<T> componentList = new List<T>();
            // We fill the list from the children of the current object
            parent.GetComponentsInChildren<T>(true, componentList);
            // If we found any...
            if (componentList.Count > 0)
            {
                // We remove the entries that are not matching with tagToCompare 
                // We use a lamda expression (x goes to NOT compareTag(tagToCompare))                                
                componentList.RemoveAll(x => !(x as Component).CompareTag(tagToCompare));
                // We then copy this list to our component field 
                // We resize the array 
                Array.Resize<T>(ref arrayToPopulate, componentList.Count);
                // We copy each result as a gameObject to the component array
                for (int i = 0; i < componentList.Count; i++)
                {
                    arrayToPopulate[i] = componentList[i];
                }
            }
        }
    }

    /// <summary>
    /// Class containing tools and methods for dealing with Arrays
    /// </summary>
    public static class Lists
    {
        /// <summary>
        /// Checks if a list is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToCheck"></param>
        /// <returns>True if Null or Empty. False Otherwise.</returns>
        public static bool IsNullOrEmpty<T>(ref List<T> listToCheck)
        {
            // If the list is not empty or does have any elements return false
            if ((listToCheck != null) && (listToCheck.Count != 0))
            {
                return false;
            }
            // If it is empty or doesn't have any elements return true
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Resizes a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sz"></param>
        /// <param name="c"></param>
        /// <param name="destroyItems">Destroy items on removal</param>
        public static void Resize<T>(this List<T> list, int sz, T c = default(T), bool destroyItems = false)
        {
            int cur = list.Count;
            if (sz < cur)
            {
                // Will attempt to destroy the objects during removal
                if (destroyItems)
                {
                    var itemsToDestroy = list.GetRange(sz, cur - sz);
                    // Loop all the objects. 
                    for (int i = 0; i < itemsToDestroy.Count; i++)
                    {
                        var unityObject = itemsToDestroy[i] as UnityEngine.Object;
                        // If it is an unity object, we call unity's destroy function
                        if (unityObject)
                        {
                            // If it is a component, we attempt to destroy the gameObject containing it
                            if (unityObject is Component)
                            {
                                UnityEngine.Object.Destroy((unityObject as Component).gameObject);
                            }
                            // If it is not, we just destroy whatever it is
                            else
                            {
                                UnityEngine.Object.Destroy(unityObject);
                            }
                        }

                        // If not, we set to default value
                        else
                            itemsToDestroy[i] = default(T);

                    }
                }               
                // Remove the range now
                list.RemoveRange(sz, cur - sz);
            }
            else if (sz > cur)
            {
                list.AddRange(Enumerable.Repeat(c, sz - cur));
            }
        }

        /// <summary>
        /// Casts a list of type T to another of type U
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<U> CastNewList<T, U>(this List<T> list) where U: class
        {
            List<U> newList = new List<U>();
            foreach (var item in list)
            {
                // Attempt to cast item
                var newItem = item as U;
                // If cast failed, throw an exception
                if (newItem == null)
                    throw new Exception("List cast is invalid. " + typeof(T).ToString() + " can't be casted to " + typeof(U).ToString() );
                // If cast didn't fail, add item to list
                newList.Add(newItem);
            }
            return newList;
        }
    }

    /// <summary>
    /// Helper class to deal with game components
    /// </summary>
    public static class GameComponents
    {
        /// <summary>
        /// Finds a component in the object passed by
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSearch"></param>
        /// <param name="componentToFind"></param>
        /// <returns></returns>
        public static object FindComponent(GameObject objectToSearch, Component componentToFind)
        {
            object componentFound = null;
            Type componentType = componentToFind.GetType();

            componentFound = objectToSearch.GetComponent(componentType);

            return componentFound;
        }

        /// <summary>
        /// Tries to find the component in the Gobject passed in. If not, it adds it to the GObject
        /// </summary>
        /// <param name="objectToSearch"></param>
        /// <param name="componentToCheck"></param>
        public static void CreateIfNull(GameObject objectToSearch, Component componentToCheck)
        {
            if (componentToCheck == null)
            {
                // Try and find the component in the game object
                componentToCheck = (Component)FindComponent(objectToSearch, componentToCheck);

                // If it is still null, we didn't find it, so we go and create it
                if (componentToCheck == null)
                    componentToCheck = objectToSearch.AddComponent(componentToCheck.GetType());

            }
        }
    }

    /// <summary>
    /// Helper class to deal with Floats
    /// </summary>
    public static class Floats
    {
        public static bool NearlyEqual(float a, float b, float epsilon)
        {
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == b)
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || absA + absB < float.MinValue)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * float.MinValue);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

    }

    public static class Types
    {
        /// <summary>
        /// Checks if a class is derived from a generic class
        /// </summary>
        /// <param name="generic"></param>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        public static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
