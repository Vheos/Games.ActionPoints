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
        // Publics
        public Actionable Actionable
        { get; private set; }
        public readonly Getter<Rect> Rect = new();
        public ActionPointsBar Points
        { get; private set; }
        public IReadOnlyDictionary<ActionPhase, ActionButtonsWheel> ButtonWheels
        => _buttonsWheelsByPhase;
        public void ExpandButtonWheel(ActionPhase phase, bool instantly = false)
        {
            foreach (var buttonWheelByPhase in _buttonsWheelsByPhase)
                if (buttonWheelByPhase.Key == phase)
                    buttonWheelByPhase.Value.Get<Expandable>().TryExpand(instantly);
                else
                    buttonWheelByPhase.Value.Get<Expandable>().TryCollapse(instantly);
        }
        public void CollapseButtonWheels(bool instantly = false)
        {
            foreach (var buttonWheelByPhase in _buttonsWheelsByPhase)
                buttonWheelByPhase.Value.Get<Expandable>().TryCollapse(instantly);
        }

        // Privates
        private Dictionary<ActionPhase, ActionButtonsWheel> _buttonsWheelsByPhase;

        // Play
        public void Initialize(Actionable actionable, Func<Rect> rectGetter)
        {
            Actionable = actionable;
            Rect.Set(rectGetter);
            name = $"{Actionable.name}_{typeof(ActionUI).Name}";

            Get<MoveTowards>().SetTarget(Actionable, true);
            Get<RotateAs>().SetTarget(CameraManager.AnyNonUI, true);

            Points = this.CreateChildComponent(SettingsManager.Prefabs.ActionPointsBar);
            Points.Initialize(this);

            _buttonsWheelsByPhase = new Dictionary<ActionPhase, ActionButtonsWheel>();
            foreach (var phase in Utility.GetEnumValues<ActionPhase>())
            {
                var newButtonsWheel = this.CreateChildComponent(SettingsManager.Prefabs.ActionButtonsWheel);
                newButtonsWheel.Initialize(this, phase);
                _buttonsWheelsByPhase[phase] = newButtonsWheel;
            }
        }
    }
}