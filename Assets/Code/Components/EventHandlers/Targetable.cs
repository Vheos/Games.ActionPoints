namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Collections;

    [DisallowMultipleComponent]
    sealed public class Targetable : AEventSubscriber
    {
        // Events
        public Event<Targeter, bool> OnGainTargeting
        { get; } = new Event<Targeter, bool>();
        public Event<Targeter, bool> OnLoseTargeting
        { get; } = new Event<Targeter, bool>();

        // Publics
        public IReadOnlyList<Targeter> Targeters
        => _targeters;
        public void ClearAllTargeting()
        {
            for (int i = 0; i < _targeters.Count; i++)
                LoseTargetingFrom(_targeters[i]);
        }

        // Privates
        private readonly List<Targeter> _targeters = new List<Targeter>();
        internal void GainTargetingFrom(Targeter targeter)
        {
            if (_targeters.TryAddUnique(targeter))
                OnGainTargeting?.Invoke(targeter, _targeters.Count == 1);
        }
        internal void LoseTargetingFrom(Targeter targeter)
        {
            if (_targeters.TryRemove(targeter))
                OnLoseTargeting?.Invoke(targeter, _targeters.Count == 0);
        }
    }
}