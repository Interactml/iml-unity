using UnityEngine;
using System.Collections;


namespace MalbersAnimations
{
    /// <summary>
    /// Simple Script to make damage to every Animal
    /// </summary>
    public class AttackTrigger : MonoBehaviour
    {
        public int index = 1;
        public float damageMultiplier = 1;
        public float PushForce = 0;

        private Animal myAnimal;
        private IMDamagable enemy;
        private Collider _collider;


        public bool debug = true;
        public Color DebugColor = new Color(1, 0.25f, 0, 0.15f);

        public Collider Collider
        {
            get
            {
                if (!_collider)
                {
                    _collider = GetComponent<Collider>(); ;
                }
                return _collider;
            }
        }

       public void Start()
        {
            myAnimal = transform.GetComponentInParent<Animal>();                //Get the Animal Component on the Root of this Animal


            if (Collider)
            {
                Collider.isTrigger = true;
                Collider.enabled = false;
            }
            else
            {
                Debug.LogWarning(name + " needs a Collider so 'AttackTrigger' can function correctly");
            }

            gameObject.SetActive(false);        //Hide this GO
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger) return;                                                //just collapse when is a collider what we are hitting
            enemy = other.GetComponentInParent<IMDamagable>();                          //Get the Animal on the Other collider

            if (enemy != null) //if the other does'nt have the animal script skip
            {
               
                if (myAnimal.GetComponent<IMDamagable>() == enemy) return;                      //Don't Hit yourself

                Vector3 direction = myAnimal.transform.position - other.bounds.center;       //Calculate the direction of the attack

                DamageValues DV = new DamageValues(direction, damageMultiplier * (myAnimal ? myAnimal.attackStrength : 1));

             

                enemy.getDamaged(DV);
            }
            else
            {
                if (other.attachedRigidbody && PushForce != 0)        //If the other has a riggid body and it can be pushed
                {
                    other.attachedRigidbody.AddForce((other.transform.position- transform.position).normalized * PushForce, ForceMode.VelocityChange);
                }
            }
        }



        void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = DebugColor;
                Gizmos.matrix = transform.localToWorldMatrix;

                if (Collider && Collider.enabled)
                {
                    if (Collider is BoxCollider)
                    {
                        BoxCollider _C = Collider as BoxCollider;
                        if (!_C.enabled) return;
                        var sizeX = transform.lossyScale.x * _C.size.x;
                        var sizeY = transform.lossyScale.y * _C.size.y;
                        var sizeZ = transform.lossyScale.z * _C.size.z;
                        Matrix4x4 rotationMatrix = Matrix4x4.TRS(_C.bounds.center, transform.rotation, new Vector3(sizeX, sizeY, sizeZ));

                        Gizmos.matrix = rotationMatrix;
                        Gizmos.DrawCube(Vector3.zero, Vector3.one);
                        Gizmos.color = new Color(DebugColor.r, DebugColor.g, DebugColor.b, 1f);
                        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    }
                    else if (Collider is SphereCollider)
                    {
                        SphereCollider _C = Collider as SphereCollider;

                        if (!_C.enabled) return;

                        Gizmos.matrix = transform.localToWorldMatrix;

                        Gizmos.DrawSphere(Vector3.zero + _C.center, _C.radius);
                        Gizmos.color = new Color(DebugColor.r, DebugColor.g, DebugColor.b, 1f);
                        Gizmos.DrawWireSphere(Vector3.zero + _C.center, _C.radius);
                    }
                }
            }



        }



    }
}

