namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using Games.Core;
    using Tools.Extensions.Math;
    using System.Collections.Generic;
    using Tools.Extensions.UnityObjects;
    using Vheos.Tools.Utilities;

    [RequireComponent(typeof(ActionPointMProps))]
    [DisallowMultipleComponent]
    public class ActionPoint : ABaseComponent
    {
        // Inspector
        [SerializeField] [Range(0f, 1f)] protected float _PartialOpacity;
        [SerializeField] [Range(0f, 1f)] protected float _FullOpacity;
        [SerializeField] protected Texture2D _ActionShape;
        [SerializeField] protected Texture2D _FocusShape;

        // Privates
        private int _index;
        private void UpdateActionProgress(float from, float to)
        {
            Get<ActionPointMProps>().ActionProgress = to.Sub(_index).Clamp01();
        }
        private void UpdateFocusProgress(float from, float to)
        {
            Get<ActionPointMProps>().FocusProgress = to.Sub(_index).Clamp01();
        }
        private void UpdateOpacity(int from, int to)
        {
            if (_index >= from.Abs() && _index < to.Abs())
                AnimateOpacity(_FullOpacity);
            else if (_index >= to.Abs() && _index < from.Abs())
                AnimateOpacity(_PartialOpacity);
        }
        private void UpdateShape(int from, int to)
        {
            if (_index >= from && _index < to)
                AnimateShape(_FocusShape);
            else if (_index >= to && _index < from)
                AnimateShape(_ActionShape);
        }
        private void AnimateOpacity(float to)
        => this.NewTween(ConflictResolution.Interrupt)
            .SetDuration(0.5f)
            .AddPropertyModifier(v => Get<ActionPointMProps>().Opacity += v, to - Get<ActionPointMProps>().Opacity);
        private void AnimateShape(Texture2D to)
        => this.NewTween()
            .SetDuration(1f)
            .SetCurveShape(CurveShape.Bounce)
            .LocalScale(transform.localScale.NewY(0f))
            .AddOnChangeCurveValueDirectionEvents(t => Get<ActionPointMProps>().Shape = to);

        // Play
        public void Initialize(ActionPointsBar actionPointsBar, int index)
        {
            _index = index;
            Get<ActionPointMProps>().Initialize();
            actionPointsBar.OnChangeVisualActionProgress.SubscribeAuto(this, UpdateActionProgress);
            actionPointsBar.OnChangeVisualFocusProgress.SubscribeAuto(this, UpdateFocusProgress);
            actionPointsBar.Get<Actionable>().OnChangeActionPoints.SubscribeAuto(this, UpdateOpacity);
            actionPointsBar.Get<Actionable>().OnChangeFocusPoints.SubscribeAuto(this, UpdateShape);

            Get<ActionPointMProps>().Opacity = _PartialOpacity;
            Get<ActionPointMProps>().Shape = _ActionShape;
        }
    }
}