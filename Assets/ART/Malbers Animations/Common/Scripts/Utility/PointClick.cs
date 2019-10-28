using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using MalbersAnimations.Events;
using UnityEngine.AI;

namespace MalbersAnimations.Utilities
{
    public class PointClick : MonoBehaviour
    {
        private const float navMeshSampleDistance = 4f;
        public Vector3Event OnPointClick = new Vector3Event();
        public GameObjectEvent OnInteractableClick = new GameObjectEvent();
    

        Vector3 destinationPosition;

        public void OnGroundClick(BaseEventData data)   
        {
            PointerEventData pData = (PointerEventData)data;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(pData.pointerCurrentRaycast.worldPosition, out hit, navMeshSampleDistance, NavMesh.AllAreas))
                destinationPosition = hit.position;
            else
                destinationPosition = pData.pointerCurrentRaycast.worldPosition;

            OnPointClick.Invoke(destinationPosition);
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;

                Gizmos.DrawWireSphere(destinationPosition, 0.1f);
                Gizmos.DrawSphere(destinationPosition, 0.1f);
            }
        }
    }
}