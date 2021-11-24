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

        // Inputs
        public ComponentInput<Slot> EquipSlot
        { get; } = new ComponentInput<Slot>();

        // Publics
        public Equiper Equiper
        {
            get => _equiper;
            internal set
            {
                Equiper previousEquiper = _equiper;
                _equiper = value;

                if (previousEquiper != _equiper)
                    OnChangeEquiper?.Invoke(previousEquiper, _equiper);
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