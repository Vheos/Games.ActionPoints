namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using Vheos.Tools.Extensions.Math;

    public class SegmentIntersectTest : MonoBehaviour
    {
        [SerializeField] protected Segment2D _A;
        [SerializeField] protected Segment2D _B;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_A.From, _A.To);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(_B.From, _B.To);


            if (NewUtility.IntersectSegments(_A, _B, out var intersectionPoint))
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(intersectionPoint, 0.05f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_A.From, 0.05f);
            }
        }
    }
}