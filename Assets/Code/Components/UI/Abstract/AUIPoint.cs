namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;

    abstract public class AUIPoint : AUIComponent
    {
        // Publics
        public int Index
        { get; set; }
        public void PlayCantUseAnim()
        => transform.AnimateLocalScaleRatio(Settings.CantUseScale.ToVector3(), Settings.CantUseAnimDuration, Settings.CantUseCurve);

        // Private
        protected ActionPointDrawable _drawable;
        protected UISettings.ActionPointSettings Settings
        => UIManager.Settings.ActionPoint;

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _drawable = Get<ActionPointDrawable>();
        }
        protected override void PlayStart()
        {
            base.PlayStart();
            _drawable.Shape = Settings.ActionShape;
            _drawable.BackgroundColor = Settings.BackgroundColor;
            _drawable.ActionColor = Settings.ActionColor;
            _drawable.FocusColor = Settings.FocusColor;
        }

        // Defines
        private enum MProp
        {
            Shape,
            ColorA,
            ColorB,
            ColorC,
            ThresholdA,
            ThresholdB,
            Opacity,
        }
    }
}