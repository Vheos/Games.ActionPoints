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
        public QAnimFloat _ThicknessAnim = new QAnimFloat();
        public QAnimColor _ColorAnim = new QAnimColor();

        // Public
        public void Grow()
        {
            enabled = true;
            _ThicknessAnim.Start(_currentThickness, _Thickness);
            _ColorAnim.Start(_outlineSpriteRenderer.color, _Color);
        }
        public void Shrink()
        {
            _ThicknessAnim.Start(_currentThickness, 0f);
            _ColorAnim.Start(_outlineSpriteRenderer.color, _Color.NewA(0f));
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
            UpdateMaterialProperties();
        }
        public override void PlayUpdateLate()
        {
            base.PlayUpdate();

            if (_ThicknessAnim.IsActive)
                _currentThickness = _ThicknessAnim.Value;
            else if (_ThicknessAnim._To == 0)
                enabled = false;

            if (_ColorAnim.IsActive)
                _outlineSpriteRenderer.color = _ColorAnim.Value;

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