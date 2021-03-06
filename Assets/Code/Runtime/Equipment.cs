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
        private void Selectable_OnPress(Selectable selectable, Selecter selecter)
        {
            selecter.Get<Player>().TargetingLine.Show(transform);
        }
        private void Selectable_OnRelease(Selectable selectable, Selecter selecter, bool isClick)
        {
            if (Get<Targeter>().TryGetTargetable(out Equiper equiper))
                if (!equiper.TryEquip(Get<Equipable>()))
                    equiper.TryUnequip(Get<Equipable>());

            selecter.Get<Player>().TargetingLine.Hide();
        }
        private void Targetable_OnGainTargeting(Targetable targetable, Targeter targeter)
        {
            if (targetable.IsTargetedByMany
            || targeter.SameGOAs(this))
                return;

            Get<SpriteOutline>().Show();
        }
        private void Targetable_OnLoseTargeting(Targetable targetable, Targeter targeter)
        {
            if (targetable.IsTargeted
            || targeter.SameGOAs(this))
                return;

            Get<SpriteOutline>().Hide();
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();

            Get<Equipable>().EquipSlot = (int)Slot;

            Get<Selectable>().OnGetPressed.SubEnableDisable(this, Selectable_OnPress);
            Get<Selectable>().OnGetReleased.SubEnableDisable(this, Selectable_OnRelease);

            Get<Targetable>().OnGainTargeting.SubEnableDisable(this, Targetable_OnGainTargeting);
            Get<Targetable>().OnLoseTargeting.SubEnableDisable(this, Targetable_OnLoseTargeting);
        }
    }
}