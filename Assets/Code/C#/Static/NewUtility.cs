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
    }
}