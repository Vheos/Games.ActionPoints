namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(HealDamage), menuName = CONTEXT_MENU_PATH + nameof(HealDamage))]
    public class HealDamage : ActionEffect
    {
        // Overrides
        override public Type[] ObjectRequiredComponents
        => new[] { typeof(Woundable) };
        override public int RequiredValuesCount
        => 1;

        override public void Invoke(ABaseComponent user, ABaseComponent target, float[] values, ActionStats stats)
        {
            // Cache       
            var targetWoundable = target.Get<Woundable>();
            int targetPreviousWounds = targetWoundable.Wounds;
            float amount = values[0];

            // Execute
            targetWoundable.ReceiveHealing(amount);

            // Stats
            if (stats != null)
            {
                stats.DamageHealed += targetWoundable.CalculateHealing(amount);
                stats.WoundsHealed += targetPreviousWounds - targetWoundable.Wounds;
            }
        }
    }
}