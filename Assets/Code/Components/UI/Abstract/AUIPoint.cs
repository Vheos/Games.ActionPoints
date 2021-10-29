namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    abstract public class AUIPoint : ACustomDrawable, IUIHierarchy
    {
        // Publics
        public UIBase UI
        { get; private set; }
        public int Index
        { get; set; }
        public void PlayCantUseAnim()
        => transform.AnimateLocalScale(this, _originalScale, _originalScale * Settings.CantUseScale, Settings.CantUseAnimDuration, null, QAnimator.Curve.Boomerang);

        // Private
        private Vector2 _originalScale;
        protected UISettings.ActionPointSettings Settings
        => UIManager.Settings.ActionPoint;

        // MProps
        public Texture Shape
        {
            get => GetTexture(nameof(MProp.Shape));
            set => SetTexture(nameof(MProp.Shape), value);
        }
        public Color BackgroundColor
        {
            get => GetColor(nameof(MProp.ColorC));
            set => SetColor(nameof(MProp.ColorC), value);
        }
        public Color ActionColor
        {
            get => GetColor(nameof(MProp.ColorB));
            set => SetColor(nameof(MProp.ColorB), value);
        }
        public Color FocusColor
        {
            get => GetColor(nameof(MProp.ColorA));
            set => SetColor(nameof(MProp.ColorA), value);
        }
        public float Opacity
        {
            get => GetFloat(nameof(MProp.Opacity));
            set => SetFloat(nameof(MProp.Opacity), value);
        }
        public float ActionProgress
        {
            get => GetFloat(nameof(MProp.ThresholdB));
            set => SetFloat(nameof(MProp.ThresholdB), value);
        }
        public float FocusProgress
        {
            get => GetFloat(nameof(MProp.ThresholdA));
            set => SetFloat(nameof(MProp.ThresholdA), value);
        }
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

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            _originalScale = transform.localScale;
            Shape = Settings.ActionShape;
            BackgroundColor = Settings.BackgroundColor;
            ActionColor = Settings.ActionColor;
            FocusColor = Settings.FocusColor;
        }
    }
}