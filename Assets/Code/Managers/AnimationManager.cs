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
    using TMPro;

    static public class AnimationManager
    {
        // Publics
        static public void Animate<T>(MonoBehaviour owner, string uid, Action<T> assignFunction, T from, T to, float duration,
            bool boomerang = false, System.Action finalAction = null) where T : struct
        {
            float startTime = Time.time;
            float elapsed() => Time.time - startTime;
            Func<float> qurveValue = boomerang switch
            {
                false => () => Qurve.ValueAt(elapsed() / duration),
                true => () => Qurve.ValueAt(elapsed() / duration).Sub(0.5f).Abs().Neg().Add(0.5f).Mul(2f),
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

            AnimationGUID guid = GetGUIDAndTryStopExisting(owner, uid);
            _coroutinesByGUID[guid] = owner.StartCoroutine(Coroutines.While
            (
                () => elapsed() < duration,
                typedAssignFunction,
                () =>
                {
                    _coroutinesByGUID[guid] = null;
                    assignFunction(finalValue);
                    finalAction?.Invoke();                    
                }
            ));
        }
        static public void Animate<T>(MonoBehaviour owner, ComponentProperty property, Action<T> assignFunction, T from, T to, float duration,
            bool boomerang = false, System.Action finalAction = null) where T : struct
        => Animate(owner, _uidsByComponentProperty[property], assignFunction, from, to, duration, boomerang, finalAction);
        static public void Wait(MonoBehaviour owner, string uid, float duration, System.Action action)
        {
            AnimationGUID guid = GetGUIDAndTryStopExisting(owner, uid);
            _coroutinesByGUID[guid] = owner.StartCoroutine(Coroutines.AfterSeconds(duration, action));
        }
        static public void Wait(MonoBehaviour owner, ComponentProperty property, float duration, System.Action action)
        {
            AnimationGUID guid = GetGUIDAndTryStopExisting(owner, _uidsByComponentProperty[property]);
            _coroutinesByGUID[guid] = owner.StartCoroutine(Coroutines.AfterSeconds(duration, action));
        }
        static public void CleanUpDictionary()
        {
            Debug.Log($"Cleaning up {nameof(AnimationManager)}'s {nameof(_coroutinesByGUID)} dictionary:");
            Debug.Log($"\tBefore: {_coroutinesByGUID.Count}");

            HashSet<AnimationGUID> pendingRemoves = new HashSet<AnimationGUID>();
            foreach (var coroutineByGUID in _coroutinesByGUID)
                if (coroutineByGUID.Key.Item1 == null || coroutineByGUID.Value == null)
                    pendingRemoves.Add(coroutineByGUID.Key);

            foreach (var pendingRemove in pendingRemoves)
                _coroutinesByGUID.Remove(pendingRemove);

            Debug.Log($"\tRemoved: {pendingRemoves.Count}");
            Debug.Log($"\tAfter: {_coroutinesByGUID.Count}");
        }

        // Private
        static private Dictionary<AnimationGUID, Coroutine> _coroutinesByGUID;
        static private Dictionary<ComponentProperty, string> _uidsByComponentProperty;
        static private AnimationGUID GetGUIDAndTryStopExisting(MonoBehaviour owner, string uid)
        {
            AnimationGUID guid = (owner.gameObject, uid);
            if (_coroutinesByGUID.TryGetNonNull(guid, out var coroutine))
                owner.StopCoroutine(coroutine);
            return guid;
        }

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
            TextMeshProAlpha,
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

#if UNITY_EDITOR
        // Debug
        static public void DisplayDebugInfo()
        {
            Debug.Log($"COROUTINES BY GUID ({_coroutinesByGUID.Count})");
            foreach (var coroutineByGUID in _coroutinesByGUID)
            {
                string gameObject = coroutineByGUID.Key.Item1 == null ? "null" : coroutineByGUID.Key.Item1.name;
                string uid = coroutineByGUID.Key.Item2 ?? "null";
                string coroutine = coroutineByGUID.Value == null ? "null" : coroutineByGUID.Value.GetHashCode().ToString();
                Debug.Log($"\t[{gameObject}, {uid}] = {coroutine}");
            }
        }
#endif
    }

    static public class AnimationManager_Extensions
    {
        #region MonoBehaviour
        // T
        static public void Animate<T>(this MonoBehaviour t, string uid, Action<T> assignFunction, T from, T to, float duration,
           bool boomerang = false, System.Action finalAction = null) where T : struct
        => AnimationManager.Animate(t, uid, assignFunction, from, to, duration, boomerang, finalAction);
        static public void Animate<T>(this MonoBehaviour t, AnimationManager.ComponentProperty property, Action<T> assignFunction, T from, T to, float duration,
           bool boomerang = false, System.Action finalAction = null) where T : struct
        => AnimationManager.Animate(t, property, assignFunction, from, to, duration, boomerang, finalAction);
        #endregion
        #region Transform
        // LocalPosition
        static public void AnimateLocalPosition(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.TransformPosition,
           v => t.localPosition = v, from, to, duration, boomerang, finalAction);
        static public void AnimateLocalPosition(this Transform t, MonoBehaviour owner, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateLocalPosition(owner, t.localPosition, to, duration, boomerang, finalAction);
        // WorldPosition
        static public void AnimateWorldPosition(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.TransformPosition,
           v => t.position = v, from, to, duration, boomerang, finalAction);
        static public void AnimateWorldPosition(this Transform t, MonoBehaviour owner, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateWorldPosition(owner, t.position, to, duration, boomerang, finalAction);
        // LocalRotation
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, Quaternion from, Quaternion to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.TransformRotation,
           v => t.localRotation = v, from, to, duration, boomerang, finalAction);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, Quaternion to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateLocalRotation(owner, t.localRotation, to, duration, boomerang, finalAction);
        // WorldRotation
        static public void AnimateWorldRotation(this Transform t, MonoBehaviour owner, Quaternion from, Quaternion to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.TransformRotation,
           v => t.rotation = v, from, to, duration, boomerang, finalAction);
        static public void AnimateWorldRotation(this Transform t, MonoBehaviour owner, Quaternion to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateWorldRotation(owner, t.rotation, to, duration, boomerang, finalAction);
        // LocalScale
        static public void AnimateLocalScale(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.TransformScale,
           v => t.localScale = v, from, to, duration, boomerang, finalAction);
        static public void AnimateLocalScale(this Transform t, MonoBehaviour owner, Vector3 to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateLocalScale(owner, t.localScale, to, duration, boomerang, finalAction);
        #endregion
        #region SpriteRenderer
        // Color
        static public void AnimateColor(this SpriteRenderer t, MonoBehaviour owner, Color from, Color to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.SpriteRendererColor,
           v => t.color = v, from, to, duration, boomerang, finalAction);
        static public void AnimateColor(this SpriteRenderer t, MonoBehaviour owner, Color to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateColor(owner, t.color, to, duration, boomerang, finalAction);
        #endregion SpriteRenderer
        #region TextMeshPro
        // Alpha
        static public void AnimateAlpha(this TextMeshPro t, MonoBehaviour owner, float from, float to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => AnimationManager.Animate(owner, AnimationManager.ComponentProperty.TextMeshProAlpha,
           v => t.alpha = v, from, to, duration, boomerang, finalAction);
        static public void AnimateAlpha(this TextMeshPro t, MonoBehaviour owner, float to, float duration,
            bool boomerang = false, System.Action finalAction = null)
        => t.AnimateAlpha(owner, t.alpha, to, duration, boomerang, finalAction);
        #endregion
    }
}