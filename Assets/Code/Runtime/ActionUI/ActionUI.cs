namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Utilities;

    [RequireComponent(typeof(MoveTowards))]
    [RequireComponent(typeof(RotateAs))]
    [DisallowMultipleComponent]
    public class ActionUI : ABaseComponent
    {
        // Inspector
        [SerializeField] protected ActionPointsBar _PointsBarPrefab;
        [SerializeField] protected ActionButtonsWheel _ButtonsWheelPrefab;

        // Publics
        public Actionable Actionable
        { get; private set; }
        public readonly Getter<Rect> Rect = new();
        public ActionPointsBar PointsBar
        { get; private set; }
        public ActionButtonsWheel ButtonsWheel
        { get; private set; }

        // Play
        public void Initialize(Actionable actionable, Func<Rect> rectGetter)
        {
            Actionable = actionable;
            Rect.Set(rectGetter);
            name = $"{Actionable.name}_{typeof(ActionUI).Name}";

            Get<MoveTowards>().SetTarget(Actionable, true);
            Get<RotateAs>().SetTarget(CameraManager.AnyNonUI, true);

            PointsBar = this.CreateChildComponent(_PointsBarPrefab);
            PointsBar.Initialize(this);

            ButtonsWheel = this.CreateChildComponent(_ButtonsWheelPrefab);
            ButtonsWheel.Initialize(this);
        }
    }
}