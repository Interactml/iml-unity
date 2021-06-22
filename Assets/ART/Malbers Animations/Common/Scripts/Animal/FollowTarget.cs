using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// Simple follow target for the animal
    /// </summary>
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public float stopDistance = 3;
        Animal animal;

        // Use this for initialization
        void Start()
        {
            animal = GetComponentInParent<Animal>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 Direction = target.position - transform.position;               //Calculate the direction from the animal to the target
            float distance = Vector3.Distance(transform.position, target.position); //Calculate the distance..

            animal.Move(distance > stopDistance ? Direction : Vector3.zero);        //Move the Animal if we are not on the Stop Distance Radius
        }

        private void OnDisable()
        {
            animal.Move(Vector3.zero);      //In case this script gets disabled stop the movement of the Animal
        }
    }
}