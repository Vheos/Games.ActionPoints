namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(HasWoundable), menuName = CONTEXT_MENU_PATH + nameof(HasWoundable))]
    public class HasWoundable : AActionTargetTest
    {
        public override bool Invoke(Targeter user, Targetable target)
        => target.Has<Woundable>();
    }
}