namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;
    using System;

    [RequireComponent(typeof(LineRenderer))]
    public class UITargetingLine : ACustomDrawable, IUIHierarchy
    {
        // Events
        public event Action<Mousable, Mousable> OnTargetChanged;

        // Publics
        public UIBase UI
        { get; private set; }
        public float TilingX
        {
            get => GetFloat(nameof(TilingX));
            set => SetFloat(nameof(TilingX), value);
        }
        public Mousable Target
        => CursorManager.CursorMousable;
        public void Show(Transform from, Transform to)
        {
            _from = from;
            _to = to;
            _isDeactivating = false;
            UpdatePositionsAndTiling();
            CursorManager.OnCursorMousableChanged += InvokeOnTargetChanged;
            this.GOActivate();
            this.Animate(null, SetWidth, Get<LineRenderer>().startWidth, Settings.StartWidth, Settings.WidthAnimDuration);
        }
        public void ShowAndFollowCursor(Transform from)
        {
            CursorManager.SetCursorDistance(from);
            Show(from, CursorManager.CursorTransform);
        }
        public void Hide()
        {
            _isDeactivating = true;
            CursorManager.OnCursorMousableChanged -= InvokeOnTargetChanged;
            this.Animate(null, SetWidth, Get<LineRenderer>().startWidth, 0f, Settings.WidthAnimDuration, this.GODeactivate);
        }
        public void UpdatePositionsAndTiling()
        {
            Get<LineRenderer>().SetPosition(0, _from.position);
            Get<LineRenderer>().SetPosition(1, _to.position);
            TilingX = _from.DistanceTo(_to) * Settings.Tiling;
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
        private Transform _from;
        private Transform _to;
        private bool _isDeactivating;
        private UISettings.TargetingLineSettings Settings
        => UIManager.Settings.TargetingLine;
        private void SetWidth(float width)
        {
            Get<LineRenderer>().startWidth = width;
            Get<LineRenderer>().endWidth = width * Settings.EndWidthRatio;
        }
        private void InvokeOnTargetChanged(Mousable from, Mousable to)
        => OnTargetChanged?.Invoke(from, to);

        // Play
        protected override void AddToComponentCache()
        {
            base.AddToComponentCache();
            AddToCache<LineRenderer>();
        }
        public override void PlayAwake()
        {
            base.PlayAwake();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            Get<LineRenderer>().positionCount = 2;
            Get<LineRenderer>().startColor = UI.Character.Color.NewA(Settings.StartOpacity);
            this.GODeactivate();
        }
        protected override void SubscribeToPlayEvents()
        {
            base.SubscribeToPlayEvents();
            Updatable.OnPlayUpdate += () =>
            {
                if (!_isDeactivating)
                    UpdatePositionsAndTiling();
            };
        }
    }
}