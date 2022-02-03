namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.Extensions.General;

    [Serializable]
    public struct ActionButtonVisuals
    {
        // Publics
        public Sprite Sprite;
        public string Text;
        public Color Color;
        public bool HasAnyVisuals
        => Sprite != null || Text.IsNotNullOrEmpty();
    }
}