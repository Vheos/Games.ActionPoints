namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(IsEnemy), menuName = CONTEXT_MENU_PATH + nameof(IsEnemy))]
    public class IsEnemy : AActionTargetTest
    {
        public override bool Invoke(Targeter user, Targetable target)
        => target.IsEnemyOf(user);
    }
}