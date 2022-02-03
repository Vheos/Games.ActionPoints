namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;

    [RequireComponent(typeof(Selectable))]
    public class SelectableButtonVisuals : ABaseComponent
    {
        // Inspector


        // Privates
        private ColorComponentType _colorComponentType;
        private void Selectable_OnGainHighlight(Selecter selecter, bool isFirst)
        {
            if (isFirst)
                this.NewTween()
                    .SetDuration(0.2f)
                    .LocalScaleRatio(1.1f);
        }
        private void Selectable_OnLoseHighlight(Selecter selecter, bool isLast)
        {
            if (isLast)
                this.NewTween()
                    .SetDuration(0.2f)
                    .LocalScaleRatio(1.1f.Inv());
        }
        private void Selectable_OnPress(Selecter selecter)
        => this.NewTween()
            .SetDuration(0.1f)
            .LocalScaleRatio(0.95f)
            .RGBRatio(_colorComponentType, 0.75f);
        private void Selectable_OnRelease(Selecter selecter, bool withinTrigger)
        => this.NewTween()
            .SetDuration(0.1f)
            .LocalScaleRatio(0.95f.Inv())
            .RGBRatio(_colorComponentType, 0.75f.Inv());

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _colorComponentType = this.FindColorComponentType();
            Get<Selectable>().OnGainSelection.SubscribeAuto(this, Selectable_OnGainHighlight);
            Get<Selectable>().OnLoseSelection.SubscribeAuto(this, Selectable_OnLoseHighlight);
            Get<Selectable>().OnPress.SubscribeAuto(this, Selectable_OnPress);
            Get<Selectable>().OnRelease.SubscribeAuto(this, Selectable_OnRelease);
        }
    }
}