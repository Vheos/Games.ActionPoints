namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    abstract public class AUnityComponentWrapper<T> : ABaseComponent where T : Component
    {
        // Publics
        public T Unity
        => _unityComponent;

        // Privates
        private T _unityComponent;

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _unityComponent = Get<T>();
        }
    }
}