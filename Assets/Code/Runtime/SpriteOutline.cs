namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SpriteOutlineMProps))]
    [RequireComponent(typeof(SpriteChangable))]
    public class SpriteOutline : ABaseComponent
    {
        // Inspector
        [field: SerializeField] public Color Color { get; private set; } = Color.white;

        // Publics
        public void Show(bool instantly = false)
        {
            _outlineRenderer.gameObject.SetActive(true);
            _outlineRenderer.NewTween(ConflictResolution.Interrupt)
                .SetDuration(this.Settings().ExpandDuration)
                .AddPropertyModifier(v => _mprops.Thickness += v, this.Settings().Thickness - _mprops.Thickness)
                .Color(ColorComponent.SpriteRenderer, Color)
                .FinishIf(instantly);
        }
        public void Hide(bool instantly = false)
        => _outlineRenderer.NewTween(ConflictResolution.Interrupt)
            .SetDuration(this.Settings().CollapseDuration)
            .AddPropertyModifier(v => _mprops.Thickness += v, 0f - _mprops.Thickness)
            .Alpha(ColorComponent.SpriteRenderer, 0f)
            .AddEventsOnFinish(() => _outlineRenderer.gameObject.SetActive(false))
            .FinishIf(instantly);


        // Private
        private SpriteOutlineMProps _mprops;
        private SpriteRenderer _outlineRenderer;
        private void UpdateOutlineSprite(Sprite from, Sprite to)
        {
            _outlineRenderer.sprite = to;
            _mprops.InternalTexture = to.texture;
        }

        // Play
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _outlineRenderer = this.CreateChildComponent<SpriteRenderer>(nameof(SpriteOutline));
            _outlineRenderer.sharedMaterial = this.Settings().Material;
            _mprops = Get<SpriteOutlineMProps>();
            _mprops.Initialize(_outlineRenderer);

            UpdateOutlineSprite(null, Get<SpriteRenderer>().sprite);
            Hide(true);

            Get<SpriteChangable>().OnChangeSprite.SubEnableDisable(this, UpdateOutlineSprite);
        }
    }
}