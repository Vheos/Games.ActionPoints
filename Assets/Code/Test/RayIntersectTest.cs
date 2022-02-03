namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using Vheos.Tools.Extensions.Math;

    public class RayIntersectTest : MonoBehaviour
    {
        [SerializeField] protected Vector3 _RayA;
        [SerializeField] protected Vector3 _RayB;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Ray2D rayA = new(_RayA.XY(), Vector2.right.Rotate(_RayA.z, true));
            Gizmos.DrawLine(rayA.origin, rayA.origin + rayA.direction * 100f);

            Gizmos.color = Color.magenta;
            Ray2D rayB = new(_RayB.XY(), Vector2.right.Rotate(_RayB.z, true));
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