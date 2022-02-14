namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(DealDamage), menuName = CONTEXT_MENU_PATH + nameof(DealDamage))]
    public class DealDamage : ActionEffect
    {
        // Overrides
        override public Type[] RequiredComponentTypes
        => new[] { typeof(Woundable) };
        override public int RequiredValuesCount
        => 3;

        override public void Invoke(ABaseComponent target, float[] values, ActionStats stats)
        {
            // Cache           
            var woundable = target.Get<Woundable>();
            int previousWounds = woundable.Wounds;
            float blunt = values[0];
            float sharp = values[1];
            float pure = values[2];

            // Execute
            woundable.ReceiveDamage(blunt, sharp, pure);

            // Stats
            if (stats != null)
            {
                stats.AddDamageStats(blunt, woundable.CalculateBluntDamage(blunt), sharp, woundable.CalculateBluntDamage(sharp), pure);
                stats.WoundsDealt += woundable.Wounds - previousWounds;
            }
        }
    }
}