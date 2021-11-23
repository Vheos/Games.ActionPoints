namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(StartCombat), menuName = CONTEXT_MENU_PATH + nameof(StartCombat))]
    public class StartCombat : AActionEffect
    {
        // Overrides
        protected override int RequiredValuesCount
        => 0;
        override public void Invoke(ABaseComponent user, ABaseComponent target, params float[] values)
        {

            user.Get<Combatable>().TryStartCombatWith(target.Get<Combatable>());
        }
    }
}