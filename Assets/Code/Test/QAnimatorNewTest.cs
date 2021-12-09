namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;

    public class QAnimatorNewTest : AAutoSubscriber
    {
        [SerializeField] protected Vector3 _Offset;
        [SerializeField] protected AnimationCurve _Curve;
        [SerializeField] [Range(0f, 10f)] protected float _Duration;
        private EventInfo[] _animEventInfos;

        private void OnUpdate()
        {
            if (KeyCode.KeypadPlus.Released())
                QAnimator.Animate(t => transform.localRotation *= t, Quaternion.Euler(_Offset), _Duration);
            if (KeyCode.KeypadMultiply.Released())
                this.AnimateLocalScale(_Offset, _Duration);
        }

        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdate, OnUpdate);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _animEventInfos = new[]
            {
                new EventInfo(EventThresholdType.Progress, 0.5f, () => Debug.Log($"HALFWAY THERE!")),
                new EventInfo(EventThresholdType.Value, 1f, () => Debug.Log($"THERE!!!")),
            };
        }
    }
}