namespace Vheos.Games.ActionPoints
{
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class UIActionPoint : AUIPoint
    {
        // Publics
        public void UpdateLocalProgresses(float visualActionProgress, float visualFocusProgress)
        {
            _drawable.ActionProgress = visualActionProgress.Abs().Sub(Index).Clamp01();
            _drawable.ActionColor = visualActionProgress >= 0 ? Settings.ActionColor : Settings.ExhaustColor;
            _drawable.FocusProgress = visualFocusProgress.Sub(Index).Clamp01();
        }

        // Privates
        private UIWound _uiWound;
        private void UpdateOpacityOnPointsCountChange(int previousCount, int currentCount)
        {
            previousCount = previousCount.Abs();
            currentCount = currentCount.Abs();
            if (Index >= previousCount && Index < currentCount)
                _drawable.AnimateOpacity(1f, Settings.AnimDuration);
            else if (Index >= currentCount && Index < previousCount)
                _drawable.AnimateOpacity(Settings.PartialProgressOpacity, Settings.AnimDuration);
        }
        private void UpdateWoundVisibility(int previousCount, int currentCount)
        {
            int indexFromLast = Character.Get<Actionable>().MaxActionPoints - Index - 1;
            if (indexFromLast >= previousCount && indexFromLast < currentCount)
                _uiWound.Show(Index);
            else if (indexFromLast >= currentCount && indexFromLast < previousCount)
                _uiWound.Hide();
        }

        // Play
        protected override void SubscribeToEvents()
        {
            SubscribeTo(Character.GetHandler<Actionable>().OnActionPointsCountChanged, UpdateOpacityOnPointsCountChange);
            SubscribeTo(Character.GetHandler<Woundable>().OnWoundsCountChanged, UpdateWoundVisibility);
        }
        protected override void PlayStart()
        {
            base.PlayStart();
            name = GetType().Name;

            _drawable.Opacity = Settings.PartialProgressOpacity;
            _uiWound = this.CreateChildComponent<UIWound>(UIManager.Settings.Prefab.Wound);
        }
    }
}