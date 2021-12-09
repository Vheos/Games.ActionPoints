namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(LeaveCombat), menuName = CONTEXT_MENU_PATH + nameof(LeaveCombat))]
    public class LeaveCombat : AActionEffect
    {
        // Overrides
        protected override int RequiredValuesCount
        => 0;
        override public void Invoke(ABaseComponent user, ABaseComponent target, params float[] values)
        {
            user.Get<Combatable>().TryLeaveCombat();
        }
    }
}