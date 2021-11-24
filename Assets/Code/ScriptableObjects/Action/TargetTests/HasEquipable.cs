namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(HasEquipable), menuName = CONTEXT_MENU_PATH + nameof(HasEquipable))]
    public class HasEquipable : AActionTargetTest
    {
        public override bool Invoke(Targeter user, Targetable target)
        => target.Has<Equipable>();
    }
}