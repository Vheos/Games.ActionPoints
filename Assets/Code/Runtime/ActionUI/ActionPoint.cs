namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;

    [RequireComponent(typeof(ActionPointMProps))]
    [DisallowMultipleComponent]
    public class ActionPoint : ABaseComponent
    {
        // Inspector
        [SerializeField] [Range(0f, 1f)] protected float _PartialOpacity;
        [SerializeField] [Range(0f, 1f)] protected float _FullOpacity;
        [SerializeField] protected Texture2D _ActionShape;
        [SerializeField] protected Texture2D _FocusShape;

        // Publics
        public ActionPointsBar Bar
        { get; private set; }

        // Privates
        private int _index;
        private void UpdateActionProgress(float from, float to)
        => Get<ActionPointMProps>().ActionProgress = to.Abs().Sub(_index).Clamp01() * to.Sig();
        private void UpdateFocusProgress(float from, float to)
        => Get<ActionPointMProps>().FocusProgress = to.Abs().Sub(_index).Clamp01() * to.Sig();
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
        private void UpdateInstantly()
        {
            Get<ActionPointMProps>().Opacity = _index < Bar.UI.Actionable.ActionPoints.Abs() ? _FullOpacity : _PartialOpacity;
            Get<ActionPointMProps>().Shape = _index < Bar.UI.Actionable.FocusPoints ? _FocusShape : _ActionShape;
        }

        // Play
        public void Initialize(ActionPointsBar bar, int index)
        {
            Bar = bar;
            _index = index;
            name = $"Point{_index + 1}";
            BindEnableDisable(bar);

            Get<ActionPointMProps>().Initialize();
            OnPlayEnable.Sub(UpdateInstantly);
            UpdateInstantly();

            Bar.OnChangeVisualActionProgress.SubEnableDisable(this, UpdateActionProgress);
            Bar.OnChangeVisualFocusProgress.SubEnableDisable(this, UpdateFocusProgress);

            Bar.UI.Actionable.OnChangeActionPoints.SubEnableDisable(this, UpdateOpacity);
            Bar.UI.Actionable.OnChangeFocusPoints.SubEnableDisable(this, UpdateShape);
        }
    }
}