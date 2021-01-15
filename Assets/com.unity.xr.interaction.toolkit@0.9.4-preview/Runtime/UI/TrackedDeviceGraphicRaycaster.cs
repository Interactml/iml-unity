using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
    public class TrackedDeviceGraphicRaycaster : BaseRaycaster
    {
        static readonly int k_MaxRaycastHits = 10;
        private struct RaycastHitData
        {
            public RaycastHitData(Graphic graphic, Vector3 worldHitPosition, Vector2 screenPosition, float distance)
            {
                this.graphic = graphic;
                this.worldHitPosition = worldHitPosition;
                this.screenPosition = screenPosition;
                this.distance = distance;
            }

            public Graphic graphic { get; set; }
            public Vector3 worldHitPosition { get; set; }
            public Vector2 screenPosition { get; set; }
            public float distance { get; set; }
        }

        private class RaycastHitComparer : IComparer<RaycastHitData>
        {
            public int Compare(RaycastHitData a, RaycastHitData b)
            {
                int result = b.graphic.depth.CompareTo(a.graphic.depth);
                return result;
            }
        }

        [SerializeField]
        private bool m_IgnoreReversedGraphics = false;

        [SerializeField]
        private bool m_CheckFor2DOcclusion = false;

        [SerializeField]
        private bool m_CheckFor3DOcclusion = false;

        [SerializeField]
        private LayerMask m_BlockingMask = int.MaxValue;

        private Canvas m_Canvas;

        private RaycastHit[] occlusionHits3D = new RaycastHit[k_MaxRaycastHits];
        private RaycastHit2D[] occlusionHits2D = new RaycastHit2D[k_MaxRaycastHits];
        private RaycastHitComparer m_raycastHitComparer = new RaycastHitComparer();

        private Canvas canvas
        {
            get
            {
                if (m_Canvas != null)
                    return m_Canvas;

                m_Canvas = GetComponent<Canvas>();
                return m_Canvas;
            }
        }


        public override Camera eventCamera
        {
            get
            {
                var myCanvas = canvas;
                return myCanvas != null ? myCanvas.worldCamera : null;
            }
        }

        /// <summary>Perform a raycast against objects within this Raycaster's domain.</summary>
        /// <param name="eventData">Data containing where and how to raycast.</param>
        /// <param name="resultAppendList">The resultant hits from the raycast.</param>
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            var trackedEventData = eventData as TrackedDeviceEventData;
            if (trackedEventData != null)
            {
                PerformRaycasts(trackedEventData, resultAppendList);
            }
        }

        // Use this list on each raycast to avoid continually allocating.
        private List<RaycastHitData> m_RaycastResultsCache = new List<RaycastHitData>();

        static RaycastHit FindClosestHit(RaycastHit[] hits, int count)
        {
            int index = 0;
            var distance = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                if (hits[i].distance < distance)
                {
                    distance = hits[i].distance;
                    index = i;
                }
            }

            return hits[index];
        }

        static RaycastHit2D FindClosestHit(RaycastHit2D[] hits, int count)
        {
            int index = 0;
            var distance = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                if (hits[i].distance < distance)
                {
                    distance = hits[i].distance;
                    index = i;
                }
            }

            return hits[index];
        }
        private void PerformRaycasts(TrackedDeviceEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (canvas == null)
                return;

            if (eventCamera == null)
                return;

            var rayPoints = eventData.rayPoints;
            var layerMask = eventData.layerMask;
            for(int i = 1; i < rayPoints.Count; i++)
            {
                var from = rayPoints[i - 1];
                var to = rayPoints[i];
                if (PerformRaycast(from, to, layerMask, resultAppendList))
                {
                    eventData.rayHitIndex = i;
                    break;
                }
            }
        }

        private RaycastHit[] physicsHits = new RaycastHit[1];
        private RaycastHit2D[] graphicHits = new RaycastHit2D[1];

        bool PerformRaycast(Vector3 from, Vector3 to, LayerMask layerMask, List<RaycastResult> resultAppendList)
        {
            bool hitSomething = false;

            float rayDistance = Vector3.Distance(to, from);
            Ray ray = new Ray(from, (to - from).normalized * rayDistance);
             
            var hitDistance = rayDistance;
            if (m_CheckFor3DOcclusion)
            {
                var hitCount = Physics.RaycastNonAlloc(ray, occlusionHits3D, hitDistance, m_BlockingMask);

                if (hitCount > 0)
                {                    var hit = FindClosestHit(occlusionHits3D, hitCount);
                    hitDistance = hit.distance;
                    hitSomething = true;                }
            }

            if (m_CheckFor2DOcclusion)
            {
                var raycastDistance = hitDistance;
                var hitCount = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, occlusionHits2D, m_BlockingMask);
                
                if (hitCount > 0)
                {                    var hit = FindClosestHit(occlusionHits2D, hitCount);
                    hitDistance = hit.distance > hitDistance ? hitDistance : hit.distance;
                    hitSomething = true;
                }
            }

            m_RaycastResultsCache.Clear();
            SortedRaycastGraphics(canvas, ray, hitDistance, layerMask, m_RaycastResultsCache);

            //Now that we have a list of sorted hits, process any extra settings and filters.
            for (var i = 0; i < m_RaycastResultsCache.Count; i++)
            {
                var validHit = true;

                var hitData = m_RaycastResultsCache[i];

                var go = hitData.graphic.gameObject;
                if (m_IgnoreReversedGraphics)
                {
                    var forward = ray.direction;
                    var goDirection = go.transform.rotation * Vector3.forward;
                    validHit = Vector3.Dot(forward, goDirection) > 0;
                }

                validHit &= hitData.distance < hitDistance;

                if (validHit)
                {
                    Transform trans = go.transform;
                    Vector3 transForward = trans.forward;
                    var castResult = new RaycastResult
                    {
                        gameObject = go,
                        module = this,
                        index = resultAppendList.Count,
                        distance = hitData.distance,
                        depth = hitData.graphic.depth,
                        sortingLayer = canvas.sortingLayerID,
                        sortingOrder = canvas.sortingOrder,
                        worldPosition = hitData.worldHitPosition,
                        worldNormal = -transForward
                    };
                    resultAppendList.Add(castResult);

                    hitSomething = true;
                }
            }

            return hitSomething;
        }

        [NonSerialized]
        static readonly List<RaycastHitData> s_SortedGraphics = new List<RaycastHitData>();
        private void SortedRaycastGraphics(Canvas canvas, Ray ray, float maxDistance, LayerMask layerMask, List<RaycastHitData> results)
        {
            var graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);

            s_SortedGraphics.Clear();
            for (int i = 0; i < graphics.Count; ++i)
            {
                Graphic graphic = graphics[i];

                if (graphic.depth == -1)
                    continue;

                if (((1 << graphic.gameObject.layer) & layerMask) == 0)
                    continue;

                Vector3 worldPos;
                float distance;
                if (RayIntersectsRectTransform(graphic.rectTransform, ray, out worldPos, out distance))
                {
                    if(distance <= maxDistance)
                    {
                        Vector2 screenPos = eventCamera.WorldToScreenPoint(worldPos);
                        // mask/image intersection - See Unity docs on eventAlphaThreshold for when this does anything
                        if (graphic.Raycast(screenPos, eventCamera))
                        {
                            s_SortedGraphics.Add(new RaycastHitData(graphic, worldPos, screenPos, distance));
                        }
                    }      
                }
            }
            SortingHelpers.Sort(s_SortedGraphics, m_raycastHitComparer);
            results.AddRange(s_SortedGraphics);
        }

        static Vector3[] s_Corners = new Vector3[4];

        private bool RayIntersectsRectTransform(RectTransform transform, Ray ray, out Vector3 worldPosition, out float distance)
        {
            transform.GetWorldCorners(s_Corners);
            var plane = new Plane(s_Corners[0], s_Corners[1], s_Corners[2]);

            float enter;
            if (plane.Raycast(ray, out enter))
            {
                var intersection = ray.GetPoint(enter);

                var bottomEdge = s_Corners[3] - s_Corners[0];
                var leftEdge = s_Corners[1] - s_Corners[0];
                var bottomDot = Vector3.Dot(intersection - s_Corners[0], bottomEdge);
                var leftDot = Vector3.Dot(intersection - s_Corners[0], leftEdge);

                // If the intersection is right of the left edge and above the bottom edge.
                if (leftDot >= 0 && bottomDot >= 0)
                {
                    var topEdge = s_Corners[1] - s_Corners[2];
                    var rightEdge = s_Corners[3] - s_Corners[2];
                    var topDot = Vector3.Dot(intersection - s_Corners[2], topEdge);
                    var rightDot = Vector3.Dot(intersection - s_Corners[2], rightEdge);

                    //If the intersection is left of the right edge, and below the top edge
                    if (topDot >= 0 && rightDot >= 0)
                    {
                        worldPosition = intersection;
                        distance = enter;
                        return true;
                    }
                }
            }
            worldPosition = Vector3.zero;
            distance = 0;
            return false;
        }
    }
}
