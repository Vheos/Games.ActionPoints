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
    }
}