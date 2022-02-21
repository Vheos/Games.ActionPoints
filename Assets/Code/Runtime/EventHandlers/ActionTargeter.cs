namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    [RequireComponent(typeof(Targeter))]
    public class ActionTargeter : ABaseComponent
    {
        // Events
        public readonly AutoEvent<ActionTargetable, ActionTargetable, Action> OnChangeTargetable = new();

        // Publics
        public ActionTargetable Targetable
        {
            get => _targetable;
            set
            {
                if (value == _targetable
                || value != null && !value.CanGetTargetedBy(this))
                    return;

                var previousTargetable = _targetable;
                if (previousTargetable != null
                && previousTargetable.CanGetUntargetedBy(this))
                    previousTargetable.GetUntargetedBy(this);

                _targetable = value;
                if (_targetable != null)
                    _targetable.GetTargetedBy(this, _action);

                OnChangeTargetable.Invoke(previousTargetable, _targetable, _action);
            }
        }
        public bool HasRequiredComponentsToTargetWith(Action action)
        {
            foreach (var requiredComponentType in action.RequiredComponentTypes[ActionTarget.User])
                if (!Has(requiredComponentType))
                    return false;
            return true;
        }
        public bool TryStartTargeting(Action action)
        {
            if (!HasRequiredComponentsToTargetWith(action))
                return false;

            _action = action;
            Get<Targeter>().OnChangeTargetable.Sub(Targeter_OnChangeTargetable);

            return true;
        }
        public bool TryStartTargeting(Action action, UITargetingLine targetingLine, Transform from)
        {
            if (!TryStartTargeting(action))
                return false;

            _targetingLine = targetingLine;
            _targetingLine.Show(Get<Targeter>(), from);
            _targetingLine.Player.Get<Selecter>().Disable();
            _targetingLine.Player.OnInputReleaseConfirm.SubOnce(TryFinishTargeting);
            return true;
        }
        public void TryFinishTargeting()
        {
            if (_action == null)
                return;

            if (_targetable != null)
                _action.Use(this, _targetable);

            if (_targetingLine != null)
            {
                _targetingLine.Player.Get<Selecter>().Enable();
                _targetingLine.Hide();
                _targetingLine = null;               
            }

            Get<Targeter>().OnChangeTargetable.Unsub(Targeter_OnChangeTargetable);
            _action = null;
        }

        // Privates
        private Action _action;
        private ActionTargetable _targetable;
        private UITargetingLine _targetingLine;
        private bool CanTarget(Targetable targetable)
        => targetable != null
        && targetable.TryGet(out ActionTargetable actionTargetable)
        && actionTargetable.HasRequiredComponentsToGetTargetedWith(_action);
        private void Targeter_OnChangeTargetable(Targetable from, Targetable to)
        => Targetable = CanTarget(to) ? to.Get<ActionTargetable>() : null;
    }
}