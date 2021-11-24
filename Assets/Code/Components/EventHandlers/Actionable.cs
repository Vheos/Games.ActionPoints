namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Vheos.Tools.Extensions.Collections;

    public class Actionable : ABaseComponent
    {
        // Events
        public Event<int, int> OnChangeActionPointsCount
        { get; } = new Event<int, int>();
        public Event<int, int> OnChangeFocusPointsCount
        { get; } = new Event<int, int>();
        public Event<bool> OnChangeExhausted
        { get; } = new Event<bool>();
        public Event<float> OnOverflowActionProgress
        { get; } = new Event<float>();

        // Input
        public ComponentInput<int> MaxActionPoints
        { get; } = new ComponentInput<int>();
        public ComponentInput<int> LockedMaxActionPoints
        { get; } = new ComponentInput<int>();

        // Publics
        public IReadOnlyList<Action> Actions
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
                int previousCount = ActionPointsCount;
                bool previousExhausted = IsExhausted;
                _actionProgress = value;

                if (_actionProgress > UsableMaxActionPoints)
                {
                    OnOverflowActionProgress?.Invoke(_actionProgress - UsableMaxActionPoints);
                    _actionProgress.SetClampMax(+UsableMaxActionPoints);
                }
                if (previousCount != ActionPointsCount)
                    OnChangeActionPointsCount?.Invoke(previousCount, ActionPointsCount);
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
                int previousCount = FocusPointsCount;
                _focusProgress = value;

                if (previousCount != FocusPointsCount)
                    OnChangeFocusPointsCount?.Invoke(previousCount, FocusPointsCount);

                _focusProgress.SetClampMax(_actionProgress);
                _focusProgress.SetClampMin(0f);
            }
        }
        public int ActionPointsCount
        => _actionProgress.RoundTowardsZero();
        public int FocusPointsCount
        => _focusProgress.RoundTowardsZero();
        public int UsableMaxActionPoints
        => MaxActionPoints - LockedMaxActionPoints;
        public bool IsExhausted
         => _actionProgress < 0;
        public bool CanUse(Action action)
        => action.CanBeUsedBy(this);
        public void Use(Action action, Targetable target)
        => action.Use(this, target);

        // Privates
        private readonly List<Action> _actions = new List<Action>();
        private float _actionProgress;
        private float _focusProgress;
    }
}