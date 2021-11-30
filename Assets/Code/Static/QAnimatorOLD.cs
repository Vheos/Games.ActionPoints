namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using Tools.UnityCore; 
    using Tools.Extensions.Math;
    using Tools.UtilityN;
    using AnimationGUID = System.ValueTuple<UnityEngine.MonoBehaviour, string>;
    using static QAnimatorOLD;   

    static public class QAnimatorOLD
    {
        // Publics
        static public void Animate<T>(MonoBehaviour owner, string uid, Action<T> assignFunction, T from, T to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve) where T : struct
        {
            // NullOwner warning
            if (owner == null)
            {
                WarningNullOwner(uid);
                return;
            }

            // Handle instant animations
            if (duration <= 0f)
            {
                assignFunction(GetFinalValue(curve, from, to));
                finalAction?.Invoke();
                return;
            }

            // InactiveGameObject warning
            if (!owner.gameObject.activeInHierarchy)
            {
                WarningInactiveGameObject(owner, uid);
                return;
            }

            // Start and add new coroutine 
            float startTime = Time.time - Time.deltaTime;
            float progress() => (Time.time - startTime) / duration;
            float lerpAlphaFunc() => GetCurveFunction(curve)(progress());
            System.Action typedAssignFunction = GetTypedAssignFunction(assignFunction, from, to, lerpAlphaFunc);
            Coroutine newCoroutine = owner.StartCoroutine(Coroutines.While
            (
                () => progress() < 1f,
                typedAssignFunction,
                () =>
                {
                    assignFunction(GetFinalValue(curve, from, to));
                    finalAction?.Invoke();
                }
            ));

            // Group override reset
            AnimationGUID guid = (owner, uid);
            InitializeCoroutineList(guid, _groupOwner == null);
            _coroutineListsByGUID[guid].Add(newCoroutine);
        }
        static public void GroupAnimate<T>(Action<T> assignFunction, T from, T to) where T : struct
        => Animate(_groupOwner, _groupUID, assignFunction, from, to, _groupDuration, null, _groupStyle);
        static public void Delay(MonoBehaviour owner, string uid, float duration, System.Action action)
        {
            // Instant action
            if (duration == 0f)
            {
                action();
                return;
            }

            // NullOwner warning
            if (owner == null)
            {
                WarningNullOwner(uid);
                return;
            }

            // InactiveGameObject warning
            if (!owner.gameObject.activeInHierarchy)
            {
                WarningInactiveGameObject(owner, uid);
                return;
            }

            Coroutine newCoroutine = owner.StartCoroutine(Coroutines.AfterSeconds(duration, action));
            AnimationGUID guid = (owner, uid);
            InitializeCoroutineList(guid, false);
            _coroutineListsByGUID[guid].Add(newCoroutine);
        }
        static public IDisposable Group(MonoBehaviour owner, string uid,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
        {
            _groupOwner = owner;
            _groupUID = uid;
            _groupDuration = duration;
            _groupFinalAction = finalAction;
            _groupStyle = curve;
            InitializeCoroutineList((owner, uid), true);
            return _groupDisposable;
        }
        static public string GetUID(ComponentProperty property)
        => _uidsByComponentProperty[property];
        static public void StopAllAnimations()
        {
            foreach (var coroutineListByGUID in _coroutineListsByGUID)
                foreach (var coroutine in coroutineListByGUID.Value)
                    if (coroutine != null)
                        coroutineListByGUID.Key.Item1.StopCoroutine(coroutine);
            _coroutineListsByGUID.Clear();
        }
        static public void Stop(MonoBehaviour owner)
        {
            foreach (var coroutineListByGUID in _coroutineListsByGUID)
            {
                MonoBehaviour guidOwner = coroutineListByGUID.Key.Item1;
                if (guidOwner == owner)
                {
                    foreach (var coroutine in coroutineListByGUID.Value)
                        if (coroutine != null)
                            guidOwner.StopCoroutine(coroutine);
                    coroutineListByGUID.Value.Clear();
                }
            }
        }
        static public void Stop(MonoBehaviour owner, string uid)
        {
            AnimationGUID guid = (owner, uid);
            if (_coroutineListsByGUID.ContainsKey(guid))
                StopAndRemoveCoroutines(guid);
        }

        // Private (common)
        static private Dictionary<AnimationGUID, HashSet<Coroutine>> _coroutineListsByGUID;
        static private Dictionary<ComponentProperty, string> _uidsByComponentProperty;
        static private T GetFinalValue<T>(Curve curve, T from, T to)
        => curve switch
        {
            Curve.Linear => to,
            Curve.Qurve => to,
            Curve.QurveInverted => to,
            Curve.Boomerang => from,
            Curve.BoomerangInverted => from,
            _ => default,
        };
        static private Func<float, float> GetCurveFunction(Curve curve)
        => curve switch
        {
            Curve.Linear => t => t,
            Curve.Qurve => t => Qurve.ValueAt(t),
            Curve.QurveInverted => t => 1f - Qurve.ValueAt(1f - t),
            Curve.Boomerang => t => Qurve.ValueAt(t).Sub(0.5f).Abs().Neg().Add(0.5f).Mul(2f),
            Curve.BoomerangInverted => t => (1f - Qurve.ValueAt(1f - t)).Sub(0.5f).Abs().Neg().Add(0.5f).Mul(2f),
            _ => t => 0f,
        };
        static private System.Action GetTypedAssignFunction<T>(Action<T> genericAssignFunc, T from, T to, Func<float> lerpAlphaFunc)
        => new GenericParams<T>(genericAssignFunc, from, to) switch
        {
            GenericParams<float> t => () => t.AssignFunction(t.From.Lerp(t.To, lerpAlphaFunc())),
            GenericParams<Vector2> t => () => t.AssignFunction(t.From.Lerp(t.To, lerpAlphaFunc())),
            GenericParams<Vector3> t => () => t.AssignFunction(t.From.Lerp(t.To, lerpAlphaFunc())),
            GenericParams<Vector4> t => () => t.AssignFunction(t.From.Lerp(t.To, lerpAlphaFunc())),
            GenericParams<Quaternion> t => () => t.AssignFunction(t.From.Lerp(t.To, lerpAlphaFunc())),
            GenericParams<Color> t => () => t.AssignFunction(t.From.Lerp(t.To, lerpAlphaFunc())),
            _ => () => genericAssignFunc(default),
        };
        static private void InitializeCoroutineList(AnimationGUID guid, bool stopAndRemoveExisting)
        {
            if (!_coroutineListsByGUID.ContainsKey(guid))
                _coroutineListsByGUID.Add(guid, new HashSet<Coroutine>());
            else if (stopAndRemoveExisting)
                StopAndRemoveCoroutines(guid);
        }
        static private void StopAndRemoveCoroutines(AnimationGUID guid)
        {
            foreach (var coroutine in _coroutineListsByGUID[guid])
                if (coroutine != null)
                    guid.Item1.StopCoroutine(coroutine);
            _coroutineListsByGUID[guid].Clear();
        }
        static private void WarningNullOwner(string uid)
        => Debug.LogWarning($"{nameof(QAnimatorOLD)} / NullOwner   -   uid {uid ?? "null"}");
        static private void WarningInactiveGameObject(MonoBehaviour owner, string uid)
        => Debug.LogWarning($"{nameof(QAnimatorOLD)} / InactiveGameObject   -   owner {owner.GetType().Name}, uid {uid ?? "null"}");

        // Privates (group animation)
        static private MonoBehaviour _groupOwner;
        static private string _groupUID;
        static private float _groupDuration;
        static private System.Action _groupFinalAction;
        static private Curve _groupStyle;
        static private CustomDisposable _groupDisposable;
        static private void FinalizeGroup()
        {
            if (_groupFinalAction != null)
                Delay(_groupOwner, _groupUID, _groupDuration, _groupFinalAction);
            _groupOwner = null;
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
        public enum Curve
        {
            Linear = 0,
            Qurve,
            QurveInverted,
            Boomerang,
            BoomerangInverted,
        }
        public enum ComponentProperty
        {
            TransformPosition = 0,
            TransformRotation,
            TransformScale,
            SpriteRendererColor,
            TextMeshProColor,
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
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve) where T : struct
            => QAnimatorOLD.Animate(t, uid, assignFunction, from, to, duration, finalAction, curve);
        static public void Animate<T>(this MonoBehaviour t, ComponentProperty property, Action<T> assignFunction, T from, T to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve) where T : struct
            => QAnimatorOLD.Animate(t, GetUID(property), assignFunction, from, to, duration, finalAction, curve);
        #endregion

        #region Transform
        // Position
        static public void AnimatePosition(this Transform t, MonoBehaviour owner, string uid, Vector3 from, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => QAnimatorOLD.Animate(owner, uid, v => t.position = v, from, to, duration, finalAction, curve);
        static public void AnimatePosition(this Transform t, MonoBehaviour owner, string uid, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimatePosition(owner, uid, t.position, to, duration, finalAction, curve);
        static public void AnimatePosition(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimatePosition(owner, GetUID(ComponentProperty.TransformPosition), from, to, duration, finalAction, curve);
        static public void AnimatePosition(this Transform t, MonoBehaviour owner, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimatePosition(owner, GetUID(ComponentProperty.TransformPosition), t.position, to, duration, finalAction, curve);
        // Group
        static public void GroupAnimatePosition(this Transform t, Vector3 from, Vector3 to)
        => QAnimatorOLD.GroupAnimate(v => t.position = v, from, to);
        static public void GroupAnimatePosition(this Transform t, Vector3 to)
        => t.GroupAnimatePosition(t.position, to);

        // LocalPosition
        static public void AnimateLocalPosition(this Transform t, MonoBehaviour owner, string uid, Vector3 from, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => QAnimatorOLD.Animate(owner, uid, v => t.localPosition = v, from, to, duration, finalAction, curve);
        static public void AnimateLocalPosition(this Transform t, MonoBehaviour owner, string uid, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateLocalPosition(owner, uid, t.localPosition, to, duration, finalAction, curve);
        static public void AnimateLocalPosition(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateLocalPosition(owner, GetUID(ComponentProperty.TransformPosition), from, to, duration, finalAction, curve);
        static public void AnimateLocalPosition(this Transform t, MonoBehaviour owner, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateLocalPosition(owner, GetUID(ComponentProperty.TransformPosition), t.localPosition, to, duration, finalAction, curve);
        // Group
        static public void GroupAnimateLocalPosition(this Transform t, Vector3 from, Vector3 to)
        => QAnimatorOLD.GroupAnimate(v => t.localPosition = v, from, to);
        static public void GroupAnimateLocalPosition(this Transform t, Vector3 to)
        => t.GroupAnimateLocalPosition(t.localPosition, to);

        // Rotation (Quaternion)
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, string uid, Quaternion from, Quaternion to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => QAnimatorOLD.Animate(owner, uid, v => t.rotation = v, from, to, duration, finalAction, curve);
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, string uid, Quaternion to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateRotation(owner, uid, t.rotation, to, duration, finalAction, curve);
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, Quaternion from, Quaternion to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateRotation(owner, GetUID(ComponentProperty.TransformRotation), from, to, duration, finalAction, curve);
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, Quaternion to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateRotation(owner, GetUID(ComponentProperty.TransformRotation), t.rotation, to, duration, finalAction, curve);
        // Group
        static public void GroupAnimateRotation(this Transform t, Quaternion from, Quaternion to)
        => QAnimatorOLD.GroupAnimate(v => t.rotation = v, from, to);
        static public void GroupAnimateRotation(this Transform t, Quaternion to)
        => t.GroupAnimateRotation(t.rotation, to);

        // Rotation (Vector3)
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, string uid, Vector3 from, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => QAnimatorOLD.Animate(owner, uid, v => t.rotation = Quaternion.Euler(v), from, to, duration, finalAction, curve);
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, string uid, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateRotation(owner, uid, t.rotation.eulerAngles, to, duration, finalAction, curve);
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateRotation(owner, GetUID(ComponentProperty.TransformRotation), from, to, duration, finalAction, curve);
        static public void AnimateRotation(this Transform t, MonoBehaviour owner, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateRotation(owner, GetUID(ComponentProperty.TransformRotation), t.rotation.eulerAngles, to, duration, finalAction, curve);
        // Group
        static public void GroupAnimateRotation(this Transform t, Vector3 from, Vector3 to)
        => QAnimatorOLD.GroupAnimate(v => t.rotation = Quaternion.Euler(v), from, to);
        static public void GroupAnimateRotation(this Transform t, Vector3 to)
        => t.GroupAnimateRotation(t.rotation.eulerAngles, to);

        // LocalRotation (Quaternion)
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, string uid, Quaternion from, Quaternion to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => QAnimatorOLD.Animate(owner, uid, v => t.localRotation = v, from, to, duration, finalAction, curve);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, string uid, Quaternion to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateLocalRotation(owner, uid, t.localRotation, to, duration, finalAction, curve);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, Quaternion from, Quaternion to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateLocalRotation(owner, GetUID(ComponentProperty.TransformRotation), from, to, duration, finalAction, curve);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, Quaternion to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateLocalRotation(owner, GetUID(ComponentProperty.TransformRotation), t.localRotation, to, duration, finalAction, curve);
        // Group
        static public void GroupAnimateLocalRotation(this Transform t, Quaternion from, Quaternion to)
        => QAnimatorOLD.GroupAnimate(v => t.localRotation = v, from, to);
        static public void GroupAnimateLocalRotation(this Transform t, Quaternion to)
        => t.GroupAnimateLocalRotation(t.localRotation, to);

        // LocalRotation (Vector3)
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, string uid, Vector3 from, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => QAnimatorOLD.Animate(owner, uid, v => t.localRotation = Quaternion.Euler(v), from, to, duration, finalAction, curve);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, string uid, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateLocalRotation(owner, uid, t.localRotation.eulerAngles, to, duration, finalAction, curve);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateLocalRotation(owner, GetUID(ComponentProperty.TransformRotation), from, to, duration, finalAction, curve);
        static public void AnimateLocalRotation(this Transform t, MonoBehaviour owner, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateLocalRotation(owner, GetUID(ComponentProperty.TransformRotation), t.localRotation.eulerAngles, to, duration, finalAction, curve);
        // Group
        static public void GroupAnimateLocalRotation(this Transform t, Vector3 from, Vector3 to)
        => QAnimatorOLD.GroupAnimate(v => t.localRotation = Quaternion.Euler(v), from, to);
        static public void GroupAnimateLocalRotation(this Transform t, Vector3 to)
        => t.GroupAnimateLocalRotation(t.localRotation.eulerAngles, to);

        // LocalScale
        static public void AnimateLocalScale(this Transform t, MonoBehaviour owner, string uid, Vector3 from, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => QAnimatorOLD.Animate(owner, uid, v => t.localScale = v, from, to, duration, finalAction, curve);
        static public void AnimateLocalScale(this Transform t, MonoBehaviour owner, string uid, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateLocalScale(owner, uid, t.localScale, to, duration, finalAction, curve);
        static public void AnimateLocalScale(this Transform t, MonoBehaviour owner, Vector3 from, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateLocalScale(owner, GetUID(ComponentProperty.TransformScale), from, to, duration, finalAction, curve);
        static public void AnimateLocalScale(this Transform t, MonoBehaviour owner, Vector3 to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateLocalScale(owner, GetUID(ComponentProperty.TransformScale), t.localScale, to, duration, finalAction, curve);
        // Group
        static public void GroupAnimateLocalScale(this Transform t, Vector3 from, Vector3 to)
        => QAnimatorOLD.GroupAnimate(v => t.localScale = v, from, to);
        static public void GroupAnimateLocalScale(this Transform t, Vector3 to)
        => t.GroupAnimateLocalScale(t.localScale, to);
        #endregion

        #region SpriteRenderer
        // Color
        static public void AnimateColor(this SpriteRenderer t, MonoBehaviour owner, string uid, Color from, Color to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => QAnimatorOLD.Animate(owner, uid, v => t.color = v, from, to, duration, finalAction, curve);
        static public void AnimateColor(this SpriteRenderer t, MonoBehaviour owner, string uid, Color to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateColor(owner, uid, t.color, to, duration, finalAction, curve);
        static public void AnimateColor(this SpriteRenderer t, MonoBehaviour owner, Color from, Color to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateColor(owner, GetUID(ComponentProperty.SpriteRendererColor), from, to, duration, finalAction, curve);
        static public void AnimateColor(this SpriteRenderer t, MonoBehaviour owner, Color to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateColor(owner, GetUID(ComponentProperty.SpriteRendererColor), t.color, to, duration, finalAction, curve);
        // Group
        static public void GroupAnimateColor(this SpriteRenderer t, Color from, Color to)
        => QAnimatorOLD.GroupAnimate(v => t.color = v, from, to);
        static public void GroupAnimateColor(this SpriteRenderer t, Color to)
        => t.GroupAnimateColor(t.color, to);

        // Alpha
        static public void AnimateAlpha(this SpriteRenderer t, MonoBehaviour owner, string uid, float from, float to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => QAnimatorOLD.Animate(owner, uid, v => t.color = t.color.NewA(v), from, to, duration, finalAction, curve);
        static public void AnimateAlpha(this SpriteRenderer t, MonoBehaviour owner, string uid, float to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateAlpha(owner, uid, t.color.a, to, duration, finalAction, curve);
        static public void AnimateAlpha(this SpriteRenderer t, MonoBehaviour owner, float from, float to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateAlpha(owner, GetUID(ComponentProperty.SpriteRendererColor), from, to, duration, finalAction, curve);
        static public void AnimateAlpha(this SpriteRenderer t, MonoBehaviour owner, float to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateAlpha(owner, GetUID(ComponentProperty.SpriteRendererColor), t.color.a, to, duration, finalAction, curve);
        // Alpha
        static public void GroupAnimateAlpha(this SpriteRenderer t, float from, float to)
        => QAnimatorOLD.GroupAnimate(v => t.color = t.color.NewA(v), from, to);
        static public void GroupAnimateAlpha(this SpriteRenderer t, float to)
        => t.GroupAnimateAlpha(t.color.a, to);
        #endregion SpriteRenderer

        #region TextMeshPro
        // Color
        static public void AnimateColor(this TextMeshPro t, MonoBehaviour owner, string uid, Color from, Color to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => QAnimatorOLD.Animate(owner, uid, v => t.color = v, from, to, duration, finalAction, curve);
        static public void AnimateColor(this TextMeshPro t, MonoBehaviour owner, string uid, Color to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateColor(owner, uid, t.color, to, duration, finalAction, curve);
        static public void AnimateColor(this TextMeshPro t, MonoBehaviour owner, Color from, Color to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateColor(owner, GetUID(ComponentProperty.TextMeshProColor), from, to, duration, finalAction, curve);
        static public void AnimateColor(this TextMeshPro t, MonoBehaviour owner, Color to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateColor(owner, GetUID(ComponentProperty.TextMeshProColor), t.color, to, duration, finalAction, curve);
        // Group
        static public void GroupAnimateColor(this TextMeshPro t, Color from, Color to)
        => QAnimatorOLD.GroupAnimate(v => t.color = v, from, to);
        static public void GroupAnimateColor(this TextMeshPro t, Color to)
        => t.GroupAnimateColor(t.color, to);

        // Alpha
        static public void AnimateAlpha(this TextMeshPro t, MonoBehaviour owner, string uid, float from, float to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => QAnimatorOLD.Animate(owner, uid, v => t.alpha = v, from, to, duration, finalAction, curve);
        static public void AnimateAlpha(this TextMeshPro t, MonoBehaviour owner, string uid, float to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateAlpha(owner, uid, t.alpha, to, duration, finalAction, curve);
        static public void AnimateAlpha(this TextMeshPro t, MonoBehaviour owner, float from, float to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateAlpha(owner, GetUID(ComponentProperty.TextMeshProColor), from, to, duration, finalAction, curve);
        static public void AnimateAlpha(this TextMeshPro t, MonoBehaviour owner, float to,
            float duration, System.Action finalAction = null, Curve curve = Curve.Qurve)
            => t.AnimateAlpha(owner, GetUID(ComponentProperty.TextMeshProColor), t.alpha, to, duration, finalAction, curve);
        // Group
        static public void GroupAnimateAlpha(this TextMeshPro t, float from, float to)
        => QAnimatorOLD.GroupAnimate(v => t.alpha = v, from, to);
        static public void GroupAnimateAlpha(this TextMeshPro t, float to)
        => t.GroupAnimateAlpha(t.alpha, to);
        #endregion
    }
}