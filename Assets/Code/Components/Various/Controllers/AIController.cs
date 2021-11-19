namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.UtilityN;
    using Tools.Extensions.General;

    public class AIController : AController
    {
        // Inspector
        [SerializeField] protected bool _DebugMode;
        [SerializeField] [Range(0f, 5f)] protected float _MinTargetingDuration;


        // Private
        private float _targetingStartTime;
        private bool HasTargetegForMinDuration
        => Time.time - _targetingStartTime > _MinTargetingDuration;
        private State _state;
        private Action _action;
        private Targetable _target;
        private void OnUpdate()
        {
            if (!Get<Combatable>().IsInCombat)
                return;

            switch (_state)
            {
                case State.Inactive:
                    if (AvailableActions.Random().TryNonNull(out _action)
                    && AvailableTargets.Random().TryNonNull(out _target))
                    {
                        _action.TryPlayAnimation(Get<ActionAnimator>(), Action.Animation.Charge);
                        _action.StartTargeting(Get<Targeter>(), _target);
                        _targetingStartTime = Time.time;
                        _state = State.Charging;
                    }
                    break;
                case State.Charging:
                    if (!Get<ActionAnimator>().IsPlaying
                    && HasTargetegForMinDuration)
                    {
                        _action.TryPlayAnimation(Get<ActionAnimator>(), Action.Animation.Release);
                        _action.Use(Get<Actionable>(), _target);
                        _state = State.Releasing;
                    }
                    break;
                case State.Releasing:
                    if (!Get<ActionAnimator>().IsPlaying)
                        _state = State.Inactive;
                    break;
            }
        }
        private IEnumerable<Action> AvailableActions
        {
            get
            {
                Actionable actionable = Get<Actionable>();
                foreach (var action in actionable.Actions)
                    if (action.CanBeUsedBy(actionable))
                        yield return action;
           }
        }
        private IEnumerable<Targetable> AvailableTargets
        {
            get
            {
                foreach (var combatMember in Get<Combatable>().Combat.Members)
                    if (combatMember.TryGetComponent<Targetable>(out var target)
                    && _action.CanTarget(Get<Targeter>(), target))
                        yield return target;
            }
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdated, OnUpdate);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _MinTargetingDuration += UnityEngine.Random.value;
        }

        // Defines
        public enum State
        {
            Inactive,
            Charging,
            Releasing,
        }
    }
}