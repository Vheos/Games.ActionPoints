namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using Tools.Extensions.Math;
    using Tools.Extensions.General;
    using Tools.UtilityN;
    using AnimationGUID = System.ValueTuple<UnityEngine.MonoBehaviour, string>;
    using static AnimationManager;

    static public class AnimationManager
    {
        // Publics
        static public void Animate<T>(MonoBehaviour owner, string uid, Action<T> assignFunction, T from, T to,
            float duration, bool boomerang = false, System.Action finalAction = null) where T : struct
        {
            // Group override duration and finalAction
            if (_isGroup)
            {
                if (_groupDuration > 0f)
                    duration = _groupDuration;
                if (_groupFinalAction != null)
                {
                    finalAction = _groupFinalAction;
                    _groupFinalAction = null;
                }
            }

            // Optimize instant animations
            T finalValue = boomerang ? from : to;
            if (duration <= 0f)
            {
                assignFunction(finalValue);
                finalAction?.Invoke();
                return;
            }

            // Initialize parameters
            float startTime = Time.time - Time.deltaTime;
            float elapsed() => Time.time - startTime;
            Func<float> qurveValue = boomerang switch
            {
                false => () => Qurve.ValueAt(elapsed() / duration),
                true => () => Qurve.ValueAt(elapsed() / duration).Sub(0.5f).Abs().Neg().Add(0.5f).Mul(2f),
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

            // Group override GUID and reset
            AnimationGUID guid;
            if (_isGroup)
            {
                guid = _groupGUID;
            }
            else
            {
                guid = (owner, uid);
                ResetCoroutineList(guid);
            }

            // Start and add new coroutine           
            Coroutine newCoroutine = guid.Item1.StartCoroutine(Coroutines.While
            (
                () => elapsed() < duration,
                typedAssignFunction,
                () =>
                {
                    assignFunction(finalValue);
                    finalAction?.Invoke();
                }
            ));
            _coroutineListsByGUID[guid].Add(newCoroutine);
        }
        static public void GroupAnimate<T>(Action<T> assignFunction, T from, T to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null) where T : struct
            => Animate(null, null, assignFunction, from, to, duration, boomerang, finalAction);
        static public void Wait(MonoBehaviour owner, string uid, float duration, System.Action action)
        {
            AnimationGUID guid = (owner, uid);
            ResetCoroutineList(guid);
            Coroutine newCoroutine = owner.StartCoroutine(Coroutines.AfterSeconds(duration, action));
            _coroutineListsByGUID[guid].Add(newCoroutine);
        }
        static public IDisposable Group(MonoBehaviour owner, string uid, float duration = 0f, System.Action finalAction = null)
        {
            InitializeGroup((owner, uid), duration, finalAction);
            return _groupDisposable;
        }
        static public string GetUID(ComponentProperty property)
        => _uidsByComponentProperty[property];

        // Private (common)
        static private Dictionary<AnimationGUID, HashSet<Coroutine>> _coroutineListsByGUID;
        static private Dictionary<ComponentProperty, string> _uidsByComponentProperty;
        static private void ResetCoroutineList(AnimationGUID guid)
        {
            if (_coroutineListsByGUID.ContainsKey(guid))
            {
                foreach (var coroutine in _coroutineListsByGUID[guid])
                    if (coroutine != null)
                        guid.Item1.StopCoroutine(coroutine);
                _coroutineListsByGUID[guid].Clear();
            }
            else
                _coroutineListsByGUID.Add(guid, new HashSet<Coroutine>());
        }
        // Privates (group animation)
        static private bool _isGroup;
        static private AnimationGUID _groupGUID;
        static private float _groupDuration;
        static private System.Action _groupFinalAction;
        static private CustomDisposable _groupDisposable;
        static private void InitializeGroup(AnimationGUID guid, float duration, System.Action finalAction)
        {
            _isGroup = true;
            _groupGUID = guid;
            _groupDuration = duration;
            _groupFinalAction = finalAction;
            ResetCoroutineList(guid);
        }
        static private void FinalizeGroup()
        {
            _isGroup = false;
            _groupGUID = (null, null);
            _groupDuration = 0f;
            _groupFinalAction = null;
        }

        // Initializers
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void StaticInitialize()
        {
            _coroutineListsByGUID = new Dictionary<AnimationGUID, HashSet<Coroutine>>();
            _uidsByComponentProperty = new Dictionary<ComponentProperty, string>();
            foreach (var commonUID in Utility.GetEnumValues<ComponentProperty>())
                _uidsByComponentProperty.Add(commonUID, Guid.NewGuid().ToString());
            _groupDisposable = new CustomDisposable(FinalizeGroup);
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
            Debug.Log($"COROUTINES BY GUID ({_coroutineListsByGUID.Count})");
            foreach (var coroutineListByGUID in _coroutineListsByGUID)
            {
                AnimationGUID guid = coroutineListByGUID.Key;
                HashSet<Coroutine> coroutineList = coroutineListByGUID.Value;
                string gameObject = guid.Item1 == null ? "null" : guid.Item1.name;
                string uid = guid.Item2 ?? "null";
                string coroutineCount = coroutineList == null ? "null" : coroutineList.Count.ToString();
                Debug.Log($"\t[{gameObject}, {uid}] = {coroutineCount}");
            }
        }
#endif
    }


    static public class AnimationManager_Extensions
    {
        #region MonoBehaviour
        // T
        static public void Animate<T>(this MonoBehaviour t, string uid, Action<T> assignFunction, T from, T to,
            float duration, bool boomerang = false, System.Action finalAction = null) where T : struct
            => AnimationManager.Animate(t, uid, assignFunction, from, to, duration, boomerang, finalAction);
        static public void Animate<T>(this MonoBehaviour t, ComponentProperty property, Action<T> assignFunction, T from, T to,
            float duration, bool boomerang = false, System.Action finalAction = null) where T : struct
            => AnimationManager.Animate(t, GetUID(property), assignFunction, from, to, duration, boomerang, finalAction);
        static public void GroupAnimate<T>(this MonoBehaviour t, Action<T> assignFunction, T from, T to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null) where T : struct
            => AnimationManager.Animate(null, null, assignFunction, from, to, duration, boomerang, finalAction);
        #endregion

        #region Transform
        // Position
        static public void AnimatePosition(this Transform t, MonoBehaviour owner, string uid, Vector3 from, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate(owner, uid, v => t.position = v, from, to, duration, boomerang, finalAction);
        static public void AnimatePosition(this Transform t, MonoBehaviour owner, string uid, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimatePosition(owner, uid, t.position, to, duration, boomerang, finalAction);
        static public void AnimatePosition(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimatePosition(owner, GetUID(ComponentProperty.TransformPosition), from, to, duration, boomerang, finalAction);
        static public void AnimatePosition(this Transform t, MonoBehaviour owner, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimatePosition(owner, GetUID(ComponentProperty.TransformPosition), t.position, to, duration, boomerang, finalAction);
        // Group
        static public void GroupAnimatePosition(this Transform t, Vector3 from, Vector3 to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimatePosition(null, null, from, to, duration, boomerang, finalAction);
        static public void GroupAnimatePosition(this Transform t, Vector3 to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimatePosition(null, null, t.position, to, duration, boomerang, finalAction);

        // LocalPosition
        static public void AnimateLocalPosition(this Transform t, MonoBehaviour owner, string uid, Vector3 from, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate(owner, uid, v => t.localPosition = v, from, to, duration, boomerang, finalAction);
        static public void AnimateLocalPosition(this Transform t, MonoBehaviour owner, string uid, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalPosition(owner, uid, t.localPosition, to, duration, boomerang, finalAction);
        static public void AnimateLocalPosition(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalPosition(owner, GetUID(ComponentProperty.TransformPosition), from, to, duration, boomerang, finalAction);
        static public void AnimateLocalPosition(this Transform t, MonoBehaviour owner, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalPosition(owner, GetUID(ComponentProperty.TransformPosition), t.localPosition, to, duration, boomerang, finalAction);
        // Group
        static public void GroupAnimateLocalPosition(this Transform t, Vector3 from, Vector3 to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalPosition(null, null, from, to, duration, boomerang, finalAction);
        static public void GroupAnimateLocalPosition(this Transform t, Vector3 to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalPosition(null, null, t.localPosition, to, duration, boomerang, finalAction);

        // Rotation (Quaternion)
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, string uid, Quaternion from, Quaternion to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate(owner, uid, v => t.rotation = v, from, to, duration, boomerang, finalAction);
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, string uid, Quaternion to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateRotation(owner, uid, t.rotation, to, duration, boomerang, finalAction);
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, Quaternion from, Quaternion to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateRotation(owner, GetUID(ComponentProperty.TransformRotation), from, to, duration, boomerang, finalAction);
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, Quaternion to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateRotation(owner, GetUID(ComponentProperty.TransformRotation), t.rotation, to, duration, boomerang, finalAction);
        // Group
        static public void GroupAnimateRotation(this Transform t, Quaternion from, Quaternion to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateRotation(null, null, from, to, duration, boomerang, finalAction);
        static public void GroupAnimateRotation(this Transform t, Quaternion to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateRotation(null, null, t.rotation, to, duration, boomerang, finalAction);

        // Rotation (Vector3)
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, string uid, Vector3 from, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate(owner, uid, v => t.rotation = Quaternion.Euler(v), from, to, duration, boomerang, finalAction);
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, string uid, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateRotation(owner, uid, t.rotation.eulerAngles, to, duration, boomerang, finalAction);
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateRotation(owner, GetUID(ComponentProperty.TransformRotation), from, to, duration, boomerang, finalAction);
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateRotation(owner, GetUID(ComponentProperty.TransformRotation), t.rotation.eulerAngles, to, duration, boomerang, finalAction);
        // Group
        static public void GroupAnimateRotation(this Transform t, Vector3 from, Vector3 to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateRotation(null, null, from, to, duration, boomerang, finalAction);
        static public void GroupAnimateRotation(this Transform t, Vector3 to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateRotation(null, null, t.rotation.eulerAngles, to, duration, boomerang, finalAction);

        // LocalRotation (Quaternion)
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, string uid, Quaternion from, Quaternion to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate(owner, uid, v => t.localRotation = v, from, to, duration, boomerang, finalAction);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, string uid, Quaternion to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalRotation(owner, uid, t.localRotation, to, duration, boomerang, finalAction);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, Quaternion from, Quaternion to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalRotation(owner, GetUID(ComponentProperty.TransformRotation), from, to, duration, boomerang, finalAction);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, Quaternion to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalRotation(owner, GetUID(ComponentProperty.TransformRotation), t.localRotation, to, duration, boomerang, finalAction);
        // Group
        static public void GroupAnimateLocalRotation(this Transform t, Quaternion from, Quaternion to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalRotation(null, null, from, to, duration, boomerang, finalAction);
        static public void GroupAnimateLocalRotation(this Transform t, Quaternion to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalRotation(null, null, t.localRotation, to, duration, boomerang, finalAction);

        // LocalRotation (Vector3)
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, string uid, Vector3 from, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate(owner, uid, v => t.localRotation = Quaternion.Euler(v), from, to, duration, boomerang, finalAction);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, string uid, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalRotation(owner, uid, t.localRotation.eulerAngles, to, duration, boomerang, finalAction);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalRotation(owner, GetUID(ComponentProperty.TransformRotation), from, to, duration, boomerang, finalAction);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalRotation(owner, GetUID(ComponentProperty.TransformRotation), t.localRotation.eulerAngles, to, duration, boomerang, finalAction);
        // Group
        static public void GroupAnimateLocalRotation(this Transform t, Vector3 from, Vector3 to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalRotation(null, null, from, to, duration, boomerang, finalAction);
        static public void GroupAnimateLocalRotation(this Transform t, Vector3 to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalRotation(null, null, t.localRotation.eulerAngles, to, duration, boomerang, finalAction);

        // LocalScale
        static public void AnimateLocalScale(this Transform t, MonoBehaviour owner, string uid, Vector3 from, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate(owner, uid, v => t.localScale = v, from, to, duration, boomerang, finalAction);
        static public void AnimateLocalScale(this Transform t, MonoBehaviour owner, string uid, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalScale(owner, uid, t.localScale, to, duration, boomerang, finalAction);
        static public void AnimateLocalScale(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalScale(owner, GetUID(ComponentProperty.TransformScale), from, to, duration, boomerang, finalAction);
        static public void AnimateLocalScale(this Transform t, MonoBehaviour owner, Vector3 to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalScale(owner, GetUID(ComponentProperty.TransformScale), t.localScale, to, duration, boomerang, finalAction);
        // Group
        static public void GroupAnimateLocalScale(this Transform t, Vector3 from, Vector3 to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalScale(null, null, from, to, duration, boomerang, finalAction);
        static public void GroupAnimateLocalScale(this Transform t, Vector3 to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateLocalScale(null, null, t.localScale, to, duration, boomerang, finalAction);
        #endregion

        #region SpriteRenderer
        // Color
        static public void AnimateColor(this SpriteRenderer t, MonoBehaviour owner, string uid, Color from, Color to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate(owner, uid, v => t.color = v, from, to, duration, boomerang, finalAction);
        static public void AnimateColor(this SpriteRenderer t, MonoBehaviour owner, string uid, Color to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateColor(owner, uid, t.color, to, duration, boomerang, finalAction);
        static public void AnimateColor(this SpriteRenderer t, MonoBehaviour owner, Color from, Color to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateColor(owner, GetUID(ComponentProperty.SpriteRendererColor), from, to, duration, boomerang, finalAction);
        static public void AnimateColor(this SpriteRenderer t, MonoBehaviour owner, Color to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateColor(owner, GetUID(ComponentProperty.SpriteRendererColor), t.color, to, duration, boomerang, finalAction);
        // Group
        static public void GroupAnimateColor(this SpriteRenderer t, Color from, Color to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateColor(null, null, from, to, duration, boomerang, finalAction);
        static public void GroupAnimateColor(this SpriteRenderer t, Color to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateColor(null, null, t.color, to, duration, boomerang, finalAction);
        #endregion SpriteRenderer

        #region TextMeshPro
        // Alpha
        static public void AnimateAlpha(this TextMeshPro t, MonoBehaviour owner, string uid, float from, float to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => AnimationManager.Animate(owner, uid, v => t.alpha = v, from, to, duration, boomerang, finalAction);
        static public void AnimateAlpha(this TextMeshPro t, MonoBehaviour owner, string uid, float to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateAlpha(owner, uid, t.alpha, to, duration, boomerang, finalAction);
        static public void AnimateAlpha(this TextMeshPro t, MonoBehaviour owner, float from, float to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateAlpha(owner, GetUID(ComponentProperty.SpriteRendererColor), from, to, duration, boomerang, finalAction);
        static public void AnimateAlpha(this TextMeshPro t, MonoBehaviour owner, float to,
            float duration, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateAlpha(owner, GetUID(ComponentProperty.SpriteRendererColor), t.alpha, to, duration, boomerang, finalAction);
        // Group
        static public void GroupAnimateAlpha(this TextMeshPro t, float from, float to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateAlpha(null, null, from, to, duration, boomerang, finalAction);
        static public void GroupAnimateAlpha(this TextMeshPro t, float to,
            float duration = 0f, bool boomerang = false, System.Action finalAction = null)
            => t.AnimateAlpha(null, null, t.alpha, to, duration, boomerang, finalAction);
        #endregion
    }
}

/*
static public void CleanUpDictionary()
{
    Debug.Log($"Cleaning up {nameof(AnimationManager)}'s {nameof(_coroutineListsByGUID)} dictionary:");
    Debug.Log($"\tBefore: {_coroutineListsByGUID.Count}");

    HashSet<AnimationGUID> pendingRemoves = new HashSet<AnimationGUID>();
    foreach (var coroutineByGUID in _coroutineListsByGUID)
        if (coroutineByGUID.Key.Item1 == null || coroutineByGUID.Value == null)
            pendingRemoves.Add(coroutineByGUID.Key);

    foreach (var pendingRemove in pendingRemoves)
        _coroutineListsByGUID.Remove(pendingRemove);

    Debug.Log($"\tRemoved: {pendingRemoves.Count}");
    Debug.Log($"\tAfter: {_coroutineListsByGUID.Count}");
}
*/