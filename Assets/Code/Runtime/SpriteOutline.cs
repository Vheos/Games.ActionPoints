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
        [field: SerializeField] public Color Color { get; private set; } = UnityEngine.Color.white;
        [field: SerializeField, Range(0f, 1f)] public float Thickness { get; private set; } = 0.5f;
        [field: SerializeField, Range(0f, 1f)] public float FadeInDuration { get; private set; }
        [field: SerializeField, Range(0f, 1f)] public float FadeOutDuration { get; private set; }
        [field: SerializeField] public Material Material { get; private set; }

        // Publics
        public void Show()
        {
            _outlineRenderer.gameObject.SetActive(true);
            _outlineRenderer.NewTween(ConflictResolution.Interrupt)
                .SetDuration(FadeInDuration)
                .AddPropertyModifier(v => _mprops.Thickness += v, Thickness - _mprops.Thickness)
                .Color(ColorComponentType.SpriteRenderer, Color);
        }
        public void Hide(bool isInstant = false)
        {
            _outlineRenderer.NewTween(ConflictResolution.Interrupt)
                .SetDuration(FadeOutDuration)
                .AddPropertyModifier(v => _mprops.Thickness += v, 0f - _mprops.Thickness)
                .Alpha(ColorComponentType.SpriteRenderer, 0f)
                .AddEventsOnFinish(() => _outlineRenderer.gameObject.SetActive(false))
                .FinishIf(isInstant);
        }

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
            _outlineRenderer.sharedMaterial = Material;
            _mprops = Get<SpriteOutlineMProps>();
            _mprops.Initialize(_outlineRenderer);

            UpdateOutlineSprite(null, Get<SpriteRenderer>().sprite);
            Hide(true);

            Get<SpriteChangable>().OnChangeSprite.SubEnableDisable(this, UpdateOutlineSprite);
        }
    }
}