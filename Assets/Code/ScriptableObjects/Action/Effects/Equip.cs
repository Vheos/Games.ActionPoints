namespace Vheos.Games.ActionPoints.ActionScripts
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;

    [CreateAssetMenu(fileName = nameof(Equip), menuName = CONTEXT_MENU_PATH + nameof(Equip))]
    public class Equip : AActionEffect
    {
        // Overrides
        protected override int RequiredValuesCount
        => 0;
        override public void Invoke(ABaseComponent user, ABaseComponent target, params float[] values)
        {
            Equipable targetEquipable = target.Get<Equipable>();
            if (targetEquipable.Equiper == null)
                user.Get<Equiper>().Equip(targetEquipable);
        }
    }
}