namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using Tools.UtilityN;
    using Tools.Extensions.Math;
    using Tools.Extensions.Sprite;

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

        // Image
        static public void SetA(this Image t, float a)
        {
            Color newColor = t.color;
            newColor.a = a;
            t.color = newColor;
        }

        // Sprite
        /// <summary> Returns coords of a pixel at the given position. </summary>
        static public Vector2Int PositionToPixelCoords(this Sprite t, Vector3 position, Transform transform = null)
        {
            if (transform != null)
                position = position.Untransform(transform);
            return Vector2Int.FloorToInt(t.RectPixels().center + position.XY() * t.pixelsPerUnit);
        }
        /// <summary> Returns color of a pixel at the given position. </summary>
        static public Color PositionToPixelColor(this Sprite t, Vector3 position, Transform transform = null)
        {
            Vector2Int pixelPosition = t.PositionToPixelCoords(position, transform);
            return t.Texture().GetPixel(pixelPosition.x, pixelPosition.y);
        }
        /// <summary> Returns alpha of a pixel at the given position. </summary>
        static public float PositionToPixelAlpha(this Sprite t, Vector3 position, Transform transform = null)
        => t.PositionToPixelColor(position, transform).a;
        /// <summary> Returns position at the given pixel. </summary>
        static public Vector3 PixelCoordsToPosition(this Sprite t, Vector2Int pixel, Transform transform = null)
        {

            Vector3 position = (pixel - t.RectPixels().center) / t.pixelsPerUnit;
            if (transform != null)
                position = position.Transform(transform);
            return position;
        }

        // Camera
        static public Ray CursorRay(this Camera t)
        => t.ScreenPointToRay(Input.mousePosition);
        static public Plane ScreenPlane(this Camera t, Vector3 worldPoint)
        => new Plane(t.transform.forward.Neg(), worldPoint);
        static public Vector3 CursorToWorldPoint(this Camera t, float distanceFromCamera)
        => t.ScreenToWorldPoint(Input.mousePosition.XY().Append(distanceFromCamera));
        static public Vector3 CursorToPlanePoint(this Camera t, Plane plane)
        {
            Ray ray = t.CursorRay();
            if (plane.Raycast(ray, out float distance))
                return ray.GetPoint(distance);
            return float.NaN.ToVector3();
        }
        static public Vector3 CursorToScreenPlanePoint(this Camera t, Vector3 worldPoint)
        => t.CursorToPlanePoint(t.ScreenPlane(worldPoint));

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

        // Transform
        /// <summary> Applies chosen a transform to this direction. </summary>
        static public Vector3 TransformDirection(this Vector2 t, Transform a)
        => a.TransformDirection(t);
        /// <summary> Reverts chosen a transform from this direction. </summary>
        static public Vector3 UntransformDirection(this Vector2 t, Transform a)
        => a.InverseTransformDirection(t);
        /// <summary> Applies chosen a transform to this direction. </summary>
        static public Vector3 TransformDirection(this Vector3 t, Transform a)
        => a.TransformDirection(t);
        /// <summary> Reverts chosen a transform from this direction. </summary>
        static public Vector3 UntransformDirection(this Vector3 t, Transform a)
        => a.InverseTransformDirection(t);
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

        // Vector
        /// <summary> Returns the on-screen distance between this vector and a from camera b's perspective. </summary>
        static public float ScreenDistanceTo(this Vector3 t, Vector3 a, Camera b)
        => (b.WorldToScreenPoint(a) - b.WorldToScreenPoint(t)).magnitude;
        /// <summary> Returns the on-screen offset from this vector to a from camera b's perspective. </summary>
        static public Vector3 ScreenOffsetTo(this Vector3 t, Vector3 a, Camera b)
        => (b.WorldToScreenPoint(a) - b.WorldToScreenPoint(t));
        /// <summary> Returns the on-screen offset from a to this vector from camera b's perspective. </summary>
        static public Vector3 ScreenOffsetFrom(this Vector3 t, Vector3 a, Camera b)
        => (b.WorldToScreenPoint(t) - b.WorldToScreenPoint(a));
        /// <summary> Returns the on-screen direction from this vector towards a from camera b's perspective. </summary>
        static public Vector3 ScreenDirectionTowards(this Vector3 t, Vector3 a, Camera b)
        => (b.WorldToScreenPoint(a) - b.WorldToScreenPoint(t)).normalized;
        /// <summary> Returns the on-screen direction from a towards this vector from camera b's perspective. </summary>
        static public Vector3 ScreenDirectionAwayFrom(this Vector3 t, Vector3 a, Camera b)
        => (b.WorldToScreenPoint(t) - b.WorldToScreenPoint(a)).normalized;
        public static Vector2 PerpendicularCW(this Vector2 t)
        => new Vector2(t.y, -t.x);
        public static Vector2 PerpendicularCCW(this Vector2 t)
        => new Vector2(-t.y, t.x);

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