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
                    previousTargetable.GetUntargetedBy(this, _action);

                _targetable = value;
                if (_targetable != null)
                    _targetable.GetTargetedBy(this, _action);

                OnChangeTargetable.Invoke(previousTargetable, _targetable, _action);
            }
        }
        public bool CanTargetWithAction(ActionTargetable actionTargetable, Action action)
        => true;
        public void ShowTargetingLine(Selecter selecter, Action action, Transform from)
        {
            _action = action;
            Get<Targeter>().OnChangeTargetable.Sub(Targeter_OnChangeTargetable);
            selecter.Get<Player>().TargetingLine.Show(Get<Targeter>(), from);

        }
        public void HideTargetingLine(Selecter selecter)
        {
            if (_targetable != null)
                _action.Use(this, _targetable);
            selecter.Get<Player>().TargetingLine.Hide();
            Get<Targeter>().OnChangeTargetable.Unsub(Targeter_OnChangeTargetable);
            _action = null;
        }

        // Privates
        private Action _action;
        private ActionTargetable _targetable;
        private bool CanTarget(Targetable targetable)
        => targetable != null
        && targetable.TryGet(out ActionTargetable actionTargetable)
        && CanTargetWithAction(actionTargetable, _action);
        private void Targeter_OnChangeTargetable(Targetable from, Targetable to)
        => Targetable = CanTarget(to) ? to.Get<ActionTargetable>() : null;
    }
}