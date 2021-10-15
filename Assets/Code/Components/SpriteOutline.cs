namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteOutline : AUpdatable
    {
        // Inspector
        public Material _Material = null;
        [Range(0f, 0.1f)] public float _Thickness = 0.02f;
        public Color _Color = Color.white;
        [Range(0f, 1f)] public float _AnimDuration = 0.5f;

        // Public
        public void Grow()
        {
            enabled = true;
            AnimationManager.Animate((this, null), v => _currentThickness = v, _currentThickness, _Thickness, _AnimDuration);
            _outlineSpriteRenderer.AnimateColor(_Color, _AnimDuration);
        }
        public void Shrink()
        {
            AnimationManager.Animate((this, null), v => _currentThickness = v, _currentThickness, 0f, _AnimDuration, () => enabled = false);
            _outlineSpriteRenderer.AnimateColor(_Color.NewA(0f), _AnimDuration);
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

        // Mono
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
        public override void PlayUpdateLate()
        {
            base.PlayUpdate();
            UpdateMaterialProperties();
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
    }
}