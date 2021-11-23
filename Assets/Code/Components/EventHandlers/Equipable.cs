namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Collections;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    [DisallowMultipleComponent]
    public class Equipable : AEventSubscriber
    {
        // Events
        public Event<Equiper, Equiper> OnChangeEquiper
        { get; } = new Event<Equiper, Equiper>();

        // Publics
        public Slot EquipSlot
        { get; private set; }
        public Equiper Equiper
        {
            get => _equiper;
            internal set
            {
                Equiper previousEquiper = _equiper;
                _equiper = value;

                if (previousEquiper != _equiper)
                {
                    OnChangeEquiper?.Invoke(previousEquiper, _equiper);
                    if (previousEquiper != null)
                        previousEquiper.TryUnequip(this);
                    if (_equiper != null)
                        _equiper.Equip(this);
                }
            }
        }

        // Privates
        private Equiper _equiper;

        // Defines
        public enum Slot
        {
            Hand,
            Head,
        }
    }
}