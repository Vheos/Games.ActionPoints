namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using System;

    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteOutline : ACustomDrawable
    {
        // Constants
        private const string TEXTURE_MPROP_NAME = "_MainTex";

        // Inspector
        [SerializeField] protected Color _Color = Color.white;
        [SerializeField] [Range(0f, 1f)] protected float _Thickness = 0.5f;

        // Public
        public UISettings.SpriteOutlineSettings Settings
        => UIManager.Settings.SpriteOutline;
        public float Thickness
        {
            get => GetFloat(nameof(Thickness));
            set => SetFloat(nameof(Thickness), value);
        }
        public Color Color
        {
            get => _Color;
            set => _Color = value;
        }
        public void Show()
        {
            _outlineRenderer.GOActivate();
            using (QAnimator.Group(Settings.FadeInDuration, _outlineRenderer.Interrupt()))
            {
                QAnimator.GroupAnimate(v => Thickness += v, _Thickness - Thickness);
                _outlineRenderer.GroupAnimateColor(_Color);
            }
        }
        public void Hide(bool isInstant = false)
        {
            using (QAnimator.Group(isInstant ? 0f : Settings.FadeOutDuration, _outlineRenderer.InterruptAndDeactivate()))
            {
                QAnimator.GroupAnimate(v => Thickness += v, 0f - Thickness);
                _outlineRenderer.GroupAnimateAlpha(0f);
            }
        }

        // Private
        private SpriteRenderer _outlineRenderer;
        private void UpdateOutlineSprite(Sprite from, Sprite to)
        {
            _outlineRenderer.sprite = to;
            SetTexture(TEXTURE_MPROP_NAME, to.texture);
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            if (TryGet<SpriteChangable>(out var spriteChangable))
                SubscribeTo(spriteChangable.OnChangeSprite, UpdateOutlineSprite);
        }
        protected override void InitializeRenderer(out Renderer renderer)
        {
            _outlineRenderer = this.CreateChildComponent<SpriteRenderer>(nameof(SpriteOutline));
            renderer = _outlineRenderer;
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _outlineRenderer.sharedMaterial = Settings.Material;
            UpdateOutlineSprite(null, Get<SpriteRenderer>().sprite);
            Hide(true);
        }
    }
}