namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using Tools.UtilityN;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    static public class NewExtensions
    {
        // Various
        static public void ForEachSetFlag<T>(this T t, Action<T> action) where T : Enum
        {
            foreach (var flag in Utility.GetEnumValues<T>())
                if (t.HasFlag(flag))
                    action(flag);
        }
        static public bool TryNonNull<T>(this T t, out T r) where T : UnityEngine.Object
        {
            r = t;
            return r != null;
        }
        static public void GOActivate(this Component t)
        => t.gameObject.SetActive(true);
        static public void GODeactivate(this Component t)
        => t.gameObject.SetActive(false);

        // Color
        static public Color Lerp(this Color t, Color a, float b)
        => Color.LerpUnclamped(t, a, b);
        static public Color NewA(this Color t, float a)
        => new Color(t.r, t.g, t.b, a);

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
        static public Vector3 ProjectVectorOnPlane(this Vector3 t, Vector3 a)
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