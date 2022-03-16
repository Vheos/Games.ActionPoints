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

    static public class PlayerOwnable_Extensions
    {
        static public Player PlayerOwner(this ABaseComponent t)
        => t.TryGet(out PlayerOwnable playerOwnable)
            ? playerOwnable.Owner
            : null;
        static public bool TryGetPlayerOwner(this ABaseComponent t, out Player owner)
        {
            if (t.TryGet(out PlayerOwnable playerOwnable)
            && playerOwnable.Owner != null)
            {
                owner = playerOwnable.Owner;
                return true;
            }
            owner = null;
            return false;
        }
        static public bool HasAnyPlayerOwner(this ABaseComponent t)
        => t.TryGet(out PlayerOwnable playerOwnable)
            && playerOwnable.Owner != null;
        static public bool HasPlayerOwner(this ABaseComponent t, Player a)
        => t.PlayerOwner() == a;
        static public bool HasSamePlayerOwnerAs(this ABaseComponent t, ABaseComponent a)
        => t.PlayerOwner() == a.PlayerOwner();
        static public bool TrySetPlayerOwnerIfNull(this ABaseComponent t, Player a)
        {
            if (t.TryGet(out PlayerOwnable playerOwnable)
            && playerOwnable.Owner == null)
            {
                playerOwnable.Owner = a;
                return true;
            }
            return false;
        }
        static public bool TryRemovePlayerOwner(this ABaseComponent t)
        {
            if (t.TryGet(out PlayerOwnable playerOwnable)
            && playerOwnable.Owner != null)
            {
                playerOwnable.Owner = null;
                return true;
            }
            return false;
        }
    }
}