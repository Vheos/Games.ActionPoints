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
            Color targetColor = Action.CanBeUsed(Base.Character) ? Base.Character.Color : Settings.UnusableColor;
            GetComponent<SpriteRenderer>().AnimateColor(this, targetColor, Settings.UnusableDuration);
        }
        private void OnGainHighlight()
        {
            if (!Action.CanBeUsed(Base.Character))
                return;

            transform.AnimateLocalScale(this, _originalScale * Settings.HighlightScale, Settings.HighlightDuration);
        }
        private void OnPress(CursorManager.Button button, Vector3 location)
        {
            if (!Action.CanBeUsed(Base.Character))
            {
                if (Base.Character.IsExhausted)
                    Base.PointsBar.NotifyExhausted();

                if (Base.Character.FocusPointsCount < Action.FocusPointsCost)
                    _costPointsBar.NotifyUnfocused();
                return;
            }

            if (!Action.IsInstant)
            {
                Base.TargetingLine.ShowAndFollowCursor(transform);
                if (Action.Animation.TryNonNull(out var animation))
                    Base.Character.ActionAnimator.Animate(animation.Charge);
                _isTargeting = true;
            }

            transform.AnimateLocalScale(this, transform.localScale * Settings.ClickScale, Settings.ClickDuration);
            GetComponent<SpriteRenderer>().AnimateColor(this, GetComponent<SpriteRenderer>().color * Settings.ClickColorScale, Settings.ClickDuration);
        }
        private void OnHold(CursorManager.Button button, Vector3 location)
        {
            if (_isTargeting
            && Base.TargetingLine.TryGetCursorCharacter(out var target))
                Base.Character.LookAt(target.transform);
        }
        private void OnRelease(CursorManager.Button button, Vector3 location)
        {
            if (!Action.CanBeUsed(Base.Character))
                return;

            if (Action.IsInstant)
                Action.Use(Base.Character, null);
            else if (_isTargeting)
            {
                Base.TargetingLine.Hide();
                if (Base.TargetingLine.TryGetCursorCharacter(out var target))
                {
                    if (Action.Animation.TryNonNull(out var animation))
                        Base.Character.ActionAnimator.Animate(animation.Release);
                    Action.Use(Base.Character, target);
                }
                else
                     if (Action.Animation.TryNonNull(out var animation))
                    Base.Character.ActionAnimator.Animate(animation.Cancel);
                _isTargeting = false;
            }

            transform.AnimateLocalScale(this, _originalScale * Settings.HighlightScale, Settings.ClickDuration);
            GetComponent<SpriteRenderer>().AnimateColor(this, Base.Character.Color, Settings.ClickDuration);
        }
        private void OnLoseHighlight()
        {
            transform.AnimateLocalScale(this, _originalScale, Settings.HighlightDuration);
        }

        // Play
        protected override void SubscribeToEvents()
        {
            Mousable mousable = GetComponent<Mousable>();
            SubscribeTo(mousable.OnGainHighlight, OnGainHighlight);
            SubscribeTo(mousable.OnPress, OnPress);
            SubscribeTo(mousable.OnHold, OnHold);
            SubscribeTo(mousable.OnRelease, OnRelease);
            SubscribeTo(mousable.OnLoseHighlight, OnLoseHighlight);
            SubscribeTo(Base.Character.OnActionPointsCountChanged, UpdateUsability);
            SubscribeTo(Base.Character.OnWoundsCountChanged, UpdateUsability);
        }
        protected override void PlayStart()
        {
            base.PlayStart();

            GetComponent<SpriteRenderer>().sprite = Action.Sprite;
            GetComponent<SpriteRenderer>().color = Base.Character.Color;
            _originalScale = transform.localScale;

            UpdateUsability(0, 0);

            _costPointsBar = this.CreateChildComponent<UICostPointsBar>(UIManager.Settings.Prefab.CostPointsBar);
            _costPointsBar.Button = this;
        }

    }
}