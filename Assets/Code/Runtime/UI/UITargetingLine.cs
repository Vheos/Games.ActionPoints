namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;

    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(UITargetingLineMProps))]
    public class UITargetingLine : ABaseComponent
    {
        // Events
        public AutoEvent<Targetable, Targetable> OnChangeTarget { get; } = new();

        // Publics
        public Player Player
        { get; private set; }
        public void Show(Transform from, bool instantly = false)
        => Show(from, Player.Cursor.transform, instantly);
        public void Show(Transform from, Transform to, bool instantly = false)
        {
            IsActive = true;
            this.NewTween(ConflictResolution.Interrupt)
                .SetDuration(this.Settings().ExpandDuration)
                .AddPropertyModifier(v => Get<LineRenderer>().startWidth += v, this.Settings().StartWidth * _uiCanvas.Size.y - Get<LineRenderer>().startWidth)
                .AddPropertyModifier(v => Get<LineRenderer>().endWidth += v, this.Settings().EndWidth * _uiCanvas.Size.y - Get<LineRenderer>().endWidth)
                .If(instantly).Finish();

            _from = from;
            _to = to;
        }
        public void Hide(bool instantly = false)
        {
            this.NewTween(ConflictResolution.Interrupt)
                .SetDuration(this.Settings().CollapseDuration)
                .AddPropertyModifier(v => Get<LineRenderer>().startWidth += v, 0f - Get<LineRenderer>().startWidth)
                .AddPropertyModifier(v => Get<LineRenderer>().endWidth += v, 0f - Get<LineRenderer>().endWidth)
                .AddEventsOnFinish(() => IsActive = false)
                .If(instantly).Finish();

            _from = null;
            _to = null;
        }

        // Privates
        private UICanvas _uiCanvas;
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
        private void OnUpdate()
        {
            if (_from != null)
                LineFrom = _uiCanvas.CanvasPosition(_from);
            if (_to != null)
                LineTo = _uiCanvas.CanvasPosition(_to);

            Get<UITargetingLineMProps>().TilingX = LineFrom.DistanceTo(LineTo) * this.Settings().Tiling / _uiCanvas.Size.y;
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

            Get<LineRenderer>().startColor = new(1f, 1f, 1f, this.Settings().StartOpacity);
            Get<LineRenderer>().endColor = Player.Color;
        }
    }
}