namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(HasCombatable), menuName = CONTEXT_MENU_PATH + nameof(HasCombatable))]
    public class HasCombatable : AActionTargetTest
    {
        public override bool Invoke(Targeter user, Targetable target)
        => target.Has<Combatable>();
    }
}