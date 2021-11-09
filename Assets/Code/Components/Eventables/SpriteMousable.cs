namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Vheos.Tools.Extensions.General;
    using Vheos.Tools.Extensions.UnityObjects;

    sealed public class SpriteMousable : Mousable
    {
        // Mousable
        private const float CURSOR_RAYCAST_TEST_ALPHA_CUTOFF = 0.5f;
        private bool CursorRaycastTest(Vector3 position)
        {
            if (Renderer.sprite.TryNonNull(out var sprite)
            && sprite.texture.isReadable)
                return sprite.PositionToPixelAlpha(position, transform) >= CURSOR_RAYCAST_TEST_ALPHA_CUTOFF;
            return true;
        }

        // Privates
        public SpriteRenderer Renderer
        { get; private set; }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            Renderer = GetComponent<SpriteRenderer>();
            RaycastTests += CursorRaycastTest;
        }
    }
}