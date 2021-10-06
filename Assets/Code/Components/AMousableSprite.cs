namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    [RequireComponent(typeof(SpriteRenderer))]
    abstract public class AMousableSprite : AMousable
    {
        // Inspector
        [Range(0f, 1f)] public float _TransparencyThreshold = 0.5f;

        // Privates
        protected SpriteRenderer _spriteRenderer;

        // Mouse
        public override bool RaycastTest(Vector3 location)
        {
            if (_spriteRenderer.sprite.TryNonNull(out var sprite)
            && sprite.texture.isReadable)
                return sprite.PositionToPixelAlpha(location, transform) >= _TransparencyThreshold;
            return true;
        }

        // Mono
        public override void PlayAwake()
        {
            base.PlayAwake();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}