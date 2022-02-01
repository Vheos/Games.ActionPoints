namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

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
        static public bool CursorRaycast(this Collider t, Camera camera, out RaycastHit hitInfo)
        => t.Raycast(camera.CursorRay(), out hitInfo, float.PositiveInfinity);
        static public bool IsUnderCursor(this Collider t, Camera camera)
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
                a *= Mathf.Rad2Deg;

            float sina = Mathf.Sin(a);
            float cosa = Mathf.Cos(a);
            return new Vector2(cosa * t.x - sina * t.y, sina * t.x + cosa * t.y);
        }



        // UTILITY
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