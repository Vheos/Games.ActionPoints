namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    [DisallowMultipleComponent]
    public class PlayerOwnable : ABaseComponent
    {
        // Events
        public readonly AutoEvent<Player, Player> OnChangeOwner = new();

        // Publics
        public Player Owner
        {
            get => _owner;
            set
            {
                if (value == _owner)
                    return;

                Player previousOwner = _owner;
                _owner = value;
                OnChangeOwner.Invoke(previousOwner, _owner);
            }
        }

        // Privates
        private Player _owner;
    }
}