namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;

    sealed public class SpriteOutlineMProps : ASpriteRendererAutoMProps
    {
        // Publics
        public float Thickness
        {
            get => GetFloat(nameof(Thickness));
            set => SetFloat(nameof(Thickness), value);
        }
    }
}