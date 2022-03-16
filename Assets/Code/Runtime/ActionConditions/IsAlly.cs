namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(IsAlly), menuName = ASSET_MENU_PATH + nameof(IsAlly))]
    public class IsAlly : ActionCondition
    {
        override public bool Check(ABaseComponent @subject, ABaseComponent @object, float[] values)
        => @subject.IsAllyOf(@object);
    }
}