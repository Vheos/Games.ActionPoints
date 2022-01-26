namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    sealed public class ActionPointMProps : AAutoMProps
    {
        // Pubilcs
        public void AnimateOpacity(float to, float duration)
        => Tween.New
            .SetDuration(duration)
            .AddPropertyModifier(v => Opacity += v, to - Opacity);

        // MProps
        public Texture Shape
        {
            get => GetTexture(nameof(Shape));
            set => SetTexture(nameof(Shape), value);
        }
        public Color BackgroundColor
        {
            get => GetColor(nameof(BackgroundColor));
            set => SetColor(nameof(BackgroundColor), value);
        }
        public Color ActionColor
        {
            get => GetColor(nameof(ActionColor));
            set => SetColor(nameof(ActionColor), value);
        }
        public Color FocusColor
        {
            get => GetColor(nameof(FocusColor));
            set => SetColor(nameof(FocusColor), value);
        }
        public float Opacity
        {
            get => GetFloat(nameof(Opacity));
            set => SetFloat(nameof(Opacity), value);
        }
        public float ActionProgress
        {
            get => GetFloat(nameof(ActionProgress));
            set => SetFloat(nameof(ActionProgress), value);
        }
        public float FocusProgress
        {
            get => GetFloat(nameof(FocusProgress));
            set => SetFloat(nameof(FocusProgress), value);
        }

        // Defines
        private enum MProp
        {
            Shape,
            ColorA,  // Focus
            ColorB,  // Action
            ColorC,  // Background
            ThresholdA,  // Focus
            ThresholdB,  // Action
            Opacity,
        }
    }
}