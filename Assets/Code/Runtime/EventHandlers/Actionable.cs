namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.Collections;

    [DisallowMultipleComponent]
    sealed public class Actionable : ABaseComponent
    {
        // Events
        public readonly AutoEvent<int, int> OnChangeActionPoints = new();
        public readonly AutoEvent<int, int> OnChangeFocusPoints = new();
        public readonly AutoEvent<bool> OnChangeExhausted = new();
        public readonly AutoEvent<float> OnOverflowActionProgress = new();

        // Getters
        public Getter<int> MaxActionPoints { get; } = new();
        public Getter<int> LockedMaxActionPoints { get; } = new();

        // Publics
        public IReadOnlyCollection<Action> Actions
        => _actions;
        public void TryAddActions(params Action[] actions)
        {
            foreach (var action in actions)
                _actions.TryAddUnique(action);
        }
        public void TryRemoveActions(params Action[] actions)
        {
            foreach (var action in actions)
                _actions.TryRemove(action);
        }
        public void ClearActions()
        => _actions.Clear();
        public float ActionProgress
        {
            get => _actionProgress;
            set
            {
                int previous = ActionPoints;
                bool previousExhausted = IsExhausted;
                _actionProgress = value;

                if (_actionProgress > UsableMaxActionPoints)
                {
                    OnOverflowActionProgress?.Invoke(_actionProgress - UsableMaxActionPoints);
                    _actionProgress.SetClampMax(+UsableMaxActionPoints);
                }
                if (previous != ActionPoints)
                    OnChangeActionPoints?.Invoke(previous, ActionPoints);
                if (previousExhausted && !IsExhausted)
                    OnChangeExhausted?.Invoke(false);
                else if (!previousExhausted && IsExhausted)
                    OnChangeExhausted?.Invoke(true);

                _actionProgress.SetClampMin(-UsableMaxActionPoints);
            }
        }
        public float FocusProgress
        {
            get => _focusProgress;
            set
            {
                int previous = FocusPoints;
                _focusProgress = value;

                if (previous != FocusPoints)
                    OnChangeFocusPoints?.Invoke(previous, FocusPoints);

                _focusProgress.SetClampMax(_actionProgress);
                _focusProgress.SetClampMin(0f);
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
        public bool CanUse(Action action)
        => action.CanBeUsedBy(this);
        public void Use(Action action, Targetable target)
        => action.Use(this, target);

        // Privates
        private readonly HashSet<Action> _actions = new();
        private float _actionProgress;
        private float _focusProgress;
    }
}