namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    abstract public class AFollowTarget : AAutoSubscriber
    {
        // Inspector
        [SerializeField] protected Transform _Transform = null;
        [SerializeField] protected Vector3 _Vector = Vector3.zero;
        [SerializeField] protected Vector3 _Offset = Vector3.zero;
        [SerializeField] protected Axes _LockedAxes = 0;
        [SerializeField] [Range(0f, 1f)] protected float _HalfTime = 0.25f;
        [SerializeField] [Range(0f, 1f)] protected float _AnimDuration = 0.5f;

        // Publics
        public void AnimateFollow()
        {
            if (_useTransform && _Transform == null)
                return;

            enabled = false;
            FollowOnAnimate();
        }
        public Transform Target
        => _Transform;
        public void SetTarget(GameObject target, bool animate = false)
        {
            _Transform = target.transform.ChooseIf(t => t != transform);
            _useTransform = true;
            if (animate)
                AnimateFollow();
        }
        public void SetTarget(Component target, bool animate = false)
        => SetTarget(target.gameObject, animate);
        public void SetTarget(Vector3 position, bool animate = false)
        {
            _Vector = position;
            _useTransform = false;
            if (animate)
                AnimateFollow();
        }

        // Private
        protected bool _useTransform;
        private void TryFollowTargetOnUpdate()
        {
            if (_useTransform && _Transform == null)
                return;

            FollowOnUpdate();
        }
        abstract protected void FollowOnUpdate();
        abstract protected void FollowOnAnimate();
        abstract protected Vector3 TargetVector
        { get; }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdate, TryFollowTargetOnUpdate);
        }
        public override void EditAwake()
        {
            base.EditAwake();
            _useTransform = _Transform != null;
        }

        // Defines
        [Flags]
        public enum Axes
        {
            X = 1 << 0,
            Y = 1 << 1,
            Z = 1 << 2,
        }
    }
}