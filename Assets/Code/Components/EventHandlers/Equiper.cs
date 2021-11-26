namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Collections;
    using static Equipable;
    using Tools.Extensions.General;

    [DisallowMultipleComponent]
    sealed public class Equiper : AEventSubscriber
    {
        // Events
        public Event<Slot, Equipable, Equipable> OnChangeEquipable
        { get; } = new Event<Slot, Equipable, Equipable>();

        // Inputs
        public ComponentInput<Slot, Transform> AttachTransformsBySlot
        { get; } = new ComponentInput<Slot, Transform>();

        // Publics
        public IReadOnlyDictionary<Slot, Equipable> EquipablesBySlot
        => _equipablesBySlot;
        public void TryEquip(Equipable equipable)
        {
            if (equipable == null || HasEquiped(equipable))
                return;

            Slot slot = equipable.EquipSlot;
            TryGetEquiped(slot, out var previousEquipable);
            if (previousEquipable != null)
                RemoveEquipable(previousEquipable);

            AddEquipable(equipable);
            OnChangeEquipable?.Invoke(slot, previousEquipable, equipable);
        }
        public void TryUnequip(Slot slot)
        {
            if (!HasEquiped(slot))
                return;

            Equipable equipable = _equipablesBySlot[slot];
            RemoveEquipable(equipable);
            OnChangeEquipable?.Invoke(slot, equipable, null);
        }
        public void TryUnequip(Equipable equipable)
        {
            if (!HasEquiped(equipable))
                return;

            RemoveEquipable(equipable);
            OnChangeEquipable?.Invoke(equipable.EquipSlot, equipable, null);
        }
        public bool HasEquiped(Slot slot)
        => _equipablesBySlot.ContainsKey(slot);
        public bool HasEquiped(Equipable equipable)
        => TryGetEquiped(equipable.EquipSlot, out var equiped) && equipable == equiped;
        public Equipable GetEquiped(Slot slot)
        => _equipablesBySlot[slot];
        public bool TryGetEquiped(Slot slot, out Equipable equipable)
        => _equipablesBySlot.TryGetValue(slot, out equipable);

        // Privates
        private readonly Dictionary<Slot, Equipable> _equipablesBySlot = new Dictionary<Slot, Equipable>();
        private void AddEquipable(Equipable equipable)
        {
            _equipablesBySlot.Add(equipable.EquipSlot, equipable);
            equipable.Equiper = this;
        }
        private void RemoveEquipable(Equipable equipable)
        {
            _equipablesBySlot.Remove(equipable.EquipSlot);
            equipable.Equiper = null;
        }
    }
}