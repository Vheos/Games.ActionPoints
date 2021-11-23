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
        public Event<Targetable, Targetable> OnChangeTarget
        { get; } = new Event<Targetable, Targetable>();

        // Publics
        public Targetable Target
        { get; private set; }
        public void Initialize()
        {
            _drawable = Get<TargetingLineDrawable>();
            Get<LineRenderer>().positionCount = 2;
            Get<LineRenderer>().startColor = Character.Color.NewA(Settings.StartOpacity);
            Hide(true);
        }
        public void Show(Transform from, Transform to, Action<Targetable, Targetable> onChangeTarget = null)
        {
            enabled = true;
            this.GOActivate();
            this.Animate(null, SetWidth, Get<LineRenderer>().startWidth, Settings.StartWidth, Settings.WidthAnimDuration);

            _from = from;
            _to = to;
            UpdatePositionsAndTiling();
            
            SubscribeTo(OnChangeTarget, onChangeTarget);
            _onChangeTarget = onChangeTarget;
        }
        public void ShowAndFollowCursor(Transform from, Action<Targetable, Targetable> onChangeTarget = null)
        {
            CursorManager.SetCursorDistance(from);
            Show(from, CursorManager.CursorTransform, onChangeTarget);
        }
        public void Hide(bool instantly = false)
        {
            enabled = false;
            UnsubscribeFrom(OnChangeTarget, _onChangeTarget);
            this.Animate(null, SetWidth, Get<LineRenderer>().startWidth, 0f, instantly ? 0f : Settings.WidthAnimDuration, this.GODeactivate);
        }
        public void UpdatePositionsAndTiling()
        {
            Get<LineRenderer>().SetPosition(0, _from.position);
            Get<LineRenderer>().SetPosition(1, _to.position);
            _drawable.TilingX = _from.DistanceTo(_to) * Settings.Tiling;
        }

        // Privates
        private Transform _from;
        private Transform _to;
        private Action<Targetable, Targetable> _onChangeTarget;
        protected TargetingLineDrawable _drawable;
        private UISettings.TargetingLineSettings Settings
        => UIManager.Settings.TargetingLine;
        private void SetWidth(float width)
        {
            Get<LineRenderer>().startWidth = width;
            Get<LineRenderer>().endWidth = width * Settings.EndWidthRatio;
        }
        private void TryInvokeEvents(Mousable from, Mousable to)
        {
            Targetable fromTargetable = from == null ? null : from.GetOrNull<Targetable>();
            Target = to == null ? null : to.GetOrNull<Targetable>();
            if (fromTargetable != Target)
                OnChangeTarget?.Invoke(fromTargetable, Target);
        }
        // Play        
        protected override void DefineAutoSubscriptions()
        {
            base.DefineAutoSubscriptions();
            SubscribeTo(Get<Updatable>().OnUpdate, UpdatePositionsAndTiling);
            SubscribeTo(CursorManager.OnChangeCursorMousable, TryInvokeEvents);
        }
    }
}