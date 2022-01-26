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
        [SerializeField] protected Color _Color = Color.white;
        [SerializeField] [Range(0f, 1f)] protected float _Thickness = 0.5f;
        [SerializeField] [Range(0f, 1f)] protected float _FadeInDuration;
        [SerializeField] [Range(0f, 1f)] protected float _FadeOutDuration;
        [SerializeField] protected Material _Material;

        // Publics
        public Color Color
        {
            get => _Color;
            set => _Color = value;
        }
        public void Show()
        {
            _outlineRenderer.gameObject.SetActive(true);
            _outlineRenderer.NewTween()
                .SetDuration(_FadeInDuration)
                .SetConflictResolution(ConflictResolution.Interrupt)
                .AddPropertyModifier(v => _mprops.Thickness += v, _Thickness - _mprops.Thickness)
                .SpriteColor(_Color);
        }
        public void Hide(bool isInstant = false)
        {
            _outlineRenderer.NewTween()
                .SetDuration(_FadeOutDuration)
                .SetConflictResolution(ConflictResolution.Interrupt)
                .AddPropertyModifier(v => _mprops.Thickness += v, 0f - _mprops.Thickness)
                .SpriteAlpha(0f)
                .AddOnFinishEvents(() => _outlineRenderer.gameObject.SetActive(false))
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
            _outlineRenderer.sharedMaterial = _Material;
            _mprops = Get<SpriteOutlineMProps>();
            _mprops.Renderer = _outlineRenderer;

            UpdateOutlineSprite(null, Get<SpriteRenderer>().sprite);
            Hide(true);

            Get<SpriteChangable>().OnChangeSprite.SubscribeAuto(this, UpdateOutlineSprite);
        }
    }
}