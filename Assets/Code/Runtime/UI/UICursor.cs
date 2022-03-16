namespace Vheos.Games.ActionPoints
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
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
            AnimatePress();
            if (!_selecter.isActiveAndEnabled)
                return;

            _selecter.TryPress();
        }
        private void OnInputReleaseConfirm()
        {
            AnimateRelease();
            if (!_selecter.isActiveAndEnabled
            || !_selecter.IsSelectingAny)
                return;

            var fullClick = _selecter.Selectable == RaycastableManager.FindClosest<Selectable>(_uiCanvas, this);
            _selecter.TryRelease(fullClick);
        }
        private void OnUpdate()
        {
            if (!_selecter.isActiveAndEnabled
            || _selecter.IsHolding)
                return;

            _selecter.Selectable = RaycastableManager.FindClosest<Selectable>(_uiCanvas, this);
        }
        private void AnimatePress(bool instantly = false)
        {
            Get<Image>().sprite = this.Settings().PressSprite;
            this.NewTween()
                .SetDuration(this.Settings().PressDuration)
                .LocalScaleRatio(this.Settings().PressScale)
                .RGBRatio(ColorComponent.Image, this.Settings().PressColorScale)
                .FinishIf(instantly);
        }
        private void AnimateRelease(bool instantly = false)
        {
            Get<Image>().sprite = this.Settings().IdleSprite;
            this.NewTween()
                .SetDuration(this.Settings().ReleaseDuration)
                .LocalScaleRatio(this.Settings().PressScale.Inv())
                .RGBRatio(ColorComponent.Image, this.Settings().PressColorScale.Inv())
                .FinishIf(instantly);
        }
        public void MoveTo(ViewSpace viewSpace, Vector3 position, float duration)
        {
            Vector2 canvasPosition = viewSpace switch
            {
                ViewSpace.World => _uiCanvas.WorldToCanvasPosition(position),
                ViewSpace.Canvas => position,
                ViewSpace.Screen => _uiCanvas.ScreenToCanvasPosition(position),
                _ => default,
            };

            this.NewTween(ConflictResolution.Interrupt)
              .SetDuration(duration)
              .Position(canvasPosition)
              .FinishIf(duration <= 0f);
        }

        // Play
        public void Initialize(UICanvas uiCanvas)
        {
            _uiCanvas = uiCanvas;
            this.BecomeChildOf(_uiCanvas);
            transform.position = _uiCanvas.Size * 0.5f.ToVector2();
            Get<Updatable>().OnUpdate.SubEnableDisable(this, OnUpdate);

            AnimateRelease(true);
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

            Get<Image>().color = Player.Color;
        }
    }
}