namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;

    public class UIButton : AUIComponent
    {
        // Publics
        public Action Action
        { get; set; }
        public void Initialize(Action action)
        {
            Action = action;
            _costPointsBar = this.CreateChildComponent<UICostPointsBar>(UIManager.Settings.Prefab.CostPointsBar);
            _costPointsBar.Initialize(this);

            Get<SpriteRenderer>().sprite = Action.Sprite;
            Get<SpriteRenderer>().color = Character.Color;
            _originalScale = transform.localScale;

            UpdateUsability();
        }
        public void MoveTo(Vector2 targetLocalPosition)
        => transform.AnimateLocalPosition(this, targetLocalPosition, Settings.AnimDuration);

        // Privates
        private UICostPointsBar _costPointsBar;
        private Vector2 _originalScale;
        private bool _isPressed;
        private UISettings.ButtonSettings Settings
        => UIManager.Settings.Button;
        private void UpdateUsability()
        {
            Color targetColor = Character.Get<Actionable>().CanUse(Action) ? Character.Color : Settings.UnusableColor;
            Get<SpriteRenderer>().AnimateColor(this, targetColor, Settings.UnusableDuration);
        }
        private void OnGainHighlight()
        {
            if (!Character.Get<Actionable>().CanUse(Action))
                return;

            transform.AnimateLocalScale(this, _originalScale * Settings.HighlightScale, Settings.HighlightDuration);
        }
        private void OnPress(UIManager.ButtonFunction function)
        {
            if (!Character.Get<Actionable>().CanUse(Action))
            {
                if (Character.Get<Actionable>().IsExhausted)
                    Base.PointsBar.NotifyExhausted();
                if (Character.Get<Actionable>().FocusPointsCount < Action.FocusPointsCost)
                    _costPointsBar.NotifyUnfocused();
                return;
            }

            if (Action.IsTargeted)
                Base.TargetingLine.ShowAndFollowCursor(transform, OnTargetChanged);

            _isPressed = true;
            Character.Get<ActionAnimator>().AnimateAction(Action, ActionAnimation.Type.Target);
            transform.AnimateLocalScale(this, transform.localScale * Settings.ClickScale, Settings.ClickDuration);
            Get<SpriteRenderer>().AnimateColor(this, Get<SpriteRenderer>().color * Settings.ClickColorScale, Settings.ClickDuration);
        }
        private void OnRelease(UIManager.ButtonFunction function, bool isClick)
        {
            if (!_isPressed)
                return;

            if (Action.IsTargeted)
            {
                if (Character.Get<Targeter>().Target != null)
                {
                    Character.Get<ActionAnimator>().AnimateAction(Action, ActionAnimation.Type.UseThenIdle);
                    Character.Get<Actionable>().Use(Action, Character.Get<Targeter>().Target);
                    Character.Get<Targeter>().Target = null;
                }
                else
                    Character.Get<ActionAnimator>().AnimateAction(Action, ActionAnimation.Type.Idle);

                Character.Get<Targeter>().Target = null;
                Base.TargetingLine.Hide();
            }
            else if (isClick)
                Character.Get<Actionable>().Use(Action, null);

            _isPressed = false;
            transform.AnimateLocalScale(this, _originalScale * Settings.HighlightScale, Settings.ClickDuration);
            Get<SpriteRenderer>().AnimateColor(this, Character.Color, Settings.ClickDuration);
        }
        private void OnLoseHighlight()
        {
            transform.AnimateLocalScale(this, _originalScale, Settings.HighlightDuration);
        }
        private void OnTargetChanged(Targetable from, Targetable to)
        {
            if (to == null)
                Character.Get<Targeter>().Target = null;
            else if (Action.CanTarget(Character.Get<Targeter>(), to))
                Character.Get<Targeter>().Target = to;
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            Selectable selectable = Get<Selectable>();
            SubscribeTo(selectable.OnGainHighlight, OnGainHighlight);
            SubscribeTo(selectable.OnPress, OnPress);
            SubscribeTo(selectable.OnRelease, OnRelease);
            SubscribeTo(selectable.OnLoseHighlight, OnLoseHighlight);
            SubscribeTo(Character.Get<Actionable>().OnChangeActionPointsCount, (from, to) => UpdateUsability());
            SubscribeTo(Character.Get<Actionable>().OnChangeExhausted, (state) => UpdateUsability());
            SubscribeTo(Character.Get<Woundable>().OnChangeWoundsCount, (from, to) => UpdateUsability());
        }
#if CACHED_COMPONENTS
        protected override void DefineCachedComponents()
        {
            base.DefineCachedComponents();
            TryAddToCache<Mousable>();
        }
#endif
    }
}