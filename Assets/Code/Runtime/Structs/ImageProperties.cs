namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct ImageProperties
    {
        // Publics
        public Sprite Sprite;
        [Range(0f, 1f)] public float ColorScale;
        [Range(0f, 2f)] public float Scale;
        static public ImageProperties Default
        => new()
        {
            Sprite = null,
            ColorScale = 1f,
            Scale = 1f,
        };
    }
}