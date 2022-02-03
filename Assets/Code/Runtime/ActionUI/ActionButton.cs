namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Utilities;

    [RequireComponent(typeof(Raycastable))]
    [RequireComponent(typeof(Selectable))]
    [DisallowMultipleComponent]
    public class ActionButton : ABaseComponent
    {
        // Inspector
        [SerializeField] [Range(0.1f, 1f)] protected float _Radius;

        // Publics
        public ActionButtonsWheel Wheel
        { get; private set; }
        public float Radius
        => _Radius;

        // Play
        public void Initialize(ActionButtonsWheel wheel, int index)
        {
            Wheel = wheel;
            name = $"Button{index + 1}";
            BindEnableDisable(wheel);
        }
        protected override void PlayEnable()
        {
            base.PlayEnable();
            Get<Raycastable>().Enable();
            Get<Selectable>().Enable();
        }
        protected override void PlayDisable()
        {
            base.PlayDisable();
            Get<Raycastable>().Disable();
            Get<Selectable>().Disable();
        }
    }
}