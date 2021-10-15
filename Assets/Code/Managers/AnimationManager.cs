namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.General;

    public class AnimationManager : AUpdatable
    {
        // Private
        static private AnimationManager _instance;
        static private Dictionary<(UnityEngine.Object Instance, string UID), Coroutine> _coroutinesByGUID;
        static private IEnumerator CoroutineWhile(Func<bool> test, System.Action action, System.Action finalAction)
        {
            while (test())
            {
                action();
                yield return null;
            }
            finalAction();
        }

        // Publics
        static public void Animate<T>((UnityEngine.Object Instance, string UID) guid, Action<T> assignFunction, T from, T to, float duration,
            bool boomerang = false, System.Action finalAction = null) where T : struct
        {
            float startTime = Time.time;
            float elapsed() => Time.time - startTime;
            Func<float> qurveValue = boomerang switch
            {
                false => () => Qurve.ValueAt(elapsed() / duration),
                true => () => Qurve.ValueAt(elapsed() / duration).Sub(0.5f).Abs().Neg().Add(0.5f),
            };
            T finalValue = boomerang switch
            {
                false => to,
                true => from,
            };
            System.Action typedAssignFunction = new GenericParams<T>(assignFunction, from, to) switch
            {
                GenericParams<float> t => () => t.AssignFunction(t.From.Lerp(t.To, qurveValue())),
                GenericParams<Vector2> t => () => t.AssignFunction(t.From.Lerp(t.To, qurveValue())),
                GenericParams<Vector3> t => () => t.AssignFunction(t.From.Lerp(t.To, qurveValue())),
                GenericParams<Vector4> t => () => t.AssignFunction(t.From.Lerp(t.To, qurveValue())),
                GenericParams<Quaternion> t => () => t.AssignFunction(t.From.Lerp(t.To, qurveValue())),
                GenericParams<Color> t => () => t.AssignFunction(t.From.Lerp(t.To, qurveValue())),
                _ => throw new NotImplementedException(),
            };

            if (_coroutinesByGUID.TryGetNonNull(guid, out var oldCoroutine))
                _instance.StopCoroutine(oldCoroutine);

            _coroutinesByGUID[guid] = _instance.StartCoroutine(CoroutineWhile
            (
                () => elapsed() < duration,
                typedAssignFunction,
                () =>
                {
                    assignFunction(finalValue);
                    finalAction?.Invoke();
                    _coroutinesByGUID[guid] = null;
                }
            ));
        }
        static public void StopAnimation(Coroutine coroutine)
        => _instance.StopCoroutine(coroutine);

        // Mono
        public override void PlayAwake()
        {
            base.PlayAwake();
            _instance = this;
            _coroutinesByGUID = new Dictionary<(UnityEngine.Object Instance, string UID), Coroutine>();
        }

        // Definitions
        private struct GenericParams<T>
        {
            // Publics
            public T From;
            public T To;
            public Action<T> AssignFunction;

            // Constructors
            public GenericParams(Action<T> assingFunction, T from, T to)
            {
                AssignFunction = assingFunction;
                From = from;
                To = to;
            }
        }
    }

    static public class AnimationManager_Extensions
    {
        // UIDs
        static private readonly string GUID_TRANSFORM_POSITION = Guid.NewGuid().ToString();
        static private readonly string GUID_TRANSFORM_ROTATION = Guid.NewGuid().ToString();
        static private readonly string GUID_TRANSFORM_SCALE = Guid.NewGuid().ToString();
        static private readonly string GUID_SPRITERENDERER_COLOR = Guid.NewGuid().ToString();

        // Transform (from, to)
        static public void AnimateLocalPosition(this Transform t, Vector3 from, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate((t, GUID_TRANSFORM_POSITION), v => t.localPosition = v, from, to, duration, boomerang, finalAction);
        static public void AnimateWorldPosition(this Transform t, Vector3 from, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate((t, GUID_TRANSFORM_POSITION), v => t.position = v, from, to, duration, boomerang, finalAction);
        static public void AnimateLocalRotation(this Transform t, Quaternion from, Quaternion to, float duration,
            bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate((t, GUID_TRANSFORM_ROTATION), v => t.localRotation = v, from, to, duration, boomerang, finalAction);
        static public void AnimateWorldRotation(this Transform t, Quaternion from, Quaternion to, float duration,
            bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate((t, GUID_TRANSFORM_ROTATION), v => t.rotation = v, from, to, duration, boomerang, finalAction);
        static public void AnimateLocalScale(this Transform t, Vector3 from, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate((t, GUID_TRANSFORM_SCALE), v => t.localScale = v, from, to, duration, boomerang, finalAction);

        // Transform (to)
        static public void AnimateLocalPosition(this Transform t, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalPosition(t.localPosition, to, duration, boomerang, finalAction);
        static public void AnimateWorldPosition(this Transform t, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
            => t.AnimateWorldPosition(t.position, to, duration, boomerang, finalAction);
        static public void AnimateLocalRotation(this Transform t, Quaternion to, float duration,
            bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalRotation(t.localRotation, to, duration, boomerang, finalAction);
        static public void AnimateWorldRotation(this Transform t, Quaternion to, float duration,
            bool boomerang = false, System.Action finalAction = null)
            => t.AnimateWorldRotation(t.rotation, to, duration, boomerang, finalAction);
        static public void AnimateLocalScale(this Transform t, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalScale(t.localScale, to, duration, boomerang, finalAction);

        // SpriteRenderer (from, to)
        static public void AnimateColor(this SpriteRenderer t, Color from, Color to, float duration, bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate((t, GUID_SPRITERENDERER_COLOR), v => t.color = v, from, to, duration, boomerang, finalAction);

        // SpriteRenderer (to)
        static public void AnimateColor(this SpriteRenderer t, Color to, float duration,
            bool boomerang = false, System.Action finalAction = null)
            => t.AnimateColor(t.color, to, duration, boomerang, finalAction);
    }
}