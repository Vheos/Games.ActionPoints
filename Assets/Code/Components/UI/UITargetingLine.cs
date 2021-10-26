namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    [RequireComponent(typeof(LineRenderer))]
    public class UITargetingLine : ABaseComponent, IUIHierarchy
    {
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
            this.Animate(nameof(_lineRenderer.startWidth), SetWidth, _lineRenderer.startWidth, UIManager.Settings._StartWidth, UIManager.Settings._WidthAnimDuration);
        }
        public void Deactivate()
        {
            _isDeactivating = true;
            this.Animate(nameof(_lineRenderer.startWidth), SetWidth, _lineRenderer.startWidth, 0f, UIManager.Settings._WidthAnimDuration, this.GODeactivate);
        }
        public void UpdatePositionsAndTiling()
        {
            _lineRenderer.SetPosition(0, _from.position);
            _lineRenderer.SetPosition(1, _to.position);
            _lineRenderer.sharedMaterial.mainTextureScale = new Vector2(_from.DistanceTo(_to) * UIManager.Settings._Tiling, 1);
        }
        public bool TryGetCursorCharacter(out Character target)
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
        private LineRenderer _lineRenderer;
        private Transform _from;
        private Transform _to;
        private bool _isDeactivating;
        private void SetWidth(float width)
        {
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width * UIManager.Settings._EndWidthRatio;
        }

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.startColor = UI.Character.Color.NewA(UIManager.Settings._StartOpacity);
            this.GODeactivate();
        }
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            OnPlayUpdate += () =>
            {
                if (!_isDeactivating)
                    UpdatePositionsAndTiling();
            };
        }
    }
}