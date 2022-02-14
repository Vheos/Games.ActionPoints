namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(HealDamage), menuName = CONTEXT_MENU_PATH + nameof(HealDamage))]
    public class HealDamage : ActionEffect
    {
        // Overrides
        override public Type[] RequiredComponentTypes
        => new[] { typeof(Woundable) };
        override public int RequiredValuesCount
        => 1;

        override public void Invoke(ABaseComponent target, float[] values, ActionStats stats)
        {
            // Cache       
            var woundable = target.Get<Woundable>();
            int previousWounds = woundable.Wounds;
            float amount = values[0];

            // Execute
            woundable.ReceiveHealing(amount);

            // Stats
            if (stats != null)
            {
                stats.DamageHealed += woundable.CalculateHealing(amount);
                stats.WoundsHealed += previousWounds - woundable.Wounds;
            }
        }
    }
}