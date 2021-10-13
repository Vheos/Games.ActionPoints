namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class UIButton : AMousableSprite, IUIHierarchy
    {
        // Inspector
        public QAnimVector2 _HighlightScaleAnim = new QAnimVector2();
        public QAnimVector2 _AlignMoveAnim = new QAnimVector2();
        public QAnimColor _UsabilityColorAnim = new QAnimColor();

        // Publics
        public UIBase UI
        { get; private set; }
        public Action Action
        { get; set; }
        public void MoveTo(Vector2 targetLocalPosition)
        => _AlignMoveAnim.Start(transform.localPosition.XY(), targetLocalPosition);

        // Privates
        private UICostPointsBar _costPointsBar;
        private Vector2 _originalScale;
        private bool _isTargeting;
        private Character _target;
        private void UpdateUsability(int actionPointsCount, int focusPointsCount)
        {
            if (!isActiveAndEnabled)
                return;

            Color targetColor = Action.CanBeUsed(UI.Character) ? UI.Character._Color : UI._ButtonUnusableColor;
            _UsabilityColorAnim.Start(_spriteRenderer.color, targetColor);
        }

        // Mouse
        override public void MouseGainHighlight()
        {
            base.MouseGainHighlight();
            _HighlightScaleAnim.Start(transform.localScale, _originalScale * UI._ButtonHighlightedScale);
        }
        override public void MouseLoseHighlight()
        {
            base.MouseLoseHighlight();
            _HighlightScaleAnim.Start(transform.localScale, _originalScale);
        }
        public override void MousePress(CursorManager.Button button, Vector3 location)
        {
            base.MousePress(button, location);
            if (Action.IsTargeted)
            {
                UI.StartTargeting(transform, CursorManager.CursorTransform);
                _isTargeting = true;
            }
        }
        public override void MouseHold(CursorManager.Button button, Vector3 location)
        {
            base.MouseHold(button, location);
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
                }
            }
        }
        public override void MouseRelease(CursorManager.Button button, Vector3 location)
        {
            base.MouseRelease(button, location);
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
            UI.Character.OnPointsCountChanged += UpdateUsability;

            _costPointsBar = this.CreateChild<UICostPointsBar>(UI._PrefabCostPointsBar);
            _costPointsBar.Button = this;
        }
        public override void PlayUpdate()
        {
            base.PlayUpdate();
            if (_HighlightScaleAnim.IsActive)
                transform.localScale = _HighlightScaleAnim.Value;
            if (_AlignMoveAnim.IsActive)
                transform.localPosition = _AlignMoveAnim.Value;
            if (_UsabilityColorAnim.IsActive)
                _spriteRenderer.color = _UsabilityColorAnim.Value;
        }
    }
}