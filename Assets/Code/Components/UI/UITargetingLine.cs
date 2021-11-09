namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;
    using System;

    [RequireComponent(typeof(LineRenderer))]
    public class UITargetingLine : AUIComponent
    {
        // Events
        public event Action<Mousable, Mousable> OnTargetChanged;

        // Publics
        public Mousable Target
        => CursorManager.CursorMousable;
        public void Show(Transform from, Transform to)
        {
            enabled = true;
            this.GOActivate();
            this.Animate(null, SetWidth, GetComponent<LineRenderer>().startWidth, Settings.StartWidth, Settings.WidthAnimDuration);

            _from = from;
            _to = to;
            UpdatePositionsAndTiling();
        }
        public void ShowAndFollowCursor(Transform from)
        {
            CursorManager.SetCursorDistance(from);
            Show(from, CursorManager.CursorTransform);
        }
        public void Hide(bool instantly = false)
        {
            enabled = false;
            this.Animate(null, SetWidth, GetComponent<LineRenderer>().startWidth, 0f, instantly ? 0f : Settings.WidthAnimDuration, this.GODeactivate);
        }
        public void UpdatePositionsAndTiling()
        {
            GetComponent<LineRenderer>().SetPosition(0, _from.position);
            GetComponent<LineRenderer>().SetPosition(1, _to.position);
            _drawable.TilingX = _from.DistanceTo(_to) * Settings.Tiling;
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
        protected TargetingLineDrawable _drawable;
        private UISettings.TargetingLineSettings Settings
        => UIManager.Settings.TargetingLine;
        private void SetWidth(float width)
        {
            GetComponent<LineRenderer>().startWidth = width;
            GetComponent<LineRenderer>().endWidth = width * Settings.EndWidthRatio;
        }
        private void InvokeOnTargetChanged(Mousable from, Mousable to)
        => OnTargetChanged?.Invoke(from, to);

        // Play
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            SubscribeTo(GetComponent<Updatable>().OnUpdated, UpdatePositionsAndTiling);
            SubscribeTo(CursorManager.OnCursorMousableChanged, InvokeOnTargetChanged);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _drawable = GetComponent<TargetingLineDrawable>();
            GetComponent<LineRenderer>().positionCount = 2;
            GetComponent<LineRenderer>().startColor = Base.Character.Color.NewA(Settings.StartOpacity);
            Hide(true);
        }
    }
}