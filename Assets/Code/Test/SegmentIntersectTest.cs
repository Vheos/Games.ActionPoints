namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using Vheos.Tools.Extensions.Math;

    public class SegmentIntersectTest : MonoBehaviour
    {
        [field: SerializeField] public Segment2D A { get; private set; }
        [field: SerializeField] public Segment2D B { get; private set; }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(A.From, A.To);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(B.From, B.To);


            if (NewUtility.IntersectSegments(A, B, out var intersectionPoint))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(intersectionPoint, 0.05f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(A.From, 0.05f);
            }
        }
    }
}