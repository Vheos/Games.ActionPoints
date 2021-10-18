namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class UIButton : AMousableSprite, IUIHierarchy
    {
        // Inspector
        [Range(0.1f, 1f)] public float _HighlightDuration = 0.5f;
        [Range(1.0f, 2f)] public float _HighlightScale = 1.25f;
        [Range(0.1f, 1f)] public float _ClickDuration = 0.1f;
        [Range(0.5f, 1f)] public float _ClickScale = 0.9f;
        [Range(0.5f, 1f)] public float _ClickColorScale = 0.9f;
        [Range(0.1f, 1f)] public float _UnusableDuration = 0.5f;
        public Color _UnusableColor = 3.Inv().ToVector4();
        [Range(0f, 1f)] public float _AnimDuration;

        // Publics
        public UIBase UI
        { get; private set; }
        public Action Action
        { get; set; }
        public void MoveTo(Vector2 targetLocalPosition)
        => transform.AnimateLocalPosition(this, targetLocalPosition, _AnimDuration);

        // Privates
        private UICostPointsBar _costPointsBar;
        private Vector2 _originalScale;
        private bool _isTargeting;
        private Character _target;
        private void UpdateUsability(int actionPointsCount, int focusPointsCount)
        {
            if (!isActiveAndEnabled)
                return;

            Color targetColor = Action.CanBeUsed(UI.Character) ? UI.Character._Color : _UnusableColor;
            _spriteRenderer.AnimateColor(this, targetColor, _UnusableDuration);
        }

        // Mouse
        override public void MouseGainHighlight()
        {
            base.MouseGainHighlight();
            if (!Action.CanBeUsed(UI.Character))
                return;

            transform.AnimateLocalScale(this, _originalScale * _HighlightScale, _HighlightDuration);
        }
        public override void MousePress(CursorManager.Button button, Vector3 location)
        {
            base.MousePress(button, location);
            if (UI.Character.IsExhausted)
            {
                UI.NotifyExhausted();
                return;
            }
            else if (UI.Character.FocusPointsCount < Action.FocusPointsCost)
            {
                _costPointsBar.NotifyUnfocused();
                return;
            }
            else if (!Action.CanBeUsed(UI.Character))
                return;

            if (Action.IsTargeted)
            {
                UI.StartTargeting(transform, CursorManager.CursorTransform);
                _isTargeting = true;
            }

            transform.AnimateLocalScale(this, transform.localScale * _ClickScale, _ClickDuration);
            _spriteRenderer.AnimateColor(this, _spriteRenderer.color * _ClickColorScale, _ClickDuration);
        }
        public override void MouseHold(CursorManager.Button button, Vector3 location)
        {
            base.MouseHold(button, location);
            if (!Action.CanBeUsed(UI.Character))
                return;

            if (_isTargeting)
            {
                UI.TryGetTarget(out var newTarget);
                if (_target != null && (newTarget == null || _target != newTarget))
                {
                    _target.LoseTargeting();
                    _target = null;
                }
                if (newTarget != null && (_target == null || _target != newTarget))
                {
                    _target = newTarget;
                    _target.GainTargeting(Action);
                    UI.Character.LookAt(_target.transform);
                }
            }
        }
        public override void MouseRelease(CursorManager.Button button, Vector3 location)
        {
            base.MouseRelease(button, location);
            if (!Action.CanBeUsed(UI.Character))
                return;

            if (_isTargeting)
            {
                UI.StopTargeting();
                if (_target != null)
                {
                    Action.Use(UI.Character, _target);
                    _target.LoseTargeting();
                }
                _target = null;
                _isTargeting = false;
            }


            transform.AnimateLocalScale(this, _originalScale * _HighlightScale, _ClickDuration);
            _spriteRenderer.AnimateColor(this, UI.Character._Color, _ClickDuration);
        }
        override public void MouseLoseHighlight()
        {
            base.MouseLoseHighlight();
            transform.AnimateLocalScale(this, _originalScale, _HighlightDuration);
        }

        // Mono
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            _spriteRenderer.sprite = Action.Sprite;
            _spriteRenderer.color = UI.Character._Color;
            _originalScale = transform.localScale;

            UpdateUsability(UI.Character.ActionPointsCount, UI.Character.FocusPointsCount);
            UI.Character.OnActionPointsCountChanged += UpdateUsability;

            _costPointsBar = this.CreateChild<UICostPointsBar>(UI._PrefabCostPointsBar);
            _costPointsBar.Button = this;
        }
    }
}