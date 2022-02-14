namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Vheos.Tools.Extensions.General;

    [RequireComponent(typeof(Targetable))]
    public class ActionTargetable : ABaseComponent
    {
        // Events
        public readonly AutoEvent<ActionTargeter, Action, bool> OnGainTargeting = new();
        public readonly AutoEvent<ActionTargeter, Action, bool> OnLoseTargeting = new();

        // Publics
        public IReadOnlyDictionary<ActionTargeter, Action> ActionsByTargeters
        => _actionsByTargeters;
        public bool IsTargeted
        => _actionsByTargeters.Count != 0;
        public bool IsTargetedBy(ActionTargeter targeter)
        => _actionsByTargeters.ContainsKey(targeter);
        public void ClearTargeting()
        {
            if (IsTargeted)
                foreach (var actionByTargeter in _actionsByTargeters.MakeCopy())
                {
                    _actionsByTargeters.Remove(actionByTargeter.Key);
                    OnLoseTargeting.Invoke(actionByTargeter.Key, actionByTargeter.Value, _actionsByTargeters.Count == 0);
                }
        }
        public bool HasRequiredComponentsToGetTargetedWith(Action action)
        {
            foreach (var requiredComponentType in action.RequiredComponentTypes[ActionTarget.Target])
                if (!Has(requiredComponentType))
                    return false;
            return true;
        }

        // Internals
        internal bool CanGetTargetedBy(ActionTargeter targeter)
        => isActiveAndEnabled && !IsTargetedBy(targeter);
        internal bool CanGetUntargetedBy(ActionTargeter targeter)
        => isActiveAndEnabled && IsTargetedBy(targeter);
        internal void GetTargetedBy(ActionTargeter targeter, Action action)
        {
            _actionsByTargeters.Add(targeter, action);
            OnGainTargeting.Invoke(targeter, action, _actionsByTargeters.Count == 1);   // is first
        }
        internal void GetUntargetedBy(ActionTargeter targeter)
        {
            var previousAction = _actionsByTargeters[targeter];
            _actionsByTargeters.Remove(targeter);
            OnLoseTargeting.Invoke(targeter, previousAction, _actionsByTargeters.Count == 0);   // is last
        }

        // Privates
        private readonly Dictionary<ActionTargeter, Action> _actionsByTargeters = new();

        // Play
        protected override void PlayDisable()
        {
            base.PlayDisable();
            ClearTargeting();
        }
    }
}