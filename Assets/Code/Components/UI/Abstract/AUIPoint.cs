namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Vheos.Tools.Extensions.Math;

    abstract public class AUIPoint : ACustomDrawable, IUIHierarchy
    {
        // Publics
        public UIBase UI
        { get; private set; }

        // MProps
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
            ColorA,
            ColorB,
            ColorC,
            ThresholdA,
            ThresholdB,
            Opacity,
        }

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            BackgroundColor = UI._PointBackgroundColor;
            ActionColor = UI._PointActionColor;
            FocusColor = UI._PointFocusColor;
        }
    }
}