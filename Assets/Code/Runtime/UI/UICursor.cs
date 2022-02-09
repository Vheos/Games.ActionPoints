namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;
    using Selectable = Games.Core.Selectable;

    [RequireComponent(typeof(Updatable))]
    [DisallowMultipleComponent]
    public class UICursor : ABaseComponent
    {
        // Inspector
        [field: SerializeField, Range(0f, 2f)] public float Sensitivity { get; private set; } = 1f;
        [field: SerializeField] public ImageProperties Idle { get; private set; } = ImageProperties.Default;
        [field: SerializeField] public ImageProperties Pressed { get; private set; } = ImageProperties.Default;

        // Publics
        public Player Player
        { get; private set; }

        // Privates
        private UICanvas _uiCanvas;
        private Selecter _selecter;
        private void OnInputMoveCursor(Vector2 offset)
        => transform.position = transform.position.Add(offset * Sensitivity / _uiCanvas.ScaleFactor).Clamp(Vector2.zero, _uiCanvas.Size);
        private void OnInputPressConfirm()
        {
            SetImageProperties(Pressed);
            _selecter.TryPress();
        }
        private void OnInputReleaseConfirm()
        {
            SetImageProperties(Idle);

            if (!_selecter.IsSelectingAny)
                return;

            var fullClick = _selecter.Selectable == RaycastableManager.FindClosest<Selectable>(_uiCanvas, this);
            _selecter.TryRelease(fullClick);
        }
        private void OnUpdate()
        {
            if (_selecter.IsHolding)
                return;

            _selecter.Selectable = RaycastableManager.FindClosest<Selectable>(_uiCanvas, this);
        }
        private void SetImageProperties(ImageProperties properties)
        {
            Get<Image>().sprite = properties.Sprite;
            this.NewTween(ConflictResolution.Interrupt)
                .SetDuration(0.1f)
                .LocalScale(properties.Scale.ToVector3())
                .RGB(ColorComponentType.Image, Player.Color * properties.ColorScale);
        }

        // Play
        public void Initialize(UICanvas uiCanvas)
        {
            _uiCanvas = uiCanvas;
            this.BecomeChildOf(_uiCanvas);
            transform.position = _uiCanvas.Size * 0.5f.ToVector2();
            Get<Updatable>().OnUpdate.SubEnableDisable(this, OnUpdate);
        }
        public void BindToPlayer(Player player)
        {
            Player = player;
            _selecter = Player.Get<Selecter>();
            name = $"{Player.name}_Cursor";

            BindDestroyObject(Player);
            Player.OnInputMoveCursor.SubEnableDisable(this, OnInputMoveCursor);
            Player.OnInputPressConfirm.SubEnableDisable(this, OnInputPressConfirm);
            Player.OnInputReleaseConfirm.SubEnableDisable(this, OnInputReleaseConfirm);

            transform.localScale = default;
            SetImageProperties(Idle);
        }
    }
}