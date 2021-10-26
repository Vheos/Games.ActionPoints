namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    [RequireComponent(typeof(SpriteRenderer))]
    abstract public class MousableSprite : Mousable
    {
        // Inspector
        [SerializeField] [Range(0f, 1f)] protected float _TransparencyThreshold = 0.5f;

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

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}