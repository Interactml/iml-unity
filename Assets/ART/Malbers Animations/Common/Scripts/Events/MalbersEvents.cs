using UnityEngine;
using System;
using UnityEngine.Events;

 namespace MalbersAnimations.Events
{
    /// <summary>
    /// Serializable Dynamic UnityEvents
    /// </summary>
    [Serializable]
    public class GameObjectEvent : UnityEvent<GameObject>
    {}

    [Serializable]
    public class TransformEvent : UnityEvent<Transform>
    {}

    [Serializable]
    public class RayCastHitEvent : UnityEvent<RaycastHit>
    {}

    [Serializable]
    public class Vector3Event : UnityEvent<Vector3>
    { }

    [Serializable]
    public class IntEvent : UnityEvent<int>
    {}

    [Serializable]
    public class FloatEvent : UnityEvent<float>
    { }

    [Serializable]
    public class BoolEvent : UnityEvent<bool>
    { }

    [Serializable]
    public class StringEvent : UnityEvent<string>
    { }

    [Serializable]
    public class ColliderEvent : UnityEvent<Collider>
    { }

    [Serializable]
    public class CollisionEvent : UnityEvent<Collision>
    { }

    [Serializable]
    public class ComponentEvent : UnityEvent<Component>
    { }
}