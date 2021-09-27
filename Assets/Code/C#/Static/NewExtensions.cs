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
    }
}