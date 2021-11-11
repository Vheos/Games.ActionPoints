namespace Vheos.Games.ActionPoints
{
    using UnityEngine;
    using Tools.UnityCore;
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.General;

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

            UpdateUsability(0, 0);
        }
        public void MoveTo(Vector2 targetLocalPosition)
        => transform.AnimateLocalPosition(this, targetLocalPosition, Settings.AnimDuration);

        // Privates
        private UICostPointsBar _costPointsBar;
        private Vector2 _originalScale;
        private bool _isTargeting;
        private UISettings.ButtonSettings Settings
        => UIManager.Settings.Button;
        private void UpdateUsability(int from, int to)
        {
            Color targetColor = Action.CanBeUsed(Character.Get<Actionable>()) ? Character.Color : Settings.UnusableColor;
            Get<SpriteRenderer>().AnimateColor(this, targetColor, Settings.UnusableDuration);
        }
        private void OnGainHighlight()
        {
            if (!Action.CanBeUsed(Character.Get<Actionable>()))
                return;

            transform.AnimateLocalScale(this, _originalScale * Settings.HighlightScale, Settings.HighlightDuration);
        }
        private void OnPress(CursorManager.Button button, Vector3 location)
        {
            if (!Action.CanBeUsed(Character.Get<Actionable>()))
            {
                if (Character.Get<Actionable>().IsExhausted)
                    Base.PointsBar.NotifyExhausted();

                if (Character.Get<Actionable>().FocusPointsCount < Action.FocusPointsCost)
                    _costPointsBar.NotifyUnfocused();
                return;
            }

            if (!Action.IsInstant)
            {
                Base.TargetingLine.ShowAndFollowCursor(transform);
                if (Action.Animation.TryNonNull(out var animation))
                    Character.ActionAnimator.Animate(animation.Charge);
                _isTargeting = true;
            }

            transform.AnimateLocalScale(this, transform.localScale * Settings.ClickScale, Settings.ClickDuration);
            Get<SpriteRenderer>().AnimateColor(this, Get<SpriteRenderer>().color * Settings.ClickColorScale, Settings.ClickDuration);
        }
        private void OnHold(CursorManager.Button button, Vector3 location)
        {
            if (_isTargeting
            && Base.TargetingLine.TryGetCursorCharacter(out var target))
                Character.LookAt(target.transform);
        }
        private void OnRelease(CursorManager.Button button, Vector3 location)
        {
            if (!Action.CanBeUsed(Character.Get<Actionable>()))
                return;

            if (Action.IsInstant)
                Action.Use(Character.Get<Actionable>(), null);
            else if (_isTargeting)
            {
                Base.TargetingLine.Hide();
                if (Base.TargetingLine.TryGetCursorCharacter(out var target))
                {
                    if (Action.Animation.TryNonNull(out var animation))
                        Character.ActionAnimator.Animate(animation.Release);
                    Action.Use(Character.Get<Actionable>(), target);
                }
                else
                     if (Action.Animation.TryNonNull(out var animation))
                    Character.ActionAnimator.Animate(animation.Cancel);
                _isTargeting = false;
            }

            transform.AnimateLocalScale(this, _originalScale * Settings.HighlightScale, Settings.ClickDuration);
            Get<SpriteRenderer>().AnimateColor(this, Character.Color, Settings.ClickDuration);
        }
        private void OnLoseHighlight()
        {
            transform.AnimateLocalScale(this, _originalScale, Settings.HighlightDuration);
        }

        // Play
        protected override void SubscribeToEvents()
        {
            Mousable mousable = Get<Mousable>();
            SubscribeTo(mousable.OnGainHighlight, OnGainHighlight);
            SubscribeTo(mousable.OnPress, OnPress);
            SubscribeTo(mousable.OnHold, OnHold);
            SubscribeTo(mousable.OnRelease, OnRelease);
            SubscribeTo(mousable.OnLoseHighlight, OnLoseHighlight);
            SubscribeTo(Character.Get<Actionable>().OnActionPointsCountChanged, UpdateUsability);
            SubscribeTo(Character.Get<Woundable>().OnWoundsCountChanged, UpdateUsability);
        }
    }
}