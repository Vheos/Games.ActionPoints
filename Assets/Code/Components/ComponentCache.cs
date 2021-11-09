/*
namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;

    public class ComponentCache : APlayable
    {
        // Publics
        public void Add<T>()
        {
            Type type = typeof(T);
            if (_cachedComponentsByType.ContainsKey(type))
                return;
            if (!TryGetComponent(type, out var component))
            {
                WarningComponentNotFound(type);
                return;
            }
            _cachedComponentsByType.Add(type, component);
        }
        public T Get<T>() where T : Component
        => (T)_cachedComponentsByType[typeof(T)];

        // Privates
        private Dictionary<Type, Component> _cachedComponentsByType;

        // Warnings
        private void WarningComponentNotFound(Type type)
        => Debug.LogWarning($"{nameof(ComponentCache)} / ComponentNotFound   -   gameobject {name}, component {GetType().Name}, type {type.Name}");

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _cachedComponentsByType = new Dictionary<Type, Component>();
        }

#if UNITY_EDITOR
        // Debug
        [ContextMenu(nameof(PrintLocalCache))]
        public void PrintLocalCache()
        {
            int localCachedCount = _cachedComponentsByType.Count;
            int localAllCount = GetComponents<Component>().Length - 1;
            Debug.Log($"{name.ToUpper()} ({localCachedCount}/{localAllCount})");
            foreach (var componentByType in _cachedComponentsByType)
                Debug.Log($"\t- {componentByType.Key.Name}");
            Debug.Log($"");
        }
#endif
    }
}
*/