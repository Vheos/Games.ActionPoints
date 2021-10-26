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
        // Events
        public event System.Action OnPlayUpdate
        {
            add
            {
                if (TryGetComponentOrThrowWarning<Updatable>(out var handler, nameof(OnPlayUpdate)))
                    handler.OnPlayUpdate += value;
            }
            remove
            {
                if (TryGetComponentOrThrowWarning<Updatable>(out var handler, nameof(OnPlayUpdate)))
                    handler.OnPlayUpdate -= value;
            }
        }
        public event System.Action OnPlayUpdateLate
        {
            add
            {
                if (TryGetComponentOrThrowWarning<Updatable>(out var handler, nameof(OnPlayUpdateLate)))
                    handler.OnPlayUpdateLate += value;
            }
            remove
            {
                if (TryGetComponentOrThrowWarning<Updatable>(out var handler, nameof(OnPlayUpdateLate)))
                    handler.OnPlayUpdateLate -= value;
            }
        }
        public event System.Action OnPlayUpdateFixed
        {
            add
            {
                if (TryGetComponentOrThrowWarning<Updatable>(out var handler, nameof(OnPlayUpdateFixed)))
                    handler.OnPlayUpdateFixed += value;
            }
            remove
            {
                if (TryGetComponentOrThrowWarning<Updatable>(out var handler, nameof(OnPlayUpdateFixed)))
                    handler.OnPlayUpdateFixed -= value;
            }
        }
        public event System.Action OnMouseGainHighlight
        {
            add
            {
                if (TryGetComponentOrThrowWarning<Mousable>(out var handler, nameof(OnMouseGainHighlight)))
                    handler.OnMouseGainHighlight += value;
            }
            remove
            {
                if (TryGetComponentOrThrowWarning<Mousable>(out var handler, nameof(OnMouseGainHighlight)))
                    handler.OnMouseGainHighlight -= value;
            }
        }
        public event System.Action OnMouseLoseHighlight
        {
            add
            {
                if (TryGetComponentOrThrowWarning<Mousable>(out var handler, nameof(OnMouseLoseHighlight)))
                    handler.OnMouseLoseHighlight += value;
            }
            remove
            {
                if (TryGetComponentOrThrowWarning<Mousable>(out var handler, nameof(OnMouseLoseHighlight)))
                    handler.OnMouseLoseHighlight -= value;
            }
        }
        public event System.Action<CursorManager.Button, Vector3> OnMousePress
        {
            add
            {
                if (TryGetComponentOrThrowWarning<Mousable>(out var handler, nameof(OnMousePress)))
                    handler.OnMousePress += value;
            }
            remove
            {
                if (TryGetComponentOrThrowWarning<Mousable>(out var handler, nameof(OnMousePress)))
                    handler.OnMousePress -= value;
            }
        }
        public event System.Action<CursorManager.Button, Vector3> OnMouseHold
        {
            add
            {
                if (TryGetComponentOrThrowWarning<Mousable>(out var handler, nameof(OnMouseHold)))
                    handler.OnMouseHold += value;
            }
            remove
            {
                if (TryGetComponentOrThrowWarning<Mousable>(out var handler, nameof(OnMouseHold)))
                    handler.OnMouseHold -= value;
            }
        }
        public event System.Action<CursorManager.Button, Vector3> OnMouseRelease
        {
            add
            {
                if (TryGetComponentOrThrowWarning<Mousable>(out var handler, nameof(OnMouseRelease)))
                    handler.OnMouseRelease += value;
            }
            remove
            {
                if (TryGetComponentOrThrowWarning<Mousable>(out var handler, nameof(OnMouseRelease)))
                    handler.OnMouseRelease -= value;
            }
        }

        // Virtuals
        virtual protected void SubscribeToEvents()
        { }
        virtual protected Type[] ComponentsTypesToCache
        => new Type[0];

        // Publics
        public T Get<T>() where T : Component
        => (T)_cachedComponentsByType[typeof(T)];

        // Privates
        private Dictionary<Type, Component> _cachedComponentsByType;
        private void CacheComponents()
        {
            _cachedComponentsByType = new Dictionary<Type, Component>();
            foreach (var type in ComponentsTypesToCache)
            {
                if (!type.IsAssignableTo<Component>())
                {
                    ThrowWarningAboutWrongType(type);
                    continue;
                }
                if (_cachedComponentsByType.ContainsKey(type))
                {
                    ThrowWarningAboutDuplicateType(type);
                    continue;
                }
                if (!TryGetComponent(type, out var component))
                {
                    ThrowWarningAboutComponentNotFound(type);
                    continue;
                }
                _cachedComponentsByType.Add(type, component);
            }
        }
        private bool TryGetComponentOrThrowWarning<T>(out T component, string eventName) where T : Component
        {
            if (TryGetComponent(out component))
                return true;

            ThrowWarningAboutEventHandlerNotFound(eventName);
            return false;
        }

        // Warnings
        private void ThrowWarningAboutEventHandlerNotFound(string eventName)
        => Debug.LogWarning($"EventHandlerNotFound   -   gameobject {name}, component {GetType().Name}, event {eventName}");
        private void ThrowWarningAboutWrongType(Type type)
        => Debug.LogWarning($"WrongType   -   gameobject {name}, component {GetType().Name}, type {type}");
        private void ThrowWarningAboutDuplicateType(Type type)
        => Debug.LogWarning($"DuplicateType   -   gameobject {name}, component {GetType().Name}, type {type}");
        private void ThrowWarningAboutComponentNotFound(Type type)
        => Debug.LogWarning($"ComopnentNotFound   -   gameobject {name}, component {GetType().Name}, type {type}");

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            CacheComponents();
            SubscribeToEvents();
        }
    }
}