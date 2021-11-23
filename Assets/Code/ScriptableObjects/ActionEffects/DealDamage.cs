namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(DealDamage), menuName = CONTEXT_MENU_PATH + nameof(DealDamage), order = 2)]
    public class DealDamage : AActionEffect
    {
        // Overrides
        protected override int RequiredValuesCount
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