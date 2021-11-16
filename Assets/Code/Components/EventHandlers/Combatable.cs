namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using System.Linq;

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
        public IEnumerable<Combatable> Allies
        {
            get
            {
                if (Combat == null
                || !TryGetComponent<Teamable>(out var teamable))
                    yield break;

                foreach (var combatable in Combat.Members)
                    if (combatable.TryGetComponent<Teamable>(out var combatTeamable)
                    && teamable.IsAlliedWith(combatTeamable))
                        yield return combatable;
            }
        }
        public IEnumerable<Combatable> Enemies
        {
            get
            {
                if (Combat == null)
                    yield break;

                Teamable teamable = Get<Teamable>();
                foreach (var combatable in Combat.Members)
                    if (teamable != null
                    && combatable.TryGetComponent<Teamable>(out var combatTeamable)
                    && !teamable.IsAlliedWith(combatTeamable)
                    || teamable == null
                    && this != combatable)
                        yield return combatable;
            }
        }
        public Vector3 AllyMidpoint
        {
            get
            {
                Vector3 r = Vector3.zero;
                int count = 0;
                foreach (var ally in Allies)
                {
                    r += ally.transform.position;
                    count++;
                }
                return r / count;
            }
        }
        public Vector3 EnemyMidpoint
        {
            get
            {
                Vector3 r = Vector3.zero;
                int count = 0;
                foreach (var enemy in Enemies)
                {
                    r += enemy.transform.position;
                    count++;
                }
                return r / count;
            }
        }
        public bool IsInCombat
        => Combat != null;
        public bool IsInCombatWith(Combatable other)
        => this != other && Combat == other.Combat;
        public void TryStartCombatWith(Combatable target)
        {
            if (this.IsInCombat && target.IsInCombat
            || !this.enabled || !target.enabled
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
        public void TryLeaveCombat()
        {
            if (Combat == null)
                return;

            Combat.TryRemoveMember(this);
            Combat = null;
            OnCombatChanged?.Invoke(null);
        }

        // Privates
        private void JoinCombat(Combat combat)
        {
            Combat = combat;
            combat.TryAddMember(this);
            AnchorPosition = transform.position;
            OnCombatChanged?.Invoke(combat);
        }
    }
}