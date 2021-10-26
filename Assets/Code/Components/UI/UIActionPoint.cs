namespace Vheos.Games.ActionPoints
{
    using Tools.Extensions.UnityObjects;
    using Tools.Extensions.Math;

    public class UIActionPoint : AUIPoint
    {
        // Publics
        public void UpdateLocalProgresses(float visualActionProgress, float visualFocusProgress)
        {
            ActionProgress = visualActionProgress.Abs().Sub(Index).Clamp01();
            ActionColor = visualActionProgress >= 0 ? Settings.ActionColor : Settings.ExhaustColor;
            FocusProgress = visualFocusProgress.Sub(Index).Clamp01();
        }

        // Privates
        private UIWound _uiWound;
        private void UpdateOpacityOnPointsCountChange(int previousCount, int currentCount)
        {
            previousCount = previousCount.Abs();
            currentCount = currentCount.Abs();
            if (Index >= previousCount && Index < currentCount)
                this.Animate(nameof(Opacity), v => Opacity = v, Opacity, 1f, Settings.AnimDuration);
            if (Index >= currentCount && Index < previousCount)
                this.Animate(nameof(Opacity), v => Opacity = v, Opacity, Settings.PartialProgressOpacity, Settings.AnimDuration);
        }
        private void UpdateWoundVisibility(int previousCount, int currentCount)
        {
            int indexFromLast = UI.Character.RawMaxActionPoints - Index - 1;
            if (indexFromLast >= previousCount && indexFromLast < currentCount)
                _uiWound.Show(Index);
            if (indexFromLast >= currentCount && indexFromLast < previousCount)
                _uiWound.Hide();
        }

        // Play
        public override void PlayAwake()
        {
            base.PlayAwake();
            name = GetType().Name;

            Opacity = Settings.PartialProgressOpacity;
            UI.Character.OnActionPointsCountChanged += UpdateOpacityOnPointsCountChange;

            _uiWound = this.CreateChildComponent<UIWound>(UIManager.Settings.Prefab.Wound);
            UI.Character.OnWoundsCountChanged += UpdateWoundVisibility;
        }
    }
}