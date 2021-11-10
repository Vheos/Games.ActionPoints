namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;    

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
            this.Animate(null, SetWidth, Get<LineRenderer>().startWidth, Settings.StartWidth, Settings.WidthAnimDuration);

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
            this.Animate(null, SetWidth, Get<LineRenderer>().startWidth, 0f, instantly ? 0f : Settings.WidthAnimDuration, this.GODeactivate);
        }
        public void UpdatePositionsAndTiling()
        {
            Get<LineRenderer>().SetPosition(0, _from.position);
            Get<LineRenderer>().SetPosition(1, _to.position);
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
            Get<LineRenderer>().startWidth = width;
            Get<LineRenderer>().endWidth = width * Settings.EndWidthRatio;
        }
        private void InvokeOnTargetChanged(Mousable from, Mousable to)
        => OnTargetChanged?.Invoke(from, to);

        // Play
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            SubscribeTo(GetHandler<Updatable>().OnUpdated, UpdatePositionsAndTiling);
            SubscribeTo(CursorManager.OnCursorMousableChanged, InvokeOnTargetChanged);
        }
        protected override void PlayAwake()
        {
            base.PlayAwake();
            _drawable = Get<TargetingLineDrawable>();
        }
        protected override void PlayStart()
        {
            base.PlayStart();
            Get<LineRenderer>().positionCount = 2;
            Get<LineRenderer>().startColor = Character.Color.NewA(Settings.StartOpacity);
            Hide(true);
        }
    }
}