namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    [RequireComponent(typeof(Woundable))]
    public class DamagePopupHandler : ABaseComponent
    {
        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            Get<Woundable>().OnReceiveDamage.SubEnableDisable(this,
                (amount, hasDealtWound) => PopupManager.PopDamage(transform.position, amount, hasDealtWound));
            Get<Woundable>().OnReceiveHealing.SubEnableDisable(this,
                (amount, hasHealedWound) => PopupManager.PopHealing(transform.position, amount, hasHealedWound));
        }
    }
}