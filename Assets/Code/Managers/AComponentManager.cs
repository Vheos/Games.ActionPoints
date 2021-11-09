namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using System.Linq;

    [DefaultExecutionOrder(-1)]
    [DisallowMultipleComponent]
    abstract public class AComponentManager<T> : AEventSubscriber where T : Behaviour
    {
        // Publics
        static public T FirstActive
        => _components.FirstOrDefault(c => c != null && c.isActiveAndEnabled);
        static public T AddComponentTo(GameObject t)
        {
            T newComponent = t.AddComponent<T>();
            _components.Add(newComponent);
            return newComponent;
        }
        static public T AddComponentTo(Component t)
        => AddComponentTo(t.gameObject);

        // Privates
        static protected AComponentManager<T> _instance;
        static protected List<T> _components;

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _instance = this;
            _components = new List<T>(FindObjectsOfType<T>(true));
        }
    }
}