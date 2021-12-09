namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    static public class NewUtility
    {
        // Various
        static public float LerpHalfTimeToAlpha(float lerpHalfTime)
        {
            if (lerpHalfTime == 0f)
                return 1f;

            float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
            return 1f - 0.5f.Pow(deltaTime / lerpHalfTime);
        }
        static public Vector2 PointOnCircle(float angle, float radius = 1f, bool inDegrees = true)
        {
            if (inDegrees)
                angle *= Mathf.Deg2Rad;
            return new Vector2(radius * Mathf.Cos(angle), radius * -Mathf.Sin(angle));
        }
        static public Vector3 PointOnCircleClosestTo(Vector3 point, Transform circleTransform = null)
        {
            if (circleTransform != null)
                point = point.Untransform(circleTransform);
            point = point.XY().normalized;
            if (circleTransform != null)
                point = point.Transform(circleTransform);
            return point;
        }
        static public bool Roll(float chance = 0.5f)
        => Random.value < chance;
        static public bool Flip()
        => Roll(0.5f);
        static public RaycastHit[] RaycastFromCamera(Camera camera, int layerMask, bool hitTriggers, bool sortByDistance)
        {
            QueryTriggerInteraction triggerInteraction = hitTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;
            RaycastHit[] hits = Physics.RaycastAll(camera.CursorRay(), float.PositiveInfinity, layerMask, triggerInteraction);
            if (sortByDistance)
                hits = hits.SortedByDistanceFrom(camera);
            return hits;
        }
    }
}