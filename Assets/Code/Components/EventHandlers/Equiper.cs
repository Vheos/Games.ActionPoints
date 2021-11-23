namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Collections;
    using static Equipable;

    [DisallowMultipleComponent]
    sealed public class Equiper : AEventSubscriber
    {
        // Events
        public Event<Equipable> OnEquip
        { get; } = new Event<Equipable>();
        public Event<Equipable> OnUnequip
        { get; } = new Event<Equipable>();

        // Publics
        public IReadOnlyDictionary<Slot, Equipable> EquipablesBySlot
        => _equipablesBySlot;
        public bool HasEquiped(Slot slot)
        => _equipablesBySlot.ContainsKey(slot);
        public bool HasEquiped(Equipable equipable)
        => _equipablesBySlot.ContainsValue(equipable);
        public Equipable GetEquiped(Slot slot)
        => _equipablesBySlot[slot];
        public bool TryGetEquiped(Slot slot, out Equipable equipable)
        => _equipablesBySlot.TryGetValue(slot, out equipable);

        // Privates
        private readonly Dictionary<Slot, Equipable> _equipablesBySlot = new Dictionary<Slot, Equipable>();
        internal void Equip(Equipable equipable)
        {
            Slot slot = equipable.EquipSlot;
            TryUnequip(slot);

            equipable.Equiper = this;
            _equipablesBySlot[slot] = equipable;
            OnEquip?.Invoke(equipable);
        }
        internal void TryUnequip(Slot slot)
        {
            if (!TryGetEquiped(slot, out var equipable))
                return;

            equipable.Equiper = null;
            _equipablesBySlot.Remove(slot);
            OnUnequip?.Invoke(equipable);
        }
        internal void TryUnequip(Equipable equipable)
        {
            if (HasEquiped(equipable))
                TryUnequip(equipable.EquipSlot);
        }
    }
}