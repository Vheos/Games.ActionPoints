namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.Math;
    using Tools.Extensions.UnityObjects;
    using UnityEngine.UI;
    using Vheos.Tools.Extensions.General;

    [RequireComponent(typeof(Updatable))]
    [DisallowMultipleComponent]
    public class UICursor : ABaseComponent
    {
        // Inspector
        [Header("Visual")]
        [SerializeField] protected ImageProperties _Idle = ImageProperties.Default;
        [SerializeField] protected ImageProperties _Pressed = ImageProperties.Default;
        [Header("Gameplay")]
        [SerializeField] [Range(0f, 2f)] protected float _Sensitivity = 1f;

        // Events
        public AutoEvent<Vector2, Vector2> OnMove
        { get; } = new AutoEvent<Vector2, Vector2>();
        public AutoEvent<UICursorable, UICursorable> OnChangeCursorable
        { get; } = new AutoEvent<UICursorable, UICursorable>();

        // Public
        public Color Color
        { get; private set; }

        // Private
        private Canvas _canvas;
        private UICursorable _cursorable;
        private Vector3 _hitPosition;
        private bool _isPressed;
        private void OnInputMoveCursor(Vector2 offset)
        {
            var newPosition = transform.position.Add(offset * _Sensitivity).Clamp(Vector2.zero, _canvas.renderingDisplaySize);
            if (newPosition == transform.position)
                return;

            var previousPosition = transform.position;
            transform.position = newPosition;
            OnMove.Invoke(previousPosition, newPosition);
        }
        private void OnInputPressConfirm()
        {
            SetImageProperties(_Pressed);
            if (_cursorable != null)
                _cursorable.OnPress.Invoke(_hitPosition);
        }
        private void SetImageProperties(ImageProperties properties)
        {
            Get<Image>().sprite = properties.Sprite;
            Get<Image>().color = Color * properties.ColorScale.ToVector3().Append(1f);
            transform.localScale = properties.Scale.ToVector3();
        }
        private void PerformRaycasts()
        {
            if (CameraManager.TryGetAnyActive(out var camera))
                foreach (var newCursorable in UICursorableManager.ActiveComponents)
                    if (newCursorable.Trigger.Raycast(camera.ScreenPointToRay(Get<RectTransform>().anchoredPosition), out var hitInfo, float.PositiveInfinity)
                    && newCursorable.PerformRaycastTests(hitInfo.point))
                    {
                        _cursorable = newCursorable;
                        _hitPosition = hitInfo.point;
                        return;
                    }

            _cursorable = null;
            _hitPosition = default;
        }
        private void OnUpdate()
        {
            var previousCursorable = _cursorable;
            PerformRaycasts();
            if (_cursorable != previousCursorable)
                OnChangeCursorable.Invoke(previousCursorable, _cursorable);
        }

        // Play
        public void Initialize(Player player, Color color)
        {
            player.OnPlayDestroy.SubscribeAuto(this, this.DestroyObject);
            player.OnInputMoveCursor.SubscribeAuto(this, OnInputMoveCursor);
            player.OnInputPressConfirm.SubscribeAuto(this, OnInputPressConfirm);
            player.OnInputReleaseConfirm.SubscribeAuto(this, () => SetImageProperties(_Idle));
            Get<Updatable>().OnUpdate.SubscribeAuto(this, OnUpdate);
            OnChangeCursorable.SubscribeAuto(this, (from, to) => Debug.Log($"{from?.name}   ->   {to?.name}"));

            Color = color;
            SetImageProperties(_Idle);
            name = $"{player.name}_Cursor";

            _canvas = CanvasManager.Any;
            this.BecomeChildOf(_canvas);
            transform.position = _canvas.renderingDisplaySize / 2f;
        }
    }
}



/* 
        protected override void PlayEnable()
        {
            base.PlayEnable();
            if (!this.ParentHasComponent<Canvas>())
            {
                Debug.LogWarning($"CanvasNotFound: Cursor has been instantiated, but there's no Canvas to attach it to!" +
                    $"\nFallback: disable the Cursor (re-enable it manually when Canvas has been created)");
                IsActive = false;
                return;
            }
        }
 */