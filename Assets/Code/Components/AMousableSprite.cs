namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    [RequireComponent(typeof(SpriteRenderer))]
    abstract public class AMousableSprite : AMousable
    {
        // Inspector
        [SerializeField] [Range(0f, 1f)] protected float _TransparencyThreshold = 0.5f;

        // Privates
        protected SpriteRenderer _spriteRenderer;

        // Mouse
        public override bool RaycastTest(Vector3 location)
        {
            if (_spriteRenderer.sprite.texture.isReadable)
                return _spriteRenderer.sprite.PositionToPixelAlpha(location, transform) >= _TransparencyThreshold;
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