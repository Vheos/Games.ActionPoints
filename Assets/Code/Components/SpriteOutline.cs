namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using System;

    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteOutline : ABaseComponent
    {
        // Inspector
        [SerializeField] protected Material _Material = null;
        [SerializeField] [Range(0f, 0.1f)] protected float _Thickness = 0.02f;
        [SerializeField] protected Color _Color = Color.white;
        [SerializeField] [Range(0f, 1f)] protected float _FadeInDuration = 0.5f;
        [SerializeField] [Range(0f, 1f)] protected float _FadeOutDuration = 0.5f;

        // Public
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
                QAnimator.GroupAnimate(v => _currentThickness = v, _currentThickness, _Thickness);
                _outlineRenderer.GroupAnimateColor(_Color);
            }
        }
        public void Hide()
        {
            using (QAnimator.Group(this, null, _FadeOutDuration, () => enabled = false))
            {
                QAnimator.GroupAnimate(v => _currentThickness = v, _currentThickness, 0f);
                _outlineRenderer.GroupAnimateAlpha(0f);
            }
        }

        // Private
        static private MaterialPropertyBlock _mprops;
        private SpriteRenderer _outlineRenderer;
        private float _currentThickness;
        private void UpdateMProps()
        {
            string propName = "_Thickness";
            if (_outlineRenderer.sharedMaterial.enableInstancing)
                propName = "Instanced" + propName;

            _outlineRenderer.GetPropertyBlock(_mprops);
            _mprops.SetFloat(propName, _currentThickness);
            _outlineRenderer.SetPropertyBlock(_mprops);
        }

        // Initializer
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static private void StaticInitialize()
        => _mprops = null;

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            if (_mprops == null)
                _mprops = new MaterialPropertyBlock();
            _outlineRenderer = this.CreateChildComponent<SpriteRenderer>(nameof(SpriteOutline));
            _outlineRenderer.sharedMaterial = _Material;
            _outlineRenderer.GODeactivate();
        }
        public override void PlayEnable()
        {
            base.PlayEnable();
            _outlineRenderer.GOActivate();
            UpdateMProps();
        }
        public override void PlayDisable()
        {
            base.PlayDisable();
            _outlineRenderer.GODeactivate();
        }
        protected override void SubscribeToPlayEvents()
        {
            base.SubscribeToPlayEvents();
            Updatable.OnPlayUpdateLate += () =>
            {
                UpdateMProps();
            };
            SpriteChangable.OnSpriteChange += (from, to) =>
            {
                _outlineRenderer.sprite = to;
            };
        }
    }
}