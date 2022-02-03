namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using System.Linq;
    using System.Collections.Generic;

    public struct RectCircle
    {
        // Privates
        private IEnumerable<Vector2> EdgePoints
        {
            get
            {
                var localDirections = new[] { Vector2.right, Vector2.up, Vector2.down };
                Rect scaledRect = Rect;
                for (int i = 0; i < localDirections.Length; i++)
                {
                    Ray2D ray = new(scaledRect.center, localDirections[i].Rotate(Angle, true));
                    foreach (var segment in scaledRect.EdgeSegments())
                        if (NewUtility.IntersectRayAndSegment(ray, segment, out var edgePoint))
                            yield return edgePoint;
                }
            }
        }
        private Circle Circle
        {
            get
            {
                Vector2[] edgePoints = EdgePoints.ToArray();
                Vector2 center = NewUtility.FindCircleCenter(edgePoints);
                Vector2 circlePoint = EncapsulateClosestCorner
                    ? Rect.ClosestCorner(edgePoints.First())
                    : edgePoints.First();
                float radius = center.DistanceTo(circlePoint);
                return new(center, radius);
            }
        }

        // Publics
        public Rect Rect;
        public float Angle;
        public bool EncapsulateClosestCorner;        
        public IEnumerable<(Vector2 Position, float Angle)> GetElementsPositionsAndAngles(int targetCount, float radius, float distanceFromPerimeter = 1f)
        {
            var (WheelCenter, WheelRadius) = Circle;
            WheelRadius += radius * distanceFromPerimeter;

            float angle = radius.Div(WheelRadius).ArcSin().Mul(2) * Mathf.Rad2Deg;
            int count = 360.Div(angle).RoundDown().ClampMax(targetCount);

            for (int i = 0; i < count; i++)
            {
                Vector2 offset = Vector2.right.Mul(WheelRadius);
                float offsetAngle = Angle + angle * count.Sub(1).Div(2).Sub(i);
                yield return (WheelCenter + offset.Rotate(offsetAngle, true), offsetAngle);
            }
        }
    }
}

/*
#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using System;
    using System.Linq;
    using UnityEngine;
    using UnityEditor;
    using Tools.Extensions.Math;

    public static class TransformArm_GizmoDrawer
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        static void Pickable(ButtonsWheel component, GizmoType type)
        {
            if (!component.isActiveAndEnabled)
                return;

            Gizmos.color = Color.magenta;
            foreach (var scaledEdgePoint in component.EdgePoints)
                Gizmos.DrawWireSphere(scaledEdgePoint.TransformNoScale(component.transform), 0.05f);

            Gizmos.color = Color.cyan;
            var (WheelCenter, WheelRadius) = component.Circle;
            Gizmos.DrawWireSphere(WheelCenter.TransformNoScale(component.transform), WheelRadius);


            Gizmos.color = Color.yellow;
            foreach (var localButtonOffset in component.LocalButtonPositions)
                Gizmos.DrawWireSphere(localButtonOffset.TransformNoScale(component.transform), component.ButtonRadius);
        }
    }
}
#endif
*/
//float minY = scaledRect.size.y.Sub(scaledRect.size.x).Div(2f);
//.ClampMin(float.NegativeInfinity, minY);