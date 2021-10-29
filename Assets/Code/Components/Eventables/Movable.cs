namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    using Tools.Extensions.UnityObjects;
    using System;

    [DisallowMultipleComponent]
    sealed public class Movable : ABaseComponent
    {
        // Events
        public event System.Action<Vector3, Vector3> OnMoved;
        public event System.Action<Vector3> OnStopped;

        // Privates
        private Vector3 _previousPosition;

        // Play
        protected override void SubscribeToPlayEvents()
        {
            base.SubscribeToPlayEvents();
            Updatable.OnPlayUpdate += () =>
            {
                Vector3 currentPosition = transform.position;
                if (_previousPosition != currentPosition)
                    OnMoved?.Invoke(_previousPosition, currentPosition);
                else
                    OnStopped?.Invoke(currentPosition);
                _previousPosition = currentPosition;
            };
        }
    }
}