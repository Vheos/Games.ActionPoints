namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using System.Collections.Generic;

    [RequireComponent(typeof(Equipable))]
    public class Equipment : ABaseComponent
    {
        // Inspector
        [field: SerializeField] public EquipSlot Slot { get; private set; }
        [field: SerializeField] public Action[] Actions { get; private set; }
        [field: SerializeField, Range(-2, +2)] public int MaxActionPoints { get; private set; }

        // Privates
        private void Selectable_OnPress(Selecter selecter)
        {
            selecter.Get<Player>().TargetingLine.Show(Get<Targeter>(), this.transform);
        }
        private void Selectable_OnRelease(Selecter selecter, bool withinTrigger)
        {
            if (Get<Targeter>().TryGetTargetable(out Equiper equiper))
                if (!equiper.TryEquip(Get<Equipable>()))
                    equiper.TryUnequip(Get<Equipable>());

            selecter.Get<Player>().TargetingLine.Hide();
        }
        private void Targetable_OnGainTargeting(Targeter targeter, bool isFirst)
        {
            if (isFirst && !targeter.SameGOAs(this))
                Get<SpriteOutline>().Show();
        }
        private void Targetable_OnLoseTargeting(Targeter targeter, bool isLast)
        {
            if (isLast && !targeter.SameGOAs(this))
                Get<SpriteOutline>().Hide();
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();

            Get<Equipable>().EquipSlot.Set(() => (int)Slot);

            Get<Selectable>().OnGetPressed.SubEnableDisable(this, Selectable_OnPress);
            Get<Selectable>().OnGetReleased.SubEnableDisable(this, Selectable_OnRelease);

            Get<Targetable>().OnGainTargeting.SubEnableDisable(this, Targetable_OnGainTargeting);
            Get<Targetable>().OnLoseTargeting.SubEnableDisable(this, Targetable_OnLoseTargeting);
        }
    }
}