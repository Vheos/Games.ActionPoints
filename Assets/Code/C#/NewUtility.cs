namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.UtilityN;
    using Tools.Extensions.Math;
    static public class NewUtility
    {
        // Various
        static public void ForEachSetFlag<T>(this T t, Action<T> action) where T : Enum
        {
            foreach (var flag in Utility.GetEnumValues<T>())
                if (t.HasFlag(flag))
                    action(flag);
        }
        static public float LerpHalfTimeToAlpha(float lerpHalfTime)
        {
            if (lerpHalfTime == 0f)
                return 1f;

            float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
            return 1f - 0.5f.Pow(deltaTime / lerpHalfTime);
        }
        static public bool TryNonNull<T>(this T t, out T r) where T : UnityEngine.Object
        {
            r = t;
            return r != null;
        }

        // Camera extensions
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