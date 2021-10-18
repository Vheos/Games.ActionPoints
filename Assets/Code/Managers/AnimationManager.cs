namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.Extensions.Math;
    using Tools.Extensions.General;
    using Tools.UtilityN;
    using AnimationGUID = System.ValueTuple<UnityEngine.GameObject, string>;

    public class AnimationManager
    {
        // Private
        static private Dictionary<AnimationGUID, Coroutine> _coroutinesByGUID;
        static private Dictionary<ComponentProperty, string> _uidsByComponentProperty;
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
        static public void Animate<T>(MonoBehaviour owner, string uid, Action<T> assignFunction, T from, T to, float duration,
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

            AnimationGUID guid = (owner.gameObject, uid);
            if (_coroutinesByGUID.TryGetNonNull(guid, out var coroutine))
                owner.StopCoroutine(coroutine);

            _coroutinesByGUID[guid] = owner.StartCoroutine(CoroutineWhile
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
        static public void Animate<T>(MonoBehaviour owner, ComponentProperty property, Action<T> assignFunction, T from, T to, float duration,
            bool boomerang = false, System.Action finalAction = null) where T : struct
        => Animate(owner, _uidsByComponentProperty[property], assignFunction, from, to, duration, boomerang, finalAction);

        // Initializers
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void StaticInitialize()
        {
            _coroutinesByGUID = new Dictionary<AnimationGUID, Coroutine>();
            _uidsByComponentProperty = new Dictionary<ComponentProperty, string>();
            foreach (var commonUID in Utility.GetEnumValues<ComponentProperty>())
                _uidsByComponentProperty.Add(commonUID, Guid.NewGuid().ToString());
        }

        // Definitions
        public enum ComponentProperty
        {
            TransformPosition,
            TransformRotation,
            TransformScale,
            SpriteRendererColor,
        }
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
        // MonoBehaviour
        static public void Animate<T>(this MonoBehaviour t, string uid, Action<T> assignFunction, T from, T to, float duration,
           bool boomerang = false, System.Action finalAction = null) where T : struct
        => AnimationManager.Animate(t, uid, assignFunction, from, to, duration, boomerang, finalAction);
        static public void Animate<T>(this MonoBehaviour t, AnimationManager.ComponentProperty property, Action<T> assignFunction, T from, T to, float duration,
           bool boomerang = false, System.Action finalAction = null) where T : struct
        => AnimationManager.Animate(t, property, assignFunction, from, to, duration, boomerang, finalAction);

        // Transform (from, to)
        static public void AnimateLocalPosition(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.TransformPosition,
           v => t.localPosition = v, from, to, duration, boomerang, finalAction);
        static public void AnimateWorldPosition(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.TransformPosition,
           v => t.position = v, from, to, duration, boomerang, finalAction);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, Quaternion from, Quaternion to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.TransformRotation,
           v => t.localRotation = v, from, to, duration, boomerang, finalAction);
        static public void AnimateWorldRotation(this Transform t, MonoBehaviour owner, Quaternion from, Quaternion to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.TransformRotation,
           v => t.rotation = v, from, to, duration, boomerang, finalAction);
        static public void AnimateLocalScale(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.TransformScale,
           v => t.localScale = v, from, to, duration, boomerang, finalAction);

        // Transform (to)
        static public void AnimateLocalPosition(this Transform t, MonoBehaviour owner, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateLocalPosition(owner, t.localPosition, to, duration, boomerang, finalAction);
        static public void AnimateWorldPosition(this Transform t, MonoBehaviour owner, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateWorldPosition(owner, t.position, to, duration, boomerang, finalAction);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, Quaternion to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateLocalRotation(owner, t.localRotation, to, duration, boomerang, finalAction);
        static public void AnimateWorldRotation(this Transform t, MonoBehaviour owner, Quaternion to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateWorldRotation(owner, t.rotation, to, duration, boomerang, finalAction);
        static public void AnimateLocalScale(this Transform t, MonoBehaviour owner, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateLocalScale(owner, t.localScale, to, duration, boomerang, finalAction);

        // SpriteRenderer (from, to)
        static public void AnimateColor(this SpriteRenderer t, MonoBehaviour owner, Color from, Color to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.SpriteRendererColor,
           v => t.color = v, from, to, duration, boomerang, finalAction);

        // SpriteRenderer (to)
        static public void AnimateColor(this SpriteRenderer t, MonoBehaviour owner, Color to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateColor(owner, t.color, to, duration, boomerang, finalAction);
    }
}