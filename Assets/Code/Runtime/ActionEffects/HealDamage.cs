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

        override public void Invoke(ABaseComponent target, float[] values, ref ActionStats actionStats)
        {
            // Params
            float amount = values[0];

            // Execute
            target.Get<Woundable>().ReceiveHealing(amount);
        }
    }
}