namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;
    using Tools.Extensions.Collections;

    [DisallowMultipleComponent]
    public class Cursor : ABaseComponent
    {
        [SerializeField] protected Sprite _DefaultState;
        [SerializeField] protected Sprite _PressedState;
    }
}