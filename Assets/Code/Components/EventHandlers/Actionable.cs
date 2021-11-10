namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class Actionable : AEventSubscriber
    {
        // Inspector
        [SerializeField] protected List<Action> _Actions = new List<Action>();
        [SerializeField] [Range(1, 10)] protected int _MaxPoints = 5;
        [SerializeField] [Range(0f, 1f)] protected float _ActionSpeed = 1f;
        [SerializeField] [Range(0f, 1f)] protected float _FocusRate = 0.5f;
        [SerializeField] [Range(0f, 1f)] protected float _ExhaustRate = 0.5f;

        // Events
        public Event<int, int> OnActionPointsCountChanged
        { get; } = new Event<int, int>();
        public Event<int, int> OnFocusPointsCountChanged
        { get; } = new Event<int, int>();
        public Event<bool> OnExhaustStateChanged
        { get; } = new Event<bool>();
        public Event<float> OnActionProgressOverflow
        { get; } = new Event<float>();

        // Publics
        public IEnumerable<Action> Actions
        => _Actions;
        // Action
        public float ActionSpeed
        => _ActionSpeed;
        public float ActionProgress
        { get; private set; }
        public int ActionPointsCount
        => ActionProgress.RoundTowardsZero();
        public void ChangeActionPoints(int diff)
        => ActionProgress = ActionProgress.Add(diff).Clamp(-UsableMaxActionPoints, +UsableMaxActionPoints);
        // Focus
        public float FocusRate
        => _FocusRate;
        public float FocusProgress
        { get; private set; }
        public int FocusPointsCount
        => FocusProgress.RoundTowardsZero();
        public void ChangeFocusPoints(int diff)
        => FocusProgress = FocusProgress.Add(diff).ClampMax(ActionProgress).ClampMin(0f);
        // Exhaust
        public float ExhaustRate
        => _ExhaustRate;
        public bool IsExhausted
        => ActionProgress < 0;
        // MaxAction
        public int MaxActionPoints
        => _MaxPoints;
        public int LockedMaxActionPoints
        { get; private set; }
        public int UsableMaxActionPoints
        => MaxActionPoints - LockedMaxActionPoints;

        // Privates
        private float _previousActionProgress;
        private int PreviousActionPointsCount
        => _previousActionProgress.RoundTowardsZero();
        private float _previousFocusProgress;
        private int PreviousFocusPointsCount
        => _previousFocusProgress.RoundTowardsZero();
        private bool PreviousIsExhausted
        => _previousActionProgress < 0;
        private void TryInvokeEvents()
        {
            if (PreviousActionPointsCount != ActionPointsCount)
                OnActionPointsCountChanged?.Invoke(PreviousActionPointsCount, ActionPointsCount);

            if (PreviousFocusPointsCount != FocusPointsCount)
                OnFocusPointsCountChanged?.Invoke(PreviousFocusPointsCount, FocusPointsCount);

            if (PreviousIsExhausted && !IsExhausted)
                OnExhaustStateChanged?.Invoke(false);
            else if (!PreviousIsExhausted && IsExhausted)
                OnExhaustStateChanged?.Invoke(true);

            _previousActionProgress = ActionProgress;
            _previousFocusProgress = FocusProgress;
        }
        private void UpdateProgresses()
        {
            ActionProgress += Time.deltaTime * _ActionSpeed;
            if (ActionProgress > UsableMaxActionPoints)
            {
                float overflow = ActionProgress - UsableMaxActionPoints;
                ActionProgress = UsableMaxActionPoints;
                OnActionProgressOverflow?.Invoke(overflow);
            }
        }

        // Playable
        protected override void SubscribeToEvents()
        {
            SubscribeTo(GetHandler<Updatable>().OnUpdated, UpdateProgresses, TryInvokeEvents);
        }
    }
}