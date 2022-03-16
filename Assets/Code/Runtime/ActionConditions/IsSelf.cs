namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(IsSelf), menuName = ASSET_MENU_PATH + nameof(IsSelf))]
    public class IsSelf : ActionCondition
    {
        override public bool Check(ABaseComponent @subject, ABaseComponent @object, float[] values)
        => @subject.SameGOAs(@object);
    }
}