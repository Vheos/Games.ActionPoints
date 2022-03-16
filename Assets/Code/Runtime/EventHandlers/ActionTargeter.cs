namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.General;
    using Tools.Extensions.Collections;

    [RequireComponent(typeof(Actionable))]
    [RequireComponent(typeof(Targeter))]
    public class ActionTargeter : ABaseComponent
    {
        // Events
        public readonly AutoEvent<Targetable, Targetable, Action> OnChangeTargetable = new();

        // Publics
        public void TryStartHighlightingValidTargets(Action action)
        {
            foreach (var target in TargetableManager.GetValidTargets(Get<Actionable>(), action))
                if (target.TryGet(out Highlightable highlightable))
                {
                    highlightable.GainHighlight();
                    _highlightedTargets.Add(target);
                }
        }
        public void TryStopHighlightingValidTargets()
        {
            foreach (var target in _highlightedTargets)
                target.Get<Highlightable>().LoseHighlight();
            _highlightedTargets.Clear();
        }
        public void UseAction(Action action, Transform targetingLineAnchor)
        {
            switch (action.Execution)
            {
                case ActionExecution.Instant:
                case ActionExecution.Targeted when _highlightedTargets.Count == 1:
                    action.Use(Get<Actionable>(), _highlightedTargets.First());
                    break;
                case ActionExecution.Targeted:
                    TryStartTargeting(action, Get<PlayerOwnable>().Owner.TargetingLine, targetingLineAnchor);
                    break;
            }
        }
        public bool TryStartTargeting(Action action)
        {
            TryFinishTargeting();
            if (!action.CheckUserComponents(Get<Actionable>()))
                return false;

            _action = action;
            Get<Targeter>().AddTargetingTest(CanTarget);
            return true;
        }
        public bool TryStartTargeting(Action action, UITargetingLine targetingLine, Transform from)
        {
            if (!TryStartTargeting(action))
                return false;

            _targetingLine = targetingLine;
            _targetingLine.Show(Get<Targeter>(), from);
            _targetingLine.Player.Get<Selecter>().Disable();
            _targetingLine.Player.OnInputReleaseConfirm.Sub(TryFinishTargeting);
            return true;
        }
        public void TryFinishTargeting()
        {
            if (_action == null)
                return;

            if (Get<Targeter>().Targetable.TryNonNull(out var targetable))
                _action.Use(Get<Actionable>(), targetable);

            if (_targetingLine != null)
            {
                _targetingLine.Player.OnInputReleaseConfirm.Unsub(TryFinishTargeting);
                _targetingLine.Player.Get<Selecter>().Enable();
                _targetingLine.Hide();
                _targetingLine = null;
            }

            _action = null;
            Get<Targeter>().RemoveTargetingTest(CanTarget);
        }

        // Privates
        private Action _action;
        private UITargetingLine _targetingLine;
        private readonly HashSet<Targetable> _highlightedTargets = new();
        private bool CanTarget(Targetable targetable)
        => _action.CheckTargetComponents(targetable);
    }
}