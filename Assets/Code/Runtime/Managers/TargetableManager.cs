namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;

    [DisallowMultipleComponent]
    public class TargetableManager : AStaticManager<TargetableManager, Targetable>
    {
        static public IEnumerable<Targetable> GetValidTargets(Actionable user, Action action)
        {
            foreach (var targetable in ActiveComponents)
                if (action.CheckTargetComponents(targetable)
                && action.CheckConditions(user, targetable))
                    yield return targetable;
        }
    }
}