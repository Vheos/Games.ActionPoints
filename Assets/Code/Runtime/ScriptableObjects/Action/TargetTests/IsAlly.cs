namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(IsAlly), menuName = CONTEXT_MENU_PATH + nameof(IsAlly))]
    public class IsAlly : AActionTargetTest
    {
        public override bool Invoke(Targeter user, Targetable target)
        => target.IsAllyOf(user);
    }
}