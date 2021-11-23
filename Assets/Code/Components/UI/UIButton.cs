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
            Color targetColor = Action.CanBeUsedBy(Character.Get<Actionable>()) ? Character.Color : Settings.UnusableColor;
            Get<SpriteRenderer>().AnimateColor(this, targetColor, Settings.UnusableDuration);
        }
        private void OnGainHighlight()
        {
            if (!Action.CanBeUsedBy(Character.Get<Actionable>()))
                return;

            transform.AnimateLocalScale(this, _originalScale * Settings.HighlightScale, Settings.HighlightDuration);
        }
        private void OnPress(UIManager.ButtonFunction function)
        {
            if (!Action.CanBeUsedBy(Character.Get<Actionable>()))
            {
                if (Character.Get<Actionable>().IsExhausted)
                    Base.PointsBar.NotifyExhausted();
                if (Character.Get<Actionable>().FocusPointsCount < Action.FocusPointsCost)
                    _costPointsBar.NotifyUnfocused();
                return;
            }

            if (Action.IsTargeted)
            {
                Base.TargetingLine.ShowAndFollowCursor(transform);
                SubscribeTo(Base.TargetingLine.OnTargetChanged, OnTargetChanged);
            }

            _isPressed = true;
            Action.TryPlayAnimation(Character.Get<ActionAnimator>(), Action.Animation.Charge);
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
                    Action.TryPlayAnimation(Character.Get<ActionAnimator>(), Action.Animation.Release);
                    Action.Use(Character.Get<Actionable>(), Character.Get<Targeter>().Target);
                    Character.Get<Targeter>().Target = null;
                }
                else
                    Action.TryPlayAnimation(Character.Get<ActionAnimator>(), Action.Animation.Idle);

                Character.Get<Targeter>().Target = null;
                Base.TargetingLine.Hide();
                UnsubscribeFrom(Base.TargetingLine.OnTargetChanged, OnTargetChanged);
            }
            else if (isClick)
                Action.Use(Character.Get<Actionable>(), null);

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
            if (to != null
            && Character.IsInCombatWith(to)
            && Action.CanTarget(Character.Get<Targeter>(), to))
                Character.Get<Targeter>().Target = to;
            else
                Character.Get<Targeter>().Target = null;
        }

        // Play
        protected override void DefineAutoSubscriptions()
        {
            Selectable selectable = Get<Selectable>();
            SubscribeTo(selectable.OnGainHighlight, OnGainHighlight);
            SubscribeTo(selectable.OnPress, OnPress);
            SubscribeTo(selectable.OnRelease, OnRelease);
            SubscribeTo(selectable.OnLoseHighlight, OnLoseHighlight);
            SubscribeTo(Character.Get<Actionable>().OnActionPointsCountChanged, (from, to) => UpdateUsability());
            SubscribeTo(Character.Get<Actionable>().OnExhaustStateChanged, (state) => UpdateUsability());
            SubscribeTo(Character.Get<Woundable>().OnWoundsCountChanged, (from, to) => UpdateUsability());
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