namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;

    sealed public class Combat : AComponentGroup<Combatable>
    {
        // Publics
        public override void TryRemoveMember(Combatable member)
        {
            base.TryRemoveMember(member);
            if (_members.Count == 1)
                _members[0].TryLeaveCombat();
        }

        // Privates
        private Dictionary<Team, Combatable> _combatablesByTeam;

        // Initializers
        public Combat() : base()
        => _combatablesByTeam = new Dictionary<Team, Combatable>();
        public Combat(params Combatable[] combatables) : this()
        => _members.AddRange(combatables);
    }
}