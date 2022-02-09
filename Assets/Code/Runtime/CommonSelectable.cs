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

        // Privates
        private ColorComponentType _colorComponentType;
        private void Selectable_OnGainHighlight(Selecter selecter, bool isFirst)
        {
            if (isFirst)
                this.NewTween()
                    .SetDuration(this.Settings().GainHighlightDuration)
                    .LocalScaleRatio(this.Settings().HighlightScale)
                    .RGBRatio(_colorComponentType, this.Settings().HighlightColorScale);
        }
        private void Selectable_OnLoseHighlight(Selecter selecter, bool isLast)
        {
            if (isLast)
                this.NewTween()
                    .SetDuration(this.Settings().LoseHighlightDuration)
                    .LocalScaleRatio(this.Settings().HighlightScale.Inv())
                    .RGBRatio(_colorComponentType, this.Settings().HighlightColorScale.Inv());
        }
        private void Selectable_OnPress(Selecter selecter)
        => this.NewTween()
            .SetDuration(this.Settings().PressDuration)
            .LocalScaleRatio(this.Settings().PressScale)
            .RGBRatio(_colorComponentType, this.Settings().PressColorScale);
        private void Selectable_OnRelease(Selecter selecter, bool withinTrigger)
        => this.NewTween()
            .SetDuration(this.Settings().ReleaseDuration)
            .LocalScaleRatio(this.Settings().PressScale.Inv())
            .RGBRatio(_colorComponentType, this.Settings().PressColorScale.Inv());

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            Get<Selectable>().OnGainSelection.SubEnableDisable(this, Selectable_OnGainHighlight);
            Get<Selectable>().OnLoseSelection.SubEnableDisable(this, Selectable_OnLoseHighlight);
            Get<Selectable>().OnPress.SubEnableDisable(this, Selectable_OnPress);
            Get<Selectable>().OnRelease.SubEnableDisable(this, Selectable_OnRelease);
            UpdateColorComponentType();
        }
    }
}