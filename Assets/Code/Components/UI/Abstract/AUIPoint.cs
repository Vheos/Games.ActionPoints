namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;

    abstract public class AUIPoint : AUIComponent
    {
        // Publics
        public int Index
        { get; set; }
        public void PlayCantUseAnim()
        => transform.AnimateLocalScale(this, _originalScale, _originalScale * Settings.CantUseScale, Settings.CantUseAnimDuration, null, QAnimatorOLD.Curve.Boomerang);

        // Private
        protected ActionPointDrawable _drawable;
        private Vector2 _originalScale;
        protected UISettings.ActionPointSettings Settings
        => UIManager.Settings.ActionPoint;

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _originalScale = transform.localScale;
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