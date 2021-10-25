namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class UIButton : AMousableSprite, IUIHierarchy
    {
        // Inspector
        [Range(0f, 1f)] public float _HighlightDuration = 0.5f;
        [Range(1f, 2f)] public float _HighlightScale = 1.25f;
        [Range(0f, 1f)] public float _ClickDuration = 0.1f;
        [Range(0.5f, 1f)] public float _ClickScale = 0.9f;
        [Range(0.5f, 1f)] public float _ClickColorScale = 0.9f;
        [Range(0f, 1f)] public float _UnusableDuration = 0.5f;
        public Color _UnusableColor = 3.Inv().ToVector4();
        [Range(0f, 1f)] public float _AnimDuration = 0.5f;

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
        private void UpdateUsability()
        {
            Color targetColor = Action.CanBeUsed(UI.Character) ? UI.Character.Color : _UnusableColor;
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
            if (!Action.CanBeUsed(UI.Character))
            {
                if (UI.Character.IsExhausted)
                    UI.NotifyExhausted();
                if (UI.Character.FocusPointsCount < Action._FocusPointsCost)
                    _costPointsBar.NotifyUnfocused();
                return;
            }

            if (Action._IsTargeted)
            {
                UI.StartTargeting(transform, CursorManager.CursorTransform);
                UI.Character.ActionAnimator.AnimateState(Action._Animation.Charge);
                _isTargeting = true;
            }

            transform.AnimateLocalScale(this, transform.localScale * _ClickScale, _ClickDuration);
            _spriteRenderer.AnimateColor(this, _spriteRenderer.color * _ClickColorScale, _ClickDuration);
        }
        public override void MouseHold(CursorManager.Button button, Vector3 location)
        {
            base.MouseHold(button, location);

            if (_isTargeting
            && UI.TryGetCursorCharacter(out var target))
                UI.Character.LookAt(target.transform);
        }
        public override void MouseRelease(CursorManager.Button button, Vector3 location)
        {
            base.MouseRelease(button, location);
            if (!Action.CanBeUsed(UI.Character))
                return;

            if (!Action._IsTargeted)
                Action.Use(UI.Character, null);
            else if (_isTargeting)
            {
                UI.StopTargeting();
                if (UI.TryGetCursorCharacter(out var target))
                {
                    UI.Character.ActionAnimator.AnimateState(Action._Animation.Release);
                    Action.Use(UI.Character, target);
                }
                else
                    UI.Character.ActionAnimator.AnimateState(Action._Animation.Idle);
                _isTargeting = false;
            }

            transform.AnimateLocalScale(this, _originalScale * _HighlightScale, _ClickDuration);
            _spriteRenderer.AnimateColor(this, UI.Character.Color, _ClickDuration);
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

            _spriteRenderer.sprite = Action._Sprite;
            _spriteRenderer.color = UI.Character.Color;
            _originalScale = transform.localScale;

            UpdateUsability();
            UI.Character.OnActionPointsCountChanged += (a, f) => UpdateUsability();
            UI.Character.OnExhaustStateChanged += (s) => UpdateUsability();

            _costPointsBar = this.CreateChildComponent<UICostPointsBar>(UI._PrefabCostPointsBar);
            _costPointsBar.Button = this;
        }
    }
}