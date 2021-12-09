namespace Vheos.Games.ActionPoints.Test
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteBackface : AAutoSubscriber
    {
        // Private
        private SpriteRenderer _backfaceSpriteRenderer;

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _backfaceSpriteRenderer = this.CreateChildComponent<SpriteRenderer>(nameof(SpriteOutline));
            _backfaceSpriteRenderer.sharedMaterial = Get<SpriteRenderer>().sharedMaterial;
            _backfaceSpriteRenderer.transform.Rotate(Vector3.up, 180f);
            _backfaceSpriteRenderer.flipX = true;

            Get<SpriteRenderer>().shadowCastingMode = _backfaceSpriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            Get<SpriteRenderer>().receiveShadows = _backfaceSpriteRenderer.receiveShadows = true;
        }
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<SpriteChangable>().OnChangeSprite, (from, to) => _backfaceSpriteRenderer.sprite = to);
        }
    }
}