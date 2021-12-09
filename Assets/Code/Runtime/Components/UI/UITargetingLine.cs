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
        public AutoEvent<Targetable, Targetable> OnChangeTarget
        { get; } = new AutoEvent<Targetable, Targetable>();

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
            QAnimator.Animate(SetWidth, Settings.StartWidth - Get<LineRenderer>().startWidth, Settings.WidthAnimDuration, this.Interrupt());

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
        public void Hide(bool isInstant = false)
        {
            enabled = false;
            UnsubscribeFrom(OnChangeTarget, _onChangeTarget);
            QAnimator.Animate(SetWidth, 0f - Get<LineRenderer>().startWidth, isInstant ? 0f : Settings.WidthAnimDuration, this.InterruptAndDeactivate());
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
            Get<LineRenderer>().startWidth += width;
            Get<LineRenderer>().endWidth = Get<LineRenderer>().startWidth * Settings.EndWidthRatio;
        }
        private void TryInvokeEvents(Mousable from, Mousable to)
        {
            Targetable fromTargetable = from != null && from.Has<Targetable>() 
                                      ? from.Get<Targetable>() : null;
            Target = to != null && to.Has<Targetable>()
                   ? to.Get<Targetable>() : null;

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