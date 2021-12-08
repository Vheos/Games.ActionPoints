namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    
    [DisallowMultipleComponent]
    sealed public class Movable : AAutoSubscriber
    {
        // Events
        public AutoEvent OnStartMoving
        { get; } = new AutoEvent();
        public AutoEvent<Vector3, Vector3> OnMove
        { get; } = new AutoEvent<Vector3, Vector3>();
        public AutoEvent OnStop
        { get; } = new AutoEvent();

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
                    OnStartMoving?.Invoke();
                OnMove?.Invoke(_previousPosition, currentPosition);
            }
            else if (_previousHasMoved)
                OnStop?.Invoke();

            _previousPosition = currentPosition;
            _previousHasMoved = currentHasMoved;
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdate, TryInvokeEvents);
        }
        protected override void PlayEnable()
        {
            base.PlayEnable();
            _previousPosition = transform.position;
            _previousHasMoved = false;
        }
    }
}