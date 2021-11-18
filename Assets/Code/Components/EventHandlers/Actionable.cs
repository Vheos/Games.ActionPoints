namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class Actionable : ABaseComponent
    {
        // Events
        public Event<int, int> OnActionPointsCountChanged
        { get; } = new Event<int, int>();
        public Event<int, int> OnFocusPointsCountChanged
        { get; } = new Event<int, int>();
        public Event<bool> OnExhaustStateChanged
        { get; } = new Event<bool>();
        public Event<float> OnActionProgressOverflow
        { get; } = new Event<float>();

        // Input
        public ComponentInput<ICollection<Action>> Actions
        { get; } = new ComponentInput<ICollection<Action>>();
        public ComponentInput<int> MaxActionPoints
        { get; } = new ComponentInput<int>();
        public ComponentInput<int> LockedMaxActionPoints
        { get; } = new ComponentInput<int>();

        // Publics
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
                    OnActionProgressOverflow?.Invoke(_actionProgress - UsableMaxActionPoints);
                    _actionProgress.SetClampMax(+UsableMaxActionPoints);
                }
                if (previousCount != ActionPointsCount)
                    OnActionPointsCountChanged?.Invoke(previousCount, ActionPointsCount);
                if (previousExhausted && !IsExhausted)
                    OnExhaustStateChanged?.Invoke(false);
                else if (!previousExhausted && IsExhausted)
                    OnExhaustStateChanged?.Invoke(true);

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
                    OnFocusPointsCountChanged?.Invoke(previousCount, FocusPointsCount);

                _focusProgress.SetClamp(0f, _actionProgress);
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

        // Privates
        private float _actionProgress;
        private float _focusProgress;
    }
}