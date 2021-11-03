namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;
    using Tools.Extensions.UnityObjects;

    abstract public class ABaseComponent :
#if UNITY_EDITOR
        AEditable
#else
        APlayable
#endif
    {
        // Publics
        public Updatable Updatable
        => GetComponentOrThrowWarning<Updatable>();
        public T Get<T>() where T : Component
        => _componentCache.Get<T>();
        public void AddToCache<T>() where T : Component
        => _componentCache.Add<T>();

        // Privates
        private ComponentCache _componentCache;
        virtual protected void SubscribeToPlayEvents()
        { }
        virtual protected void AddToComponentCache()
        { }
        private T GetComponentOrThrowWarning<T>() where T : Component
        {
            T r = GetComponent<T>();
            if (r == null)
                WarningEventHandlerNotFound<T>();
            return r;
        }

        // Warnings
        private void WarningEventHandlerNotFound<T>()
        => Debug.LogWarning($"{nameof(ABaseComponent)} / EventHandlerNotFound   -   gameobject {name}, component {GetType().Name}, type {typeof(T).Name}");

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            _componentCache = gameObject.GetOrAddComponent<ComponentCache>();
            AddToComponentCache();
            SubscribeToPlayEvents();
        }
    }
}

