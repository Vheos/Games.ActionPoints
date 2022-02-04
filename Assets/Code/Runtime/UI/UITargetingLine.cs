namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Vheos.Tools.Extensions.General;
    using Vheos.Tools.Utilities;

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
        public Player Player
        { get; private set; }
        public void Show(Targeter targeter, Transform from)
        => Show(targeter, from, Player.Cursor.transform);
        public void Show(Targeter targeter, Transform from, Transform to)
        {
            IsActive = true;
            this.NewTween(ConflictResolution.Interrupt)
                .SetDuration(_WidthAnimDuration)
                .AddPropertyModifier(AssignWidth, _StartWidth * _uiCanvas.Size.y - Get<LineRenderer>().startWidth);

            _targeter = targeter;
            _from = from;
            _to = to;

            if (_targeter != null)
                _targeter.Targetable = null;
        }
        public void Hide(bool isInstant = false)
        {
            this.NewTween(ConflictResolution.Interrupt)
                .SetDuration(_WidthAnimDuration)
                .AddPropertyModifier(AssignWidth, 0f - Get<LineRenderer>().startWidth)
                .AddOnFinishEvents(() => IsActive = false)
                .FinishIf(isInstant);

            if (_targeter != null)
                _targeter.Targetable = null;

            _targeter = null;
            _from = null;
            _to = null;
        }

        // Privates
        private UICanvas _uiCanvas;
        private Targeter _targeter;
        private Transform _from;
        private Transform _to;
        private Vector3 LineFrom
        {
            get => Get<LineRenderer>().GetPosition(0);
            set => Get<LineRenderer>().SetPosition(0, value);
        }
        private Vector3 LineTo
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
                LineFrom = _uiCanvas.CanvasPosition(_from);
            if (_to != null)
                LineTo = _uiCanvas.CanvasPosition(_to);
            if (_targeter != null)
                _targeter.Targetable = RaycastableManager.FindClosest<Targetable>(_uiCanvas, LineTo);

            Get<UITargetingLineMProps>().TilingX = LineFrom.DistanceTo(LineTo) * _Tiling / _uiCanvas.Size.y;
        }

        // Play
        public void Initialize(UICanvas uicanvas)
        {
            _uiCanvas = uicanvas;

            this.BecomeChildOf(_uiCanvas);
            Get<UITargetingLineMProps>().Initialize();
            Get<LineRenderer>().positionCount = 2;
            Hide(true);

            Get<Updatable>().OnUpdate.SubEnableDisable(this, OnUpdate);
        }
        public void BindToPlayer(Player player)
        {
            Player = player;
            name = $"{player.name}_TargetingLine";
            BindDestroyObject(Player);

            Get<LineRenderer>().startColor = Get<LineRenderer>().startColor.NewA(_StartOpacity);
            Get<LineRenderer>().endColor = Player.Color;
        }
    }
}