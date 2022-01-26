
namespace Vheos.Games.ActionPoints
{
    using UnityEngine;

    sealed public class SpriteOutlineMProps : AAutoMProps
    {
        // Constants
        private const string SPRITE_RENDERER_INTERNAL_TEXTURE_NAME = "_MainTex";

        // Publics
        public float Thickness
        {
            get => GetFloat(nameof(Thickness));
            set => SetFloat(nameof(Thickness), value);
        }
        public Texture InternalTexture
        {
            get => GetTexture(SPRITE_RENDERER_INTERNAL_TEXTURE_NAME);
            set => SetTexture(SPRITE_RENDERER_INTERNAL_TEXTURE_NAME, value);
        }
    }
}