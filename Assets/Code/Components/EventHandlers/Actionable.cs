namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class Actionable : AEventSubscriber
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
        public ComponentInput<float> ActionSpeed
        { get; } = new ComponentInput<float>();
        //public ComponentInput<float> FocusRate
        //{ get; } = new ComponentInput<float>();
        //public ComponentInput<float> ExhaustRate
        //{ get; } = new ComponentInput<float>();
        public ComponentInput<int> MaxActionPoints
        { get; } = new ComponentInput<int>();
        public ComponentInput<int> LockedMaxActionPoints
        { get; } = new ComponentInput<int>();
        public ComponentInput<List<Action>> Actions
        { get; } = new ComponentInput<List<Action>>();

        // Publics
        public float ActionProgress
        { get; private set; }
        public float FocusProgress
        { get; private set; }
        public int ActionPointsCount
        => ActionProgress.RoundTowardsZero();
        public int FocusPointsCount
        => FocusProgress.RoundTowardsZero();
        public void ChangeActionPoints(int diff)
        => ActionProgress = ActionProgress.Add(diff).Clamp(-UsableMaxActionPoints, +UsableMaxActionPoints);
        public void ChangeFocusPoints(int diff)
        => FocusProgress = FocusProgress.Add(diff).ClampMax(ActionProgress).ClampMin(0f);
        public int UsableMaxActionPoints
        => MaxActionPoints - LockedMaxActionPoints;
        public bool IsExhausted
         => ActionProgress < 0;

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
            ActionProgress += Time.deltaTime * ActionSpeed * ActionManager.GlobalSpeedScale;
            if (ActionProgress > UsableMaxActionPoints)
            {
                float overflow = ActionProgress - UsableMaxActionPoints;
                ActionProgress = UsableMaxActionPoints;
                OnActionProgressOverflow?.Invoke(overflow);
            }
        }

        // Playable
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdated, UpdateProgresses, TryInvokeEvents);
        }
    }
}