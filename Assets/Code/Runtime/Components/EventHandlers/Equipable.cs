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
    public class Equipable : AAutoSubscriber
    {
        // Events
        public AutoEvent<Equiper, Equiper> OnChangeEquiper
        { get; } = new AutoEvent<Equiper, Equiper>();

        // Inputs
        public Getter<Slot> EquipSlot
        { get; } = new Getter<Slot>();

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