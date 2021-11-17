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
        [SerializeField] protected Material _Material = null;
        [SerializeField] [Range(0f, 1f)] protected float _Thickness = 0.5f;
        [SerializeField] protected Color _Color = Color.white;
        [SerializeField] [Range(0f, 1f)] protected float _FadeInDuration = 0.5f;
        [SerializeField] [Range(0f, 1f)] protected float _FadeOutDuration = 0.5f;

        // Public
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
            using (QAnimator.Group(this, null, _FadeInDuration))
            {
                QAnimator.GroupAnimate(v => Thickness = v, Thickness, _Thickness);
                _outlineRenderer.GroupAnimateColor(_Color);
            }
        }
        public void Hide(bool instantly = false)
        {
            using (QAnimator.Group(this, null, instantly ? 0f : _FadeOutDuration, _outlineRenderer.GODeactivate))
            {
                QAnimator.GroupAnimate(v => Thickness = v, Thickness, 0f);
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
            SubscribeTo(Get<SpriteChangable>().OnSpriteChanged, UpdateOutlineSprite);
        }
        protected override void InitializeRenderer(out Renderer renderer)
        {
            _outlineRenderer = this.CreateChildComponent<SpriteRenderer>(nameof(SpriteOutline));
            renderer = _outlineRenderer;
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _outlineRenderer.sharedMaterial = _Material;
            _outlineRenderer.sprite = Get<SpriteRenderer>().sprite;
            Hide(true);
        }
    }
}