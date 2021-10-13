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
        public QAnimFloat _WidthAnim = new QAnimFloat();

        // Privates
        private LineRenderer _lineRenderer;
        private Transform _from;
        private Transform _to;

        // Publics
        public UIBase UI
        { get; private set; }
        public void Activate(Transform from, Transform to)
        {
            this.GOActivate();
            _WidthAnim.Start(_lineRenderer.startWidth, UI._LineStartWidth);
            _from = from;
            _to = to;
            UpdatePositionsAndTiling();
        }
        public void Deactivate()
        {
            _WidthAnim.Start(_lineRenderer.startWidth, 0f);
        }
        public void UpdatePositionsAndTiling()
        {
            _lineRenderer.SetPosition(0, _from.position);
            _lineRenderer.SetPosition(1, _to.position);
            _lineRenderer.sharedMaterial.mainTextureScale = new Vector2(_from.DistanceTo(_to) * UI._LineTiling, 1);
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

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.startColor = UI.Character._Color.NewA(UI._LineStartOpacity);
            this.GODeactivate();
        }
        public override void PlayUpdate()
        {
            base.PlayUpdate();

            if (_WidthAnim.IsActive)
            {
                _lineRenderer.startWidth = _WidthAnim.Value;
                _lineRenderer.endWidth = _WidthAnim.Value * UI._LineEndWidthRatio;
            }
            else if (_WidthAnim._To == 0f)
                this.GODeactivate();

            if (_WidthAnim._To != 0f)
                UpdatePositionsAndTiling();
        }
    }
}