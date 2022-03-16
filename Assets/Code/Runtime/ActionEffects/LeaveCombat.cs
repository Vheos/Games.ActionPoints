namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;

    [CreateAssetMenu(fileName = nameof(LeaveCombat), menuName = ASSET_MENU_PATH + nameof(LeaveCombat))]
    public class LeaveCombat : ActionEffect
    {
        // Overrides
        override public Type[] SubjectRequiredComponents
        => new[] { typeof(Combatable) };
        override public int RequiredValuesCount
        => 0;

        override public void Invoke(ABaseComponent @subject, ABaseComponent @object, float[] values, ActionStats stats)
        {
            // Execute
            @subject.Get<Combatable>().Combat = null;
        }
    }
}