namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    sealed public class ActionPointMProps : AAutoMProps
    {
        // MProps
        public Texture Shape
        {
            get => GetTexture(nameof(Shape));
            set => SetTexture(nameof(Shape), value);
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
    }
}