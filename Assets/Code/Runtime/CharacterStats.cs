namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    [RequireComponent(typeof(Actionable))]
    [RequireComponent(typeof(Woundable))]
    public class CharacterStats : ABaseComponent
    {
        // Inspector
        [field: SerializeField, Range(0, 10)] public int MaxActionPoints { get; private set; }
        [field: SerializeField, Range(-1f, 1f)] public float ActionSpeed { get; private set; }
        [field: SerializeField, Range(-1f, 1f)] public float OverflowFocusRate { get; private set; }
        [field: SerializeField] public Action[] StartingActions { get; private set; }
        [field: SerializeField] public PredefinedTeam StartingTeam { get; private set; }

        // Private
        private float _recentExplorationActionProgress;
        private float _recentExplorationFocusProgress;
        private void ApplyActionSpeed()
        => Get<Actionable>().ActionProgress += Time.deltaTime * ActionSpeed;
        private void ApplyOverflowFocusRate(float overflow)
        => Get<Actionable>().FocusProgress += overflow * OverflowFocusRate;
        private void HandleResourcesOnChangeCombat(Combatable combatable)
        {
            if (combatable.Combat != null)
            {
                _recentExplorationActionProgress = Get<Actionable>().ActionProgress;
                _recentExplorationFocusProgress = Get<Actionable>().FocusProgress;
                if (SettingsManager.Gameplay.ResetActionPointsWhenStartingCombat)
                    Get<Actionable>().ActionProgress = Get<Actionable>().FocusProgress = 0;
                Get<Updatable>().OnUpdate.Sub(ApplyActionSpeed);
            }
            else
            {
                Get<Actionable>().ActionProgress = _recentExplorationActionProgress;
                Get<Actionable>().FocusProgress = _recentExplorationFocusProgress;
                Get<Updatable>().OnUpdate.Unsub(ApplyActionSpeed);
            }
        }

        // Play
        protected override void PlayStart()
        {
            base.PlayStart();
            Get<Actionable>().MaxActionPoints = MaxActionPoints;
            Get<Actionable>().LockedMaxActionPoints.Set(() => Get<Woundable>().Wounds);
            Get<Actionable>().TryChangeActions(null, StartingActions);
            Get<Actionable>().OnOverflowActionProgress.SubEnableDisable(this, ApplyOverflowFocusRate);

            Get<Woundable>().MaxWounds.Set(() => MaxActionPoints);

            Get<Combatable>().OnChangeCombat.SubEnableDisable(this, HandleResourcesOnChangeCombat);
            Get<Teamable>().Team = Team.GetPredefinedTeam(StartingTeam);

            Get<Actionable>().ActionPoints = MaxActionPoints - 1;
        }
    }
}