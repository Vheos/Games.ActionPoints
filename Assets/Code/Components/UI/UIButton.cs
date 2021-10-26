namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.Extensions.UnityObjects;

    public class UIButton : ABaseComponent, IUIHierarchy
    {
        // CachedComponents
        protected override System.Type[] ComponentsTypesToCache => new[]
        {
           typeof(Mousable),
           typeof(SpriteRenderer),
        };

        // Publics
        public UIBase UI
        { get; private set; }
        public Action Action
        { get; set; }
        public void MoveTo(Vector2 targetLocalPosition)
        => transform.AnimateLocalPosition(this, targetLocalPosition, UIManager.Settings._ButtonAnimDuration);

        // Privates
        private UICostPointsBar _costPointsBar;
        private Vector2 _originalScale;
        private bool _isTargeting;
        private void UpdateUsability()
        {
            Color targetColor = Action.CanBeUsed(UI.Character) ? UI.Character.Color : UIManager.Settings._UnusableColor;
            Get<SpriteRenderer>().AnimateColor(this, targetColor, UIManager.Settings._UnusableDuration);
        }

        // Play
        public override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;
            UI = transform.parent.GetComponent<IUIHierarchy>().UI;

            Get<SpriteRenderer>().sprite = Action.Sprite;
            Get<SpriteRenderer>().color = UI.Character.Color;
            _originalScale = transform.localScale;

            UpdateUsability();
            UI.Character.OnActionPointsCountChanged += (a, f) => UpdateUsability();
            UI.Character.OnExhaustStateChanged += (s) => UpdateUsability();

            _costPointsBar = this.CreateChildComponent<UICostPointsBar>(UIManager.Prefabs.CostPointsBar);
            _costPointsBar.Button = this;
        }
        protected override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            OnMouseGainHighlight += () =>
            {
                if (!Action.CanBeUsed(UI.Character))
                    return;

                transform.AnimateLocalScale(this, _originalScale * UIManager.Settings._HighlightScale, UIManager.Settings._HighlightDuration);
            };
            OnMousePress += (button, position) =>
            {
                if (!Action.CanBeUsed(UI.Character))
                {
                    if (UI.Character.IsExhausted)
                        UI.NotifyExhausted();

                    if (UI.Character.FocusPointsCount < Action.FocusPointsCost)
                        _costPointsBar.NotifyUnfocused();
                    return;
                }

                if (Action.IsTargeted)
                {
                    UI.StartTargeting(transform, CursorManager.CursorTransform);
                    UI.Character.ActionAnimator.AnimateState(Action.Animation.Charge);
                    _isTargeting = true;
                }

                transform.AnimateLocalScale(this, transform.localScale * UIManager.Settings._ClickScale, UIManager.Settings._ClickDuration);
                Get<SpriteRenderer>().AnimateColor(this, Get<SpriteRenderer>().color * UIManager.Settings._ClickColorScale, UIManager.Settings._ClickDuration);
            };
            OnMouseHold += (button, position) =>
            {
                if (_isTargeting
                && UI.TryGetCursorCharacter(out var target))
                    UI.Character.LookAt(target.transform);
            };
            OnMouseRelease += (button, position) =>
            {
                if (!Action.CanBeUsed(UI.Character))
                    return;

                if (!Action.IsTargeted)
                    Action.Use(UI.Character, null);
                else if (_isTargeting)
                {
                    UI.StopTargeting();
                    if (UI.TryGetCursorCharacter(out var target))
                    {
                        UI.Character.ActionAnimator.AnimateStateThenIdle(Action.Animation.Release);
                        Action.Use(UI.Character, target);
                    }
                    else
                        UI.Character.ActionAnimator.AnimateState(UI.Character.Tool.Idle);
                    _isTargeting = false;
                }

                transform.AnimateLocalScale(this, _originalScale * UIManager.Settings._HighlightScale, UIManager.Settings._ClickDuration);
                Get<SpriteRenderer>().AnimateColor(this, UI.Character.Color, UIManager.Settings._ClickDuration);
            };
            OnMouseLoseHighlight += () =>
            {
                transform.AnimateLocalScale(this, _originalScale, UIManager.Settings._HighlightDuration);
            };
        }
    }
}