namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    sealed public class SpriteShadowMProps : ASpriteRendererAutoMProps
    {
        // Publics
        public float OpacityDitheringSize
        {
            get => GetFloat(nameof(OpacityDitheringSize));
            set => SetFloat(nameof(OpacityDitheringSize), value);
        }
        public float OpacityDitheringRatio
        {
            get => GetFloat(nameof(OpacityDitheringRatio));
            set => SetFloat(nameof(OpacityDitheringRatio), value);
        }

        public override void Initialize()
        {
            Initialize(Get<SpriteRenderer>());
            OpacityDitheringSize = 100 / 3f;
            OpacityDitheringRatio = 1f;
        }
    }
}