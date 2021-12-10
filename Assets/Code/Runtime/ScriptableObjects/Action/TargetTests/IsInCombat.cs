namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(IsInCombat), menuName = CONTEXT_MENU_PATH + nameof(IsInCombat))]
    public class IsInCombat : AActionTargetTest
    {
        public override bool Invoke(Targeter user, Targetable target)
        => target.IsInCombat();
    }
}