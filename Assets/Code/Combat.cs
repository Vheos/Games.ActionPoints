namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;

    sealed public class Combat : AComponentGroup<Combatable>
    {
        // Publics
        public override void RemoveMember(Combatable member)
        {
            base.RemoveMember(member);
            if (_members.Count == 1)
                _members[0].LeaveCombat();
        }

        // Initializers
        public Combat(params Combatable[] combatables) : base()
        => _members.AddRange(combatables);
    }
}