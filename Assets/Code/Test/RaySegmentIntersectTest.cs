namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using Vheos.Tools.Extensions.Math;

    public class RaySegmentIntersectTest : MonoBehaviour
    {
        [SerializeField] protected Vector3 _RayA;
        [SerializeField] protected Segment2D _B;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Ray2D rayA = new(_RayA.XY(), Vector2.right.Rotate(_RayA.z, true));
            Gizmos.DrawLine(rayA.origin, rayA.origin + rayA.direction * 100f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(_B.From, _B.To);


            if (NewUtility.IntersectRayAndSegment(rayA, _B, out var intersectionPoint))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(intersectionPoint, 0.05f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(rayA.origin, 0.05f);
            }
        }
    }
}