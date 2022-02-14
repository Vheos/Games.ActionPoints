namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    public enum InputActionEnum
    {
        Confirm,
        MoveCursor,
    }

    public enum EquipSlot
    {
        Hand,
        Head,
        Accessory,
    }

    public enum ActionPhase
    {
        Combat,
        Camp,
    }
}