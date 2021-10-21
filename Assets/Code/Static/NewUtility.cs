namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;

    static public class NewUtility
    {
        // Various
        static public float LerpHalfTimeToAlpha(float lerpHalfTime)
        {
            if (lerpHalfTime == 0f)
                return 1f;

            float deltaTime = Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
            return 1f - 0.5f.Pow(deltaTime / lerpHalfTime);
        }
        static public AnimationCurve CreateLinearAnimationCurve(params (float Time, float Value)[] data)
        {
            Keyframe[] keyframes = new Keyframe[data.Length];
            for (int i = 0; i < keyframes.Length; i++)
                keyframes[i] = new Keyframe(data[i].Time, data[i].Value, 0f, 0f, 0f, 0f);

            return new AnimationCurve(keyframes);
        }
        static public Vector2 PointOnCircle(float angle, float radius = 1f, bool inDegrees = true)
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
        => Random.value < chance;
        static public bool Flip()
        => Roll(0.5f);
    }
}