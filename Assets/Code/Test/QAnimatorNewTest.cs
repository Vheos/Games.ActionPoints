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
                this.NewTween()
                   .SetDuration(_Duration)
                   .LocalRotation(Quaternion.Euler(_Offset));
            if (KeyCode.KeypadMultiply.Released())
                this.NewTween()
                   .SetDuration(_Duration)
                   .LocalScaleRatio(_Offset);
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
                new EventInfo(EventThresholdVariable.Progress, 0.5f, () => Debug.Log($"HALFWAY THERE!")),
                new EventInfo(EventThresholdVariable.CurveValue, 1f, () => Debug.Log($"THERE!!!")),
            };
        }
    }
}