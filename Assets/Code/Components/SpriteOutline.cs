namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

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
                this.GroupAnimate(v => _currentThickness = v, _currentThickness, _Thickness);
                _outlineSpriteRenderer.GroupAnimateColor(_Color);
            }
        }
        public void Hide()
        {
            using (QAnimator.Group(this, null, _FadeOutDuration, () => enabled = false))
            {
                this.GroupAnimate(v => _currentThickness = v, _currentThickness, 0f);
                _outlineSpriteRenderer.GroupAnimateColor(_Color.NewA(0f));
            }
        }

        // Private
        static private MaterialPropertyBlock _mprops;
        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer _outlineSpriteRenderer;
        private float _currentThickness;
        private void UpdateMaterialProperties()
        {
            _outlineSpriteRenderer.sprite = _spriteRenderer.sprite;

            string propName = "_Thickness";
            if (_outlineSpriteRenderer.sharedMaterial.enableInstancing)
                propName = "Instanced" + propName;

            _outlineSpriteRenderer.GetPropertyBlock(_mprops);
            _mprops.SetFloat(propName, _currentThickness);
            _outlineSpriteRenderer.SetPropertyBlock(_mprops);
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
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _outlineSpriteRenderer = new GameObject(nameof(SpriteOutline)).AddComponent<SpriteRenderer>();
            _outlineSpriteRenderer.BecomeChildOf(this);
            _outlineSpriteRenderer.sharedMaterial = _Material;
            _outlineSpriteRenderer.sortingOrder = _spriteRenderer.sortingOrder - 1;
            _outlineSpriteRenderer.GODeactivate();
        }
        public override void PlayEnable()
        {
            base.PlayEnable();
            _outlineSpriteRenderer.GOActivate();
            UpdateMaterialProperties();
        }
        public override void PlayDisable()
        {
            base.PlayDisable();
            _outlineSpriteRenderer.GODeactivate();
        }
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            OnPlayUpdateLate += () =>
            {
                UpdateMaterialProperties();
            };
        }
    }
}