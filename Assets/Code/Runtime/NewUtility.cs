namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using System.Linq;

    static public class NewUtility
    {
        // EXTENSIONS
        static public int SetFlagsCount(this int v)
        {
            v -= (v >> 1) & 0x55555555;
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333);
            int c = ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
            return c;
        }
        static public float RandomMinMax(this Vector2 t)
        => UnityEngine.Random.Range(t.x, t.y);
        static public Quaternion ToZRotation(this float t)
        => Quaternion.Euler(0, 0, t);
        static public bool IsCompilerGenerated(this Type t)
        => Attribute.GetCustomAttribute(t, typeof(CompilerGeneratedAttribute)) != null;
        static public bool CursorRaycast(this Collider t, CCamera camera, out RaycastHit hitInfo)
        => t.Raycast(camera.Unity.CursorRay(), out hitInfo, float.PositiveInfinity);
        static public bool IsUnderCursor(this Collider t, CCamera camera)
        => t.CursorRaycast(camera, out _);
        static public T Random<T>(this IEnumerable<T> t)
        {
            T r = default;
            int count = 0;
            foreach (T element in t)
            {
                count++;
                if (UnityEngine.Random.Range(0, count) == 0)
                    r = element;
            }
            return r;
        }
        static public bool SameGOAs(this Component t, Component a)
        => t.gameObject == a.gameObject;
        static public T[] InArray<T>(this T t)
        => new[] { t };
        static public Vector3 WorldPointToCameraSpaceWorldPoint(this Canvas t, Vector3 a)
        => t.worldCamera.transform.position + t.worldCamera.transform.position.DirectionTowards(a) * t.planeDistance;
        static public Vector3 WorldPointToAnchoredPoint(this Canvas t, Vector3 a)
        => t.worldCamera.WorldToScreenPoint(a) / t.scaleFactor;
        static public bool IsOnLayer(this GameObject t, BuiltInLayer a)
        => t.layer == (int)a;
        static public bool IsOnLayer(this Component t, BuiltInLayer a)
        => t.gameObject.IsOnLayer(a);
        static public Bounds Scale(this Bounds t, Vector3 a)
        => new(t.center, t.size.Mul(a));
        static public Bounds Scale(this Bounds t, GameObject a)
        => new(t.center, t.size.Mul(a.transform.localScale));
        static public Bounds Scale(this Bounds t, Component a)
        => new(t.center, t.size.Mul(a.transform.localScale));
        static public Rect Scale(this Rect t, Vector3 a)
        => new()
        {
            size = t.size.Mul(a),
            center = t.center,
        };
        static public Rect Scale(this Rect t, GameObject a)
        => new()
        {
            size = t.size.Mul(a.transform.localScale),
            center = t.center,
        };
        static public Rect Scale(this Rect t, Component a)
        => new()
        {
            size = t.size.Mul(a.transform.localScale),
            center = t.center,
        };
        static public string SplitCamelCase(this string t)
        => Regex.Replace(t, "([a-z](?=[A-Z0-9]))", "$1 ");
        static public void Shuffle<T>(this IList<T> t)
        {
            for (int i = 0; i < t.Count - 1; i++)
            {
                int j = UnityEngine.Random.Range(i, t.Count);
                T tmp = t[i];
                t[i] = t[j];
                t[j] = tmp;
            }
        }
        static public IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> t)
        => t ?? Enumerable.Empty<T>();
        static public TReturn IfNonNull<T1, TReturn>(this T1 t, Func<T1, TReturn> a, TReturn b = default)
        => t != null ? a(t) : b;
        static public TReturn IfNonNull<T1, TReturn>(this T1 t, TReturn a, TReturn b = default)
        => t != null ? a : b;

        // Try
        static public bool TryNonDefault<T>(this T t, out T r)
        {
            if (!t.Equals(default))
            {
                r = t;
                return true;
            }
            r = default;
            return false;
        }
        static public bool TryFind<T>(this IEnumerable<T> t, Func<T, bool> test, out T r)
        {
            foreach (var element in t)
                if (test(element))
                {
                    r = element;
                    return true;
                }
            r = default;
            return false;
        }
        static public bool TryFindIndex<T>(this IEnumerable<T> t, T a, out int r)
        {
            r = 0;
            foreach (var element in t)
            {
                if (element.Equals(a))
                    return true;
                r++;
            }
            return false;
        }
        static public bool TryGet<T>(this IList<T> t, int a, out T r, T defaultValue)
        {
            if (a >= 0 && a < t.Count)
            {
                r = t[a];
                return true;
            }

            r = defaultValue;
            return false;
        }

        // Quaternion
        static public Quaternion RotateAround(this Quaternion quaternion, Vector3 axis, float angle)
        => quaternion.Add(Quaternion.AngleAxis(angle, axis));
        static public Quaternion YFlippedIfCloser(this Quaternion from, Quaternion to)
        {
            Quaternion flipped = to.RotateAround(Vector3.up, 180f);
            return from.AngleTo(flipped) < from.AngleTo(to) ? flipped : to;
        }

        /// <summary> Returns this vector projected on a plane with normal a. </summary>
        static public Vector3 ProjectOnPlane(this Vector3 t, Vector3 a)
        => Vector3.ProjectOnPlane(t, a);
        static public Vector3 Rotate(this Vector2 t, Vector3 a, float b, bool inDegrees = false)
        => t.Append().Rotate(a, b, inDegrees);
        static public Vector3 Rotate(this Vector3 t, Vector3 a, float b, bool inDegrees = false)
        {
            if (!inDegrees)
                b *= Mathf.Deg2Rad;
            return Quaternion.AngleAxis(b, a) * t;
        }
        public static Vector2 Rotate(this Vector2 t, float a, bool inDegrees = false)
        {
            if (inDegrees)
                a *= Mathf.Deg2Rad;

            float sin = Mathf.Sin(-a);
            float cos = Mathf.Cos(-a);
            return new Vector2(t.x * cos - t.y * sin, t.x * sin + t.y * cos);
        }
        static public bool IntersectRays(Ray2D a, Ray2D b, out Vector2 r)
        {
            var det = a.direction.y * b.direction.x - a.direction.x * b.direction.y;

            if (det != 0)
            {
                float dx = b.origin.x - a.origin.x;
                float dy = b.origin.y - a.origin.y;
                float u = (a.direction.x * dy - a.direction.y * dx) / det;
                float v = (b.direction.x * dy - b.direction.y * dx) / det;

                if (u >= 0f && v >= 0f)
                {
                    r = a.GetPoint(v);
                    return true;
                }
            }

            r = float.NaN.ToVector2();
            return false;
        }
        public static bool IntersectSegments(Segment2D a, Segment2D b, out Vector2 r)
        {
            float dax = a.To.x - a.From.x;
            float day = a.To.y - a.From.y;
            float dby = b.To.y - b.From.y;
            float dbx = b.To.x - b.From.x;
            float det = day * dbx - dax * dby;

            if (det != 0f)
            {
                float dx = b.From.x - a.From.x;
                float dy = b.From.y - a.From.y;
                float u = (dax * dy - day * dx) / det;   // progress of intersection along segment B
                float v = (dbx * dy - dby * dx) / det;   // progress of intersection along segment A

                if (u >= 0f && u <= 1f
                && v >= 0f && v <= 1f)
                {
                    float x = a.From.x + dax * v;
                    float y = a.From.y + day * v;
                    r = new(x, y);
                    return true;
                }
            }

            r = float.NaN.ToVector2();
            return false;
        }
        public static bool IntersectRayAndSegment(Ray2D a, Segment2D b, out Vector2 r)
        {
            float dby = b.To.y - b.From.y;
            float dbx = b.To.x - b.From.x;
            float det = a.direction.y * dbx - a.direction.x * dby;

            if (det != 0f)
            {
                float dx = b.From.x - a.origin.x;
                float dy = b.From.y - a.origin.y;
                float u = (a.direction.x * dy - a.direction.y * dx) / det;   // progress of intersection along segment B
                float v = (dbx * dy - dby * dx) / det;   // distance of intersection along ray A

                if (u >= 0f && u <= 1f
                && v >= 0f)
                {
                    r = a.GetPoint(v);
                    return true;
                }
            }

            r = float.NaN.ToVector2();
            return false;
        }
        static public IEnumerable<Vector2> Corners(this Rect t)
        {
            yield return t.center + t.size.Mul(-0.5f, +0.5f);
            yield return t.center + t.size.Mul(+0.5f, +0.5f);
            yield return t.center + t.size.Mul(+0.5f, -0.5f);
            yield return t.center + t.size.Mul(-0.5f, -0.5f);
        }
        static public IEnumerable<Ray2D> EdgeRays(this Rect t)
        {
            yield return new(t.center + t.size.Mul(-0.5f, +0.5f), Vector2.right);
            yield return new(t.center + t.size.Mul(+0.5f, +0.5f), Vector2.down);
            yield return new(t.center + t.size.Mul(+0.5f, -0.5f), Vector2.left);
            yield return new(t.center + t.size.Mul(-0.5f, -0.5f), Vector2.up);
        }
        static public IEnumerable<Segment2D> EdgeSegments(this Rect t)
        {
            yield return new(t.center + t.size.Mul(-0.5f, +0.5f), t.center + t.size.Mul(+0.5f, +0.5f));
            yield return new(t.center + t.size.Mul(+0.5f, +0.5f), t.center + t.size.Mul(+0.5f, -0.5f));
            yield return new(t.center + t.size.Mul(+0.5f, -0.5f), t.center + t.size.Mul(-0.5f, -0.5f));
            yield return new(t.center + t.size.Mul(-0.5f, -0.5f), t.center + t.size.Mul(-0.5f, +0.5f));
        }
        static public Vector2 ClosestCorner(this Rect t, Vector2 a)
        {
            float minDistance = float.PositiveInfinity;
            Vector2 minCorner = default;
            foreach (var corner in t.Corners())
            {
                float distance = a.DistanceTo(corner);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minCorner = corner;
                }
            }
            return minCorner;
        }


        // UTILITY
        static public Vector2 RandomPointOnCircle(float radius = 1f)
        => PointOnCircle(UnityEngine.Random.value * 2 * Mathf.PI, radius);
        static public Vector2 PointOnCircle(float angle, float radius = 1f, bool inDegrees = false)
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
        => UnityEngine.Random.value < chance;
        static public bool Flip()
        => Roll(0.5f);
        static private Vector2 FindCircleCenter(Vector2 a, Vector2 b, Vector2 c)
        {
            float xab = a.x - b.x;
            float xac = a.x - c.x;
            float yab = a.y - b.y;
            float yac = a.y - c.y;
            float sxac = a.x.Pow(2) - c.x.Pow(2);
            float syac = a.y.Pow(2) - c.y.Pow(2);
            float sxba = b.x.Pow(2) - a.x.Pow(2);
            float syba = b.y.Pow(2) - a.y.Pow(2);

            float f = (sxac * xab + syac * xab + sxba * xac + syba * xac)
                    / 2f / (yac * xab - yab * xac);
            float g = (sxac * yab + syac * yab + sxba * yac + syba * yac)
                    / 2f / (xac * yab - xab * yac);

            return new(g, f);
        }
        static public Vector2 FindCircleCenter(Vector2[] points)
        {
            if (points.Length < 3)
                return float.NaN.ToVector2();
            return FindCircleCenter(points[0], points[1], points[2]);
        }
    }
}