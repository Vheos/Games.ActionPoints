namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.General;

    abstract public class ABaseComponent :
#if UNITY_EDITOR
        AEditable
#else
        APlayable
#endif
    {
        // Publics
        public T Get<T>() where T : Component
        => (T)_cachedComponentsByTypeByGameObject[gameObject][typeof(T)];
        public Updatable Updatable
        => GetComponentOrThrowWarning<Updatable>();
        public Mousable Mousable
        => GetComponentOrThrowWarning<Mousable>();
        public SpriteChangable SpriteChangable
        => GetComponentOrThrowWarning<SpriteChangable>();
        public Movable Movable
        => GetComponentOrThrowWarning<Movable>();

        // Privates
        virtual protected void SubscribeToPlayEvents()
        { }
        virtual protected Type[] ComponentsTypesToCache
        => null;
        static private Dictionary<GameObject, Dictionary<Type, Component>> _cachedComponentsByTypeByGameObject;
        private void TryCacheComponents()
        {
            if (!ComponentsTypesToCache.TryNonNull(out var componentTypesToCache))
                return;

            if (!_cachedComponentsByTypeByGameObject.ContainsKey(gameObject))
                _cachedComponentsByTypeByGameObject[gameObject] = new Dictionary<Type, Component>();

            foreach (var type in componentTypesToCache)
            {
                if (_cachedComponentsByTypeByGameObject[gameObject].ContainsKey(type))
                    continue;
                if (!type.IsAssignableTo<Component>())
                {
                    WarningWrongType(type);
                    continue;
                }
                if (!TryGetComponent(type, out var component))
                {
                    WarningComponentNotFound(type);
                    continue;
                }
                _cachedComponentsByTypeByGameObject[gameObject].Add(type, component);
            }
        }
        private T GetComponentOrThrowWarning<T>() where T : Component
        {
            T r = GetComponent<T>();
            if (r == null)
                WarningEventHandlerNotFound<T>();
            return r;
        }

        // Warnings
        private void WarningEventHandlerNotFound<T>()
        => Debug.LogWarning($"{nameof(ABaseComponent)} / EventHandlerNotFound   -   gameobject {name}, component {GetType().Name}, type {typeof(T)}");
        private void WarningWrongType(Type type)
        => Debug.LogWarning($"{nameof(ABaseComponent)} / WrongType   -   gameobject {name}, component {GetType().Name}, type {type}");
        private void WarningComponentNotFound(Type type)
        => Debug.LogWarning($"{nameof(ABaseComponent)} / ComopnentNotFound   -   gameobject {name}, component {GetType().Name}, type {type}");

        // Initializers
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void StaticInitialize()
        => _cachedComponentsByTypeByGameObject = new Dictionary<GameObject, Dictionary<Type, Component>>();

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            TryCacheComponents();
            SubscribeToPlayEvents();
        }

#if UNITY_EDITOR
        // Debug
        [ContextMenu(nameof(PrintLocalCache))]
        public void PrintLocalCache()
        {
            int localCachedCount = _cachedComponentsByTypeByGameObject[gameObject].Count;
            int localAllCount = GetComponents<Component>().Length - 1;
            Debug.Log($"{name.ToUpper()} ({localCachedCount}/{localAllCount})");
            foreach (var componentByType in _cachedComponentsByTypeByGameObject[gameObject])
                Debug.Log($"\t- {componentByType.Key.Name}");
            Debug.Log($"");
        }
        [ContextMenu(nameof(PrintGlobalCache))]
        public void PrintGlobalCache()
        {
            int cachedCount = 0;
            int allCount = 0;
            foreach (var componentByTypeByGameObject in _cachedComponentsByTypeByGameObject)
            {
                int localCachedCount = componentByTypeByGameObject.Value.Count;
                int localAllCount = componentByTypeByGameObject.Key.GetComponents<Component>().Length - 1;
                Debug.Log($"{componentByTypeByGameObject.Key.name.ToUpper()} ({localCachedCount}/{localAllCount})");
                foreach (var componentByType in componentByTypeByGameObject.Value)
                    Debug.Log($"\t- {componentByType.Key.Name}");
                Debug.Log($"");
                cachedCount += localCachedCount;
                allCount += localAllCount;
            }
            Debug.Log($"Cached GameObjects: {_cachedComponentsByTypeByGameObject.Count}");
            Debug.Log($"Cached Components: {cachedCount}");
            Debug.Log($"All Components: {allCount}");
            Debug.Log($"");
        }
#endif
    }
}