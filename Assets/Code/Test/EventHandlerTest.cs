namespace Vheos.Games.ActionPoints.Test
{
    using System.Collections.Generic;
    using Tools.UnityCore;
    using UnityEngine;
    using UnityEngine.Events;

    public class EventHandlerTest : AAutoSubscriber
    {
        // Events
        public AutoEvent<Vector3, Vector3> OnPositionChanged
        { get; } = new AutoEvent<Vector3, Vector3>();
        public AutoEvent<Quaternion, Quaternion> OnRotationChanged
        { get; } = new AutoEvent<Quaternion, Quaternion>();

        // Privates
        private Vector3 _previousPosition;
        private Quaternion _previousRotation;
        private Vector3 CurrentPosition
        => transform.position;
        private Quaternion CurrentRotation
        => transform.rotation;
        private void TryInvokeOnPositionChanged()
        {
            if (_previousPosition == CurrentPosition)
                return;

            OnPositionChanged?.Invoke(_previousPosition, CurrentPosition);
            _previousPosition = CurrentPosition;
        }
        private void TryInvokeOnRotationChanged()
        {
            if (_previousRotation == CurrentRotation)
                return;

            OnRotationChanged?.Invoke(_previousRotation, CurrentRotation);
            _previousRotation = CurrentRotation;
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdate, TryInvokeOnPositionChanged);
        }
        protected override void PlayEnable()
        {
            base.PlayEnable();
            _previousPosition = CurrentPosition;
            _previousRotation = CurrentRotation;
        }

    }
}