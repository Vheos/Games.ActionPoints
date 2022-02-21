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

        // Private
        private Actionable _actionable;
        private void ApplyActionSpeed()
        {
            _actionable.ActionProgress += Time.deltaTime * ActionSpeed;
        }
        private void ApplyOverflowFocusRate(float overflow)
        {
            _actionable.FocusProgress += overflow * OverflowFocusRate;
        }

        // Play
        protected override void PlayStart()
        {
            base.PlayStart();
            _actionable = Get<Actionable>();

            _actionable.MaxActionPoints = MaxActionPoints;            
            _actionable.LockedMaxActionPoints.Set(() => Get<Woundable>().Wounds);
            _actionable.TryChangeActions(null, StartingActions);
            Get<Updatable>().OnUpdate.SubEnableDisable(this, ApplyActionSpeed);
            _actionable.OnOverflowActionProgress.SubEnableDisable(this, ApplyOverflowFocusRate);

            Get<Woundable>().MaxWounds.Set(() => MaxActionPoints);
        }
    }
}