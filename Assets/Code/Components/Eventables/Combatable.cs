namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    public class Combatable : ABaseComponent
    {
        // Events
        public event System.Action<bool> OnCombatStateChanged;

        // Publics
        public Vector3 AnchorPosition
        { get; private set; }


        public bool TryStartCombat(Combatable combatable)
        {

            return true;
        }
    }
}