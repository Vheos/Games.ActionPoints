namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using Vheos.Tools.Extensions.Math;

    public class RayIntersectTest : MonoBehaviour
    {
        [field: SerializeField] public Vector3 RayA { get; private set; }
        [field: SerializeField] public Vector3 RayB { get; private set; }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Ray2D rayA = new(RayA.XY(), Vector2.right.Rotate(RayA.z, true));
            Gizmos.DrawLine(rayA.origin, rayA.origin + rayA.direction * 100f);

            Gizmos.color = Color.magenta;
            Ray2D rayB = new(RayB.XY(), Vector2.right.Rotate(RayB.z, true));
            Gizmos.DrawLine(rayB.origin, rayB.origin + rayB.direction * 100f);


            if (NewUtility.IntersectRays(rayA, rayB, out var intersectionPoint))
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