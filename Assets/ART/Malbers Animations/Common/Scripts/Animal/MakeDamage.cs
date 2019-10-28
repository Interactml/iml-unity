using UnityEngine;
using System.Collections;


namespace MalbersAnimations
{
    /// <summary>
    /// Simple Script to make damage to every Animal
    /// </summary>
    public class MakeDamage : MonoBehaviour
    {
        public float damageMultiplier = 1;

        private Collider _collider;

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

        void Start()
        {
            if (Collider)
            {
                Collider.isTrigger = true;
            }
            else
            {
                Debug.LogWarning(name + " needs a Collider so 'AttackTrigger' can function correctly");
            }
        }


        void OnTriggerEnter(Collider other)
        {
            if (other.transform.root == transform.root) return;                      //Don't hit yourself

            Vector3 direction = -other.bounds.center + Collider.bounds.center;

            DamageValues DV = new DamageValues(direction, damageMultiplier);

            if (other.isTrigger) return; // just collapse when is a collider what we are hitting

            IMDamagable Enemy = other.GetComponentInParent<IMDamagable>();

            if (Enemy != null)
            {
                Enemy.getDamaged(DV);
            }
        }
    }
}
