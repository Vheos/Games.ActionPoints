namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
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
        Explore,
        Combat,
        Camp,
    }

    public enum ActionAgent
    {
        User,
        Target,
    }

    public enum ViewSpace
    {
        World,
        Canvas,
        Screen,
    }
}