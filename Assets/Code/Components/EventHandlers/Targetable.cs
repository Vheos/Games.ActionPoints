namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Collections;

    [DisallowMultipleComponent]
    sealed public class Targetable : AAutoSubscriber
    {
        // Events
        public AutoEvent<Targeter, bool> OnGainTargeting
        { get; } = new AutoEvent<Targeter, bool>();
        public AutoEvent<Targeter, bool> OnLoseTargeting
        { get; } = new AutoEvent<Targeter, bool>();

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