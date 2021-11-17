namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Event = Tools.UnityCore.Event;

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
        public bool IsInCombatWith(Combatable other)
        => this != other && Combat == other.Combat;
        public void TryStartCombatWith(Combatable target)
        {
            if (!this.enabled || !target.enabled || target == this)
                return;

            Combat combat;
            if (!this.IsInCombat && !target.IsInCombat)
                combat = new Combat();
            else if (this.IsInCombat)
                combat = this.Combat;
            else
                combat = target.Combat;

            this.TryJoinCombat(combat);
            target.TryJoinCombat(combat);
        }
        public void TryJoinCombat(Combat combat)
        {
            if (IsInCombat || combat == Combat)
                return;

            Combat = combat;
            combat.TryAddMember(this);
            AnchorPosition = transform.position;
            OnCombatChanged?.Invoke(combat);
        }
        public void TryLeaveCombat()
        {
            if (Combat == null)
                return;

            Combat.TryRemoveMember(this);
            Combat = null;
            OnCombatChanged?.Invoke(null);
        }

        // Publics (team-related)
        public IEnumerable<Combatable> Allies
        {
            get
            {
                if (Combat == null
                || !TryGetComponent<Teamable>(out var teamable))
                    yield break;

                foreach (var combatable in Combat.Members)
                    if (combatable.TryGetComponent<Teamable>(out var otherTeamable)
                    && teamable.IsAlliesWith(otherTeamable))
                        yield return combatable;
            }
        }
        public IEnumerable<Combatable> Enemies
        {
            get
            {
                if (Combat == null)
                    yield break;

                Func<Combatable, bool> enemyTest = TryGetComponent<Teamable>(out var teamable) switch
                {
                    true => (other) => other.TryGetComponent<Teamable>(out var otherTeamable)
                                    && teamable.IsEnemiesWith(otherTeamable),
                    false => (other) => this != other,
                };

                foreach (var combatable in Combat.Members)
                    if (enemyTest(combatable))
                        yield return combatable;
            }
        }
        public Vector3 AllyMidpoint
        => Allies.Midpoint();
        public Vector3 EnemyMidpoint
        => Enemies.Midpoint();
        public bool HasAnyAllies
        => Allies.Any();
        public bool HasAnyEnemies
        => Enemies.Any();
    }
}