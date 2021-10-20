#if UNITY_EDITOR
namespace Vheos.Games.ActionPoints.Editor
{
    using UnityEngine;
    using Tools.Extensions.Math;

    static public class EditorUtility
    {
        // Gizmo Draws
        static public void DrawLine(Vector3 from, Vector3 to, Transform transform, Color color)
        {
            Vector3 drawFrom = from;
            Vector3 drawTo = to;
            if (transform != null)
            {
                drawFrom = drawFrom.Transform(transform);
                drawTo = drawTo.Transform(transform);
            }

            Gizmos.color = color;
            Gizmos.DrawLine(drawFrom, drawTo);
        }
        static public void DrawVector(Vector3 at, Vector3 vector, Transform transform, Color color)
        => DrawLine(at, at + vector, transform, color);
        static public void DrawAxis(Vector3 axis, Transform transform, Color color)
        => DrawLine(-axis * 1000, axis * 1000, transform, color);
        static public void DrawCircle(Vector3 at, Vector3 normal, float translationAlongNormal, float radius, Transform transform, Color color, int vertices = 12)
        {
            // Initialize points
            Gizmos.color = color;
            Quaternion alignmentRotation = Vector3.back.RotationTo(normal);
            Vector3 offset = translationAlongNormal * normal.normalized;
            Vector3 previousPoint = Vector3.zero;
            for (int i = 0; i <= vertices; i++)
            {
                Vector3 currentPoint = at + radius * NewUtility.PointOnCircle(i.Div(vertices) * 2 * Mathf.PI, 1f, false).Append();
                currentPoint = currentPoint.Rotate(alignmentRotation) + offset;
                if (transform != null)
                    currentPoint = currentPoint.Transform(transform);
                if (i > 0)
                    Gizmos.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }
        }
        static public void DrawCircle(Vector3 at, float radius, Transform transform, Color color, int vertices = 12)
        => DrawCircle(at, Vector3.back, 0f, radius, transform, color, vertices);
        static public void DrawSphere(Vector3 at, float radius, Transform transform, Color color)
        {

            Vector3 drawAt = at;
            float drawRadius = radius;
            if (transform != null)
            {
                drawAt = drawAt.Transform(transform);
                drawRadius *= transform.lossyScale.MaxComp();
            }

            Gizmos.color = color;
            Gizmos.DrawWireSphere(drawAt, drawRadius);
        }
        static public void DrawRect(Rect rect, Vector3 normal, float translationAlongNormal, Transform transform, Color color, float diagonalsAlpha = 0f)
        {
            // Initialize points
            Vector3[] points = new Vector3[]
            {
                rect.center + rect.size.Mul(-0.5f, -0.5f),
                rect.center + rect.size.Mul(-0.5f, +0.5f),
                rect.center + rect.size.Mul(+0.5f, +0.5f),
                rect.center + rect.size.Mul(+0.5f, -0.5f),
            };

            // Transform points
            Quaternion alignmentRotation = Vector3.back.RotationTo(normal);
            Vector3 offset = translationAlongNormal * normal.normalized;
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = (points[i].Rotate(alignmentRotation) + offset);
                if (transform != null)
                    points[i] = points[i].Transform(transform);
            }

            // Draw rect
            if (color.a > 0)
            {
                Gizmos.color = color;
                Gizmos.DrawLine(points[0], points[1]);
                Gizmos.DrawLine(points[1], points[2]);
                Gizmos.DrawLine(points[2], points[3]);
                Gizmos.DrawLine(points[3], points[0]);
            }

            // Draw diagonals
            if (diagonalsAlpha > 0f)
            {
                color.a *= diagonalsAlpha;
                Gizmos.color = color;
                Gizmos.DrawLine(points[0], points[2]);
                Gizmos.DrawLine(points[1], points[3]);
            }
        }
        static public void DrawRect(Rect rect, Transform transform, Color color)
        => DrawRect(rect, Vector3.back, 0f, transform, color);
        static public void DrawBounds(Bounds bounds, Transform transform, Color color, float diagonalsAlpha = 0f)
        {
            // Initialize points
            Vector3[] points = new Vector3[]
            {
                bounds.center + bounds.extents.Mul(-1, -1, -1),
                bounds.center + bounds.extents.Mul(+1, -1, -1),
                bounds.center + bounds.extents.Mul(+1, +1, -1),
                bounds.center + bounds.extents.Mul(-1, +1, -1),

                bounds.center + bounds.extents.Mul(-1, -1, +1),
                bounds.center + bounds.extents.Mul(+1, -1, +1),
                bounds.center + bounds.extents.Mul(+1, +1, +1),
                bounds.center + bounds.extents.Mul(-1, +1, +1),
            };

            // Transform points
            if (transform != null)
                for (int i = 0; i < points.Length; i++)
                    points[i] = points[i].Transform(transform);

            // Draw bounds
            if (color.a > 0)
            {
                Gizmos.color = color;
                Gizmos.DrawLine(points[0], points[1]);
                Gizmos.DrawLine(points[1], points[2]);
                Gizmos.DrawLine(points[2], points[3]);
                Gizmos.DrawLine(points[3], points[0]);

                if (bounds.size.z > 0)
                {
                    Gizmos.DrawLine(points[4], points[5]);
                    Gizmos.DrawLine(points[5], points[6]);
                    Gizmos.DrawLine(points[6], points[7]);
                    Gizmos.DrawLine(points[7], points[4]);
                    Gizmos.DrawLine(points[0], points[4]);
                    Gizmos.DrawLine(points[1], points[5]);
                    Gizmos.DrawLine(points[2], points[6]);
                    Gizmos.DrawLine(points[3], points[7]);
                }

            }

            // Draw diagonals
            if (diagonalsAlpha > 0f)
            {
                color.a *= diagonalsAlpha;
                Gizmos.color = color;
                Gizmos.DrawLine(points[0], points[2]);
                Gizmos.DrawLine(points[1], points[3]);

                if (bounds.size.z > 0)
                {
                    Gizmos.DrawLine(points[0], points[5]);
                    Gizmos.DrawLine(points[1], points[4]);
                    Gizmos.DrawLine(points[0], points[7]);
                    Gizmos.DrawLine(points[3], points[4]);
                    Gizmos.DrawLine(points[6], points[4]);
                    Gizmos.DrawLine(points[5], points[7]);
                    Gizmos.DrawLine(points[6], points[3]);
                    Gizmos.DrawLine(points[7], points[2]);
                    Gizmos.DrawLine(points[6], points[1]);
                    Gizmos.DrawLine(points[5], points[2]);
                }
            }
        }
    }
}
#endif