using UnityEngine;
using UnityEngine.AI;

namespace FIMSpace.GroundFitter
{
    public class FGroundFitter_Demo_NavMeshInput : MonoBehaviour
    {
        public NavMeshAgent TargetAgent;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (TargetAgent)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    RaycastHit hit;
                    if (Physics.Raycast(ray.origin, ray.direction, out hit))
                    {
                        NavMeshHit navMeshHit;
                        if (NavMesh.SamplePosition(hit.point, out navMeshHit, 1, 1))
                        {
                            TargetAgent.SetDestination(navMeshHit.position);
                        }
                    }
                }
            }
        }
    }
}