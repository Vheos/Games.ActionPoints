namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using System.Linq;
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-1)]
    abstract public class AComponentManager<T> : AUpdatable where T : Behaviour
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

        // Mono
        public override void PlayAwake()
        {
            base.PlayAwake();
            _instance = this;
            _components = new List<T>(FindObjectsOfType<T>(true));
        }
    }
}