namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(IsSelf), menuName = CONTEXT_MENU_PATH + nameof(IsSelf))]
    public class IsSelf : AActionTargetTest
    {
        public override bool Invoke(Targeter user, Targetable target)
        => target.SameGOAs(user);
    }
}