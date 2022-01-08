namespace Vheos.Games.ActionPoints
{
    using System;
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Vheos.Tools.Extensions.Math;

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

            UpdateUsability();
        }
        public void MoveTo(Vector2 targetLocalPosition)
        => this.NewTween()
            .SetDuration(Settings.AnimDuration)
            .LocalPosition(targetLocalPosition);

        // Privates
        private UICostPointsBar _costPointsBar;
        private bool _isPressed;
        private UISettings.ButtonSettings Settings
        => UIManager.Settings.Button;
        private void UpdateUsability()
        {
            Color targetColor = Character.Get<Actionable>().CanUse(Action) ? Character.Color : Settings.UnusableColor;
            this.NewTween()
                .SetDuration(Settings.UnusableDuration)
                .SpriteColor(targetColor);
        }
        private void OnGainHighlight()
        {
            if (!Character.Get<Actionable>().CanUse(Action))
                return;

            this.NewTween()
                .SetDuration(Settings.HighlightDuration)
                .LocalScaleRatio(Settings.HighlightScale);
        }
        private void OnPress(UIManager.ButtonFunction function)
        {
            if (!Character.Get<Actionable>().CanUse(Action))
            {
                if (Character.Get<Actionable>().IsExhausted
                || Character.Get<Actionable>().UsableActionPoints < Action.ActionPointsCost)
                    Base.PointsBar.NotifyExhausted();
                if (Character.Get<Actionable>().FocusPoints < Action.FocusPointsCost)
                    _costPointsBar.NotifyUnfocused();
                return;
            }

            if (Action.IsTargeted)
                Base.TargetingLine.ShowAndFollowCursor(transform, OnTargetChanged);

            _isPressed = true;
            Character.Get<ActionAnimator>().Animate(Action, ActionAnimationSet.Type.Target);
            this.NewTween()
                .SetDuration(Settings.ClickDuration)
                .LocalScaleRatio(Settings.ClickScale)
                .SpriteColorRatio(Settings.ClickColorScale);
        }
        private void OnRelease(UIManager.ButtonFunction function, bool isClick)
        {
            if (!_isPressed)
                return;

            if (Action.IsTargeted)
            {
                if (Character.Get<Targeter>().Target != null)
                {
                    Character.Get<ActionAnimator>().Animate(Action, ActionAnimationSet.Type.UseThenIdle);
                    Character.Get<Actionable>().Use(Action, Character.Get<Targeter>().Target);
                    Character.Get<Targeter>().Target = null;
                }
                else
                    Character.Get<ActionAnimator>().Animate(Action, ActionAnimationSet.Type.Idle);

                Character.Get<Targeter>().Target = null;
                Base.TargetingLine.Hide();
            }
            else if (isClick)
                Character.Get<Actionable>().Use(Action, null);

            _isPressed = false;
            this.NewTween()
                .SetDuration(Settings.ClickDuration)
                .LocalScaleRatio(Settings.ClickScale.Inv())
                .SpriteColorRatio(Settings.ClickColorScale.Inv());
        }
        private void OnLoseHighlight()
        {
            if (!Character.Get<Actionable>().CanUse(Action))
                return;

            this.NewTween()
                .SetDuration(Settings.HighlightDuration)
                .LocalScaleRatio(Settings.HighlightScale.Inv());
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
            SubscribeTo(Character.Get<Actionable>().OnChangeActionPoints, (from, to) => UpdateUsability());
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