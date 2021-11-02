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
        private const string THICKNESS_MPROP_NAME = "Thickness";

        // Inspector
        [SerializeField] protected Material _Material = null;
        [SerializeField] [Range(0f, 0.1f)] protected float _Thickness = 0.02f;
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
            enabled = true;
            using (QAnimator.Group(this, null, _FadeInDuration))
            {
                QAnimator.GroupAnimate(v => Thickness = v, Thickness, _Thickness);
                _outlineRenderer.GroupAnimateColor(_Color);
            }
        }
        public void Hide()
        {
            using (QAnimator.Group(this, null, _FadeOutDuration, () => enabled = false))
            {
                QAnimator.GroupAnimate(v => Thickness = v, Thickness, 0f);
                _outlineRenderer.GroupAnimateAlpha(0f);
            }
        }

        // Private
        private MaterialPropertyBlock _mprops;
        private SpriteRenderer _outlineRenderer;
        protected override Renderer TargetRenderer
        => _outlineRenderer;

        // Play
        protected override void AddToComponentCache()
        {
            base.AddToComponentCache();
            AddToCache<SpriteChangable>();
            AddToCache<SpriteRenderer>();
        }
        public override void PlayAwake()
        {
            base.PlayAwake();            
            _mprops = new MaterialPropertyBlock();
            _outlineRenderer = this.CreateChildComponent<SpriteRenderer>( nameof(SpriteOutline));
            _outlineRenderer.sharedMaterial = _Material;
            _outlineRenderer.sprite = Get<SpriteRenderer>().sprite;
            _outlineRenderer.GODeactivate();
        }
        public override void PlayEnable()
        {
            base.PlayEnable();
            _outlineRenderer.GOActivate();
        }
        public override void PlayDisable()
        {
            base.PlayDisable();
            _outlineRenderer.GODeactivate();
        }
        protected override void SubscribeToPlayEvents()
        {
            base.SubscribeToPlayEvents();
            Get<SpriteChangable>().OnSpriteChange += (from, to) =>
            {
                _outlineRenderer.sprite = to;
            };

            if (TryGetComponent<Mousable>(out var mousable))
            {
                mousable.OnGainHighlight += Show;
                mousable.OnLoseHighlight += Hide;
            }
        }
    }
}