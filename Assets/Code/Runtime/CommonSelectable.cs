namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;

    [RequireComponent(typeof(Selectable))]
    public class CommonSelectable : ABaseComponent
    {
        // Publics
        public void UpdateColorComponentType()
        => _colorComponentType = this.FindColorComponentType();
        public readonly Getter<float> HighlightScale = new();
        public readonly Getter<float> PressScale = new();

        // Privates
        private ColorComponent _colorComponentType;
        private void Selectable_OnGainHighlight(Selecter selecter, bool isFirst)
        {
            if (isFirst)
                this.NewTween()
                    .SetDuration(this.Settings().GainHighlightDuration)
                    .LocalScaleRatio(HighlightScale)
                    .RGBRatio(_colorComponentType, this.Settings().HighlightColorScale);
        }
        private void Selectable_OnLoseHighlight(Selecter selecter, bool isLast)
        {
            if (isLast)
                this.NewTween()
                    .SetDuration(this.Settings().LoseHighlightDuration)
                    .LocalScaleRatio(HighlightScale.Value.Inv())
                    .RGBRatio(_colorComponentType, this.Settings().HighlightColorScale.Inv());
        }
        private void Selectable_OnPress(Selecter selecter)
        => this.NewTween()
            .SetDuration(this.Settings().PressDuration)
            .LocalScaleRatio(PressScale)
            .RGBRatio(_colorComponentType, this.Settings().PressColorScale);
        private void Selectable_OnRelease(Selecter selecter, bool isFullClick)
        => this.NewTween()
            .SetDuration(this.Settings().ReleaseDuration)
            .LocalScaleRatio(PressScale.Value.Inv())
            .RGBRatio(_colorComponentType, this.Settings().PressColorScale.Inv());

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            HighlightScale.Set(() => this.Settings().HighlightScale);
            PressScale.Set(() => this.Settings().PressScale);
            Get<Selectable>().OnGainSelection.SubEnableDisable(this, Selectable_OnGainHighlight);
            Get<Selectable>().OnLoseSelection.SubEnableDisable(this, Selectable_OnLoseHighlight);
            Get<Selectable>().OnPress.SubEnableDisable(this, Selectable_OnPress);
            Get<Selectable>().OnRelease.SubEnableDisable(this, Selectable_OnRelease);
            UpdateColorComponentType();
        }
    }
}