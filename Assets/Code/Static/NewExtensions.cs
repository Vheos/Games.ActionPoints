namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;
    using Tools.UtilityN;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    static public class NewExtensions
    {
        // Various
        static public int SetFlagsCount(this int v)
        {
            v -= (v >> 1) & 0x55555555;
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333);
            int c = ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24;
            return c;
        }
        static public void GOActivate(this Component t)
        => t.gameObject.SetActive(true);
        static public void GODeactivate(this Component t)
        => t.gameObject.SetActive(false);
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
        static public T ChooseIf<T>(this T t, Func<T, bool> test, T onFalse = default)
        => test(t) ? t : onFalse;

        // Midpoint
        static public Vector3 Midpoint<T>(this IEnumerable<T> t, Func<T, Vector3> positionFunc)
        {
            Vector3 r = Vector3.zero;
            int count = 0;
            foreach (var element in t)
            {
                r += positionFunc(element);
                count++;
            }
            return r / count;
        }
        static public Vector3 Midpoint<T>(this ICollection<T> t, Func<T, Vector3> positionFunc)
        {
            Vector3 r = Vector3.zero;
            foreach (var element in t)
                r += positionFunc(element);
            return r / t.Count;
        }
        static public Vector3 Midpoint(this IEnumerable<Component> t)
        => Midpoint(t, (component) => component.transform.position);
        static public Vector3 Midpoint(this ICollection<Component> t)
        => Midpoint(t, (component) => component.transform.position);

        // Legacy
        /// <summary> Returns this array of hits sorted by distance from point a. </summary>
        static public RaycastHit[] SortedByDistanceFrom(this RaycastHit[] t, Vector3 a)
        {
            RaycastHit[] r = t.MakeCopy();
            for (int i = 0; i < r.Length; i++)
            {
                int jMin = i;
                for (int j = i + 1; j < r.Length; j++)
                    if (a.DistanceTo(r[j].point) < a.DistanceTo(r[jMin].point))
                        jMin = j;

                if (jMin != i)
                    r[i].SwapWith(ref r[jMin]);
            }

            return r;
        }
        /// <summary> Returns this array of hits sorted by distance from object a. </summary>
        static public RaycastHit[] SortedByDistanceFrom(this RaycastHit[] t, GameObject a)
        => t.SortedByDistanceFrom(a.transform.position);
        /// <summary> Returns this array of hits sorted by distance from object a. </summary>
        static public RaycastHit[] SortedByDistanceFrom(this RaycastHit[] t, Component a)
        => t.SortedByDistanceFrom(a.gameObject);


        // Input
        /// <summary> Checks if this key has just been pressed. </summary>
        static public bool Pressed(this KeyCode t)
        => Input.GetKeyDown(t);
        /// <summary> Checks if this key has just been released. </summary>
        static public bool Released(this KeyCode t)
        => Input.GetKeyUp(t);
        /// <summary> Checks if this key is held down. </summary>
        static public bool Down(this KeyCode t)
        => Input.GetKey(t);
        /// <summary> Checks if this key is resting. </summary>
        static public bool Up(this KeyCode t)
        => !Input.GetKey(t);

        /// <summary> Returns this vector projected on a plane with normal a. </summary>
        static public Vector3 ProjectOnPlane(this Vector3 t, Vector3 a)
        => Vector3.ProjectOnPlane(t, a);
        static public Vector3 Rotate(this Vector2 t, Vector3 a, float b, bool inDegrees = true)
        => t.Append().Rotate(a, b, inDegrees);
        static public Vector3 Rotate(this Vector3 t, Vector3 a, float b, bool inDegrees = true)
        {
            if (!inDegrees)
                b *= Mathf.Deg2Rad;

            return Quaternion.AngleAxis(b, a) * t;
        }
        static public void SetLocalX(this Transform t, float x)
        {
            Vector3 temp = t.localPosition;
            temp.x = x;
            t.localPosition = temp;
        }
        static public void SetLocalY(this Transform t, float y)
        {
            Vector3 temp = t.localPosition;
            temp.y = y;
            t.localPosition = temp;
        }
        static public void SetLocalZ(this Transform t, float z)
        {
            Vector3 temp = t.localPosition;
            temp.z = z;
            t.localPosition = temp;
        }
    }
}