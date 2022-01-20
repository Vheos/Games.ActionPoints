namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(DealDamage), menuName = CONTEXT_MENU_PATH + nameof(DealDamage))]
    public class DealDamage : ActionEffect
    {
        // Overrides
        override public int RequiredValuesCount
        => 3;
        override public void Invoke(ABaseComponent user, ABaseComponent target, params float[] values)
        {
            // Params
            float blunt = values[0];
            float sharp = values[1];
            float raw = values[2];

            // Execute
            target.Get<Woundable>().ReceiveDamage(blunt, sharp, raw);
        }
    }
}