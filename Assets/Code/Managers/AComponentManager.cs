namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;

    abstract public class AComponentManager<TManager, TComponent> : AManager<TManager>
        where TManager : AManager<TManager>
        where TComponent : Behaviour
    {
        // Publics
        static public TComponent FirstActive
        => _components.FirstOrDefault(c => c != null && c.isActiveAndEnabled);
        static public TComponent AddComponentTo(ABaseComponent t)
        {
            TComponent newComponent = t.Add<TComponent>();
            _components.Add(newComponent);
            return newComponent;
        }

        // Privates
        static protected List<TComponent> _components;

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _components = new List<TComponent>(FindObjectsOfType<TComponent>(true));
        }
    }
}