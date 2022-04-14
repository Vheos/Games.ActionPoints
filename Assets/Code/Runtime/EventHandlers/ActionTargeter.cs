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
        public bool IsTargeting
        => _action != null;
        public void StartHighlightingValidTargets(Selecter selecter, Action action)
        {
            foreach (var target in TargetableManager.GetValidTargets(Get<Actionable>(), action))
                if (target.TryGet(out Highlightable highlightable))
                {
                    selecter.Get<Highlighter>().TryAddHighlightable(target.Get<Highlightable>());
                    _highlightedTargets.Add(highlightable);
                }
        }
        public void StopHighlightingValidTargets(Selecter selecter)
        {
            foreach (var highlightedTarget in _highlightedTargets)
                if (highlightedTarget != null)
                    selecter.Get<Highlighter>().TryRemoveHighlightable( highlightedTarget);
            _highlightedTargets.Clear();
        }

        public void TryStartTargeting(Selecter selecter, ActionButton button)
        {
            TryFinishTargeting(selecter);
            if (!button.Action.CheckUserComponents(Get<Actionable>()))
                return;

            _action = button.Action;
            Get<Targeter>().Targetable = null;

            selecter.AddTest(TargetingTest);
            selecter.AddPressTest(DisablePressEvents);
            selecter.OnChangeSelectable.Sub(SetTargetable);
            selecter.OnRelease.Sub(TryFinishTargeting);
            selecter.Get<Player>().TargetingLine.Show(button.transform);
        }

        public void TryFinishTargeting(Selecter selecter)
        {
            if (_action == null)
                return;

            if (Get<Targeter>().Targetable.TryNonNull(out var targetable))
                _action.Use(Get<Actionable>(), targetable);

            selecter.Get<Player>().TargetingLine.Hide();
            selecter.OnRelease.Unsub(TryFinishTargeting);
            selecter.OnChangeSelectable.Unsub(SetTargetable);
            selecter.RemovePressTest(DisablePressEvents);
            selecter.RemoveTest(TargetingTest);

            Get<Targeter>().Targetable = null;
            _action = null;
            StopHighlightingValidTargets(selecter);
        }

        // Privates
        private Action _action;
        private readonly HashSet<Highlightable> _highlightedTargets = new();
        private bool TargetingTest(Selectable selectable)
        => _highlightedTargets.Any(t => t.SameGOAs(selectable));
        private void SetTargetable(Selecter selecter)
        => Get<Targeter>().Targetable = selecter.IsSelectingAny ? selecter.Selectable.Get<Targetable>() : null;
        private bool DisablePressEvents(Selectable selectable)
        => false;
    }
}

// Auto-use if only one target
//if (_highlightedTargets.Count == 1)
//    button.Action.Use(Get<Actionable>(), _highlightedTargets.First());


// Move mouse to valid targets midpoint
//Vector2 screenMidpoint = _highlightedTargets.Midpoint(t => UICanvasManager.Any.WorldToScreenPosition(t.transform.position));
//Get<PlayerOwnable>().Owner.Cursor.MoveTo(ViewSpace.Screen, screenMidpoint, 0.4f);