namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(IsEquiped), menuName = CONTEXT_MENU_PATH + nameof(IsEquiped))]
    public class IsEquiped : AActionTargetTest
    {
        public override bool Invoke(Targeter user, Targetable target)
        => target.Get<Equipable>().Equiper != null;
    }
}