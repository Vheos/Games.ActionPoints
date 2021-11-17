namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Event = Tools.UnityCore.Event;

    [DisallowMultipleComponent]
    sealed public class Movable : AEventSubscriber
    {
        // Events
        public Event OnStartedMoving
        { get; } = new Event();
        public Event<Vector3, Vector3> OnMoved
        { get; } = new Event<Vector3, Vector3>();
        public Event OnStoppedMoving
        { get; } = new Event();

        // Privates
        private Vector3 _previousPosition;
        private bool _previousHasMoved;
        private void TryInvokeEvents()
        {
            Vector3 currentPosition = transform.position;
            bool currentHasMoved = currentPosition != _previousPosition;

            if (currentHasMoved)
            {
                if (!_previousHasMoved)
                    OnStartedMoving?.Invoke();
                OnMoved?.Invoke(_previousPosition, currentPosition);
            }
            else if (_previousHasMoved)
                OnStoppedMoving?.Invoke();

            _previousPosition = currentPosition;
            _previousHasMoved = currentHasMoved;
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdated, TryInvokeEvents);
        }
        protected override void PlayEnable()
        {
            base.PlayEnable();
            _previousPosition = transform.position;
            _previousHasMoved = false;
        }
    }
}