namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(IsEnemy), menuName = ASSET_MENU_PATH + nameof(IsEnemy))]
    public class IsEnemy : ActionCondition
    {
        override public bool Check(ABaseComponent @subject, ABaseComponent @object, float[] values)
        => @subject.IsEnemyOf(@object);
    }
}