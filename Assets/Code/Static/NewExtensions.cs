namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Globalization;
    using UnityEngine;
    using UnityEngine.UI;
    using Tools.UtilityN;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using System.Collections.Generic;

    static public class NewExtensions
    {
        // Various
        static public void ForEachSetFlag<T>(this T t, Action<T> action) where T : Enum
        {
            foreach (var flag in Utility.GetEnumValues<T>())
                if (t.HasFlag(flag))
                    action(flag);
        }
        static public void GOActivate(this Component t)
        => t.gameObject.SetActive(true);
        static public void GODeactivate(this Component t)
        => t.gameObject.SetActive(false);
        static public float RandomMinMax(this Vector2 t)
        => UnityEngine.Random.Range(t.x, t.y);
        static public Quaternion ToZRotation(this float t)
        => Quaternion.Euler(0, 0, t);
        static public bool Roll(this float t)
        => UnityEngine.Random.value < t;
        static public bool RollPercent(this float t)
        => t.Div(100f).Roll();
        static public string ToInvariant(this float t)
        => t.ToString(CultureInfo.InvariantCulture);
        static public string ToInvariant(this float t, string format)
        => t.ToString(format, CultureInfo.InvariantCulture);
        static public GameObject CreateChildGameObject(this GameObject t, string name = null)
        {
            GameObject r = new GameObject();
            if (name != null)
                r.name = name;
            r.BecomeChildOf(t);
            return r;
        }
        static public GameObject CreateChildGameObject(this Component t, string name = null)
        => t.gameObject.CreateChildGameObject(name);

        // Float
        static public Vector2 Append(this float t, float y = 0f)
        => new Vector2(t, y);
        static public Vector3 Append(this float t, float y, float z)
        => new Vector3(t, y, z);
        static public Vector3 Append(this float t, Vector2 a)
        => new Vector3(t, a.x, a.y);
        static public Vector4 Append(this float t, float y, float z, float w)
        => new Vector4(t, y, z, w);
        static public Vector3 Append(this float t, Vector3 a)
        => new Vector4(t, a.x, a.y, a.z);

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
        /// <summary> Returns a shallow copy of this array. </summary>
        static public T[] MakeCopy<T>(this T[] t)
        {
            T[] r = new T[t.Length];
            t.CopyTo(r, 0);
            return r;
        }
        /// <summary> Swaps the references of this object and a. </summary>
        static public void SwapWith<T>(this T t, T a) where T : class
        {
            T temp = t;
            t = a;
            a = temp;
        }
        /// <summary> Swaps the references of this object and a. </summary>
        static public void SwapWith<T>(ref this T t, ref T a) where T : struct
        {
            T temp = t;
            t = a;
            a = temp;
        }

        // Bool
        static public int To01(this bool t)
        => t ? 1 : 0;
        static public int Map(this bool t, int a, int b)
        => t ? a : b;
        static public float Map(this bool t, float a, float b)
        => t ? a : b;

        // Color
        static public Color NewA(this Color t, float a)
        => new Color(t.r, t.g, t.b, a);

        /// <summary> Lerps from this color to a at alpha b. </summary>
        static public Color Lerp(this Color t, Color a, float b)
        => new Color(t.r.Lerp(a.r, b), t.g.Lerp(a.g, b), t.b.Lerp(a.b, b), t.a.Lerp(a.a, b));
        /// <summary> Lerps from this color to a at alpha b (clamped between 0 and 1). </summary>
        static public Color LerpClamped(this Color t, Color a, float b)
        => new Color(t.r.LerpClamped(a.r, b), t.g.LerpClamped(a.g, b), t.b.LerpClamped(a.b, b), t.a.LerpClamped(a.a, b));
        /// <summary> Maps this color from the range [a, b] to [c, d]. </summary>
        static public Color Map(this Color t, Color a, Color b, Color c, Color d)
        => new Color(t.r.Map(a.r, b.r, c.r, d.r), t.g.Map(a.g, b.g, c.g, d.g), t.b.Map(a.b, b.b, c.b, d.b), t.a.Map(a.a, b.a, c.a, d.a));
        /// <summary> Maps this color from the range [a, b] to [c, d] (with clamped output). </summary>
        static public Color MapClamped(this Color t, Color a, Color b, Color c, Color d)
        => new Color(t.r.MapClamped(a.r, b.r, c.r, d.r), t.g.MapClamped(a.g, b.g, c.g, d.g), t.b.MapClamped(a.b, b.b, c.b, d.b), t.a.MapClamped(a.a, b.a, c.a, d.a));
        /// <summary> Maps this color from the range [a, b] to [0, 1]. </summary>
        static public Color MapTo01(this Color t, Color a, Color b)
        => new Color(t.r.MapTo01(a.r, b.r), t.g.MapTo01(a.g, b.g), t.b.MapTo01(a.b, b.b), t.a.MapTo01(a.a, b.a));
        /// <summary> Maps this color from the range [0, 1] to [a, b]. </summary>
        static public Color MapFrom01(this Color t, Color a, Color b)
        => new Color(t.r.MapFrom01(a.r, b.r), t.g.MapFrom01(a.g, b.g), t.b.MapFrom01(a.b, b.b), t.a.MapFrom01(a.a, b.a));

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
        static public void SetLocalX(this Transform t, float a)
        {
            Vector3 temp = t.localPosition;
            temp.x = a;
            t.localPosition = temp;
        }

        // Edge points
        /// <summary> Returns a point on this rectangle's edge given direction a. </summary>
        static public Vector2 EdgePoint(this Rect t, Vector2 a)
        => t.center + t.size.Div(2f).Mul(a.NormalizeComps());
        /// <summary> Returns a point on this cuboid's surface given direction a. </summary>
        static public Vector3 SurfacePoint(this Bounds t, Vector3 a)
        => t.center + t.extents.Mul(a.NormalizeComps());

        // Bounds
        static public Bounds LocalBounds(this Collider t)
        {
            return t switch
            {
                BoxCollider r => new Bounds(r.center, r.size),
                SphereCollider r => new Bounds(r.center, (2 * r.radius).ToVector3()),
                CapsuleCollider r => new Bounds(r.center, new Vector3(r.height, 2 * r.radius, 2 * r.radius).Rotate(r.transform.rotation)),
                MeshCollider r => r.sharedMesh.bounds,
                _ => default,
            };
        }
        /// <summary> Returns a rectangle equivalent to this cuboid with depth removed. </summary>
        static public Rect ToRect(this Bounds t)
        => new Rect
        {
            size = t.size,
            center = t.center,
        };
    }
}