namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;  

    sealed public class Combat : AComponentGroup<Combatable>
    {
        // Publics
        public override void TryRemoveMember(Combatable member)
        {
            base.TryRemoveMember(member);

            if (!_isEnding
            && _members.TryGet(0, out var firstMember)
            && !firstMember.HasAnyEnemies)
                End();       
        }

        // Privates
        private bool _isEnding;
        private void End()
        {
            _isEnding = true;
            foreach (var member in _members.MakeCopy())
                member.TryLeaveCombat();
            _isEnding = false;
        }

        // Initializers
        public Combat(params Combatable[] combatables) : base()
        => _members.AddRange(combatables);
    }
}