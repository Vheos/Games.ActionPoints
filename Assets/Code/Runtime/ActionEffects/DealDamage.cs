namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(DealDamage), menuName = ASSET_MENU_PATH + nameof(DealDamage))]
    public class DealDamage : ActionEffect
    {
        // Overrides
        override public Type[] ObjectRequiredComponents
        => new[] { typeof(Woundable) };
        override public int RequiredValuesCount
        => 3;

        override public void Invoke(ABaseComponent user, ABaseComponent target, float[] values, ActionStats stats)
        {
            // Cache           
            var targetWoundable = target.Get<Woundable>();
            int targetPreviousWounds = targetWoundable.Wounds;
            float blunt = values[0];
            float sharp = values[1];
            float pure = values[2];

            // Execute
            targetWoundable.ReceiveDamage(blunt, sharp, pure);

            // Stats
            if (stats != null)
            {
                stats.AddDamageStats(blunt, targetWoundable.CalculateBluntDamage(blunt), sharp, targetWoundable.CalculateBluntDamage(sharp), pure);
                stats.WoundsDealt += targetWoundable.Wounds - targetPreviousWounds;
            }
        }
    }
}