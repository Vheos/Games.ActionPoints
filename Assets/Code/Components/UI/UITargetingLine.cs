namespace Vheos.Games.ActionPoints
{
    using System.Collections.Generic;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(LineRenderer))]
    public class UITargetingLine : AUpdatable, IUIHierarchy
    {
        // Inspector
        [Range(0f, 10f)] public float _Tiling = 5f;
        [Range(0f, 1f)] public float _StartOpacity = 0.25f;
        [Range(0f, 1f)] public float _StartWidth = 0.1f;
        [Range(0f, 1f)] public float _EndWidthRatio = 0.25f;
        [Range(0f, 1f)] public float _WidthAnimDuration = 0.5f;

        // Privates
        private LineRenderer _lineRenderer;
        private Transform _from;
        private Transform _to;

        // Publics
        public UIBase UI
        { get; private set; }
        public void Activate(Transform from, Transform to)
        {
            _from = from;
            _to = to;
            _isDeactivating = false;
            UpdatePositionsAndTiling();
            this.GOActivate();
            AnimationManager.Animate((this, null), SetWidth, _lineRenderer.startWidth, _StartWidth, _WidthAnimDuration);
        }
        public void Deactivate()
        {
            _isDeactivating = true;
            AnimationManager.Animate((this, null), SetWidth, _lineRenderer.startWidth, 0f, _WidthAnimDuration, false, this.GODeactivate);
        }

        public void UpdatePositionsAndTiling()
        {
            _lineRenderer.SetPosition(0, _from.position);
            _lineRenderer.SetPosition(1, _to.position);
            _lineRenderer.sharedMaterial.mainTextureScale = new Vector2(_from.DistanceTo(_to) * _Tiling, 1);
        }
        public bool TryGetTarget(out Character target)
        {
            if (CursorManager.CursorMousable.TryNonNull(out var mousable)
            && mousable.TryGetComponent<Character>(out var character))
            {
                target = character;
                return true;
            }
            target = null;
            return false;
        }

        // Privates
        private bool _isDeactivating;
        private void SetWidth(float width)
        {
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width * _EndWidthRatio;
        }

        // Mono
        public override void PlayAwake()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.startColor = UI.Character._Color.NewA(_StartOpacity);
            this.GODeactivate();
        }
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            if (!_isDeactivating)
                UpdatePositionsAndTiling();
        }
    }
}