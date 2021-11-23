namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    abstract public class AFollowTarget : AEventSubscriber
    {
        // Inspector
        [SerializeField] protected Transform _Target = null;
        [SerializeField] protected Vector3 _Offset = Vector3.zero;
        [SerializeField] protected Axes _LockedAxes = 0;
        [SerializeField] [Range(0f, 1f)] protected float _HalfTime = 0.25f;
        [SerializeField] [Range(0f, 1f)] protected float _AnimDuration = 0.5f;

        // Publics
        public void AnimateFollow()
        {
            if (enabled)
            {
                enabled = false;
                FollowOnAnimate(() => enabled = true);
            }
            else
                FollowOnAnimate(() => { });
        }
        public Transform Target
        => _Target;
        public void SetTarget(GameObject target, bool animate = false)
        {
            _Target = target.transform;
            if (animate)
                AnimateFollow();
        }
        public void SetTarget(Component target, bool animate = false)
        {
            _Target = target.transform;
            if (animate)
                AnimateFollow();
        }

        // Private
        abstract protected void UpdateFollow();
        abstract protected void FollowOnAnimate(System.Action tryRestoreEnabled);
        private void TryFollowTargetOnUpdate()
        {
            if (_Target != null)
                UpdateFollow();
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdated, TryFollowTargetOnUpdate);
        }

        // Defines
        [System.Flags]
        public enum Axes
        {
            X = 1 << 0,
            Y = 1 << 1,
            Z = 1 << 2,
        }
    }
}