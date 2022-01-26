namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using Games.Core;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

    [RequireComponent(typeof(Updatable))]
    [DisallowMultipleComponent]
    public class UICursor : ABaseComponent
    {
        // Inspector
        [SerializeField] [Range(0f, 2f)] protected float _Sensitivity = 1f;
        [SerializeField] protected ImageProperties _Idle = ImageProperties.Default;
        [SerializeField] protected ImageProperties _Pressed = ImageProperties.Default;

        // Publics
        public Player Player
        { get; private set; }

        // Privates
        private UICanvas _uiCanvas;
        private Cursorable _cursorable;
        private void OnInputMoveCursor(Vector2 offset)
        => transform.position = transform.position.Add(offset * _Sensitivity).Clamp(Vector2.zero, _uiCanvas.Size);
        private void OnInputPressConfirm()
        {
            SetImageProperties(_Pressed);
            if (_cursorable != null)
                _cursorable.TryInvokeOnPress(this);
        }
        private void OnInputReleaseConfirm()
        {
            SetImageProperties(_Idle);
            if (_cursorable != null)
            {
                Ray ray = _uiCanvas.GetCameraFor(_cursorable).ScreenPointToRay(_uiCanvas.ScreenPosition(this));
                bool withinTrigger = _cursorable.Trigger.Raycast(ray, out _, float.PositiveInfinity);
                _cursorable.TryInvokeOnRelease(this, withinTrigger);
            }
        }
        private void OnUpdate()
        {
            if (_cursorable != null
            && _cursorable.TryInvokeOnHold(this))
                return;

            var previousCursorable = _cursorable;
            var closestCursorable = CursorableManager.FindClosest<Cursorable>(_uiCanvas, _uiCanvas.CanvasPosition(this));
            _cursorable = closestCursorable.ChooseIf(t => t != null && !t.IsHeld);
            if (_cursorable == previousCursorable)
                return;

            if (previousCursorable != null)
                previousCursorable.TryInvokeOnLoseHighlight(this);
            if (_cursorable != null)
                _cursorable.TryInvokeOnGainHighlight(this);
        }
        private Color _color;
        private void SetImageProperties(ImageProperties properties)
        {
            Get<Image>().sprite = properties.Sprite;
            this.NewTween()
                .SetDuration(0.1f)
                .SetConflictResolution(ConflictResolution.Interrupt)
                .LocalScale(properties.Scale.ToVector3())
                .ImageRGB(_color * properties.ColorScale);
        }

        // Play
        public void Initialize(UICanvas uiCanvas)
        {
            _uiCanvas = uiCanvas;
            this.BecomeChildOf(_uiCanvas);
            transform.position = _uiCanvas.Size * 0.5f.ToVector2();
            Get<Updatable>().OnUpdate.SubscribeAuto(this, OnUpdate);
        }
        public void BindToPlayer(Player player, Color color)
        {
            Player = player;
            _color = color;

            Player.OnPlayDestroy.SubscribeOneShot(this.DestroyObject);
            Player.OnInputMoveCursor.SubscribeAuto(this, OnInputMoveCursor);
            Player.OnInputPressConfirm.SubscribeAuto(this, OnInputPressConfirm);
            Player.OnInputReleaseConfirm.SubscribeAuto(this, OnInputReleaseConfirm);
            name = $"{Player.name}_Cursor";

            transform.localScale = new();
            SetImageProperties(_Idle);
        }
    }
}