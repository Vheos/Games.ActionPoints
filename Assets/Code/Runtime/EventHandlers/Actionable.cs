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
        public readonly AutoEvent OnChangeActions = new();
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
            if (_actions.TryAddUnique(actions))
                OnChangeActions.Invoke();
        }
        public void TryRemoveActions(params Action[] actions)
        {
            if (_actions.TryRemove(actions))
                OnChangeActions.Invoke();
        }
        public void ClearActions()
        => _actions.Clear();
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