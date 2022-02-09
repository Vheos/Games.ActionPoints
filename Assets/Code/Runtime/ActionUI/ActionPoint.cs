namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(ActionPointMProps))]
    [DisallowMultipleComponent]
    public class ActionPoint : AActionUIElement<ActionPointsBar>
    {
        // Inspector
        [field: SerializeField, Range(0f, 1f)] public float PartialOpacity { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float FullOpacity { get; private set; }
        [field: SerializeField] public Texture2D ActionShape { get; private set; }
        [field: SerializeField] public Texture2D FocusShape { get; private set; }

        public void UpdateInstantly(float visualActionProgress, float visualFocusProgress, int realActionPoints, int realFocusPoints)
        {
            UpdateActionProgress(default, visualActionProgress);
            UpdateFocusProgress(default, visualFocusProgress);
            Get<ActionPointMProps>().Opacity = _index < realActionPoints.Abs() ? FullOpacity : PartialOpacity;
            Get<ActionPointMProps>().Shape = _index < realFocusPoints ? FocusShape : ActionShape;
        }

        // Privates        
        private void UpdateActionProgress(float from, float to)
        => Get<ActionPointMProps>().ActionProgress = to.Abs().Sub(_index).Clamp01() * to.Sig();
        private void UpdateFocusProgress(float from, float to)
        => Get<ActionPointMProps>().FocusProgress = to.Abs().Sub(_index).Clamp01() * to.Sig();
        private void TryUpdateOpacity(int from, int to)
        {
            if (_index >= from.Abs() && _index < to.Abs())
                AnimateOpacity(FullOpacity);
            else if (_index >= to.Abs() && _index < from.Abs())
                AnimateOpacity(PartialOpacity);
        }
        private void TryUpdateShape(int from, int to)
        {
            if (_index >= from && _index < to)
                AnimateShape(FocusShape);
            else if (_index >= to && _index < from)
                AnimateShape(ActionShape);
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
            .AddEventsOnChangeCurveDirection(t => Get<ActionPointMProps>().Shape = to);

        // Play
        override public void Initialize(ActionPointsBar bar)
        {
            base.Initialize(bar);
            Get<ActionPointMProps>().Initialize();

            _group.OnChangeVisualActionProgress.SubEnableDisable(this, UpdateActionProgress);
            _group.OnChangeVisualFocusProgress.SubEnableDisable(this, UpdateFocusProgress);

            _group.OnChangeRealActionPoints.SubEnableDisable(this, TryUpdateOpacity);
            _group.OnChangeRealFocusPoints.SubEnableDisable(this, TryUpdateShape);
        }
    }
}