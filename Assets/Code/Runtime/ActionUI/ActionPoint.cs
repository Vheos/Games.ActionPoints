namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(ActionPointMProps))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    [DisallowMultipleComponent]
    public class ActionPoint : AActionUIElement<ActionPointsBar>
    {
        // Publics
        public void UpdateInstantly(float visualActionProgress, float visualFocusProgress, int realActionPoints, int realFocusPoints)
        {
            UpdateActionProgress(default, visualActionProgress);
            UpdateFocusProgress(default, visualFocusProgress);
            Get<ActionPointMProps>().Opacity = _index < realActionPoints.Abs() ? this.Settings().FullOpacity : this.Settings().PartialOpacity;
            Get<ActionPointMProps>().Shape = _index < realFocusPoints ? this.Settings().FocusShape : this.Settings().ActionShape;
        }

        // Privates        
        private void UpdateActionProgress(float from, float to)
        => Get<ActionPointMProps>().ActionProgress = to.Abs().Sub(_index).Clamp01() * to.Sig();
        private void UpdateFocusProgress(float from, float to)
        => Get<ActionPointMProps>().FocusProgress = to.Abs().Sub(_index).Clamp01() * to.Sig();
        private void TryUpdateOpacity(int from, int to)
        {
            if (_index >= from.Abs() && _index < to.Abs())
                AnimateOpacity(this.Settings().FullOpacity);
            else if (_index >= to.Abs() && _index < from.Abs())
                AnimateOpacity(this.Settings().PartialOpacity);
        }
        private void TryUpdateShape(int from, int to)
        {
            if (_index >= from && _index < to)
                AnimateShape(this.Settings().FocusShape);
            else if (_index >= to && _index < from)
                AnimateShape(this.Settings().ActionShape);
        }
        private void AnimateOpacity(float to)
        => this.NewTween(ConflictResolution.Interrupt)
            .SetDuration(this.Settings().ChangeOpacityDuration)
            .AddPropertyModifier(v => Get<ActionPointMProps>().Opacity += v, to - Get<ActionPointMProps>().Opacity);
        private void AnimateShape(Texture2D to)
        => this.NewTween()
            .SetDuration(this.Settings().ChangeShapeDuration)
            .SetCurveShape(CurveShape.Bounce)
            .LocalScale(transform.localScale.NewY(0f))
            .AddEventsOnChangeCurveDirection(t => Get<ActionPointMProps>().Shape = to);

        // Play
        override public void Initialize(ActionPointsBar bar)
        {
            base.Initialize(bar);
            Get<ActionPointMProps>().Initialize();
            Get<MeshRenderer>().sharedMaterial = this.Settings().Material;
            Get<MeshFilter>().sharedMesh = SettingsManager.Visual.General.SpriteMesh;

            _group.OnChangeVisualActionProgress.SubEnableDisable(this, UpdateActionProgress);
            _group.OnChangeVisualFocusProgress.SubEnableDisable(this, UpdateFocusProgress);

            _group.OnChangeRealActionPoints.SubEnableDisable(this, TryUpdateOpacity);
            _group.OnChangeRealFocusPoints.SubEnableDisable(this, TryUpdateShape);
        }
    }
}