namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    sealed public class ActionPointDrawable : ACustomDrawable
    {
        // Pubilcs
        public void AnimateOpacity(float to, float duration)
        => this.Animate(nameof(Opacity), v => Opacity = v, Opacity, to, duration);

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