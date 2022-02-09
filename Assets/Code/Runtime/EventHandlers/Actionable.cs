namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.Collections;


    [DisallowMultipleComponent]
    sealed public class Actionable : ABaseComponent
    {
        // Events
        public readonly AutoEvent<IEnumerable<Action>, IEnumerable<Action>> OnChangeActions = new();
        public readonly AutoEvent<int, int> OnChangeMaxActionPoints = new();
        public readonly AutoEvent<int, int> OnChangeActionPoints = new();
        public readonly AutoEvent<int, int> OnChangeFocusPoints = new();
        public readonly AutoEvent<bool> OnChangeExhausted = new();
        public readonly AutoEvent<float> OnOverflowActionProgress = new();

        // Getters
        public readonly Getter<int> LockedMaxActionPoints = new();

        // Publics
        public IReadOnlyCollection<Action> Actions
        => _actions;
        public bool TryChangeActions(IEnumerable<Action> toRemove, IEnumerable<Action> toAdd)
        {
            HashSet<Action> actuallyRemovedActions = new();
            if (toRemove != null)
                foreach (var action in toAdd != null ? toRemove.Except(toAdd) : toRemove)
                    if (_actions.TryRemove(action))
                        actuallyRemovedActions.Add(action);

            HashSet<Action> actuallyAddedActions = new();
            if (toAdd != null)
                foreach (var action in toRemove != null ? toAdd.Except(toRemove) : toAdd)
                    if (_actions.TryAddUnique(action))
                        actuallyAddedActions.Add(action);

            if (actuallyRemovedActions.IsEmpty()
            && actuallyAddedActions.IsEmpty())
                return false;

            OnChangeActions.Invoke(actuallyRemovedActions, actuallyAddedActions);
            return true;
        }
        public void ClearActions()
        => _actions.Clear();
        public int MaxActionPoints
        {
            get => _maxActionPoints;
            set
            {
                if (value == _maxActionPoints)
                    return;

                int previousMaxActionPoints = _maxActionPoints;
                _maxActionPoints = value;
                OnChangeMaxActionPoints.Invoke(previousMaxActionPoints, _maxActionPoints);

                if (ActionProgress > _maxActionPoints)
                    ActionProgress = ActionProgress.ClampMax(_maxActionPoints);
            }
        }
        public float ActionProgress
        {
            get => _actionProgress;
            set
            {
                if (_actionProgress == value)
                    return;

                int previousActionPoints = ActionPoints;
                bool previousExhausted = IsExhausted;

                _actionProgress = value;
                float overflow = _actionProgress - UsableMaxActionPoints;
                _actionProgress.SetClamp(-UsableMaxActionPoints, +UsableMaxActionPoints);
                if (_actionProgress < _focusProgress)
                    FocusProgress = _actionProgress;

                if (overflow > 0f)
                    OnOverflowActionProgress?.Invoke(overflow);
                if (previousActionPoints != ActionPoints)
                    OnChangeActionPoints?.Invoke(previousActionPoints, ActionPoints);
                if (previousExhausted && !IsExhausted)
                    OnChangeExhausted?.Invoke(false);
                else if (!previousExhausted && IsExhausted)
                    OnChangeExhausted?.Invoke(true);
            }
        }
        public float FocusProgress
        {
            get => _focusProgress;
            set
            {
                if (_focusProgress == value)
                    return;

                int previousFocusPoints = FocusPoints;
                _focusProgress = value;
                _focusProgress.SetClampMax(_actionProgress.ClampMin(0f));

                if (previousFocusPoints != FocusPoints)
                    OnChangeFocusPoints?.Invoke(previousFocusPoints, FocusPoints);
            }
        }
        public int ActionPoints
        => _actionProgress.RoundTowardsZero();
        public int FocusPoints
        => _focusProgress.RoundTowardsZero();
        public int UsableActionPoints
        => _actionProgress.Add(UsableMaxActionPoints).RoundTowardsZero();
        public int UsableMaxActionPoints
        => MaxActionPoints - LockedMaxActionPoints;
        public bool IsExhausted
         => _actionProgress < 0;
        public bool CanAfford(Action action)
        => !IsExhausted
        && UsableActionPoints >= action.ActionPointsCost
        && FocusPoints >= action.FocusPointsCost;

        // Privates
        private readonly HashSet<Action> _actions = new();
        private int _maxActionPoints;
        private float _actionProgress;
        private float _focusProgress;
    }
}