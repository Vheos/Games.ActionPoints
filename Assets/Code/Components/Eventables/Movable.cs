namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;

    [DisallowMultipleComponent]
    sealed public class Movable : AEventSubscriber
    {
        // Events
        public Event<Vector3, Vector3> OnMoved
        { get; } = new Event<Vector3, Vector3>();
        public Event<Vector3> OnStopped
        { get; } = new Event<Vector3>();

        // Privates
        private Vector3 _previousPosition;
        private void TryInvokeEvents()
        {
            Vector3 currentPosition = transform.position;
            if (_previousPosition != currentPosition)
                OnMoved?.Invoke(_previousPosition, currentPosition);
            else
                OnStopped?.Invoke(currentPosition);
            _previousPosition = currentPosition;
        }

        // Play
        protected override void SubscribeToEvents()
        => SubscribeTo(GetComponent<Updatable>().OnUpdated, TryInvokeEvents);
    }
}