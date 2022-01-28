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
        [SerializeField] [Range(0f, 2f)] protected float _Sensitivity = 1f;
        [SerializeField] protected ImageProperties _Idle = ImageProperties.Default;
        [SerializeField] protected ImageProperties _Pressed = ImageProperties.Default;

        // Privates
        private Player _player;
        private UICanvas _uiCanvas;
        private Selecter _selecter;
        private void OnInputMoveCursor(Vector2 offset)
        => transform.position = transform.position.Add(offset * _Sensitivity).Clamp(Vector2.zero, _uiCanvas.Size);
        private void OnInputPressConfirm()
        {
            SetImageProperties(_Pressed);
            _selecter.TryPress();
        }
        private void OnInputReleaseConfirm()
        {
            SetImageProperties(_Idle);

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
                .ImageRGB(_player.Color * properties.ColorScale);
        }

        // Play
        public void Initialize(UICanvas uiCanvas)
        {
            _uiCanvas = uiCanvas;
            this.BecomeChildOf(_uiCanvas);
            transform.position = _uiCanvas.Size * 0.5f.ToVector2();
            Get<Updatable>().OnUpdate.SubscribeAuto(this, OnUpdate);
        }
        public void BindToPlayer(Player player)
        {
            _player = player;
            _selecter = _player.Get<Selecter>();

            _player.OnPlayDestroy.SubscribeOneShot(this.DestroyObject);
            _player.OnInputMoveCursor.SubscribeAuto(this, OnInputMoveCursor);
            _player.OnInputPressConfirm.SubscribeAuto(this, OnInputPressConfirm);
            _player.OnInputReleaseConfirm.SubscribeAuto(this, OnInputReleaseConfirm);
            name = $"{_player.name}_Cursor";

            transform.localScale = new();
            SetImageProperties(_Idle);
        }
    }
}