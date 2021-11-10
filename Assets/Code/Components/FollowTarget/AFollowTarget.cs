namespace Vheos.Games.ActionPoints
{
    using Tools.UnityCore;
    using UnityEngine;
    abstract public class AFollowTarget : AEventSubscriber
    {
        // Inspector
        [SerializeField] protected Transform _Target = null;
        [SerializeField] protected Vector3 _Offset = Vector3.zero;
        [SerializeField] protected Axes _LockedAxes = 0;
        [SerializeField] [Range(0f, 1f)] protected float _HalfTime = 0.25f;

        // Publics
        abstract public void Follow(Transform target, float lerpAlpha);
        public Transform Target
        {
            get => _Target;
            set => _Target = value;
        }

        // Private
        private void TryFollowTarget()
        {
            if (_Target == null)
                return;

            Follow(_Target, NewUtility.LerpHalfTimeToAlpha(_HalfTime));
        }

        // Play
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            SubscribeTo(GetHandler<Updatable>().OnUpdated, TryFollowTarget);
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