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
            this.Animate(nameof(_lineRenderer.startWidth), SetWidth, _lineRenderer.startWidth, Settings.StartWidth, Settings.WidthAnimDuration);
        }
        public void Deactivate()
        {
            _isDeactivating = true;
            this.Animate(nameof(_lineRenderer.startWidth), SetWidth, _lineRenderer.startWidth, 0f, Settings.WidthAnimDuration, this.GODeactivate);
        }
        public void UpdatePositionsAndTiling()
        {
            _lineRenderer.SetPosition(0, _from.position);
            _lineRenderer.SetPosition(1, _to.position);
            _lineRenderer.sharedMaterial.mainTextureScale = new Vector2(_from.DistanceTo(_to) * Settings.Tiling, 1);
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
        private UISettings.TargetingLineSettings Settings
        => UIManager.Settings.TargetingLine;
        private void SetWidth(float width)
        {
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width * Settings.EndWidthRatio;
        }

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            _lineRenderer.startColor = UI.Character.Color.NewA(Settings.StartOpacity);
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