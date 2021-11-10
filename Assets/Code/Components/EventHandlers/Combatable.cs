namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    public class Combatable : ABaseComponent
    {
        // Events
        public Event<Combat> OnCombatChanged
        { get; } = new Event<Combat>();

        // Publics
        public Combat Combat
        { get; private set; }
        public Vector3 AnchorPosition
        { get; private set; }
        public bool IsInCombat
        => Combat != null;
        public void StartCombatWith(Combatable target)
        {
            if (this.IsInCombat && target.IsInCombat
            || this == target)
                return;

            if (!this.IsInCombat && !target.IsInCombat)
            {
                Combat newCombat = new Combat(this, target);
                this.JoinCombat(newCombat);
                target.JoinCombat(newCombat);
            }
            else if (this.IsInCombat)
                target.JoinCombat(this.Combat);
            else
                this.JoinCombat(target.Combat);
        }
        public void LeaveCombat()
        {
            Combat.RemoveMember(this);
            Combat = null;
            OnCombatChanged?.Invoke(null);
        }

        // Privates
        private void JoinCombat(Combat combat)
        {
            Combat = combat;
            combat.AddMember(this);
            AnchorPosition = transform.position;
            OnCombatChanged?.Invoke(combat);
        }
    }
}