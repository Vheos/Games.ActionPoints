namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Vheos.Tools.Extensions.General;
    using Vheos.Tools.UtilityN;

    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(UITargetingLineMProps))]
    public class UITargetingLine : ABaseComponent
    {
        // Inspector
        [SerializeField] [Range(0f, 1f)] protected float _StartOpacity;
        [SerializeField] [Range(0f, 1f)] protected float _StartWidth;
        [SerializeField] [Range(0f, 1f)] protected float _EndWidthRatio;
        [SerializeField] [Range(0f, 1f)] protected float _WidthAnimDuration;
        [SerializeField] [Range(1f, 100f)] protected float _Tiling;

        // Events
        public AutoEvent<Targetable, Targetable> OnChangeTarget { get; } = new();

        // Publics
        public void Show(Targeter targeter, Transform from, Transform to)
        {
            IsActive = true;
            this.NewTween()
                .SetDuration(_WidthAnimDuration)
                .SetConflictResolution(ConflictResolution.Interrupt)
                .AddPropertyModifier(AssignWidth, _StartWidth * _uiCanvas.Size.y - Get<LineRenderer>().startWidth);

            _targeter = targeter;
            _from = from;
            _to = to;
            //OnUpdate();
        }
        public void Hide(bool isInstant = false)
        {
            this.NewTween()
                .SetDuration(_WidthAnimDuration)
                .SetConflictResolution(ConflictResolution.Interrupt)
                .AddPropertyModifier(AssignWidth, 0f - Get<LineRenderer>().startWidth)
                .AddOnFinishEvents(() => IsActive = false)
                .FinishIf(isInstant);

            _targeter = null;
            _from = null;
            _to = null;
        }

        // Privates
        private UICanvas _uiCanvas;
        private Color _color;
        private Transform _from;
        private Transform _to;
        private Targeter _targeter;
        private Vector3 From
        {
            get => Get<LineRenderer>().GetPosition(0);
            set => Get<LineRenderer>().SetPosition(0, value);
        }
        private Vector3 To
        {
            get => Get<LineRenderer>().GetPosition(1);
            set => Get<LineRenderer>().SetPosition(1, value);
        }
        private void AssignWidth(float width)
        {
            Get<LineRenderer>().startWidth += width;
            Get<LineRenderer>().endWidth = Get<LineRenderer>().startWidth * _EndWidthRatio;
        }
        private void OnUpdate()
        {
            if (_from != null)
                From = _uiCanvas.CanvasPosition(_from);
            if (_to != null)
                To = _uiCanvas.CanvasPosition(_to);
            if (_targeter != null)
                _targeter.Targetable = CursorableManager.FindClosest<Targetable>(_uiCanvas, To);

            Get<UITargetingLineMProps>().TilingX = From.DistanceTo(To) * _Tiling / _uiCanvas.Size.y;
        }

        // Play
        public void Initialize(UICanvas uicanvas)
        {
            _uiCanvas = uicanvas;
            this.BecomeChildOf(_uiCanvas);
            Get<LineRenderer>().positionCount = 2;
            Get<Updatable>().OnUpdate.SubscribeAuto(this, OnUpdate);
            Hide(true);
        }
        public void BindToPlayer(Player player, Color color)
        {
            _color = color;

            name = $"{player.name}_TargetingLine";
            Get<LineRenderer>().startColor = Get<LineRenderer>().startColor.NewA(_StartOpacity);
            Get<LineRenderer>().endColor  = _color;
        }

        /*
        public void ShowAndFollowCursor(Transform from, Action<Targetable, Targetable> onChangeTarget = null)
        {
            CursorManager.SetCursorDistance(from);
            Show(from, CursorManager.CursorTransform, onChangeTarget);
        }
        private void TryInvokeEvents(Mousable from, Mousable to)
        {
            Targetable fromTargetable = from != null && from.Has<Targetable>()
                                      ? from.Get<Targetable>() : null;
            Target = to != null && to.Has<Targetable>()
                   ? to.Get<Targetable>() : null;

            if (fromTargetable != Target)
                OnChangeTarget.Invoke(fromTargetable, Target);
        }
        */
    }
}